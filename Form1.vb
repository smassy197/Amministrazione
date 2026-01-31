Imports System.Globalization
Imports System.IO
Imports System.Net.Http
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Windows.Forms.DataVisualization.Charting
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel
Imports ClosedXML.Excel
Imports DocumentFormat.OpenXml.Spreadsheet
Imports DocumentFormat.OpenXml.Wordprocessing
Imports Microsoft.Data.Sqlite
'Imports FirebaseAdmin
'Imports FirebaseAdmin.Auth


Public Class Form1
    Private txtPassword As New TextBox()
    '\Private cmbMonthFilter As ComboBox


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            ' Me.Size = New Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height)
            ' Me.Location = New Point(0, 0)
            ApriConsole()


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

            ' Carica dati iniziali
            LoadData()
            LoadDataIntoComboboxes()

            ' Inizializza filtro mese e popolalo
            InitializeMonthFilterCombo()
            PopulateMonths()
            ' Associa evento per aggiornare il grafico quando cambia filtro banca
            AddHandler cmbAccountFilter.SelectedIndexChanged, AddressOf Filters_Changed
            AddHandler cmbMonthFilter.SelectedIndexChanged, AddressOf Filters_Changed

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



            ' Aggiungi "Adebito", "Acredito" e "Saldo" a cmbContoType
            cmbContoType.Items.Add("Adebito")
            cmbContoType.Items.Add("Acredito")
            cmbContoType.Items.Add("Saldo")
            'cmbContoType.Items.Add("Saldo Progressivo")

            CheckForUpdate()

            AggiornaSaldoAnnoCorrente()
            Dim dt As DataTable = CaricaAndamentoAnnoCorrente()
            DisegnaGraficoSaldoLinee(dt, Chart1)   ' chart1 è il controllo Chart sul form


        Catch ex As Exception
            Console.WriteLine("Errore in Form1_Load: " & ex.Message & vbCrLf & ex.StackTrace)
        End Try
    End Sub

    ' Aggiungi questo codice all'interno della tua classe Form5
    <DllImport("kernel32.dll")>
    Private Shared Function AllocConsole() As Boolean
    End Function


    Private Sub ApriConsole()
        AllocConsole()

        ' Personalizzazione estetica

        Console.BackgroundColor = ConsoleColor.DarkBlue

        Console.ForegroundColor = ConsoleColor.White

        Console.Clear() ' Applica i colori a tutta la finestra
        Console.Title = "processi attivi"

        Console.WriteLine("Console avviata e configurata correttamente.")
    End Sub




    Private Async Function GetLatestVersionFromGitHubAsync() As Task(Of String)
        Dim versionUrl As String = "https://raw.githubusercontent.com/smassy197/Amministrazione/master/version.txt"
        Console.WriteLine("Avvio richiesta HTTP verso: " & versionUrl)

        Try
            Using client As New HttpClient()
                Dim response As HttpResponseMessage = Await client.GetAsync(versionUrl)
                Console.WriteLine("Risposta ricevuta. Codice di stato: " & response.StatusCode.ToString())

                If response.IsSuccessStatusCode Then
                    Dim content As String = Await response.Content.ReadAsStringAsync()
                    Console.WriteLine("Contenuto ricevuto correttamente. Versione: " & content.Trim())
                    Return content.Trim()
                Else
                    Console.WriteLine("La richiesta non ha avuto successo. Codice di stato: " & response.StatusCode.ToString())
                End If
            End Using
        Catch ex As Exception
            Console.WriteLine("Errore durante la richiesta HTTP: " & ex.Message)
            Console.WriteLine("Dettagli eccezione: " & ex.ToString())
        End Try

        Console.WriteLine("Nessuna versione recuperata. Restituisco stringa vuota.")
        Return ""
    End Function

    Private Function GetCurrentAppVersion() As String
        Dim currentVersion As String = My.Application.Info.Version.ToString()
        Console.WriteLine("Versione corrente dell'applicazione: " & currentVersion)
        Return currentVersion
    End Function


    Private Async Sub CheckForUpdate()
        Console.WriteLine("Avvio controllo aggiornamenti...")

        Dim latestVersion As String = Await GetLatestVersionFromGitHubAsync()
        Console.WriteLine("Versione più recente disponibile online: " & latestVersion)

        Dim currentVersion As String = GetCurrentAppVersion()
        Console.WriteLine("Versione attualmente installata: " & currentVersion)

        If Not String.IsNullOrEmpty(latestVersion) AndAlso latestVersion <> currentVersion Then
            Console.WriteLine("È disponibile una nuova versione. Prompt utente per aggiornamento.")

            Dim result = MessageBox.Show(
            $"È disponibile una nuova versione: {latestVersion}.{vbCrLf}Vuoi scaricarla e installarla ora?",
            "Aggiornamento disponibile",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question
        )

            If result = DialogResult.Yes Then
                Console.WriteLine("L'utente ha accettato l'aggiornamento. Avvio download e installazione.")
                Await DownloadAndInstallUpdate()
            Else
                Console.WriteLine("L'utente ha rifiutato l'aggiornamento.")
            End If
        Else
            Console.WriteLine("Nessun aggiornamento disponibile o versione già aggiornata.")
        End If

        Console.WriteLine("
           *********************************************************************************
           ####   
           #  #  ### ###  ### ###  #   ###  #   ##  ###  # ##  ###   ###  #   ##    ###  ###
           ####  #  #  #  #  #  #     #  #     #     #    #    # #     #     #  #  #  #  #
           #  #  #  #  #  #  #  #  #  #  #  #   #    #    #    ###    #   #  #  #  #  #  ###
           #  #  #  #  #  #  #  #  #  #  #  #    #   #    #    # #   #    #  #  #  #  #  #
           #  #  #  #  #  #  #  #  #  #  #  #  ##    #    #    # #   ###  #   ##   #  #  ###  
           **********************************************************************************
")
    End Sub


    Private Async Function DownloadAndInstallUpdate() As Task
        Try
            Dim exeUrl As String = "https://github.com/smassy197/Amministrazione/releases/latest/download/mysetup_amministrazione.exe"
            Dim tempPath As String = Path.Combine(Path.GetTempPath(), "Amministrazione_update.exe")

            Console.WriteLine("Inizio download aggiornamento da: " & exeUrl)
            Console.WriteLine("Percorso temporaneo: " & tempPath)

            ProgressBar1.Value = 0
            Panel1.Visible = True
            ProgressBar1.Visible = True
            lblDownloadStatus.Visible = True
            lblDownloadStatus.Text = "Download in corso... 0%"

            Using client As New HttpClient()
                Using response As HttpResponseMessage = Await client.GetAsync(exeUrl, HttpCompletionOption.ResponseHeadersRead)
                    Console.WriteLine("Risposta HTTP ricevuta. StatusCode: " & response.StatusCode.ToString())
                    response.EnsureSuccessStatusCode()

                    Dim totalBytes = response.Content.Headers.ContentLength.GetValueOrDefault()
                    Console.WriteLine("Dimensione file da scaricare: " & totalBytes & " byte")

                    Using inputStream = Await response.Content.ReadAsStreamAsync()
                        Using outputStream = File.Create(tempPath)
                            Dim buffer(8191) As Byte
                            Dim totalRead As Long = 0
                            Dim read As Integer
                            Do
                                read = Await inputStream.ReadAsync(buffer, 0, buffer.Length)
                                If read = 0 Then Exit Do
                                Await outputStream.WriteAsync(buffer, 0, read)
                                totalRead += read

                                If totalBytes > 0 Then
                                    Dim percent = CInt((totalRead * 100) / totalBytes)
                                    ProgressBar1.Value = percent
                                    lblDownloadStatus.Text = $"Download in corso... {percent}%"
                                    Console.WriteLine("Scaricati: " & totalRead & " / " & totalBytes & " (" & percent & "%)")
                                End If

                                Application.DoEvents()
                            Loop
                        End Using
                    End Using
                End Using
            End Using

            ProgressBar1.Value = 100
            lblDownloadStatus.Text = "Download completato! Avvio aggiornamento..."
            Console.WriteLine("Download completato. Avvio eseguibile: " & tempPath)

            Await Task.Delay(1000)
            Panel1.Visible = False
            ProgressBar1.Visible = False
            lblDownloadStatus.Visible = False

            Process.Start(tempPath)
            Console.WriteLine("Installazione avviata. Chiusura applicazione.")
            Application.Exit()
        Catch ex As Exception
            Panel1.Visible = False
            ProgressBar1.Visible = False
            lblDownloadStatus.Visible = False

            MessageBox.Show("Errore durante l'aggiornamento: " & ex.Message)
            Console.WriteLine("Errore durante il download o l'installazione: " & ex.Message)
            Console.WriteLine("Dettagli eccezione: " & ex.ToString())
        End Try
    End Function

    Private Sub InitializeApp(userName As String)
        ' Imposta il titolo della finestra con il nome dell'utente

        Me.Text = $"Amministrazione di {userName}"
        Console.WriteLine("Applicazione inizializzata per l'utente: " & userName)
    End Sub



    'secure application-----------------------------------------------------------
    Private Sub SecureApplication()
        Dim directoryPath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
        Dim configFilePath As String = Path.Combine(directoryPath, "userconfig.txt")
        Dim userName As String = ""

        Console.WriteLine("Avvio procedura di sicurezza applicativa.")
        Console.WriteLine("Percorso configurazione: " & configFilePath)

        If Not File.Exists(configFilePath) Then
            Console.WriteLine("File di configurazione non trovato. Prima installazione.")

            userName = InputBox("Inserisci il tuo nome:", "Prima Installazione", Environment.UserName)
            Console.WriteLine("Nome utente inserito: " & userName)

            Dim password As String = GetPassword()
            Console.WriteLine("Password acquisita. Salvataggio in corso...")

            SaveConfiguration(configFilePath, userName, password)
            Console.WriteLine("Configurazione salvata correttamente.")

            InitializeApp(userName)
        Else
            Console.WriteLine("File di configurazione trovato. Verifica credenziali...")

            userName = ReadUserNameFromConfig(configFilePath)
            Console.WriteLine("Nome utente letto da file: " & userName)

            CheckPassword(configFilePath)
            Console.WriteLine("Password verificata. Inizializzazione applicazione...")

            InitializeApp(userName)
        End If
    End Sub

    ' Funzione per ottenere la password tramite la TextBox
    Private Function GetPassword() As String
        Console.WriteLine("Apertura finestra per inserimento password...")

        Dim passwordForm As New Form()
        passwordForm.Text = "Inserisci la password"
        passwordForm.FormBorderStyle = FormBorderStyle.FixedDialog
        passwordForm.MaximizeBox = False
        passwordForm.MinimizeBox = False
        passwordForm.StartPosition = FormStartPosition.CenterScreen

        ' TextBox della password
        Dim txtPassword As New TextBox()
        txtPassword.PasswordChar = "*"c
        txtPassword.Width = 250
        txtPassword.Location = New Point(10, 10)
        passwordForm.Controls.Add(txtPassword)

        ' Pulsante OK
        Dim btnOK As New Button()
        btnOK.Text = "OK"
        btnOK.DialogResult = DialogResult.OK
        btnOK.Size = New Size(100, 30)
        btnOK.Location = New Point(10, 60)
        passwordForm.Controls.Add(btnOK)
        passwordForm.AcceptButton = btnOK

        ' Pulsante Annulla
        Dim btnCancel As New Button()
        btnCancel.Text = "Annulla"
        btnCancel.DialogResult = DialogResult.Cancel
        btnCancel.Size = New Size(100, 30)
        btnCancel.Location = New Point(120, 60)
        passwordForm.Controls.Add(btnCancel)

        passwordForm.ClientSize = New Size(300, 150)

        Dim result = passwordForm.ShowDialog()

        If result = DialogResult.OK Then
            Console.WriteLine("Password confermata.")
            Return txtPassword.Text
        Else
            Console.WriteLine("Inserimento password annullato.")
            Return ""
        End If
    End Function

    Private Function CheckPassword(configFilePath As String) As Boolean
        Console.WriteLine("Avvio verifica password. File di configurazione: " & configFilePath)

        Try
            Dim lines() As String = File.ReadAllLines(configFilePath)
            Console.WriteLine("File letto correttamente. Numero righe: " & lines.Length)

            If lines.Length >= 2 Then
                Dim storedPasswordLine As String = lines(1)


                Dim storedPassword As String = storedPasswordLine.Substring("password: ".Length).Trim()


                Dim enteredPassword As String = GetPassword()
                Console.WriteLine("Password inserita (mascherata): " & New String("*"c, enteredPassword.Length))

                If enteredPassword = storedPassword Then
                    Console.WriteLine("Password corretta. Accesso consentito.")
                    Return True
                Else
                    Console.WriteLine("Password errata. Accesso negato.")
                    MessageBox.Show("Password non corretta. Chiusura dell'applicazione.")
                    Me.Close()
                    Return False
                End If
            Else
                Console.WriteLine("File di configurazione incompleto. Meno di 2 righe.")
                MessageBox.Show("Il file di configurazione non è valido. Chiusura dell'applicazione.")
                Me.Close()
                Return False
            End If
        Catch ex As Exception
            Console.WriteLine("Errore durante la lettura del file: " & ex.Message)
            Console.WriteLine("Dettagli eccezione: " & ex.ToString())
            MessageBox.Show($"Errore durante la lettura del file di configurazione: {ex.Message}. Chiusura dell'applicazione.")
            Me.Close()
            Return False
        End Try
    End Function

    Private Function ReadUserNameFromConfig(configFilePath As String) As String
        Console.WriteLine("Avvio lettura nome utente dal file di configurazione: " & configFilePath)

        Try
            Dim lines() As String = File.ReadAllLines(configFilePath)
            Console.WriteLine("File letto correttamente. Numero righe: " & lines.Length)

            If lines.Length >= 1 Then
                Dim userNameLine As String = lines(0)
                Console.WriteLine("Riga nome utente: " & userNameLine)

                Dim userName As String = userNameLine.Substring("username: ".Length).Trim()
                Console.WriteLine("Nome utente estratto: " & userName)
                Return userName
            Else
                Console.WriteLine("File di configurazione incompleto. Meno di 1 riga.")
                MessageBox.Show("Il file di configurazione non è valido. Chiusura dell'applicazione.")
                Me.Close()
                Return ""
            End If
        Catch ex As Exception
            Console.WriteLine("Errore durante la lettura del file: " & ex.Message)
            Console.WriteLine("Dettagli eccezione: " & ex.ToString())
            MessageBox.Show($"Errore durante la lettura del file di configurazione: {ex.Message}. Chiusura dell'applicazione.")
            Me.Close()
            Return ""
        End Try
    End Function


    ' Salva il nome utente e la password nel file di configurazione
    Private Sub SaveConfiguration(configFilePath As String, userName As String, password As String)
        Console.WriteLine("Salvataggio configurazione utente in: " & configFilePath)
        Console.WriteLine("Username da salvare: " & userName)
        Console.WriteLine("Password da salvare (mascherata): " & New String("*"c, password.Length))

        File.WriteAllText(configFilePath, $"username: {userName}{Environment.NewLine}password: {password}")
        Console.WriteLine("Configurazione salvata correttamente.")
    End Sub



    '------------------------------------------------------------------------
    Private Sub MonthCalendar1_DateSelected(sender As Object, e As DateRangeEventArgs)
        ' Quando una nuova data viene selezionata, carica i dati relativi a quella data nella DataGridView
        LoadDataByDate(e.Start, e.End)
    End Sub

    Private Sub LoadDataByDate(startDate As Date, endDate As Date)
        Dim databasePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath

        Try
            Using conn As New SqliteConnection(connString)
                conn.Open()

                Dim query As String = "
                SELECT * FROM Movimenti
                WHERE Data >= @startDate AND Data <= @endDate"

                Console.WriteLine("Query eseguita: " & query)
                Console.WriteLine("StartDate: " & startDate.ToString("yyyy-MM-dd"))
                Console.WriteLine("EndDate: " & endDate.ToString("yyyy-MM-dd"))

                Using cmd As New SqliteCommand(query, conn)
                    cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                    cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))

                    Using reader As SqliteDataReader = cmd.ExecuteReader()
                        Dim dt As New DataTable()
                        dt.Columns.Add("ID", GetType(Integer))
                        dt.Columns.Add("Data", GetType(String))
                        dt.Columns.Add("Conto", GetType(String))
                        dt.Columns.Add("Descrizione", GetType(String))
                        dt.Columns.Add("Adebito", GetType(Double))
                        dt.Columns.Add("Acredito", GetType(Double))
                        dt.Columns.Add("Nota", GetType(String))
                        dt.Columns.Add("BankAccount", GetType(String)) ' aggiunta colonna BankAccount

                        While reader.Read()
                            Dim row As DataRow = dt.NewRow()

                            row("ID") = Convert.ToInt32(reader("ID"))
                            row("Data") = reader("Data").ToString()
                            row("Conto") = reader("Conto").ToString()
                            row("Descrizione") = reader("Descrizione").ToString()

                            ' Conversione sicura Adebito
                            Dim adebitoValue As Double
                            If Double.TryParse(reader("Adebito").ToString(), adebitoValue) Then
                                row("Adebito") = adebitoValue
                            Else
                                row("Adebito") = 0
                            End If

                            ' Conversione sicura Acredito
                            Dim acreditValue As Double
                            If Double.TryParse(reader("Acredito").ToString(), acreditValue) Then
                                row("Acredito") = acreditValue
                            Else
                                row("Acredito") = 0
                            End If

                            ' Nota
                            row("Nota") = If(reader("Nota") IsNot DBNull.Value, reader("Nota").ToString(), "")

                            ' BankAccount (se presente)
                            row("BankAccount") = If(reader("BankAccount") IsNot DBNull.Value, reader("BankAccount").ToString(), "HelloBank")

                            dt.Rows.Add(row)
                        End While

                        Console.WriteLine("Righe caricate: " & dt.Rows.Count.ToString())
                        DataGridView1.DataSource = dt
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Errore nel caricamento dei dati: " & ex.Message, "Errore DataGridView", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Console.WriteLine("Errore nel caricamento dei dati: " & ex.ToString())
        End Try
    End Sub

    Private Sub btnResetSearch_Click(sender As Object, e As EventArgs)
        Console.WriteLine("Reset ricerca avviato.")

        ' Ricarica i dati nella DataGridView
        LoadData()
        Console.WriteLine("Dati ricaricati nella griglia.")

        ' Pulisce la selezione nel calendario
        MonthCalendar1.SelectionStart = Date.Today
        MonthCalendar1.SelectionEnd = Date.Today
        Console.WriteLine("Selezione calendario reimpostata alla data odierna.")

        ' Nasconde il pulsante "Mostra tutto"
        btnShowAll.Visible = False
        Console.WriteLine("Pulsante 'Mostra tutto' nascosto.")
    End Sub


    Private Sub CreateDatabaseAndTable()
        Dim databaseFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
        Dim databasePath As String = Path.Combine(databaseFolder, "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath

        Console.WriteLine("Verifica cartella database: " & databaseFolder)

        If Not Directory.Exists(databaseFolder) Then
            Console.WriteLine("Cartella non trovata. Creazione in corso...")
            Directory.CreateDirectory(databaseFolder)
        Else
            Console.WriteLine("Cartella già esistente.")
        End If

        If Not File.Exists(databasePath) Then
            Console.WriteLine("File database non trovato. Creazione in corso...")
            Using conn As New SqliteConnection("Data Source=" & databasePath)
                conn.Open()
            End Using
            Console.WriteLine("File database creato.")
        Else
            Console.WriteLine("File database già esistente.")
        End If

        Using conn As New SqliteConnection(connString)
            conn.Open()

            ' Movimenti: aggiungo la colonna BankAccount con default HelloBank per retrocompatibilità
            Using cmd As New SqliteCommand("CREATE TABLE IF NOT EXISTS Movimenti (ID INTEGER PRIMARY KEY AUTOINCREMENT, Data TEXT, Conto TEXT, Descrizione TEXT, Adebito REAL, Acredito REAL, Nota TEXT, BankAccount TEXT DEFAULT 'HelloBank')", conn)
                cmd.ExecuteNonQuery()
                Console.WriteLine("Tabella 'Movimenti' verificata/creata (con BankAccount).")
            End Using

            ' Descrizioni (categorie)
            Using cmd As New SqliteCommand("CREATE TABLE IF NOT EXISTS Descrizioni (ID INTEGER PRIMARY KEY AUTOINCREMENT, Descrizione TEXT UNIQUE)", conn)
                cmd.ExecuteNonQuery()
                Console.WriteLine("Tabella 'Descrizioni' verificata/creata.")
            End Using

            ' Conti (categorie già esistenti nel tuo DB)
            Using cmd As New SqliteCommand("CREATE TABLE IF NOT EXISTS Conti (ID INTEGER PRIMARY KEY AUTOINCREMENT, Conto TEXT UNIQUE)", conn)
                cmd.ExecuteNonQuery()
                Console.WriteLine("Tabella 'Conti' verificata/creata.")
            End Using

            ' BankAccounts: nuovi conti bancari con InitialBalance
            Using cmd As New SqliteCommand("CREATE TABLE IF NOT EXISTS BankAccounts (ID INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT UNIQUE, InitialBalance REAL DEFAULT 0)", conn)
                cmd.ExecuteNonQuery()
                Console.WriteLine("Tabella 'BankAccounts' verificata/creata.")
            End Using

            ' Migrazione: se Movimenti esisteva senza BankAccount, aggiungila (PRAGMA table_info)
            Try
                Using pragmaCmd As New SqliteCommand("PRAGMA table_info(Movimenti);", conn)
                    Using reader As SqliteDataReader = pragmaCmd.ExecuteReader()
                        Dim hasBankAccount As Boolean = False
                        While reader.Read()
                            Dim colName As String = reader("name").ToString()
                            If String.Equals(colName, "BankAccount", StringComparison.OrdinalIgnoreCase) Then
                                hasBankAccount = True
                                Exit While
                            End If
                        End While

                        If Not hasBankAccount Then
                            Using alterCmd As New SqliteCommand("ALTER TABLE Movimenti ADD COLUMN BankAccount TEXT DEFAULT 'HelloBank';", conn)
                                alterCmd.ExecuteNonQuery()
                                Console.WriteLine("Colonna 'BankAccount' aggiunta a 'Movimenti'.")
                            End Using
                        End If
                    End Using
                End Using
            Catch ex As Exception
                Console.WriteLine("Impossibile verificare/aggiungere BankAccount: " & ex.ToString())
            End Try
        End Using

        Console.WriteLine("Creazione database completata.")
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Dim databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
        Dim connString = "Data Source=" & databasePath

        Try
            Using conn As New SqliteConnection(connString)
                conn.Open()

                Dim insertMovimentiQuery = "
        INSERT INTO Movimenti (Data, Conto, Descrizione, Adebito, Acredito, Nota, BankAccount)
        VALUES (@data, @conto, @descrizione, @addebito, @acredito, @nota, @bankaccount)"


                Using cmd As New SqliteCommand(insertMovimentiQuery, conn)
                    Dim dataStr = MonthCalendar1.SelectionStart.ToString("yyyy-MM-dd")
                    Dim conto = cmbConto.Text
                    Dim descrizione = cmbDescrizione.Text
                    Dim nota = RichTextBoxNota.Text
                    Dim bankAccount = If(cmbBankAccount IsNot Nothing AndAlso Not String.IsNullOrWhiteSpace(cmbBankAccount.Text), cmbBankAccount.Text, "HelloBank")

                    Dim adebitoText = txtAdebito.Text.Replace(".", ",")
                    Dim acreditText = txtAcredito.Text.Replace(".", ",")

                    Dim adebitoValue As Double
                    Dim acreditValue As Double

                    Double.TryParse(adebitoText, NumberStyles.Any, CultureInfo.GetCultureInfo("it-IT"), adebitoValue)
                    Double.TryParse(acreditText, NumberStyles.Any, CultureInfo.GetCultureInfo("it-IT"), acreditValue)

                    cmd.Parameters.AddWithValue("@data", dataStr)
                    cmd.Parameters.AddWithValue("@conto", conto)
                    cmd.Parameters.AddWithValue("@descrizione", descrizione)
                    cmd.Parameters.AddWithValue("@addebito", adebitoValue)
                    cmd.Parameters.AddWithValue("@acredito", acreditValue)
                    cmd.Parameters.AddWithValue("@nota", nota)
                    cmd.Parameters.AddWithValue("@bankaccount", bankAccount)

                    cmd.ExecuteNonQuery()
                End Using

                ' Inserimento descrizione unica
                Using cmd As New SqliteCommand("INSERT OR IGNORE INTO Descrizioni (Descrizione) VALUES (@descrizione)", conn)
                    cmd.Parameters.AddWithValue("@descrizione", cmbDescrizione.Text)
                    cmd.ExecuteNonQuery()
                End Using

                ' Inserimento conto categoria se manca
                Using cmd As New SqliteCommand("INSERT OR IGNORE INTO Conti (Conto) VALUES (@conto)", conn)
                    cmd.Parameters.AddWithValue("@conto", cmbConto.Text)
                    cmd.ExecuteNonQuery()
                End Using

                ' Inserimento BankAccount se manca (mantieni InitialBalance se vuoi impostarlo nel form BankAccounts)
                Using cmd As New SqliteCommand("INSERT OR IGNORE INTO BankAccounts (Name) VALUES (@bank)", conn)
                    cmd.Parameters.AddWithValue("@bank", If(cmbBankAccount IsNot Nothing, cmbBankAccount.Text, "HelloBank"))
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            MessageBox.Show("Dati salvati con successo!")
            LoadData()
            UpdateLabels()
            LoadDataIntoComboboxes()
            HighlightDatesWithDocuments()
            MonthCalendar1.UpdateBoldedDates()
            AggiornaSaldoAnnoCorrente()
            Dim dt = CaricaAndamentoAnnoCorrente()
            DisegnaGraficoSaldoLinee(dt, Chart1)

            ' Reset campi
            txtAcredito.Text = ""
            txtAdebito.Text = ""
            cmbConto.Text = ""
            cmbDescrizione.Text = ""
            RichTextBoxNota.Text = ""
        Catch ex As Exception
            MessageBox.Show("Errore durante il salvataggio: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Console.WriteLine("Errore btnSave_Click: " & ex.ToString)
        End Try
    End Sub

    Private Sub LoadData(Optional contoFilter As String = "", Optional bankFilter As String = "")
        Dim databasePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath

        Dim dt As New DataTable()
        dt.Columns.Add("Id", GetType(Integer))
        dt.Columns.Add("Data", GetType(String))
        dt.Columns.Add("Conto", GetType(String))
        dt.Columns.Add("Descrizione", GetType(String))
        dt.Columns.Add("Adebito", GetType(Double))
        dt.Columns.Add("Acredito", GetType(Double))
        dt.Columns.Add("Nota", GetType(String))
        dt.Columns.Add("BankAccount", GetType(String))

        Using conn As New SqliteConnection(connString)
            conn.Open()
            Dim sql As String = "SELECT * FROM Movimenti"
            Dim whereParts As New List(Of String)
            If Not String.IsNullOrWhiteSpace(contoFilter) Then whereParts.Add("Conto = @conto")
            If Not String.IsNullOrWhiteSpace(bankFilter) Then whereParts.Add("BankAccount = @bank")
            If whereParts.Count > 0 Then sql &= " WHERE " & String.Join(" AND ", whereParts)
            sql &= " ORDER BY Data DESC"

            Using cmd As New SqliteCommand(sql, conn)
                If Not String.IsNullOrWhiteSpace(contoFilter) Then cmd.Parameters.AddWithValue("@conto", contoFilter)
                If Not String.IsNullOrWhiteSpace(bankFilter) Then cmd.Parameters.AddWithValue("@bank", bankFilter)

                Using reader As SqliteDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        Dim row As DataRow = dt.NewRow()
                        row("Id") = reader("Id")
                        row("Data") = reader("Data").ToString()
                        row("Conto") = reader("Conto").ToString()
                        row("Descrizione") = reader("Descrizione").ToString()

                        Dim adebito As Double
                        If Double.TryParse(reader("Adebito").ToString(), adebito) Then row("Adebito") = adebito Else row("Adebito") = 0

                        Dim acred As Double
                        If Double.TryParse(reader("Acredito").ToString(), acred) Then row("Acredito") = acred Else row("Acredito") = 0

                        row("Nota") = If(reader("Nota") IsNot DBNull.Value, reader("Nota").ToString(), "")
                        row("BankAccount") = If(reader("BankAccount") IsNot DBNull.Value, reader("BankAccount").ToString(), "HelloBank")
                        dt.Rows.Add(row)
                    End While
                End Using
            End Using
        End Using

        DataGridView1.DataSource = dt
    End Sub

    Private Function ReadLastBank(filePath As String) As String
        Try
            If File.Exists(filePath) Then
                Dim content = File.ReadAllText(filePath).Trim()
                If Not String.IsNullOrWhiteSpace(content) Then
                    Console.WriteLine("[DEBUG] lastbank letto: " & content)
                    Return content
                End If
            End If
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore ReadLastBank: " & ex.ToString())
        End Try
        Return ""
    End Function

    Private Sub SaveLastBank(filePath As String, bankName As String)
        Try
            File.WriteAllText(filePath, If(bankName, ""))
            Console.WriteLine("[DEBUG] lastbank salvato: " & bankName)
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore SaveLastBank: " & ex.ToString())
        End Try
    End Sub
    Private Sub cmbBankAccount_SelectedIndexChanged(sender As Object, e As EventArgs)
        Try
            Dim databaseFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
            Directory.CreateDirectory(databaseFolder)
            Dim lastBankFile As String = Path.Combine(databaseFolder, "lastbank.txt")

            Dim cb As ComboBox = TryCast(sender, ComboBox)
            If cb Is Nothing Then Return

            Dim selectedName As String = ""
            If cb.SelectedItem IsNot Nothing Then
                ' quando il combobox è bound a DataTable gli elementi sono DataRowView
                Dim drv = TryCast(cb.SelectedItem, DataRowView)
                If drv IsNot Nothing Then
                    selectedName = drv("Name").ToString()
                Else
                    selectedName = cb.Text
                End If
            End If

            If Not String.IsNullOrWhiteSpace(selectedName) Then
                SaveLastBank(lastBankFile, selectedName)
            End If
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore cmbBankAccount_SelectedIndexChanged: " & ex.ToString())
        End Try
    End Sub

    ' Sostituisci il corpo di LoadDataIntoComboboxes con questo aggiornato (modifica minima e poco invasiva)
    Private Sub LoadDataIntoComboboxes()
        Dim databaseFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
        Dim databasePath As String = Path.Combine(databaseFolder, "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath

        Console.WriteLine("Avvio caricamento dati nei ComboBox.")

        ' Descrizioni (categorie)
        Try
            Using connDesc As New SqliteConnection(connString)
                connDesc.Open()
                Using cmd As New SqliteCommand("SELECT Descrizione FROM Descrizioni ORDER BY Descrizione ASC", connDesc)
                    Using reader As SqliteDataReader = cmd.ExecuteReader()
                        Dim dtDesc As New DataTable
                        dtDesc.Load(reader)
                        cmbDescrizione.DataSource = dtDesc
                        cmbDescrizione.DisplayMember = "Descrizione"
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("Errore caricamento Descrizioni: " & ex.Message)
        End Try

        ' Conto (categorie per colonna Conto)
        Try
            Using connConto As New SqliteConnection(connString)
                connConto.Open()
                Using cmd As New SqliteCommand("SELECT Conto FROM Conti ORDER BY Conto ASC", connConto)
                    Using reader As SqliteDataReader = cmd.ExecuteReader()
                        Dim dtConto As New DataTable
                        dtConto.Load(reader)
                        cmbConto.DataSource = dtConto
                        cmbConto.DisplayMember = "Conto"
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("Errore caricamento Conti: " & ex.Message)
        End Try

        ' BankAccounts (conti bancari) -> popola cmbBankAccount e cmbAccountFilter
        Try
            Using connBank As New SqliteConnection(connString)
                connBank.Open()
                Using cmd As New SqliteCommand("SELECT Name, InitialBalance FROM BankAccounts ORDER BY Name ASC", connBank)
                    Using reader As SqliteDataReader = cmd.ExecuteReader()
                        Dim dtBank As New DataTable
                        dtBank.Load(reader)
                        ' cmbBankAccount per inserimento movimento
                        cmbBankAccount.DataSource = dtBank.Copy()
                        cmbBankAccount.DisplayMember = "Name"

                        ' Aggiungo handler per salvare la selezione su file (gestione rimozione/riaggiunta per evitare doppie iscrizioni)
                        Try
                            RemoveHandler cmbBankAccount.SelectedIndexChanged, AddressOf cmbBankAccount_SelectedIndexChanged
                        Catch
                        End Try
                        AddHandler cmbBankAccount.SelectedIndexChanged, AddressOf cmbBankAccount_SelectedIndexChanged

                        ' Ripristina ultimo conto selezionato da lastbank.txt (se presente)
                        Try
                            Directory.CreateDirectory(databaseFolder)
                            Dim lastBankFile As String = Path.Combine(databaseFolder, "lastbank.txt")
                            Dim lastBankName As String = ReadLastBank(lastBankFile)
                            If Not String.IsNullOrWhiteSpace(lastBankName) Then
                                For i As Integer = 0 To cmbBankAccount.Items.Count - 1
                                    Dim drv = TryCast(cmbBankAccount.Items(i), DataRowView)
                                    Dim name As String = If(drv IsNot Nothing, drv("Name").ToString(), cmbBankAccount.Items(i).ToString())
                                    If String.Equals(name, lastBankName, StringComparison.OrdinalIgnoreCase) Then
                                        cmbBankAccount.SelectedIndex = i
                                        Exit For
                                    End If
                                Next
                            End If
                        Catch ex As Exception
                            Console.WriteLine("[DEBUG] Errore ripristino lastbank: " & ex.ToString())
                        End Try

                        ' cmbAccountFilter per filtro vista: prima voce "Tutti i conti bancari"
                        Dim dtFilter As New DataTable()
                        dtFilter.Columns.Add("Name", GetType(String))
                        Dim rAll As DataRow = dtFilter.NewRow()
                        rAll("Name") = "Tutti i conti bancari"
                        dtFilter.Rows.Add(rAll)
                        For Each r As DataRow In dtBank.Rows
                            Dim nr As DataRow = dtFilter.NewRow()
                            nr("Name") = r("Name").ToString()
                            dtFilter.Rows.Add(nr)
                        Next
                        cmbAccountFilter.DataSource = dtFilter
                        cmbAccountFilter.DisplayMember = "Name"
                        cmbAccountFilter.SelectedIndex = 0
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("Errore caricamento BankAccounts: " & ex.Message)
        End Try

        Console.WriteLine("Caricamento ComboBox completato.")
    End Sub


    'parte dedicata ai totali e alla ricerca
    Private Function GetUniqueContiFromMovimenti() As List(Of String)
        Console.WriteLine("Avvio lettura conti unici da Movimenti.")
        Dim conti As New List(Of String)
        Dim databaseFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
        Dim databasePath As String = Path.Combine(databaseFolder, "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath

        Using conn As New SqliteConnection(connString)
            conn.Open()
            Console.WriteLine("Connessione aperta.")

            Using cmd As New SqliteCommand("SELECT DISTINCT Conto FROM Movimenti ORDER BY Conto", conn)
                Using reader As SqliteDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        Dim conto = reader("Conto").ToString()
                        conti.Add(conto)
                        Console.WriteLine("Conto trovato: " & conto)
                    End While
                End Using
            End Using
        End Using

        Console.WriteLine("Totale conti unici trovati: " & conti.Count)
        Return conti
    End Function


    Private Function GetTotalForDateRangeAndConto(startDate As Date, endDate As Date, contoType As String) As Double
        Console.WriteLine($"[DEBUG] Calcolo totale per conto '{contoType}' dal {startDate:yyyy-MM-dd} al {endDate:yyyy-MM-dd}")

        Dim total As Double = 0
        Dim databasePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath
        Console.WriteLine("[DEBUG] Percorso database: " & databasePath)

        Using conn As New SqliteConnection(connString)
            conn.Open()
            Console.WriteLine("[DEBUG] Connessione al database aperta.")

            ' Calcola delta Movimenti per il conto
            Using cmd As New SqliteCommand("", conn)
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
                    Console.WriteLine("[DEBUG] Totale movimenti calcolato: " & total)
                Else
                    total = 0
                End If
            End Using

            ' Aggiungi il saldo iniziale del conto (se presente)
            Using cmdInit As New SqliteCommand("SELECT COALESCE(InitialBalance,0) FROM Conti WHERE Conto = @conto", conn)
                cmdInit.Parameters.AddWithValue("@conto", contoType)
                Dim initResult = cmdInit.ExecuteScalar()
                Dim initBal As Double = 0
                If initResult IsNot Nothing AndAlso Double.TryParse(initResult.ToString(), initBal) Then
                    total += initBal
                    Console.WriteLine("[DEBUG] InitialBalance aggiunto: " & initBal)
                End If
            End Using
        End Using

        Console.WriteLine("[DEBUG] Totale restituito: " & total)
        Return total
    End Function


    Private Function GetTotalSaldoForDateRange(startDate As Date, endDate As Date, Optional contoFilter As String = "", Optional bankFilter As String = "") As Double
        Dim total As Double = 0
        Dim databaseFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
        Dim databasePath As String = Path.Combine(databaseFolder, "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath

        Using conn As New SqliteConnection(connString)
            conn.Open()

            Dim query As String = "SELECT COALESCE(SUM(Acredito),0) - COALESCE(SUM(Adebito),0) FROM Movimenti WHERE Data >= @startDate AND Data <= @endDate"
            If Not String.IsNullOrWhiteSpace(contoFilter) Then query &= " AND Conto = @conto"
            If Not String.IsNullOrWhiteSpace(bankFilter) Then query &= " AND BankAccount = @bank"

            Using cmd As New SqliteCommand(query, conn)
                cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))
                If Not String.IsNullOrWhiteSpace(contoFilter) Then cmd.Parameters.AddWithValue("@conto", contoFilter)
                If Not String.IsNullOrWhiteSpace(bankFilter) Then cmd.Parameters.AddWithValue("@bank", bankFilter)

                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing AndAlso Double.TryParse(result.ToString(), total) Then
                    ' ok
                Else
                    total = 0
                End If
            End Using

            ' Se è selezionato un conto bancario, aggiungi il suo InitialBalance
            If Not String.IsNullOrWhiteSpace(bankFilter) Then
                Using cmdInit As New SqliteCommand("SELECT COALESCE(InitialBalance,0) FROM BankAccounts WHERE Name = @bank", conn)
                    cmdInit.Parameters.AddWithValue("@bank", bankFilter)
                    Dim initResult = cmdInit.ExecuteScalar()
                    Dim initBal As Double = 0
                    If initResult IsNot Nothing AndAlso Double.TryParse(initResult.ToString(), initBal) Then
                        total += initBal
                    End If
                End Using
            End If
        End Using

        Return total
    End Function

    Private Sub btnCalculate_Click(sender As Object, e As EventArgs) Handles btnCalculate.Click
        Console.WriteLine("[DEBUG] Avvio calcolo totale per tipo conto selezionato.")

        If cmbContoType.SelectedItem IsNot Nothing Then
            Dim startDate As Date = dtpStartDate.Value
            Dim endDate As Date = dtpEndDate.Value
            Dim contoType As String = cmbContoType.SelectedItem.ToString()
            Dim totalForRange As Double

            Console.WriteLine($"[DEBUG] Tipo conto: {contoType}")
            Console.WriteLine($"[DEBUG] Intervallo date: {startDate:yyyy-MM-dd} → {endDate:yyyy-MM-dd}")

            If contoType = "Adebito" Then
                Console.WriteLine("[DEBUG] Calcolo totale Adebito...")
                totalForRange = GetTotalAdebitoByDateRange(startDate, endDate)
            ElseIf contoType = "Acredito" Then
                Console.WriteLine("[DEBUG] Calcolo totale Acredito...")
                totalForRange = GetTotalAcreditoByDateRange(startDate, endDate)
            ElseIf contoType = "Saldo" Then
                Console.WriteLine("[DEBUG] Calcolo saldo (Acredito - Adebito)...")
                Dim addebitoSum As Double = GetTotalAdebitoByDateRange(startDate, endDate)
                Dim creditoSum As Double = GetTotalAcreditoByDateRange(startDate, endDate)
                totalForRange = creditoSum - addebitoSum
            Else
                Console.WriteLine("[DEBUG] Calcolo totale personalizzato per conto: " & contoType)
                totalForRange = GetTotalForDateRangeAndConto(startDate, endDate, contoType)
            End If

            totalForRange = Math.Round(totalForRange, 2)

            Console.WriteLine($"[DEBUG] Totale calcolato: {totalForRange:F2}")
            MessageBox.Show($"Il totale per {contoType} tra {startDate.ToShortDateString()} e {endDate.ToShortDateString()} è: {totalForRange.ToString("F2")}")

            btnShowAll.Visible = True
            Console.WriteLine("[DEBUG] Pulsante 'Mostra tutto' reso visibile.")
        Else
            Console.WriteLine("[DEBUG] Nessun tipo conto selezionato.")
            MessageBox.Show("Seleziona un tipo di conto dal menu a discesa.")
        End If
    End Sub


    Private Function GetTotalAdebitoByDateRange(startDate As Date, endDate As Date) As Double
        Console.WriteLine($"[DEBUG] Calcolo totale Adebito dal {startDate:yyyy-MM-dd} al {endDate:yyyy-MM-dd}")

        Dim total As Double = 0
        Dim databasePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath
        Console.WriteLine("[DEBUG] Percorso database: " & databasePath)

        Using conn As New SqliteConnection(connString)
            conn.Open()
            Console.WriteLine("[DEBUG] Connessione al database aperta.")

            Using cmd As New SqliteCommand("", conn)
                cmd.CommandText = "SELECT COALESCE(SUM(Adebito),0) FROM Movimenti WHERE Data BETWEEN @startDate AND @endDate"
                Console.WriteLine("[DEBUG] Query: " & cmd.CommandText)

                cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))
                Console.WriteLine("[DEBUG] Parametri impostati.")

                Dim result = cmd.ExecuteScalar()
                Console.WriteLine("[DEBUG] Risultato query: " & If(result IsNot Nothing, result.ToString(), "NULL"))

                If result IsNot Nothing AndAlso Double.TryParse(result.ToString(), total) Then
                    Console.WriteLine("[DEBUG] Totale Adebito calcolato: " & total)
                    Return total
                Else
                    Console.WriteLine("[DEBUG] Conversione fallita o risultato nullo.")
                End If
            End Using
        End Using

        Console.WriteLine("[DEBUG] Totale restituito: " & total)
        Return total
    End Function


    Private Function GetTotalAcreditoByDateRange(startDate As Date, endDate As Date) As Double
        Console.WriteLine($"[DEBUG] Calcolo totale Acredito dal {startDate:yyyy-MM-dd} al {endDate:yyyy-MM-dd}")

        Dim total As Double = 0
        Dim databasePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath
        Console.WriteLine("[DEBUG] Percorso database: " & databasePath)

        Using conn As SqliteConnection = New SqliteConnection(connString)
            conn.Open()
            Console.WriteLine("[DEBUG] Connessione al database aperta.")

            Using cmd As SqliteCommand = New SqliteCommand("", conn)
                cmd.CommandText = "SELECT COALESCE(SUM(Acredito),0) FROM Movimenti WHERE Data BETWEEN @startDate AND @endDate"
                Console.WriteLine("[DEBUG] Query: " & cmd.CommandText)

                cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))
                Console.WriteLine("[DEBUG] Parametri impostati.")

                Dim result = cmd.ExecuteScalar()
                Console.WriteLine("[DEBUG] Risultato query: " & If(result IsNot Nothing, result.ToString(), "NULL"))

                If result IsNot Nothing AndAlso Double.TryParse(result.ToString(), total) Then
                    Console.WriteLine("[DEBUG] Totale Acredito calcolato: " & total)
                    Return total
                Else
                    Console.WriteLine("[DEBUG] Conversione fallita o risultato nullo.")
                End If
            End Using
        End Using

        Console.WriteLine("[DEBUG] Totale restituito: " & total)
        Return total
    End Function


    Private Sub UpdateLabels()
        Console.WriteLine("[DEBUG] Avvio aggiornamento etichette.")

        ' Calcolo Saldo dal primo gennaio alla data odierna
        Dim saldo As Double = GetTotalSaldoForDateRange(New Date(Date.Today.Year, 1, 1), Date.Today)
        Console.WriteLine("[DEBUG] Saldo calcolato: " & saldo)
        lblSaldo.Text = $"Saldo dal primo gennaio: {saldo:F2}"

        ' Calcolo Patrimonio
        Dim patrimonio As Double = GetTotalPatrimonio()
        Console.WriteLine("[DEBUG] Patrimonio calcolato: " & patrimonio)
        lblPatrimonio.Text = $"Patrimonio totale: {patrimonio:F2}"
    End Sub



    Private Function GetTotalPatrimonio() As Double
        Dim total As Double = 0
        Dim databaseFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
        Dim databasePath As String = Path.Combine(databaseFolder, "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath

        Using conn As New SqliteConnection(connString)
            conn.Open()

            ' Somma degli InitialBalance nella tabella BankAccounts
            Using cmdInit As New SqliteCommand("SELECT COALESCE(SUM(InitialBalance),0) FROM BankAccounts", conn)
                Dim initResult = cmdInit.ExecuteScalar()
                Dim initSum As Double = 0
                If initResult IsNot Nothing AndAlso Double.TryParse(initResult.ToString(), initSum) Then
                    total = initSum
                End If
            End Using

            ' Somma dei movimenti (tutti i conti bancari)
            Using cmdMov As New SqliteCommand("SELECT COALESCE(SUM(Acredito),0) - COALESCE(SUM(Adebito),0) FROM Movimenti", conn)
                Dim movResult = cmdMov.ExecuteScalar()
                Dim movSum As Double = 0
                If movResult IsNot Nothing AndAlso Double.TryParse(movResult.ToString(), movSum) Then
                    total += movSum
                End If
            End Using
        End Using

        Return total
    End Function


    Private Sub DeleteRecord(recordID As Integer)
        Console.WriteLine("[DEBUG] Avvio eliminazione record ID: " & recordID)

        Dim databasePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath

        Try
            Using conn As New Microsoft.Data.Sqlite.SqliteConnection(connString)
                conn.Open()
                Console.WriteLine("[DEBUG] Connessione al database aperta.")

                Using cmd As New Microsoft.Data.Sqlite.SqliteCommand("DELETE FROM Movimenti WHERE ID = @id", conn)
                    cmd.Parameters.AddWithValue("@id", recordID)
                    Console.WriteLine("[DEBUG] Eseguo DELETE per ID: " & recordID)
                    cmd.ExecuteNonQuery()
                    Console.WriteLine("[DEBUG] Record eliminato.")
                End Using
            End Using

            Console.WriteLine("[DEBUG] Aggiornamento interfaccia dopo eliminazione.")
            LoadData()
            UpdateLabels()
            LoadDataIntoComboboxes()
            HighlightDatesWithDocuments()
        Catch ex As Exception
            MessageBox.Show("Errore durante l'eliminazione del record: " & ex.Message)
            Console.WriteLine("[DEBUG] Errore eliminazione record: " & ex.ToString())
        End Try
    End Sub



    Private Sub EditRecord(recordID As Integer, updatedRow As DataGridViewRow)
        Console.WriteLine("[DEBUG] Avvio modifica record ID: " & recordID)

        Dim databasePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath
        Console.WriteLine("[DEBUG] Percorso database: " & databasePath)

        Try
            Using conn As New SqliteConnection(connString)
                conn.Open()
                Console.WriteLine("[DEBUG] Connessione al database aperta.")

                Dim updateQuery As String = "
            UPDATE Movimenti
            SET Data = @data,
                Conto = @conto,
                Descrizione = @descrizione,
                Adebito = @addebito,
                Acredito = @acredito,
                Nota = @nota
            WHERE ID = @id"

                Using cmd As New SqliteCommand(updateQuery, conn)
                    ' Data
                    Dim dataValue As Object = updatedRow.Cells("Data").Value
                    If dataValue IsNot Nothing AndAlso Not IsDBNull(dataValue) AndAlso DateTime.TryParse(dataValue.ToString(), Nothing) Then
                        cmd.Parameters.AddWithValue("@data", Convert.ToDateTime(dataValue).ToString("yyyy-MM-dd"))
                    Else
                        cmd.Parameters.AddWithValue("@data", DBNull.Value)
                    End If

                    ' Conto
                    Dim contoValue As String = If(updatedRow.Cells("Conto").Value IsNot Nothing, updatedRow.Cells("Conto").Value.ToString(), "")
                    cmd.Parameters.AddWithValue("@conto", contoValue)

                    ' Descrizione
                    Dim descrizioneValue As String = If(updatedRow.Cells("Descrizione").Value IsNot Nothing, updatedRow.Cells("Descrizione").Value.ToString(), "")
                    cmd.Parameters.AddWithValue("@descrizione", descrizioneValue)

                    ' Adebito
                    Dim adebitoRaw As String = If(updatedRow.Cells("Adebito").Value IsNot Nothing, updatedRow.Cells("Adebito").Value.ToString(), "")
                    If adebitoRaw.Contains(".") AndAlso Not adebitoRaw.Contains(",") Then
                        adebitoRaw = adebitoRaw.Replace(".", ",")
                    End If
                    Dim adebitoValue As Double
                    Double.TryParse(adebitoRaw, NumberStyles.Any, CultureInfo.GetCultureInfo("it-IT"), adebitoValue)
                    cmd.Parameters.AddWithValue("@addebito", adebitoValue)

                    ' Acredito
                    Dim acreditRaw As String = If(updatedRow.Cells("Acredito").Value IsNot Nothing, updatedRow.Cells("Acredito").Value.ToString(), "")
                    If acreditRaw.Contains(".") AndAlso Not acreditRaw.Contains(",") Then
                        acreditRaw = acreditRaw.Replace(".", ",")
                    End If
                    Dim acreditValue As Double
                    Double.TryParse(acreditRaw, NumberStyles.Any, CultureInfo.GetCultureInfo("it-IT"), acreditValue)
                    cmd.Parameters.AddWithValue("@acredito", acreditValue)

                    ' Nota
                    Dim notaValue As String = If(updatedRow.Cells("Nota").Value IsNot Nothing, updatedRow.Cells("Nota").Value.ToString(), "")
                    cmd.Parameters.AddWithValue("@nota", notaValue)

                    ' ID
                    cmd.Parameters.AddWithValue("@id", recordID)

                    Console.WriteLine("[DEBUG] Eseguo UPDATE Movimenti per ID: " & recordID)
                    Console.WriteLine($"[DEBUG]  Adebito: {adebitoValue} | Acredito: {acreditValue}")
                    cmd.ExecuteNonQuery()
                    Console.WriteLine("[DEBUG] Modifica completata.")
                End Using
            End Using

            Console.WriteLine("[DEBUG] Aggiornamento interfaccia dopo modifica.")
            UpdateLabels()
            LoadDataIntoComboboxes()
            HighlightDatesWithDocuments()
        Catch ex As Exception
            MessageBox.Show("Errore durante la modifica: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Console.WriteLine("[DEBUG] Errore EditRecord: " & ex.ToString())
        End Try
    End Sub


    Private Sub DataGridView1_EditingControlShowing(sender As Object, e As DataGridViewEditingControlShowingEventArgs) Handles DataGridView1.EditingControlShowing
        Dim currentColumn = DataGridView1.CurrentCell.OwningColumn.Name

        If currentColumn = "Adebito" OrElse currentColumn = "Acredito" Then
            Dim tb As TextBox = TryCast(e.Control, TextBox)
            If tb IsNot Nothing Then
                RemoveHandler tb.KeyPress, AddressOf PreventDotKeyPress
                AddHandler tb.KeyPress, AddressOf PreventDotKeyPress
            End If
        End If
    End Sub

    Private Sub PreventDotKeyPress(sender As Object, e As KeyPressEventArgs)
        Console.WriteLine($"[DEBUG] Tasto premuto: '{e.KeyChar}'")

        If e.KeyChar = "."c Then
            e.Handled = True
            MessageBox.Show("Usa la virgola (,) come separatore decimale, non il punto.", "Formato non valido", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Console.WriteLine("[DEBUG] Tentativo di inserimento del punto come separatore decimale bloccato.")
        End If
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
        Dim connString As String = "Data Source=" & databasePath

        Using conn As New SqliteConnection(connString)
            conn.Open()

            Using cmd As New SqliteCommand("SELECT * FROM Movimenti WHERE Conto = @conto AND Data BETWEEN @startDate AND @endDate", conn)
                cmd.Parameters.AddWithValue("@conto", contoType)
                cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))
                Using reader As SqliteDataReader = cmd.ExecuteReader()
                    Dim dt As New DataTable
                    dt.Load(reader)
                    DataGridView1.DataSource = dt
                End Using
            End Using
        End Using
    End Sub

    Private Function GetTotalByDateRangeAndConto(startDate As Date, endDate As Date, contoType As String) As Double
        Console.WriteLine($"[DEBUG] Calcolo totale per conto '{contoType}' dal {startDate:yyyy-MM-dd} al {endDate:yyyy-MM-dd}")

        Dim total As Double = 0
        Dim databaseFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
        Dim databasePath As String = Path.Combine(databaseFolder, "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath
        Console.WriteLine("[DEBUG] Percorso database: " & databasePath)

        Using conn As New SqliteConnection(connString)
            conn.Open()
            Console.WriteLine("[DEBUG] Connessione al database aperta.")

            Using cmd As New SqliteCommand(connString)
                If contoType = "Stipendi" Then
                    cmd.CommandText = "SELECT SUM(Acredito) FROM Movimenti WHERE Conto = @contoType AND Data BETWEEN @startDate AND @endDate"
                    Console.WriteLine("[DEBUG] Query per 'Stipendi': " & cmd.CommandText)
                Else
                    cmd.CommandText = "SELECT SUM(Adebito - Acredito) FROM Movimenti WHERE Conto = @contoType AND Data BETWEEN @startDate AND @endDate"
                    Console.WriteLine("[DEBUG] Query per altri conti: " & cmd.CommandText)
                End If

                cmd.Parameters.AddWithValue("@contoType", contoType)
                cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))
                Console.WriteLine("[DEBUG] Parametri impostati.")

                Dim result = cmd.ExecuteScalar()
                Console.WriteLine("[DEBUG] Risultato query: " & If(result IsNot Nothing, result.ToString(), "NULL"))

                If result IsNot Nothing AndAlso Double.TryParse(result.ToString(), total) Then
                    Console.WriteLine("[DEBUG] Totale calcolato: " & total)
                    Return total
                Else
                    Console.WriteLine("[DEBUG] Conversione fallita o risultato nullo.")
                End If
            End Using
        End Using

        Console.WriteLine("[DEBUG] Totale restituito: " & total)
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



    Private Sub btnOpenForm3_Click(sender As Object, e As EventArgs) Handles btnOpenForm3.Click
        Me.Hide()
        Form3.Show()
    End Sub


    Private Sub HighlightDatesWithDocuments()
        Console.WriteLine("[DEBUG] Avvio evidenziazione date con documenti.")

        Try
            Dim databasePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
            Dim connString As String = "Data Source=" & databasePath
            Console.WriteLine("[DEBUG] Percorso database: " & databasePath)

            Using connection As New SqliteConnection(connString)
                connection.Open()
                Console.WriteLine("[DEBUG] Connessione al database aperta.")

                Dim query As String = "SELECT DISTINCT Data FROM Movimenti"
                Using command As New SqliteCommand(query, connection)
                    Using reader As SqliteDataReader = command.ExecuteReader()
                        Dim count As Integer = 0

                        While reader.Read()
                            If Not reader.IsDBNull(0) Then
                                Dim documentDate As Date = reader.GetDateTime(0)
                                MonthCalendar1.AddBoldedDate(documentDate)
                                count += 1
                            End If
                        End While

                        Console.WriteLine("[DEBUG] Date evidenziate: " & count)
                    End Using
                End Using
            End Using

            MonthCalendar1.Update()
            Console.WriteLine("[DEBUG] Aggiornamento calendario completato.")
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore durante il caricamento delle date: " & ex.Message)
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



    Private Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        Console.WriteLine("[DEBUG] Avvio esportazione movimenti.")

        Dim startDate As Date = dtpStartDate.Value.Date
        Dim endDate As Date = dtpEndDate.Value.Date
        Console.WriteLine($"[DEBUG] Intervallo selezionato: {startDate:yyyy-MM-dd} → {endDate:yyyy-MM-dd}")

        ' Carica i dati filtrati
        Dim dt As DataTable = GetMovimentiByDateRange(startDate, endDate)
        Console.WriteLine("[DEBUG] Record trovati: " & dt.Rows.Count)

        If dt.Rows.Count = 0 Then
            Console.WriteLine("[DEBUG] Nessun dato da esportare.")
            MessageBox.Show("Nessun dato nell'intervallo selezionato.")
            Return
        End If

        Using sfd As New SaveFileDialog()
            sfd.Filter = "Excel (*.xlsx)|*.xlsx|CSV (*.csv)|*.csv"
            sfd.FileName = $"Movimenti_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}"
            Console.WriteLine("[DEBUG] Finestra di salvataggio aperta.")

            If sfd.ShowDialog() = DialogResult.OK Then
                Console.WriteLine("[DEBUG] Percorso selezionato: " & sfd.FileName)

                If sfd.FilterIndex = 1 Then
                    Console.WriteLine("[DEBUG] Esportazione in formato Excel.")
                    Using wb As New XLWorkbook()
                        wb.Worksheets.Add(dt, "Movimenti")
                        wb.SaveAs(sfd.FileName)
                    End Using
                Else
                    Console.WriteLine("[DEBUG] Esportazione in formato CSV.")
                    ExportDataTableToCSV(dt, sfd.FileName)
                End If

                Console.WriteLine("[DEBUG] Esportazione completata.")
                MessageBox.Show("Esportazione completata!")
            Else
                Console.WriteLine("[DEBUG] Esportazione annullata dall'utente.")
            End If
        End Using
    End Sub


    Private Function GetMovimentiByDateRange(startDate As Date, endDate As Date, Optional contoFilter As String = "", Optional bankFilter As String = "") As DataTable
        Dim dt As New DataTable()
        Dim databasePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath

        Using conn As New SqliteConnection(connString)
            conn.Open()
            Dim sql As String = "SELECT * FROM Movimenti WHERE Data BETWEEN @startDate AND @endDate"
            If Not String.IsNullOrWhiteSpace(contoFilter) Then sql &= " AND Conto = @conto"
            If Not String.IsNullOrWhiteSpace(bankFilter) Then sql &= " AND BankAccount = @bank"

            Using cmd As New SqliteCommand(sql, conn)
                cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))
                If Not String.IsNullOrWhiteSpace(contoFilter) Then cmd.Parameters.AddWithValue("@conto", contoFilter)
                If Not String.IsNullOrWhiteSpace(bankFilter) Then cmd.Parameters.AddWithValue("@bank", bankFilter)

                Using reader As SqliteDataReader = cmd.ExecuteReader()
                    dt.Load(reader)
                End Using
            End Using
        End Using

        Return dt
    End Function

    Private Sub ExportDataTableToCSV(dt As DataTable, filePath As String)
        Console.WriteLine("[DEBUG] Avvio esportazione CSV.")
        Console.WriteLine("[DEBUG] Percorso file: " & filePath)
        Console.WriteLine("[DEBUG] Numero colonne: " & dt.Columns.Count)
        Console.WriteLine("[DEBUG] Numero righe: " & dt.Rows.Count)

        Dim sb As New StringBuilder()

        ' Intestazioni
        For Each col As DataColumn In dt.Columns
            sb.Append(col.ColumnName & ";")
        Next
        sb.AppendLine()
        Console.WriteLine("[DEBUG] Intestazioni CSV scritte.")

        ' Dati
        For Each row As DataRow In dt.Rows
            For Each col As DataColumn In dt.Columns
                sb.Append(row(col)?.ToString() & ";")
            Next
            sb.AppendLine()
        Next
        Console.WriteLine("[DEBUG] Dati CSV scritti.")

        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8)
        Console.WriteLine("[DEBUG] File CSV salvato correttamente.")
    End Sub



    Private Function CaricaAndamentoAnnoCorrente(Optional bankFilter As String = "") As DataTable
        Console.WriteLine("[DEBUG] Avvio CaricaAndamentoAnnoCorrente. BankFilter: " & If(String.IsNullOrWhiteSpace(bankFilter), "(tutti)", bankFilter))

        Dim dt As New DataTable()
        dt.Columns.Add("Mese", GetType(String))
        dt.Columns.Add("Entrate", GetType(Double))
        dt.Columns.Add("Uscite", GetType(Double))
        dt.Columns.Add("Saldo", GetType(Double))

        Dim databasePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath

        Try
            Using conn As New Microsoft.Data.Sqlite.SqliteConnection(connString)
                conn.Open()
                Console.WriteLine("[DEBUG] Connessione al database aperta.")

                Dim annoCorrente As Integer = DateTime.Now.Year
                Dim dataInizio As New DateTime(annoCorrente, 1, 1)
                Dim dataFine As DateTime = DateTime.Now

                ' calcolo saldo iniziale (InitialBalance + delta prima dell'inizio dell'anno) se è selezionato un conto bancario
                Dim startingBalance As Double = 0
                If Not String.IsNullOrWhiteSpace(bankFilter) Then
                    Using cmdInit As New Microsoft.Data.Sqlite.SqliteCommand("SELECT COALESCE(InitialBalance,0) FROM BankAccounts WHERE Name = @bank", conn)
                        cmdInit.Parameters.AddWithValue("@bank", bankFilter)
                        Dim v = cmdInit.ExecuteScalar()
                        Dim initBal As Double = 0
                        If v IsNot Nothing AndAlso Double.TryParse(v.ToString(), initBal) Then initBal = initBal
                        ' delta prima dell'anno
                        Using cmdBefore As New Microsoft.Data.Sqlite.SqliteCommand("SELECT COALESCE(SUM(Acredito),0) - COALESCE(SUM(Adebito),0) FROM Movimenti WHERE BankAccount = @bank AND Data < @start", conn)
                            cmdBefore.Parameters.AddWithValue("@bank", bankFilter)
                            cmdBefore.Parameters.AddWithValue("@start", dataInizio.ToString("yyyy-MM-dd"))
                            Dim vb = cmdBefore.ExecuteScalar()
                            Dim deltaBefore As Double = 0
                            If vb IsNot Nothing AndAlso Double.TryParse(vb.ToString(), deltaBefore) Then deltaBefore = deltaBefore
                            startingBalance = initBal + deltaBefore
                        End Using
                    End Using
                End If

                Dim query As String = "
SELECT strftime('%Y-%m', Data) AS Mese,
       SUM(Acredito) AS TotEntrate,
       SUM(Adebito) AS TotUscite
FROM Movimenti
WHERE Data BETWEEN @DataInizio AND @DataFine"

                If Not String.IsNullOrWhiteSpace(bankFilter) Then
                    query &= " AND BankAccount = @bank"
                End If

                query &= "
GROUP BY strftime('%Y-%m', Data)
ORDER BY Mese ASC"

                Using cmd As New Microsoft.Data.Sqlite.SqliteCommand(query, conn)
                    cmd.Parameters.AddWithValue("@DataInizio", dataInizio.ToString("yyyy-MM-dd"))
                    cmd.Parameters.AddWithValue("@DataFine", dataFine.ToString("yyyy-MM-dd"))
                    If Not String.IsNullOrWhiteSpace(bankFilter) Then
                        cmd.Parameters.AddWithValue("@bank", bankFilter)
                    End If

                    Dim temp As New DataTable()
                    Using reader As Microsoft.Data.Sqlite.SqliteDataReader = cmd.ExecuteReader()
                        temp.Load(reader)
                    End Using

                    Dim saldoTotaleDelta As Double = 0
                    For i As Integer = 0 To temp.Rows.Count - 1
                        Dim mese As String = temp.Rows(i)("Mese").ToString()
                        Dim entrate As Double = Math.Round(If(IsDBNull(temp.Rows(i)("TotEntrate")), 0, Convert.ToDouble(temp.Rows(i)("TotEntrate"))), 2)
                        Dim uscite As Double = Math.Round(If(IsDBNull(temp.Rows(i)("TotUscite")), 0, Convert.ToDouble(temp.Rows(i)("TotUscite"))), 2)
                        Dim deltaMensile As Double = Math.Round(entrate - uscite, 2)

                        Dim saldoMensileForRow As Double
                        If i = 0 AndAlso Not String.IsNullOrWhiteSpace(bankFilter) Then
                            ' al primo mese aggiungiamo lo startingBalance così la serie cumulativa parte dal saldo reale
                            saldoMensileForRow = startingBalance + deltaMensile
                        Else
                            saldoMensileForRow = deltaMensile
                        End If

                        saldoTotaleDelta += deltaMensile

                        Dim row As DataRow = dt.NewRow()
                        row("Mese") = mese
                        row("Entrate") = entrate
                        row("Uscite") = uscite
                        row("Saldo") = Math.Round(saldoMensileForRow, 2)
                        dt.Rows.Add(row)

                        Console.WriteLine($"[DEBUG] {mese} → Entrate: {entrate}, Uscite: {uscite}, DeltaMensile: {deltaMensile}, SaldoRow: {saldoMensileForRow}")
                    Next

                    ' Riga finale con saldo totale (se filtro bancario: startingBalance + somma delta; altrimenti somma delta)
                    Dim rowTotale As DataRow = dt.NewRow()
                    rowTotale("Mese") = "Totale anno solare " & annoCorrente & " fino a " & dataFine.ToString("dd/MM/yyyy")
                    rowTotale("Entrate") = If(dt.Rows.Count > 0, Math.Round(dt.AsEnumerable().Sum(Function(r) r.Field(Of Double)("Entrate")), 2), 0)
                    rowTotale("Uscite") = If(dt.Rows.Count > 0, Math.Round(dt.AsEnumerable().Sum(Function(r) r.Field(Of Double)("Uscite")), 2), 0)
                    If Not String.IsNullOrWhiteSpace(bankFilter) Then
                        rowTotale("Saldo") = Math.Round(startingBalance + saldoTotaleDelta, 2)
                    Else
                        rowTotale("Saldo") = Math.Round(saldoTotaleDelta, 2)
                    End If
                    dt.Rows.Add(rowTotale)

                    Console.WriteLine("[DEBUG] Saldo totale anno solare " & annoCorrente & ": " & If(String.IsNullOrWhiteSpace(bankFilter), saldoTotaleDelta, startingBalance + saldoTotaleDelta))
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Errore durante il caricamento andamento anno corrente: " & ex.Message)
            Console.WriteLine("[DEBUG] Errore CaricaAndamentoAnnoCorrente: " & ex.ToString())
        End Try

        Return dt
    End Function


    Private Sub AggiornaSaldoAnnoCorrente(Optional bankFilter As String = "")
        Dim dt As DataTable = CaricaAndamentoAnnoCorrente(bankFilter)
        saldoattuale.DataSource = dt

        ' Formattazione colonne con due decimali (controllo presenza colonne per evitare eccezioni)
        If saldoattuale.Columns.Contains("Entrate") Then saldoattuale.Columns("Entrate").DefaultCellStyle.Format = "N2"
        If saldoattuale.Columns.Contains("Uscite") Then saldoattuale.Columns("Uscite").DefaultCellStyle.Format = "N2"
        If saldoattuale.Columns.Contains("Saldo") Then saldoattuale.Columns("Saldo").DefaultCellStyle.Format = "N2"

        Console.WriteLine("[DEBUG] DataTable assegnato a saldoattuale. Righe: " & dt.Rows.Count & "; BankFilter: " & If(String.IsNullOrWhiteSpace(bankFilter), "(tutti)", bankFilter))
    End Sub

    Private Sub DisegnaGraficoSaldoLinee(dt As DataTable, chartControl As Chart)
        Console.WriteLine("[DEBUG] Avvio DisegnaGraficoSaldoLinee.")

        ' Pulizia grafico
        chartControl.Series.Clear()
        chartControl.ChartAreas.Clear()
        chartControl.Titles.Clear()

        ' Area del grafico
        Dim area As New ChartArea("AreaSaldo")
        area.AxisX.Title = "Mesi"
        area.AxisY.Title = "Saldo (€)"
        area.AxisX.Interval = 1
        area.AxisY.LabelStyle.Format = "N2"
        chartControl.ChartAreas.Add(area)

        ' Serie Saldo mensile (linea azzurra scura)
        Dim serieMensile As New Series("Saldo Mensile")
        serieMensile.ChartType = SeriesChartType.Line
        serieMensile.Color = System.Drawing.Color.DarkBlue
        serieMensile.BorderWidth = 2
        serieMensile.XValueType = ChartValueType.String
        serieMensile.IsXValueIndexed = True
        chartControl.Series.Add(serieMensile)

        ' Serie Saldo cumulativo (linea arancione)
        Dim serieCumulativo As New Series("Saldo Cumulativo")
        serieCumulativo.ChartType = SeriesChartType.Line
        serieCumulativo.Color = System.Drawing.Color.Orange
        serieCumulativo.BorderWidth = 2
        serieCumulativo.XValueType = ChartValueType.String
        serieCumulativo.IsXValueIndexed = True
        chartControl.Series.Add(serieCumulativo)

        Dim saldoProgressivo As Double = 0
        Dim countPunti As Integer = 0

        ' Popolamento dati dalla DataTable: colonna 1 = Mese, colonna 4 = Saldo
        For Each row As DataRow In dt.Rows
            Dim meseLabel As String = row.Item(0).ToString()
            If Not meseLabel.StartsWith("Totale", StringComparison.OrdinalIgnoreCase) Then
                Dim saldoVal As Double = 0
                If Not row.IsNull(3) Then
                    Dim obj = row.Item(3)
                    If TypeOf obj Is Double Then
                        saldoVal = DirectCast(obj, Double)
                    Else
                        Double.TryParse(Convert.ToString(obj), NumberStyles.Any, CultureInfo.InvariantCulture, saldoVal)
                    End If
                End If

                ' Linea saldo mensile
                serieMensile.Points.AddXY(meseLabel, saldoVal)

                ' Linea saldo cumulativo
                saldoProgressivo += saldoVal
                serieCumulativo.Points.AddXY(meseLabel, saldoProgressivo)

                countPunti += 1
                Console.WriteLine($"[DEBUG] {meseLabel}: saldoMensile={saldoVal}, saldoCumulativo={saldoProgressivo}")
            End If
        Next

        chartControl.Titles.Add("Andamento Saldo anno solare " & DateTime.Now.Year)
        Console.WriteLine("[DEBUG] Grafico SaldoLinee completato. Punti aggiunti: " & countPunti)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Console.WriteLine("[DEBUG] Apertura Form5 e chiusura form corrente.")
        Dim nuovaForm As New Form5
        nuovaForm.Show()
    End Sub

    ' Apertura form gestione conti
    Private Sub BtnManageConti_Click(sender As Object, e As EventArgs) Handles btnManageConti.Click
        Try
            Dim f As New FormAccounts()
            f.ShowDialog(Me)
            RefreshAccounts()
        Catch ex As Exception
            Console.WriteLine("Errore apertura FormAccounts: " & ex.ToString())
        End Try
    End Sub

    ' Metodo pubblico chiamabile da FormAccounts per rinfrescare le combo e i totali
    Public Sub RefreshAccounts()
        Try
            LoadDataIntoComboboxes()
            LoadData()
            UpdateLabels()
            HighlightDatesWithDocuments()
            AggiornaSaldoAnnoCorrente()
            Dim dt As DataTable = CaricaAndamentoAnnoCorrente()
            DisegnaGraficoSaldoLinee(dt, Chart1)
        Catch ex As Exception
            Console.WriteLine("Errore in RefreshAccounts: " & ex.ToString())
        End Try
    End Sub

    Private Sub cmbAccountFilter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbAccountFilter.SelectedIndexChanged
        Try
            Dim bankFilter As String = ""
            If cmbAccountFilter.SelectedItem IsNot Nothing Then
                bankFilter = cmbAccountFilter.Text
                If bankFilter = "Tutti i conti bancari" Then bankFilter = ""
            End If

            LoadData("", bankFilter)

            ' Aggiorna etichette rispettando il filtro del conto bancario
            Dim startOfYear As Date = New Date(Date.Today.Year, 1, 1)
            lblSaldo.Text = $"Saldo dal primo gennaio: {GetTotalSaldoForDateRange(startOfYear, Date.Today, "", bankFilter):F2}"
            lblPatrimonio.Text = $"Patrimonio totale: {GetTotalPatrimonio():F2}"

            ' Aggiorna la tabella saldoattuale filtrata
            AggiornaSaldoAnnoCorrente(bankFilter)

        Catch ex As Exception
            Console.WriteLine("Errore filtro conto bancario: " & ex.ToString())
        End Try
    End Sub

    Private Sub InitializeMonthFilterCombo()
        Try
            ' Se il controllo è già stato aggiunto nel Designer, usalo (così puoi posizionarlo manualmente)
            Dim existing As ComboBox = TryCast(Me.Controls("cmbMonthFilter"), ComboBox)
            If existing IsNot Nothing Then
                cmbMonthFilter = existing
                cmbMonthFilter.DropDownStyle = ComboBoxStyle.DropDownList
                Console.WriteLine("[Form1] InitializeMonthFilterCombo: trovato cmbMonthFilter nel Designer.")
                Return
            End If

            ' Altrimenti crealo (puoi sempre spostarlo nel Designer in un secondo momento)
            If cmbMonthFilter Is Nothing Then
                cmbMonthFilter = New ComboBox() With {
                    .Name = "cmbMonthFilter",
                    .DropDownStyle = ComboBoxStyle.DropDownList,
                    .Font = New System.Drawing.Font("Segoe UI", 10.0F),
                    .Size = New System.Drawing.Size(160, 28)
                }
                ' Posizione di default; se preferisci posizionarlo manualmente eliminare questa riga e aggiungerlo via Designer
                cmbMonthFilter.Location = New System.Drawing.Point(dtpEndDate.Location.X + dtpEndDate.Width + 10, dtpEndDate.Location.Y)
                Me.Controls.Add(cmbMonthFilter)
                Console.WriteLine("[Form1] InitializeMonthFilterCombo: cmbMonthFilter aggiunta al form (default position).")
            End If
        Catch ex As Exception
            Console.WriteLine("[Form1] Errore InitializeMonthFilterCombo: " & ex.ToString())
        End Try
    End Sub

    Private Sub UpdateChart()
        Try
            If cmbMonthFilter Is Nothing OrElse cmbMonthFilter.SelectedItem Is Nothing Then
                Console.WriteLine("[Form1] UpdateChart: nessun mese selezionato, disegno annuale.")
                Dim dtAnno = CaricaAndamentoAnnoCorrente()
                DisegnaGraficoSaldoLinee(dtAnno, Chart1)
                Return
            End If

            Dim monthSelection As String = cmbMonthFilter.SelectedItem.ToString()
            If monthSelection = "Tutti i mesi" Then
                ' disegno annuale (comportamento precedente)
                Dim dtAnno = CaricaAndamentoAnnoCorrente()
                DisegnaGraficoSaldoLinee(dtAnno, Chart1)
                Return
            End If

            ' mese selezionato: disegna andamento giornaliero
            Dim bankFilter As String = ""
            If cmbAccountFilter IsNot Nothing AndAlso cmbAccountFilter.SelectedItem IsNot Nothing Then
                bankFilter = cmbAccountFilter.Text
                If bankFilter = "Tutti i conti bancari" Then bankFilter = ""
            End If

            Dim dic = CaricaAndamentoMeseSelezionato(monthSelection, bankFilter)

            ' imposta grafico
            Chart1.Series.Clear()
            Chart1.ChartAreas.Clear()
            Chart1.Titles.Clear()

            Dim area As New ChartArea("AreaMensile")
            area.AxisX.Title = "" ' non mostrare label ascisse
            area.AxisY.Title = "Saldo (€)"
            area.AxisX.Interval = 1
            area.AxisX.LabelStyle.Enabled = False   ' nasconde le etichette sull'asse X (giorni)
            area.AxisX.MajorGrid.Enabled = False
            area.AxisY.LabelStyle.Format = "N2"
            Chart1.ChartAreas.Add(area)

            ' palette colori (qualificata System.Drawing.Color per evitare ambiguità)
            Dim palette As System.Drawing.Color() = {System.Drawing.Color.DarkBlue, System.Drawing.Color.Orange, System.Drawing.Color.Green, System.Drawing.Color.Purple, System.Drawing.Color.Red, System.Drawing.Color.Teal, System.Drawing.Color.Brown}
            Dim idx As Integer = 0

            For Each bankName In dic.Keys
                Dim dt As DataTable = dic(bankName)
                Dim s As New Series(bankName)
                s.ChartType = SeriesChartType.Line
                s.BorderWidth = 2
                s.MarkerStyle = MarkerStyle.Circle
                s.XValueType = ChartValueType.String
                s.IsXValueIndexed = True
                s.Color = palette(idx Mod palette.Length)
                s.IsVisibleInLegend = True

                For Each r As DataRow In dt.Rows
                    s.Points.AddXY(r("Giorno").ToString(), Convert.ToDouble(r("Saldo")))
                Next

                Chart1.Series.Add(s)
                idx += 1
            Next

            Chart1.Titles.Add($"Andamento mese {monthSelection}" & If(String.IsNullOrWhiteSpace(bankFilter), " (tutti i conti)", $" - {bankFilter}"))
            Console.WriteLine("[Form1] UpdateChart: grafico mensile aggiornato.")
        Catch ex As Exception
            Console.WriteLine("[Form1] Errore UpdateChart: " & ex.ToString())
        End Try
    End Sub

    Public Sub Filters_Changed(sender As Object, e As EventArgs)
        Try
            UpdateChart()
        Catch ex As Exception
            Console.WriteLine("[Form1] Filters_Changed errore: " & ex.ToString())
        End Try
    End Sub

    Private Sub PopulateMonths()
        Try
            If cmbMonthFilter Is Nothing Then InitializeMonthFilterCombo()
            cmbMonthFilter.Items.Clear()
            cmbMonthFilter.Items.Add("Tutti i mesi")

            Dim databasePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
            Dim connString As String = "Data Source=" & databasePath

            Using conn As New Microsoft.Data.Sqlite.SqliteConnection(connString)
                conn.Open()
                Using cmd As New Microsoft.Data.Sqlite.SqliteCommand("SELECT DISTINCT strftime('%Y-%m', Data) AS Mese FROM Movimenti WHERE Data IS NOT NULL ORDER BY Mese DESC", conn)
                    Using reader As Microsoft.Data.Sqlite.SqliteDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            If Not reader.IsDBNull(0) Then
                                cmbMonthFilter.Items.Add(reader.GetString(0))
                            End If
                        End While
                    End Using
                End Using
            End Using

            cmbMonthFilter.SelectedIndex = 0
            Console.WriteLine("[Form1] PopulateMonths: mesi popolati.")
        Catch ex As Exception
            Console.WriteLine("[Form1] Errore PopulateMonths: " & ex.ToString())
        End Try
    End Sub

    Public Function CaricaAndamentoMeseSelezionato(yearMonth As String, Optional bankFilter As String = "") As Dictionary(Of String, DataTable)
        Dim result As New Dictionary(Of String, DataTable)()
        Try
            If String.IsNullOrWhiteSpace(yearMonth) Then Return result

            Dim databasePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
            Dim connString As String = "Data Source=" & databasePath

            Using conn As New Microsoft.Data.Sqlite.SqliteConnection(connString)
                conn.Open()

                ' Determina lista conti bancari da disegnare
                Dim banks As New List(Of String)
                If String.IsNullOrWhiteSpace(bankFilter) Then
                    Using cmdBanks As New Microsoft.Data.Sqlite.SqliteCommand("SELECT Name FROM BankAccounts ORDER BY Name", conn)
                        Using r As Microsoft.Data.Sqlite.SqliteDataReader = cmdBanks.ExecuteReader()
                            While r.Read()
                                banks.Add(r.GetString(0))
                            End While
                        End Using
                    End Using
                Else
                    banks.Add(bankFilter)
                End If

                ' Range mese
                Dim parts() = yearMonth.Split("-"c)
                If parts.Length <> 2 Then Return result
                Dim anno As Integer = Integer.Parse(parts(0))
                Dim mese As Integer = Integer.Parse(parts(1))
                Dim monthStart As Date = New Date(anno, mese, 1)
                Dim monthEnd As Date = monthStart.AddMonths(1).AddDays(-1)

                For Each bankName In banks
                    ' saldo iniziale = InitialBalance + delta prima del mese
                    Dim initBal As Double = 0
                    Using cmdInit As New Microsoft.Data.Sqlite.SqliteCommand("SELECT COALESCE(InitialBalance,0) FROM BankAccounts WHERE Name = @bank", conn)
                        cmdInit.Parameters.AddWithValue("@bank", bankName)
                        Dim v = cmdInit.ExecuteScalar()
                        If v IsNot Nothing Then Double.TryParse(v.ToString(), initBal)
                    End Using

                    Dim deltaBefore As Double = 0
                    Using cmdBefore As New Microsoft.Data.Sqlite.SqliteCommand("SELECT COALESCE(SUM(Acredito),0) - COALESCE(SUM(Adebito),0) FROM Movimenti WHERE BankAccount = @bank AND Data < @start", conn)
                        cmdBefore.Parameters.AddWithValue("@bank", bankName)
                        cmdBefore.Parameters.AddWithValue("@start", monthStart.ToString("yyyy-MM-dd"))
                        Dim vb = cmdBefore.ExecuteScalar()
                        If vb IsNot Nothing Then Double.TryParse(vb.ToString(), deltaBefore)
                    End Using

                    Dim startingBalance As Double = initBal + deltaBefore

                    ' raccogli delta giornalieri per il mese
                    Dim deltas As New Dictionary(Of Integer, Double)
                    Using cmdDays As New Microsoft.Data.Sqlite.SqliteCommand("SELECT Data, COALESCE(SUM(Acredito),0) - COALESCE(SUM(Adebito),0) AS Delta FROM Movimenti WHERE BankAccount = @bank AND Data BETWEEN @start AND @end GROUP BY Data ORDER BY Data", conn)
                        cmdDays.Parameters.AddWithValue("@bank", bankName)
                        cmdDays.Parameters.AddWithValue("@start", monthStart.ToString("yyyy-MM-dd"))
                        cmdDays.Parameters.AddWithValue("@end", monthEnd.ToString("yyyy-MM-dd"))
                        Using rd As Microsoft.Data.Sqlite.SqliteDataReader = cmdDays.ExecuteReader()
                            While rd.Read()
                                Dim dStr = rd("Data").ToString()
                                Dim dDate As Date
                                If Date.TryParse(dStr, dDate) Then
                                    Dim day = dDate.Day
                                    Dim delta As Double = 0
                                    Double.TryParse(rd("Delta").ToString(), delta)
                                    deltas(day) = delta
                                End If
                            End While
                        End Using
                    End Using

                    Dim dtDaily As New DataTable()
                    dtDaily.Columns.Add("Giorno", GetType(String))
                    dtDaily.Columns.Add("Saldo", GetType(Double))

                    Dim running = startingBalance
                    Dim daysInMonth = DateTime.DaysInMonth(anno, mese)
                    For d = 1 To daysInMonth
                        Dim dayDelta As Double = 0
                        If deltas.ContainsKey(d) Then dayDelta = deltas(d)
                        running += dayDelta
                        Dim row As DataRow = dtDaily.NewRow()
                        row("Giorno") = d.ToString()
                        row("Saldo") = Math.Round(running, 2)
                        dtDaily.Rows.Add(row)
                    Next

                    result(bankName) = dtDaily
                Next
            End Using
        Catch ex As Exception
            Console.WriteLine("[Form1] Errore CaricaAndamentoMeseSelezionato: " & ex.ToString())
        End Try

        Return result
    End Function
End Class

