Imports System.IO
Imports System.IO.Compression
Imports Microsoft.Data.Sqlite
Public Class Form2
    Private connectionString As String
    Private sqliteConnection As SQLiteConnection

    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Percorso del database
        Dim databasePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
        connectionString = $"Data Source={databasePath}"

        ' Carica la tabella sqlite_sequence nella DataGridView
        LoadSqliteSequence()

        ' Carica il contenuto del file log.txt
        LoadLogFile()
    End Sub

    Private Sub LoadSqliteSequence()
        Console.WriteLine("[DEBUG] Avvio caricamento tabella sqlite_sequence.")

        Try
            Using sqliteConnection As New SqliteConnection(connectionString)
                sqliteConnection.Open()
                Console.WriteLine("[DEBUG] Connessione aperta.")

                Dim query As String = "SELECT name, seq FROM sqlite_sequence"
                Console.WriteLine("[DEBUG] Query: " & query)

                Using cmd As New SqliteCommand(query, sqliteConnection)
                    Using reader As SqliteDataReader = cmd.ExecuteReader()
                        Dim dataTable As New DataTable()
                        dataTable.Load(reader)
                        Console.WriteLine("[DEBUG] Righe caricate: " & dataTable.Rows.Count)

                        dgvSqliteSequence.DataSource = dataTable
                        Console.WriteLine("[DEBUG] DataGridView aggiornato.")
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore: " & ex.ToString())
            MessageBox.Show($"Errore durante il caricamento della tabella sqlite_sequence: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Sub LoadLogFile()
        Console.WriteLine("[DEBUG] Avvio caricamento file log.txt")

        Try
            ' Percorso del file log.txt nella directory Documenti dell'app
            Dim logFilePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "Documenti", "log.txt")
            Console.WriteLine("[DEBUG] Percorso file: " & logFilePath)

            If File.Exists(logFilePath) Then
                RichTextBoxLog.Text = File.ReadAllText(logFilePath)
                Console.WriteLine("[DEBUG] File log.txt caricato correttamente.")
            Else
                RichTextBoxLog.Text = "Il file log.txt non è stato trovato."
                Console.WriteLine("[DEBUG] File log.txt non trovato.")
            End If
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore: " & ex.ToString())
            MessageBox.Show($"Errore durante il caricamento del file log.txt: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Sub btnSaveChanges_Click(sender As Object, e As EventArgs) Handles btnSaveChanges.Click
        Console.WriteLine("[DEBUG] Avvio salvataggio modifiche su sqlite_sequence.")

        Try
            Using sqliteConnection As New SqliteConnection(connectionString)
                sqliteConnection.Open()
                Console.WriteLine("[DEBUG] Connessione al database aperta.")

                Dim updateQuery As String = "UPDATE sqlite_sequence SET seq = @seq WHERE name = @name"
                Console.WriteLine("[DEBUG] Query di aggiornamento: " & updateQuery)

                Dim modificheEseguite As Integer = 0

                For Each row As DataGridViewRow In dgvSqliteSequence.Rows
                    If Not row.IsNewRow Then
                        Dim nameValue = row.Cells("name").Value?.ToString()
                        Dim seqValue = row.Cells("seq").Value?.ToString()

                        If Not String.IsNullOrEmpty(nameValue) AndAlso Not String.IsNullOrEmpty(seqValue) Then
                            Using cmd As New SqliteCommand(updateQuery, sqliteConnection)
                                cmd.Parameters.AddWithValue("@name", nameValue)
                                cmd.Parameters.AddWithValue("@seq", seqValue)
                                modificheEseguite += cmd.ExecuteNonQuery()
                                Console.WriteLine($"[DEBUG] Aggiornato: name={nameValue}, seq={seqValue}")
                            End Using
                        Else
                            Console.WriteLine("[DEBUG] Riga ignorata: valori nulli o vuoti.")
                        End If
                    End If
                Next

                MessageBox.Show("Modifiche salvate con successo.", "Successo", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Console.WriteLine("[DEBUG] Modifiche totali eseguite: " & modificheEseguite)

                LoadSqliteSequence()
            End Using
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore: " & ex.ToString())
            MessageBox.Show($"Errore durante il salvataggio delle modifiche: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub



    Private Sub btnBackupDatabase_Click(sender As Object, e As EventArgs) Handles btnBackupDatabase.Click
        Console.WriteLine("[DEBUG] Avvio procedura di backup database.")

        Try
            Dim databasePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
            Console.WriteLine("[DEBUG] Percorso database: " & databasePath)

            If Not File.Exists(databasePath) Then
                Console.WriteLine("[DEBUG] File database non trovato.")
                MessageBox.Show("Il file del database non esiste.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Dim saveFileDialog As New SaveFileDialog()
            saveFileDialog.Filter = "Database SQLite (*.db)|*.db"
            saveFileDialog.Title = "Salva il backup del database"

            If saveFileDialog.ShowDialog() = DialogResult.OK Then
                File.Copy(databasePath, saveFileDialog.FileName, True)
                Console.WriteLine("[DEBUG] Backup salvato in: " & saveFileDialog.FileName)
                MessageBox.Show("Backup del database completato con successo!", "Successo", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                Console.WriteLine("[DEBUG] Operazione di salvataggio annullata dall'utente.")
            End If
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore: " & ex.ToString())
            MessageBox.Show($"Errore durante il backup del database: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Sub btnBackupConfigurazione_Click(sender As Object, e As EventArgs) Handles btnBackupConfigurazione.Click
        Console.WriteLine("[DEBUG] Avvio backup configurazione cartella 'Amministrazione'.")

        Try
            Dim amministrazioneFolderPath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
            Console.WriteLine("[DEBUG] Percorso cartella: " & amministrazioneFolderPath)

            If Not Directory.Exists(amministrazioneFolderPath) Then
                Console.WriteLine("[DEBUG] Cartella non trovata.")
                MessageBox.Show("La cartella 'Amministrazione' non esiste.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Dim saveFileDialog As New SaveFileDialog() With {
            .Filter = "File ZIP (*.zip)|*.zip",
            .Title = "Salva il backup della configurazione"
        }

            If saveFileDialog.ShowDialog() = DialogResult.OK Then
                Dim zipFilePath As String = saveFileDialog.FileName
                Console.WriteLine("[DEBUG] Percorso ZIP selezionato: " & zipFilePath)

                If File.Exists(zipFilePath) Then
                    File.Delete(zipFilePath)
                    Console.WriteLine("[DEBUG] ZIP esistente eliminato.")
                End If

                Dim tempFolderPath As String = Path.Combine(Path.GetTempPath(), "AmministrazioneTemp")
                If Directory.Exists(tempFolderPath) Then Directory.Delete(tempFolderPath, True)
                Console.WriteLine("[DEBUG] Cartella temporanea: " & tempFolderPath)

                My.Computer.FileSystem.CopyDirectory(amministrazioneFolderPath, tempFolderPath)
                Console.WriteLine("[DEBUG] Copia cartella completata.")

                Dim tempDatabasePath As String = Path.Combine(tempFolderPath, "amministrazione.db")
                If File.Exists(tempDatabasePath) Then
                    File.Delete(tempDatabasePath)
                    Console.WriteLine("[DEBUG] File database escluso dal backup.")
                End If

                ZipFile.CreateFromDirectory(tempFolderPath, zipFilePath)
                Console.WriteLine("[DEBUG] Compressione completata.")

                Directory.Delete(tempFolderPath, True)
                Console.WriteLine("[DEBUG] Cartella temporanea eliminata.")

                MessageBox.Show("Backup della configurazione completato con successo!", "Successo", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                Console.WriteLine("[DEBUG] Operazione annullata dall'utente.")
            End If
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore: " & ex.ToString())
            MessageBox.Show($"Errore durante il backup della configurazione: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Sub btnCaricaDatabase_Click(sender As Object, e As EventArgs) Handles btnCaricaDatabase.Click
        Console.WriteLine("[DEBUG] Avvio procedura di caricamento database.")

        Try
            Dim openFileDialog As New OpenFileDialog With {
            .Filter = "Database SQLite (*.db)|*.db",
            .Title = "Seleziona un file di database"
        }

            If openFileDialog.ShowDialog() = DialogResult.OK Then
                Dim selectedDatabasePath = openFileDialog.FileName
                Console.WriteLine("[DEBUG] File selezionato: " & selectedDatabasePath)

                Dim amministrazioneFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
                Console.WriteLine("[DEBUG] Cartella destinazione: " & amministrazioneFolderPath)

                If Not Directory.Exists(amministrazioneFolderPath) Then
                    Directory.CreateDirectory(amministrazioneFolderPath)
                    Console.WriteLine("[DEBUG] Cartella 'Amministrazione' creata.")
                End If

                Dim targetDatabasePath = Path.Combine(amministrazioneFolderPath, "amministrazione.db")
                File.Copy(selectedDatabasePath, targetDatabasePath, True)
                Console.WriteLine("[DEBUG] Database copiato in: " & targetDatabasePath)

                MessageBox.Show("Database caricato con successo!", "Successo", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                Console.WriteLine("[DEBUG] Operazione annullata dall'utente.")
            End If
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore: " & ex.ToString())
            MessageBox.Show($"Errore durante il caricamento del database: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Private Sub btnCaricaConfigurazione_Click(sender As Object, e As EventArgs) Handles btnCaricaConfigurazione.Click
        Console.WriteLine("[DEBUG] Avvio procedura di caricamento configurazione ZIP.")

        Try
            Dim openFileDialog As New OpenFileDialog() With {
            .Filter = "File ZIP (*.zip)|*.zip",
            .Title = "Seleziona un file ZIP di configurazione"
        }

            If openFileDialog.ShowDialog() = DialogResult.OK Then
                Dim zipFilePath As String = openFileDialog.FileName
                Console.WriteLine("[DEBUG] File ZIP selezionato: " & zipFilePath)

                Dim amministrazioneFolderPath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
                Console.WriteLine("[DEBUG] Cartella destinazione: " & amministrazioneFolderPath)

                Dim tempFolderPath As String = Path.Combine(Path.GetTempPath(), "AmministrazioneTemp")
                If Directory.Exists(tempFolderPath) Then
                    Directory.Delete(tempFolderPath, True)
                    Console.WriteLine("[DEBUG] Cartella temporanea preesistente eliminata.")
                End If

                ZipFile.ExtractToDirectory(zipFilePath, tempFolderPath)
                Console.WriteLine("[DEBUG] File ZIP estratto in: " & tempFolderPath)

                For Each filePath As String In Directory.GetFiles(tempFolderPath, "*", SearchOption.AllDirectories)
                    Dim relativePath As String = filePath.Substring(tempFolderPath.Length + 1)
                    Dim targetPath As String = Path.Combine(amministrazioneFolderPath, relativePath)

                    If Path.GetFileName(targetPath).ToLower() = "amministrazione.db" Then
                        Console.WriteLine("[DEBUG] File 'amministrazione.db' ignorato.")
                        Continue For
                    End If

                    Dim targetDirectory As String = Path.GetDirectoryName(targetPath)
                    If Not Directory.Exists(targetDirectory) Then
                        Directory.CreateDirectory(targetDirectory)
                        Console.WriteLine("[DEBUG] Creata directory: " & targetDirectory)
                    End If

                    File.Copy(filePath, targetPath, True)
                    Console.WriteLine("[DEBUG] File copiato: " & targetPath)
                Next

                Directory.Delete(tempFolderPath, True)
                Console.WriteLine("[DEBUG] Cartella temporanea eliminata.")

                MessageBox.Show("Configurazione caricata con successo!", "Successo", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                Console.WriteLine("[DEBUG] Operazione annullata dall'utente.")
            End If
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore: " & ex.ToString())
            MessageBox.Show($"Errore durante il caricamento della configurazione: {ex.Message}", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

End Class

