Imports System.Data.SQLite
Imports System.IO
Imports System.IO.Compression

Public Class Form2
    Private connectionString As String
    Private sqliteConnection As SQLiteConnection

    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Percorso del database
        Dim databasePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
        connectionString = $"Data Source={databasePath};Version=3;"

        ' Carica la tabella sqlite_sequence nella DataGridView
        LoadSqliteSequence()

        ' Carica il contenuto del file log.txt
        LoadLogFile()
    End Sub

    Private Sub LoadSqliteSequence()
        Try
            Using sqliteConnection = New SQLiteConnection(connectionString)
                sqliteConnection.Open()

                ' Query per recuperare la tabella sqlite_sequence
                Dim query As String = "SELECT * FROM sqlite_sequence"
                Dim adapter As New SQLiteDataAdapter(query, sqliteConnection)
                Dim dataTable As New DataTable()

                adapter.Fill(dataTable)
                dgvSqliteSequence.DataSource = dataTable
            End Using
        Catch ex As Exception
            MessageBox.Show($"Errore durante il caricamento della tabella sqlite_sequence: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadLogFile()
        Try
            ' Percorso del file log.txt nella stessa directory del database
            Dim logFilePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione/Documenti", "log.txt")

            ' Verifica se il file esiste
            If File.Exists(logFilePath) Then
                ' Leggi il contenuto del file e visualizzalo nella RichTextBox
                RichTextBoxLog.Text = File.ReadAllText(logFilePath)
            Else
                RichTextBoxLog.Text = "Il file log.txt non è stato trovato."
            End If
        Catch ex As Exception
            MessageBox.Show($"Errore durante il caricamento del file log.txt: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnSaveChanges_Click(sender As Object, e As EventArgs) Handles btnSaveChanges.Click
        Try
            Using sqliteConnection = New SQLiteConnection(connectionString)
                sqliteConnection.Open()

                ' Preparare un comando per aggiornare la tabella
                Dim updateQuery As String = "UPDATE sqlite_sequence SET seq = @seq WHERE name = @name"
                For Each row As DataGridViewRow In dgvSqliteSequence.Rows
                    If Not row.IsNewRow Then
                        Using cmd As New SQLiteCommand(updateQuery, sqliteConnection)
                            cmd.Parameters.AddWithValue("@name", row.Cells("name").Value)
                            cmd.Parameters.AddWithValue("@seq", row.Cells("seq").Value)
                            cmd.ExecuteNonQuery()
                        End Using
                    End If
                Next

                MessageBox.Show("Modifiche salvate con successo.", "Successo", MessageBoxButtons.OK, MessageBoxIcon.Information)
                LoadSqliteSequence() ' Ricarica i dati aggiornati
            End Using
        Catch ex As Exception
            MessageBox.Show($"Errore durante il salvataggio delle modifiche: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Sub btnBackupDatabase_Click(sender As Object, e As EventArgs) Handles btnBackupDatabase.Click
        Try
            ' Percorso del file di database
            Dim databasePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")

            ' Verifica se il file esiste
            If Not File.Exists(databasePath) Then
                MessageBox.Show("Il file del database non esiste.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            ' Apri una finestra di dialogo per salvare il file di database
            Dim saveFileDialog As New SaveFileDialog()
            saveFileDialog.Filter = "Database SQLite (*.db)|*.db"
            saveFileDialog.Title = "Salva il backup del database"

            If saveFileDialog.ShowDialog() = DialogResult.OK Then
                ' Copia il file del database nella posizione selezionata
                File.Copy(databasePath, saveFileDialog.FileName, True)
                MessageBox.Show("Backup del database completato con successo!", "Successo", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MessageBox.Show($"Errore durante il backup del database: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnBackupConfigurazione_Click(sender As Object, e As EventArgs) Handles btnBackupConfigurazione.Click
        Try
            ' Percorso della cartella "Amministrazione"
            Dim amministrazioneFolderPath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")

            ' Verifica se la cartella esiste
            If Not Directory.Exists(amministrazioneFolderPath) Then
                MessageBox.Show("La cartella 'Amministrazione' non esiste.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            ' Apri una finestra di dialogo per salvare il file ZIP
            Dim saveFileDialog As New SaveFileDialog()
            saveFileDialog.Filter = "File ZIP (*.zip)|*.zip"
            saveFileDialog.Title = "Salva il backup della configurazione"

            If saveFileDialog.ShowDialog() = DialogResult.OK Then
                ' Crea un file ZIP della cartella "Amministrazione" escludendo il file del database
                Dim zipFilePath As String = saveFileDialog.FileName
                If File.Exists(zipFilePath) Then File.Delete(zipFilePath) ' Rimuovi il file ZIP esistente

                ' Crea una copia temporanea della cartella senza il file del database
                Dim tempFolderPath As String = Path.Combine(Path.GetTempPath(), "AmministrazioneTemp")
                If Directory.Exists(tempFolderPath) Then Directory.Delete(tempFolderPath, True)
                My.Computer.FileSystem.CopyDirectory(amministrazioneFolderPath, tempFolderPath)

                ' Rimuovi il file del database dalla copia temporanea
                Dim tempDatabasePath As String = Path.Combine(tempFolderPath, "amministrazione.db")
                If File.Exists(tempDatabasePath) Then File.Delete(tempDatabasePath)

                ' Comprimi la cartella temporanea
                ZipFile.CreateFromDirectory(tempFolderPath, zipFilePath)

                ' Elimina la cartella temporanea
                Directory.Delete(tempFolderPath, True)

                MessageBox.Show("Backup della configurazione completato con successo!", "Successo", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MessageBox.Show($"Errore durante il backup della configurazione: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnCaricaDatabase_Click(sender As Object, e As EventArgs) Handles btnCaricaDatabase.Click
        Try
            ' Apri una finestra di dialogo per selezionare un file di database
            Dim openFileDialog As New OpenFileDialog
            openFileDialog.Filter = "Database SQLite (*.db)|*.db"
            openFileDialog.Title = "Seleziona un file di database"

            If openFileDialog.ShowDialog = DialogResult.OK Then
                ' Percorso del file di database selezionato
                Dim selectedDatabasePath = openFileDialog.FileName

                ' Percorso della cartella "Amministrazione"
                Dim amministrazioneFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
                Dim targetDatabasePath = Path.Combine(amministrazioneFolderPath, "amministrazione.db")

                ' Copia il file di database nella directory dell'applicazione
                File.Copy(selectedDatabasePath, targetDatabasePath, True)

                MessageBox.Show("Database caricato con successo!", "Successo", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MessageBox.Show($"Errore durante il caricamento del database: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnCaricaConfigurazione_Click(sender As Object, e As EventArgs) Handles btnCaricaConfigurazione.Click
        Try
            ' Apri una finestra di dialogo per selezionare un file ZIP
            Dim openFileDialog As New OpenFileDialog()
            openFileDialog.Filter = "File ZIP (*.zip)|*.zip"
            openFileDialog.Title = "Seleziona un file ZIP di configurazione"

            If openFileDialog.ShowDialog() = DialogResult.OK Then
                ' Percorso del file ZIP selezionato
                Dim zipFilePath As String = openFileDialog.FileName

                ' Percorso della cartella "Amministrazione"
                Dim amministrazioneFolderPath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")

                ' Estrai il contenuto del file ZIP in una directory temporanea
                Dim tempFolderPath As String = Path.Combine(Path.GetTempPath(), "AmministrazioneTemp")
                If Directory.Exists(tempFolderPath) Then
                    Directory.Delete(tempFolderPath, True)
                End If
                ZipFile.ExtractToDirectory(zipFilePath, tempFolderPath)

                ' Copia i file dalla directory temporanea alla cartella "Amministrazione", escludendo il database
                For Each filePath As String In Directory.GetFiles(tempFolderPath, "*", SearchOption.AllDirectories)
                    Dim relativePath As String = filePath.Substring(tempFolderPath.Length + 1) ' Percorso relativo
                    Dim targetPath As String = Path.Combine(amministrazioneFolderPath, relativePath)

                    ' Salta il file del database
                    If Path.GetFileName(targetPath).ToLower() = "amministrazione.db" Then
                        Continue For
                    End If

                    ' Crea la directory di destinazione se non esiste
                    Dim targetDirectory As String = Path.GetDirectoryName(targetPath)
                    If Not Directory.Exists(targetDirectory) Then
                        Directory.CreateDirectory(targetDirectory)
                    End If

                    ' Copia il file
                    File.Copy(filePath, targetPath, True)
                Next

                ' Elimina la directory temporanea
                Directory.Delete(tempFolderPath, True)

                MessageBox.Show("Configurazione caricata con successo!", "Successo", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            MessageBox.Show($"Errore durante il caricamento della configurazione: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

End Class

