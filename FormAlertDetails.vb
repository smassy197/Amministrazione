Imports System.Windows.Forms
Imports System.Drawing

Public Class FormAlertDetails
    Inherits Form

    Private lblTitle As Label
    Private lvDetails As ListView
    Private btnClose As Button

    Public Sub New(info As FormMonitor.AlertInfo)
        Me.Text = MonthName(info.MonthIndex + 1)
        Me.Size = New Size(600, 400)
        InitializeComponent()
        ' Try to set application icon if available and disable maximize
        Try
            Dim candidates As String() = {
                IO.Path.Combine(Application.StartupPath, "C:\Users\smass\source\repos\Amministrazione\amministrazione_new_logo.ico"),
                IO.Path.Combine(IO.Path.GetDirectoryName(Application.ExecutablePath), "amministrazione_new_logo.ico"),
                IO.Path.Combine(IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "amministrazione_new_logo.ico")
            }
            For Each icoPath In candidates
                If Not String.IsNullOrEmpty(icoPath) AndAlso IO.File.Exists(icoPath) Then
                    Me.Icon = New Icon(icoPath)
                    Exit For
                End If
            Next
        Catch ex As Exception
            ' ignore icon load errors
        End Try
        Me.MaximizeBox = False
        Populate(info)
    End Sub

    Private Sub InitializeComponent()
        lblTitle = New Label() With {.Left = 10, .Top = 10, .Width = 560, .AutoSize = True}
        lvDetails = New ListView() With {.Left = 10, .Top = 40, .Width = 560, .Height = 280, .View = View.Details, .FullRowSelect = True}
        lvDetails.Columns.Add("Descrizione", 360)
        lvDetails.Columns.Add("Importo", 160)

        btnClose = New Button() With {.Left = 490, .Top = 330, .Width = 80, .Text = "Chiudi"}
        AddHandler btnClose.Click, Sub(sender As Object, e As EventArgs) Me.Close()
        Me.Controls.Add(lblTitle)
        Me.Controls.Add(lvDetails)
        Me.Controls.Add(btnClose)
    End Sub

    Private Sub Populate(info As FormMonitor.AlertInfo)
        lblTitle.Text = $"Mese: {MonthName(info.MonthIndex + 1)} - {info.CurrentYear}"

        ' Show detailed transactions for selected month (current year only): Descrizione + Importo
        Try
            lvDetails.Items.Clear()
            Dim dbPath = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
            Dim connString = "Data Source=" & dbPath
            Dim monthStr = (info.MonthIndex + 1).ToString("00")

            Dim maxVal As Double = Double.MinValue
            Dim maxIdx As Integer = -1
            Dim idx As Integer = 0
            Dim total As Double = 0

            Using conn As New Microsoft.Data.Sqlite.SqliteConnection(connString)
                conn.Open()
                Using cmd As New Microsoft.Data.Sqlite.SqliteCommand("SELECT Descrizione, Adebito FROM Movimenti WHERE " & info.Field & " = @key AND strftime('%Y', Data) = @year AND strftime('%m', Data) = @mm ORDER BY Adebito DESC", conn)
                    cmd.Parameters.AddWithValue("@key", info.Key)
                    cmd.Parameters.AddWithValue("@year", info.CurrentYear.ToString())
                    cmd.Parameters.AddWithValue("@mm", monthStr)
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim desc = If(IsDBNull(reader("Descrizione")), "", reader("Descrizione").ToString())
                            Dim val As Double = 0
                            If Not IsDBNull(reader("Adebito")) Then Double.TryParse(reader("Adebito").ToString(), val)
                            Dim row As String() = {desc, val.ToString("F2")}
                            Dim lvi As New ListViewItem(row)
                            lvDetails.Items.Add(lvi)
                            total += val
                            If val > maxVal Then
                                maxVal = val
                                maxIdx = idx
                            End If
                            idx += 1
                        End While
                    End Using
                End Using
            End Using

            ' Add totals row
            Dim totRow As String() = {"Totale", total.ToString("F2")}
            Dim lviTot As New ListViewItem(totRow)
            lviTot.Font = New Drawing.Font(lviTot.Font, Drawing.FontStyle.Bold)
            lvDetails.Items.Add(lviTot)

            ' Highlight largest expense row (single transaction)
            If maxIdx >= 0 AndAlso maxIdx < lvDetails.Items.Count Then
                Try
                    Dim li = lvDetails.Items(maxIdx)
                    li.BackColor = Drawing.Color.Yellow
                    li.ForeColor = Drawing.Color.Black
                    li.Font = New Drawing.Font(li.Font, Drawing.FontStyle.Bold)
                    lvDetails.EnsureVisible(maxIdx)
                Catch ex As Exception
                    MessageBox.Show("Errore evidenziazione dettagli: " & ex.Message)
                End Try
            End If
        Catch ex As Exception
            MessageBox.Show("Errore caricamento dettagli: " & ex.Message)
        End Try
    End Sub
End Class
