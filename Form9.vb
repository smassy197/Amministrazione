Imports Microsoft.Data.Sqlite
Imports System.Diagnostics
Imports System.IO

Public Class FormInserimentoGuidato
    Private connString As String
    Private tipoPagamento As String
    Private utenteID As Integer
    Private documentsFolderPath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "Documenti")





    ' Modifica il costruttore per accettare anche l'ID utente
    Public Sub New(connString As String, tipoPagamento As String, utenteID As Integer)
        Console.WriteLine("[DEBUG] Inizializzazione form. Parametri ricevuti → connString: " & connString & ", tipoPagamento: " & tipoPagamento & ", utenteID: " & utenteID)

        InitializeComponent()

        Me.connString = connString
        Me.tipoPagamento = tipoPagamento
        Me.utenteID = utenteID

        txtUtente.Text = $"Utente selezionato: {utenteID}"
        txtTipoPagamento.Text = $"Tipo pagamento selezionato: {tipoPagamento}"

        Console.WriteLine("[DEBUG] txtUtente e txtTipoPagamento impostati.")
    End Sub


    Private Sub FormInserimentoGuidato_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Console.WriteLine("[DEBUG] FormInserimentoGuidato caricato correttamente.")

        Console.WriteLine("[DEBUG] Avvio caricamento documenti per tipo pagamento e utente.")
        LoadDocumentsForTipoPagamentoAndUtente()

        Console.WriteLine("[DEBUG] Avvio caricamento riepilogo scadenze.")
        CaricaRiepilogoScadenze()
    End Sub


    Private Sub LoadDocumentsForTipoPagamentoAndUtente()
        Console.WriteLine("[DEBUG] Avvio caricamento documenti per tipo pagamento: " & tipoPagamento & ", utenteID: " & utenteID)

        Try
            Using connection As New Microsoft.Data.Sqlite.SqliteConnection(connString)
                connection.Open()
                Console.WriteLine("[DEBUG] Connessione al database aperta.")

                Dim query As String = "
                SELECT d.ID, d.UtenteID, d.Data, d.AnnoRiferimento, d.TipoPagamento, d.Importo, d.File, d.Note, d.Scadenza, d.StatoPagamento,
                       GROUP_CONCAT(a.NomeFile, '; ') AS Allegati
                FROM Documenti d
                LEFT JOIN Allegati a ON d.ID = a.DocumentoID
                WHERE d.TipoPagamento = @TipoPagamento AND d.UtenteID = @UtenteID
                GROUP BY d.ID"

                Using command As New Microsoft.Data.Sqlite.SqliteCommand(query, connection)
                    command.Parameters.AddWithValue("@TipoPagamento", tipoPagamento)
                    command.Parameters.AddWithValue("@UtenteID", utenteID)

                    Using reader As Microsoft.Data.Sqlite.SqliteDataReader = command.ExecuteReader()
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
                        dt.Columns.Add("Allegati", GetType(String))
                        dt.Columns.Add("IconaPagamento", GetType(Image))

                        Dim iconFolderPath As String = Path.Combine(Application.StartupPath, "Icons")
                        Dim iconaPagato As Image = New Bitmap(Image.FromFile(Path.Combine(iconFolderPath, "check.png")), New Size(20, 20))
                        Dim iconaDaPagare As Image = New Bitmap(Image.FromFile(Path.Combine(iconFolderPath, "attention.png")), New Size(20, 20))
                        Dim iconaWhite As Image = New Bitmap(Image.FromFile(Path.Combine(iconFolderPath, "white.png")), New Size(20, 20))
                        Dim iconaTimer As Image = New Bitmap(Image.FromFile(Path.Combine(iconFolderPath, "timer.png")), New Size(20, 20))

                        While reader.Read()
                            Dim row As DataRow = dt.NewRow()
                            row("ID") = Convert.ToInt32(reader("ID"))
                            row("UtenteID") = Convert.ToInt32(reader("UtenteID"))
                            row("Data") = reader("Data").ToString()
                            row("AnnoRiferimento") = If(Integer.TryParse(reader("AnnoRiferimento").ToString(), Nothing), Convert.ToInt32(reader("AnnoRiferimento")), 0)
                            row("TipoPagamento") = reader("TipoPagamento").ToString()
                            row("File") = reader("File").ToString()
                            row("Note") = reader("Note").ToString()
                            row("Scadenza") = reader("Scadenza").ToString()
                            row("StatoPagamento") = reader("StatoPagamento").ToString()
                            row("Allegati") = reader("Allegati").ToString()

                            Dim importoValue As Double
                            If Double.TryParse(reader("Importo").ToString(), importoValue) Then
                                row("Importo") = importoValue
                            Else
                                row("Importo") = 0
                            End If

                            If Not IsDBNull(row("Scadenza")) AndAlso Not String.IsNullOrEmpty(row("Scadenza").ToString()) Then
                                row("IconaPagamento") = iconaTimer
                            ElseIf row("StatoPagamento").ToString() = "Pagato" Then
                                row("IconaPagamento") = iconaPagato
                            ElseIf row("StatoPagamento").ToString() = "Da Pagare" Then
                                row("IconaPagamento") = iconaDaPagare
                            Else
                                row("IconaPagamento") = iconaWhite
                            End If

                            dt.Rows.Add(row)
                        End While

                        dgvDocumenti.DataSource = dt
                        Console.WriteLine("[DEBUG] Documenti caricati: " & dt.Rows.Count)

                        If dgvDocumenti.Columns.Contains("File") Then
                            dgvDocumenti.Columns("File").Visible = False
                        End If

                        If Not dgvDocumenti.Columns.Contains("IconaPagamento") Then
                            Dim imageColumn As New DataGridViewImageColumn() With {
                            .Name = "IconaPagamento",
                            .HeaderText = "Stato",
                            .ImageLayout = DataGridViewImageCellLayout.Zoom
                        }
                            dgvDocumenti.Columns.Insert(0, imageColumn)
                            Console.WriteLine("[DEBUG] Colonna icona pagamento inserita.")
                        End If
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Errore durante il caricamento dei documenti: " & ex.Message)
            Console.WriteLine("[DEBUG] Errore caricamento documenti: " & ex.ToString())
        End Try
    End Sub


    Private Sub btnAggiungiDocumento_Click(sender As Object, e As EventArgs) Handles btnAggiungiDocumento.Click
        Console.WriteLine("[DEBUG] Click su Aggiungi Documento.")
        Dim nuovoDocumentoForm As New FormNuovoDocumento(connString, tipoPagamento, utenteID)
        nuovoDocumentoForm.ShowDialog()

        Console.WriteLine("[DEBUG] Documento inserito. Ricarico dati.")
        LoadDocumentsForTipoPagamentoAndUtente()
        CaricaRiepilogoScadenze()
    End Sub


    Private Sub btnModifica_Click(sender As Object, e As EventArgs) Handles btnModifica.Click
        Console.WriteLine("[DEBUG] Click su Modifica Documento.")

        If dgvDocumenti.SelectedRows.Count = 0 Then
            MessageBox.Show("Seleziona una riga da modificare.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Console.WriteLine("[DEBUG] Nessuna riga selezionata.")
            Return
        End If

        Dim selectedRow As DataGridViewRow = dgvDocumenti.SelectedRows(0)
        If selectedRow.Cells("ID").Value Is Nothing OrElse selectedRow.Cells("File").Value Is Nothing Then
            MessageBox.Show("La riga selezionata non è valida.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Console.WriteLine("[DEBUG] Riga selezionata non valida.")
            Return
        End If

        Dim documentId As Integer = Convert.ToInt32(selectedRow.Cells("ID").Value)
        Console.WriteLine("[DEBUG] Documento da modificare. ID: " & documentId)

        Dim modificaForm As New FormNuovoDocumento(connString, tipoPagamento, utenteID, documentId)
        modificaForm.ShowDialog()

        Console.WriteLine("[DEBUG] Documento modificato. Ricarico dati.")
        LoadDocumentsForTipoPagamentoAndUtente()
        CaricaRiepilogoScadenze()
    End Sub





    Private Sub btnElimina_Click(sender As Object, e As EventArgs) Handles btnElimina.Click
        Console.WriteLine("[DEBUG] Click su Elimina Documento.")

        If dgvDocumenti.SelectedRows.Count = 0 Then
            MessageBox.Show("Seleziona una riga da eliminare.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Console.WriteLine("[DEBUG] Nessuna riga selezionata.")
            Return
        End If

        Dim selectedRow = dgvDocumenti.SelectedRows(0)
        If selectedRow.Cells("ID").Value Is Nothing OrElse selectedRow.Cells("File").Value Is Nothing Then
            MessageBox.Show("La riga selezionata non è valida.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Console.WriteLine("[DEBUG] Riga selezionata non valida.")
            Return
        End If

        Dim documentId = Convert.ToInt32(selectedRow.Cells("ID").Value)
        Dim fileName = selectedRow.Cells("File").Value.ToString
        Console.WriteLine("[DEBUG] Documento da eliminare. ID: " & documentId & ", File: " & fileName)

        Dim result = MessageBox.Show("Sei sicuro di voler eliminare questo documento?", "Conferma Eliminazione", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
        If result = DialogResult.Yes Then
            Try
                Dim filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "Documenti", fileName)
                If File.Exists(filePath) Then
                    File.Delete(filePath)
                    Console.WriteLine("[DEBUG] File eliminato: " & filePath)
                Else
                    Console.WriteLine("[DEBUG] File non trovato: " & filePath)
                End If

                Using connection As New Microsoft.Data.Sqlite.SqliteConnection(connString)
                    connection.Open()
                    Console.WriteLine("[DEBUG] Connessione al database aperta.")

                    Dim query = "DELETE FROM Documenti WHERE ID = @ID"
                    Using command As New Microsoft.Data.Sqlite.SqliteCommand(query, connection)
                        command.Parameters.AddWithValue("@ID", documentId)
                        command.ExecuteNonQuery()
                        Console.WriteLine("[DEBUG] Record eliminato dal database. ID: " & documentId)
                    End Using
                End Using

                MessageBox.Show("Documento eliminato con successo.")
                LoadDocumentsForTipoPagamentoAndUtente()
                CaricaRiepilogoScadenze()
            Catch ex As Exception
                MessageBox.Show("Errore durante l'eliminazione del documento: " & ex.Message)
                Console.WriteLine("[DEBUG] Errore eliminazione documento: " & ex.ToString())
            End Try
        Else
            Console.WriteLine("[DEBUG] Eliminazione annullata dall'utente.")
        End If
    End Sub

    Private Sub btnApriFile_Click(sender As Object, e As EventArgs) Handles btnApriFile.Click
        Console.WriteLine("[DEBUG] Click su Apri File.")

        If dgvDocumenti.SelectedRows.Count = 0 Then
            Dim errorMessage As String = "Seleziona una riga per aprire il file."
            MessageBox.Show(errorMessage, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Console.WriteLine("[DEBUG] " & errorMessage)
            Return
        End If

        Dim selectedRow As DataGridViewRow = dgvDocumenti.SelectedRows(0)
        If selectedRow.Cells("ID").Value Is Nothing OrElse selectedRow.Cells("File").Value Is Nothing Then
            MessageBox.Show("La riga selezionata non è valida.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Console.WriteLine("[DEBUG] Riga selezionata non valida.")
            Return
        End If

        Dim fileName As String = selectedRow.Cells("File").Value.ToString()
        Dim filePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "Documenti", fileName)
        Console.WriteLine("[DEBUG] Percorso file: " & filePath)

        If Not File.Exists(filePath) Then
            Dim errorMessage As String = $"Il file selezionato non esiste: {filePath}"
            MessageBox.Show(errorMessage, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Console.WriteLine("[DEBUG] " & errorMessage)
            Return
        End If

        Try
            Dim psi As New ProcessStartInfo() With {
            .FileName = filePath,
            .UseShellExecute = True
        }
            Process.Start(psi)
            Console.WriteLine("[DEBUG] File aperto correttamente.")
        Catch ex As System.ComponentModel.Win32Exception
            Dim errorMessage As String = $"Errore durante l'apertura del file: {ex.Message}. Verifica che esista un'applicazione predefinita per aprire il file."
            MessageBox.Show(errorMessage, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Console.WriteLine("[DEBUG] " & errorMessage)
        Catch ex As Exception
            Dim errorMessage As String = $"Errore imprevisto durante l'apertura del file: {ex.Message}"
            MessageBox.Show(errorMessage, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Console.WriteLine("[DEBUG] " & errorMessage)
        End Try
    End Sub

    Private Sub FormInserimentoGuidato_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        Console.WriteLine("[DEBUG] FormInserimentoGuidato chiuso. Verifica presenza di Form3.")

        For Each form As Form In Application.OpenForms
            If TypeOf form Is Form3 Then
                Console.WriteLine("[DEBUG] Form3 rilevato. Eseguo aggiornamento controlli.")
                DirectCast(form, Form3).ResetFormControls()
                DirectCast(form, Form3).AggiornaDataGridView()
                DirectCast(form, Form3).HighlightDatesWithDocuments()
                Exit For
            End If
        Next
    End Sub


    'timer scadenza e menu contestuale-----------------------------------------------------------------------
    Private Sub TimerScadenze_Tick(sender As Object, e As EventArgs) Handles TimerScadenze.Tick
        Dim now As DateTime = DateTime.Now
        Dim righeScadute As Integer = 0
        Console.WriteLine("[DEBUG] TimerScadenze_Tick avviato. Ora: " & now.ToString("yyyy-MM-dd HH:mm:ss"))

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
                    Console.WriteLine($"[DEBUG] Documento con scadenza {scadenza:yyyy-MM-dd} → {giorniRimanenti} giorni rimanenti.")

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
                    Console.WriteLine("[DEBUG] Scadenza non valida: " & scadenzaStr)
                End If
            Else
                scadenzaCell.Style.BackColor = Color.White
            End If
        Next

        lblNotifica.Font = New Font(lblNotifica.Font, FontStyle.Bold)
        lblNotifica.ForeColor = Color.Red
        lblNotifica.Text = righeScadute.ToString()
        lblNotifica.Visible = (righeScadute > 0)
        picNotifica.Visible = (righeScadute > 0)

        Console.WriteLine("[DEBUG] Righe scadute: " & righeScadute)
    End Sub

    '--------------------------------------------------------------------------------------------------------

    Private Sub CaricaRiepilogoScadenze()
        Console.WriteLine("[DEBUG] Avvio CaricaRiepilogoScadenze.")

        Try
            Using connection As New Microsoft.Data.Sqlite.SqliteConnection(connString)
                connection.Open()
                Console.WriteLine("[DEBUG] Connessione al database aperta.")

                Dim query As String = "SELECT ID, Note, Scadenza, TipoPagamento, UtenteID FROM Documenti WHERE Scadenza IS NOT NULL AND Scadenza >= @Oggi ORDER BY Scadenza ASC"

                Using command As New Microsoft.Data.Sqlite.SqliteCommand(query, connection)
                    command.Parameters.AddWithValue("@Oggi", DateTime.Now.ToString("yyyy-MM-dd"))

                    Using reader As Microsoft.Data.Sqlite.SqliteDataReader = command.ExecuteReader()
                        lstRiepilogoScadenze.Items.Clear()
                        Console.WriteLine("[DEBUG] Riepilogo scadenze pulito.")

                        While reader.Read()
                            Dim id As Integer = Convert.ToInt32(reader("ID"))
                            Dim nota As String = reader("Note").ToString()
                            Dim scadenza As DateTime = Convert.ToDateTime(reader("Scadenza"))
                            Dim tipoPagamento As String = reader("TipoPagamento").ToString()
                            Dim utenteId As Integer = Convert.ToInt32(reader("UtenteID"))
                            Dim giorniRimanenti As Integer = (scadenza - DateTime.Now).Days

                            Dim riepilogo As String = $"{id} - {scadenza:dd/MM/yyyy} - {nota} ({giorniRimanenti} giorni rimanenti) - {tipoPagamento} - Utente: {utenteId}"
                            lstRiepilogoScadenze.Items.Add(riepilogo)
                            Console.WriteLine("[DEBUG] Aggiunto riepilogo: " & riepilogo)
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show("Errore durante il caricamento del riepilogo delle scadenze: " & ex.Message)
            Console.WriteLine("[DEBUG] Errore caricamento scadenze: " & ex.ToString())
        End Try
    End Sub

    Private Sub lstRiepilogoScadenze_DoubleClick(sender As Object, e As EventArgs) Handles lstRiepilogoScadenze.DoubleClick
        Console.WriteLine("[DEBUG] Doppio click su riepilogo scadenze.")

        If lstRiepilogoScadenze.SelectedItem IsNot Nothing Then
            Dim selectedItem As String = lstRiepilogoScadenze.SelectedItem.ToString()
            Console.WriteLine("[DEBUG] Elemento selezionato: " & selectedItem)

            Try
                Dim parts As String() = selectedItem.Split("-"c)
                Dim id As Integer = Convert.ToInt32(parts(0).Trim())
                Dim tipoPagamento As String = parts(parts.Length - 2).Trim()
                Dim utenteId As Integer = Convert.ToInt32(parts.Last().Replace("Utente:", "").Trim())

                Console.WriteLine($"[DEBUG] ID: {id}, TipoPagamento: {tipoPagamento}, UtenteID: {utenteId}")

                Dim result As DialogResult = MessageBox.Show($"Vuoi aprire la scheda per il pagamento associato a: {tipoPagamento} dell'utente {utenteId}?", "Conferma", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

                If result = DialogResult.Yes Then
                    Console.WriteLine("[DEBUG] Conferma ricevuta. Apro FormInserimentoGuidato e FormNuovoDocumento.")
                    Dim nuovoForm As New FormInserimentoGuidato(connString, tipoPagamento, utenteId)
                    nuovoForm.Show()

                    Dim modificaForm As New FormNuovoDocumento(connString, tipoPagamento, utenteId, id)
                    modificaForm.ShowDialog()
                Else
                    Console.WriteLine("[DEBUG] Apertura annullata dall'utente.")
                End If
            Catch ex As Exception
                MessageBox.Show("Errore nell'interpretazione dell'elemento selezionato: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Console.WriteLine("[DEBUG] Errore parsing elemento: " & ex.ToString())
            End Try
        Else
            Console.WriteLine("[DEBUG] Nessun elemento selezionato.")
        End If
    End Sub

End Class