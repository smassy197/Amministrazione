Imports System.Data.SQLite
Imports System.IO
Imports System.Net.Http
Imports System.Reflection
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel
Imports DocumentFormat.OpenXml.Wordprocessing
'Imports FirebaseAdmin
'Imports FirebaseAdmin.Auth


Public Class Form1
    Private txtPassword As New TextBox()
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Me.Size = New Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height)
        ' Me.Location = New Point(0, 0)

        ' Crea il database e la tabella se non esistono
        CreateDatabaseAndTable()

        ' Imposta il percorso del file di configurazione in AppData/Local/Amministrazione
        Dim appDataFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
        Directory.CreateDirectory(appDataFolder) ' Crea la cartella se non esiste
        Dim configFilePath As String = Path.Combine(appDataFolder, "userconfig.txt")
        Dim userName As String = Environment.UserName
        InitializeApp(userName)
        SecureApplication()

        ' Aggiungi l'handler per l'evento DateSelected di MonthCalendar1
        AddHandler MonthCalendar1.DateSelected, AddressOf MonthCalendar1_DateSelected

        ' Aggiungi l'handler per l'evento Click di btnResetSearch
        AddHandler btnResetSearch.Click, AddressOf btnResetSearch_Click

        ' Aggiungi l'handler per l'evento DateSelected di MonthCalendar1
        AddHandler MonthCalendar1.DateSelected, AddressOf MonthCalendar1_DateSelected
        ' Se il file di configurazione non esiste, chiedi all'utente di inserire il nome
        ' If Not File.Exists(configFilePath) Then
        'userName = InputBox("Inserisci il tuo nome:", "Prima Installazione", Environment.UserName)
        'File.WriteAllText(configFilePath, userName)
        'Else
        ' userName = File.ReadAllText(configFilePath).Trim()
        'End If



        ' Carica dati iniziali
        LoadData()
        LoadDataIntoComboboxes()
        HighlightDatesWithDocuments()
        MonthCalendar1.UpdateBoldedDates()

        Dim contiList As List(Of String) = GetUniqueContiFromMovimenti()

        ' Imposta il pulsante btnShowAll come invisibile all'avvio
        btnShowAll.Visible = False

        For Each conto In contiList
            cmbContoType.Items.Add(conto)
        Next

        UpdateLabels()
        cmbConto.Text = ""
        cmbDescrizione.Text = ""

        ' Aggiunta delle colonne "Elimina" e "Modifica" alla DataGridView
        Dim deleteColumn As New DataGridViewButtonColumn()
        deleteColumn.Name = "Delete"
        deleteColumn.Text = "Elimina"
        deleteColumn.UseColumnTextForButtonValue = True
        DataGridView1.Columns.Add(deleteColumn)

        Dim editColumn As New DataGridViewButtonColumn()
        editColumn.Name = "Edit"
        editColumn.Text = "Modifica"
        editColumn.UseColumnTextForButtonValue = True
        DataGridView1.Columns.Add(editColumn)

        ' Aggiungi "Adebito" e "Acredito" a cmbContoType
        cmbContoType.Items.Add("Adebito")
        cmbContoType.Items.Add("Acredito")
        ' Aggiungi il conto "Saldo" alla ComboBox
        cmbContoType.Items.Add("Saldo")
        'cmbContoType.Items.Add("Saldo Progressivo")





    End Sub

    Private Async Function GetLatestVersionFromGitHubAsync() As Task(Of String)
        Dim versionUrl As String = "https://raw.githubusercontent.com/smassy197/Amministrazione/main/version.txt"
        Try
            Using client As New HttpClient()
                Dim response As HttpResponseMessage = Await client.GetAsync(versionUrl)
                If response.IsSuccessStatusCode Then
                    Return (Await response.Content.ReadAsStringAsync()).Trim()
                End If
            End Using
        Catch ex As Exception
            ' Gestisci eventuali errori di rete
        End Try
        Return ""
    End Function


    Private Function GetCurrentAppVersion() As String
        Return Application.ProductVersion ' oppure My.Application.Info.Version.ToString()
    End Function

    Private Async Sub CheckForUpdate()
        Dim latestVersion As String = Await GetLatestVersionFromGitHubAsync()
        Dim currentVersion As String = GetCurrentAppVersion()

        If Not String.IsNullOrEmpty(latestVersion) AndAlso latestVersion <> currentVersion Then
            MessageBox.Show($"È disponibile una nuova versione: {latestVersion}.", "Aggiornamento disponibile", MessageBoxButtons.OK, MessageBoxIcon.Information)
            ' Qui puoi aggiungere il link per il download o l’aggiornamento automatico
        End If
    End Sub
    Private Sub InitializeApp(userName As String)

        ' Imposta il titolo della finestra con il nome dell'utente
        Me.Text = $"Amministrazione di {userName}"

    End Sub


    'secure application-----------------------------------------------------------
    Private Sub SecureApplication()
        Dim directoryPath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
        Dim configFilePath As String = Path.Combine(directoryPath, "userconfig.txt")
        Dim userName As String = ""
        ' Se il file di configurazione non esiste, richiedi il nome utente e la password e salvali nel file
        If Not File.Exists(configFilePath) Then
            userName = InputBox("Inserisci il tuo nome:", "Prima Installazione", Environment.UserName)
            'Dim password As String = InputBox("Inserisci la password:", "Prima Installazione", "")

            ' Nel metodo Form1_Load, sostituisci l'uso di InputBox con la tua TextBox
            Dim password As String = GetPassword()

            ' Salva il nome utente e la password nel file di configurazione
            SaveConfiguration(configFilePath, userName, password)

            ' Procedi con l'inizializzazione dell'app
            InitializeApp(userName)
        Else
            ' Se il file di configurazione esiste, verifica la password
            If File.Exists(configFilePath) Then

                ' Leggi il nome utente dal file di configurazione
                userName = ReadUserNameFromConfig(configFilePath)

                CheckPassword(configFilePath)
                ' La password è corretta, procedi con l'inizializzazione dell'app
                InitializeApp(userName)


            Else
                ' La password non è corretta o il nome utente non è stato letto correttamente, chiudi l'applicazione
                MessageBox.Show("Password non corretta. Chiusura dell'applicazione.")
                Me.Close()
            End If

        End If

    End Sub
    ' Funzione per ottenere la password tramite la TextBox
    Private Function GetPassword() As String
        Dim passwordForm As New Form()

        ' Imposta le proprietà del form della password
        passwordForm.Text = "Inserisci la password"
        passwordForm.FormBorderStyle = FormBorderStyle.FixedDialog
        passwordForm.MaximizeBox = False
        passwordForm.MinimizeBox = False
        passwordForm.StartPosition = FormStartPosition.CenterScreen

        ' Imposta la TextBox della password
        txtPassword.PasswordChar = "*"
        txtPassword.Width = 250
        txtPassword.Location = New Point(10, 10)
        passwordForm.Controls.Add(txtPassword)

        ' Aggiungi un pulsante "OK" per confermare la password
        Dim btnOK As New Button()
        btnOK.Text = "OK"
        btnOK.DialogResult = DialogResult.OK
        btnOK.Size = New Size(100, 30)
        btnOK.Location = New Point(10, 60)
        passwordForm.Controls.Add(btnOK)
        ' Imposta il pulsante "OK" come pulsante predefinito del form
        passwordForm.AcceptButton = btnOK

        ' Aggiungi un pulsante "Annulla" per chiudere la finestra senza confermare la password
        Dim btnCancel As New Button()
        btnCancel.Text = "Annulla"
        btnCancel.DialogResult = DialogResult.Cancel
        btnCancel.Size = New Size(100, 30)
        btnCancel.Location = New Point(120, 60)
        passwordForm.Controls.Add(btnCancel)

        ' Imposta le dimensioni della finestra
        passwordForm.ClientSize = New Size(300, 150)

        ' Mostra il form e restituisci la password
        If passwordForm.ShowDialog() = DialogResult.OK Then
            Return txtPassword.Text
        Else
            Return ""
        End If
    End Function
    Private Function CheckPassword(configFilePath As String) As Boolean
        Try
            ' Leggi il contenuto del file di configurazione
            Dim lines() As String = File.ReadAllLines(configFilePath)

            ' Verifica se il file contiene almeno due righe (username e password)
            If lines.Length >= 2 Then
                ' Estrai la password dalla seconda riga
                Dim storedPasswordLine As String = lines(1)
                Dim storedPassword As String = storedPasswordLine.Substring("password: ".Length).Trim()

                ' Chiedi all'utente di inserire la password
                ' Dim enteredPassword As String = InputBox("Inserisci la password:", "Password", "")
                Dim enteredPassword As String = GetPassword()


                ' Confronta la password inserita con quella salvata
                If enteredPassword = storedPassword Then
                    ' La password è corretta
                    Return True
                Else
                    ' La password non è corretta
                    MessageBox.Show("Password non corretta. Chiusura dell'applicazione.")
                    Me.Close()
                    Return False
                End If
            Else
                ' Il file di configurazione non contiene le informazioni necessarie
                MessageBox.Show("Il file di configurazione non è valido. Chiusura dell'applicazione.")
                Me.Close()
                Return False
            End If
        Catch ex As Exception
            ' Gestisci eventuali eccezioni durante la lettura del file
            MessageBox.Show($"Errore durante la lettura del file di configurazione: {ex.Message}. Chiusura dell'applicazione.")
            Me.Close()
            Return False
        End Try
    End Function
    Private Function ReadUserNameFromConfig(configFilePath As String) As String
        Try
            ' Leggi il contenuto del file di configurazione
            Dim lines() As String = File.ReadAllLines(configFilePath)

            ' Verifica se il file contiene almeno una riga
            If lines.Length >= 1 Then
                ' Estrai il nome utente dalla prima riga
                Dim userNameLine As String = lines(0)
                Return userNameLine.Substring("username: ".Length).Trim()
            Else
                ' Il file di configurazione non contiene le informazioni necessarie
                MessageBox.Show("Il file di configurazione non è valido. Chiusura dell'applicazione.")
                Me.Close()
                Return ""
            End If
        Catch ex As Exception
            ' Gestisci eventuali eccezioni durante la lettura del file
            MessageBox.Show($"Errore durante la lettura del file di configurazione: {ex.Message}. Chiusura dell'applicazione.")
            Me.Close()
            Return ""
        End Try
    End Function

    ' Salva il nome utente e la password nel file di configurazione
    Private Sub SaveConfiguration(configFilePath As String, userName As String, password As String)
        ' Salva il nome utente e la password nel file di configurazione
        File.WriteAllText(configFilePath, $"username: {userName}{Environment.NewLine}password: {password}")
    End Sub


    '------------------------------------------------------------------------
    Private Sub MonthCalendar1_DateSelected(sender As Object, e As DateRangeEventArgs)
        ' Quando una nuova data viene selezionata, carica i dati relativi a quella data nella DataGridView
        LoadDataByDate(e.Start, e.End)
    End Sub

    Private Sub LoadDataByDate(startDate As Date, endDate As Date)
        Dim databasePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath & ";Version=3;"

        Using conn As New SQLiteConnection(connString)
            conn.Open()

            Using da As New SQLiteDataAdapter("SELECT * FROM Movimenti WHERE Data BETWEEN @startDate AND @endDate", conn)
                da.SelectCommand.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                da.SelectCommand.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))

                Dim dt As New DataTable
                da.Fill(dt)

                DataGridView1.DataSource = dt
            End Using
        End Using
    End Sub

    Private Sub btnResetSearch_Click(sender As Object, e As EventArgs)
        ' Quando il pulsante di reset viene cliccato, carica nuovamente tutti i dati nella DataGridView
        LoadData()

        ' Pulisci la selezione nel MonthCalendar1
        MonthCalendar1.SelectionStart = Date.Today
        MonthCalendar1.SelectionEnd = Date.Today

        ' Nascondi nuovamente il pulsante btnShowAll
        btnShowAll.Visible = False
    End Sub

    Private Sub CreateDatabaseAndTable()
        Dim databaseFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
        Dim databasePath As String = Path.Combine(databaseFolder, "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath & ";Version=3;"

        ' Verifica se la cartella "Amministrazione" esiste, altrimenti visualizza un messaggio e creala
        If Not Directory.Exists(databaseFolder) Then
            MessageBox.Show("La cartella 'Amministrazione' non esiste. Verrà creata.")
            Directory.CreateDirectory(databaseFolder)
        End If

        ' Verifica se il file del database esiste, altrimenti visualizza un messaggio e crealo
        If Not File.Exists(databasePath) Then
            MessageBox.Show("Il file del database non esiste. Verrà creato.")
            SQLiteConnection.CreateFile(databasePath)
        End If

        Using conn As New SQLiteConnection(connString)
            conn.Open()

            ' Creazione della tabella Movimenti
            Using cmd As New SQLiteCommand("CREATE TABLE IF NOT EXISTS Movimenti (ID INTEGER PRIMARY KEY AUTOINCREMENT, Data TEXT, Conto TEXT, Descrizione TEXT, Adebito REAL, Acredito REAL, Nota TEXT)", conn)
                cmd.ExecuteNonQuery()
            End Using

            ' Creazione della tabella Descrizioni
            Using cmd As New SQLiteCommand("CREATE TABLE IF NOT EXISTS Descrizioni (ID INTEGER PRIMARY KEY AUTOINCREMENT, Descrizione TEXT UNIQUE)", conn)
                cmd.ExecuteNonQuery()
            End Using

            ' Creazione della tabella Conti
            Using cmd As New SQLiteCommand("CREATE TABLE IF NOT EXISTS Conti (ID INTEGER PRIMARY KEY AUTOINCREMENT, Conto TEXT UNIQUE)", conn)
                cmd.ExecuteNonQuery()
            End Using

        End Using
    End Sub


    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Dim databasePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath & ";Version=3;"


        Using conn As New SQLiteConnection(connString)
            conn.Open()

            ' Insert into Movimenti
            Using cmd As New SQLiteCommand(conn)
                cmd.CommandText = "INSERT INTO Movimenti (Data, Conto, Descrizione, Adebito, Acredito, Nota) VALUES (@data, @conto, @descrizione, @addebito, @acredito, @nota)"
                cmd.Parameters.AddWithValue("@data", MonthCalendar1.SelectionStart.ToString("yyyy-MM-dd"))

                cmd.Parameters.AddWithValue("@conto", cmbConto.Text)
                cmd.Parameters.AddWithValue("@descrizione", cmbDescrizione.Text)
                cmd.Parameters.AddWithValue("@addebito", txtAdebito.Text)
                cmd.Parameters.AddWithValue("@acredito", txtAcredito.Text)
                cmd.Parameters.AddWithValue("@nota", RichTextBoxNota.Text)
                cmd.ExecuteNonQuery()
            End Using

            ' Insert unique Descrizione if it doesn't exist
            Using cmd As New SQLiteCommand(conn)
                cmd.CommandText = "INSERT OR IGNORE INTO Descrizioni (Descrizione) VALUES (@descrizione)"
                cmd.Parameters.AddWithValue("@descrizione", cmbDescrizione.Text)
                cmd.ExecuteNonQuery()
            End Using

            ' Insert unique Conto if it doesn't exist
            Using cmd As New SQLiteCommand(conn)
                cmd.CommandText = "INSERT OR IGNORE INTO Conti (Conto) VALUES (@conto)"
                cmd.Parameters.AddWithValue("@conto", cmbConto.Text)
                cmd.ExecuteNonQuery()
            End Using
        End Using

        MessageBox.Show("Dati salvati con successo!")
        LoadData()
        UpdateLabels()
        LoadDataIntoComboboxes()
        HighlightDatesWithDocuments()
        txtAcredito.Text = ""
        txtAdebito.Text = ""
        cmbConto.Text = ""
        cmbDescrizione.Text = ""
        RichTextBoxNota.Text = ""
    End Sub


    Private Sub LoadData()
        Dim databasePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath & ";Version=3;"

        ' Verifica se il database esiste prima di tentare di aprirlo
        If File.Exists(databasePath) Then
            Dim dt As New DataTable

            Using conn As New SQLiteConnection(connString)
                conn.Open()

                Using cmd As New SQLiteCommand("SELECT * FROM Movimenti", conn)
                    Using reader As SQLiteDataReader = cmd.ExecuteReader()
                        dt.Load(reader)
                    End Using
                End Using
            End Using

            DataGridView1.DataSource = dt
        Else
            MessageBox.Show("Il database non esiste. Assicurati di aver creato il database e le tabelle prima di caricare i dati.")
        End If
    End Sub



    Private Sub LoadDataIntoComboboxes()
        Dim databaseFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
        Dim databasePath As String = Path.Combine(databaseFolder, "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath & ";Version=3;"


        ' Carica dati per ComboBox Descrizione
        Using connDesc As New SQLiteConnection(connString)
            connDesc.Open()
            Using daDesc As New SQLiteDataAdapter("SELECT Descrizione FROM Descrizioni ORDER BY Descrizione ASC", connDesc)
                Dim dtDesc As New DataTable
                daDesc.Fill(dtDesc)
                cmbDescrizione.DataSource = dtDesc
                cmbDescrizione.DisplayMember = "Descrizione"
            End Using
        End Using

        ' Carica dati per ComboBox Conto
        Using connConto As New SQLiteConnection(connString)
            connConto.Open()
            Using daConto As New SQLiteDataAdapter("SELECT Conto FROM Conti ORDER BY Conto ASC", connConto)
                Dim dtConto As New DataTable
                daConto.Fill(dtConto)
                cmbConto.DataSource = dtConto
                cmbConto.DisplayMember = "Conto"
            End Using
        End Using
    End Sub


    'parte dedicata ai totali e alla ricerca
    Private Function GetUniqueContiFromMovimenti() As List(Of String)
        Dim conti As New List(Of String)
        Dim databaseFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
        Dim databasePath As String = Path.Combine(databaseFolder, "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath & ";Version=3;"


        Using conn As New SQLiteConnection(connString)
            conn.Open()

            Using cmd As New SQLiteCommand(conn)
                cmd.CommandText = "SELECT DISTINCT Conto FROM Movimenti ORDER BY Conto"

                Using reader As SQLiteDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        conti.Add(reader("Conto").ToString())
                    End While
                End Using
            End Using
        End Using

        Return conti
    End Function
    Private Function GetTotalForDateRangeAndConto(startDate As Date, endDate As Date, contoType As String) As Double
        Dim total As Double = 0
        Dim databasePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath & ";Version=3;"


        Using conn As New SQLiteConnection(connString)
            conn.Open()

            Using cmd As New SQLiteCommand(conn)
                If contoType = "Stipendi" Then
                    cmd.CommandText = "SELECT SUM(Acredito) FROM Movimenti WHERE Conto = @contoType AND Data BETWEEN @startDate AND @endDate"
                Else
                    cmd.CommandText = "SELECT SUM(Adebito - Acredito) FROM Movimenti WHERE Conto = @contoType AND Data BETWEEN @startDate AND @endDate"
                End If
                cmd.Parameters.AddWithValue("@contoType", contoType)
                cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))

                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing AndAlso Double.TryParse(result.ToString(), total) Then
                    Return total
                End If
            End Using
        End Using

        Return total
    End Function

    Private Function GetTotalSaldoForDateRange(startDate As Date, endDate As Date) As Double
        Dim total As Double = 0
        Dim databaseFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
        Dim databasePath As String = Path.Combine(databaseFolder, "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath & ";Version=3;"


        Using conn As New SQLiteConnection(connString)
            conn.Open()

            Using cmd As New SQLiteCommand(conn)
                ' Calcoliamo la differenza tra crediti (Acrediti) e debiti (Adebito)
                cmd.CommandText = "SELECT SUM(Acredito) - SUM(Adebito) FROM Movimenti WHERE Data >= @startDate AND Data <= @endDate"
                cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))

                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing AndAlso Double.TryParse(result.ToString(), total) Then
                    Return total
                End If
            End Using
        End Using

        Return total
    End Function

    Private Sub btnCalculate_Click(sender As Object, e As EventArgs) Handles btnCalculate.Click
        If cmbContoType.SelectedItem IsNot Nothing Then
            Dim startDate As Date = dtpStartDate.Value
            Dim endDate As Date = dtpEndDate.Value
            Dim contoType As String = cmbContoType.SelectedItem.ToString()
            Dim totalForRange As Double

            If contoType = "Adebito" Then
                totalForRange = GetTotalAdebitoByDateRange(startDate, endDate)
            ElseIf contoType = "Acredito" Then
                totalForRange = GetTotalAcreditoByDateRange(startDate, endDate)
            ElseIf contoType = "Saldo" Then
                ' Calcola la somma di Adebito tra le date selezionate
                Dim addebitoSum As Double = GetTotalAdebitoByDateRange(startDate, endDate)

                ' Calcola la somma di Acredito tra le date selezionate
                Dim acreditoSum As Double = GetTotalAcreditoByDateRange(startDate, endDate)

                ' Calcola il saldo come differenza tra Acredito e Adebito
                totalForRange = acreditoSum - addebitoSum
            Else
                ' Chiamata a una funzione per calcolare il totale in base a contoType e alle date selezionate
                totalForRange = GetTotalByDateRangeAndConto(startDate, endDate, contoType)
            End If

            ' Visualizza il totale
            MessageBox.Show($"Il totale per {contoType} tra {startDate.ToShortDateString()} e {endDate.ToShortDateString()} è: {totalForRange.ToString("F2")}")

            ' Rendi visibile il pulsante btnShowAll
            btnShowAll.Visible = True
        Else
            MessageBox.Show("Seleziona un tipo di conto dal menu a discesa.")
        End If
    End Sub





    Private Function GetTotalAdebitoByDateRange(startDate As Date, endDate As Date) As Double
        ' Calcola la somma degli Addebiti tra le date selezionate
        Dim total As Double = 0
        Dim databasePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath & ";Version=3;"

        Using conn As New SQLiteConnection(connString)
            conn.Open()

            Using cmd As New SQLiteCommand(conn)
                cmd.CommandText = "SELECT SUM(Adebito) FROM Movimenti WHERE Data BETWEEN @startDate AND @endDate"
                cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))

                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing AndAlso Double.TryParse(result.ToString(), total) Then
                    Return total
                End If
            End Using
        End Using

        Return total
    End Function

    Private Function GetTotalAcreditoByDateRange(startDate As Date, endDate As Date) As Double
        ' Calcola la somma degli Accredito tra le date selezionate
        Dim total As Double = 0
        Dim databasePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath & ";Version=3;"

        Using conn As New SQLiteConnection(connString)
            conn.Open()

            Using cmd As New SQLiteCommand(conn)
                cmd.CommandText = "SELECT SUM(Acredito) FROM Movimenti WHERE Data BETWEEN @startDate AND @endDate"
                cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))

                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing AndAlso Double.TryParse(result.ToString(), total) Then
                    Return total
                End If
            End Using
        End Using

        Return total
    End Function

    Private Sub UpdateLabels()
        ' Calcolo Saldo dal primo gennaio alla data odierna
        Dim saldo As Double = GetTotalSaldoForDateRange(New Date(Date.Today.Year, 1, 1), Date.Today)
        lblSaldo.Text = $"Saldo dal primo gennaio: {saldo:F2}"

        ' Calcolo Patrimonio
        Dim patrimonio As Double = GetTotalPatrimonio()
        lblPatrimonio.Text = $"Patrimonio totale: {patrimonio:F2}"
    End Sub


    Private Function GetTotalPatrimonio() As Double
        Dim total As Double = 0
        Dim importoIniziale As Double = 0 ' Puoi modificare questo valore in base al tuo importo iniziale
        Dim databaseFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
        Dim databasePath As String = Path.Combine(databaseFolder, "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath & ";Version=3;"


        Using conn As New SQLiteConnection(connString)
            conn.Open()

            Using cmd As New SQLiteCommand(conn)
                cmd.CommandText = "SELECT SUM(Acredito) - SUM(Adebito) - @importoIniziale FROM Movimenti"
                cmd.Parameters.AddWithValue("@importoIniziale", importoIniziale)

                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing AndAlso Double.TryParse(result.ToString(), total) Then
                    Return total
                End If
            End Using
        End Using

        Return total
    End Function

    Private Sub DeleteRecord(recordID As Integer)
        Dim databasePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath & ";Version=3;"


        Using conn As New SQLiteConnection(connString)
            conn.Open()

            Using cmd As New SQLiteCommand(conn)
                cmd.CommandText = "DELETE FROM Movimenti WHERE ID = @id"
                cmd.Parameters.AddWithValue("@id", recordID)

                cmd.ExecuteNonQuery()
            End Using
        End Using

        ' Aggiorna la DataGridView
        LoadData()
        UpdateLabels()
        LoadDataIntoComboboxes()
        HighlightDatesWithDocuments()
    End Sub

    Private Sub EditRecord(recordID As Integer, updatedRow As DataGridViewRow)
        Dim databasePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath & ";Version=3;"

        Using conn As New SQLiteConnection(connString)
            conn.Open()

            Using cmd As New SQLiteCommand(conn)
                cmd.CommandText = "UPDATE Movimenti SET Data = @data, Conto = @conto, Descrizione = @descrizione, Adebito = @addebito, Acredito = @acredito, Nota = @nota WHERE ID = @id"
                cmd.Parameters.AddWithValue("@data", DateTime.Parse(updatedRow.Cells("Data").Value.ToString()).ToString("yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@conto", updatedRow.Cells("Conto").Value.ToString())
                cmd.Parameters.AddWithValue("@descrizione", updatedRow.Cells("Descrizione").Value.ToString())
                cmd.Parameters.AddWithValue("@addebito", Convert.ToDouble(updatedRow.Cells("Adebito").Value))
                cmd.Parameters.AddWithValue("@acredito", Convert.ToDouble(updatedRow.Cells("Acredito").Value))
                cmd.Parameters.AddWithValue("@nota", updatedRow.Cells("Nota").Value.ToString())
                cmd.Parameters.AddWithValue("@id", recordID)

                cmd.ExecuteNonQuery()
            End Using
        End Using

        ' Aggiorna la DataGridView
        'LoadData()
        UpdateLabels()
        LoadDataIntoComboboxes()
        HighlightDatesWithDocuments()
    End Sub


    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        If e.RowIndex >= 0 Then
            Dim selectedRow = DataGridView1.Rows(e.RowIndex)

            ' Elimina record
            If DataGridView1.Columns(e.ColumnIndex).Name = "Delete" Then
                Dim result As DialogResult = MessageBox.Show("Sei sicuro di voler eliminare questo record?", "Conferma", MessageBoxButtons.YesNo)
                If result = DialogResult.Yes Then
                    Dim recordID As Integer = Convert.ToInt32(selectedRow.Cells("ID").Value)
                    DeleteRecord(recordID)
                End If
            End If

            ' Modifica record
            If DataGridView1.Columns(e.ColumnIndex).Name = "Edit" Then
                Dim recordID As Integer = Convert.ToInt32(selectedRow.Cells("ID").Value)

                Dim result As DialogResult = MessageBox.Show("Sei sicuro di voler aggiornare questo record?", "Conferma modifica", MessageBoxButtons.YesNo)

                If result = DialogResult.Yes Then
                    EditRecord(recordID, selectedRow)
                    MessageBox.Show("Record aggiornato con successo!")
                    LoadData()  ' per ricaricare i dati e assicurarsi che la visualizzazione sia aggiornata
                End If
            End If


        End If
    End Sub

    Private Sub LoadDataByDateRangeAndConto(startDate As Date, endDate As Date, contoType As String)
        Dim databaseFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
        Dim databasePath As String = Path.Combine(databaseFolder, "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath & ";Version=3;"

        Using conn As New SQLiteConnection(connString)
            conn.Open()

            Using da As New SQLiteDataAdapter("SELECT * FROM Movimenti WHERE Conto = @conto AND Data BETWEEN @startDate AND @endDate", conn)
                da.SelectCommand.Parameters.AddWithValue("@conto", contoType)
                da.SelectCommand.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                da.SelectCommand.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))

                Dim dt As New DataTable
                da.Fill(dt)

                DataGridView1.DataSource = dt
            End Using
        End Using
    End Sub

    Private Function GetTotalByDateRangeAndConto(startDate As Date, endDate As Date, contoType As String) As Double
        Dim total As Double = 0
        Dim databaseFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
        Dim databasePath As String = Path.Combine(databaseFolder, "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath & ";Version=3;"


        Using conn As New SQLiteConnection(connString)
            conn.Open()

            Using cmd As New SQLiteCommand(conn)
                If contoType = "Stipendi" Then
                    cmd.CommandText = "SELECT SUM(Acredito) FROM Movimenti WHERE Conto = @contoType AND Data BETWEEN @startDate AND @endDate"
                Else
                    cmd.CommandText = "SELECT SUM(Adebito - Acredito) FROM Movimenti WHERE Conto = @contoType AND Data BETWEEN @startDate AND @endDate"
                End If
                cmd.Parameters.AddWithValue("@contoType", contoType)
                cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))

                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing AndAlso Double.TryParse(result.ToString(), total) Then
                    Return total
                End If
            End Using
        End Using

        Return total
    End Function

    Private Sub btnShowAll_Click(sender As Object, e As EventArgs) Handles btnShowAll.Click
        ' Chiama il metodo LoadData per caricare nuovamente tutti i movimenti
        LoadData()

        ' Chiamare il metodo UpdateLabels per aggiornare le etichette
        UpdateLabels()

        ' Cancella la selezione attuale nella ComboBox del tipo di conto
        cmbContoType.SelectedItem = Nothing

        ' Resetta il filtro delle date
        dtpStartDate.Value = New Date(Date.Today.Year, 1, 1)
        dtpEndDate.Value = Date.Today

        ' Nascondi nuovamente il pulsante btnShowAll
        btnShowAll.Visible = False
    End Sub

    Private Sub btnbackup_Click(sender As Object, e As EventArgs) Handles btnbackup.Click
        Dim form2 As New Form2
        form2.ShowDialog()
    End Sub

    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        ' Aggiusta la posizione e le dimensioni dei controlli in base allo stato della finestra
        If Me.WindowState = FormWindowState.Maximized Then
            ' La finestra è a schermo intero
            ' Regola la posizione e le dimensioni dei controlli per la modalità a schermo intero
            ' Ad esempio, puoi ridistribuire i controlli e adattarli alle nuove dimensioni
            DataGridView1.Width = Me.ClientSize.Width - 20 ' Aggiusta la larghezza
            DataGridView1.Height = Me.ClientSize.Height - 400 ' Aggiusta l'altezza

            ' E così via per gli altri controlli

            ' Ridistribuisci il pulsante btnbackup
            btnbackup.Location = New Point(Me.ClientSize.Width - btnbackup.Width - 10, Me.ClientSize.Height - btnbackup.Height - 600)
            btnShowAll.Location = New Point(Me.ClientSize.Width - btnShowAll.Width - 150, Me.ClientSize.Height - btnShowAll.Height - 600)

        Else
            ' La finestra è in modalità normale
            ' Aggiusta la posizione e le dimensioni dei controlli per la modalità normale
            ' Ad esempio, riporta i controlli alle posizioni e alle dimensioni originali
            DataGridView1.Width = 400 ' Larghezza originale
            DataGridView1.Height = 272 ' Altezza originale


            ' E così via per gli altri controlli
        End If
    End Sub

    Private Sub btnOpenForm3_Click(sender As Object, e As EventArgs) Handles btnOpenForm3.Click
        Me.Hide()
        Form3.Show()
    End Sub


    Private Sub HighlightDatesWithDocuments()
        Try
            Dim databasePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
            Dim connString As String = "Data Source=" & databasePath & ";Version=3;"

            Using connection As New SQLiteConnection(connString)
                connection.Open()

                Dim query As String = "SELECT DISTINCT Data FROM Movimenti" ' Assicurati che il nome della colonna sia corretto
                Using command As New SQLiteCommand(query, connection)
                    Using reader As SQLiteDataReader = command.ExecuteReader()
                        While reader.Read()
                            If Not reader.IsDBNull(0) Then
                                Dim documentDate As Date = reader.GetDateTime(0)
                                MonthCalendar1.AddBoldedDate(documentDate)
                            End If
                        End While
                    End Using
                End Using
            End Using

            ' Aggiorna il controllo KryptonMonthCalendar per riflettere le date evidenziate
            MonthCalendar1.Update()
        Catch ex As Exception
            MessageBox.Show("Si è verificato un errore durante il caricamento delle date con documenti.")
        End Try
    End Sub

    Private Sub OpenChart_Click(sender As Object, e As EventArgs) Handles OpenChart.Click
        Dim formChart As New FormChart()
        formChart.Show()
    End Sub

    Private Sub bntInfo_Click(sender As Object, e As EventArgs) Handles bntInfo.Click
        AboutBox1.Show()
    End Sub


End Class

