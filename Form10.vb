Imports System.Data.SQLite
Imports System.IO
Imports System.Diagnostics
Imports DocumentFormat.OpenXml.Wordprocessing
Imports System.Drawing
Imports System.IO.Compression


Public Class FormNuovoDocumento
    Private connString As String
    Private tipoPagamento As String
    Private documentsFolderPath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "Documenti")
    Private utenteID As Integer
    Private documentId As Integer? ' ID del documento da modificare (opzionale)
    Private allegati As New List(Of String) ' Lista dei nomi file allegati

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

        Else
            lblIndicatore.Text = "Nuovo inserimento"
            lblIndicatore.ForeColor = System.Drawing.Color.FromArgb(255, 0, 128, 0) ' Verde

        End If





        ' Imposta il valore della TextBox per ricordare l'utente
        txtUtente.Text = $"Utente selezionato: {utenteID}"
        txtTipoPagamento.Text = $"Tipo pagamento selezionato: {tipoPagamento}"

        ' Se è in modalità modifica, carica i dati del documento
        ' Se è in modalità modifica, carica i dati del documento
        If documentId.HasValue Then
            CaricaDatiDocumento(documentId.Value)
            If allegati.Count > 0 Then
                Dim firstFile = allegati(0)
                Dim fullPath = Path.Combine(documentsFolderPath, firstFile)
                MostraAnteprimaDocumentoDaPercorso(fullPath)
            End If
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
                            Dim importoObj = reader("Importo")
                            If Not IsDBNull(importoObj) Then
                                Dim importo As Decimal = Convert.ToDecimal(importoObj)
                                TextBoxImporto.Text = importo.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)
                            End If
                            chkDaPagare.Checked = (reader("StatoPagamento").ToString() = "Da Pagare")
                            chkPagato.Checked = (reader("StatoPagamento").ToString() = "Pagato")
                            If Not IsDBNull(reader("Scadenza")) Then
                                chkScadenzaAttiva.Checked = True
                                DateTimePickerScadenza.Value = Convert.ToDateTime(reader("Scadenza"))
                            Else
                                chkScadenzaAttiva.Checked = False
                            End If

                            ' Imposta la data del documento
                            If Not IsDBNull(reader("Data")) Then
                                DateTimePickerData.Value = Convert.ToDateTime(reader("Data"))
                            End If

                            ' --- COMPATIBILITÀ VECCHI FILE ---
                            If Not IsDBNull(reader("File")) AndAlso Not String.IsNullOrWhiteSpace(reader("File").ToString()) Then
                                Dim oldFileName = reader("File").ToString()
                                If Not allegati.Contains(oldFileName) Then
                                    allegati.Add(oldFileName)
                                    lstAllegati.Items.Add(oldFileName)
                                End If
                            End If
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Errore durante il caricamento dei dati del documento: " & ex.Message)
        End Try

        ' Carica anche gli allegati dalla tabella Allegati
        Using conn As New SQLiteConnection(connString)
            conn.Open()
            Dim cmd As New SQLiteCommand("SELECT NomeFile FROM Allegati WHERE DocumentoID = @ID", conn)
            cmd.Parameters.AddWithValue("@ID", documentId)
            Using rdr = cmd.ExecuteReader()
                While rdr.Read()
                    Dim nome = rdr("NomeFile").ToString()
                    If Not allegati.Contains(nome) Then
                        allegati.Add(nome)
                        lstAllegati.Items.Add(nome)
                    End If
                End While
            End Using
        End Using
    End Sub
    Private Sub btnRimuoviAllegato_Click(sender As Object, e As EventArgs) Handles btnRimuoviAllegato.Click
        If lstAllegati.SelectedItem IsNot Nothing Then
            Dim fileName = lstAllegati.SelectedItem.ToString()
            allegati.Remove(fileName)
            lstAllegati.Items.Remove(fileName)
            ' (Opzionale) Elimina fisicamente il file dalla cartella
            Dim fullPath = Path.Combine(documentsFolderPath, fileName)
            If File.Exists(fullPath) Then File.Delete(fullPath)
        End If
    End Sub

    Private Sub lstAllegati_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstAllegati.SelectedIndexChanged
        If lstAllegati.SelectedItem IsNot Nothing Then
            Dim fileName = lstAllegati.SelectedItem.ToString()
            Dim fullPath = Path.Combine(documentsFolderPath, fileName)
            MostraAnteprimaDocumentoDaPercorso(fullPath)
        End If
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

        If allegati.Count = 0 Then
            MessageBox.Show("Devi inserire almeno un allegato.")
            Return
        End If

        If documentId.HasValue Then
            ' Modalità modifica
            Try
                Using connection As New SQLiteConnection(connString)
                    connection.Open()

                    ' Aggiorna i dati del documento
                    Dim queryUpdate As String = "UPDATE Documenti SET Data = @Data, AnnoRiferimento = @AnnoRiferimento, Note = @Note, Importo = @Importo, StatoPagamento = @StatoPagamento, Scadenza = @Scadenza WHERE ID = @ID"
                    Using commandUpdate As New SQLiteCommand(queryUpdate, connection)
                        commandUpdate.Parameters.AddWithValue("@Data", DateTimePickerData.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                        commandUpdate.Parameters.AddWithValue("@AnnoRiferimento", TextBoxAnno.Text)
                        commandUpdate.Parameters.AddWithValue("@Note", TextBoxNote.Text)
                        Dim importo As Decimal
                        If Not Decimal.TryParse(TextBoxImporto.Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, importo) Then
                            MessageBox.Show("Importo non valido.")
                            TextBoxImporto.Focus()
                            Return
                        End If
                        commandUpdate.Parameters.AddWithValue("@Importo", importo)
                        commandUpdate.Parameters.AddWithValue("@StatoPagamento", If(chkDaPagare.Checked, "Da Pagare", If(chkPagato.Checked, "Pagato", DBNull.Value)))
                        commandUpdate.Parameters.AddWithValue("@Scadenza", If(chkScadenzaAttiva.Checked, DateTimePickerScadenza.Value.ToString("yyyy-MM-dd HH:mm:ss"), DBNull.Value))
                        commandUpdate.Parameters.AddWithValue("@ID", documentId.Value)
                        commandUpdate.ExecuteNonQuery()
                    End Using

                    ' Elimina tutti gli allegati precedenti
                    Using cmdDel As New SQLiteCommand("DELETE FROM Allegati WHERE DocumentoID = @ID", connection)
                        cmdDel.Parameters.AddWithValue("@ID", documentId.Value)
                        cmdDel.ExecuteNonQuery()
                    End Using

                    ' Inserisci i nuovi allegati
                    For Each fileName In allegati
                        Using cmd As New SQLiteCommand("INSERT INTO Allegati (DocumentoID, NomeFile, PercorsoFile) VALUES (@docId, @nome, @percorso)", connection)
                            cmd.Parameters.AddWithValue("@docId", documentId.Value)
                            cmd.Parameters.AddWithValue("@nome", fileName)
                            cmd.Parameters.AddWithValue("@percorso", Path.Combine(documentsFolderPath, fileName))
                            cmd.ExecuteNonQuery()
                        End Using
                    Next
                End Using

                MessageBox.Show("Documento aggiornato con successo.")
                Me.Close()
            Catch ex As Exception
                MessageBox.Show("Si è verificato un errore durante l'aggiornamento del documento: " & ex.Message)
            End Try
        Else
            ' Modalità inserimento
            Try
                Using connection As New SQLiteConnection(connString)
                    connection.Open()

                    ' Inserisci il documento (il campo File può essere lasciato vuoto o NULL)
                    Dim query As String = "INSERT INTO Documenti (UtenteID, Data, AnnoRiferimento, TipoPagamento, Importo, File, Note, StatoPagamento" &
                    If(chkScadenzaAttiva.Checked, ", Scadenza", "") & ") " &
                    "VALUES (@UtenteID, @Data, @AnnoRiferimento, @TipoPagamento, @Importo, NULL, @Note, @StatoPagamento" &
                    If(chkScadenzaAttiva.Checked, ", @Scadenza", "") & ");"

                    Using command As New SQLiteCommand(query, connection)
                        command.Parameters.AddWithValue("@UtenteID", utenteID)
                        Dim dataConOraAzzerata As DateTime = DateTimePickerData.Value.Date
                        command.Parameters.AddWithValue("@Data", dataConOraAzzerata.ToString("yyyy-MM-dd HH:mm:ss"))
                        command.Parameters.AddWithValue("@AnnoRiferimento", TextBoxAnno.Text)
                        command.Parameters.AddWithValue("@TipoPagamento", tipoPagamento)
                        Dim importo As Decimal
                        If Not Decimal.TryParse(TextBoxImporto.Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, importo) Then
                            MessageBox.Show("Importo non valido.")
                            TextBoxImporto.Focus()
                            Return
                        End If
                        command.Parameters.AddWithValue("@Importo", importo)
                        command.Parameters.AddWithValue("@Note", TextBoxNote.Text)
                        command.Parameters.AddWithValue("@StatoPagamento", If(chkDaPagare.Checked, "Da Pagare", If(chkPagato.Checked, "Pagato", DBNull.Value)))
                        If chkScadenzaAttiva.Checked Then
                            command.Parameters.AddWithValue("@Scadenza", DateTimePickerScadenza.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                        End If
                        command.ExecuteNonQuery()
                    End Using

                    ' Recupera l'ID del nuovo documento
                    Dim newDocId As Long
                    Using cmd As New SQLiteCommand("SELECT last_insert_rowid()", connection)
                        newDocId = CLng(cmd.ExecuteScalar())
                    End Using

                    ' Inserisci gli allegati
                    For Each fileName In allegati
                        Using cmd As New SQLiteCommand("INSERT INTO Allegati (DocumentoID, NomeFile, PercorsoFile) VALUES (@docId, @nome, @percorso)", connection)
                            cmd.Parameters.AddWithValue("@docId", newDocId)
                            cmd.Parameters.AddWithValue("@nome", fileName)
                            cmd.Parameters.AddWithValue("@percorso", Path.Combine(documentsFolderPath, fileName))
                            cmd.ExecuteNonQuery()
                        End Using
                    Next
                End Using

                MessageBox.Show("Documento caricato con successo.")
                ResetCampi()

                ' Log
                Dim scadenza = If(chkScadenzaAttiva.Checked, DateTimePickerScadenza.Value.ToString("yyyy-MM-dd"), "Nessuna scadenza")
                Dim logDetails = $"Utente: {utenteID}, Anno: {TextBoxAnno.Text.Trim}, TipoPagamento: {tipoPagamento}, Note: {TextBoxNote.Text}, Allegati: {String.Join(";", allegati)}, Scadenza: {scadenza}"
                WriteLogEntry("Inserimento", logDetails)

                chkScadenzaAttiva.Checked = False
            Catch ex As Exception
                MessageBox.Show("Si è verificato un errore durante il caricamento del documento: " & ex.Message)
            End Try
        End If
    End Sub

    Private Sub ResetCampi()
        TextBoxAnno.Text = ""
        TextBoxNote.Text = ""
        TextBoxImporto.Text = ""

        chkDaPagare.Checked = False
        chkPagato.Checked = False
        chkScadenzaAttiva.Checked = False

        DateTimePickerScadenza.Value = DateTime.Now
        ' Se hai altri campi da azzerare, aggiungili qui
    End Sub

    Public Function UpdateDocument(documentId As Integer) As Boolean
        Try
            Using connection As New SQLiteConnection(connString)
                connection.Open()

                ' Aggiorna i dati del documento (senza campo File)
                Dim queryUpdate As String = "UPDATE Documenti SET AnnoRiferimento = @AnnoRiferimento, Note = @Note, Importo = @Importo, StatoPagamento = @StatoPagamento, Scadenza = @Scadenza WHERE ID = @ID"
                Using commandUpdate As New SQLiteCommand(queryUpdate, connection)
                    commandUpdate.Parameters.AddWithValue("@AnnoRiferimento", TextBoxAnno.Text)
                    commandUpdate.Parameters.AddWithValue("@Note", TextBoxNote.Text)
                    commandUpdate.Parameters.AddWithValue("@Importo", TextBoxImporto.Text)
                    commandUpdate.Parameters.AddWithValue("@StatoPagamento", If(chkDaPagare.Checked, "Da Pagare", If(chkPagato.Checked, "Pagato", DBNull.Value)))
                    commandUpdate.Parameters.AddWithValue("@Scadenza", If(chkScadenzaAttiva.Checked, DateTimePickerScadenza.Value.ToString("yyyy-MM-dd HH:mm:ss"), DBNull.Value))
                    commandUpdate.Parameters.AddWithValue("@ID", documentId)
                    commandUpdate.ExecuteNonQuery()
                End Using

                ' Elimina tutti gli allegati precedenti
                Using cmdDel As New SQLiteCommand("DELETE FROM Allegati WHERE DocumentoID = @ID", connection)
                    cmdDel.Parameters.AddWithValue("@ID", documentId)
                    cmdDel.ExecuteNonQuery()
                End Using

                ' Inserisci i nuovi allegati
                For Each fileName In allegati
                    Using cmd As New SQLiteCommand("INSERT INTO Allegati (DocumentoID, NomeFile, PercorsoFile) VALUES (@docId, @nome, @percorso)", connection)
                        cmd.Parameters.AddWithValue("@docId", documentId)
                        cmd.Parameters.AddWithValue("@nome", fileName)
                        cmd.Parameters.AddWithValue("@percorso", Path.Combine(documentsFolderPath, fileName))
                        cmd.ExecuteNonQuery()
                    End Using
                Next
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

                ' Dopo InsertDocument, recupera l'ID del nuovo documento
                Dim newDocId As Long
                Using cmd As New SQLiteCommand("SELECT last_insert_rowid()", connection)
                    newDocId = CLng(cmd.ExecuteScalar())
                End Using

                ' Inserisci gli allegati
                For Each fileName In allegati
                    Using cmd As New SQLiteCommand("INSERT INTO Allegati (DocumentoID, NomeFile, PercorsoFile) VALUES (@docId, @nome, @percorso)", connection)
                        cmd.Parameters.AddWithValue("@docId", newDocId)
                        cmd.Parameters.AddWithValue("@nome", fileName)
                        cmd.Parameters.AddWithValue("@percorso", Path.Combine(documentsFolderPath, fileName))
                        cmd.ExecuteNonQuery()
                    End Using
                Next

                Return True
            End Using
        Catch ex As Exception
            MessageBox.Show("Si è verificato un errore durante l'inserimento del documento nel database: " & ex.Message)
            Return False
        End Try
    End Function



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


    Private Sub MostraAnteprimaDocumentoDaPercorso(fullPath As String)
        panelPreview.Controls.Clear()
        If Not File.Exists(fullPath) Then Exit Sub

        Dim ext As String = Path.GetExtension(fullPath).ToLower()

        ' --- ZIP ---
        If ext = ".zip" Then
            Dim lstZipFiles As New ListBox()
            lstZipFiles.Dock = DockStyle.Fill
            lstZipFiles.SelectionMode = SelectionMode.One

            Dim folders As New HashSet(Of String)
            Using zip As ZipArchive = ZipFile.OpenRead(fullPath)
                For Each entry As ZipArchiveEntry In zip.Entries
                    lstZipFiles.Items.Add(entry.FullName)
                    Dim dir = Path.GetDirectoryName(entry.FullName)
                    If Not String.IsNullOrEmpty(dir) AndAlso Not folders.Contains(dir) Then
                        lstZipFiles.Items.Add(dir & "/")
                        folders.Add(dir)
                    End If
                Next
            End Using

            ' Gestore doppio click per anteprima file
            AddHandler lstZipFiles.DoubleClick, Sub(sender2, e2)
                                                    If lstZipFiles.SelectedItem Is Nothing Then Return
                                                    Dim selectedEntry = lstZipFiles.SelectedItem.ToString()
                                                    If selectedEntry.EndsWith("/") Then Return ' Ignora cartelle

                                                    ' Estrai il file in una cartella temporanea
                                                    Dim tempDir = Path.Combine(Path.GetTempPath(), "AmministrazionePreview")
                                                    If Not Directory.Exists(tempDir) Then Directory.CreateDirectory(tempDir)
                                                    Dim tempFile = Path.Combine(tempDir, Path.GetFileName(selectedEntry))

                                                    If File.Exists(tempFile) Then File.Delete(tempFile)

                                                    Using zip As ZipArchive = ZipFile.OpenRead(fullPath)
                                                        Dim entry = zip.GetEntry(selectedEntry)
                                                        If entry IsNot Nothing Then
                                                            entry.ExtractToFile(tempFile)
                                                            ' Apri anteprima in una finestra separata
                                                            Using anteprima As New FormAnteprimaDocumento(tempFile)
                                                                anteprima.ShowDialog(Me)
                                                            End Using
                                                        End If
                                                    End Using
                                                End Sub

            panelPreview.Controls.Add(lstZipFiles)
            Return
        End If

        ' --- PDF ---
        If ext = ".pdf" Then
            Try
                Dim webView As New Microsoft.Web.WebView2.WinForms.WebView2()
                webView.Dock = DockStyle.Fill
                webView.Source = New Uri(fullPath)
                panelPreview.Controls.Add(webView)
            Catch ex As Exception
                Dim lbl As New Label()
                lbl.Text = "Errore anteprima PDF: " & ex.Message
                lbl.Dock = DockStyle.Fill
                lbl.TextAlign = ContentAlignment.MiddleCenter
                panelPreview.Controls.Add(lbl)
            End Try
            Return
        End If

        ' --- Immagini ---
        If ext = ".jpg" OrElse ext = ".jpeg" OrElse ext = ".png" OrElse ext = ".bmp" Then
            Try
                Dim pb As New PictureBox()
                pb.Dock = DockStyle.Fill
                pb.SizeMode = PictureBoxSizeMode.Zoom
                pb.Image = Image.FromFile(fullPath)
                panelPreview.Controls.Add(pb)
            Catch ex As Exception
                Dim lbl As New Label()
                lbl.Text = "Errore anteprima immagine: " & ex.Message
                lbl.Dock = DockStyle.Fill
                lbl.TextAlign = ContentAlignment.MiddleCenter
                panelPreview.Controls.Add(lbl)
            End Try
            Return
        End If

        ' --- TXT ---
        If ext = ".txt" Then
            Try
                Dim tb As New TextBox()
                tb.Multiline = True
                tb.ReadOnly = True
                tb.Dock = DockStyle.Fill
                tb.Text = File.ReadAllText(fullPath)
                panelPreview.Controls.Add(tb)
            Catch ex As Exception
                Dim lbl As New Label()
                lbl.Text = "Errore anteprima testo: " & ex.Message
                lbl.Dock = DockStyle.Fill
                lbl.TextAlign = ContentAlignment.MiddleCenter
                panelPreview.Controls.Add(lbl)
            End Try
            Return
        End If

        ' --- Default ---
        Dim lblDefault As New Label()
        lblDefault.Text = "Nessuna anteprima disponibile"
        lblDefault.Dock = DockStyle.Fill
        lblDefault.TextAlign = ContentAlignment.MiddleCenter
        panelPreview.Controls.Add(lblDefault)
    End Sub
    Private Function ReadExactly(fs As FileStream, buffer As Byte(), offset As Integer, count As Integer) As Integer
        Dim totalRead As Integer = 0
        While totalRead < count
            Dim bytesRead As Integer = fs.Read(buffer, offset + totalRead, count - totalRead)
            If bytesRead = 0 Then Exit While
            totalRead += bytesRead
        End While
        Return totalRead
    End Function

    Private Sub TextBoxImporto_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBoxImporto.KeyPress
        Dim ch As Char = e.KeyChar

        ' Consenti solo cifre, punto e backspace
        If Not Char.IsDigit(ch) AndAlso ch <> "."c AndAlso ch <> ControlChars.Back Then
            e.Handled = True
            MessageBox.Show("Inserisci solo numeri e il punto come separatore decimale.", "Formato non valido", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Permetti solo un punto
        If ch = "."c AndAlso TextBoxImporto.Text.Contains(".") Then
            e.Handled = True
            MessageBox.Show("Puoi inserire un solo punto come separatore decimale.", "Formato non valido", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    ' Sposta l'allegato selezionato verso l'alto
    Private Sub btnAllegatoUp_Click(sender As Object, e As EventArgs) Handles btnAllegatoUp.Click
        Dim idx = lstAllegati.SelectedIndex
        If idx > 0 Then
            Dim item = lstAllegati.Items(idx)
            lstAllegati.Items.RemoveAt(idx)
            lstAllegati.Items.Insert(idx - 1, item)
            lstAllegati.SelectedIndex = idx - 1

            ' Aggiorna anche la lista allegati
            Dim file = allegati(idx)
            allegati.RemoveAt(idx)
            allegati.Insert(idx - 1, file)
        End If
    End Sub

    ' Sposta l'allegato selezionato verso il basso
    Private Sub btnAggiungiAllegato_Click(sender As Object, e As EventArgs) Handles btnAggiungiAllegato.Click
        Using ofd As New OpenFileDialog()
            ofd.Filter = "Tutti i file|*.*"
            ofd.Multiselect = True ' Permetti selezione multipla
            If ofd.ShowDialog() = DialogResult.OK Then
                For Each filePath In ofd.FileNames
                    Dim fileName = Path.GetFileName(filePath)
                    Dim destPath = Path.Combine(documentsFolderPath, fileName)
                    If Not File.Exists(destPath) Then
                        File.Copy(filePath, destPath)
                    End If
                    If Not allegati.Contains(fileName) Then
                        allegati.Add(fileName)
                        lstAllegati.Items.Add(fileName)
                    End If
                Next
            End If
        End Using
    End Sub


End Class