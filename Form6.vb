Imports System.Data.SQLite
Imports System.IO

Public Class Form6
    ' Database path and connection string
    Private databaseFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
    Private databasePath As String = Path.Combine(databaseFolder, "amministrazione.db")
    Private connectionString As String = "Data Source=" & databasePath & ";Version=3;"
    Private WithEvents contextMenu As New ContextMenuStrip()
    Private WithEvents toolStripMenuItemCopia As New ToolStripMenuItem()
    ' Proprietà per memorizzare le celle selezionate
    Public Shared SelectedCellsData As List(Of Object) ' Per memorizzare i dati copiati




    Private Sub Form6_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' Populate ComboBoxes and load initial data
        PopulateComboBoxFromDatabase(comboBoxDocType, "TipoPagamento")
        PopulateComboBoxFromDatabase(comboBoxUser, "UtenteID")
        LoadData()
        UpdateCalendarBoldedDates()

        ' Aggiungere il codice di inizializzazione dopo la chiamata a InitializeComponent().
        contextMenu = New ContextMenuStrip()
        toolStripMenuItemCopia = New ToolStripMenuItem("Copia")

        ' Aggiungi l'elemento Copia al menu contestuale
        contextMenu.Items.Add(toolStripMenuItemCopia)

        ' Associa il ContextMenuStrip al DataGridView
        dataGridViewResults.ContextMenuStrip = contextMenu
    End Sub

    Private Sub LoadData(Optional query As String = "")
        Try
            Using connection As New SQLiteConnection(connectionString)
                connection.Open()
                Dim finalQuery As String = "SELECT * FROM Documenti"
                If Not String.IsNullOrEmpty(query) Then
                    finalQuery &= " " & query
                End If
                Dim command As New SQLiteCommand(finalQuery, connection)

                ' Apply date filter based on selected date in MonthCalendar
                Dim selectedDate As DateTime = MonthCalendarDate.SelectionStart
                Dim startDate As DateTime = selectedDate.Date
                Dim endDate As DateTime = selectedDate.Date.AddDays(1).AddSeconds(-1)
                command.Parameters.AddWithValue("@Data", MonthCalendarDate.SelectionStart.ToString("yyyy-MM-dd"))
                command.Parameters.AddWithValue("@UtenteID", GetSelectedUtenteID())
                command.Parameters.AddWithValue("@TipoPagamento", comboBoxDocType.Text)
                Dim adapter As New SQLiteDataAdapter(command)
                Dim table As New DataTable()
                adapter.Fill(table)
                dataGridViewResults.DataSource = table
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub UpdateCalendarBoldedDates()
        Try
            Dim datesWithData As New List(Of DateTime)()
            Using connection As New SQLiteConnection(connectionString)
                connection.Open()
                Dim query As String = "SELECT DISTINCT Data FROM Documenti"
                Using command As New SQLiteCommand(query, connection)
                    Using reader As SQLiteDataReader = command.ExecuteReader()
                        While reader.Read()
                            If Not reader.IsDBNull(0) Then
                                Dim documentDate As DateTime = reader.GetDateTime(0)
                                datesWithData.Add(documentDate.Date)
                            End If
                        End While
                    End Using
                End Using
            End Using
            MonthCalendarDate.BoldedDates = datesWithData.ToArray()
        Catch ex As Exception
            MessageBox.Show($"Error updating calendar dates: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub MonthCalendarDate_DateChanged(sender As Object, e As DateRangeEventArgs) Handles MonthCalendarDate.DateChanged
        UpdateCalendarBoldedDates()
        LoadData(BuildQuery())
    End Sub


    Private Sub comboBoxDocType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles comboBoxDocType.SelectedIndexChanged
        LoadData(BuildQuery())
    End Sub

    Private Sub comboBoxUser_SelectedIndexChanged(sender As Object, e As EventArgs) Handles comboBoxUser.SelectedIndexChanged
        LoadData(BuildQuery())
    End Sub


    Private Sub PopulateComboBoxFromDatabase(comboBox As ComboBox, columnName As String)
        Try
            Using connection As New SQLiteConnection(connectionString)
                connection.Open()
                Dim query As String = $"SELECT DISTINCT {columnName} FROM Documenti ORDER BY {columnName} ASC"
                Using command As New SQLiteCommand(query, connection)
                    Using reader As SQLiteDataReader = command.ExecuteReader()
                        comboBox.Items.Clear()
                        While reader.Read()
                            If Not reader.IsDBNull(0) Then
                                comboBox.Items.Add(reader.GetValue(0))
                            End If
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show($"Error loading data from column '{columnName}': {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function GetSelectedUtenteID() As Integer
        ' Restituisce l'ID dell'utente selezionato
        If comboBoxDocType.SelectedValue = 1 Then
            Return 1
        ElseIf comboBoxDocType.SelectedValue = 2 Then
            Return 2
        Else
            Return 0 ' Nessun utente selezionato
        End If
    End Function

    Private Function BuildQuery() As String
        Dim docType As String = comboBoxDocType.Text
        Dim user As String = comboBoxUser.Text

        Dim query As String = "WHERE 1=1"
        If Not String.IsNullOrEmpty(docType) Then
            query &= $" AND TipoPagamento LIKE '%{docType}%'"
        End If
        If Not String.IsNullOrEmpty(user) Then
            query &= $" AND UtenteID = {user}"
        End If

        Dim selectedDate As DateTime = MonthCalendarDate.SelectionStart
        query &= $" AND Data >= '{selectedDate.ToString("yyyy-MM-dd")}' AND Data < '{selectedDate.AddDays(1).ToString("yyyy-MM-dd")}'"

        Return query
    End Function

    Private Sub toolStripMenuItemCopia_Click(sender As Object, e As EventArgs) Handles toolStripMenuItemCopia.Click
        SelectedCellsData = New List(Of Object)

        For Each cell As DataGridViewCell In dataGridViewResults.SelectedCells
            SelectedCellsData.Add(cell.Value)
        Next

        MessageBox.Show("Dati copiati con successo.", "Successo", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Me.Close()
    End Sub



End Class