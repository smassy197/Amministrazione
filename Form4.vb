Imports System.Data.SQLite
Imports System.IO
Imports System.Windows.Forms.DataVisualization.Charting

Public Class FormChart
    Private Sub FormChart_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadDataIntoListBox()
        ' Abilita la selezione multipla nella ListBox
        lstConto.SelectionMode = SelectionMode.MultiSimple ' o SelectionMode.MultiExtended
        UpdateLabels()
        UpdateMonthlySummary()
        UpdateChartForSelectedYear()
        UpdateYearlyAverageGrid()
        PopulateYearComboBox()
    End Sub

    Private Function GetTotalSpese(startDate As Date, endDate As Date) As Double
        Dim totalSpese As Double = 0
        Dim databaseFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
        Dim databasePath As String = Path.Combine(databaseFolder, "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath & ";Version=3;"

        Using conn As New SQLiteConnection(connString)
            conn.Open()
            Using cmd As New SQLiteCommand("SELECT SUM(Adebito) FROM Movimenti WHERE Data BETWEEN @startDate AND @endDate", conn)
                cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))
                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing AndAlso Double.TryParse(result.ToString(), totalSpese) Then
                    ' La somma è stata calcolata correttamente
                End If
            End Using
        End Using

        Return totalSpese
    End Function

    Private Sub UpdateTotalSpeseLabel(startDate As Date, endDate As Date)
        Dim totalSpese As Double = GetTotalSpese(startDate, endDate)
        lblTotalSpese.Text = $"Totale spese: {totalSpese.ToString("C2")}"
    End Sub



    Private Sub LoadDataIntoListBox()
        Dim databaseFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
        Dim databasePath As String = Path.Combine(databaseFolder, "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath & ";Version=3;"

        ' Carica dati per ListBox Conto
        Using connConto As New SQLiteConnection(connString)
            connConto.Open()
            Using daConto As New SQLiteDataAdapter("SELECT Conto FROM Conti ORDER BY Conto ASC", connConto)
                Dim dtConto As New DataTable
                daConto.Fill(dtConto)
                For Each row As DataRow In dtConto.Rows
                    lstConto.Items.Add(row("Conto").ToString())
                Next
            End Using
        End Using
    End Sub


    Private Sub btnCalculate_Click(sender As Object, e As EventArgs) Handles btnCalculate.Click
        ' Pulisci il grafico Chart1 prima di aggiungere nuovi dati
        Chart1.Series.Clear()

        ' Ottieni l'intervallo di date selezionato dal MonthCalendar
        Dim selectedRange As SelectionRange = MonthCalendar1.SelectionRange
        Dim startDate As Date = selectedRange.Start
        Dim endDate As Date = selectedRange.End

        ' Imposta l'intervallo di date selezionato nel controllo MonthCalendar
        MonthCalendar1.SetSelectionRange(startDate, endDate)

        ' Popola la ListBox con i conti per l'intervallo di date selezionato
        PopulateContiListBoxByDateRange(startDate, endDate)

        If lstConto.SelectedItems.Count > 0 Then
            Dim totalForAllConti As Double = 0

            ' Calcola il totale di tutti i conti selezionati
            For Each selectedItem As Object In lstConto.SelectedItems
                Dim contoType As String = selectedItem.ToString()

                If contoType = "Adebito" Then
                    totalForAllConti += GetTotalAdebitoByDateRange(startDate, endDate)
                ElseIf contoType = "Acredito" Then
                    totalForAllConti += GetTotalAcreditoByDateRange(startDate, endDate)
                ElseIf contoType = "Saldo" Then
                    ' Calcola la somma di Adebito tra le date selezionate
                    Dim addebitoSum As Double = GetTotalAdebitoByDateRange(startDate, endDate)

                    ' Calcola la somma di Acredito tra le date selezionate
                    Dim acreditoSum As Double = GetTotalAcreditoByDateRange(startDate, endDate)

                    ' Calcola il saldo come differenza tra Acredito e Adebito
                    totalForAllConti += (acreditoSum - addebitoSum)
                Else
                    ' Chiamata a una funzione per calcolare il totale in base a contoType e alle date selezionate
                    totalForAllConti += GetTotalByDateRangeAndConto(startDate, endDate, contoType)
                End If
            Next

            ' Aggiungi i dati al grafico Chart1
            For Each selectedItem As Object In lstConto.SelectedItems
                Dim contoType As String = selectedItem.ToString()
                Dim totalForConto As Double = 0

                If contoType = "Adebito" Then
                    totalForConto = GetTotalAdebitoByDateRange(startDate, endDate)
                ElseIf contoType = "Acredito" Then
                    totalForConto = GetTotalAcreditoByDateRange(startDate, endDate)
                ElseIf contoType = "Saldo" Then
                    ' Calcola la somma di Adebito tra le date selezionate
                    Dim addebitoSum As Double = GetTotalAdebitoByDateRange(startDate, endDate)

                    ' Calcola la somma di Acredito tra le date selezionate
                    Dim acreditoSum As Double = GetTotalAcreditoByDateRange(startDate, endDate)

                    ' Calcola il saldo come differenza tra Acredito e Adebito
                    totalForConto = (acreditoSum - addebitoSum)
                Else
                    ' Chiamata a una funzione per calcolare il totale in base a contoType e alle date selezionate
                    totalForConto = GetTotalByDateRangeAndConto(startDate, endDate, contoType)
                End If

                ' Aggiungi il dato al grafico Chart1
                Dim series As Series = Chart1.Series.Add(contoType)
                series.Points.Add(totalForConto)

                ' Imposta l'etichetta della legenda con l'importo
                series.Label = contoType & ": " & totalForConto.ToString("F2")
            Next



            ' Imposta l'etichetta dell'asse X per il grafico Chart1
            Chart1.ChartAreas(0).AxisX.Title = "Conti"
        Else
            MessageBox.Show("Seleziona almeno un tipo di conto dalla lista.")
        End If

        ' Aggiorna il totale spese nell'etichetta
        UpdateTotalSpeseLabel(startDate, endDate)
    End Sub

    Private Sub PopulateContiListBoxByDateRange(startDate As Date, endDate As Date)
        ' Pulisci la ListBox prima di aggiungere nuovi conti
        lstConto2.Items.Clear()

        Dim databasePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath & ";Version=3;"

        ' Variabili per calcolare il totale degli addebiti e degli accrediti
        Dim totalAdebito As Double = 0
        Dim totalAcredito As Double = 0
        Dim saldoComplessivo As Double = 0 ' Saldo complessivo dei conti
        Dim totaleComplessivo As Double = 0 ' Totale complessivo dei conti

        ' Creiamo una nuova serie per il grafico
        Dim series As New Series("Conti")
        ' Impostiamo il tipo di grafico a torta
        series.ChartType = SeriesChartType.Pie


        ' Otteniamo il totale degli addebiti e degli accrediti per tutto l'intervallo di date
        Using conn As New SQLiteConnection(connString)
            conn.Open()

            ' Calcoliamo il totale degli addebiti
            Using cmd As New SQLiteCommand("SELECT SUM(Adebito) FROM Movimenti WHERE Data BETWEEN @startDate AND @endDate", conn)
                cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))
                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing AndAlso Double.TryParse(result.ToString(), totalAdebito) Then
                    ' Calcoliamo il totale degli accrediti
                    Using cmd2 As New SQLiteCommand("SELECT SUM(Acredito) FROM Movimenti WHERE Data BETWEEN @startDate AND @endDate", conn)
                        cmd2.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                        cmd2.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))
                        Dim result2 = cmd2.ExecuteScalar()
                        If result2 IsNot Nothing AndAlso Double.TryParse(result2.ToString(), totalAcredito) Then
                            ' Calcoliamo il totale complessivo
                            saldoComplessivo = totalAcredito - totalAdebito
                            totaleComplessivo = totalAcredito - (-totalAdebito)

                            ' Otteniamo il numero reale dei conti interessati
                            Using cmd3 As New SQLiteCommand("SELECT COUNT(DISTINCT Conto) FROM Movimenti WHERE Data BETWEEN @startDate AND @endDate", conn)
                                cmd3.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                                cmd3.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))
                                Dim contiInteressati As Integer = Convert.ToInt32(cmd3.ExecuteScalar())
                                ' Aggiungiamo il numero reale dei conti interessati e il saldo complessivo alla ListBox
                                lstConto2.Items.Add($"Numero totale di conti interessati: {contiInteressati}, Saldo complessivo = {saldoComplessivo.ToString("F2")}, Totale complessivo {totaleComplessivo.ToString("F2")}")

                                ' Otteniamo i conti distinti e calcoliamo le percentuali
                                Using cmd4 As New SQLiteCommand("SELECT DISTINCT Conto, SUM(Adebito) AS TotalAdebito, SUM(Acredito) AS TotalAcredito FROM Movimenti WHERE Data BETWEEN @startDate AND @endDate GROUP BY Conto", conn)
                                    cmd4.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                                    cmd4.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))

                                    Using reader As SQLiteDataReader = cmd4.ExecuteReader()
                                        While reader.Read()
                                            Dim conto As String = reader.GetString(0)
                                            Dim saldo As Double = reader.GetDouble(2) - reader.GetDouble(1)
                                            Dim percentage As Double = 0

                                            ' Calcoliamo la percentuale in base al saldo
                                            If saldo <> 0 Then
                                                percentage = Math.Abs(saldo / totaleComplessivo) * 100

                                                ' Aggiungiamo solo i conti con importo diverso da zero alla ListBox
                                                lstConto2.Items.Add($"{conto}: Saldo = {saldo.ToString("F2")}, Percentuale = {percentage.ToString("F0")} %")

                                                ' Aggiungiamo i dati della serie solo per i conti con importo diverso da zero
                                                series.Points.AddXY(conto, percentage)
                                                series.Points(series.Points.Count - 1).LegendText = conto ' Impostiamo il nome del conto nella leggenda

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

        ' Impostiamo l'attributo IsVisibleInLegend a False per quei punti della serie che rappresentano conti con importo zero
        For Each point As DataPoint In series.Points
            If point.YValues(0) = 0 Then
                point.IsVisibleInLegend = False
            End If
        Next

        ' Impostiamo la personalizzazione delle etichette per visualizzare le percentuali
        For Each point As DataPoint In series.Points
            If point.YValues(0) <> 0 Then
                point.Label = "#PERCENT{P0}"

            End If
        Next
    End Sub


    Private Function GetTotalByDateRangeAndConto(startDate As Date, endDate As Date, contoType As String) As Double
        Dim total As Double = 0
        Dim databaseFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
        Dim databasePath As String = Path.Combine(databaseFolder, "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath & ";Version=3;"


        Using conn As New SQLiteConnection(connString)
            conn.Open()

            Using cmd As New SQLiteCommand(conn)
                If contoType = "Stipendi" Then
                    cmd.CommandText = "SELECT SUM(Acredito) FROM Movimenti WHERE Conto = @contoType AND Data BETWEEN @startDate AND @endDate"
                Else
                    cmd.CommandText = "SELECT SUM(Adebito - Acredito) FROM Movimenti WHERE Conto = @contoType AND Data BETWEEN @startDate AND @endDate"
                End If
                cmd.Parameters.AddWithValue("@contoType", contoType)
                cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))

                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing AndAlso Double.TryParse(result.ToString(), total) Then
                    Return total
                End If
            End Using
        End Using

        Return total
    End Function





    Private Function GetTotalAdebitoByDateRange(startDate As Date, endDate As Date) As Double
        ' Calcola la somma degli Addebiti tra le date selezionate
        Dim total As Double = 0
        Dim databasePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath & ";Version=3;"

        Using conn As New SQLiteConnection(connString)
            conn.Open()

            Using cmd As New SQLiteCommand(conn)
                cmd.CommandText = "SELECT SUM(Adebito) FROM Movimenti WHERE Data BETWEEN @startDate AND @endDate"
                cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))

                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing AndAlso Double.TryParse(result.ToString(), total) Then
                    Return total
                End If
            End Using
        End Using

        Return total
    End Function

    Private Function GetTotalAcreditoByDateRange(startDate As Date, endDate As Date) As Double
        ' Calcola la somma degli Accredito tra le date selezionate
        Dim total As Double = 0
        Dim databasePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath & ";Version=3;"

        Using conn As New SQLiteConnection(connString)
            conn.Open()

            Using cmd As New SQLiteCommand(conn)
                cmd.CommandText = "SELECT SUM(Acredito) FROM Movimenti WHERE Data BETWEEN @startDate AND @endDate"
                cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))

                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing AndAlso Double.TryParse(result.ToString(), total) Then
                    Return total
                End If
            End Using
        End Using

        Return total
    End Function





    Private Sub UpdateLabels()
        ' Calcolo Saldo dal primo gennaio alla data odierna
        Dim saldo As Double = GetTotalSaldoForDateRange(New Date(Date.Today.Year, 1, 1), Date.Today)
        lblSaldo.Text = $"Saldo dal primo gennaio: {saldo:F2}"

        ' Calcolo Patrimonio
        Dim patrimonio As Double = GetTotalPatrimonio()
        lblPatrimonio.Text = $"Patrimonio totale: {patrimonio:F2}"
    End Sub

    Private Function GetTotalSaldoForDateRange(startDate As Date, endDate As Date) As Double
        Dim total As Double = 0
        Dim databaseFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
        Dim databasePath As String = Path.Combine(databaseFolder, "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath & ";Version=3;"


        Using conn As New SQLiteConnection(connString)
            conn.Open()

            Using cmd As New SQLiteCommand(conn)
                ' Calcoliamo la differenza tra crediti (Acrediti) e debiti (Adebito)
                cmd.CommandText = "SELECT SUM(Acredito) - SUM(Adebito) FROM Movimenti WHERE Data >= @startDate AND Data <= @endDate"
                cmd.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                cmd.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))

                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing AndAlso Double.TryParse(result.ToString(), total) Then
                    Return total
                End If
            End Using
        End Using

        Return total
    End Function


    Private Function GetTotalPatrimonio() As Double
        Dim total As Double = 0
        Dim importoIniziale As Double = 0 ' Puoi modificare questo valore in base al tuo importo iniziale
        Dim databaseFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
        Dim databasePath As String = Path.Combine(databaseFolder, "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath & ";Version=3;"


        Using conn As New SQLiteConnection(connString)
            conn.Open()

            Using cmd As New SQLiteCommand(conn)
                cmd.CommandText = "SELECT SUM(Acredito) - SUM(Adebito) - @importoIniziale FROM Movimenti"
                cmd.Parameters.AddWithValue("@importoIniziale", importoIniziale)

                Dim result = cmd.ExecuteScalar()
                If result IsNot Nothing AndAlso Double.TryParse(result.ToString(), total) Then
                    Return total
                End If
            End Using
        End Using

        Return total
    End Function

    'riepilogo mensile ----------------------------------------------------------------------------------------------
    Private Function GetMonthlySummary() As DataTable
        Dim dtSummary As New DataTable
        dtSummary.Columns.Add("Mese")
        dtSummary.Columns.Add("Anno")
        dtSummary.Columns.Add("Spese Mensili", GetType(Double))
        dtSummary.Columns.Add("Entrate Mensili", GetType(Double))

        Dim databaseFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
        Dim databasePath As String = Path.Combine(databaseFolder, "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath & ";Version=3;"

        Using conn As New SQLiteConnection(connString)
            conn.Open()

            ' Query per estrarre tutti i mesi e anni distinti dal database
            Using cmd As New SQLiteCommand("SELECT DISTINCT strftime('%Y', Data) AS Anno, strftime('%m', Data) AS Mese FROM Movimenti", conn)
                Using reader As SQLiteDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        Dim year As String = reader("Anno").ToString()
                        Dim month As String = reader("Mese").ToString()

                        ' Calcolare la data di inizio e fine mese
                        Dim startDate As New Date(Convert.ToInt32(year), Convert.ToInt32(month), 1)
                        Dim endDate As New Date(Convert.ToInt32(year), Convert.ToInt32(month), Date.DaysInMonth(Convert.ToInt32(year), Convert.ToInt32(month)))

                        ' Calcolare le spese mensili
                        Using cmd2 As New SQLiteCommand("SELECT SUM(Adebito) FROM Movimenti WHERE Data BETWEEN @startDate AND @endDate", conn)
                            cmd2.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                            cmd2.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))
                            Dim speseMensili As Object = cmd2.ExecuteScalar()
                            If speseMensili Is DBNull.Value Then speseMensili = 0 ' Gestione di DBNull

                            ' Calcolare gli ingressi mensili
                            Using cmd3 As New SQLiteCommand("SELECT SUM(Acredito) FROM Movimenti WHERE Data BETWEEN @startDate AND @endDate", conn)
                                cmd3.Parameters.AddWithValue("@startDate", startDate.ToString("yyyy-MM-dd"))
                                cmd3.Parameters.AddWithValue("@endDate", endDate.ToString("yyyy-MM-dd"))
                                Dim ingressiMensili As Object = cmd3.ExecuteScalar()
                                If ingressiMensili Is DBNull.Value Then ingressiMensili = 0 ' Gestione di DBNull

                                ' Aggiungi i dati alla tabella
                                dtSummary.Rows.Add(MonthName(Convert.ToInt32(month)) & " " & year, year, Convert.ToDouble(speseMensili), Convert.ToDouble(ingressiMensili))
                            End Using
                        End Using
                    End While
                End Using
            End Using
        End Using

        Return dtSummary
    End Function


    Private Sub UpdateMonthlySummary()
        ' Ottieni i dati del riepilogo mensile
        Dim monthlySummary As DataTable = GetMonthlySummary()

        ' Imposta la sorgente dati del DataGridView
        DataGridView1.DataSource = monthlySummary
    End Sub

    Private Sub DataGridView1_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles DataGridView1.CellFormatting
        ' Verifica che il valore sia numerico
        If e.ColumnIndex = DataGridView1.Columns("Spese Mensili").Index OrElse e.ColumnIndex = DataGridView1.Columns("Entrate Mensili").Index Then
            If e.Value IsNot Nothing AndAlso IsNumeric(e.Value) Then
                ' Format the value as currency (Euro, two decimals)
                e.Value = Convert.ToDouble(e.Value).ToString("C2")
            End If
        End If
    End Sub


    Private Sub PopulateYearComboBox()
        Dim monthlySummary As DataTable = GetMonthlySummary()
        Dim uniqueYears = monthlySummary.AsEnumerable() _
                       .Select(Function(row) DateTime.ParseExact(row("Mese").ToString(), "MMMM yyyy", New Globalization.CultureInfo("it-IT")).Year) _
                       .Distinct() _
                       .OrderBy(Function(year) year) _
                       .ToList()

        cmbAnno.Items.Clear()
        For Each year As Integer In uniqueYears
            cmbAnno.Items.Add(year)
        Next

        ' Seleziona automaticamente il primo anno disponibile (opzionale)
        If cmbAnno.Items.Count > 0 Then
            cmbAnno.SelectedIndex = 0
        End If
    End Sub
    Private Sub UpdateChartForSelectedYear()
        ' Controlla se è stato selezionato un anno
        If cmbAnno.SelectedItem Is Nothing Then Exit Sub

        ' Ottieni l'anno selezionato
        Dim selectedYear As Integer = Convert.ToInt32(cmbAnno.SelectedItem)

        ' Pulisci il grafico
        Chart3.Series.Clear()

        ' Ottieni i dati del riepilogo mensile
        Dim monthlySummary As DataTable = GetMonthlySummary()

        ' Filtra i dati per l'anno selezionato
        Dim filteredSummary = monthlySummary.AsEnumerable() _
                         .Where(Function(row) DateTime.ParseExact(row("Mese").ToString(), "MMMM yyyy", New Globalization.CultureInfo("it-IT")).Year = selectedYear) _
                         .ToList()

        ' Calcola il patrimonio cumulativo
        Dim patrimonioCumulativo As Double = 0

        ' Aggiungi la serie per il patrimonio cumulativo
        Dim seriesPatrimonioCumulativo As Series = Chart3.Series.Add("Patrimonio Cumulativo")
        seriesPatrimonioCumulativo.ChartType = SeriesChartType.Line
        seriesPatrimonioCumulativo.BorderWidth = 3

        ' Aggiungi la serie per il patrimonio totale
        Dim seriesPatrimonioTotale As Series = Chart3.Series.Add("Patrimonio Totale")
        seriesPatrimonioTotale.ChartType = SeriesChartType.Line
        seriesPatrimonioTotale.BorderWidth = 3
        seriesPatrimonioTotale.Color = Color.Red

        ' Ordina i mesi per il grafico
        Dim culture As New Globalization.CultureInfo("it-IT")
        filteredSummary.Sort(Function(row1, row2)
                                 Dim date1 As DateTime = DateTime.ParseExact(row1("Mese").ToString(), "MMMM yyyy", culture)
                                 Dim date2 As DateTime = DateTime.ParseExact(row2("Mese").ToString(), "MMMM yyyy", culture)
                                 Return date1.CompareTo(date2)
                             End Function)

        ' Aggiungi i dati al grafico
        For Each row In filteredSummary
            Dim speseMensili As Double = Convert.ToDouble(row("Spese Mensili"))
            Dim ingressiMensili As Double = Convert.ToDouble(row("Entrate Mensili"))

            patrimonioCumulativo += ingressiMensili - speseMensili

            Dim dataMese As DateTime = DateTime.ParseExact(row("Mese").ToString(), "MMMM yyyy", culture)
            Dim monthIndex As Integer = dataMese.Month

            seriesPatrimonioCumulativo.Points.AddXY(monthIndex, patrimonioCumulativo)
            seriesPatrimonioTotale.Points.AddXY(monthIndex, patrimonioCumulativo)
        Next

        ' Imposta l'intervallo dell'asse X
        Chart3.ChartAreas(0).AxisX.Minimum = 1
        Chart3.ChartAreas(0).AxisX.Maximum = 12
        Chart3.ChartAreas(0).AxisX.Interval = 1
        Chart3.ChartAreas(0).AxisX.Title = "Mesi"

        ' Imposta l'asse Y (valori monetari)
        Chart3.ChartAreas(0).AxisY.Title = "Patrimonio (€)"
        Chart3.ChartAreas(0).AxisY.LabelStyle.Format = "C0"

        ' Impostazioni generali per il grafico
        Chart3.Legends.Clear()
    End Sub
    Private Sub cmbAnno_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbAnno.SelectedIndexChanged
        UpdateChartForSelectedYear()
    End Sub


    Private Function GetYearlyAverage() As DataTable
        Dim dtYearlyAverage As New DataTable
        dtYearlyAverage.Columns.Add("Anno")
        dtYearlyAverage.Columns.Add("Media Spese Mensili", GetType(Double))
        dtYearlyAverage.Columns.Add("Media Entrate Mensili", GetType(Double))
        dtYearlyAverage.Columns.Add("Media Spese Giornaliera", GetType(Double))
        dtYearlyAverage.Columns.Add("Media Entrate Giornaliera", GetType(Double))

        Dim databaseFolder As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
        Dim databasePath As String = Path.Combine(databaseFolder, "amministrazione.db")
        Dim connString As String = "Data Source=" & databasePath & ";Version=3;"

        Using conn As New SQLiteConnection(connString)
            conn.Open()

            Dim query As String = "SELECT strftime('%Y', Data) AS Anno, 
                                      strftime('%m', Data) AS Mese, 
                                      SUM(Adebito) AS SommaSpeseMensili, 
                                      SUM(Acredito) AS SommaIngressiMensili 
                               FROM Movimenti 
                               GROUP BY strftime('%Y', Data), strftime('%m', Data)"
            Using cmd As New SQLiteCommand(query, conn)
                Dim tempData As New Dictionary(Of String, (Double, Double, Integer)) ' Anno -> (Somma Spese, Somma Ingressi, Conteggio Mesi)

                Using reader As SQLiteDataReader = cmd.ExecuteReader()
                    While reader.Read()
                        Dim anno As String = reader("Anno").ToString()
                        Dim sommaSpese As Double = If(IsDBNull(reader("SommaSpeseMensili")), 0, Convert.ToDouble(reader("SommaSpeseMensili")))
                        Dim sommaIngressi As Double = If(IsDBNull(reader("SommaIngressiMensili")), 0, Convert.ToDouble(reader("SommaIngressiMensili")))

                        If Not tempData.ContainsKey(anno) Then
                            tempData(anno) = (0, 0, 0)
                        End If

                        Dim current = tempData(anno)
                        tempData(anno) = (current.Item1 + sommaSpese, current.Item2 + sommaIngressi, current.Item3 + 1)
                    End While
                End Using

                ' Calcola la media per ogni anno
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

                    dtYearlyAverage.Rows.Add(anno, Math.Round(mediaSpeseMensili, 2), Math.Round(mediaIngressiMensili, 2), Math.Round(mediaSpeseGiornaliere, 2), Math.Round(mediaIngressiGiornaliere, 2))
                Next
            End Using
        End Using

        Return dtYearlyAverage
    End Function


    Private Sub UpdateYearlyAverageGrid()
        Dim yearlyAverage As DataTable = GetYearlyAverage()
        DataGridView2.DataSource = yearlyAverage
    End Sub




    'fine riepilogo mensile------------------------------------------------------------------------------------------


End Class