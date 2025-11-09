Imports Microsoft.Data.Sqlite
Imports System.Diagnostics
Imports System.Drawing
Imports System.IO
Imports System.IO.Compression
Imports DocumentFormat.OpenXml.Wordprocessing


Public Class FormNuovoDocumento
    Private connString As String
    Private tipoPagamento As String
    Private documentsFolderPath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "Documenti")
    Private utenteID As Integer
    Private documentId As Integer? ' ID del documento da modificare (opzionale)
    Private allegati As New List(Of String) ' Lista dei nomi file allegati

    Public Sub New(connString As String, tipoPagamento As String, utenteID As Integer, Optional documentId As Integer? = Nothing)
        Console.WriteLine("[DEBUG] Inizializzazione form. Parametri ricevuti → connString: " & connString & ", tipoPagamento: " & tipoPagamento & ", utenteID: " & utenteID & ", documentId: " & If(documentId.HasValue, documentId.Value.ToString(), "null"))

        InitializeComponent()

        Me.connString = connString
        Me.tipoPagamento = tipoPagamento
        Me.utenteID = utenteID
        Me.documentId = documentId

        lstAllegati.AllowDrop = True
        Console.WriteLine("[DEBUG] Drag & Drop abilitato su lstAllegati.")

        If documentId.HasValue Then
            Me.Text = $"Documento ID: {documentId.Value}"
            lblIndicatore.Text = "Modifica in atto"
            lblIndicatore.ForeColor = System.Drawing.Color.FromArgb(255, 255, 165, 0)
            Console.WriteLine("[DEBUG] Modalità modifica attiva. Documento ID: " & documentId.Value)
        Else
            lblIndicatore.Text = "Nuovo inserimento"
            lblIndicatore.ForeColor = System.Drawing.Color.FromArgb(255, 0, 128, 0)
            Console.WriteLine("[DEBUG] Modalità nuovo inserimento.")
        End If

        txtUtente.Text = $"Utente selezionato: {utenteID}"
        txtTipoPagamento.Text = $"Tipo pagamento selezionato: {tipoPagamento}"
        Console.WriteLine("[DEBUG] txtUtente e txtTipoPagamento aggiornati.")

        If documentId.HasValue Then
            Console.WriteLine("[DEBUG] Avvio caricamento dati documento.")
            CaricaDatiDocumento(documentId.Value)

            If allegati.Count > 0 Then
                Dim firstFile = allegati(0)
                Dim fullPath = Path.Combine(documentsFolderPath, firstFile)
                Console.WriteLine("[DEBUG] Primo allegato trovato: " & firstFile)
                If File.Exists(fullPath) Then
                    MostraAnteprimaDocumentoDaPercorso(fullPath)
                    Console.WriteLine("[DEBUG] Anteprima documento avviata.")
                Else
                    Console.WriteLine("[DEBUG] File non trovato: " & fullPath)
                End If
            Else
                Console.WriteLine("[DEBUG] Nessun allegato disponibile.")
            End If
        End If
    End Sub

    Private Sub CaricaDatiDocumento(documentId As Integer)
        Console.WriteLine("[DEBUG] Avvio CaricaDatiDocumento. ID: " & documentId)

        Try
            Using connection As New Microsoft.Data.Sqlite.SqliteConnection(connString)
                connection.Open()
                Console.WriteLine("[DEBUG] Connessione aperta per tabella Documenti.")

                Dim query As String = "SELECT * FROM Documenti WHERE ID = @ID"
                Using command As New Microsoft.Data.Sqlite.SqliteCommand(query, connection)
                    command.Parameters.AddWithValue("@ID", documentId)

                    Using reader As Microsoft.Data.Sqlite.SqliteDataReader = command.ExecuteReader()
                        If reader.Read() Then
                            TextBoxAnno.Text = reader("AnnoRiferimento").ToString()
                            TextBoxNote.Text = reader("Note").ToString()

                            Dim importoObj = reader("Importo")
                            If Not IsDBNull(importoObj) Then
                                Dim importo As Decimal = Convert.ToDecimal(importoObj)
                                TextBoxImporto.Text = importo.ToString("0.00", Globalization.CultureInfo.InvariantCulture)
                            End If

                            Dim stato = reader("StatoPagamento").ToString()
                            chkDaPagare.Checked = (stato = "Da Pagare")
                            chkPagato.Checked = (stato = "Pagato")

                            If Not IsDBNull(reader("Scadenza")) Then
                                chkScadenzaAttiva.Checked = True
                                DateTimePickerScadenza.Value = Convert.ToDateTime(reader("Scadenza"))
                            Else
                                chkScadenzaAttiva.Checked = False
                            End If

                            If Not IsDBNull(reader("Data")) Then
                                DateTimePickerData.Value = Convert.ToDateTime(reader("Data"))
                            End If

                            If Not IsDBNull(reader("File")) AndAlso Not String.IsNullOrWhiteSpace(reader("File").ToString()) Then
                                Dim oldFileName = reader("File").ToString()
                                If Not allegati.Contains(oldFileName) Then
                                    allegati.Add(oldFileName)
                                    lstAllegati.Items.Add(oldFileName)
                                    Console.WriteLine("[DEBUG] Allegato legacy aggiunto: " & oldFileName)
                                End If
                            End If
                        Else
                            Console.WriteLine("[DEBUG] Nessun record trovato per ID: " & documentId)
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Errore durante il caricamento dei dati del documento: " & ex.Message)
            Console.WriteLine("[DEBUG] Errore: " & ex.ToString())
        End Try

        Try
            Using conn As New Microsoft.Data.Sqlite.SqliteConnection(connString)
                conn.Open()
                Console.WriteLine("[DEBUG] Connessione aperta per tabella Allegati.")

                Using cmd As New Microsoft.Data.Sqlite.SqliteCommand("SELECT NomeFile FROM Allegati WHERE DocumentoID = @ID", conn)
                    cmd.Parameters.AddWithValue("@ID", documentId)

                    Using rdr = cmd.ExecuteReader()
                        While rdr.Read()
                            Dim nome = rdr("NomeFile").ToString()
                            If Not allegati.Contains(nome) Then
                                allegati.Add(nome)
                                lstAllegati.Items.Add(nome)
                                Console.WriteLine("[DEBUG] Allegato aggiunto da tabella: " & nome)
                            End If
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Errore durante il caricamento degli allegati: " & ex.Message)
            Console.WriteLine("[DEBUG] Errore Allegati: " & ex.ToString())
        End Try
    End Sub

    Private Sub btnRimuoviAllegato_Click(sender As Object, e As EventArgs) Handles btnRimuoviAllegato.Click
        If lstAllegati.SelectedItem IsNot Nothing Then
            Dim fileName = lstAllegati.SelectedItem.ToString()
            allegati.Remove(fileName)
            lstAllegati.Items.Remove(fileName)
            Console.WriteLine("[DEBUG] Allegato rimosso: " & fileName)

            Dim fullPath = Path.Combine(documentsFolderPath, fileName)
            If File.Exists(fullPath) Then
                File.Delete(fullPath)
                Console.WriteLine("[DEBUG] File eliminato fisicamente: " & fullPath)
            Else
                Console.WriteLine("[DEBUG] File non trovato per eliminazione: " & fullPath)
            End If
        End If
    End Sub


    Private Sub lstAllegati_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstAllegati.SelectedIndexChanged
        If lstAllegati.SelectedItem IsNot Nothing Then
            Dim fileName = lstAllegati.SelectedItem.ToString()
            Dim fullPath = Path.Combine(documentsFolderPath, fileName)

            Console.WriteLine("[DEBUG] Selezionato file: " & fileName)
            Console.WriteLine("[DEBUG] Percorso completo: " & fullPath)

            If File.Exists(fullPath) Then
                MostraAnteprimaDocumentoDaPercorso(fullPath)
                Console.WriteLine("[DEBUG] Anteprima avviata.")
            Else
                MessageBox.Show("Il file selezionato non è disponibile.", "File mancante", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Console.WriteLine("[DEBUG] File non trovato: " & fullPath)
            End If
        End If
    End Sub

    Private Sub btnSalva_Click(sender As Object, e As EventArgs) Handles btnSalva.Click
        Console.WriteLine("[DEBUG] Avvio btnSalva_Click.")

        If String.IsNullOrEmpty(TextBoxAnno.Text.Trim) Then
            MessageBox.Show("Per favore inserisci l'anno di riferimento.")
            TextBoxAnno.Focus()
            Console.WriteLine("[DEBUG] Campo AnnoRiferimento mancante.")
            Return
        End If

        If TextBoxNote.Text = "" Then
            MessageBox.Show("Per favore inserisci una nota.")
            TextBoxNote.Focus()
            Console.WriteLine("[DEBUG] Campo Note mancante.")
            Return
        End If

        If allegati.Count = 0 Then
            MessageBox.Show("Devi inserire almeno un allegato.")
            Console.WriteLine("[DEBUG] Nessun allegato presente.")
            Return
        End If

        If documentId.HasValue Then
            Console.WriteLine("[DEBUG] Modalità modifica attiva. Documento ID: " & documentId.Value)
            Try
                Using connection As New Microsoft.Data.Sqlite.SqliteConnection(connString)
                    connection.Open()
                    Console.WriteLine("[DEBUG] Connessione aperta.")

                    Dim queryUpdate As String = "UPDATE Documenti SET Data = @Data, AnnoRiferimento = @AnnoRiferimento, Note = @Note, Importo = @Importo, StatoPagamento = @StatoPagamento, Scadenza = @Scadenza WHERE ID = @ID"
                    Using commandUpdate As New Microsoft.Data.Sqlite.SqliteCommand(queryUpdate, connection)
                        commandUpdate.Parameters.AddWithValue("@Data", DateTimePickerData.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                        commandUpdate.Parameters.AddWithValue("@AnnoRiferimento", TextBoxAnno.Text)
                        commandUpdate.Parameters.AddWithValue("@Note", TextBoxNote.Text)

                        Dim importo As Decimal
                        If Not Decimal.TryParse(TextBoxImporto.Text, Globalization.NumberStyles.Any, Globalization.CultureInfo.InvariantCulture, importo) Then
                            MessageBox.Show("Importo non valido.")
                            TextBoxImporto.Focus()
                            Console.WriteLine("[DEBUG] Importo non valido.")
                            Return
                        End If
                        commandUpdate.Parameters.AddWithValue("@Importo", importo)
                        commandUpdate.Parameters.AddWithValue("@StatoPagamento", If(chkDaPagare.Checked, "Da Pagare", If(chkPagato.Checked, "Pagato", DBNull.Value)))
                        commandUpdate.Parameters.AddWithValue("@Scadenza", If(chkScadenzaAttiva.Checked, DateTimePickerScadenza.Value.ToString("yyyy-MM-dd HH:mm:ss"), DBNull.Value))
                        commandUpdate.Parameters.AddWithValue("@ID", documentId.Value)
                        commandUpdate.ExecuteNonQuery()
                        Console.WriteLine("[DEBUG] Documento aggiornato.")
                    End Using

                    Using cmdDel As New Microsoft.Data.Sqlite.SqliteCommand("DELETE FROM Allegati WHERE DocumentoID = @ID", connection)
                        cmdDel.Parameters.AddWithValue("@ID", documentId.Value)
                        cmdDel.ExecuteNonQuery()
                        Console.WriteLine("[DEBUG] Allegati precedenti eliminati.")
                    End Using

                    For Each fileName In allegati
                        Using cmd As New Microsoft.Data.Sqlite.SqliteCommand("INSERT INTO Allegati (DocumentoID, NomeFile, PercorsoFile) VALUES (@docId, @nome, @percorso)", connection)
                            cmd.Parameters.AddWithValue("@docId", documentId.Value)
                            cmd.Parameters.AddWithValue("@nome", fileName)
                            cmd.Parameters.AddWithValue("@percorso", Path.Combine(documentsFolderPath, fileName))
                            cmd.ExecuteNonQuery()
                            Console.WriteLine("[DEBUG] Allegato inserito: " & fileName)
                        End Using
                    Next
                End Using

                MessageBox.Show("Documento aggiornato con successo.")
                Me.Close()
            Catch ex As Exception
                MessageBox.Show("Errore durante l'aggiornamento: " & ex.Message)
                Console.WriteLine("[DEBUG] Errore aggiornamento: " & ex.ToString())
            End Try
        Else
            Console.WriteLine("[DEBUG] Modalità inserimento attiva.")
            Try
                Using connection As New Microsoft.Data.Sqlite.SqliteConnection(connString)
                    connection.Open()
                    Console.WriteLine("[DEBUG] Connessione aperta.")

                    Dim query As String = "INSERT INTO Documenti (UtenteID, Data, AnnoRiferimento, TipoPagamento, Importo, File, Note, StatoPagamento" &
                    If(chkScadenzaAttiva.Checked, ", Scadenza", "") & ") " &
                    "VALUES (@UtenteID, @Data, @AnnoRiferimento, @TipoPagamento, @Importo, NULL, @Note, @StatoPagamento" &
                    If(chkScadenzaAttiva.Checked, ", @Scadenza", "") & ");"

                    Using command As New Microsoft.Data.Sqlite.SqliteCommand(query, connection)
                        command.Parameters.AddWithValue("@UtenteID", utenteID)
                        command.Parameters.AddWithValue("@Data", DateTimePickerData.Value.Date.ToString("yyyy-MM-dd HH:mm:ss"))
                        command.Parameters.AddWithValue("@AnnoRiferimento", TextBoxAnno.Text)
                        command.Parameters.AddWithValue("@TipoPagamento", tipoPagamento)

                        Dim importo As Decimal
                        If Not Decimal.TryParse(TextBoxImporto.Text, Globalization.NumberStyles.Any, Globalization.CultureInfo.InvariantCulture, importo) Then
                            MessageBox.Show("Importo non valido.")
                            TextBoxImporto.Focus()
                            Console.WriteLine("[DEBUG] Importo non valido.")
                            Return
                        End If
                        command.Parameters.AddWithValue("@Importo", importo)
                        command.Parameters.AddWithValue("@Note", TextBoxNote.Text)
                        command.Parameters.AddWithValue("@StatoPagamento", If(chkDaPagare.Checked, "Da Pagare", If(chkPagato.Checked, "Pagato", DBNull.Value)))
                        If chkScadenzaAttiva.Checked Then
                            command.Parameters.AddWithValue("@Scadenza", DateTimePickerScadenza.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                        End If
                        command.ExecuteNonQuery()
                        Console.WriteLine("[DEBUG] Documento inserito.")
                    End Using

                    Dim newDocId As Long
                    Using cmd As New Microsoft.Data.Sqlite.SqliteCommand("SELECT last_insert_rowid()", connection)
                        newDocId = CLng(cmd.ExecuteScalar())
                        Console.WriteLine("[DEBUG] Nuovo ID documento: " & newDocId)
                    End Using

                    For Each fileName In allegati
                        Using cmd As New Microsoft.Data.Sqlite.SqliteCommand("INSERT INTO Allegati (DocumentoID, NomeFile, PercorsoFile) VALUES (@docId, @nome, @percorso)", connection)
                            cmd.Parameters.AddWithValue("@docId", newDocId)
                            cmd.Parameters.AddWithValue("@nome", fileName)
                            cmd.Parameters.AddWithValue("@percorso", Path.Combine(documentsFolderPath, fileName))
                            cmd.ExecuteNonQuery()
                            Console.WriteLine("[DEBUG] Allegato inserito: " & fileName)
                        End Using
                    Next
                End Using

                MessageBox.Show("Documento caricato con successo.")
                ResetCampi()

                Dim scadenza = If(chkScadenzaAttiva.Checked, DateTimePickerScadenza.Value.ToString("yyyy-MM-dd"), "Nessuna scadenza")
                Dim logDetails = $"Utente: {utenteID}, Anno: {TextBoxAnno.Text.Trim}, TipoPagamento: {tipoPagamento}, Note: {TextBoxNote.Text}, Allegati: {String.Join(";", allegati)}, Scadenza: {scadenza}"
                WriteLogEntry("Inserimento", logDetails)
                Console.WriteLine("[DEBUG] Log inserimento scritto.")

                chkScadenzaAttiva.Checked = False
            Catch ex As Exception
                MessageBox.Show("Errore durante il caricamento: " & ex.Message)
                Console.WriteLine("[DEBUG] Errore inserimento: " & ex.ToString())
            End Try
        End If
    End Sub

    Private Sub ResetCampi()
        Console.WriteLine("[DEBUG] Avvio ResetCampi.")

        TextBoxAnno.Text = ""
        TextBoxNote.Text = ""
        TextBoxImporto.Text = ""
        Console.WriteLine("[DEBUG] Campi testo azzerati.")

        chkDaPagare.Checked = False
        chkPagato.Checked = False
        chkScadenzaAttiva.Checked = False
        Console.WriteLine("[DEBUG] Checkbox ripristinate.")

        DateTimePickerScadenza.Value = DateTime.Now
        Console.WriteLine("[DEBUG] Scadenza impostata a oggi.")

        ' Se hai altri campi da azzerare, aggiungili qui
    End Sub


    Public Function UpdateDocument(documentId As Integer) As Boolean
        Console.WriteLine("[DEBUG] Avvio UpdateDocument. ID: " & documentId)

        Try
            Using connection As New Microsoft.Data.Sqlite.SqliteConnection(connString)
                connection.Open()
                Console.WriteLine("[DEBUG] Connessione al database aperta.")

                Dim queryUpdate As String = "
                UPDATE Documenti 
                SET AnnoRiferimento = @AnnoRiferimento, 
                    Note = @Note, 
                    Importo = @Importo, 
                    StatoPagamento = @StatoPagamento, 
                    Scadenza = @Scadenza 
                WHERE ID = @ID"

                Using commandUpdate As New Microsoft.Data.Sqlite.SqliteCommand(queryUpdate, connection)
                    commandUpdate.Parameters.AddWithValue("@AnnoRiferimento", TextBoxAnno.Text)
                    commandUpdate.Parameters.AddWithValue("@Note", TextBoxNote.Text)

                    Dim importo As Decimal
                    If Not Decimal.TryParse(TextBoxImporto.Text, Globalization.NumberStyles.Any, Globalization.CultureInfo.InvariantCulture, importo) Then
                        MessageBox.Show("Importo non valido.")
                        Console.WriteLine("[DEBUG] Importo non valido: " & TextBoxImporto.Text)
                        Return False
                    End If
                    commandUpdate.Parameters.AddWithValue("@Importo", importo)

                    commandUpdate.Parameters.AddWithValue("@StatoPagamento", If(chkDaPagare.Checked, "Da Pagare", If(chkPagato.Checked, "Pagato", DBNull.Value)))
                    commandUpdate.Parameters.AddWithValue("@Scadenza", If(chkScadenzaAttiva.Checked, DateTimePickerScadenza.Value.ToString("yyyy-MM-dd HH:mm:ss"), DBNull.Value))
                    commandUpdate.Parameters.AddWithValue("@ID", documentId)

                    commandUpdate.ExecuteNonQuery()
                    Console.WriteLine("[DEBUG] Documento aggiornato.")
                End Using

                Using cmdDel As New Microsoft.Data.Sqlite.SqliteCommand("DELETE FROM Allegati WHERE DocumentoID = @ID", connection)
                    cmdDel.Parameters.AddWithValue("@ID", documentId)
                    cmdDel.ExecuteNonQuery()
                    Console.WriteLine("[DEBUG] Allegati precedenti eliminati.")
                End Using

                For Each fileName In allegati
                    Using cmd As New Microsoft.Data.Sqlite.SqliteCommand("INSERT INTO Allegati (DocumentoID, NomeFile, PercorsoFile) VALUES (@docId, @nome, @percorso)", connection)
                        cmd.Parameters.AddWithValue("@docId", documentId)
                        cmd.Parameters.AddWithValue("@nome", fileName)
                        cmd.Parameters.AddWithValue("@percorso", Path.Combine(documentsFolderPath, fileName))
                        cmd.ExecuteNonQuery()
                        Console.WriteLine("[DEBUG] Allegato inserito: " & fileName)
                    End Using
                Next

                Return True
            End Using
        Catch ex As Exception
            MessageBox.Show("Errore durante l'aggiornamento: " & ex.Message)
            Console.WriteLine("[DEBUG] Errore UpdateDocument: " & ex.ToString())
            Return False
        End Try
    End Function

    Private Sub WriteLogEntry(action As String, details As String)
        Console.WriteLine("[DEBUG] Avvio WriteLogEntry. Azione: " & action)

        Try
            Dim logFilePath As String = Path.Combine(documentsFolderPath, "log.txt")

            If Not Directory.Exists(documentsFolderPath) Then
                Directory.CreateDirectory(documentsFolderPath)
                Console.WriteLine("[DEBUG] Cartella documenti creata: " & documentsFolderPath)
            End If

            Dim logEntry As String = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {action} | {details}"
            Console.WriteLine("[DEBUG] Entry da scrivere: " & logEntry)

            Const maxLogFileSize As Long = 5 * 1024 * 1024 ' 5 MB
            If File.Exists(logFilePath) AndAlso New FileInfo(logFilePath).Length > maxLogFileSize Then
                Dim archivedLogPath As String = Path.Combine(documentsFolderPath, $"log_{DateTime.Now:yyyyMMddHHmmss}.txt")
                File.Move(logFilePath, archivedLogPath)
                Console.WriteLine("[DEBUG] Log archiviato: " & archivedLogPath)
            End If

            Using writer As New StreamWriter(logFilePath, append:=True)
                writer.WriteLine(logEntry)
            End Using

            Console.WriteLine("[DEBUG] Log scritto correttamente.")
        Catch ex As Exception
            MessageBox.Show($"Errore durante la scrittura del log: {ex.Message}", "Errore Log", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Console.WriteLine("[DEBUG] Errore WriteLogEntry: " & ex.ToString())
        End Try
    End Sub


    Private Function InsertDocument(filePath As String) As Boolean
        Console.WriteLine("[DEBUG] Avvio InsertDocument. File ricevuto: " & filePath)

        Try
            Using connection As New Microsoft.Data.Sqlite.SqliteConnection(connString)
                connection.Open()
                Console.WriteLine("[DEBUG] Connessione al database aperta.")

                Dim documentFileName As String = Nothing

                If Not String.IsNullOrEmpty(filePath) Then
                    If Not File.Exists(filePath) Then
                        MessageBox.Show("Il file specificato non esiste.")
                        Console.WriteLine("[DEBUG] File non trovato: " & filePath)
                        Return False
                    End If

                    documentFileName = Path.GetFileName(filePath)
                    Console.WriteLine("[DEBUG] Nome file estratto: " & documentFileName)

                    If documentFileName.Contains(" ") Then
                        MessageBox.Show("Il nome del file non può contenere spazi. Rinomina il file e riprova.", "Errore Nome File", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Console.WriteLine("[DEBUG] Nome file non valido (spazi): " & documentFileName)
                        Return False
                    End If

                    Dim destinationPath As String = Path.Combine(documentsFolderPath, documentFileName)
                    If File.Exists(destinationPath) Then
                        MessageBox.Show("Il file esiste già.")
                        Console.WriteLine("[DEBUG] File già presente: " & destinationPath)
                        Return False
                    End If

                    File.Copy(filePath, destinationPath)
                    Console.WriteLine("[DEBUG] File copiato in: " & destinationPath)
                End If

                Dim statoPagamento As String = Nothing
                If chkDaPagare.Checked Then
                    statoPagamento = "Da Pagare"
                ElseIf chkPagato.Checked Then
                    statoPagamento = "Pagato"
                End If

                Dim query As String = "INSERT INTO Documenti (UtenteID, Data, AnnoRiferimento, TipoPagamento, Importo, File, Note, StatoPagamento" &
                If(chkScadenzaAttiva.Checked, ", Scadenza", "") & ") " &
                "VALUES (@UtenteID, @Data, @AnnoRiferimento, @TipoPagamento, @Importo, @File, @Note, @StatoPagamento" &
                If(chkScadenzaAttiva.Checked, ", @Scadenza", "") & ");"

                Using command As New Microsoft.Data.Sqlite.SqliteCommand(query, connection)
                    command.Parameters.AddWithValue("@UtenteID", utenteID)
                    command.Parameters.AddWithValue("@Data", DateTimePickerData.Value.Date.ToString("yyyy-MM-dd HH:mm:ss"))
                    command.Parameters.AddWithValue("@AnnoRiferimento", TextBoxAnno.Text)
                    command.Parameters.AddWithValue("@TipoPagamento", tipoPagamento)

                    Dim importo As Decimal
                    If Not Decimal.TryParse(TextBoxImporto.Text, Globalization.NumberStyles.Any, Globalization.CultureInfo.InvariantCulture, importo) Then
                        MessageBox.Show("Importo non valido.")
                        Console.WriteLine("[DEBUG] Importo non valido: " & TextBoxImporto.Text)
                        Return False
                    End If
                    command.Parameters.AddWithValue("@Importo", importo)

                    command.Parameters.AddWithValue("@File", documentFileName)
                    command.Parameters.AddWithValue("@Note", TextBoxNote.Text)
                    command.Parameters.AddWithValue("@StatoPagamento", If(statoPagamento, DBNull.Value))
                    If chkScadenzaAttiva.Checked Then
                        command.Parameters.AddWithValue("@Scadenza", DateTimePickerScadenza.Value.ToString("yyyy-MM-dd HH:mm:ss"))
                    End If

                    command.ExecuteNonQuery()
                    Console.WriteLine("[DEBUG] Documento inserito.")
                End Using

                Dim newDocId As Long
                Using cmd As New Microsoft.Data.Sqlite.SqliteCommand("SELECT last_insert_rowid()", connection)
                    newDocId = CLng(cmd.ExecuteScalar())
                    Console.WriteLine("[DEBUG] Nuovo ID documento: " & newDocId)
                End Using

                For Each fileName In allegati
                    Using cmd As New Microsoft.Data.Sqlite.SqliteCommand("INSERT INTO Allegati (DocumentoID, NomeFile, PercorsoFile) VALUES (@docId, @nome, @percorso)", connection)
                        cmd.Parameters.AddWithValue("@docId", newDocId)
                        cmd.Parameters.AddWithValue("@nome", fileName)
                        cmd.Parameters.AddWithValue("@percorso", Path.Combine(documentsFolderPath, fileName))
                        cmd.ExecuteNonQuery()
                        Console.WriteLine("[DEBUG] Allegato inserito: " & fileName)
                    End Using
                Next

                Return True
            End Using
        Catch ex As Exception
            MessageBox.Show("Errore durante l'inserimento: " & ex.Message)
            Console.WriteLine("[DEBUG] Errore InsertDocument: " & ex.ToString())
            Return False
        End Try
    End Function


    Private Sub FormNuovoDocumento_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        Console.WriteLine("[DEBUG] FormNuovoDocumento chiuso. Verifica presenza di Form3.")

        For Each form As Form In Application.OpenForms
            If TypeOf form Is Form3 Then
                Console.WriteLine("[DEBUG] Form3 rilevato. Aggiornamento interfaccia in corso.")
                DirectCast(form, Form3).AggiornaDataGridView()
                DirectCast(form, Form3).HighlightDatesWithDocuments()
                Exit For
            End If
        Next
    End Sub


    Private Sub chkDaPagare_CheckedChanged(sender As Object, e As EventArgs) Handles chkDaPagare.CheckedChanged
        If chkDaPagare.Checked Then
            chkScadenzaAttiva.Checked = True
            chkScadenzaAttiva.Enabled = True
            chkPagato.Checked = False
            DateTimePickerScadenza.Enabled = True
            Console.WriteLine("[DEBUG] Stato: Da Pagare attivo. Scadenza abilitata.")
        Else
            chkScadenzaAttiva.Checked = False
            chkScadenzaAttiva.Enabled = True
            chkPagato.Checked = True
            DateTimePickerScadenza.Enabled = True
            Console.WriteLine("[DEBUG] Stato: Da Pagare disattivato. Pagato attivo.")
        End If
    End Sub


    Private Sub chkPagato_CheckedChanged(sender As Object, e As EventArgs) Handles chkPagato.CheckedChanged
        If chkPagato.Checked Then
            chkDaPagare.Checked = False
            chkScadenzaAttiva.Checked = False
            Console.WriteLine("[DEBUG] Stato: Pagato attivo. Da Pagare e Scadenza disattivati.")
        End If
    End Sub



    Private Sub MostraAnteprimaDocumentoDaPercorso(fullPath As String)
        Console.WriteLine("[DEBUG] Avvio anteprima documento: " & fullPath)

        panelPreview.Controls.Clear()

        If Not File.Exists(fullPath) Then
            Console.WriteLine("[DEBUG] File non trovato: " & fullPath)
            Exit Sub
        End If

        Dim ext As String = Path.GetExtension(fullPath).ToLower()
        Console.WriteLine("[DEBUG] Estensione rilevata: " & ext)

        ' --- ZIP ---
        If ext = ".zip" Then
            Console.WriteLine("[DEBUG] Anteprima ZIP.")
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

            AddHandler lstZipFiles.DoubleClick, Sub(sender2, e2)
                                                    If lstZipFiles.SelectedItem Is Nothing Then Return
                                                    Dim selectedEntry = lstZipFiles.SelectedItem.ToString()
                                                    If selectedEntry.EndsWith("/") Then Return

                                                    Dim tempDir = Path.Combine(Path.GetTempPath(), "AmministrazionePreview")
                                                    If Not Directory.Exists(tempDir) Then Directory.CreateDirectory(tempDir)
                                                    Dim tempFile = Path.Combine(tempDir, Path.GetFileName(selectedEntry))

                                                    If File.Exists(tempFile) Then File.Delete(tempFile)

                                                    Using zip As ZipArchive = ZipFile.OpenRead(fullPath)
                                                        Dim entry = zip.GetEntry(selectedEntry)
                                                        If entry IsNot Nothing Then
                                                            entry.ExtractToFile(tempFile)
                                                            Console.WriteLine("[DEBUG] File estratto da ZIP: " & tempFile)
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
            Console.WriteLine("[DEBUG] Anteprima PDF.")
            Try
                Dim webView As New Microsoft.Web.WebView2.WinForms.WebView2()
                webView.Dock = DockStyle.Fill
                webView.Source = New Uri(fullPath)
                panelPreview.Controls.Add(webView)
            Catch ex As Exception
                Console.WriteLine("[DEBUG] Errore anteprima PDF: " & ex.Message)
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
            Console.WriteLine("[DEBUG] Anteprima immagine.")
            Try
                Dim pb As New PictureBox()
                pb.Dock = DockStyle.Fill
                pb.SizeMode = PictureBoxSizeMode.Zoom
                pb.Image = Image.FromFile(fullPath)
                panelPreview.Controls.Add(pb)
            Catch ex As Exception
                Console.WriteLine("[DEBUG] Errore anteprima immagine: " & ex.Message)
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
            Console.WriteLine("[DEBUG] Anteprima file di testo.")
            Try
                Dim tb As New TextBox()
                tb.Multiline = True
                tb.ReadOnly = True
                tb.Dock = DockStyle.Fill
                tb.Text = File.ReadAllText(fullPath)
                panelPreview.Controls.Add(tb)
            Catch ex As Exception
                Console.WriteLine("[DEBUG] Errore anteprima testo: " & ex.Message)
                Dim lbl As New Label()
                lbl.Text = "Errore anteprima testo: " & ex.Message
                lbl.Dock = DockStyle.Fill
                lbl.TextAlign = ContentAlignment.MiddleCenter
                panelPreview.Controls.Add(lbl)
            End Try
            Return
        End If

        ' --- Default ---
        Console.WriteLine("[DEBUG] Nessuna anteprima disponibile per: " & ext)
        Dim lblDefault As New Label()
        lblDefault.Text = "Nessuna anteprima disponibile"
        lblDefault.Dock = DockStyle.Fill
        lblDefault.TextAlign = ContentAlignment.MiddleCenter
        panelPreview.Controls.Add(lblDefault)
    End Sub

    Private Function ReadExactly(fs As FileStream, buffer As Byte(), offset As Integer, count As Integer) As Integer
        Console.WriteLine($"[DEBUG] Avvio ReadExactly. Offset={offset}, Count={count}")
        Dim totalRead As Integer = 0

        While totalRead < count
            Dim bytesRead As Integer = fs.Read(buffer, offset + totalRead, count - totalRead)
            If bytesRead = 0 Then
                Console.WriteLine("[DEBUG] Fine stream raggiunta.")
                Exit While
            End If
            totalRead += bytesRead
            Console.WriteLine($"[DEBUG] Bytes letti: {bytesRead}, Totale: {totalRead}")
        End While

        Return totalRead
    End Function


    Private Sub TextBoxImporto_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBoxImporto.KeyPress
        Dim ch As Char = e.KeyChar
        Console.WriteLine("[DEBUG] Tasto premuto: " & ch)

        If Not Char.IsDigit(ch) AndAlso ch <> "."c AndAlso ch <> ControlChars.Back Then
            e.Handled = True
            MessageBox.Show("Inserisci solo numeri e il punto come separatore decimale.", "Formato non valido", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Console.WriteLine("[DEBUG] Carattere non valido: " & ch)
            Return
        End If

        If ch = "."c AndAlso TextBoxImporto.Text.Contains(".") Then
            e.Handled = True
            MessageBox.Show("Puoi inserire un solo punto come separatore decimale.", "Formato non valido", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Console.WriteLine("[DEBUG] Punto già presente.")
        End If
    End Sub


    ' Sposta l'allegato selezionato verso l'alto
    Private Sub btnAllegatoUp_Click(sender As Object, e As EventArgs) Handles btnAllegatoUp.Click
        Dim idx = lstAllegati.SelectedIndex
        Console.WriteLine("[DEBUG] btnAllegatoUp_Click. Indice selezionato: " & idx)

        If idx > 0 Then
            Dim item = lstAllegati.Items(idx)
            lstAllegati.Items.RemoveAt(idx)
            lstAllegati.Items.Insert(idx - 1, item)
            lstAllegati.SelectedIndex = idx - 1
            Console.WriteLine("[DEBUG] Elemento spostato in alto nella ListBox: " & item.ToString())

            Dim file = allegati(idx)
            allegati.RemoveAt(idx)
            allegati.Insert(idx - 1, file)
            Console.WriteLine("[DEBUG] Elemento spostato in alto nella lista allegati: " & file)
        Else
            Console.WriteLine("[DEBUG] Spostamento non eseguito. Indice non valido o già in cima.")
        End If
    End Sub


    ' Sposta l'allegato selezionato verso il basso
    Private Sub btnAggiungiAllegato_Click(sender As Object, e As EventArgs) Handles btnAggiungiAllegato.Click
        Console.WriteLine("[DEBUG] Avvio selezione allegati.")

        Using ofd As New OpenFileDialog()
            ofd.Filter = "Tutti i file|*.*"
            ofd.Multiselect = True

            If ofd.ShowDialog() = DialogResult.OK Then
                For Each filePath In ofd.FileNames
                    Dim fileName = Path.GetFileName(filePath)
                    Dim destPath = Path.Combine(documentsFolderPath, fileName)

                    Console.WriteLine("[DEBUG] File selezionato: " & fileName)

                    If Not File.Exists(destPath) Then
                        File.Copy(filePath, destPath)
                        Console.WriteLine("[DEBUG] File copiato in: " & destPath)
                    Else
                        Console.WriteLine("[DEBUG] File già presente: " & destPath)
                    End If

                    If Not allegati.Contains(fileName) Then
                        allegati.Add(fileName)
                        lstAllegati.Items.Add(fileName)
                        Console.WriteLine("[DEBUG] File aggiunto alla lista: " & fileName)
                    Else
                        Console.WriteLine("[DEBUG] File già presente nella lista: " & fileName)
                    End If
                Next
            Else
                Console.WriteLine("[DEBUG] Selezione file annullata.")
            End If
        End Using
    End Sub

    ' Gestione evento DragEnter
    Private Sub lstAllegati_DragEnter(sender As Object, e As DragEventArgs) Handles lstAllegati.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
            Console.WriteLine("[DEBUG] File trascinato rilevato.")
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    ' Gestione evento DragDrop
    Private Sub lstAllegati_DragDrop(sender As Object, e As DragEventArgs) Handles lstAllegati.DragDrop
        Dim files() As String = CType(e.Data.GetData(DataFormats.FileDrop), String())
        For Each filePath In files
            Dim fileName = Path.GetFileName(filePath)
            lstAllegati.Items.Add(fileName)
            Console.WriteLine("[DEBUG] File aggiunto: " & fileName)
            ' Puoi anche salvare/copiare il file in documentsFolderPath se serve
            ' File.Copy(filePath, Path.Combine(documentsFolderPath, fileName), True)
        Next
    End Sub
End Class