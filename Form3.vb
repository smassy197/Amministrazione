Imports Microsoft.Data.Sqlite
Imports System.IO
Imports System.Text
Imports ClosedXML.Excel


Public Class Form3
    ' Percorso della cartella in cui verranno memorizzati i documenti
    Private documentsFolderPath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "Documenti")

    ' Percorso del database
    Private databaseFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
    Private databasePath As String = Path.Combine(databaseFolder, "amministrazione.db")
    Private connString As String = "Data Source=" & databasePath

    ' Dichiarazioni aggiuntive nell'area delle variabili di classe
    Private keywordSearch As String = ""
    Private startDate As Date
    Private endDate As Date
    Private documentType As String = ""
    Private selectedTipoPagamento As String = ""
    Private selectedUtenteID As Integer = 0 ' Inizializza con nessun utente selezionato 
    ' Dichiarazioni in alto nel tuo modulo
    Private WithEvents contextMenuStrip1 As New ContextMenuStrip()
    Private WithEvents eliminaScadenzaToolStripMenuItem As New ToolStripMenuItem("Elimina Scadenza")



    Public Sub AggiornaDataGridView()
        Console.WriteLine("[DEBUG] Avvio aggiornamento DataGridView.")

        Try
            Dim start As Date = If(MonthCalendar1 IsNot Nothing, MonthCalendar1.SelectionStart.Date, Date.MinValue)
            Dim [end] As Date = If(MonthCalendar1 IsNot Nothing, MonthCalendar1.SelectionEnd.Date, Date.MaxValue)
            Dim tipo As String = If(ComboBoxTipoPagamento IsNot Nothing, ComboBoxTipoPagamento.Text, String.Empty)
            Dim anno As String = TextBoxAnno.Text.Trim()
            Dim note As String = If(ComboBoxNote IsNot Nothing, ComboBoxNote.Text.Trim(), String.Empty)
            Dim utente As Integer = GetSelectedUtenteID()

            Console.WriteLine($"[DEBUG] AggiornaDataGridView parametri → start={start:yyyy-MM-dd}, end={[end]:yyyy-MM-dd}, tipo='{tipo}', anno='{anno}', note='{note}', utenteID={utente}")

            LoadDataIntoDataGridView(keywordSearch, start, [end], tipo, utente, anno, note)
            Console.WriteLine("[DEBUG] Dati caricati nella griglia.")

            HighlightDatesWithDocuments()
            Console.WriteLine("[DEBUG] Date con documenti evidenziate.")
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore in AggiornaDataGridView: " & ex.ToString())
            MessageBox.Show("Errore durante l'aggiornamento dei dati: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Sub Form3_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Console.WriteLine("[DEBUG] Avvio Form3_Load.")

        ' Inizializzazione parametri
        Dim keywordSearch As String = ""
        Dim startDate As Date = Date.MinValue
        Dim endDate As Date = Date.MaxValue
        Dim documentType As String = ""
        Dim anno As String = ""
        Dim note As String = ""
        Dim scadenza As String = ""

        ' Gestione evento
        AddHandler ComboBoxTipoPagamento.SelectedIndexChanged, AddressOf ComboBoxTipoPagamento_SelectedIndexChanged

        ' Verifica cartella documenti
        If Not Directory.Exists(documentsFolderPath) Then
            Directory.CreateDirectory(documentsFolderPath)
            Console.WriteLine("[DEBUG] Cartella documenti creata: " & documentsFolderPath)
        Else
            Console.WriteLine("[DEBUG] Cartella documenti esistente.")
        End If

        ' Verifica tabella Documenti
        CreateDocumentiTableIfNotExists()
        Try
            Using conn As New SqliteConnection(connString)
                conn.Open()
                Using cmd As New SqliteCommand("SELECT COUNT(*) FROM Documenti", conn)
                    Dim count = Convert.ToInt32(cmd.ExecuteScalar())
                    Console.WriteLine($"[DEBUG] Tabella Documenti accessibile. Record presenti: {count}")
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore accesso tabella Documenti: " & ex.ToString())
            MessageBox.Show("Errore nell'accesso alla tabella 'Documenti'. Verifica che il database sia stato creato correttamente." & vbCrLf & ex.Message, "Errore Database", MessageBoxButtons.OK, MessageBoxIcon.Error)
            WriteLogEntry("Errore accesso tabella Documenti", ex.Message)
            Exit Sub
        End Try

        ' Popola ComboBox
        PopulateComboBoxFromDatabase(ComboBoxTipoPagamento, "TipoPagamento")
        PopulateComboBoxFromDatabase(ComboBoxNote, "Note")

        ' Evidenzia date
        HighlightDatesWithDocuments()

        ' Ordine tabulazione
        SetTabOrder()

        ' Carica dati iniziali
        LoadDataIntoDataGridView(keywordSearch, startDate, endDate, documentType, GetSelectedUtenteID(), TextBoxAnno.Text.Trim(), ComboBoxNote.Text.Trim())
        LoadUltimiDocumenti()

        ' Menu contestuale
        contextMenuStrip1.Items.Add(eliminaScadenzaToolStripMenuItem)
        TimerScadenze_Tick(Nothing, Nothing)


        Console.WriteLine("[DEBUG] Form3_Load completato.")
    End Sub

    Private Sub btnReset_Click(sender As Object, e As EventArgs) Handles btnReset.Click
        Console.WriteLine("[DEBUG] Avvio reset filtri e ricaricamento dati.")

        Try
            ' Imposta i valori iniziali per il caricamento
            Dim keywordSearch As String = ""
            Dim startDate As Date = Date.MinValue
            Dim endDate As Date = Date.MaxValue
            Dim documentType As String = ""
            Dim anno As String = ""
            Dim note As String = ""
            Dim scadenza As String = ""

            Console.WriteLine("[DEBUG] Parametri impostati: keyword='', date=Min-Max, tipo='', anno='', note='', scadenza=''")

            LoadDataIntoDataGridView(keywordSearch, startDate, endDate, documentType, GetSelectedUtenteID(), TextBoxAnno.Text.Trim(), ComboBoxNote.Text.Trim())
            Console.WriteLine("[DEBUG] Dati ricaricati nella griglia.")

            HighlightDatesWithDocuments()
            Console.WriteLine("[DEBUG] Date con documenti evidenziate.")

            UpdateNotifications()
            Console.WriteLine("[DEBUG] Notifiche ricaricate.")

        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore in btnReset_Click: " & ex.ToString())
            MessageBox.Show("Errore durante il reset dei filtri: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ResetUtenteRadioButton()
        Console.WriteLine("[DEBUG] Reset dei RadioButton utente.")
        rbtnUtente1.Checked = False
        rbtnUtente2.Checked = False
        rbtnUtente3.Checked = False
        Console.WriteLine("[DEBUG] RadioButton utente impostati su False.")
    End Sub



    Private Sub ComboBoxTipoPagamento_SelectedIndexChanged(sender As Object, e As EventArgs)
        selectedTipoPagamento = ComboBoxTipoPagamento.Text
        Console.WriteLine("[DEBUG] Tipo pagamento selezionato: " & selectedTipoPagamento)
    End Sub



    Private Sub PopulateComboBoxFromDatabase(comboBox As ComboBox, columnName As String)
        Console.WriteLine($"[DEBUG] Avvio popolamento ComboBox da colonna '{columnName}'.")

        Try
            Using connection As New SqliteConnection(connString)
                connection.Open()
                Console.WriteLine("[DEBUG] Connessione al database aperta.")

                Dim query As String = $"SELECT DISTINCT {columnName} FROM Documenti ORDER BY {columnName} ASC"
                Console.WriteLine("[DEBUG] Query eseguita: " & query)

                Using command As New SqliteCommand(query, connection)
                    Using reader As SqliteDataReader = command.ExecuteReader()
                        comboBox.Items.Clear()
                        Dim count As Integer = 0

                        While reader.Read()
                            If Not reader.IsDBNull(0) Then
                                comboBox.Items.Add(reader.GetString(0))
                                count += 1
                            End If
                        End While

                        Console.WriteLine($"[DEBUG] Valori caricati nella ComboBox: {count}")
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore in PopulateComboBoxFromDatabase: " & ex.ToString())
            MessageBox.Show($"Si è verificato un errore durante il caricamento dei dati dalla colonna '{columnName}': {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub CreateDocumentiTableIfNotExists()
        Console.WriteLine("[DEBUG] Avvio creazione tabelle Documenti e Allegati.")

        Try
            Using connection As New SqliteConnection(connString)
                connection.Open()
                Console.WriteLine("[DEBUG] Connessione al database aperta.")

                ' Query per la tabella Documenti
                Dim createDocumentiQuery As String =
                "CREATE TABLE IF NOT EXISTS Documenti (" &
                "ID INTEGER PRIMARY KEY AUTOINCREMENT, " &
                "UtenteID INTEGER, " &
                "Data DATE, " &
                "AnnoRiferimento INTEGER, " &
                "TipoPagamento TEXT, " &
                "Importo REAL, " &
                "File TEXT, " &
                "Note TEXT, " &
                "Scadenza DATE, " &
                "StatoPagamento TEXT);"

                Using cmdDocumenti As New SqliteCommand(createDocumentiQuery, connection)
                    cmdDocumenti.ExecuteNonQuery()
                    Console.WriteLine("[DEBUG] Tabella 'Documenti' verificata/creata.")
                End Using

                ' Query per la tabella Allegati
                Dim createAllegatiQuery As String =
                "CREATE TABLE IF NOT EXISTS Allegati (" &
                "ID INTEGER PRIMARY KEY AUTOINCREMENT, " &
                "DocumentoID INTEGER, " &
                "NomeFile TEXT, " &
                "PercorsoFile TEXT, " &
                "FOREIGN KEY (DocumentoID) REFERENCES Documenti(ID));"

                Using cmdAllegati As New SqliteCommand(createAllegatiQuery, connection)
                    cmdAllegati.ExecuteNonQuery()
                    Console.WriteLine("[DEBUG] Tabella 'Allegati' verificata/creata.")
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore creazione tabelle: " & ex.ToString())
            MessageBox.Show("Errore durante la creazione delle tabelle nel database: " & ex.Message, "Errore Database", MessageBoxButtons.OK, MessageBoxIcon.Error)
            WriteLogEntry("Errore creazione tabelle", ex.ToString())
        End Try
    End Sub

    Private Sub btnSfogliaFile_Click(sender As Object, e As EventArgs)
        Console.WriteLine("[DEBUG] Avvio selezione file da disco.")

        Using openFileDialog As New OpenFileDialog
            openFileDialog.Filter = "Tutti i file|*.*"
            openFileDialog.Title = "Seleziona un file"

            If openFileDialog.ShowDialog() = DialogResult.OK Then
                Dim selectedFilePath As String = openFileDialog.FileName
                Dim selectedFileName As String = Path.GetFileName(selectedFilePath)

                Console.WriteLine("[DEBUG] File selezionato: " & selectedFilePath)

                If selectedFileName.Contains(" ") Then
                    Console.WriteLine("[DEBUG] Nome file non valido (contiene spazi): " & selectedFileName)
                    MessageBox.Show("Il nome del file non deve contenere spazi. Rinomina il file e riprova.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If

                ' Visualizza il percorso del file nella TextBox
                ' txtNomeFile.Text = selectedFilePath
                Console.WriteLine("[DEBUG] Percorso file assegnato a txtNomeFile.")
            Else
                Console.WriteLine("[DEBUG] Selezione file annullata dall'utente.")
            End If
        End Using
    End Sub


    Private Sub WriteLogEntry(action As String, details As String)
        Console.WriteLine("[DEBUG] Avvio scrittura log: " & action)

        Try
            Dim logFilePath As String = Path.Combine(documentsFolderPath, "log.txt")
            Console.WriteLine("[DEBUG] Percorso file log: " & logFilePath)

            ' Verifica e crea la directory se assente
            If Not Directory.Exists(documentsFolderPath) Then
                Directory.CreateDirectory(documentsFolderPath)
                Console.WriteLine("[DEBUG] Cartella documenti creata.")
            End If

            ' Componi la riga di log
            Dim logEntry As String = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {action} | {details}"
            Console.WriteLine("[DEBUG] Entry da scrivere: " & logEntry)

            ' Controlla dimensione file log
            Const maxLogFileSize As Long = 5 * 1024 * 1024 ' 5 MB
            If File.Exists(logFilePath) AndAlso New FileInfo(logFilePath).Length > maxLogFileSize Then
                Dim archivedLogPath As String = Path.Combine(documentsFolderPath, $"log_{DateTime.Now:yyyyMMddHHmmss}.txt")
                File.Move(logFilePath, archivedLogPath)
                Console.WriteLine("[DEBUG] File log rinominato: " & archivedLogPath)
            End If

            ' Scrivi nel file
            Using writer As New StreamWriter(logFilePath, append:=True)
                writer.WriteLine(logEntry)
            End Using
            Console.WriteLine("[DEBUG] Scrittura log completata.")
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore scrittura log: " & ex.ToString())
            MessageBox.Show($"Errore durante la scrittura del log: {ex.Message}", "Errore Log", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    ' Aggiorna il gestore dell'evento btnSearch_Click
    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        Console.WriteLine("[DEBUG] Avvio ricerca con filtri selezionati.")

        Try
            ' Aggiorna i valori dei filtri
            Dim documentType As String = ComboBoxTipoPagamento.Text
            Dim startDate As Date = Date.MinValue
            Dim endDate As Date = Date.MaxValue
            Dim anno As String = TextBoxAnno.Text.Trim()
            Dim note As String = ComboBoxNote.Text.Trim()

            Console.WriteLine($"[DEBUG] Filtri impostati: TipoPagamento='{documentType}', Anno='{anno}', Note='{note}'")

            ' Ottieni l'ID dell'utente selezionato
            Dim selectedUtenteID As Integer = GetSelectedUtenteID()
            Console.WriteLine("[DEBUG] Utente selezionato: ID=" & selectedUtenteID)

            ' Carica i dati nel DataGridView con i nuovi filtri
            LoadDataIntoDataGridView(keywordSearch, startDate, endDate, documentType, selectedUtenteID, anno, note)
            Console.WriteLine("[DEBUG] Dati caricati nel DataGridView.")
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore in btnSearch_Click: " & ex.ToString())
            MessageBox.Show("Errore durante la ricerca: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Public Sub ResetFormControls()
        Console.WriteLine("[DEBUG] Avvio reset controlli del form.")

        ' Resetta le TextBox
        TextBoxAnno.Clear()
        Console.WriteLine("[DEBUG] TextBoxAnno resettata.")
        ' TextBoxImporto.Clear()
        Console.WriteLine("[DEBUG] TextBoxImporto resettata.")
        'txtNomeFile.Clear() ' Scommentare se necessario

        ' Resetta le ComboBox
        ComboBoxTipoPagamento.Text = ""
        Console.WriteLine("[DEBUG] ComboBoxTipoPagamento resettata.")
        ComboBoxNote.Text = ""
        Console.WriteLine("[DEBUG] ComboBoxNote resettata.")

        ' Resetta i RadioButton
        rbtnUtente1.Checked = False
        rbtnUtente2.Checked = False
        rbtnUtente3.Checked = False
        Console.WriteLine("[DEBUG] RadioButton utente resettati.")

        ' Imposta il focus iniziale
        TextBoxAnno.Focus()
        Console.WriteLine("[DEBUG] Focus impostato su TextBoxAnno.")
    End Sub


    Private Function InsertDocument(filePath As String) As Boolean
        Console.WriteLine("[DEBUG] Avvio inserimento documento: " & filePath)

        Try
            If Not File.Exists(filePath) Then
                Console.WriteLine("[DEBUG] Il file non esiste: " & filePath)
                MessageBox.Show("Il file specificato non esiste.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return False
            End If

            Dim documentFileName As String = Path.GetFileName(filePath)
            Dim destinationPath As String = Path.Combine(documentsFolderPath, documentFileName)

            If File.Exists(destinationPath) Then
                Console.WriteLine("[DEBUG] Il file esiste già nella cartella documenti: " & destinationPath)
                MessageBox.Show("Il file esiste già.", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            File.Copy(filePath, destinationPath)
            Console.WriteLine("[DEBUG] File copiato in: " & destinationPath)

            Using connection As New SqliteConnection(connString)
                connection.Open()
                Console.WriteLine("[DEBUG] Connessione al database aperta.")

                Dim query As String = "INSERT INTO Documenti (UtenteID, Data, AnnoRiferimento, TipoPagamento, Importo, File, Note) " &
                                  "VALUES (@UtenteID, @Data, @AnnoRiferimento, @TipoPagamento, @Importo, @File, @Note)"

                Using command As New SqliteCommand(query, connection)
                    command.Parameters.AddWithValue("@UtenteID", GetSelectedUtenteID())
                    command.Parameters.AddWithValue("@Data", MonthCalendar1.SelectionStart.ToString("yyyy-MM-dd HH:mm:ss"))
                    command.Parameters.AddWithValue("@AnnoRiferimento", TextBoxAnno.Text.Trim())
                    command.Parameters.AddWithValue("@TipoPagamento", ComboBoxTipoPagamento.Text.Trim())
                    ' command.Parameters.AddWithValue("@Importo", TextBoxImporto.Text.Trim())
                    command.Parameters.AddWithValue("@File", documentFileName)
                    command.Parameters.AddWithValue("@Note", ComboBoxNote.Text.Trim())

                    command.ExecuteNonQuery()
                    Console.WriteLine("[DEBUG] Inserimento record completato.")
                End Using
            End Using

            Return True
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore in InsertDocument: " & ex.ToString())
            MessageBox.Show("Si è verificato un errore durante l'inserimento del documento nel database: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return False
        End Try
    End Function

    Public Sub LoadDataIntoDataGridView(keyword As String, start As Date, [end] As Date, type As String, utenteID As Integer, anno As String, note As String)
        Console.WriteLine("[DEBUG] Avvio caricamento dati in DataGridView.")

        Try
            Using connection As New SqliteConnection(connString)
                connection.Open()
                Console.WriteLine("[DEBUG] Connessione al database aperta.")

                Dim query As String = "
                SELECT * FROM Documenti
                WHERE (@Type = '' OR TipoPagamento = @Type)
                  AND (@Keyword = '' OR Note LIKE '%' || @Keyword || '%')
                  AND (Data BETWEEN @StartDate AND @EndDate)
                  AND (@UtenteID = 0 OR UtenteID = @UtenteID)
                  AND (@AnnoRiferimento = '' OR AnnoRiferimento = @AnnoRiferimento)
                  AND (@Note = '' OR Note = @Note)"

                Using command As New SqliteCommand(query, connection)
                    command.Parameters.AddWithValue("@Type", type)
                    command.Parameters.AddWithValue("@Keyword", keyword)
                    command.Parameters.AddWithValue("@StartDate", start.ToString("yyyy-MM-dd HH:mm:ss"))
                    command.Parameters.AddWithValue("@EndDate", [end].ToString("yyyy-MM-dd HH:mm:ss"))
                    command.Parameters.AddWithValue("@UtenteID", utenteID)
                    command.Parameters.AddWithValue("@AnnoRiferimento", anno)
                    command.Parameters.AddWithValue("@Note", note)

                    Using reader As SqliteDataReader = command.ExecuteReader()
                        Dim dt As New DataTable()
                        dt.Columns.Add("ID", GetType(Integer))
                        dt.Columns.Add("UtenteID", GetType(Integer))
                        dt.Columns.Add("Data", GetType(String))
                        dt.Columns.Add("AnnoRiferimento", GetType(Integer))
                        dt.Columns.Add("TipoPagamento", GetType(String))
                        dt.Columns.Add("Importo", GetType(Double))
                        dt.Columns.Add("File", GetType(String))
                        dt.Columns.Add("Note", GetType(String))
                        dt.Columns.Add("Scadenza", GetType(String))
                        dt.Columns.Add("StatoPagamento", GetType(String))

                        Dim rowCount As Integer = 0

                        While reader.Read()
                            Dim row As DataRow = dt.NewRow()

                            row("ID") = Convert.ToInt32(reader("ID"))
                            row("UtenteID") = If(Integer.TryParse(reader("UtenteID").ToString(), Nothing), Convert.ToInt32(reader("UtenteID")), 0)
                            row("Data") = reader("Data").ToString()
                            row("AnnoRiferimento") = If(Integer.TryParse(reader("AnnoRiferimento").ToString(), Nothing), Convert.ToInt32(reader("AnnoRiferimento")), 0)
                            row("TipoPagamento") = reader("TipoPagamento").ToString()
                            row("File") = reader("File").ToString()
                            row("Note") = reader("Note").ToString()
                            row("Scadenza") = reader("Scadenza").ToString()
                            row("StatoPagamento") = reader("StatoPagamento").ToString()

                            Dim importoValue As Double
                            If Double.TryParse(reader("Importo").ToString(), importoValue) Then
                                row("Importo") = importoValue
                            Else
                                row("Importo") = 0
                            End If

                            dt.Rows.Add(row)
                            rowCount += 1
                        End While

                        Console.WriteLine("[DEBUG] Record letti: " & rowCount)
                        dgvDocumenti.DataSource = dt
                    End Using
                End Using

                ' Aggiunta colonne azione se non esistono
                If Not dgvDocumenti.Columns.Contains("Modifica") Then
                    dgvDocumenti.Columns.Add(New DataGridViewButtonColumn() With {
                    .Name = "Modifica",
                    .HeaderText = "Modifica",
                    .Text = "Modifica",
                    .UseColumnTextForButtonValue = True
                })
                    Console.WriteLine("[DEBUG] Colonna 'Modifica' aggiunta.")
                End If

                If Not dgvDocumenti.Columns.Contains("Elimina") Then
                    dgvDocumenti.Columns.Add(New DataGridViewButtonColumn() With {
                    .Name = "Elimina",
                    .HeaderText = "Elimina",
                    .Text = "Elimina",
                    .UseColumnTextForButtonValue = True
                })
                    Console.WriteLine("[DEBUG] Colonna 'Elimina' aggiunta.")
                End If

                If Not dgvDocumenti.Columns.Contains("Apri File") Then
                    dgvDocumenti.Columns.Add(New DataGridViewButtonColumn() With {
                    .Name = "Apri File",
                    .HeaderText = "Apri File",
                    .Text = "Apri File",
                    .UseColumnTextForButtonValue = True
                })
                    Console.WriteLine("[DEBUG] Colonna 'Apri File' aggiunta.")
                End If
            End Using
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore in LoadDataIntoDataGridView: " & ex.ToString())
            MessageBox.Show("Errore nel caricamento dei dati: " & ex.Message, "Errore DataGridView", MessageBoxButtons.OK, MessageBoxIcon.Error)
            WriteLogEntry("Errore LoadDataIntoDataGridView", ex.ToString())
        End Try
    End Sub


    Private Sub dgvDocumenti_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvDocumenti.CellClick
        If e.RowIndex < 0 Then
            Console.WriteLine("[DEBUG] Clic su intestazione, nessuna azione.")
            Return
        End If

        Dim selectedRow As DataGridViewRow = dgvDocumenti.Rows(e.RowIndex)
        Console.WriteLine("[DEBUG] Riga selezionata: ID=" & selectedRow.Cells("ID").Value.ToString())

        Try
            If e.ColumnIndex = dgvDocumenti.Columns("Modifica").Index Then
                Console.WriteLine("[DEBUG] Pulsante 'Modifica' cliccato.")
                EditDocument(selectedRow)

            ElseIf e.ColumnIndex = dgvDocumenti.Columns("Elimina").Index Then
                Console.WriteLine("[DEBUG] Pulsante 'Elimina' cliccato.")
                DeleteDocument(selectedRow)

            ElseIf e.ColumnIndex = dgvDocumenti.Columns("Apri File").Index Then
                Console.WriteLine("[DEBUG] Pulsante 'Apri File' cliccato.")
                OpenDocumentFile(selectedRow)
            End If
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore in dgvDocumenti_CellClick: " & ex.ToString())
            MessageBox.Show("Errore durante l'elaborazione della riga selezionata: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
            WriteLogEntry("Errore CellClick DataGridView", ex.ToString())
        End Try
    End Sub


    Private Sub EditDocument(row As DataGridViewRow)
        Console.WriteLine("[DEBUG] Avvio modifica documento.")

        If row Is Nothing Then
            Console.WriteLine("[DEBUG] Riga nulla, operazione annullata.")
            MessageBox.Show("Seleziona una riga valida.", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim result As DialogResult = MessageBox.Show("Sei sicuro di voler modificare questa riga?", "Conferma modifica", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If result <> DialogResult.Yes Then
            Console.WriteLine("[DEBUG] Modifica annullata dall'utente.")
            Return
        End If

        Dim documentFileName As String = row.Cells("File").Value.ToString()
        Console.WriteLine("[DEBUG] File associato alla riga: " & documentFileName)

        If documentFileName.Contains(" ") Then
            Console.WriteLine("[DEBUG] Nome file non valido (contiene spazi).")
            MessageBox.Show("Il nome del file non deve contenere spazi. Rinomina il file e riprova.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' Imposta tutte le celle come editabili
        For Each cell As DataGridViewCell In row.Cells
            cell.ReadOnly = False
        Next
        Console.WriteLine("[DEBUG] Celle della riga impostate come editabili.")

        ' Registra il log
        Dim logDetails As String = $"Modifica documento ID: {row.Cells("ID").Value}, File: {documentFileName}, Note precedenti: {row.Cells("Note").Value}"
        WriteLogEntry("Modifica", logDetails)
        Console.WriteLine("[DEBUG] Log modifica registrato.")

        ' Ricarica dati e interfaccia
        LoadDataIntoDataGridView(keywordSearch, startDate, endDate, documentType, GetSelectedUtenteID(), TextBoxAnno.Text.Trim(), ComboBoxNote.Text.Trim())
        Console.WriteLine("[DEBUG] DataGridView ricaricato.")

        SetTabOrder()
        HighlightDatesWithDocuments()
        Console.WriteLine("[DEBUG] Interfaccia aggiornata.")

        PopulateComboBoxFromDatabase(ComboBoxTipoPagamento, "TipoPagamento")
        PopulateComboBoxFromDatabase(ComboBoxNote, "Note")
        Console.WriteLine("[DEBUG] ComboBox ripopolate.")
    End Sub

    Private Sub DeleteDocument(row As DataGridViewRow)
        Console.WriteLine("[DEBUG] Avvio eliminazione documento.")

        If row Is Nothing Then
            Console.WriteLine("[DEBUG] Riga nulla, operazione annullata.")
            MessageBox.Show("Seleziona una riga valida.", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim result As DialogResult = MessageBox.Show("Sei sicuro di voler eliminare questa riga?", "Conferma eliminazione", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
        If result <> DialogResult.Yes Then
            Console.WriteLine("[DEBUG] Eliminazione annullata dall'utente.")
            Return
        End If

        Dim documentId As Integer = Convert.ToInt32(row.Cells("ID").Value)
        Dim fileName As String = row.Cells("File").Value.ToString()
        Dim fullPath As String = Path.Combine(documentsFolderPath, fileName)

        Console.WriteLine($"[DEBUG] Documento ID={documentId}, File={fileName}")

        ' Elimina il file dal filesystem
        If File.Exists(fullPath) Then
            File.Delete(fullPath)
            Console.WriteLine("[DEBUG] File eliminato dal filesystem: " & fullPath)
        Else
            Console.WriteLine("[DEBUG] File non trovato nel filesystem: " & fullPath)
        End If

        ' Elimina il record dal database
        Try
            Using connection As New SqliteConnection(connString)
                connection.Open()
                Console.WriteLine("[DEBUG] Connessione al database aperta.")

                Dim deleteQuery As String = "DELETE FROM Documenti WHERE ID = @ID"
                Using command As New SqliteCommand(deleteQuery, connection)
                    command.Parameters.AddWithValue("@ID", documentId)
                    command.ExecuteNonQuery()
                    Console.WriteLine("[DEBUG] Record eliminato dal database.")
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore eliminazione record: " & ex.ToString())
            MessageBox.Show("Errore durante l'eliminazione del record: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
            WriteLogEntry("Errore eliminazione documento", ex.ToString())
            Return
        End Try

        ' Registra il log
        Dim logDetails As String = $"Eliminazione documento ID: {documentId}, File: {fileName}"
        WriteLogEntry("Eliminazione", logDetails)
        Console.WriteLine("[DEBUG] Log eliminazione registrato.")

        ' Ricarica interfaccia
        LoadDataIntoDataGridView(keywordSearch, startDate, endDate, documentType, GetSelectedUtenteID(), TextBoxAnno.Text.Trim(), ComboBoxNote.Text.Trim())
        SetTabOrder()
        HighlightDatesWithDocuments()
        PopulateComboBoxFromDatabase(ComboBoxTipoPagamento, "TipoPagamento")
        PopulateComboBoxFromDatabase(ComboBoxNote, "Note")
        Console.WriteLine("[DEBUG] Interfaccia aggiornata dopo eliminazione.")
    End Sub

    Private Sub OpenDocumentFile(row As DataGridViewRow)
        Console.WriteLine("[DEBUG] Avvio apertura file associato alla riga.")

        If row Is Nothing OrElse row.Cells("File").Value Is Nothing Then
            Console.WriteLine("[DEBUG] Riga nulla o campo 'File' vuoto.")
            MessageBox.Show("Nessun file associato a questa riga.", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim fileName As String = row.Cells("File").Value.ToString()

        If String.IsNullOrWhiteSpace(fileName) Then
            Console.WriteLine("[DEBUG] Nome file non valido o vuoto.")
            MessageBox.Show("Nome file non valido.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Dim fullPath As String = Path.Combine(documentsFolderPath, fileName)
        Console.WriteLine("[DEBUG] Percorso completo del file: " & fullPath)

        If File.Exists(fullPath) Then
            Try
                Dim psi As New ProcessStartInfo() With {
                .UseShellExecute = True,
                .FileName = fullPath
            }
                Process.Start(psi)
                Console.WriteLine("[DEBUG] File aperto correttamente.")
            Catch winEx As System.ComponentModel.Win32Exception
                Console.WriteLine("[DEBUG] Nessuna applicazione predefinita per il file: " & winEx.Message)
                MessageBox.Show("Non c'è un'applicazione predefinita associata a questo tipo di file. Seleziona un'applicazione predefinita per questo tipo di file nel sistema operativo e riprova.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Catch ex As Exception
                Console.WriteLine("[DEBUG] Errore apertura file: " & ex.ToString())
                MessageBox.Show("Errore nell'apertura del file. Dettagli: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Else
            Console.WriteLine("[DEBUG] File non trovato nel percorso: " & fullPath)
            MessageBox.Show("File non trovato.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub dgvDocumenti_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles dgvDocumenti.CellEndEdit
        Console.WriteLine("[DEBUG] Fine modifica cella. Riga: " & e.RowIndex & ", Colonna: " & e.ColumnIndex)

        Try
            Dim column As DataGridViewColumn = dgvDocumenti.Columns(e.ColumnIndex)
            Dim row As DataGridViewRow = dgvDocumenti.Rows(e.RowIndex)

            Dim documentId As Integer = Convert.ToInt32(row.Cells("ID").Value)
            Dim newValue As Object = row.Cells(e.ColumnIndex).Value

            Console.WriteLine($"[DEBUG] Colonna modificata: {column.Name}, Nuovo valore: {newValue}, ID documento: {documentId}")

            Using connection As New SqliteConnection(connString)
                connection.Open()
                Console.WriteLine("[DEBUG] Connessione al database aperta.")

                Dim updateQuery As String = $"UPDATE Documenti SET {column.Name} = @Value WHERE ID = @ID"
                Using command As New SqliteCommand(updateQuery, connection)
                    command.Parameters.AddWithValue("@Value", newValue)
                    command.Parameters.AddWithValue("@ID", documentId)
                    command.ExecuteNonQuery()
                    Console.WriteLine("[DEBUG] Record aggiornato nel database.")
                End Using
            End Using

            ' Log modifica
            Dim logDetails As String = $"Aggiornamento campo '{column.Name}' per ID={documentId} con valore: {newValue}"
            WriteLogEntry("Aggiornamento campo", logDetails)
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore in CellEndEdit: " & ex.ToString())
            MessageBox.Show("Errore durante l'aggiornamento del campo: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
            WriteLogEntry("Errore CellEndEdit", ex.ToString())
        End Try
    End Sub


    Private Sub SetTabOrder()
        Console.WriteLine("[DEBUG] Impostazione ordine di tabulazione.")

        TextBoxAnno.TabIndex = 0
        ComboBoxTipoPagamento.TabIndex = 1
        'TextBoxImporto.TabIndex = 2
        ComboBoxNote.TabIndex = 3
        'txtNomeFile.TabIndex = 4
        'btnSfogliaFile.TabIndex = 5
        'btnInserisciDati.TabIndex = 6

        Console.WriteLine("[DEBUG] TabIndex assegnati ai controlli principali.")
    End Sub


    Private Function GetSelectedUtenteID() As Integer
        Console.WriteLine("[DEBUG] Verifica utente selezionato.")

        If rbtnUtente1.Checked Then
            Console.WriteLine("[DEBUG] Utente 1 selezionato.")
            Return 1
        ElseIf rbtnUtente2.Checked Then
            Console.WriteLine("[DEBUG] Utente 2 selezionato.")
            Return 2
        ElseIf rbtnUtente3.Checked Then
            Console.WriteLine("[DEBUG] Utente 3 selezionato.")
            Return 3
        Else
            Console.WriteLine("[DEBUG] Nessun utente selezionato.")
            Return 0
        End If
    End Function


    Public Sub HighlightDatesWithDocuments()
        Console.WriteLine("[DEBUG] Avvio evidenziazione date con documenti.")

        Try
            ' Pulisce le date precedenti
            ClearBoldedDates()
            Console.WriteLine("[DEBUG] Date precedenti rimosse dal calendario.")

            Using connection As New SqliteConnection(connString)
                connection.Open()
                Console.WriteLine("[DEBUG] Connessione al database aperta.")

                Dim query As String = "SELECT DISTINCT Data FROM Documenti"
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

            MonthCalendar1.UpdateBoldedDates()
            Console.WriteLine("[DEBUG] Calendario aggiornato con date evidenziate.")
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore in HighlightDatesWithDocuments: " & ex.ToString())
            MessageBox.Show("Si è verificato un errore durante il caricamento delle date con documenti.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
            WriteLogEntry("Errore HighlightDatesWithDocuments", ex.ToString())
        End Try
    End Sub


    Private Sub MonthCalendar1_DateChanged(sender As Object, e As DateRangeEventArgs) Handles MonthCalendar1.DateChanged
        Console.WriteLine("[DEBUG] Data selezionata nel calendario: " & MonthCalendar1.SelectionStart.ToShortDateString())

        startDate = MonthCalendar1.SelectionStart.Date
        endDate = MonthCalendar1.SelectionEnd.Date

        LoadDataIntoDataGridView(keywordSearch, startDate, endDate, documentType, GetSelectedUtenteID(), TextBoxAnno.Text.Trim(), ComboBoxNote.Text.Trim())
        Console.WriteLine("[DEBUG] Dati filtrati per data selezionata.")
    End Sub


    Private Sub ClearBoldedDates()
        Console.WriteLine("[DEBUG] Rimozione date evidenziate dal calendario.")
        MonthCalendar1.RemoveAllBoldedDates()
        MonthCalendar1.UpdateBoldedDates()
        Console.WriteLine("[DEBUG] Calendario aggiornato.")
    End Sub


    Private Sub btnOpenForm3_Click(sender As Object, e As EventArgs) Handles btnOpenForm3.Click
        Console.WriteLine("[DEBUG] Passaggio da form corrente a Form1.")
        Me.Hide()
        Form1.Show()
    End Sub


    Private Sub rbtnUtente1_CheckedChanged(sender As Object, e As EventArgs) Handles rbtnUtente1.CheckedChanged
        Console.WriteLine("[DEBUG] rbtnUtente1 stato modificato: " & rbtnUtente1.Checked.ToString())
    End Sub


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Console.WriteLine("[DEBUG] Apertura Form5 e chiusura form corrente.")
        Dim nuovaForm As New Form5
        nuovaForm.Show()

    End Sub


    Private Sub TimerScadenze_Tick(sender As Object, e As EventArgs) Handles TimerScadenze.Tick
        Console.WriteLine("[DEBUG] TimerScadenze attivato.")
        TimerScadenze.Stop()
        Dim now As DateTime = DateTime.Now
        Dim righeScadute As Integer = 0

        For Each row As DataGridViewRow In dgvDocumenti.Rows
            Dim scadenzaCell = row.Cells("Scadenza")
            Dim statoCell = row.Cells("StatoPagamento")
            Dim scadenzaStr As String = scadenzaCell.Value?.ToString()

            If statoCell.Value IsNot Nothing AndAlso statoCell.Value.ToString().Trim().ToLower() = "scaduto" Then
                scadenzaCell.Style.BackColor = Color.White
                Continue For
            End If

            If Not String.IsNullOrEmpty(scadenzaStr) Then
                Dim scadenza As DateTime
                If DateTime.TryParse(scadenzaStr, scadenza) Then
                    Dim giorniRimanenti As Integer = (scadenza - now).Days
                    Select Case giorniRimanenti
                        Case <= 10
                            scadenzaCell.Style.BackColor = Color.Red
                            righeScadute += 1
                        Case <= 15
                            scadenzaCell.Style.BackColor = Color.Orange
                        Case <= 20
                            scadenzaCell.Style.BackColor = Color.Yellow
                        Case Else
                            scadenzaCell.Style.BackColor = Color.White
                    End Select
                Else
                    Console.WriteLine("[DEBUG] Data di scadenza non valida: " & scadenzaStr)
                    scadenzaCell.Style.BackColor = Color.White
                End If
            Else
                scadenzaCell.Style.BackColor = Color.White
            End If
        Next

        ' Aggiorna notifiche visive
        lblNotifica.Font = New Font(lblNotifica.Font, FontStyle.Bold)
        lblNotifica.ForeColor = Color.Red
        lblNotifica.Text = righeScadute.ToString()
        lblNotifica.Visible = (righeScadute > 0)
        picNotifica.Visible = (righeScadute > 0)

        Console.WriteLine("[DEBUG] Righe scadute: " & righeScadute)
    End Sub

    ' Gestisci l'evento clic dell'elemento del menu "Elimina Scadenza"
    Private Sub eliminaScadenzaToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles eliminaScadenzaToolStripMenuItem.Click
        Console.WriteLine("[DEBUG] Avvio eliminazione scadenza da riga selezionata.")

        Dim selectedRow As DataGridViewRow = dgvDocumenti.CurrentRow
        If selectedRow IsNot Nothing Then
            selectedRow.Cells("Scadenza").Value = DBNull.Value
            Console.WriteLine("[DEBUG] Scadenza rimossa dalla cella.")

            UpdateScadenzaInDatabase(selectedRow.Index, Nothing)
            Console.WriteLine("[DEBUG] Scadenza aggiornata nel database.")

            MessageBox.Show("La scadenza è stata eliminata con successo.", "Conferma", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Dim logDetails As String = $"Scadenza rimossa per documento ID: {selectedRow.Cells("ID").Value}"
            WriteLogEntry("Eliminazione scadenza", logDetails)
        Else
            Console.WriteLine("[DEBUG] Nessuna riga selezionata.")
            MessageBox.Show("Seleziona una riga valida.", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub


    Private Sub UpdateScadenzaInDatabase(rowIndex As Integer, scadenza As DateTime?)
        Console.WriteLine("[DEBUG] Aggiornamento campo Scadenza nel database.")

        Try
            Using connection As New SqliteConnection(connString)
                connection.Open()
                Console.WriteLine("[DEBUG] Connessione al database aperta.")

                Dim query As String = "UPDATE Documenti SET Scadenza = @Scadenza WHERE ID = @ID"
                Using command As New SqliteCommand(query, connection)
                    command.Parameters.AddWithValue("@Scadenza", If(scadenza.HasValue, scadenza.Value.ToString("yyyy-MM-dd HH:mm:ss"), DBNull.Value))
                    command.Parameters.AddWithValue("@ID", dgvDocumenti.Rows(rowIndex).Cells("ID").Value)
                    command.ExecuteNonQuery()
                    Console.WriteLine("[DEBUG] Query eseguita correttamente.")
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore in UpdateScadenzaInDatabase: " & ex.ToString())
            MessageBox.Show("Errore durante l'aggiornamento della scadenza: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
            WriteLogEntry("Errore aggiornamento scadenza", ex.ToString())
        End Try
    End Sub


    Private Sub dgvDocumenti_CellMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgvDocumenti.CellMouseClick
        If e.Button = MouseButtons.Right AndAlso e.RowIndex >= 0 Then
            Console.WriteLine("[DEBUG] Click destro su riga: " & e.RowIndex)

            dgvDocumenti.ClearSelection()
            dgvDocumenti.Rows(e.RowIndex).Selected = True

            Dim selectedRow As DataGridViewRow = dgvDocumenti.Rows(e.RowIndex)
            Dim documentId As Integer = Convert.ToInt32(selectedRow.Cells("ID").Value)
            Console.WriteLine("[DEBUG] Documento selezionato: ID=" & documentId)

            ' Crea menu contestuale
            Dim contextMenu As New ContextMenuStrip()
            Dim modifyAttachmentMenuItem As New ToolStripMenuItem("Modifica Allegato")
            Dim setDeadlineMenuItem As New ToolStripMenuItem("Imposta Scadenza")
            Dim setExpiredMenuItem As New ToolStripMenuItem("Imposta scaduto")

            ' Handler: Modifica Allegato
            AddHandler modifyAttachmentMenuItem.Click, Sub()
                                                           Console.WriteLine("[DEBUG] Modifica Allegato selezionata.")
                                                           ModifyAttachment(selectedRow, documentId)
                                                       End Sub

            ' Handler: Imposta Scadenza
            AddHandler setDeadlineMenuItem.Click, Sub()
                                                      Console.WriteLine("[DEBUG] Imposta Scadenza selezionata.")
                                                      SetDeadline(selectedRow, selectedRow.Index)
                                                  End Sub

            ' Handler: Imposta Scaduto
            AddHandler setExpiredMenuItem.Click, Sub()
                                                     Console.WriteLine("[DEBUG] Imposta Scaduto selezionata.")
                                                     SetScaduto(selectedRow)
                                                 End Sub

            contextMenu.Items.Add(modifyAttachmentMenuItem)
            contextMenu.Items.Add(setDeadlineMenuItem)
            contextMenu.Items.Add(setExpiredMenuItem)

            contextMenu.Show(Cursor.Position)
            Console.WriteLine("[DEBUG] Menu contestuale visualizzato.")
        End If
    End Sub

    Private Sub SetScaduto(selectedRow As DataGridViewRow)
        Console.WriteLine("[DEBUG] Avvio impostazione stato 'Scaduto'.")

        If selectedRow Is Nothing Then
            Console.WriteLine("[DEBUG] Nessuna riga selezionata.")
            MessageBox.Show("Seleziona una riga valida.", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Imposta sfondo bianco sulla cella Scadenza
        selectedRow.Cells("Scadenza").Style.BackColor = Color.White
        Console.WriteLine("[DEBUG] Sfondo cella 'Scadenza' impostato su bianco.")

        ' Aggiorna StatoPagamento nel database
        Dim documentId As Integer = Convert.ToInt32(selectedRow.Cells("ID").Value)
        Try
            Using connection As New SqliteConnection(connString)
                connection.Open()
                Console.WriteLine("[DEBUG] Connessione al database aperta.")

                Dim query As String = "UPDATE Documenti SET StatoPagamento = @Stato WHERE ID = @ID"
                Using command As New SqliteCommand(query, connection)
                    command.Parameters.AddWithValue("@Stato", "Scaduto")
                    command.Parameters.AddWithValue("@ID", documentId)
                    command.ExecuteNonQuery()
                    Console.WriteLine("[DEBUG] StatoPagamento aggiornato nel database.")
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore aggiornamento StatoPagamento: " & ex.ToString())
            MessageBox.Show("Errore durante l'aggiornamento dello stato: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
            WriteLogEntry("Errore SetScaduto", ex.ToString())
            Return
        End Try

        ' Aggiorna la cella nella DataGridView
        If selectedRow.DataGridView.Columns.Contains("StatoPagamento") Then
            selectedRow.Cells("StatoPagamento").Value = "Scaduto"
            Console.WriteLine("[DEBUG] StatoPagamento aggiornato nella griglia.")
        End If

        ' Aggiorna la notifica
        TimerScadenze_Tick(Nothing, Nothing)
        Console.WriteLine("[DEBUG] TimerScadenze_Tick richiamato per aggiornamento visivo.")

        ' Log operazione
        Dim logDetails As String = $"StatoPagamento impostato su 'Scaduto' per documento ID: {documentId}"
        WriteLogEntry("Imposta Scaduto", logDetails)
    End Sub

    Private Sub SetDeadline(selectedRow As DataGridViewRow, rowIndex As Integer)
        Console.WriteLine("[DEBUG] Avvio impostazione scadenza per riga: " & rowIndex)

        Dim currentDeadline As DateTime? = Nothing

        ' Recupera la scadenza esistente
        If Not IsDBNull(selectedRow.Cells("Scadenza").Value) Then
            Dim tempDate As DateTime
            If DateTime.TryParse(selectedRow.Cells("Scadenza").Value.ToString(), tempDate) Then
                currentDeadline = tempDate
                Console.WriteLine("[DEBUG] Scadenza esistente rilevata: " & tempDate.ToString("yyyy-MM-dd HH:mm"))
            End If
        End If

        ' Apri il modulo per impostare la nuova scadenza
        Using deadlineForm As New SetDeadlineForm()
            deadlineForm.Deadline = currentDeadline

            If deadlineForm.ShowDialog() = DialogResult.OK Then
                Console.WriteLine("[DEBUG] Nuova scadenza confermata: " & If(deadlineForm.Deadline.HasValue, deadlineForm.Deadline.Value.ToString("yyyy-MM-dd HH:mm"), "nessuna"))

                ' Aggiorna nel database
                UpdateScadenzaInDatabase(rowIndex, deadlineForm.Deadline)

                ' Aggiorna nella griglia
                If deadlineForm.Deadline.HasValue Then
                    selectedRow.Cells("Scadenza").Value = deadlineForm.Deadline.Value.ToString("dd/MM/yyyy HH:mm")
                Else
                    selectedRow.Cells("Scadenza").Value = DBNull.Value
                End If

                dgvDocumenti.Refresh()
                Console.WriteLine("[DEBUG] DataGridView aggiornato.")
            Else
                Console.WriteLine("[DEBUG] Impostazione scadenza annullata dall'utente.")
            End If
        End Using
    End Sub


    Private Sub ModifyAttachment(selectedRow As DataGridViewRow, documentId As Integer)
        Console.WriteLine("[DEBUG] Avvio modifica allegato per documento ID: " & documentId)

        Dim oldFileName As String = selectedRow.Cells("File").Value.ToString()
        Console.WriteLine("[DEBUG] File precedente: " & oldFileName)

        Using openFileDialog As New OpenFileDialog()
            If openFileDialog.ShowDialog() = DialogResult.OK Then
                Dim newFilePath As String = openFileDialog.FileName
                Dim newFileName As String = Path.GetFileName(newFilePath)
                Console.WriteLine("[DEBUG] Nuovo file selezionato: " & newFileName)

                If newFileName.Contains(" ") Then
                    Console.WriteLine("[DEBUG] Nome file non valido (contiene spazi).")
                    MessageBox.Show("Il nome del file non deve contenere spazi. Rinomina il file e riprova.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If

                Dim targetPath As String = Path.Combine(documentsFolderPath, newFileName)
                Try
                    File.Copy(newFilePath, targetPath, True)
                    Console.WriteLine("[DEBUG] File copiato in: " & targetPath)
                Catch ex As Exception
                    Console.WriteLine("[DEBUG] Errore copia file: " & ex.ToString())
                    MessageBox.Show("Errore durante la copia del file: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End Try

                UpdateDocumentFile(documentId, newFileName)
                Console.WriteLine("[DEBUG] Database aggiornato con nuovo file.")

                If Not String.IsNullOrEmpty(oldFileName) AndAlso File.Exists(Path.Combine(documentsFolderPath, oldFileName)) Then
                    Dim result As DialogResult = MessageBox.Show("Vuoi eliminare il file precedente?", "Conferma Eliminazione", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    If result = DialogResult.Yes Then
                        Try
                            File.Delete(Path.Combine(documentsFolderPath, oldFileName))
                            Console.WriteLine("[DEBUG] File precedente eliminato: " & oldFileName)
                            MessageBox.Show("Il file precedente è stato eliminato con successo.", "Conferma", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        Catch ex As Exception
                            Console.WriteLine("[DEBUG] Errore eliminazione file precedente: " & ex.ToString())
                            MessageBox.Show("Errore durante l'eliminazione del file precedente: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        End Try
                    End If
                End If

                selectedRow.Cells("File").Value = newFileName
                dgvDocumenti.Refresh()
                Console.WriteLine("[DEBUG] DataGridView aggiornato con nuovo file.")

                WriteLogEntry("Modifica Allegato", $"Documento ID: {documentId}, File aggiornato: {newFileName}")
                MessageBox.Show("Il file è stato aggiornato con successo.", "Conferma", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                Console.WriteLine("[DEBUG] Selezione file annullata dall'utente.")
            End If
        End Using
    End Sub

    Private Sub UpdateDocumentFile(documentId As Integer, filePath As String)
        Console.WriteLine("[DEBUG] Avvio aggiornamento campo File per documento ID: " & documentId)

        Try
            Using connection As New SqliteConnection(connString)
                connection.Open()
                Console.WriteLine("[DEBUG] Connessione al database aperta.")

                Dim query As String = "UPDATE Documenti SET File = @File WHERE ID = @ID"
                Using command As New SqliteCommand(query, connection)
                    command.Parameters.AddWithValue("@File", filePath)
                    command.Parameters.AddWithValue("@ID", documentId)
                    command.ExecuteNonQuery()
                    Console.WriteLine("[DEBUG] Campo File aggiornato nel database con: " & filePath)
                End Using
            End Using

            WriteLogEntry("Aggiornamento File", $"Documento ID: {documentId}, nuovo file: {filePath}")
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore in UpdateDocumentFile: " & ex.ToString())
            MessageBox.Show("Errore durante l'aggiornamento del file nel database: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
            WriteLogEntry("Errore UpdateDocumentFile", ex.ToString())
        End Try
    End Sub


    Private Sub btnInserimentoGuidato_Click(sender As Object, e As EventArgs) Handles btnInserimentoGuidato.Click
        Console.WriteLine("[DEBUG] Avvio inserimento guidato.")

        Dim selectedUtenteID As Integer = GetSelectedUtenteID()
        If selectedUtenteID = 0 Then
            Console.WriteLine("[DEBUG] Nessun utente selezionato.")
            MessageBox.Show("Seleziona un pagamento e utente prima di procedere.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If String.IsNullOrEmpty(ComboBoxTipoPagamento.Text) Then
            Console.WriteLine("[DEBUG] TipoPagamento non selezionato.")
            Dim result As DialogResult = MessageBox.Show(
            "Devi selezionare un pagamento e un utente. Oppure clicca su 'Sì' per inserire manualmente un nuovo pagamento dopo aver selezionato solo l'utente.",
            "Pagamento mancante",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question
        )

            If result = DialogResult.Yes Then
                Dim nuovoPagamento As String = InputBox("Inserisci il nome del nuovo pagamento:", "Nuovo Pagamento")
                If Not String.IsNullOrEmpty(nuovoPagamento) Then
                    Console.WriteLine("[DEBUG] Nuovo pagamento inserito manualmente: " & nuovoPagamento)

                    Try
                        Using connection As New SqliteConnection(connString)
                            connection.Open()
                            Console.WriteLine("[DEBUG] Connessione al database aperta.")

                            Dim query As String = "INSERT INTO Documenti (TipoPagamento) VALUES (@TipoPagamento)"
                            Using command As New SqliteCommand(query, connection)
                                command.Parameters.AddWithValue("@TipoPagamento", nuovoPagamento)
                                command.ExecuteNonQuery()
                                Console.WriteLine("[DEBUG] Nuovo pagamento inserito nel database.")
                            End Using
                        End Using

                        PopulateComboBoxFromDatabase(ComboBoxTipoPagamento, "TipoPagamento")
                        ComboBoxTipoPagamento.Text = nuovoPagamento
                        MessageBox.Show("Nuovo pagamento creato con successo!", "Successo", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Catch ex As Exception
                        Console.WriteLine("[DEBUG] Errore inserimento nuovo pagamento: " & ex.ToString())
                        MessageBox.Show($"Errore durante la creazione del nuovo pagamento: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Try
                Else
                    Console.WriteLine("[DEBUG] Inserimento pagamento annullato.")
                    MessageBox.Show("Inserimento del pagamento annullato.", "Annullato", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End If

            Return
        End If

        Dim tipoPagamento As String = ComboBoxTipoPagamento.Text
        Console.WriteLine("[DEBUG] TipoPagamento selezionato: " & tipoPagamento)

        Dim inserimentoGuidatoForm As New FormInserimentoGuidato(connString, tipoPagamento, selectedUtenteID)
        inserimentoGuidatoForm.ShowDialog()
        Console.WriteLine("[DEBUG] FormInserimentoGuidato chiuso.")
        Me.Form3_Load(Nothing, Nothing)

    End Sub

    Private Sub picNotifica_Click(sender As Object, e As EventArgs) Handles picNotifica.Click
        Console.WriteLine("[DEBUG] Click su picNotifica: avvio filtro scadenze urgenti.")

        Dim now As DateTime = DateTime.Now
        Dim urgentTable As New DataTable()

        Dim originalTable As DataTable = TryCast(dgvDocumenti.DataSource, DataTable)
        If originalTable Is Nothing OrElse Not originalTable.Columns.Contains("Scadenza") Then
            Console.WriteLine("[DEBUG] Tabella originale non valida o campo 'Scadenza' assente.")
            Return
        End If

        urgentTable = originalTable.Clone()
        Dim righeTrovate As Integer = 0

        For i As Integer = 0 To dgvDocumenti.Rows.Count - 1
            Dim rowGrid = dgvDocumenti.Rows(i)
            Dim scadenzaStr As String = rowGrid.Cells("Scadenza").Value?.ToString()
            Dim statoCell = rowGrid.Cells("StatoPagamento")
            Dim isScaduto As Boolean = statoCell.Value IsNot Nothing AndAlso statoCell.Value.ToString().Trim().ToLower() = "scaduto"

            If Not isScaduto AndAlso Not String.IsNullOrEmpty(scadenzaStr) Then
                Dim scadenza As DateTime
                If DateTime.TryParse(scadenzaStr, scadenza) Then
                    Dim giorniRimanenti As Integer = (scadenza - now).Days
                    If giorniRimanenti <= 10 Then
                        urgentTable.ImportRow(originalTable.Rows(i))
                        righeTrovate += 1
                    End If
                End If
            End If
        Next

        If righeTrovate > 0 Then
            Dim view As DataView = urgentTable.DefaultView
            view.Sort = "Scadenza ASC"
            dgvDocumenti.DataSource = view.ToTable()
            Console.WriteLine("[DEBUG] Righe urgenti trovate: " & righeTrovate)
        Else
            Console.WriteLine("[DEBUG] Nessuna scadenza urgente trovata.")
            MessageBox.Show("Nessuna scadenza urgente trovata.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub picNotifica_MouseEnter(sender As Object, e As EventArgs) Handles picNotifica.MouseEnter
        Console.WriteLine("[DEBUG] Mouse sopra picNotifica.")
        picNotifica.Cursor = Cursors.Hand
    End Sub

    Private Sub picNotifica_MouseLeave(sender As Object, e As EventArgs) Handles picNotifica.MouseLeave
        Console.WriteLine("[DEBUG] Mouse fuori da picNotifica.")
        picNotifica.Cursor = Cursors.Default
    End Sub


    Private Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        Console.WriteLine("[DEBUG] Avvio esportazione documenti.")

        Dim startDate As Date = dtpStartDate.Value.Date
        Dim endDate As Date = dtpEndDate.Value.Date
        Console.WriteLine($"[DEBUG] Intervallo selezionato: {startDate:yyyy-MM-dd} → {endDate:yyyy-MM-dd}")

        Dim dt = GetMovimentiByDateRange(startDate, endDate)

        If dt.Rows.Count = 0 Then
            Console.WriteLine("[DEBUG] Nessun dato trovato nell'intervallo.")
            MessageBox.Show("Nessun dato nell'intervallo selezionato.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Using sfd As New SaveFileDialog
            sfd.Filter = "Excel (*.xlsx)|*.xlsx|CSV (*.csv)|*.csv"
            sfd.FileName = $"Documenti_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}"

            If sfd.ShowDialog = DialogResult.OK Then
                Try
                    If sfd.FilterIndex = 1 Then
                        Console.WriteLine("[DEBUG] Esportazione in formato Excel.")
                        Using wb As New XLWorkbook
                            wb.Worksheets.Add(dt, "Documenti")
                            wb.SaveAs(sfd.FileName)
                        End Using
                    Else
                        Console.WriteLine("[DEBUG] Esportazione in formato CSV.")
                        ExportDataTableToCSV(dt, sfd.FileName)
                    End If

                    Console.WriteLine("[DEBUG] Esportazione completata: " & sfd.FileName)
                    MessageBox.Show("Esportazione completata!", "Successo", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    WriteLogEntry("Esportazione", $"Intervallo: {startDate:yyyy-MM-dd} → {endDate:yyyy-MM-dd}, File: {sfd.FileName}")
                Catch ex As Exception
                    Console.WriteLine("[DEBUG] Errore esportazione: " & ex.ToString())
                    MessageBox.Show("Errore durante l'esportazione: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    WriteLogEntry("Errore esportazione", ex.ToString())
                End Try
            Else
                Console.WriteLine("[DEBUG] Esportazione annullata dall'utente.")
            End If
        End Using
    End Sub

    Private Function GetMovimentiByDateRange(startDate As Date, endDate As Date) As DataTable
        Console.WriteLine($"[DEBUG] Caricamento movimenti tra {startDate:yyyy-MM-dd} e {endDate:yyyy-MM-dd}")

        Dim dt As New DataTable()
        Dim databasePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath

        Try
            Using conn As New SqliteConnection(connString)
                conn.Open()
                Console.WriteLine("[DEBUG] Connessione al database aperta.")

                Dim query As String = "SELECT * FROM Documenti WHERE Data BETWEEN @startDate AND @endDate"
                Using cmd As New SqliteCommand(query, conn)
                    cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                    cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))

                    Using reader As SqliteDataReader = cmd.ExecuteReader()
                        dt.Load(reader)
                        Console.WriteLine("[DEBUG] Movimenti caricati: " & dt.Rows.Count)
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore in GetMovimentiByDateRange: " & ex.ToString())
            MessageBox.Show("Errore durante il caricamento dei movimenti: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
            WriteLogEntry("Errore GetMovimentiByDateRange", ex.ToString())
        End Try

        Return dt
    End Function

    Private Sub ExportDataTableToCSV(dt As DataTable, filePath As String)
        Dim sb As New StringBuilder()
        ' Intestazioni
        For Each col As DataColumn In dt.Columns
            sb.Append(col.ColumnName & ";")
        Next
        sb.AppendLine()
        ' Dati
        For Each row As DataRow In dt.Rows
            For Each col As DataColumn In dt.Columns
                sb.Append(row(col)?.ToString() & ";")
            Next
            sb.AppendLine()
        Next
        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8)
    End Sub



    Public Sub LoadUltimiDocumenti()
        Console.WriteLine("[DEBUG] Avvio caricamento ultimi 10 documenti.")

        Try
            Using connection As New SqliteConnection(connString)
                connection.Open()
                Console.WriteLine("[DEBUG] Connessione al database aperta.")

                ' Aggiungi UtenteID alla SELECT
                Dim query As String = "
                SELECT ID, UtenteID, Data, TipoPagamento, Importo, Note
                FROM Documenti
                ORDER BY Data DESC
                LIMIT 10;"

                Using command As New SqliteCommand(query, connection)
                    Using reader As SqliteDataReader = command.ExecuteReader()
                        Dim dt As New DataTable()
                        dt.Columns.Add("ID", GetType(Integer))
                        dt.Columns.Add("UtenteID", GetType(Integer))
                        dt.Columns.Add("Data", GetType(String))
                        dt.Columns.Add("TipoPagamento", GetType(String))
                        dt.Columns.Add("Importo", GetType(Double))
                        dt.Columns.Add("Note", GetType(String))

                        Dim rowCount As Integer = 0
                        While reader.Read()
                            Dim row As DataRow = dt.NewRow()
                            row("ID") = Convert.ToInt32(reader("ID"))

                            Dim utenteValue As Integer
                            If Integer.TryParse(reader("UtenteID").ToString(), utenteValue) Then
                                row("UtenteID") = utenteValue
                            Else
                                row("UtenteID") = 0
                            End If

                            row("Data") = reader("Data").ToString()
                            row("TipoPagamento") = reader("TipoPagamento").ToString()
                            row("Note") = reader("Note").ToString()

                            Dim importoValue As Double
                            If Double.TryParse(reader("Importo").ToString(), importoValue) Then
                                row("Importo") = importoValue
                            Else
                                row("Importo") = 0
                            End If

                            dt.Rows.Add(row)
                            rowCount += 1
                        End While

                        dgvUltimiDocumenti.DataSource = dt
                        Console.WriteLine("[DEBUG] Ultimi documenti caricati: " & rowCount)
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore in LoadUltimiDocumenti: " & ex.ToString())
            MessageBox.Show("Errore nel caricamento degli ultimi documenti: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub dgvUltimiDocumenti_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvUltimiDocumenti.CellDoubleClick
        Console.WriteLine("[DEBUG] Doppio click su dgvUltimiDocumenti.")

        If e.RowIndex < 0 Then
            Console.WriteLine("[DEBUG] Click su intestazione: nessuna azione.")
            Exit Sub
        End If

        Try
            Dim row As DataGridViewRow = dgvUltimiDocumenti.Rows(e.RowIndex)

            ' Lettura diretta dalle celle (senza usare SelectedColumns/SelectedRows e senza parsing da stringa)
            If Not dgvUltimiDocumenti.Columns.Contains("ID") Then
                MessageBox.Show("Colonna 'ID' non trovata nella tabella.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Console.WriteLine("[DEBUG] Colonna 'ID' assente.")
                Exit Sub
            End If

            Dim id As Integer = Convert.ToInt32(row.Cells("ID").Value)

            Dim tipoPagamento As String = ""
            If dgvUltimiDocumenti.Columns.Contains("TipoPagamento") AndAlso row.Cells("TipoPagamento").Value IsNot Nothing Then
                tipoPagamento = row.Cells("TipoPagamento").Value.ToString()
            End If

            Dim utenteId As Integer = 0
            If dgvUltimiDocumenti.Columns.Contains("UtenteID") AndAlso row.Cells("UtenteID").Value IsNot Nothing Then
                Dim tmp As Integer
                If Integer.TryParse(row.Cells("UtenteID").Value.ToString(), tmp) Then utenteId = tmp
            End If

            Console.WriteLine($"[DEBUG] ID={id}, TipoPagamento={tipoPagamento}, UtenteID={utenteId}")

            Dim result As DialogResult = MessageBox.Show(
                $"Vuoi aprire la scheda per il pagamento: {tipoPagamento} (ID {id}, Utente {utenteId})?",
                "Conferma",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            )

            If result = DialogResult.Yes Then
                Console.WriteLine("[DEBUG] Conferma ricevuta. Apro FormInserimentoGuidato e FormNuovoDocumento.")
                ' Passo anche l'ID al form guidato (così carica la riga specifica se implementato)
                Dim nuovoForm As New FormInserimentoGuidato(connString, tipoPagamento, utenteId, id)
                nuovoForm.Show()

                Dim modificaForm As New FormNuovoDocumento(connString, tipoPagamento, utenteId, id)
                modificaForm.ShowDialog()
            Else
                Console.WriteLine("[DEBUG] Apertura annullata dall'utente.")
            End If

        Catch ex As Exception
            MessageBox.Show("Errore nell'apertura del documento: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Console.WriteLine("[DEBUG] Errore apertura documento: " & ex.ToString())
        End Try
    End Sub


    Private Sub dgvDocumenti_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvDocumenti.CellDoubleClick
        Console.WriteLine("[DEBUG] Doppio click su dgvDocumenti.")

        If e.RowIndex < 0 Then
            Console.WriteLine("[DEBUG] Click su intestazione: nessuna azione.")
            Exit Sub
        End If

        Try
            Dim row As DataGridViewRow = dgvDocumenti.Rows(e.RowIndex)

            ' Lettura diretta dalle celle (senza usare SelectedColumns/SelectedRows e senza parsing da stringa)
            If Not dgvUltimiDocumenti.Columns.Contains("ID") Then
                MessageBox.Show("Colonna 'ID' non trovata nella tabella.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Console.WriteLine("[DEBUG] Colonna 'ID' assente.")
                Exit Sub
            End If

            Dim id As Integer = Convert.ToInt32(row.Cells("ID").Value)

            Dim tipoPagamento As String = ""
            If dgvUltimiDocumenti.Columns.Contains("TipoPagamento") AndAlso row.Cells("TipoPagamento").Value IsNot Nothing Then
                tipoPagamento = row.Cells("TipoPagamento").Value.ToString()
            End If

            Dim utenteId As Integer = 0
            If dgvUltimiDocumenti.Columns.Contains("UtenteID") AndAlso row.Cells("UtenteID").Value IsNot Nothing Then
                Dim tmp As Integer
                If Integer.TryParse(row.Cells("UtenteID").Value.ToString(), tmp) Then utenteId = tmp
            End If

            Console.WriteLine($"[DEBUG] ID={id}, TipoPagamento={tipoPagamento}, UtenteID={utenteId}")

            Dim result As DialogResult = MessageBox.Show(
                $"Vuoi aprire la scheda per il pagamento: {tipoPagamento} (ID {id}, Utente {utenteId})?",
                "Conferma",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            )

            If result = DialogResult.Yes Then
                Console.WriteLine("[DEBUG] Conferma ricevuta. Apro FormInserimentoGuidato e FormNuovoDocumento.")
                ' Passo anche l'ID al form guidato (così carica la riga specifica se implementato)
                Dim nuovoForm As New FormInserimentoGuidato(connString, tipoPagamento, utenteId, id)
                nuovoForm.Show()

                Dim modificaForm As New FormNuovoDocumento(connString, tipoPagamento, utenteId, id)
                modificaForm.ShowDialog()
            Else
                Console.WriteLine("[DEBUG] Apertura annullata dall'utente.")
            End If

        Catch ex As Exception
            MessageBox.Show("Errore nell'apertura del documento: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Console.WriteLine("[DEBUG] Errore apertura documento: " & ex.ToString())
        End Try
    End Sub



    ' Metodo pubblico per aggiornare le notifiche (lblNotifica)
    Public Sub UpdateNotifications()
        Console.WriteLine("[DEBUG] Aggiornamento notifiche tramite UpdateNotifications.")
        Try
            TimerScadenze_Tick(Nothing, Nothing)
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore in UpdateNotifications: " & ex.ToString())
        End Try
    End Sub

    Public Sub CaricaRiepilogoScadenze()
        Console.WriteLine("[DEBUG] Avvio CaricaRiepilogoScadenze (Form3).")

        Try
            Using connection As New SqliteConnection(connString)
                connection.Open()
                Console.WriteLine("[DEBUG] Connessione al database aperta per riepilogo scadenze.")

                Dim query As String = "SELECT ID, Note, Scadenza, TipoPagamento, UtenteID FROM Documenti WHERE Scadenza IS NOT NULL AND Scadenza >= @Oggi ORDER BY Scadenza ASC"

                Using command As New SqliteCommand(query, connection)
                    command.Parameters.AddWithValue("@Oggi", DateTime.Now.ToString("yyyy-MM-dd"))

                    Using reader As SqliteDataReader = command.ExecuteReader()
                        ' Cerca il controllo ListBox in tutta la gerarchia dei controlli
                        Dim found() As Control = Me.Controls.Find("lstRiepilogoScadenze", True)
                        Dim lst As ListBox = Nothing
                        If found.Length > 0 AndAlso TypeOf found(0) Is ListBox Then
                            lst = DirectCast(found(0), ListBox)
                            lst.Items.Clear()
                        End If

                        Dim count As Integer = 0
                        While reader.Read()
                            Dim id As Integer = Convert.ToInt32(reader("ID"))
                            Dim nota As String = If(reader.IsDBNull(reader.GetOrdinal("Note")), "", reader("Note").ToString())
                            Dim scadenza As DateTime = Convert.ToDateTime(reader("Scadenza"))
                            Dim tipoPagamento As String = If(reader.IsDBNull(reader.GetOrdinal("TipoPagamento")), "", reader("TipoPagamento").ToString())
                            Dim utenteId As Integer = If(reader.IsDBNull(reader.GetOrdinal("UtenteID")), 0, Convert.ToInt32(reader("UtenteID")))
                            Dim giorniRimanenti As Integer = (scadenza - DateTime.Now).Days

                            Dim riepilogo As String = $"{id} - {scadenza:dd/MM/yyyy} - {nota} ({giorniRimanenti} giorni rimanenti) - {tipoPagamento} - Utente: {utenteId}"

                            If lst IsNot Nothing Then
                                lst.Items.Add(riepilogo)
                            Else
                                Console.WriteLine("[DEBUG] Riepilogo scadenza: " & riepilogo)
                            End If

                            count += 1
                        End While

                        Console.WriteLine("[DEBUG] Righe riepilogo scadenze lette: " & count)
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore CaricaRiepilogoScadenze: " & ex.ToString())
            MessageBox.Show("Errore durante il caricamento del riepilogo delle scadenze: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub
End Class
