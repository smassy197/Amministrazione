Imports System.Data.SQLite
Imports System.IO
Imports System.Diagnostics
Imports DocumentFormat.OpenXml.Wordprocessing
Imports System.Drawing
Imports Patagames.Pdf.Net.Controls.WinForms


Public Class FormNuovoDocumento
    Private connString As String
    Private tipoPagamento As String
    Private documentsFolderPath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "Documenti")
    Private utenteID As Integer
    Private documentId As Integer? ' ID del documento da modificare (opzionale)


    Public Sub New(connString As String, tipoPagamento As String, utenteID As Integer, Optional documentId As Integer? = Nothing)
        InitializeComponent()
        Me.connString = connString
        Me.tipoPagamento = tipoPagamento
        Me.utenteID = utenteID
        Me.documentId = documentId

        ' Imposta l'indicatore di stato
        If documentId.HasValue Then
            Me.Text = $"Documento ID: {documentId.Value}" ' Titolo per la modalità modifica
            lblIndicatore.Text = "Modifica in atto"
            lblIndicatore.ForeColor = System.Drawing.Color.FromArgb(255, 255, 165, 0) ' Arancione
            ' Disattiva il pulsante "Sfoglia" e la TextBox del file
            txtNomeFile.Enabled = False
            btnSfogliaFile.Enabled = False
        Else
            lblIndicatore.Text = "Nuovo inserimento"
            lblIndicatore.ForeColor = System.Drawing.Color.FromArgb(255, 0, 128, 0) ' Verde
            chkModificaFile.Visible = False
        End If





        ' Imposta il valore della TextBox per ricordare l'utente
        txtUtente.Text = $"Utente selezionato: {utenteID}"
        txtTipoPagamento.Text = $"Tipo pagamento selezionato: {tipoPagamento}"

        ' Se è in modalità modifica, carica i dati del documento
        If documentId.HasValue Then
            CaricaDatiDocumento(documentId.Value)
            MostraAnteprimaDocumento()
        End If
    End Sub

    Private Sub CaricaDatiDocumento(documentId As Integer)
        Try
            Using connection As New SQLiteConnection(connString)
                connection.Open()

                Dim query As String = "SELECT * FROM Documenti WHERE ID = @ID"
                Using command As New SQLiteCommand(query, connection)
                    command.Parameters.AddWithValue("@ID", documentId)
                    Using reader As SQLiteDataReader = command.ExecuteReader()
                        If reader.Read() Then
                            TextBoxAnno.Text = reader("AnnoRiferimento").ToString()
                            TextBoxNote.Text = reader("Note").ToString()
                            TextBoxImporto.Text = reader("Importo").ToString()
                            txtNomeFile.Text = If(IsDBNull(reader("File")), "", reader("File").ToString())
                            chkDaPagare.Checked = (reader("StatoPagamento").ToString() = "Da Pagare")
                            chkPagato.Checked = (reader("StatoPagamento").ToString() = "Pagato")
                            If Not IsDBNull(reader("Scadenza")) Then
                                chkScadenzaAttiva.Checked = True
                                DateTimePickerScadenza.Value = Convert.ToDateTime(reader("Scadenza"))
                            Else
                                chkScadenzaAttiva.Checked = False
                            End If
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Errore durante il caricamento dei dati del documento: " & ex.Message)
        End Try
    End Sub


    Private Sub btnSalva_Click(sender As Object, e As EventArgs) Handles btnSalva.Click

        ' Controllo campi obbligatori
        If String.IsNullOrEmpty(TextBoxAnno.Text.Trim) Then
            MessageBox.Show("Per favore inserisci l'anno di riferimento.")
            TextBoxAnno.Focus()
            Return
        End If

        If TextBoxNote.Text = "" Then
            MessageBox.Show("Per favore inserisci una nota.")
            TextBoxNote.Focus()
            Return
        End If

        If documentId.HasValue Then
            ' Modalità modifica
            If UpdateDocument(documentId.Value) Then
                MessageBox.Show("Documento aggiornato con successo.")
                Me.Close()
            Else
                MessageBox.Show("Si è verificato un errore durante l'aggiornamento del documento.")
            End If
        Else
            ' Modalità inserimento

            ' Prosegui con l'inserimento del documento nel database e la copia nella cartella dei documenti
            If InsertDocument(txtNomeFile.Text) Then
                MessageBox.Show("Documento caricato con successo.")

                ResetCampi()

                ' Scrivi nel log
                ' Ottieni la scadenza
                Dim scadenza = If(chkScadenzaAttiva.Checked, DateTimePickerScadenza.Value.ToString("yyyy-MM-dd"), "Nessuna scadenza")

                ' Crea i dettagli del log


                Dim logDetails = $"Utente: {utenteID}, Anno: {TextBoxAnno.Text.Trim}, TipoPagamento: {tipoPagamento}, Note: {TextBoxNote.Text}, File: {txtNomeFile.Text}, Scadenza: {scadenza}"

                ' Scrivi nel log
                WriteLogEntry("Inserimento", logDetails)




                ' Aggiorna i filtri

                Dim documentType = tipoPagamento

                chkScadenzaAttiva.Checked = False
            Else
                MessageBox.Show("Si è verificato un errore durante il caricamento del documento.")
            End If
        End If
    End Sub

    Private Sub ResetCampi()
        TextBoxAnno.Text = ""
        TextBoxNote.Text = ""
        TextBoxImporto.Text = ""
        txtNomeFile.Text = ""
        chkDaPagare.Checked = False
        chkPagato.Checked = False
        chkScadenzaAttiva.Checked = False
        chkModificaFile.Checked = False
        DateTimePickerScadenza.Value = DateTime.Now
        ' Se hai altri campi da azzerare, aggiungili qui
    End Sub

    Public Function UpdateDocument(documentId As Integer) As Boolean
        Try
            Using connection As New SQLiteConnection(connString)
                connection.Open()

                ' Recupera il nome del file esistente dal database
                Dim existingFileName As String = Nothing
                Dim querySelect As String = "SELECT File FROM Documenti WHERE ID = @ID"
                Using commandSelect As New SQLiteCommand(querySelect, connection)
                    commandSelect.Parameters.AddWithValue("@ID", documentId)
                    Using reader As SQLiteDataReader = commandSelect.ExecuteReader()
                        If reader.Read() AndAlso Not IsDBNull(reader("File")) Then
                            existingFileName = reader("File").ToString()
                        End If
                    End Using
                End Using

                ' Determina il file da utilizzare
                Dim fileNameToUse As String = existingFileName

                ' Caso 1: L'utente ha selezionato "Modifica File" e ha cancellato il contenuto della TextBox
                If chkModificaFile.Checked AndAlso String.IsNullOrEmpty(txtNomeFile.Text) Then
                    ' Elimina il file esistente dalla cartella, se presente
                    If Not String.IsNullOrEmpty(existingFileName) Then
                        Dim existingFilePath As String = Path.Combine(documentsFolderPath, existingFileName)
                        If File.Exists(existingFilePath) Then
                            File.Delete(existingFilePath)
                        End If
                    End If

                    ' Rimuovi il riferimento al file nel database
                    fileNameToUse = Nothing
                End If

                ' Caso 2: L'utente ha selezionato un nuovo file
                If Not String.IsNullOrEmpty(txtNomeFile.Text) AndAlso txtNomeFile.Text <> existingFileName Then
                    fileNameToUse = Path.GetFileName(txtNomeFile.Text)

                    ' Verifica se il nome del file contiene spazi
                    If fileNameToUse.Contains(" ") Then
                        MessageBox.Show("Il nome del file non può contenere spazi. Rinomina il file e riprova.", "Errore Nome File", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return False
                    End If

                    ' Controlla se il file esiste già nella cartella
                    Dim newFilePath As String = Path.Combine(documentsFolderPath, fileNameToUse)
                    If File.Exists(newFilePath) Then
                        Dim overwriteResult As DialogResult = MessageBox.Show($"Un file con lo stesso nome esiste già nella cartella Documenti. Vuoi sovrascriverlo?", "Conferma Sovrascrittura", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                        If overwriteResult = DialogResult.No Then
                            Return False
                        End If
                    End If

                    ' Elimina il file esistente dalla cartella, se presente
                    If Not String.IsNullOrEmpty(existingFileName) Then
                        Dim existingFilePath As String = Path.Combine(documentsFolderPath, existingFileName)
                        If File.Exists(existingFilePath) Then
                            File.Delete(existingFilePath)
                        End If
                    End If

                    ' Copia il nuovo file nella cartella
                    File.Copy(txtNomeFile.Text, newFilePath, True)
                End If

                ' Aggiorna il record nel database
                Dim queryUpdate As String = "UPDATE Documenti SET AnnoRiferimento = @AnnoRiferimento, Note = @Note, Importo = @Importo, File = @File, StatoPagamento = @StatoPagamento, Scadenza = @Scadenza WHERE ID = @ID"
                Using commandUpdate As New SQLiteCommand(queryUpdate, connection)
                    commandUpdate.Parameters.AddWithValue("@AnnoRiferimento", TextBoxAnno.Text)
                    commandUpdate.Parameters.AddWithValue("@Note", TextBoxNote.Text)
                    commandUpdate.Parameters.AddWithValue("@Importo", TextBoxImporto.Text)
                    commandUpdate.Parameters.AddWithValue("@File", If(String.IsNullOrEmpty(fileNameToUse), DBNull.Value, fileNameToUse))
                    commandUpdate.Parameters.AddWithValue("@StatoPagamento", If(chkDaPagare.Checked, "Da Pagare", If(chkPagato.Checked, "Pagato", DBNull.Value)))
                    commandUpdate.Parameters.AddWithValue("@Scadenza", If(chkScadenzaAttiva.Checked, DateTimePickerScadenza.Value.ToString("yyyy-MM-dd HH:mm:ss"), DBNull.Value))
                    commandUpdate.Parameters.AddWithValue("@ID", documentId)

                    commandUpdate.ExecuteNonQuery()
                End Using
            End Using
            Return True
        Catch ex As Exception
            MessageBox.Show("Si è verificato un errore durante l'aggiornamento del documento: " & ex.Message)
            Return False
        End Try
    End Function

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


    Private Function InsertDocument(filePath As String) As Boolean
        Try
            Using connection As New SQLiteConnection(connString)
                connection.Open()


                Dim documentFileName As String = Nothing

                If Not String.IsNullOrEmpty(filePath) Then
                    If Not File.Exists(filePath) Then
                        MessageBox.Show("Il file specificato non esiste.")
                        Return False
                    End If

                    documentFileName = Path.GetFileName(filePath)

                    ' Verifica se il nome del file contiene spazi
                    If documentFileName.Contains(" ") Then
                        MessageBox.Show("Il nome del file non può contenere spazi. Rinomina il file e riprova.", "Errore Nome File", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return False
                    End If

                    ' Verifica se il file esiste già nella cartella
                    If File.Exists(Path.Combine(documentsFolderPath, documentFileName)) Then
                        MessageBox.Show("Il file esiste già.")
                        Return False
                    End If

                    ' Copia il documento nella cartella dei documenti
                    Dim destinationPath As String = Path.Combine(documentsFolderPath, documentFileName)
                    File.Copy(filePath, destinationPath)
                End If


                ' Determina lo stato del pagamento
                Dim statoPagamento As String = Nothing
                If chkDaPagare.Checked Then
                    statoPagamento = "Da Pagare"
                ElseIf chkPagato.Checked Then
                    statoPagamento = "Pagato"
                End If

                ' Inserisci i dati del documento nella tabella "Documenti"
                Dim query As String = "INSERT INTO Documenti (UtenteID, Data, AnnoRiferimento, TipoPagamento, Importo, File, Note, StatoPagamento" &
                If(chkScadenzaAttiva.Checked, ", Scadenza", "") & ") " &
                "VALUES (@UtenteID, @Data, @AnnoRiferimento, @TipoPagamento, @Importo, @File, @Note, @StatoPagamento" &
                If(chkScadenzaAttiva.Checked, ", @Scadenza", "") & ");"

                Using command As New SQLiteCommand(query, connection)
                    command.Parameters.AddWithValue("@UtenteID", utenteID)
                    ' Usa il formato completo della data con orario
                    Dim dataConOraAzzerata As DateTime = DateTimePickerData.Value.Date
                    command.Parameters.AddWithValue("@Data", dataConOraAzzerata.ToString("yyyy-MM-dd HH:mm:ss"))
                    command.Parameters.AddWithValue("@AnnoRiferimento", TextBoxAnno.Text)
                    command.Parameters.AddWithValue("@TipoPagamento", tipoPagamento)
                    command.Parameters.AddWithValue("@Importo", TextBoxImporto.Text)
                    command.Parameters.AddWithValue("@File", documentFileName)
                    command.Parameters.AddWithValue("@Note", TextBoxNote.Text)
                    command.Parameters.AddWithValue("@StatoPagamento", If(statoPagamento, DBNull.Value)) ' Usa NULL se non è selezionato
                    ' Aggiungi Scadenza solo se il flag è attivo
                    If chkScadenzaAttiva.Checked Then
                        command.Parameters.AddWithValue("@Scadenza", DateTimePickerScadenza.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                    End If

                    command.ExecuteNonQuery()
                End Using

                Return True
            End Using
        Catch ex As Exception
            MessageBox.Show("Si è verificato un errore durante l'inserimento del documento nel database: " & ex.Message)
            Return False
        End Try
    End Function


    Private Sub btnSfogliaFile_Click(sender As Object, e As EventArgs) Handles btnSfogliaFile.Click
        Using openFileDialog As New OpenFileDialog()
            openFileDialog.Filter = "Tutti i file|*.*"
            If openFileDialog.ShowDialog() = DialogResult.OK Then
                txtNomeFile.Text = openFileDialog.FileName
            End If
        End Using
    End Sub
    Private Sub FormNuovoDocumento_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        ' Verifica se Form3 è aperto
        For Each form As Form In Application.OpenForms
            If TypeOf form Is Form3 Then
                ' Richiama il metodo per aggiornare la DataGridView
                DirectCast(form, Form3).AggiornaDataGridView()

                ' Richiama il metodo per aggiornare il MonthCalendar
                DirectCast(form, Form3).HighlightDatesWithDocuments()
                Exit For
            End If
        Next
    End Sub

    Private Sub chkDaPagare_CheckedChanged(sender As Object, e As EventArgs) Handles chkDaPagare.CheckedChanged
        If chkDaPagare.Checked Then
            ' Abilita la scadenza ma consente la modifica manuale
            chkScadenzaAttiva.Checked = True
            chkScadenzaAttiva.Enabled = True ' Permetti la modifica manuale
            chkPagato.Checked = False
            DateTimePickerScadenza.Enabled = True ' Permetti la modifica manuale della data
        Else
            ' Disabilita la scadenza se "Da Pagare" non è selezionato
            chkScadenzaAttiva.Checked = False
            chkScadenzaAttiva.Enabled = True
            chkPagato.Checked = True
            DateTimePickerScadenza.Enabled = True
        End If
    End Sub

    Private Sub chkPagato_CheckedChanged(sender As Object, e As EventArgs) Handles chkPagato.CheckedChanged
        If chkPagato.Checked Then
            ' Deseleziona "Da Pagare" e "Scadenza"
            chkDaPagare.Checked = False
            chkScadenzaAttiva.Checked = False
        End If
    End Sub


    Private Sub chkModificaFile_CheckedChanged(sender As Object, e As EventArgs) Handles chkModificaFile.CheckedChanged
        If chkModificaFile.Checked Then
            txtNomeFile.Enabled = True
            btnSfogliaFile.Enabled = True
        Else
            txtNomeFile.Enabled = False
            btnSfogliaFile.Enabled = False
        End If
    End Sub

    Private Sub btnApriDocumento_Click(sender As Object, e As EventArgs) Handles btnApriDocumento.Click
        Dim fileName As String = txtNomeFile.Text
        If String.IsNullOrWhiteSpace(fileName) Then
            MessageBox.Show("Nessun file selezionato o associato.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim fullPath As String
        If Path.IsPathRooted(fileName) Then
            fullPath = fileName
        Else
            fullPath = Path.Combine(documentsFolderPath, fileName)
        End If

        If Not File.Exists(fullPath) Then
            MessageBox.Show("File non trovato: " & fullPath, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Try
            Dim psi As New ProcessStartInfo()
            psi.FileName = fullPath
            psi.UseShellExecute = True
            Process.Start(psi)
        Catch ex As Exception
            MessageBox.Show("Errore nell'apertura del file: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub MostraAnteprimaDocumento()
        panelPreview.Controls.Clear()
        Dim fileName As String = txtNomeFile.Text
        If String.IsNullOrWhiteSpace(fileName) Then Exit Sub

        Dim fullPath As String
        If Path.IsPathRooted(fileName) Then
            fullPath = fileName
        Else
            fullPath = Path.Combine(documentsFolderPath, fileName)
        End If

        If Not File.Exists(fullPath) Then Exit Sub

        Dim ext As String = Path.GetExtension(fullPath).ToLower()
        If ext = ".pdf" Then
            ' Usa Patagames Pdfium.Net SDK
            Dim pdfViewer As New PdfViewer()
            pdfViewer.Dock = DockStyle.Fill
            pdfViewer.LoadDocument(fullPath)
            panelPreview.Controls.Add(pdfViewer)
        ElseIf ext = ".jpg" OrElse ext = ".jpeg" OrElse ext = ".png" OrElse ext = ".bmp" Then
            Dim pb As New PictureBox()
            pb.Dock = DockStyle.Fill
            pb.SizeMode = PictureBoxSizeMode.Zoom
            pb.Image = Image.FromFile(fullPath)
            panelPreview.Controls.Add(pb)
        ElseIf ext = ".txt" Then
            Dim tb As New TextBox()
            tb.Multiline = True
            tb.ReadOnly = True
            tb.Dock = DockStyle.Fill
            tb.Text = File.ReadAllText(fullPath)
            panelPreview.Controls.Add(tb)
        Else
            Dim lbl As New Label()
            lbl.Text = "Nessuna anteprima disponibile"
            lbl.Dock = DockStyle.Fill
            lbl.TextAlign = ContentAlignment.MiddleCenter
            panelPreview.Controls.Add(lbl)
        End If
    End Sub



End Class