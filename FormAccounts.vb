Imports System
Imports System.Data
Imports System.Drawing
Imports System.Globalization
Imports System.Windows.Forms
Imports System.IO
Imports Microsoft.Data.Sqlite

Public Class FormAccounts
    Inherits Form

    Private dgvBank As DataGridView
    Private txtBankName As TextBox
    Private txtInitialBalance As TextBox
    Private btnAdd As Button
    Private btnSave As Button
    Private btnDelete As Button
    Private btnClose As Button

    Public Sub New()
        Console.WriteLine("[FormAccounts] Costruttore: inizializzazione form.")
        Me.Text = "Gestione Conti Bancari"
        Me.Size = New Size(600, 400)
        Me.StartPosition = FormStartPosition.CenterParent
        InitializeComponents()
    End Sub

    Private Sub InitializeComponents()
        Console.WriteLine("[FormAccounts] InitializeComponents: creazione controlli.")
        dgvBank = New DataGridView() With {
            .Location = New Point(10, 10),
            .Size = New Size(560, 250),
            .AllowUserToAddRows = False,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        }
        Me.Controls.Add(dgvBank)

        Dim lblBank As New Label() With {
            .Text = "Nome conto bancario:",
            .Location = New Point(10, 270),
            .AutoSize = True
        }
        Me.Controls.Add(lblBank)

        txtBankName = New TextBox() With {
            .Location = New Point(160, 266),
            .Size = New Size(200, 24)
        }
        Me.Controls.Add(txtBankName)

        Dim lblInit As New Label() With {
            .Text = "Saldo iniziale:",
            .Location = New Point(370, 270),
            .AutoSize = True
        }
        Me.Controls.Add(lblInit)

        txtInitialBalance = New TextBox() With {
            .Location = New Point(450, 266),
            .Size = New Size(100, 24)
        }
        Me.Controls.Add(txtInitialBalance)

        btnAdd = New Button() With {
            .Text = "Aggiungi",
            .Location = New Point(10, 305),
            .Size = New Size(100, 30)
        }
        AddHandler btnAdd.Click, AddressOf BtnAdd_Click
        Me.Controls.Add(btnAdd)

        btnSave = New Button() With {
            .Text = "Salva modifiche",
            .Location = New Point(120, 305),
            .Size = New Size(120, 30)
        }
        AddHandler btnSave.Click, AddressOf BtnSave_Click
        Me.Controls.Add(btnSave)

        btnDelete = New Button() With {
            .Text = "Elimina selezionato",
            .Location = New Point(250, 305),
            .Size = New Size(140, 30)
        }
        AddHandler btnDelete.Click, AddressOf BtnDelete_Click
        Me.Controls.Add(btnDelete)

        btnClose = New Button() With {
            .Text = "Chiudi",
            .Location = New Point(400, 305),
            .Size = New Size(70, 30)
        }
        AddHandler btnClose.Click, AddressOf BtnClose_Click
        Me.Controls.Add(btnClose)

        Console.WriteLine("[FormAccounts] InitializeComponents: caricamento conti bancari.")
        LoadBankAccounts()
    End Sub

    Private Function GetDatabasePath() As String
        Dim databaseFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
        Dim databasePath As String = Path.Combine(databaseFolder, "amministrazione.db")
        Console.WriteLine("[FormAccounts] GetDatabasePath: " & databasePath)
        Return databasePath
    End Function

    Private Sub LoadBankAccounts()
        Console.WriteLine("[FormAccounts] LoadBankAccounts: inizio.")
        Try
            Dim db As String = GetDatabasePath()
            Dim connString As String = "Data Source=" & db
            Dim dt As New DataTable()

            Using conn As New SqliteConnection(connString)
                conn.Open()
                Console.WriteLine("[FormAccounts] LoadBankAccounts: connessione aperta.")
                Using cmd As New SqliteCommand("SELECT Name, COALESCE(InitialBalance,0) AS InitialBalance FROM BankAccounts ORDER BY Name ASC", conn)
                    Using reader As SqliteDataReader = cmd.ExecuteReader()
                        dt.Load(reader)
                    End Using
                End Using
            End Using

            dgvBank.DataSource = dt

            Dim rowsCount As Integer = If(dt Is Nothing, 0, dt.Rows.Count)
            Console.WriteLine($"[FormAccounts] LoadBankAccounts: righe caricate = {rowsCount}")

            ' render column headers friendly
            If dgvBank.Columns.Contains("Name") Then
                dgvBank.Columns("Name").HeaderText = "Conto"
                Console.WriteLine("[FormAccounts] LoadBankAccounts: colonna 'Name' trovata.")
            End If
            If dgvBank.Columns.Contains("InitialBalance") Then
                dgvBank.Columns("InitialBalance").HeaderText = "Saldo iniziale"
                Console.WriteLine("[FormAccounts] LoadBankAccounts: colonna 'InitialBalance' trovata.")
            End If
        Catch ex As Exception
            Console.WriteLine("[FormAccounts] Errore LoadBankAccounts: " & ex.ToString())
            MessageBox.Show("Errore caricamento conti bancari: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub BtnAdd_Click(sender As Object, e As EventArgs)
        Console.WriteLine("[FormAccounts] BtnAdd_Click: inizio.")
        Dim bankName As String = txtBankName.Text.Trim()
        Dim initialText As String = txtInitialBalance.Text.Trim().Replace(".", ",")
        Dim initialValue As Double = 0

        Console.WriteLine($"[FormAccounts] BtnAdd_Click: input Name='{bankName}', Initial='{initialText}'")

        If String.IsNullOrWhiteSpace(bankName) Then
            Console.WriteLine("[FormAccounts] BtnAdd_Click: nome conto vuoto.")
            MessageBox.Show("Inserisci il nome del conto bancario.", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Double.TryParse(initialText, NumberStyles.Any, CultureInfo.GetCultureInfo("it-IT"), initialValue)
        Console.WriteLine($"[FormAccounts] BtnAdd_Click: parsed initialValue = {initialValue}")

        Dim db As String = GetDatabasePath()
        Dim connString As String = "Data Source=" & db

        Try
            Using conn As New SqliteConnection(connString)
                conn.Open()
                Console.WriteLine("[FormAccounts] BtnAdd_Click: connessione aperta.")

                Using cmd As New SqliteCommand("INSERT OR IGNORE INTO BankAccounts (Name, InitialBalance) VALUES (@name, @init)", conn)
                    cmd.Parameters.AddWithValue("@name", bankName)
                    cmd.Parameters.AddWithValue("@init", initialValue)
                    Dim inserted = cmd.ExecuteNonQuery()
                    Console.WriteLine($"[FormAccounts] BtnAdd_Click: INSERT OR IGNORE eseguito, affected={inserted}")
                End Using

                Using cmdU As New SqliteCommand("UPDATE BankAccounts SET InitialBalance = @init WHERE Name = @name", conn)
                    cmdU.Parameters.AddWithValue("@init", initialValue)
                    cmdU.Parameters.AddWithValue("@name", bankName)
                    Dim updated = cmdU.ExecuteNonQuery()
                    Console.WriteLine($"[FormAccounts] BtnAdd_Click: UPDATE eseguito, affected={updated}")
                End Using
            End Using

            txtBankName.Text = ""
            txtInitialBalance.Text = ""
            Console.WriteLine("[FormAccounts] BtnAdd_Click: reset campi e ricarico conti.")
            LoadBankAccounts()
        Catch ex As Exception
            Console.WriteLine("[FormAccounts] Errore BtnAdd_Click: " & ex.ToString())
            MessageBox.Show("Errore aggiunta conto bancario: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs)
        Console.WriteLine("[FormAccounts] BtnSave_Click: inizio salvataggio griglia.")
        Try
            Dim db As String = GetDatabasePath()
            Dim connString As String = "Data Source=" & db

            Using conn As New SqliteConnection(connString)
                conn.Open()
                Console.WriteLine("[FormAccounts] BtnSave_Click: connessione aperta.")

                For Each row As DataGridViewRow In dgvBank.Rows
                    If row.IsNewRow Then Continue For
                    Dim bankName As String = Convert.ToString(row.Cells("Name").Value).Trim()
                    Dim balObj = row.Cells("InitialBalance").Value
                    Dim balText As String = If(balObj IsNot Nothing, balObj.ToString().Replace(".", ","), "0")
                    Dim bal As Double = 0
                    Double.TryParse(balText, NumberStyles.Any, CultureInfo.GetCultureInfo("it-IT"), bal)

                    Console.WriteLine($"[FormAccounts] BtnSave_Click: salvataggio conto='{bankName}', saldo={bal}")

                    Using cmd As New SqliteCommand("UPDATE BankAccounts SET InitialBalance = @init WHERE Name = @name", conn)
                        cmd.Parameters.AddWithValue("@init", bal)
                        cmd.Parameters.AddWithValue("@name", bankName)
                        Dim affected As Integer = cmd.ExecuteNonQuery()
                        Console.WriteLine($"[FormAccounts] BtnSave_Click: UPDATE affected={affected}")
                        If affected = 0 Then
                            Using cmdIns As New SqliteCommand("INSERT OR IGNORE INTO BankAccounts (Name, InitialBalance) VALUES (@name, @init)", conn)
                                cmdIns.Parameters.AddWithValue("@name", bankName)
                                cmdIns.Parameters.AddWithValue("@init", bal)
                                cmdIns.ExecuteNonQuery()
                                Console.WriteLine("[FormAccounts] BtnSave_Click: INSERT eseguito per conto mancante.")
                            End Using
                        End If
                    End Using
                Next
            End Using

            MessageBox.Show("Modifiche salvate.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Console.WriteLine("[FormAccounts] BtnSave_Click: salvataggio completato, ricarico conti.")
            LoadBankAccounts()
        Catch ex As Exception
            Console.WriteLine("[FormAccounts] Errore BtnSave_Click: " & ex.ToString())
            MessageBox.Show("Errore salvataggio: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub BtnDelete_Click(sender As Object, e As EventArgs)
        Console.WriteLine("[FormAccounts] BtnDelete_Click: inizio.")
        If dgvBank.SelectedRows.Count = 0 Then
            Console.WriteLine("[FormAccounts] BtnDelete_Click: nessuna riga selezionata.")
            MessageBox.Show("Seleziona il conto da eliminare.", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim bankName As String = Convert.ToString(dgvBank.SelectedRows(0).Cells("Name").Value)
        Console.WriteLine("[FormAccounts] BtnDelete_Click: conto selezionato = " & bankName)
        If String.IsNullOrWhiteSpace(bankName) Then
            Console.WriteLine("[FormAccounts] BtnDelete_Click: nome conto non valido.")
            MessageBox.Show("Nome conto non valido.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Dim result = MessageBox.Show($"Eliminare il conto bancario '{bankName}'? (Impossibile se esistono movimenti collegati)", "Conferma", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        If result <> DialogResult.Yes Then
            Console.WriteLine("[FormAccounts] BtnDelete_Click: cancellazione annullata dall'utente.")
            Return
        End If

        Dim db As String = GetDatabasePath()
        Dim connString As String = "Data Source=" & db

        Try
            Using conn As New SqliteConnection(connString)
                conn.Open()
                Console.WriteLine("[FormAccounts] BtnDelete_Click: connessione aperta.")

                Using cmdCheck As New SqliteCommand("SELECT COUNT(1) FROM Movimenti WHERE BankAccount = @bank", conn)
                    cmdCheck.Parameters.AddWithValue("@bank", bankName)
                    Dim cntObj = cmdCheck.ExecuteScalar()
                    Dim cnt As Integer = 0
                    If cntObj IsNot Nothing Then Integer.TryParse(cntObj.ToString(), cnt)
                    Console.WriteLine($"[FormAccounts] BtnDelete_Click: movimenti collegati = {cnt}")
                    If cnt > 0 Then
                        MessageBox.Show($"Impossibile eliminare: esistono {cnt} movimenti collegati a questo conto bancario.", "Operazione bloccata", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    End If
                End Using

                Using cmd As New SqliteCommand("DELETE FROM BankAccounts WHERE Name = @bank", conn)
                    cmd.Parameters.AddWithValue("@bank", bankName)
                    Dim deleted = cmd.ExecuteNonQuery()
                    Console.WriteLine($"[FormAccounts] BtnDelete_Click: DELETE affected={deleted}")
                End Using
            End Using

            LoadBankAccounts()
            Console.WriteLine("[FormAccounts] BtnDelete_Click: cancellazione completata e ricaricamento.")
        Catch ex As Exception
            Console.WriteLine("[FormAccounts] Errore BtnDelete_Click: " & ex.ToString())
            MessageBox.Show("Errore eliminazione conto bancario: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub BtnClose_Click(sender As Object, e As EventArgs)
        Console.WriteLine("[FormAccounts] BtnClose_Click: chiusura form e refresh Form1 se presente.")
        Try
            Dim f As Form1 = TryCast(Application.OpenForms.OfType(Of Form1)().FirstOrDefault(), Form1)
            If f IsNot Nothing Then
                Console.WriteLine("[FormAccounts] BtnClose_Click: chiamata RefreshAccounts su Form1.")
                f.RefreshAccounts()
            Else
                Console.WriteLine("[FormAccounts] BtnClose_Click: Form1 non trovato nella collection OpenForms.")
            End If
        Catch ex As Exception
            Console.WriteLine("[FormAccounts] BtnClose_Click: eccezione durante refresh: " & ex.ToString())
            ' ignore
        End Try

        Me.Close()
    End Sub
End Class