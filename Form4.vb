Imports Microsoft.Data.Sqlite
Imports System.IO
Imports System.Windows.Forms.DataVisualization.Charting

Public Class FormChart
    Private Sub FormChart_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Console.WriteLine("[DEBUG] Avvio FormChart_Load.")

        LoadDataIntoListBox()
        Console.WriteLine("[DEBUG] LoadDataIntoListBox completato.")

        lstConto.SelectionMode = SelectionMode.MultiSimple
        Console.WriteLine("[DEBUG] Selezione multipla abilitata su lstConto.")

        UpdateLabels()
        Console.WriteLine("[DEBUG] UpdateLabels completato.")

        UpdateMonthlySummary()
        Console.WriteLine("[DEBUG] UpdateMonthlySummary completato.")

        UpdateChartForSelectedYear()
        Console.WriteLine("[DEBUG] UpdateChartForSelectedYear completato.")

        UpdateYearlyAverageGrid()
        Console.WriteLine("[DEBUG] UpdateYearlyAverageGrid completato.")

        PopulateYearComboBox()
        Console.WriteLine("[DEBUG] PopulateYearComboBox completato.")

        Console.WriteLine("[DEBUG] FormChart_Load completato.")
    End Sub


    Private Function GetTotalSpese(startDate As Date, endDate As Date) As Double
        Console.WriteLine("[DEBUG] Avvio GetTotalSpese. Intervallo: " & startDate.ToShortDateString() & " - " & endDate.ToShortDateString())

        Dim totalSpese As Double = 0
        Dim databaseFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
        Dim databasePath As String = Path.Combine(databaseFolder, "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath

        Try
            Using conn As New Microsoft.Data.Sqlite.SqliteConnection(connString)
                conn.Open()
                Console.WriteLine("[DEBUG] Connessione al database aperta.")

                Using cmd As New Microsoft.Data.Sqlite.SqliteCommand("SELECT SUM(Adebito) FROM Movimenti WHERE Data BETWEEN @startDate AND @endDate", conn)
                    cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                    cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))

                    Dim result = cmd.ExecuteScalar()
                    If result IsNot Nothing AndAlso Double.TryParse(result.ToString(), totalSpese) Then
                        Console.WriteLine("[DEBUG] Somma spese calcolata correttamente: " & totalSpese)
                    Else
                        Console.WriteLine("[DEBUG] Nessun risultato valido per la somma spese.")
                    End If
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore in GetTotalSpese: " & ex.ToString())
        End Try

        Return totalSpese
    End Function


    Private Sub UpdateTotalSpeseLabel(startDate As Date, endDate As Date)
        Console.WriteLine("[DEBUG] Avvio UpdateTotalSpeseLabel. Intervallo: " & startDate.ToShortDateString() & " - " & endDate.ToShortDateString())

        Dim totalSpese As Double = GetTotalSpese(startDate, endDate)
        lblTotalSpese.Text = $"Totale spese: {totalSpese.ToString("C2")}"

        Console.WriteLine("[DEBUG] Etichetta aggiornata con totale spese: " & totalSpese.ToString("C2"))
    End Sub




    Private Sub LoadDataIntoListBox()
        Console.WriteLine("[DEBUG] Avvio LoadDataIntoListBox.")

        Dim databaseFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
        Dim databasePath As String = Path.Combine(databaseFolder, "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath

        Try
            Using connConto As New Microsoft.Data.Sqlite.SqliteConnection(connString)
                connConto.Open()
                Console.WriteLine("[DEBUG] Connessione al database aperta.")

                Using cmd As New Microsoft.Data.Sqlite.SqliteCommand("SELECT Conto FROM Conti ORDER BY Conto ASC", connConto)
                    Using reader As Microsoft.Data.Sqlite.SqliteDataReader = cmd.ExecuteReader()
                        Dim count As Integer = 0
                        While reader.Read()
                            lstConto.Items.Add(reader("Conto").ToString())
                            count += 1
                        End While
                        Console.WriteLine("[DEBUG] Conti caricati nella ListBox: " & count)
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore in LoadDataIntoListBox: " & ex.ToString())
        End Try
    End Sub



    Private Sub btnCalculate_Click(sender As Object, e As EventArgs) Handles btnCalculate.Click
        Console.WriteLine("[DEBUG] Avvio btnCalculate_Click.")

        ' Pulisci il grafico Chart1 prima di aggiungere nuovi dati
        Chart1.Series.Clear()
        Console.WriteLine("[DEBUG] Chart1.Series.Clear eseguito.")

        ' Ottieni l'intervallo di date selezionato dal MonthCalendar
        Dim selectedRange As SelectionRange = MonthCalendar1.SelectionRange
        Dim startDate As Date = selectedRange.Start
        Dim endDate As Date = selectedRange.End
        Console.WriteLine("[DEBUG] Intervallo selezionato: " & startDate.ToShortDateString() & " - " & endDate.ToShortDateString())

        ' Imposta l'intervallo di date selezionato nel controllo MonthCalendar
        MonthCalendar1.SetSelectionRange(startDate, endDate)

        ' Popola la ListBox con i conti per l'intervallo di date selezionato
        PopulateContiListBoxByDateRange(startDate, endDate)
        Console.WriteLine("[DEBUG] PopulateContiListBoxByDateRange completato.")

        If lstConto.SelectedItems.Count > 0 Then
            Console.WriteLine("[DEBUG] Conti selezionati: " & lstConto.SelectedItems.Count)
            Dim totalForAllConti As Double = 0

            ' Calcola il totale di tutti i conti selezionati
            For Each selectedItem As Object In lstConto.SelectedItems
                Dim contoType As String = selectedItem.ToString()
                Console.WriteLine("[DEBUG] Calcolo totale per conto: " & contoType)

                Select Case contoType
                    Case "Adebito"
                        totalForAllConti += GetTotalAdebitoByDateRange(startDate, endDate)
                    Case "Acredito"
                        totalForAllConti += GetTotalAcreditoByDateRange(startDate, endDate)
                    Case "Saldo"
                        Dim addebitoSum As Double = GetTotalAdebitoByDateRange(startDate, endDate)
                        Dim acreditoSum As Double = GetTotalAcreditoByDateRange(startDate, endDate)
                        totalForAllConti += (acreditoSum - addebitoSum)
                    Case Else
                        totalForAllConti += GetTotalByDateRangeAndConto(startDate, endDate, contoType)
                End Select
            Next
            Console.WriteLine("[DEBUG] Totale complessivo conti selezionati: " & totalForAllConti.ToString("F2"))

            ' Aggiungi i dati al grafico Chart1
            For Each selectedItem As Object In lstConto.SelectedItems
                Dim contoType As String = selectedItem.ToString()
                Dim totalForConto As Double = 0

                Select Case contoType
                    Case "Adebito"
                        totalForConto = GetTotalAdebitoByDateRange(startDate, endDate)
                    Case "Acredito"
                        totalForConto = GetTotalAcreditoByDateRange(startDate, endDate)
                    Case "Saldo"
                        Dim addebitoSum As Double = GetTotalAdebitoByDateRange(startDate, endDate)
                        Dim acreditoSum As Double = GetTotalAcreditoByDateRange(startDate, endDate)
                        totalForConto = (acreditoSum - addebitoSum)
                    Case Else
                        totalForConto = GetTotalByDateRangeAndConto(startDate, endDate, contoType)
                End Select

                Dim series As Series = Chart1.Series.Add(contoType)
                series.Points.Add(totalForConto)
                series.Label = contoType & ": " & totalForConto.ToString("F2")
                Console.WriteLine("[DEBUG] Serie aggiunta: " & contoType & " → " & totalForConto.ToString("F2"))
            Next

            Chart1.ChartAreas(0).AxisX.Title = "Conti"
            Console.WriteLine("[DEBUG] Titolo asse X impostato.")
        Else
            Console.WriteLine("[DEBUG] Nessun conto selezionato.")
            MessageBox.Show("Seleziona almeno un tipo di conto dalla lista.")
        End If

        ' Aggiorna il totale spese nell'etichetta
        UpdateTotalSpeseLabel(startDate, endDate)
        Console.WriteLine("[DEBUG] UpdateTotalSpeseLabel completato.")
    End Sub

    Private Sub PopulateContiListBoxByDateRange(startDate As Date, endDate As Date)
        Console.WriteLine("[DEBUG] Avvio PopulateContiListBoxByDateRange. Intervallo: " & startDate.ToShortDateString() & " - " & endDate.ToShortDateString())

        lstConto2.Items.Clear()
        Console.WriteLine("[DEBUG] ListBox lstConto2 svuotata.")

        Dim databasePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath

        Dim totalAdebito As Double = 0
        Dim totalAcredito As Double = 0
        Dim saldoComplessivo As Double = 0
        Dim totaleComplessivo As Double = 0

        Dim series As New Series("Conti")
        series.ChartType = SeriesChartType.Pie
        Console.WriteLine("[DEBUG] Serie grafico 'Conti' creata come torta.")

        Try
            Using conn As New Microsoft.Data.Sqlite.SqliteConnection(connString)
                conn.Open()
                Console.WriteLine("[DEBUG] Connessione al database aperta.")

                Using cmd As New Microsoft.Data.Sqlite.SqliteCommand("SELECT SUM(Adebito) FROM Movimenti WHERE Data BETWEEN @startDate AND @endDate", conn)
                    cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                    cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))
                    Dim result = cmd.ExecuteScalar()
                    If result IsNot Nothing AndAlso Double.TryParse(result.ToString(), totalAdebito) Then
                        Console.WriteLine("[DEBUG] Totale Adebito: " & totalAdebito.ToString("F2"))

                        Using cmd2 As New Microsoft.Data.Sqlite.SqliteCommand("SELECT SUM(Acredito) FROM Movimenti WHERE Data BETWEEN @startDate AND @endDate", conn)
                            cmd2.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                            cmd2.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))
                            Dim result2 = cmd2.ExecuteScalar()
                            If result2 IsNot Nothing AndAlso Double.TryParse(result2.ToString(), totalAcredito) Then
                                Console.WriteLine("[DEBUG] Totale Acredito: " & totalAcredito.ToString("F2"))

                                saldoComplessivo = totalAcredito - totalAdebito
                                totaleComplessivo = totalAcredito - (-totalAdebito)
                                Console.WriteLine("[DEBUG] Saldo complessivo: " & saldoComplessivo.ToString("F2") & ", Totale complessivo: " & totaleComplessivo.ToString("F2"))

                                Using cmd3 As New Microsoft.Data.Sqlite.SqliteCommand("SELECT COUNT(DISTINCT Conto) FROM Movimenti WHERE Data BETWEEN @startDate AND @endDate", conn)
                                    cmd3.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                                    cmd3.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))
                                    Dim contiInteressati As Integer = Convert.ToInt32(cmd3.ExecuteScalar())
                                    Console.WriteLine("[DEBUG] Conti interessati: " & contiInteressati)

                                    lstConto2.Items.Add($"Numero totale di conti interessati: {contiInteressati}, Saldo complessivo = {saldoComplessivo.ToString("F2")}, Totale complessivo {totaleComplessivo.ToString("F2")}")

                                    Using cmd4 As New Microsoft.Data.Sqlite.SqliteCommand("SELECT DISTINCT Conto, SUM(Adebito) AS TotalAdebito, SUM(Acredito) AS TotalAcredito FROM Movimenti WHERE Data BETWEEN @startDate AND @endDate GROUP BY Conto", conn)
                                        cmd4.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                                        cmd4.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))

                                        Using reader As Microsoft.Data.Sqlite.SqliteDataReader = cmd4.ExecuteReader()
                                            While reader.Read()
                                                Dim conto As String = reader.GetString(0)
                                                Dim saldo As Double = reader.GetDouble(2) - reader.GetDouble(1)
                                                Dim percentage As Double = 0

                                                If saldo <> 0 Then
                                                    percentage = Math.Abs(saldo / totaleComplessivo) * 100
                                                    lstConto2.Items.Add($"{conto}: Saldo = {saldo.ToString("F2")}, Percentuale = {percentage.ToString("F0")} %")
                                                    series.Points.AddXY(conto, percentage)
                                                    series.Points(series.Points.Count - 1).LegendText = conto
                                                    Console.WriteLine($"[DEBUG] Conto: {conto}, Saldo: {saldo:F2}, Percentuale: {percentage:F0}%")
                                                End If
                                            End While
                                        End Using
                                    End Using
                                End Using
                            End If
                        End Using
                    End If
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore in PopulateContiListBoxByDateRange: " & ex.ToString())
        End Try

        For Each point As DataPoint In series.Points
            If point.YValues(0) = 0 Then
                point.IsVisibleInLegend = False
            Else
                point.Label = "#PERCENT{P0}"
            End If
        Next

        Console.WriteLine("[DEBUG] Serie grafico completata con " & series.Points.Count & " punti.")
    End Sub


    Private Function GetTotalByDateRangeAndConto(startDate As Date, endDate As Date, contoType As String) As Double
        Console.WriteLine("[DEBUG] Avvio GetTotalByDateRangeAndConto. Conto: " & contoType & ", Intervallo: " & startDate.ToShortDateString() & " - " & endDate.ToShortDateString())

        Dim total As Double = 0
        Dim databaseFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
        Dim databasePath As String = Path.Combine(databaseFolder, "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath

        Try
            Using conn As New Microsoft.Data.Sqlite.SqliteConnection(connString)
                conn.Open()
                Console.WriteLine("[DEBUG] Connessione al database aperta.")

                Dim query As String
                If contoType = "Stipendi" Then
                    query = "SELECT SUM(Acredito) FROM Movimenti WHERE Conto = @contoType AND Data BETWEEN @startDate AND @endDate"
                Else
                    query = "SELECT SUM(Adebito - Acredito) FROM Movimenti WHERE Conto = @contoType AND Data BETWEEN @startDate AND @endDate"
                End If

                Using cmd As New Microsoft.Data.Sqlite.SqliteCommand(query, conn)
                    cmd.Parameters.AddWithValue("@contoType", contoType)
                    cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                    cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))

                    Dim result = cmd.ExecuteScalar()
                    If result IsNot Nothing AndAlso Double.TryParse(result.ToString(), total) Then
                        Console.WriteLine("[DEBUG] Totale calcolato per '" & contoType & "': " & total.ToString("F2"))
                    Else
                        Console.WriteLine("[DEBUG] Nessun risultato valido per '" & contoType & "'.")
                    End If
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore in GetTotalByDateRangeAndConto: " & ex.ToString())
        End Try

        Return total
    End Function



    Private Function GetTotalAdebitoByDateRange(startDate As Date, endDate As Date) As Double
        Console.WriteLine("[DEBUG] Avvio GetTotalAdebitoByDateRange. Intervallo: " & startDate.ToShortDateString() & " - " & endDate.ToShortDateString())

        Dim total As Double = 0
        Dim databasePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath

        Try
            Using conn As New Microsoft.Data.Sqlite.SqliteConnection(connString)
                conn.Open()
                Console.WriteLine("[DEBUG] Connessione al database aperta.")

                Using cmd As New Microsoft.Data.Sqlite.SqliteCommand("SELECT SUM(Adebito) FROM Movimenti WHERE Data BETWEEN @startDate AND @endDate", conn)
                    cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                    cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))

                    Dim result = cmd.ExecuteScalar()
                    If result IsNot Nothing AndAlso Double.TryParse(result.ToString(), total) Then
                        Console.WriteLine("[DEBUG] Totale Adebito calcolato: " & total.ToString("F2"))
                    Else
                        Console.WriteLine("[DEBUG] Nessun risultato valido per Adebito.")
                    End If
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore in GetTotalAdebitoByDateRange: " & ex.ToString())
        End Try

        Return total
    End Function


    Private Function GetTotalAcreditoByDateRange(startDate As Date, endDate As Date) As Double
        Console.WriteLine("[DEBUG] Avvio GetTotalAcreditoByDateRange. Intervallo: " & startDate.ToShortDateString() & " - " & endDate.ToShortDateString())

        Dim total As Double = 0
        Dim databasePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath

        Try
            Using conn As New Microsoft.Data.Sqlite.SqliteConnection(connString)
                conn.Open()
                Console.WriteLine("[DEBUG] Connessione al database aperta.")

                Using cmd As New Microsoft.Data.Sqlite.SqliteCommand("SELECT SUM(Acredito) FROM Movimenti WHERE Data BETWEEN @startDate AND @endDate", conn)
                    cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                    cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))

                    Dim result = cmd.ExecuteScalar()
                    If result IsNot Nothing AndAlso Double.TryParse(result.ToString(), total) Then
                        Console.WriteLine("[DEBUG] Totale Acredito calcolato: " & total.ToString("F2"))
                    Else
                        Console.WriteLine("[DEBUG] Nessun risultato valido per Acredito.")
                    End If
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore in GetTotalAcreditoByDateRange: " & ex.ToString())
        End Try

        Return total
    End Function






    Private Sub UpdateLabels()
        Console.WriteLine("[DEBUG] Avvio UpdateLabels.")

        Dim saldo As Double = GetTotalSaldoForDateRange(New Date(Date.Today.Year, 1, 1), Date.Today)
        lblSaldo.Text = $"Saldo dal primo gennaio: {saldo:F2}"
        Console.WriteLine("[DEBUG] Saldo aggiornato: " & saldo.ToString("F2"))

        Dim patrimonio As Double = GetTotalPatrimonio()
        lblPatrimonio.Text = $"Patrimonio totale: {patrimonio:F2}"
        Console.WriteLine("[DEBUG] Patrimonio aggiornato: " & patrimonio.ToString("F2"))
    End Sub


    Private Function GetTotalSaldoForDateRange(startDate As Date, endDate As Date) As Double
        Console.WriteLine("[DEBUG] Avvio GetTotalSaldoForDateRange. Intervallo: " & startDate.ToShortDateString() & " - " & endDate.ToShortDateString())

        Dim total As Double = 0
        Dim databaseFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
        Dim databasePath As String = Path.Combine(databaseFolder, "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath

        Try
            Using conn As New Microsoft.Data.Sqlite.SqliteConnection(connString)
                conn.Open()
                Console.WriteLine("[DEBUG] Connessione al database aperta.")

                Using cmd As New Microsoft.Data.Sqlite.SqliteCommand("SELECT SUM(Acredito) - SUM(Adebito) FROM Movimenti WHERE Data >= @startDate AND Data <= @endDate", conn)
                    cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                    cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))

                    Dim result = cmd.ExecuteScalar()
                    If result IsNot Nothing AndAlso Double.TryParse(result.ToString(), total) Then
                        Console.WriteLine("[DEBUG] Saldo calcolato: " & total.ToString("F2"))
                    Else
                        Console.WriteLine("[DEBUG] Nessun risultato valido per saldo.")
                    End If
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore in GetTotalSaldoForDateRange: " & ex.ToString())
        End Try

        Return total
    End Function

    Private Function GetTotalPatrimonio() As Double
        Console.WriteLine("[DEBUG] Avvio GetTotalPatrimonio.")

        Dim total As Double = 0
        Dim importoIniziale As Double = 0 ' Modificabile secondo necessità
        Dim databaseFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
        Dim databasePath As String = Path.Combine(databaseFolder, "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath

        Try
            Using conn As New Microsoft.Data.Sqlite.SqliteConnection(connString)
                conn.Open()
                Console.WriteLine("[DEBUG] Connessione al database aperta.")

                Using cmd As New Microsoft.Data.Sqlite.SqliteCommand("SELECT SUM(Acredito) - SUM(Adebito) - @importoIniziale FROM Movimenti", conn)
                    cmd.Parameters.AddWithValue("@importoIniziale", importoIniziale)

                    Dim result = cmd.ExecuteScalar()
                    If result IsNot Nothing AndAlso Double.TryParse(result.ToString(), total) Then
                        Console.WriteLine("[DEBUG] Patrimonio calcolato: " & total.ToString("F2"))
                    Else
                        Console.WriteLine("[DEBUG] Nessun risultato valido per patrimonio.")
                    End If
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore in GetTotalPatrimonio: " & ex.ToString())
        End Try

        Return total
    End Function


    'riepilogo mensile ----------------------------------------------------------------------------------------------
    Private Function GetMonthlySummary() As DataTable
        Console.WriteLine("[DEBUG] Avvio GetMonthlySummary.")

        Dim dtSummary As New DataTable
        dtSummary.Columns.Add("Mese")
        dtSummary.Columns.Add("Anno")
        dtSummary.Columns.Add("Spese Mensili", GetType(Double))
        dtSummary.Columns.Add("Entrate Mensili", GetType(Double))

        Dim databaseFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
        Dim databasePath As String = Path.Combine(databaseFolder, "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath

        Try
            Using conn As New Microsoft.Data.Sqlite.SqliteConnection(connString)
                conn.Open()
                Console.WriteLine("[DEBUG] Connessione al database aperta.")

                Using cmd As New Microsoft.Data.Sqlite.SqliteCommand("SELECT DISTINCT strftime('%Y', Data) AS Anno, strftime('%m', Data) AS Mese FROM Movimenti", conn)
                    Using reader As Microsoft.Data.Sqlite.SqliteDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim year As String = reader("Anno").ToString()
                            Dim month As String = reader("Mese").ToString()
                            Console.WriteLine("[DEBUG] Elaborazione mese: " & month & ", anno: " & year)

                            Dim startDate As New Date(Convert.ToInt32(year), Convert.ToInt32(month), 1)
                            Dim endDate As New Date(Convert.ToInt32(year), Convert.ToInt32(month), Date.DaysInMonth(Convert.ToInt32(year), Convert.ToInt32(month)))

                            Dim speseMensili As Double = 0
                            Dim ingressiMensili As Double = 0

                            Using cmd2 As New Microsoft.Data.Sqlite.SqliteCommand("SELECT SUM(Adebito) FROM Movimenti WHERE Data BETWEEN @startDate AND @endDate", conn)
                                cmd2.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                                cmd2.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))
                                Dim resultSpese = cmd2.ExecuteScalar()
                                If resultSpese IsNot DBNull.Value AndAlso resultSpese IsNot Nothing Then
                                    Double.TryParse(resultSpese.ToString(), speseMensili)
                                End If
                            End Using

                            Using cmd3 As New Microsoft.Data.Sqlite.SqliteCommand("SELECT SUM(Acredito) FROM Movimenti WHERE Data BETWEEN @startDate AND @endDate", conn)
                                cmd3.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                                cmd3.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))
                                Dim resultEntrate = cmd3.ExecuteScalar()
                                If resultEntrate IsNot DBNull.Value AndAlso resultEntrate IsNot Nothing Then
                                    Double.TryParse(resultEntrate.ToString(), ingressiMensili)
                                End If
                            End Using

                            dtSummary.Rows.Add(MonthName(Convert.ToInt32(month)) & " " & year, year, speseMensili, ingressiMensili)
                            Console.WriteLine($"[DEBUG] Aggiunto: {MonthName(Convert.ToInt32(month))} {year} → Spese: {speseMensili:F2}, Entrate: {ingressiMensili:F2}")
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore in GetMonthlySummary: " & ex.ToString())
        End Try

        Return dtSummary
    End Function

    Private Sub UpdateMonthlySummary()
        Console.WriteLine("[DEBUG] Avvio UpdateMonthlySummary.")

        Dim monthlySummary As DataTable = GetMonthlySummary()
        Console.WriteLine("[DEBUG] Riepilogo mensile ottenuto. Righe: " & monthlySummary.Rows.Count)

        DataGridView1.DataSource = monthlySummary
        Console.WriteLine("[DEBUG] DataGridView1 aggiornato con i dati mensili.")
    End Sub

    Private Sub DataGridView1_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles DataGridView1.CellFormatting
        If e.ColumnIndex = DataGridView1.Columns("Spese Mensili").Index OrElse e.ColumnIndex = DataGridView1.Columns("Entrate Mensili").Index Then
            If e.Value IsNot Nothing AndAlso IsNumeric(e.Value) Then
                e.Value = Convert.ToDouble(e.Value).ToString("C2")
                Console.WriteLine("[DEBUG] Formattazione valuta applicata alla cella (" & e.RowIndex & "," & e.ColumnIndex & "): " & e.Value)
            End If
        End If
    End Sub



    Private Sub PopulateYearComboBox()
        Console.WriteLine("[DEBUG] Avvio PopulateYearComboBox.")

        Dim monthlySummary As DataTable = GetMonthlySummary()
        Console.WriteLine("[DEBUG] Riepilogo mensile ottenuto. Righe: " & monthlySummary.Rows.Count)

        Dim uniqueYears = monthlySummary.AsEnumerable() _
        .Select(Function(row) DateTime.ParseExact(row("Mese").ToString(), "MMMM yyyy", New Globalization.CultureInfo("it-IT")).Year) _
        .Distinct() _
        .OrderBy(Function(year) year) _
        .ToList()

        cmbAnno.Items.Clear()
        Console.WriteLine("[DEBUG] ComboBox cmbAnno svuotata.")

        For Each year As Integer In uniqueYears
            cmbAnno.Items.Add(year)
            Console.WriteLine("[DEBUG] Anno aggiunto: " & year)
        Next

        If cmbAnno.Items.Count > 0 Then
            cmbAnno.SelectedIndex = 0
            Console.WriteLine("[DEBUG] Selezionato automaticamente anno: " & cmbAnno.SelectedItem.ToString())
        End If
    End Sub

    Private Sub UpdateChartForSelectedYear()
        Console.WriteLine("[DEBUG] Avvio UpdateChartForSelectedYear.")

        If cmbAnno.SelectedItem Is Nothing Then
            Console.WriteLine("[DEBUG] Nessun anno selezionato. Uscita dalla funzione.")
            Exit Sub
        End If

        Dim selectedYear As Integer = Convert.ToInt32(cmbAnno.SelectedItem)
        Console.WriteLine("[DEBUG] Anno selezionato: " & selectedYear)

        Chart3.Series.Clear()
        Console.WriteLine("[DEBUG] Chart3.Series.Clear eseguito.")

        Dim monthlySummary As DataTable = GetMonthlySummary()
        Console.WriteLine("[DEBUG] Riepilogo mensile ottenuto. Righe: " & monthlySummary.Rows.Count)

        Dim filteredSummary = monthlySummary.AsEnumerable() _
        .Where(Function(row) DateTime.ParseExact(row("Mese").ToString(), "MMMM yyyy", New Globalization.CultureInfo("it-IT")).Year = selectedYear) _
        .ToList()
        Console.WriteLine("[DEBUG] Righe filtrate per l'anno " & selectedYear & ": " & filteredSummary.Count)

        Dim patrimonioCumulativo As Double = 0

        Dim seriesPatrimonioCumulativo As Series = Chart3.Series.Add("Patrimonio Cumulativo")
        seriesPatrimonioCumulativo.ChartType = SeriesChartType.Line
        seriesPatrimonioCumulativo.BorderWidth = 3

        Dim seriesPatrimonioTotale As Series = Chart3.Series.Add("Patrimonio Totale")
        seriesPatrimonioTotale.ChartType = SeriesChartType.Line
        seriesPatrimonioTotale.BorderWidth = 3
        seriesPatrimonioTotale.Color = Color.Red

        Dim culture As New Globalization.CultureInfo("it-IT")
        filteredSummary.Sort(Function(row1, row2)
                                 Dim date1 As DateTime = DateTime.ParseExact(row1("Mese").ToString(), "MMMM yyyy", culture)
                                 Dim date2 As DateTime = DateTime.ParseExact(row2("Mese").ToString(), "MMMM yyyy", culture)
                                 Return date1.CompareTo(date2)
                             End Function)

        For Each row In filteredSummary
            Dim speseMensili As Double = Convert.ToDouble(row("Spese Mensili"))
            Dim ingressiMensili As Double = Convert.ToDouble(row("Entrate Mensili"))
            patrimonioCumulativo += ingressiMensili - speseMensili

            Dim dataMese As DateTime = DateTime.ParseExact(row("Mese").ToString(), "MMMM yyyy", culture)
            Dim monthIndex As Integer = dataMese.Month

            seriesPatrimonioCumulativo.Points.AddXY(monthIndex, patrimonioCumulativo)
            seriesPatrimonioTotale.Points.AddXY(monthIndex, patrimonioCumulativo)

            Console.WriteLine($"[DEBUG] Mese {monthIndex}: Spese={speseMensili:F2}, Entrate={ingressiMensili:F2}, Patrimonio={patrimonioCumulativo:F2}")
        Next

        With Chart3.ChartAreas(0)
            .AxisX.Minimum = 1
            .AxisX.Maximum = 12
            .AxisX.Interval = 1
            .AxisX.Title = "Mesi"
            .AxisY.Title = "Patrimonio (€)"
            .AxisY.LabelStyle.Format = "C0"
        End With

        Chart3.Legends.Clear()
        Console.WriteLine("[DEBUG] Grafico aggiornato per l'anno selezionato.")
    End Sub

    Private Sub cmbAnno_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbAnno.SelectedIndexChanged
        Console.WriteLine("[DEBUG] Evento cmbAnno_SelectedIndexChanged.")
        UpdateChartForSelectedYear()
    End Sub



    Private Function GetYearlyAverage() As DataTable
        Console.WriteLine("[DEBUG] Avvio GetYearlyAverage.")

        Dim dtYearlyAverage As New DataTable
        dtYearlyAverage.Columns.Add("Anno")
        dtYearlyAverage.Columns.Add("Media Spese Mensili", GetType(Double))
        dtYearlyAverage.Columns.Add("Media Entrate Mensili", GetType(Double))
        dtYearlyAverage.Columns.Add("Media Spese Giornaliera", GetType(Double))
        dtYearlyAverage.Columns.Add("Media Entrate Giornaliera", GetType(Double))

        Dim databaseFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
        Dim databasePath As String = Path.Combine(databaseFolder, "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath

        Try
            Using conn As New Microsoft.Data.Sqlite.SqliteConnection(connString)
                conn.Open()
                Console.WriteLine("[DEBUG] Connessione al database aperta.")

                Dim query As String = "
                SELECT strftime('%Y', Data) AS Anno, 
                       strftime('%m', Data) AS Mese, 
                       SUM(Adebito) AS SommaSpeseMensili, 
                       SUM(Acredito) AS SommaIngressiMensili 
                FROM Movimenti 
                GROUP BY strftime('%Y', Data), strftime('%m', Data)"

                Using cmd As New Microsoft.Data.Sqlite.SqliteCommand(query, conn)
                    Dim tempData As New Dictionary(Of String, (Double, Double, Integer))

                    Using reader As Microsoft.Data.Sqlite.SqliteDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim anno As String = reader("Anno").ToString()
                            Dim sommaSpese As Double = If(IsDBNull(reader("SommaSpeseMensili")), 0, Convert.ToDouble(reader("SommaSpeseMensili")))
                            Dim sommaIngressi As Double = If(IsDBNull(reader("SommaIngressiMensili")), 0, Convert.ToDouble(reader("SommaIngressiMensili")))

                            If Not tempData.ContainsKey(anno) Then tempData(anno) = (0, 0, 0)
                            Dim current = tempData(anno)
                            tempData(anno) = (current.Item1 + sommaSpese, current.Item2 + sommaIngressi, current.Item3 + 1)
                        End While
                    End Using

                    For Each entry In tempData
                        Dim anno = entry.Key
                        Dim sommaSpese = entry.Value.Item1
                        Dim sommaIngressi = entry.Value.Item2
                        Dim mesi = entry.Value.Item3
                        Dim giorniNellAnno = If(DateTime.IsLeapYear(Convert.ToInt32(anno)), 366, 365)

                        Dim mediaSpeseMensili = If(mesi > 0, sommaSpese / mesi, 0)
                        Dim mediaIngressiMensili = If(mesi > 0, sommaIngressi / mesi, 0)
                        Dim mediaSpeseGiornaliere = sommaSpese / giorniNellAnno
                        Dim mediaIngressiGiornaliere = sommaIngressi / giorniNellAnno

                        dtYearlyAverage.Rows.Add(anno,
                        Math.Round(mediaSpeseMensili, 2),
                        Math.Round(mediaIngressiMensili, 2),
                        Math.Round(mediaSpeseGiornaliere, 2),
                        Math.Round(mediaIngressiGiornaliere, 2))

                        Console.WriteLine($"[DEBUG] Anno {anno}: Mesi={mesi}, Spese={sommaSpese:F2}, Entrate={sommaIngressi:F2}")
                    Next
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore in GetYearlyAverage: " & ex.ToString())
        End Try

        Return dtYearlyAverage
    End Function


    Private Sub UpdateYearlyAverageGrid()
        Console.WriteLine("[DEBUG] Avvio UpdateYearlyAverageGrid.")

        Dim yearlyAverage As DataTable = GetYearlyAverage()
        Console.WriteLine("[DEBUG] Righe ottenute: " & yearlyAverage.Rows.Count)

        DataGridView2.DataSource = yearlyAverage
        Console.WriteLine("[DEBUG] DataGridView2 aggiornato con medie annuali.")
    End Sub




    'fine riepilogo mensile------------------------------------------------------------------------------------------


End Class