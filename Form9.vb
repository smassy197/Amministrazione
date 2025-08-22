Imports System.Data.SQLite
Imports System.IO
Imports System.Diagnostics

Public Class FormInserimentoGuidato
    Private connString As String
    Private tipoPagamento As String
    Private utenteID As Integer
    Private documentsFolderPath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "Documenti")





    ' Modifica il costruttore per accettare anche l'ID utente
    Public Sub New(connString As String, tipoPagamento As String, utenteID As Integer)
        InitializeComponent()
        Me.connString = connString
        Me.tipoPagamento = tipoPagamento
        Me.utenteID = utenteID
        ' Imposta il valore della TextBox per ricordare l'utente
        txtUtente.Text = $"Utente selezionato: {utenteID}"
        txtTipoPagamento.Text = $"Tipo pagamento selezionato: {tipoPagamento}"
    End Sub

    Private Sub FormInserimentoGuidato_Load(sender As Object, e As EventArgs) Handles MyBase.Load



        Debug.WriteLine("Il form è stato caricato correttamente.")

        ' Mostra i documenti esistenti per il tipo selezionato
        LoadDocumentsForTipoPagamentoAndUtente()


        ' Carica il riepilogo delle scadenze
        CaricaRiepilogoScadenze()
    End Sub

    Private Sub LoadDocumentsForTipoPagamentoAndUtente()
        Try
            Using connection As New SQLiteConnection(connString)
                connection.Open()

                Dim query As String = "SELECT d.*, GROUP_CONCAT(a.NomeFile, '; ') AS Allegati " &
                      "FROM Documenti d " &
                      "LEFT JOIN Allegati a ON d.ID = a.DocumentoID " &
                      "WHERE d.TipoPagamento = @TipoPagamento AND d.UtenteID = @UtenteID " &
                         "GROUP BY d.ID"
                Using command As New SQLiteCommand(query, connection)
                    command.Parameters.AddWithValue("@TipoPagamento", tipoPagamento)
                    command.Parameters.AddWithValue("@UtenteID", utenteID)

                    Using reader As SQLiteDataReader = command.ExecuteReader()
                        Dim dt As New DataTable()
                        dt.Load(reader)

                        ' Aggiungi una colonna immagine per lo stato del pagamento
                        If Not dt.Columns.Contains("IconaPagamento") Then
                            dt.Columns.Add("IconaPagamento", GetType(Image))
                        End If

                        ' Ottieni il percorso della cartella delle icone
                        Dim iconFolderPath As String = Path.Combine(Application.StartupPath, "Icons")

                        ' Carica le icone dai percorsi assoluti
                        Dim originalIconPagato As Image = Image.FromFile(Path.Combine(iconFolderPath, "check.png"))
                        Dim iconaPagato As Image = New Bitmap(originalIconPagato, New Size(20, 20)) ' Ridimensiona a 20x20 pixel

                        Dim originalIconDaPagare As Image = Image.FromFile(Path.Combine(iconFolderPath, "attention.png"))
                        Dim iconaDaPagare As Image = New Bitmap(originalIconDaPagare, New Size(20, 20)) ' Ridimensiona a 20x20 pixel

                        Dim originalIconWhite As Image = Image.FromFile(Path.Combine(iconFolderPath, "white.png"))
                        Dim iconaWhite As Image = New Bitmap(originalIconWhite, New Size(20, 20)) ' Ridimensiona a 20x20 pixel

                        Dim originalIconTimer As Image = Image.FromFile(Path.Combine(iconFolderPath, "timer.png"))
                        Dim iconaTimer As Image = New Bitmap(originalIconTimer, New Size(20, 20)) ' Ridimensiona a 20x20 pixel

                        ' Assegna le icone in base allo stato del pagamento e alla scadenza
                        For Each row As DataRow In dt.Rows
                            If Not IsDBNull(row("Scadenza")) AndAlso Not String.IsNullOrEmpty(row("Scadenza").ToString()) Then
                                ' Assegna l'icona timer.png se la scadenza è compilata
                                row("IconaPagamento") = iconaTimer
                            ElseIf row("StatoPagamento").ToString() = "Pagato" Then
                                row("IconaPagamento") = iconaPagato
                            ElseIf row("StatoPagamento").ToString() = "Da Pagare" Then
                                row("IconaPagamento") = iconaDaPagare
                            Else
                                ' Assegna l'icona white.png se lo stato non è specificato
                                row("IconaPagamento") = iconaWhite
                            End If
                        Next




                        ' Imposta la DataGridView
                        dgvDocumenti.DataSource = dt

                        ' Configura la colonna immagine
                        ' Nascondi la colonna File (obsoleta)
                        If dgvDocumenti.Columns.Contains("File") Then
                            dgvDocumenti.Columns("File").Visible = False
                        End If
                        If Not dgvDocumenti.Columns.Contains("IconaPagamento") Then
                            Dim imageColumn As New DataGridViewImageColumn()
                            imageColumn.Name = "IconaPagamento"
                            imageColumn.HeaderText = "Stato"
                            imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom
                            dgvDocumenti.Columns.Insert(0, imageColumn)
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Errore durante il caricamento dei documenti: " & ex.Message)
        End Try
    End Sub

    Private Sub btnAggiungiDocumento_Click(sender As Object, e As EventArgs) Handles btnAggiungiDocumento.Click
        ' Logica per aggiungere un nuovo documento
        Dim nuovoDocumentoForm As New FormNuovoDocumento(connString, tipoPagamento, utenteID)
        nuovoDocumentoForm.ShowDialog()

        ' Ricarica i documenti dopo l'inserimento
        LoadDocumentsForTipoPagamentoAndUtente()
        CaricaRiepilogoScadenze()
    End Sub

    Private Sub btnModifica_Click(sender As Object, e As EventArgs) Handles btnModifica.Click
        ' Verifica che una riga sia selezionata
        If dgvDocumenti.SelectedRows.Count = 0 Then
            MessageBox.Show("Seleziona una riga da modificare.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Ottieni l'ID del documento selezionato
        Dim selectedRow As DataGridViewRow = dgvDocumenti.SelectedRows(0)
        If selectedRow.Cells("ID").Value Is Nothing OrElse selectedRow.Cells("File").Value Is Nothing Then
            MessageBox.Show("La riga selezionata non è valida.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Dim documentId As Integer = Convert.ToInt32(selectedRow.Cells("ID").Value)

        ' Apri FormNuovoDocumento in modalità modifica
        Dim modificaForm As New FormNuovoDocumento(connString, tipoPagamento, utenteID, documentId)
        modificaForm.ShowDialog()

        ' Ricarica i documenti dopo la modifica
        LoadDocumentsForTipoPagamentoAndUtente()
        CaricaRiepilogoScadenze()
    End Sub




    Private Sub btnElimina_Click(sender As Object, e As EventArgs) Handles btnElimina.Click
        ' Verifica che una riga sia selezionata
        If dgvDocumenti.SelectedRows.Count = 0 Then
            MessageBox.Show("Seleziona una riga da eliminare.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim selectedRow = dgvDocumenti.SelectedRows(0)
        If selectedRow.Cells("ID").Value Is Nothing OrElse selectedRow.Cells("File").Value Is Nothing Then
            MessageBox.Show("La riga selezionata non è valida.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Dim documentId = Convert.ToInt32(selectedRow.Cells("ID").Value)
        Dim fileName = selectedRow.Cells("File").Value.ToString

        ' Conferma l'eliminazione
        Dim result = MessageBox.Show("Sei sicuro di voler eliminare questo documento?", "Conferma Eliminazione", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
        If result = DialogResult.Yes Then
            Try
                ' Elimina il file dal file system
                Dim filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "Documenti", fileName)
                If File.Exists(filePath) Then
                    File.Delete(filePath)
                    Debug.WriteLine($"File eliminato: {filePath}")
                Else
                    Debug.WriteLine($"File non trovato: {filePath}")
                End If

                ' Elimina il record dal database
                Using connection As New SQLiteConnection(connString)
                    connection.Open()

                    Dim query = "DELETE FROM Documenti WHERE ID = @ID"
                    Using command As New SQLiteCommand(query, connection)
                        command.Parameters.AddWithValue("@ID", documentId)
                        command.ExecuteNonQuery()
                    End Using
                End Using

                MessageBox.Show("Documento eliminato con successo.")
                LoadDocumentsForTipoPagamentoAndUtente()
                CaricaRiepilogoScadenze()
            Catch ex As Exception
                MessageBox.Show("Errore durante l'eliminazione del documento: " & ex.Message)
            End Try
        End If
    End Sub

    Private Sub btnApriFile_Click(sender As Object, e As EventArgs) Handles btnApriFile.Click
        ' Verifica che una riga sia selezionata
        If dgvDocumenti.SelectedRows.Count = 0 Then
            Dim errorMessage As String = "Seleziona una riga per aprire il file."
            MessageBox.Show(errorMessage, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Debug.WriteLine(errorMessage)
            Return
        End If

        Dim selectedRow As DataGridViewRow = dgvDocumenti.SelectedRows(0)
        If selectedRow.Cells("ID").Value Is Nothing OrElse selectedRow.Cells("File").Value Is Nothing Then
            MessageBox.Show("La riga selezionata non è valida.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Dim fileName As String = selectedRow.Cells("File").Value.ToString()

        ' Verifica che il file esista
        Dim filePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "Documenti", fileName)
        If Not File.Exists(filePath) Then
            Dim errorMessage As String = $"Il file selezionato non esiste: {filePath}"
            MessageBox.Show(errorMessage, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Debug.WriteLine(errorMessage)
            Return
        End If

        ' Apri il file
        Try
            Dim psi As New ProcessStartInfo()
            psi.FileName = filePath
            psi.UseShellExecute = True ' Usa l'applicazione predefinita per aprire il file
            Process.Start(psi)
        Catch ex As System.ComponentModel.Win32Exception
            Dim errorMessage As String = $"Errore durante l'apertura del file: {ex.Message}. Verifica che esista un'applicazione predefinita per aprire il file."
            MessageBox.Show(errorMessage, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Debug.WriteLine(errorMessage)
        Catch ex As Exception
            Dim errorMessage As String = $"Errore imprevisto durante l'apertura del file: {ex.Message}"
            MessageBox.Show(errorMessage, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Debug.WriteLine(errorMessage)
        End Try
    End Sub

    Private Sub FormInserimentoGuidato_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        ' Verifica se Form3 è aperto
        For Each form As Form In Application.OpenForms
            If TypeOf form Is Form3 Then
                'richama il metodo per aggiornare il reset
                DirectCast(form, Form3).ResetFormControls()
                ' Richiama il metodo per aggiornare la DataGridView
                DirectCast(form, Form3).AggiornaDataGridView()

                ' Richiama il metodo per aggiornare il MonthCalendar
                DirectCast(form, Form3).HighlightDatesWithDocuments()


                Exit For
            End If
        Next
    End Sub

    'timer scadenza e menu contestuale-----------------------------------------------------------------------
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
    '--------------------------------------------------------------------------------------------------------

    Private Sub CaricaRiepilogoScadenze()
        Try
            Using connection As New SQLiteConnection(connString)
                connection.Open()

                ' Query per ottenere le scadenze imminenti
                Dim query As String = "SELECT ID, Note, Scadenza, TipoPagamento, UtenteID FROM Documenti WHERE Scadenza IS NOT NULL AND Scadenza >= @Oggi ORDER BY Scadenza ASC"

                Using command As New SQLiteCommand(query, connection)
                    command.Parameters.AddWithValue("@Oggi", DateTime.Now.ToString("yyyy-MM-dd"))

                    Using reader As SQLiteDataReader = command.ExecuteReader()
                        ' Pulisci il controllo prima di aggiungere nuovi dati
                        lstRiepilogoScadenze.Items.Clear()

                        While reader.Read()
                            Dim id As Integer = Convert.ToInt32(reader("ID"))
                            Dim nota As String = reader("Note").ToString()
                            Dim scadenza As DateTime = Convert.ToDateTime(reader("Scadenza"))
                            Dim tipoPagamento As String = reader("TipoPagamento").ToString()
                            Dim utenteId As Integer = Convert.ToInt32(reader("UtenteID"))
                            Dim giorniRimanenti As Integer = (scadenza - DateTime.Now).Days

                            ' Aggiungi un elemento al riepilogo con l'ID utente incluso
                            lstRiepilogoScadenze.Items.Add($"{id} - {scadenza:dd/MM/yyyy} - {nota} ({giorniRimanenti} giorni rimanenti) - {tipoPagamento} - Utente: {utenteId}")
                        End While



                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Errore durante il caricamento del riepilogo delle scadenze: " & ex.Message)
        End Try
    End Sub

    Private Sub lstRiepilogoScadenze_DoubleClick(sender As Object, e As EventArgs) Handles lstRiepilogoScadenze.DoubleClick
        ' Verifica che un elemento sia selezionato
        If lstRiepilogoScadenze.SelectedItem IsNot Nothing Then
            ' Ottieni il testo dell'elemento selezionato
            Dim selectedItem As String = lstRiepilogoScadenze.SelectedItem.ToString()

            ' Estrai l'ID, il tipo di pagamento e l'ID utente
            ' Supponiamo che il formato sia "ID - dd/MM/yyyy - Nota (X giorni rimanenti) - TipoPagamento - Utente: UtenteID"
            Dim parts As String() = selectedItem.Split("-"c)
            Dim id As Integer = Convert.ToInt32(parts(0).Trim())
            Dim tipoPagamento As String = parts(parts.Length - 2).Trim()
            Dim utenteId As Integer = Convert.ToInt32(parts.Last().Replace("Utente:", "").Trim())

            ' Chiedi conferma all'utente
            Dim result As DialogResult = MessageBox.Show($"Vuoi aprire la scheda per il pagamento associato a: {tipoPagamento} dell'utente {utenteId}?", "Conferma", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

            If result = DialogResult.Yes Then
                ' Apri una nuova istanza di FormInserimentoGuidato per il tipo di pagamento e l'utente corretto
                Dim nuovoForm As New FormInserimentoGuidato(connString, tipoPagamento, utenteId)
                nuovoForm.Show()

                ' Apri l'istanza di modifica per il pagamento specifico
                Dim modificaForm As New FormNuovoDocumento(connString, tipoPagamento, utenteId, id)
                modificaForm.ShowDialog()
            End If
        End If
    End Sub

End Class