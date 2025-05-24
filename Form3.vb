Imports System.Data.SQLite
Imports System.IO
Imports System.Text
Imports ClosedXML.Excel


Public Class Form3
    ' Percorso della cartella in cui verranno memorizzati i documenti
    Private documentsFolderPath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "Documenti")

    ' Percorso del database
    Private databaseFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
    Private databasePath As String = Path.Combine(databaseFolder, "amministrazione.db")
    Private connString As String = "Data Source=" & databasePath & ";Version=3;"

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
        ' Aggiorna i dati nella DataGridView
        LoadDataIntoDataGridView(keywordSearch, startDate, endDate, documentType, GetSelectedUtenteID(), TextBoxAnno.Text.Trim(), ComboBoxNote.Text.Trim())
        HighlightDatesWithDocuments()
    End Sub


    Private Sub Form3_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Imposta i valori iniziali per il caricamento
        Dim keywordSearch As String = ""
        Dim startDate As Date = Date.MinValue
        Dim endDate As Date = Date.MaxValue
        Dim documentType As String = ""
        Dim anno As String = ""
        Dim note As String = ""
        Dim scadenza As String = ""


        ' Aggiungi un gestore per l'evento SelectedIndexChanged della ComboBox
        AddHandler ComboBoxTipoPagamento.SelectedIndexChanged, AddressOf ComboBoxTipoPagamento_SelectedIndexChanged



        ' Verifica se la cartella dei documenti esiste, altrimenti creala
        If Not Directory.Exists(documentsFolderPath) Then
            Directory.CreateDirectory(documentsFolderPath)
        End If

        ' Verifica se la tabella "Documenti" esiste nel database, altrimenti creala
        CreateDocumentiTableIfNotExists()


        ' Popolare le ComboBox con i dati dal database
        PopulateComboBoxFromDatabase(ComboBoxTipoPagamento, "TipoPagamento")
        PopulateComboBoxFromDatabase(ComboBoxNote, "Note")


        ' Chiamare la funzione per evidenziare le date con documenti
        HighlightDatesWithDocuments()


        SetTabOrder()
        LoadDataIntoDataGridView(keywordSearch, startDate, endDate, documentType, GetSelectedUtenteID(), TextBoxAnno.Text.Trim(), ComboBoxNote.Text.Trim())

        contextMenuStrip1.Items.Add(eliminaScadenzaToolStripMenuItem)

    End Sub

    Private Sub btnReset_Click(sender As Object, e As EventArgs) Handles btnReset.Click
        ' Imposta i valori iniziali per il caricamento
        Dim keywordSearch As String = ""
        Dim startDate As Date = Date.MinValue
        Dim endDate As Date = Date.MaxValue
        Dim documentType As String = ""
        Dim anno As String = ""
        Dim note As String = ""
        Dim scadenza As String = ""


        LoadDataIntoDataGridView(keywordSearch, startDate, endDate, documentType, GetSelectedUtenteID(), TextBoxAnno.Text.Trim(), ComboBoxNote.Text.Trim())
        HighlightDatesWithDocuments()
    End Sub

    Private Sub ResetUtenteRadioButton()
        ' Resetta i RadioButton degli utenti
        rbtnUtente1.Checked = False
        rbtnUtente2.Checked = False
        rbtnUtente3.Checked = False
    End Sub


    Private Sub ComboBoxTipoPagamento_SelectedIndexChanged(sender As Object, e As EventArgs)
        ' Aggiorna il valore selezionato quando cambia la selezione nella ComboBox
        selectedTipoPagamento = ComboBoxTipoPagamento.Text
    End Sub


    Private Sub PopulateComboBoxFromDatabase(comboBox As ComboBox, columnName As String)
        Try
            Using connection As New SQLiteConnection(connString)
                connection.Open()

                Dim query As String = $"SELECT DISTINCT {columnName} FROM Documenti ORDER BY {columnName} ASC"
                Using command As New SQLiteCommand(query, connection)
                    Using reader As SQLiteDataReader = command.ExecuteReader()
                        comboBox.Items.Clear()
                        While reader.Read()
                            If Not reader.IsDBNull(0) Then
                                comboBox.Items.Add(reader.GetString(0))
                            End If
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show($"Si è verificato un errore durante il caricamento dei dati dalla colonna '{columnName}'.")
        End Try
    End Sub
    Private Sub CreateDocumentiTableIfNotExists()
        Try
            Using connection As New SQLiteConnection(connString)
                connection.Open()

                ' Query SQL per creare la tabella "Documenti" se non esiste
                Dim createTableQuery As String = "CREATE TABLE IF NOT EXISTS Documenti (ID INTEGER PRIMARY KEY AUTOINCREMENT, UtenteID INTEGER, Data DATE, AnnoRiferimento INTEGER, TipoPagamento TEXT, Importo REAL, File TEXT, Note TEXT, Scadenza DATE, StatoPagamento TEXT);"

                Using command As New SQLiteCommand(createTableQuery, connection)
                    command.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Si è verificato un errore durante la creazione della tabella 'Documenti' nel database.")
        End Try
    End Sub


    Private Sub btnSfogliaFile_Click(sender As Object, e As EventArgs)
        ' Mostra una finestra di dialogo per selezionare qualsiasi tipo di file
        Using openFileDialog As New OpenFileDialog
            openFileDialog.Filter = "Tutti i file|*.*"
            If openFileDialog.ShowDialog = DialogResult.OK Then
                Dim selectedFilePath = openFileDialog.FileName
                Dim selectedFileName = Path.GetFileName(selectedFilePath)

                ' Verifica se il nome del file contiene spazi
                If selectedFileName.Contains(" ") Then
                    MessageBox.Show("Il nome del file non deve contenere spazi. Rinomina il file e riprova.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If

                ' Visualizza il percorso del file nella TextBox
                'txtNomeFile.Text = selectedFilePath
            End If
        End Using
    End Sub

    Private Sub WriteLogEntry(action As String, details As String)
        Try
            ' Percorso del file di log
            Dim logFilePath As String = Path.Combine(documentsFolderPath, "log.txt")

            ' Assicurati che la directory esista
            If Not Directory.Exists(documentsFolderPath) Then
                Directory.CreateDirectory(documentsFolderPath)
            End If

            ' Testo del log
            Dim logEntry As String = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {action} | {details}"

            ' Controlla la dimensione del file di log (ad esempio 5 MB)
            Const maxLogFileSize As Long = 5 * 1024 * 1024 ' 5 MB in byte
            If File.Exists(logFilePath) AndAlso New FileInfo(logFilePath).Length > maxLogFileSize Then
                ' Rinomina il file di log esistente
                Dim archivedLogPath As String = Path.Combine(documentsFolderPath, $"log_{DateTime.Now:yyyyMMddHHmmss}.txt")
                File.Move(logFilePath, archivedLogPath)
            End If

            ' Scrivi l'entry nel file di log
            Using writer As New StreamWriter(logFilePath, append:=True)
                writer.WriteLine(logEntry)
            End Using
        Catch ex As Exception
            ' Gestione degli errori di scrittura log
            MessageBox.Show($"Errore durante la scrittura del log: {ex.Message}", "Errore Log", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' Aggiorna il gestore dell'evento btnSearch_Click
    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        ' Aggiorna i valori dei filtri

        Dim documentType As String = ComboBoxTipoPagamento.Text
        Dim startDate As Date = Date.MinValue
        Dim endDate As Date = Date.MaxValue
        Dim anno As String = ""
        Dim note As String = ""

        ' Ottieni l'ID dell'utente selezionato
        Dim selectedUtenteID As Integer = GetSelectedUtenteID()

        ' Carica i dati nel DataGridView con i nuovi filtri
        LoadDataIntoDataGridView(keywordSearch, startDate, endDate, documentType, selectedUtenteID, TextBoxAnno.Text.Trim(), ComboBoxNote.Text.Trim())
    End Sub

    Public Sub ResetFormControls()


        ' Resettare le TextBox
        'txtNomeFile.Clear()
        TextBoxAnno.Clear()
        TextBoxImporto.Clear()

        ' Resettare le ComboBox
        ComboBoxTipoPagamento.Text = "" ' Seleziona nessun elemento
        ComboBoxNote.Text = ""

        ' Resettare i RadioButtons
        rbtnUtente1.Checked = False
        rbtnUtente2.Checked = False
        rbtnUtente3.Checked = False

        ' Se hai altre TextBox, ComboBox o RadioButtons, aggiungile qui...



        ' Imposta il focus su una specifica controllo, se necessario (ad esempio sulla prima TextBox o ComboBox)
        TextBoxAnno.Focus()
    End Sub

    Private Function InsertDocument(filePath As String) As Boolean
        Try
            Using connection As New SQLiteConnection(connString)
                connection.Open()

                ' Verifica se il file specificato esiste
                If Not File.Exists(filePath) Then
                    MessageBox.Show("Il file specificato non esiste.")
                    Return False
                End If

                Dim documentFileName As String = Path.GetFileName(filePath)

                ' Verifica se il file esiste già nella cartella
                If File.Exists(Path.Combine(documentsFolderPath, documentFileName)) Then
                    MessageBox.Show("Il file esiste già.")
                    Return False ' Esci dalla procedura se il file esiste già
                End If

                ' Copia il documento nella cartella dei documenti
                Dim destinationPath As String = Path.Combine(documentsFolderPath, documentFileName)
                File.Copy(filePath, destinationPath)

                ' Inserisci i dati del documento nella tabella "Documenti"
                Dim query As String = "INSERT INTO Documenti (UtenteID, Data, AnnoRiferimento, TipoPagamento, Importo, File, Note)"

                Using command As New SQLiteCommand(query, connection)
                    command.Parameters.AddWithValue("@UtenteID", GetSelectedUtenteID())
                    ' Usa il formato completo della data con orario
                    command.Parameters.AddWithValue("@Data", MonthCalendar1.SelectionStart.ToString("yyyy-MM-dd HH:mm:ss"))
                    command.Parameters.AddWithValue("@AnnoRiferimento", TextBoxAnno.Text)
                    command.Parameters.AddWithValue("@TipoPagamento", ComboBoxTipoPagamento.Text)
                    command.Parameters.AddWithValue("@Importo", TextBoxImporto.Text)
                    command.Parameters.AddWithValue("@File", documentFileName)
                    command.Parameters.AddWithValue("@Note", ComboBoxNote.Text)
                    ' Aggiungi Scadenza solo se il flag è attivo


                    command.ExecuteNonQuery()
                End Using

                Return True
            End Using
        Catch ex As Exception
            MessageBox.Show("Si è verificato un errore durante l'inserimento del documento nel database: " & ex.Message)
            Return False
        End Try
    End Function

    Private Sub LoadDataIntoDataGridView(keyword As String, start As Date, [end] As Date, type As String, utenteID As Integer, anno As String, note As String)
        Try
            Using connection As New SQLiteConnection(connString)
                connection.Open()

                ' Costruisci la query SQL dinamicamente per includere Scadenza solo se il flag è attivo
                Dim query As String = "SELECT * FROM Documenti WHERE (TipoPagamento = @Type OR @Type = '') AND " &
                                  "(Note LIKE '%' || @Keyword || '%' OR @Keyword = '') AND " &
                                  "(Data BETWEEN @StartDate AND @EndDate)  AND " &
                                  "(UtenteID = @UtenteID OR @UtenteID = 0) AND " &
                                  "(AnnoRiferimento = @AnnoRiferimento OR @AnnoRiferimento = '') AND " &
                                  "(Note = @Note OR @Note = '')"

                ' Aggiungi la condizione Scadenza alla query solo se il flag è attivo


                Using command As New SQLiteCommand(query, connection)
                    ' Aggiungi i parametri della query con il formato della data corretto
                    command.Parameters.AddWithValue("@Type", type)
                    command.Parameters.AddWithValue("@StartDate", start.ToString("yyyy-MM-dd HH:mm:ss"))
                    command.Parameters.AddWithValue("@EndDate", [end].ToString("yyyy-MM-dd HH:mm:ss"))
                    command.Parameters.AddWithValue("@Keyword", keyword)
                    command.Parameters.AddWithValue("@UtenteID", utenteID)
                    command.Parameters.AddWithValue("@AnnoRiferimento", anno)
                    command.Parameters.AddWithValue("@Note", note)

                    ' Aggiungi il parametro Scadenza solo se il flag è attivo


                    ' Esegui la query e popola il DataGridView
                    Using da As New SQLiteDataAdapter(command)
                        Dim dt As New DataTable()
                        da.Fill(dt)
                        dgvDocumenti.DataSource = dt
                    End Using
                End Using

                ' Aggiungi colonne di azione al DataGridView solo se non esistono già
                If Not dgvDocumenti.Columns.Contains("Modifica") Then
                    Dim editButton As New DataGridViewButtonColumn()
                    editButton.Name = "Modifica"
                    editButton.HeaderText = "Modifica"
                    editButton.Text = "Modifica"
                    editButton.UseColumnTextForButtonValue = True
                    dgvDocumenti.Columns.Add(editButton)
                End If

                If Not dgvDocumenti.Columns.Contains("Elimina") Then
                    Dim deleteButton As New DataGridViewButtonColumn()
                    deleteButton.Name = "Elimina"
                    deleteButton.HeaderText = "Elimina"
                    deleteButton.Text = "Elimina"
                    deleteButton.UseColumnTextForButtonValue = True
                    dgvDocumenti.Columns.Add(deleteButton)
                End If

                If Not dgvDocumenti.Columns.Contains("Apri File") Then
                    Dim openFileButton As New DataGridViewButtonColumn()
                    openFileButton.Name = "Apri File"
                    openFileButton.HeaderText = "Apri File"
                    openFileButton.Text = "Apri File"
                    openFileButton.UseColumnTextForButtonValue = True
                    dgvDocumenti.Columns.Add(openFileButton)
                End If

            End Using
        Catch ex As Exception
            MessageBox.Show("Si è verificato un errore durante il caricamento dei dati nel DataGridView.")
        End Try
    End Sub

    Private Sub dgvDocumenti_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvDocumenti.CellClick
        If e.RowIndex < 0 Then
            ' Clicked on header, do nothing
            Return
        End If

        Dim selectedRow As DataGridViewRow = dgvDocumenti.Rows(e.RowIndex)

        ' Check which button was clicked
        If e.ColumnIndex = dgvDocumenti.Columns("Modifica").Index Then
            ' Handle edit logic
            EditDocument(selectedRow)
        ElseIf e.ColumnIndex = dgvDocumenti.Columns("Elimina").Index Then
            ' Handle delete logic
            DeleteDocument(selectedRow)
        ElseIf e.ColumnIndex = dgvDocumenti.Columns("Apri File").Index Then
            ' Handle open file logic
            OpenDocumentFile(selectedRow)
        End If
    End Sub

    Private Sub EditDocument(row As DataGridViewRow)
        If row Is Nothing Then
            MessageBox.Show("Seleziona una riga valida.")
            Return
        End If

        Dim result As DialogResult = MessageBox.Show("Sei sicuro di voler modificare questa riga?", "Conferma modifica", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If result = DialogResult.Yes Then
            Dim documentFileName As String = row.Cells("File").Value.ToString()

            ' Verifica se il nome del file contiene spazi
            If documentFileName.Contains(" ") Then
                MessageBox.Show("Il nome del file non deve contenere spazi. Rinomina il file e riprova.")
                Return
            End If

            ' Imposta tutte le celle della riga selezionata come editabili
            For Each cell As DataGridViewCell In row.Cells
                cell.ReadOnly = False
            Next

            ' Registra il log
            Dim logDetails As String = $"Modifica documento ID: {row.Cells("ID").Value}, File: {documentFileName}, Note precedenti: {row.Cells("Note").Value}"
            WriteLogEntry("Modifica", logDetails)

            ' Ricarica il DataGridView
            LoadDataIntoDataGridView(keywordSearch, startDate, endDate, documentType, GetSelectedUtenteID(), TextBoxAnno.Text.Trim(), ComboBoxNote.Text.Trim())

            SetTabOrder()
            HighlightDatesWithDocuments()

            ' Popolare le ComboBox con i dati dal database
            PopulateComboBoxFromDatabase(ComboBoxTipoPagamento, "TipoPagamento")
            PopulateComboBoxFromDatabase(ComboBoxNote, "Note")
        End If
    End Sub
    Private Sub DeleteDocument(row As DataGridViewRow)
        If row Is Nothing Then
            MessageBox.Show("Seleziona una riga valida.")
            Return
        End If

        Dim result As DialogResult = MessageBox.Show("Sei sicuro di voler eliminare questa riga?", "Conferma eliminazione", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

        If result = DialogResult.Yes Then
            Dim documentId As Integer = Convert.ToInt32(row.Cells("ID").Value)
            Dim fileName As String = row.Cells("File").Value.ToString()

            ' Elimina il file dal filesystem
            Dim fullPath As String = Path.Combine(documentsFolderPath, fileName)
            If File.Exists(fullPath) Then
                File.Delete(fullPath)
            End If

            ' Elimina il record dal database
            Using connection As New SQLiteConnection(connString)
                connection.Open()
                Dim deleteQuery As String = "DELETE FROM Documenti WHERE ID = @ID"
                Using command As New SQLiteCommand(deleteQuery, connection)
                    command.Parameters.AddWithValue("@ID", documentId)
                    command.ExecuteNonQuery()
                End Using
            End Using

            ' Registra il log
            Dim logDetails As String = $"Eliminazione documento ID: {documentId}, File: {fileName}"
            WriteLogEntry("Eliminazione", logDetails)

            ' Ricarica il DataGridView
            LoadDataIntoDataGridView(keywordSearch, startDate, endDate, documentType, GetSelectedUtenteID(), TextBoxAnno.Text.Trim(), ComboBoxNote.Text.Trim())

            SetTabOrder()
            HighlightDatesWithDocuments()

            ' Popolare le ComboBox con i dati dal database
            PopulateComboBoxFromDatabase(ComboBoxTipoPagamento, "TipoPagamento")
            PopulateComboBoxFromDatabase(ComboBoxNote, "Note")
        End If
    End Sub

    Private Sub OpenDocumentFile(row As DataGridViewRow)
        If row Is Nothing OrElse row.Cells("File").Value Is Nothing Then
            MessageBox.Show("Nessun file associato a questa riga.")
            Return
        End If

        Dim fileName As String = row.Cells("File").Value.ToString()

        If String.IsNullOrEmpty(fileName) Then
            MessageBox.Show("Nome file non valido.")
            Return
        End If

        Dim fullPath As String = Path.Combine(documentsFolderPath, fileName)

        ' Verifica l'esistenza del file
        If File.Exists(fullPath) Then
            Try
                Dim psi As New ProcessStartInfo()
                psi.UseShellExecute = True
                psi.FileName = fullPath
                Process.Start(psi)
            Catch winEx As System.ComponentModel.Win32Exception
                MessageBox.Show("Non c'è un'applicazione predefinita associata a questo tipo di file. Seleziona un'applicazione predefinita per questo tipo di file nel sistema operativo e riprova.")
            Catch ex As Exception
                MessageBox.Show("Errore nell'apertura del file. Dettagli: " & ex.Message)
            End Try
        Else
            MessageBox.Show("File non trovato.")
        End If

    End Sub

    Private Sub dgvDocumenti_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles dgvDocumenti.CellEndEdit
        Dim column As DataGridViewColumn = dgvDocumenti.Columns(e.ColumnIndex)
        Dim row As DataGridViewRow = dgvDocumenti.Rows(e.RowIndex)

        Dim documentId As Integer = Convert.ToInt32(row.Cells("ID").Value)
        Dim newValue As Object = row.Cells(e.ColumnIndex).Value

        ' Aggiorna il record nel database in base al nome della colonna modificata
        Using connection As New SQLiteConnection(connString)
            connection.Open()
            Dim updateQuery As String = $"UPDATE Documenti SET {column.Name} = @Value WHERE ID = @ID"

            Using command As New SQLiteCommand(updateQuery, connection)
                command.Parameters.AddWithValue("@Value", newValue)
                command.Parameters.AddWithValue("@ID", documentId)
                command.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Private Sub SetTabOrder()
        TextBoxAnno.TabIndex = 0
        ComboBoxTipoPagamento.TabIndex = 1
        TextBoxImporto.TabIndex = 2
        ComboBoxNote.TabIndex = 3
        'txtNomeFile.TabIndex = 4
        'btnSfogliaFile.TabIndex = 5
        ' btnInserisciDati.TabIndex = 6
    End Sub

    Private Function GetSelectedUtenteID() As Integer
        ' Restituisce l'ID dell'utente selezionato
        If rbtnUtente1.Checked Then
            Return 1
        ElseIf rbtnUtente2.Checked Then
            Return 2
        ElseIf rbtnUtente3.Checked Then
            Return 3
        Else
            Return 0 ' Nessun utente selezionato
        End If
    End Function

    Public Sub HighlightDatesWithDocuments()
        Try
            ' Clear previous bolded dates
            ClearBoldedDates()

            Using connection As New SQLiteConnection(connString)
                connection.Open()

                Dim query As String = "SELECT DISTINCT Data FROM Documenti"
                Using command As New SQLiteCommand(query, connection)
                    Using reader As SQLiteDataReader = command.ExecuteReader()
                        While reader.Read()
                            If Not reader.IsDBNull(0) Then
                                ' Usa il formato completo della data con orario
                                Dim documentDate As Date = reader.GetDateTime(0)
                                MonthCalendar1.AddBoldedDate(documentDate)
                            End If
                        End While
                    End Using
                End Using
            End Using

            ' Update bolded dates
            MonthCalendar1.UpdateBoldedDates()

        Catch ex As Exception
            MessageBox.Show("Si è verificato un errore durante il caricamento delle date con documenti.")
        End Try
    End Sub

    Private Sub MonthCalendar1_DateChanged(sender As Object, e As DateRangeEventArgs) Handles MonthCalendar1.DateChanged
        ' Aggiorna la data selezionata nel filtro e carica i dati corrispondenti nel DataGridView
        startDate = MonthCalendar1.SelectionStart.Date
        endDate = MonthCalendar1.SelectionEnd.Date

        ' Aggiungi un filtro specifico per la data selezionata
        LoadDataIntoDataGridView(keywordSearch, startDate, endDate, documentType, GetSelectedUtenteID(), TextBoxAnno.Text.Trim(), ComboBoxNote.Text.Trim())
    End Sub

    Private Sub ClearBoldedDates()
        ' Clear all bolded dates
        MonthCalendar1.RemoveAllBoldedDates()
        MonthCalendar1.UpdateBoldedDates()
    End Sub

    Private Sub btnOpenForm3_Click(sender As Object, e As EventArgs) Handles btnOpenForm3.Click
        Me.Hide()
        Form1.Show()
    End Sub

    Private Sub rbtnUtente1_CheckedChanged(sender As Object, e As EventArgs)

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim nuovaForm As New Form5
        nuovaForm.Show()
        Me.Close()
    End Sub

    Private Sub TimerScadenze_Tick(sender As Object, e As EventArgs) Handles TimerScadenze.Tick
        Dim now As DateTime = DateTime.Now
        Dim righeScadute As Integer = 0

        For Each row As DataGridViewRow In dgvDocumenti.Rows
            Dim scadenzaCell = row.Cells("Scadenza")
            Dim statoCell = row.Cells("StatoPagamento")
            Dim scadenzaStr As String = scadenzaCell.Value?.ToString()

            ' Escludi le righe con StatoPagamento = "Scaduto"
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
                End If
            Else
                scadenzaCell.Style.BackColor = Color.White
            End If
        Next

        ' Aggiorna lblNotifica e picNotifica
        If righeScadute > 0 Then
            lblNotifica.Text = righeScadute.ToString()
            lblNotifica.Visible = True
            picNotifica.Visible = True
        Else
            lblNotifica.Visible = False
            picNotifica.Visible = False
        End If
        lblNotifica.Font = New Font(lblNotifica.Font, FontStyle.Bold)
        lblNotifica.ForeColor = Color.Red
        lblNotifica.Text = righeScadute.ToString()
        lblNotifica.Visible = (righeScadute > 0)
    End Sub
    ' Gestisci l'evento clic dell'elemento del menu "Elimina Scadenza"
    Private Sub eliminaScadenzaToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles eliminaScadenzaToolStripMenuItem.Click
        Dim selectedRow As DataGridViewRow = dgvDocumenti.CurrentRow
        If selectedRow IsNot Nothing Then
            ' Rimuovi la data di scadenza
            selectedRow.Cells("Scadenza").Value = DBNull.Value
            ' Salva la modifica nel database
            UpdateScadenzaInDatabase(selectedRow.Index, Nothing)
            MessageBox.Show("La scadenza è stata eliminata con successo.")
        End If
    End Sub


    Private Sub UpdateScadenzaInDatabase(rowIndex As Integer, scadenza As DateTime?)
        Using connection As New SQLiteConnection(connString)
            connection.Open()
            Dim query As String = "UPDATE Documenti SET Scadenza = @Scadenza WHERE Id = @Id"
            Using command As New SQLiteCommand(query, connection)
                command.Parameters.AddWithValue("@Scadenza", If(scadenza, DBNull.Value))
                command.Parameters.AddWithValue("@Id", dgvDocumenti.Rows(rowIndex).Cells("Id").Value)
                command.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Private Sub dgvDocumenti_CellMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles dgvDocumenti.CellMouseClick
        If e.Button = MouseButtons.Right AndAlso e.RowIndex >= 0 Then
            dgvDocumenti.ClearSelection()
            dgvDocumenti.Rows(e.RowIndex).Selected = True

            Dim selectedRow As DataGridViewRow = dgvDocumenti.Rows(e.RowIndex)
            Dim documentId As Integer = selectedRow.Cells("Id").Value

            ' Crea un menu contestuale
            Dim contextMenu As New ContextMenuStrip()
            Dim modifyAttachmentMenuItem As New ToolStripMenuItem("Modifica Allegato")
            Dim setDeadlineMenuItem As New ToolStripMenuItem("Imposta Scadenza")
            Dim setExpiredMenuItem As New ToolStripMenuItem("Imposta scaduto") ' Nuova voce

            ' Handler per "Modifica Allegato"
            AddHandler modifyAttachmentMenuItem.Click, Sub()
                                                           ModifyAttachment(selectedRow, documentId)
                                                       End Sub

            ' Handler per "Imposta Scadenza"
            AddHandler setDeadlineMenuItem.Click, Sub()
                                                      SetDeadline(selectedRow, selectedRow.Index)
                                                  End Sub

            ' Handler per "Imposta scaduto"
            AddHandler setExpiredMenuItem.Click, Sub()
                                                     SetScaduto(selectedRow)
                                                 End Sub

            contextMenu.Items.Add(modifyAttachmentMenuItem)
            contextMenu.Items.Add(setDeadlineMenuItem)
            contextMenu.Items.Add(setExpiredMenuItem) ' Aggiungi la nuova voce

            contextMenu.Show(Cursor.Position)
        End If
    End Sub

    Private Sub SetScaduto(selectedRow As DataGridViewRow)
        ' Imposta lo sfondo bianco della cella Scadenza
        selectedRow.Cells("Scadenza").Style.BackColor = Color.White

        ' Aggiorna StatoPagamento nel database
        Dim documentId As Integer = selectedRow.Cells("Id").Value
        Using connection As New SQLiteConnection(connString)
            connection.Open()
            Dim query As String = "UPDATE Documenti SET StatoPagamento = @Stato WHERE Id = @Id"
            Using command As New SQLiteCommand(query, connection)
                command.Parameters.AddWithValue("@Stato", "Scaduto")
                command.Parameters.AddWithValue("@Id", documentId)
                command.ExecuteNonQuery()
            End Using
        End Using

        ' Aggiorna la cella nella DataGridView
        If selectedRow.DataGridView.Columns.Contains("StatoPagamento") Then
            selectedRow.Cells("StatoPagamento").Value = "Scaduto"
        End If

        ' Aggiorna la notifica
        TimerScadenze_Tick(Nothing, Nothing)
    End Sub
    Private Sub SetDeadline(selectedRow As DataGridViewRow, rowIndex As Integer)
        Dim currentDeadline As DateTime? = Nothing

        ' Ottieni il valore esistente dalla DataGridView
        If Not IsDBNull(selectedRow.Cells("Scadenza").Value) Then
            Dim tempDate As DateTime ' Variabile temporanea
            If DateTime.TryParse(selectedRow.Cells("Scadenza").Value.ToString(), tempDate) Then
                currentDeadline = tempDate
            End If
        End If

        ' Apri il modulo per impostare la scadenza
        Using deadlineForm As New SetDeadlineForm()
            deadlineForm.Deadline = currentDeadline

            If deadlineForm.ShowDialog() = DialogResult.OK Then
                ' Aggiorna la scadenza nel database e nella DataGridView
                UpdateScadenzaInDatabase(rowIndex, deadlineForm.Deadline)

                If deadlineForm.Deadline.HasValue Then
                    ' Aggiorna con data e ora
                    selectedRow.Cells("Scadenza").Value = deadlineForm.Deadline.Value.ToString("dd/MM/yyyy HH:mm")
                Else
                    selectedRow.Cells("Scadenza").Value = DBNull.Value
                End If

                ' Aggiorna la visualizzazione
                dgvDocumenti.Refresh()
            End If
        End Using
    End Sub

    Private Sub ModifyAttachment(selectedRow As DataGridViewRow, documentId As Integer)
        Dim oldFileName As String = selectedRow.Cells("File").Value.ToString() ' Nome del vecchio file

        ' Apri il dialogo per selezionare il nuovo file
        Using openFileDialog As New OpenFileDialog()
            If openFileDialog.ShowDialog() = DialogResult.OK Then
                Dim newFilePath As String = openFileDialog.FileName
                Dim newFileName As String = Path.GetFileName(newFilePath)

                ' Verifica se il nome del file contiene spazi
                If newFileName.Contains(" ") Then
                    MessageBox.Show("Il nome del file non deve contenere spazi. Rinomina il file e riprova.")
                    Return
                End If

                Dim targetPath As String = Path.Combine(documentsFolderPath, newFileName)

                ' Copia il nuovo file nella cartella Documenti
                File.Copy(newFilePath, targetPath, True)

                ' Aggiorna il database con il nuovo nome del file
                UpdateDocumentFile(documentId, newFileName) ' Usa solo il nome del file

                ' Chiedi conferma prima di eliminare il vecchio file
                If Not String.IsNullOrEmpty(oldFileName) AndAlso File.Exists(Path.Combine(documentsFolderPath, oldFileName)) Then
                    Dim result As DialogResult = MessageBox.Show("Vuoi eliminare il file precedente?", "Conferma Eliminazione", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

                    If result = DialogResult.Yes Then
                        Try
                            File.Delete(Path.Combine(documentsFolderPath, oldFileName))
                            MessageBox.Show("Il file precedente è stato eliminato con successo.")
                        Catch ex As Exception
                            MessageBox.Show("Errore durante l'eliminazione del file precedente: " & ex.Message)
                        End Try
                    End If
                End If

                ' Aggiorna il valore della cella nella DataGridView con il nome del nuovo file
                selectedRow.Cells("File").Value = newFileName

                MessageBox.Show("Il file è stato aggiornato con successo.")
                dgvDocumenti.Refresh() ' Rinfresca la DataGridView
            End If
        End Using
    End Sub

    Private Sub UpdateDocumentFile(documentId As Integer, filePath As String)
        Using connection As New SQLiteConnection(connString)
            connection.Open()
            Dim query As String = "UPDATE Documenti SET File = @File WHERE Id = @Id"
            Using command As New SQLiteCommand(query, connection)
                command.Parameters.AddWithValue("@File", filePath)
                command.Parameters.AddWithValue("@Id", documentId)
                command.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Private Sub btnInserimentoGuidato_Click(sender As Object, e As EventArgs) Handles btnInserimentoGuidato.Click
        ' Verifica che sia stato selezionato un utente
        Dim selectedUtenteID As Integer = GetSelectedUtenteID()
        If selectedUtenteID = 0 Then
            MessageBox.Show("Seleziona un pagamento e utente prima di procedere.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Verifica che sia stato selezionato un tipo di pagamento
        If String.IsNullOrEmpty(ComboBoxTipoPagamento.Text) Then
            Dim result As DialogResult = MessageBox.Show(
            "Devi selezionare un pagamento e un utente. Oppure clicca su 'Sì' per inserire manualmente un nuovo pagamento dopo aver selezionato solo l'utente.",
            "Pagamento mancante",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question
        )

            If result = DialogResult.Yes Then
                ' Apri una finestra di dialogo per inserire manualmente un nuovo pagamento
                Dim nuovoPagamento As String = InputBox("Inserisci il nome del nuovo pagamento:", "Nuovo Pagamento")
                If Not String.IsNullOrEmpty(nuovoPagamento) Then
                    ' Aggiungi il nuovo pagamento al database
                    Try
                        Using connection As New SQLiteConnection(connString)
                            connection.Open()
                            Dim query As String = "INSERT INTO Documenti (TipoPagamento) VALUES (@TipoPagamento)"
                            Using command As New SQLiteCommand(query, connection)
                                command.Parameters.AddWithValue("@TipoPagamento", nuovoPagamento)
                                command.ExecuteNonQuery()
                            End Using
                        End Using

                        ' Aggiorna la ComboBox con il nuovo pagamento
                        PopulateComboBoxFromDatabase(ComboBoxTipoPagamento, "TipoPagamento")
                        ComboBoxTipoPagamento.Text = nuovoPagamento

                        MessageBox.Show("Nuovo pagamento creato con successo!", "Successo", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Catch ex As Exception
                        MessageBox.Show($"Errore durante la creazione del nuovo pagamento: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Try
                Else
                    MessageBox.Show("Inserimento del pagamento annullato.", "Annullato", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End If

            Return
        End If

        ' Ottieni il tipo di pagamento selezionato
        Dim tipoPagamento As String = ComboBoxTipoPagamento.Text

        ' Apri il form di inserimento guidato passando il tipo di pagamento e l'ID utente
        Dim inserimentoGuidatoForm As New FormInserimentoGuidato(connString, tipoPagamento, selectedUtenteID)
        inserimentoGuidatoForm.ShowDialog()
    End Sub

    Private Sub picNotifica_Click(sender As Object, e As EventArgs) Handles picNotifica.Click
        Dim now As DateTime = DateTime.Now
        Dim urgentTable As New DataTable()

        ' Copia la struttura della tabella originale
        Dim originalTable As DataTable = TryCast(dgvDocumenti.DataSource, DataTable)
        If originalTable Is Nothing OrElse Not originalTable.Columns.Contains("Scadenza") Then Return
        urgentTable = originalTable.Clone()

        ' Filtra le righe con scadenza entro 10 giorni e NON marcate come "scaduto"
        For i As Integer = 0 To dgvDocumenti.Rows.Count - 1
            Dim rowGrid = dgvDocumenti.Rows(i)
            Dim scadenzaStr As String = rowGrid.Cells("Scadenza").Value?.ToString()
            Dim statoCell = rowGrid.Cells("StatoPagamento")
            Dim isScaduto As Boolean = statoCell.Value IsNot Nothing AndAlso statoCell.Value.ToString().ToLower() = "scaduto"

            If Not isScaduto AndAlso Not String.IsNullOrEmpty(scadenzaStr) Then
                Dim scadenza As DateTime
                If DateTime.TryParse(scadenzaStr, scadenza) Then
                    Dim giorniRimanenti As Integer = (scadenza - now).Days
                    If giorniRimanenti <= 10 Then
                        urgentTable.ImportRow(originalTable.Rows(i))
                    End If
                End If
            End If
        Next

        ' Ordina la tabella per scadenza crescente
        If urgentTable.Rows.Count > 0 Then
            Dim view As DataView = urgentTable.DefaultView
            view.Sort = "Scadenza ASC"
            dgvDocumenti.DataSource = view.ToTable()
        Else
            MessageBox.Show("Nessuna scadenza urgente trovata.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
    Private Sub picNotifica_MouseEnter(sender As Object, e As EventArgs) Handles picNotifica.MouseEnter
        picNotifica.Cursor = Cursors.Hand
    End Sub

    Private Sub picNotifica_MouseLeave(sender As Object, e As EventArgs) Handles picNotifica.MouseLeave
        picNotifica.Cursor = Cursors.Default
    End Sub

    Private Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        Dim startDate As Date = dtpStartDate.Value.Date
        Dim endDate As Date = dtpEndDate.Value.Date

        ' Carica i dati filtrati
        Dim dt = GetMovimentiByDateRange(startDate, endDate)

        If dt.Rows.Count = 0 Then
            MessageBox.Show("Nessun dato nell'intervallo selezionato.")
            Return
        End If

        Using sfd As New SaveFileDialog
            sfd.Filter = "Excel (*.xlsx)|*.xlsx|CSV (*.csv)|*.csv"
            sfd.FileName = $"Documenti_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}"
            If sfd.ShowDialog = DialogResult.OK Then
                If sfd.FilterIndex = 1 Then
                    ' Esporta in Excel
                    Using wb As New XLWorkbook
                        wb.Worksheets.Add(dt, "Documenti")
                        wb.SaveAs(sfd.FileName)
                    End Using
                Else
                    ' Esporta in CSV
                    ExportDataTableToCSV(dt, sfd.FileName)
                End If
                MessageBox.Show("Esportazione completata!")
            End If
        End Using
    End Sub

    Private Function GetMovimentiByDateRange(startDate As Date, endDate As Date) As DataTable
        Dim dt As New DataTable()
        Dim databasePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath & ";Version=3;"
        Using conn As New SQLiteConnection(connString)
            conn.Open()
            Using da As New SQLiteDataAdapter("SELECT * FROM Documenti WHERE Data BETWEEN @startDate AND @endDate", conn)
                da.SelectCommand.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                da.SelectCommand.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))
                da.Fill(dt)
            End Using
        End Using
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


End Class

