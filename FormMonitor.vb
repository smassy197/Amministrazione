Imports Microsoft.Data.Sqlite
Imports System.Windows.Forms.DataVisualization.Charting
Imports System.Threading.Tasks
Imports System.IO
Imports System.Drawing
Imports System.Linq
Imports System.Text

Public Class FormMonitor
    Inherits Form

    Private cmbConto As ComboBox
    Private cmbFieldType As ComboBox
    Private topPanel As Panel
    Private numThreshold As NumericUpDown
    Private btnRunNow As Button
    Private chkAuto As CheckBox
    Private chkShowAllYears As CheckBox
    Private numInterval As NumericUpDown
    Private lblFieldType As Label
    Private lblSelection As Label
    Private lblThresholdLabel As Label
    Private lblIntervalLabel As Label
    Private lvAlerts As ListView
    Private chartMonitor As Chart
    Private lblStatus As Label
    Private monitorTimer As New Timer()
    Private txtLog As TextBox

    ' Helper object attached to ListViewItem.Tag to provide details for drilldown
    Public Class AlertInfo
        Public Property MonthIndex As Integer
        Public Property PrevValue As Double
        Public Property CurrValue As Double
        Public Property Delta As Double
        Public Property Pct As Double
        Public Property PrevYears As List(Of Integer)
        Public Property CurrentYear As Integer
        Public Property Field As String
        Public Property Key As String
        Public Property PrevLabel As String
    End Class

    Public Sub New()
        Me.Text = "Monitor Andamenti"
        Me.Size = New Drawing.Size(900, 600)
        InitializeComponent()
        ' Try to set application icon if available and disable maximize
        Try
            Dim icoPath = IO.Path.Combine(Application.StartupPath, "C:\Users\smass\source\repos\Amministrazione\amministrazione_new_logo.ico")
            If IO.File.Exists(icoPath) Then
                Me.Icon = New Icon(icoPath)
            End If
        Catch ex As Exception
            ' ignore icon load errors
        End Try
        Me.MaximizeBox = False
    End Sub

    Private Sub UpdateChartMultiYear(data As Dictionary(Of Integer, Double()), currentYear As Integer)
        chartMonitor.Series.Clear()
        chartMonitor.Annotations.Clear()
        ' Add a series per year (monthly columns)
        Dim colors As Color() = {Color.LightBlue, Color.Coral, Color.LightGreen, Color.Plum, Color.Orange, Color.SkyBlue, Color.YellowGreen, Color.Salmon}
        Dim idxCol As Integer = 0
        Dim maxVal As Double = 0
        For Each y In data.Keys.OrderBy(Function(x) x)
            Dim s As Series = chartMonitor.Series.Add(y.ToString())
            s.ChartType = SeriesChartType.Column
            s.Color = colors(idxCol Mod colors.Length)
            Dim arr = data(y)
            Dim total As Double = 0
            For i As Integer = 0 To 11
                s.Points.AddXY(MonthName(i + 1), arr(i))
                total += arr(i)
                If Math.Abs(arr(i)) > maxVal Then maxVal = Math.Abs(arr(i))
            Next
            ' Do not show per-month labels and ensure no point labels are displayed
            s.IsValueShownAsLabel = False
            For Each pt In s.Points
                pt.Label = String.Empty
            Next
            s.LegendText = y.ToString()
            idxCol += 1
        Next

        If maxVal <= 0 Then maxVal = 1
        Dim yMax As Double = Math.Ceiling(maxVal * 1.1)
        Dim yInterval As Double = Math.Max(1, Math.Round(yMax / 10))

        With chartMonitor.ChartAreas(0)
            .AxisX.Interval = 1
            .AxisX.Title = "Mese"
            .AxisY.Title = "Totale"
            .AxisY.Minimum = 0
            .AxisY.Maximum = yMax
            .AxisY.Interval = yInterval
            .AxisY.LabelStyle.Format = "C0"
        End With
    End Sub

    Private Sub InitializeComponent()
        Console.WriteLine("[DEBUG] InitializeComponent: start")
        ' Top panel to host controls and avoid overlap on different DPI/sizes
        topPanel = New Panel() With {.Left = 0, .Top = 0, .Height = 72, .Dock = DockStyle.Top}

        ' First row: field type, value, threshold, run and auto
        lblFieldType = New Label() With {.Left = 10, .Top = 12, .Width = 60, .Text = "Campo:", .AutoSize = True}
        cmbFieldType = New ComboBox() With {.Left = 75, .Top = 10, .Width = 120, .DropDownStyle = ComboBoxStyle.DropDownList}

        lblSelection = New Label() With {.Left = 205, .Top = 12, .Width = 60, .Text = "Valore:", .AutoSize = True}
        cmbConto = New ComboBox() With {.Left = 270, .Top = 10, .Width = 320, .DropDownStyle = ComboBoxStyle.DropDownList}
        cmbConto.DropDownWidth = 400

        lblThresholdLabel = New Label() With {.Left = 600, .Top = 12, .Width = 70, .Text = "Soglia %:", .AutoSize = True}
        numThreshold = New NumericUpDown() With {.Left = 675, .Top = 10, .Width = 60, .Minimum = 1, .Maximum = 100, .Value = 20}

        btnRunNow = New Button() With {.Left = 740, .Top = 8, .Text = "Run Now", .Width = 80}
        chkAuto = New CheckBox() With {.Left = 830, .Top = 12, .Text = "Auto monitor", .Width = 80}
        chkShowAllYears = New CheckBox() With {.Left = 400, .Top = 40, .Text = "Mostra tutti gli anni", .AutoSize = True}

        ' Second row: interval and status
        lblIntervalLabel = New Label() With {.Left = 10, .Top = 40, .Width = 120, .Text = "Intervallo (min):", .AutoSize = True}
        numInterval = New NumericUpDown() With {.Left = 135, .Top = 38, .Width = 60, .Minimum = 1, .Maximum = 1440, .Value = 60}
        lblStatus = New Label() With {.Left = 210, .Top = 40, .Width = 400, .Text = "Idle", .AutoSize = True}

        ' Place the alerts and chart below the controls (avoid overlap)
        lvAlerts = New ListView() With {.Left = 10, .Top = 80, .Width = 420, .Height = 480, .View = View.Details, .FullRowSelect = True}
        lvAlerts.Columns.Add("Mese", 80)
        lvAlerts.Columns.Add("AnnoPrev", 80)
        lvAlerts.Columns.Add("AnnoCurr", 80)
        lvAlerts.Columns.Add("Delta", 80)
        lvAlerts.Columns.Add("%", 80)

        chartMonitor = New Chart() With {.Left = 440, .Top = 80, .Width = 440, .Height = 480}
        chartMonitor.ChartAreas.Add(New ChartArea("Main"))
        chartMonitor.Legends.Add(New Legend("L"))

        ' Add controls into top panel
        topPanel.Controls.Add(lblFieldType)
        topPanel.Controls.Add(cmbFieldType)
        topPanel.Controls.Add(lblSelection)
        topPanel.Controls.Add(cmbConto)
        topPanel.Controls.Add(lblThresholdLabel)
        topPanel.Controls.Add(numThreshold)
        topPanel.Controls.Add(btnRunNow)
        topPanel.Controls.Add(chkAuto)
        topPanel.Controls.Add(chkShowAllYears)
        topPanel.Controls.Add(lblIntervalLabel)
        topPanel.Controls.Add(numInterval)
        topPanel.Controls.Add(lblStatus)
        Me.Controls.Add(topPanel)
        Me.Controls.Add(lvAlerts)
        Me.Controls.Add(chartMonitor)

        ' Log text box at bottom
        txtLog = New TextBox() With {
            .Left = 10,
            .Top = 450,
            .Width = 870,
            .Height = 120,
            .Multiline = True,
            .ReadOnly = True,
            .ScrollBars = ScrollBars.Vertical,
            .Anchor = AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Bottom Or AnchorStyles.Top
        }
        Me.Controls.Add(txtLog)

        AddHandler Me.Load, AddressOf FormMonitor_Load
        AddHandler btnRunNow.Click, AddressOf BtnRunNow_Click
        AddHandler lvAlerts.DoubleClick, AddressOf LvAlerts_DoubleClick
        AddHandler chkAuto.CheckedChanged, AddressOf ChkAuto_CheckedChanged
        AddHandler monitorTimer.Tick, AddressOf MonitorTimer_Tick
        AddHandler cmbFieldType.SelectedIndexChanged, AddressOf CmbFieldType_SelectedIndexChanged
        Console.WriteLine("[DEBUG] InitializeComponent: handlers attached")
        Console.WriteLine("[DEBUG] InitializeComponent: end")
    End Sub

    Private Sub FormMonitor_Load(sender As Object, e As EventArgs)
        Console.WriteLine("[DEBUG] FormMonitor_Load")
        ' Popola il tipo di campo (Conto o Descrizione)
        cmbFieldType.Items.Clear()
        cmbFieldType.Items.Add("Conto")
        cmbFieldType.Items.Add("Descrizione")
        cmbFieldType.SelectedIndex = 0

        Console.WriteLine("[DEBUG] FormMonitor_Load: PopulateConti call")
        PopulateConti()
        Console.WriteLine("[DEBUG] FormMonitor_Load: PopulateConti returned")
        monitorTimer.Interval = CInt(numInterval.Value) * 60 * 1000 ' minutes
        lblStatus.Text = "Pronto"
        InitializeNotifyIcon()
        chkShowAllYears.Checked = False
        Console.WriteLine("[DEBUG] FormMonitor_Load: InitializeNotifyIcon returned")

        ' Only run runtime-only initialization when not in designer
        If Not Me.DesignMode Then
            ' Redirect console output to the log textbox so exceptions and debug are visible
            Try
                Dim origOut = Console.Out
                Dim origErr = Console.Error
                Console.SetOut(New TextBoxWriter(txtLog, origOut))
                Console.SetError(New TextBoxWriter(txtLog, origErr))
                Console.WriteLine("[DEBUG] Console redirected to FormMonitor log textbox (and original console).")
            Catch ex As Exception
                ' If redirect fails, still log to original console
                Try
                    Console.WriteLine("[DEBUG] Errore redirect console: " & ex.ToString())
                Catch inner As Exception
                End Try
            End Try

            ' Global exception handlers to ensure exceptions are logged to the textbox
            Try
                AddHandler Application.ThreadException, AddressOf OnThreadException
                AddHandler TaskScheduler.UnobservedTaskException, AddressOf OnUnobservedTaskException
                AddHandler AppDomain.CurrentDomain.UnhandledException, AddressOf OnDomainUnhandledException
                Console.WriteLine("[DEBUG] Global exception handlers attached.")
            Catch ex As Exception
                Console.WriteLine("[DEBUG] Errore attach global handlers: " & ex.ToString())
            End Try
        End If
    End Sub
    ' No-op to ensure prior change applied correctly

    Private notify As NotifyIcon
    Private Sub InitializeNotifyIcon()
        notify = New NotifyIcon()
        notify.Icon = SystemIcons.Application
        notify.Visible = True
        Console.WriteLine("[DEBUG] InitializeNotifyIcon: notify created and visible")
    End Sub

    Private Sub LogMessage(msg As String)
        Try
            ' Write to Console (will be redirected to textbox if SetOut applied)
            Try
                Console.WriteLine(msg)
            Catch ex As Exception
            End Try

            ' Also write to standard error
            Try
                Console.Error.WriteLine(msg)
            Catch ex As Exception
            End Try

            ' VS Output
            Try
                System.Diagnostics.Debug.WriteLine(msg)
            Catch ex As Exception
            End Try
            Try
                System.Diagnostics.Trace.WriteLine(msg)
            Catch ex As Exception
            End Try

            ' Append to persistent log file
            Try
                Dim logFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
                If Not Directory.Exists(logFolder) Then Directory.CreateDirectory(logFolder)
                Dim logPath = Path.Combine(logFolder, "monitor_log.txt")
                Using sw As New StreamWriter(logPath, True)
                    sw.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {msg}")
                End Using
            Catch ex As Exception
                ' don't fail on logging
            End Try
        Catch ex As Exception
            ' swallow
        End Try
    End Sub

    Private Sub OnThreadException(sender As Object, e As Threading.ThreadExceptionEventArgs)
        Try
            Console.WriteLine("[UNHANDLED UI THREAD] " & e.Exception.ToString())
            MessageBox.Show("Errore in UI thread: " & e.Exception.Message)
        Catch ex As Exception
            Console.WriteLine("[ERROR] OnThreadException failed: " & ex.ToString())
        End Try
    End Sub

    Private Sub OnUnobservedTaskException(sender As Object, e As UnobservedTaskExceptionEventArgs)
        Try
            Console.WriteLine("[UNOBSERVED TASK] " & e.Exception.ToString())
            e.SetObserved()
        Catch ex As Exception
            Console.WriteLine("[ERROR] OnUnobservedTaskException failed: " & ex.ToString())
        End Try
    End Sub

    Private Sub OnDomainUnhandledException(sender As Object, e As UnhandledExceptionEventArgs)
        Try
            Console.WriteLine("[DOMAIN UNHANDLED] " & e.ExceptionObject.ToString())
        Catch ex As Exception
            Console.WriteLine("[ERROR] OnDomainUnhandledException failed: " & ex.ToString())
        End Try
    End Sub

    Private Sub PopulateConti()
        Try
            Console.WriteLine("[DEBUG] PopulateConti: start")
            Dim databasePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
            Dim connString As String = "Data Source=" & databasePath

            Using conn As New Microsoft.Data.Sqlite.SqliteConnection(connString)
                conn.Open()
                Dim field As String = If(cmbFieldType.SelectedItem IsNot Nothing AndAlso cmbFieldType.SelectedItem.ToString() = "Descrizione", "Descrizione", "Conto")
                Using cmd As New Microsoft.Data.Sqlite.SqliteCommand($"SELECT DISTINCT " & field & " FROM Movimenti ORDER BY " & field, conn)
                    Using reader = cmd.ExecuteReader()
                        cmbConto.Items.Clear()
                        While reader.Read()
                            Dim val = reader(field).ToString()
                            cmbConto.Items.Add(val)
                            Console.WriteLine("[DEBUG] PopulateConti: added value -> " & val)
                        End While
                    End Using
                End Using
            End Using
            If cmbConto.Items.Count > 0 Then cmbConto.SelectedIndex = 0
            Console.WriteLine("[DEBUG] PopulateConti: completed. Count=" & cmbConto.Items.Count)
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore PopulateConti: " & ex.ToString())
        End Try
    End Sub

    ' TextWriter that appends text to a TextBox (thread-safe)
    Private Class TextBoxWriter
        Inherits IO.TextWriter

        Private _txt As TextBox
        Private _fallback As IO.TextWriter

        Public Sub New(txt As TextBox, Optional fallback As IO.TextWriter = Nothing)
            _txt = txt
            _fallback = If(fallback, Console.Out)
        End Sub

        Public Overrides ReadOnly Property Encoding As Encoding
            Get
                Return Encoding.UTF8
            End Get
        End Property

        Private Sub WriteToTextBox(value As String)
            Try
                If _txt Is Nothing Then Return
                If _txt.InvokeRequired Then
                    _txt.BeginInvoke(Sub() _txt.AppendText(value))
                Else
                    _txt.AppendText(value)
                End If
            Catch ex As Exception
                ' ignore logging errors to textbox
            End Try
        End Sub

        Public Overrides Sub Write(value As String)
            ' Write to original console as well as the textbox
            Try
                If _fallback IsNot Nothing Then
                    Try
                        _fallback.Write(value)
                        _fallback.Flush()
                    Catch ex As Exception
                        ' ignore fallback write errors
                    End Try
                End If
                ' Also write to VS Output window
                Try
                    System.Diagnostics.Debug.Write(value)
                Catch ex As Exception
                End Try

                WriteToTextBox(value)
            Catch ex As Exception
                ' ignore
            End Try
        End Sub

        Public Overrides Sub WriteLine(value As String)
            Write(value & Environment.NewLine)
        End Sub
    End Class

    Private Sub CmbFieldType_SelectedIndexChanged(sender As Object, e As EventArgs)
        PopulateConti()
    End Sub

    Private Sub LvAlerts_DoubleClick(sender As Object, e As EventArgs)
        Try
            If lvAlerts.SelectedItems.Count = 0 Then Return
            Dim li = lvAlerts.SelectedItems(0)
            If li.Tag IsNot Nothing AndAlso TypeOf li.Tag Is AlertInfo Then
                Dim info = DirectCast(li.Tag, AlertInfo)
                Dim f As New FormAlertDetails(info)
                f.ShowDialog()
            Else
                MessageBox.Show("Nessun dettaglio disponibile per questa riga.")
            End If
        Catch ex As Exception
            Console.WriteLine("[DEBUG] LvAlerts_DoubleClick error: " & ex.ToString())
        End Try
    End Sub

    Private Sub BtnRunNow_Click(sender As Object, e As EventArgs)
        RunCheckAsync()
    End Sub

    Private Sub ChkAuto_CheckedChanged(sender As Object, e As EventArgs)
        If chkAuto.Checked Then
            monitorTimer.Interval = CInt(numInterval.Value) * 60 * 1000
            monitorTimer.Start()
            lblStatus.Text = "Monitor attivo"
            Console.WriteLine("[DEBUG] Monitor avviato. Interval minuti: " & numInterval.Value)
        Else
            monitorTimer.Stop()
            lblStatus.Text = "Monitor fermo"
            Console.WriteLine("[DEBUG] Monitor fermato.")
        End If
    End Sub

    Private Sub MonitorTimer_Tick(sender As Object, e As EventArgs)
        RunCheckAsync()
    End Sub

    Private Sub RunCheckAsync()
        If cmbConto.SelectedItem Is Nothing Then
            MessageBox.Show("Seleziona una categoria/conto.")
            Return
        End If

        Dim conto As String = cmbConto.SelectedItem.ToString()
        Dim threshold As Double = CDbl(numThreshold.Value)

        ' Capture any UI values needed by the background task to avoid cross-thread access
        Dim field As String = If(cmbFieldType.SelectedItem IsNot Nothing AndAlso cmbFieldType.SelectedItem.ToString() = "Descrizione", "Descrizione", "Conto")
        Dim showAllYears As Boolean = chkShowAllYears.Checked

        lblStatus.Text = "Controllo in corso..."
        Task.Run(Sub()
                     Try
                         Dim currentYear = DateTime.Today.Year
                         Dim prevYear = currentYear - 1

                         ' Get all available years for this selection
                         Dim years = GetAvailableYears(field, conto)
                         If years.Count = 0 Then
                             ' Nothing to do
                             Me.BeginInvoke(New MethodInvoker(Sub() lblStatus.Text = "Nessun dato disponibile"))
                             Return
                         End If

                         Dim data As New Dictionary(Of Integer, Double())
                         For Each y In years
                             data(y) = GetMonthlyTotals(field, conto, y)
                         Next

                         ' Use the latest year as current, previous years as baseline (average)
                         Dim currentYearFromData As Integer = years.Max()
                         Dim prevYears = years.Where(Function(yy) yy <> currentYearFromData).ToList()

                         Dim prevAvg(11) As Double
                         For i As Integer = 0 To 11
                             Dim sum As Double = 0
                             Dim cnt As Integer = 0
                             For Each py In prevYears
                                 sum += data(py)(i)
                                 cnt += 1
                             Next
                             prevAvg(i) = If(cnt > 0, sum / cnt, 0)
                         Next

                         Dim currArr(11) As Double
                         For i As Integer = 0 To 11
                             currArr(i) = data(currentYearFromData)(i)
                         Next

                         Dim alerts As New List(Of String)
                         Dim alertMonths As New List(Of Integer) ' parallel list to map alerts -> month index (0-based)
                         Dim culture = New Globalization.CultureInfo("it-IT")

                         Dim items As New List(Of String())

                         ' Track the alert with the largest positive delta (current - baseline) so we can highlight the month
                         Dim maxAlertDelta As Double = 0
                         Dim highlightedMonthIndex As Integer = -1

                         For m As Integer = 1 To 12
                             Dim prevVal = prevAvg(m - 1)
                             Dim currVal = currArr(m - 1)
                             Dim delta = currVal - prevVal
                             Dim pct As Double = 0
                             If prevVal = 0 Then
                                 If currVal > 0 Then pct = 100 Else pct = 0
                             Else
                                 pct = (delta / Math.Abs(prevVal)) * 100
                             End If

                             ' Only consider positive increases as alerts (spending more this year vs baseline)
                             If pct >= threshold AndAlso delta > 0 Then
                                 Dim prevRangeText As String
                                 If prevYears.Count = 0 Then
                                     prevRangeText = "0"
                                 ElseIf prevYears.Count = 1 Then
                                     prevRangeText = prevYears(0).ToString()
                                 Else
                                     prevRangeText = prevYears.Min().ToString() & "-" & prevYears.Max().ToString()
                                 End If
                                 alerts.Add($"{MonthName(m)}: {prevRangeText}→{currentYearFromData} Δ={delta:F2} ({pct:F1}%)")
                                 alertMonths.Add(m - 1)
                                 ' Update the highlighted month if this alert has a larger positive delta
                                 If delta > maxAlertDelta Then
                                     maxAlertDelta = delta
                                     highlightedMonthIndex = m - 1 ' zero-based index for ListView rows
                                 End If
                             End If

                             Dim row() As String = {MonthName(m), prevVal.ToString("F2"), currVal.ToString("F2"), delta.ToString("F2"), pct.ToString("F1") & "%"}
                             items.Add(row)
                         Next

                         ' Aggiorna UI
                         Me.BeginInvoke(New MethodInvoker(Sub()
                                                              lvAlerts.Items.Clear()
                                                              ' Compute prevLabel for tag
                                                              Dim prevLabelLocal As String
                                                              If prevYears.Count = 0 Then
                                                                  prevLabelLocal = "Baseline"
                                                              ElseIf prevYears.Count = 1 Then
                                                                  prevLabelLocal = prevYears(0).ToString()
                                                              Else
                                                                  prevLabelLocal = prevYears.Min().ToString() & "-" & prevYears.Max().ToString()
                                                              End If

                                                              Dim totalPrevAll As Double = 0
                                                              Dim totalCurrAll As Double = 0
                                                              Dim totalDeltaAll As Double = 0
                                                              For m As Integer = 1 To 12
                                                                  Dim prevVal = prevAvg(m - 1)
                                                                  Dim currVal = currArr(m - 1)
                                                                  Dim delta = currVal - prevVal
                                                                  Dim pct As Double = 0
                                                                  If prevVal = 0 Then
                                                                      If currVal > 0 Then pct = 100 Else pct = 0
                                                                  Else
                                                                      pct = (delta / Math.Abs(prevVal)) * 100
                                                                  End If

                                                                  Dim row() As String = {MonthName(m), prevVal.ToString("F2"), currVal.ToString("F2"), delta.ToString("F2"), pct.ToString("F1") & "%"}
                                                                  Dim lw As New ListViewItem(row)
                                                                  Dim info As New AlertInfo()
                                                                  info.MonthIndex = m - 1
                                                                  info.PrevValue = prevVal
                                                                  info.CurrValue = currVal
                                                                  info.Delta = delta
                                                                  info.Pct = pct
                                                                  info.PrevYears = prevYears
                                                                  info.CurrentYear = currentYearFromData
                                                                  info.Field = field
                                                                  info.Key = conto
                                                                  info.PrevLabel = prevLabelLocal
                                                                  lw.Tag = info
                                                                  lvAlerts.Items.Add(lw)

                                                                  ' accumulate totals
                                                                  totalPrevAll += prevVal
                                                                  totalCurrAll += currVal
                                                                  totalDeltaAll += delta
                                                              Next

                                                              ' Append totals row as the last row
                                                              Try
                                                                  Dim pctTotal As Double = 0
                                                                  If totalPrevAll = 0 Then
                                                                      pctTotal = If(totalCurrAll > 0, 100, 0)
                                                                  Else
                                                                      pctTotal = (totalDeltaAll / Math.Abs(totalPrevAll)) * 100
                                                                  End If
                                                                  Dim totalRow() As String = {"Totali", totalPrevAll.ToString("F2"), totalCurrAll.ToString("F2"), totalDeltaAll.ToString("F2"), pctTotal.ToString("F1") & "%"}
                                                                  Dim lwTot As New ListViewItem(totalRow)
                                                                  ' Ensure the totals row is bold and the style is applied to subitems
                                                                  lwTot.Font = New Drawing.Font(lwTot.Font, Drawing.FontStyle.Bold)
                                                                  Try
                                                                      lwTot.UseItemStyleForSubItems = True
                                                                  Catch exUse As Exception
                                                                      ' Older frameworks may not support this property; ignore
                                                                  End Try
                                                                  lvAlerts.Items.Add(lwTot)
                                                              Catch exTot As Exception
                                                                  Console.WriteLine("[DEBUG] Errore aggiunta riga Totali: " & exTot.ToString())
                                                              End Try

                                                              ' If we flagged a highlighted month, style that row to make it stand out
                                                              If highlightedMonthIndex >= 0 AndAlso highlightedMonthIndex < lvAlerts.Items.Count Then
                                                                  Try
                                                                      ' Reset any previous styling
                                                                      For iIdx As Integer = 0 To lvAlerts.Items.Count - 1
                                                                          Dim resetItem = lvAlerts.Items(iIdx)
                                                                          resetItem.BackColor = lvAlerts.BackColor
                                                                          resetItem.ForeColor = SystemColors.WindowText
                                                                          resetItem.Font = New Drawing.Font(resetItem.Font, Drawing.FontStyle.Regular)
                                                                          resetItem.Selected = False
                                                                          resetItem.Focused = False
                                                                      Next

                                                                      Dim li = lvAlerts.Items(highlightedMonthIndex)
                                                                      ' Set a clear yellow background and bold font for the alert row
                                                                      li.BackColor = Drawing.Color.Yellow
                                                                      li.ForeColor = Drawing.Color.Black
                                                                      li.Font = New Drawing.Font(li.Font, Drawing.FontStyle.Bold)
                                                                      ' Do not set Selected = True because selection uses system highlight and hides BackColor
                                                                      lvAlerts.EnsureVisible(highlightedMonthIndex)
                                                                  Catch exHighlight As Exception
                                                                      Console.WriteLine("[DEBUG] Errore evidenziazione riga alert: " & exHighlight.ToString())
                                                                  End Try
                                                              End If

                                                              If alerts.Count > 0 Then
                                                                  For Each a In alerts
                                                                      Console.WriteLine("[ALERT] " & a)
                                                                      Try
                                                                          Console.Error.WriteLine("[ALERT] " & a)
                                                                      Catch exErr As Exception
                                                                      End Try
                                                                      Try
                                                                          System.Diagnostics.Debug.WriteLine("[ALERT] " & a)
                                                                      Catch exDbg As Exception
                                                                      End Try
                                                                      Try
                                                                          System.Diagnostics.Trace.WriteLine("[ALERT] " & a)
                                                                      Catch exTr As Exception
                                                                      End Try
                                                                      ' Scrivi log su file
                                                                      Try
                                                                          Dim logFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
                                                                          If Not Directory.Exists(logFolder) Then Directory.CreateDirectory(logFolder)
                                                                          Dim logPath = Path.Combine(logFolder, "monitor_log.txt")
                                                                          Using sw As New StreamWriter(logPath, True)
                                                                              sw.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [ALERT] {a}")
                                                                          End Using
                                                                      Catch exLog As Exception
                                                                          Console.WriteLine("[DEBUG] Errore scrittura log monitor: " & exLog.ToString())
                                                                      End Try
                                                                  Next
                                                                  ' Mostra notifica balloon for the alert that was highlighted (largest delta)
                                                                  Try
                                                                      If notify IsNot Nothing Then
                                                                          notify.BalloonTipTitle = "Monitor Andamenti - Alert"
                                                                          Dim selectedAlert As String = alerts(0)
                                                                          If highlightedMonthIndex >= 0 Then
                                                                              Dim idxInAlerts = alertMonths.IndexOf(highlightedMonthIndex)
                                                                              If idxInAlerts >= 0 AndAlso idxInAlerts < alerts.Count Then
                                                                                  selectedAlert = alerts(idxInAlerts)
                                                                              End If
                                                                          End If
                                                                          notify.BalloonTipText = selectedAlert
                                                                          notify.ShowBalloonTip(5000)
                                                                      End If
                                                                  Catch exNotify As Exception
                                                                      Console.WriteLine("[DEBUG] Errore notifica: " & exNotify.ToString())
                                                                  End Try
                                                                  lblStatus.Text = "Alert: " & alerts.Count & " mesi"
                                                              Else
                                                                  lblStatus.Text = "Nessun alert"
                                                              End If

                                                              Dim prevLabel As String
                                                              If prevYears.Count = 0 Then
                                                                  prevLabel = "Baseline"
                                                              ElseIf prevYears.Count = 1 Then
                                                                  prevLabel = prevYears(0).ToString()
                                                              Else
                                                                  prevLabel = prevYears.Min().ToString() & "-" & prevYears.Max().ToString()
                                                              End If

                                                              If showAllYears Then
                                                                  UpdateChartMultiYear(data, currentYearFromData)
                                                              Else
                                                                  UpdateChart(prevAvg, currArr, prevLabel, currentYearFromData)
                                                              End If
                                                          End Sub))
                     Catch ex As Exception
                         ' Log exception and show message safely
                         Console.WriteLine("[ERROR] RunCheckAsync exception: " & ex.ToString())
                         Me.BeginInvoke(New MethodInvoker(Sub()
                                                              Try
                                                                  ' Show a user-friendly message
                                                                  MessageBox.Show("Errore durante il controllo: " & ex.Message)

                                                                  ' Also write full exception details to console and to the monitor log file
                                                                  Try
                                                                      Console.WriteLine("[ERROR] RunCheckAsync (UI) exception: " & ex.ToString())
                                                                  Catch consEx As Exception
                                                                      ' Swallow console logging errors
                                                                  End Try

                                                                  Try
                                                                      Dim logFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
                                                                      If Not Directory.Exists(logFolder) Then Directory.CreateDirectory(logFolder)
                                                                      Dim logPath = Path.Combine(logFolder, "monitor_log.txt")
                                                                      Using sw As New StreamWriter(logPath, True)
                                                                          sw.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [ERROR] RunCheckAsync: {ex.ToString()}")
                                                                      End Using
                                                                  Catch fileEx As Exception
                                                                      Console.WriteLine("[DEBUG] Errore scrittura log monitor (UI): " & fileEx.ToString())
                                                                  End Try

                                                              Catch innerEx As Exception
                                                                  ' If MessageBox fails on thread, log to textbox
                                                                  Console.WriteLine("[ERROR] Failed to show MessageBox: " & innerEx.ToString())
                                                              End Try
                                                              lblStatus.Text = "Errore"
                                                          End Sub))
                     End Try
                 End Sub)
    End Sub

    Private Function GetMonthlyTotals(field As String, key As String, year As Integer) As Double()
        Dim totals(11) As Double
        For i As Integer = 0 To 11
            totals(i) = 0
        Next

        Try
            Dim databasePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
            Dim connString As String = "Data Source=" & databasePath

            Using conn As New Microsoft.Data.Sqlite.SqliteConnection(connString)
                conn.Open()
                Using cmd As New Microsoft.Data.Sqlite.SqliteCommand($"SELECT strftime('%m', Data) AS MM, SUM(Adebito) AS Tot FROM Movimenti WHERE " & field & " = @key AND strftime('%Y', Data) = @year GROUP BY MM", conn)
                    cmd.Parameters.AddWithValue("@key", key)
                    cmd.Parameters.AddWithValue("@year", year.ToString())

                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim mm = reader("MM").ToString()
                            Dim idx = Convert.ToInt32(mm) - 1
                            Dim val As Double = 0
                            If Not IsDBNull(reader("Tot")) Then Double.TryParse(reader("Tot").ToString(), val)
                            If idx >= 0 AndAlso idx < 12 Then totals(idx) = val
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore GetMonthlyTotals: " & ex.ToString())
            Try
                Dim logFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")
                If Not Directory.Exists(logFolder) Then Directory.CreateDirectory(logFolder)
                Dim logPath = Path.Combine(logFolder, "monitor_log.txt")
                Using sw As New StreamWriter(logPath, True)
                    sw.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [ERROR] GetMonthlyTotals: {ex.ToString()}")
                End Using
            Catch logEx As Exception
                Console.WriteLine("[DEBUG] Errore scrittura log monitor (GetMonthlyTotals): " & logEx.ToString())
            End Try
        End Try

        Return totals
    End Function

    ' Returns a sorted list of integer years available for the given field/key
    Private Function GetAvailableYears(field As String, key As String) As List(Of Integer)
        Dim years As New List(Of Integer)
        Try
            Dim databasePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
            Dim connString As String = "Data Source=" & databasePath
            Using conn As New Microsoft.Data.Sqlite.SqliteConnection(connString)
                conn.Open()
                Using cmd As New Microsoft.Data.Sqlite.SqliteCommand($"SELECT DISTINCT strftime('%Y', Data) AS Y FROM Movimenti WHERE " & field & " = @key ORDER BY Y", conn)
                    cmd.Parameters.AddWithValue("@key", key)
                    Using reader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim y = Convert.ToInt32(reader("Y").ToString())
                            years.Add(y)
                        End While
                    End Using
                End Using
            End Using
        Catch ex As Exception
            Console.WriteLine("[DEBUG] Errore GetAvailableYears: " & ex.ToString())
        End Try

        years.Sort()
        Return years
    End Function

    Private Sub UpdateChart(prev() As Double, curr() As Double, prevYearLabel As String, currYear As Integer)
        chartMonitor.Series.Clear()
        chartMonitor.Annotations.Clear()
        Dim sPrev As Series = chartMonitor.Series.Add(prevYearLabel)
        sPrev.ChartType = SeriesChartType.Column
        sPrev.Color = Drawing.Color.LightBlue
        sPrev.IsValueShownAsLabel = False
        Dim sCurr As Series = chartMonitor.Series.Add(currYear.ToString())
        sCurr.ChartType = SeriesChartType.Column
        sCurr.Color = Drawing.Color.Coral
        sCurr.IsValueShownAsLabel = False

        Dim maxVal As Double = 0
        Dim totalPrev As Double = 0
        Dim totalCurr As Double = 0
        For i As Integer = 0 To 11
            Dim mn As String = MonthName(i + 1)
            sPrev.Points.AddXY(mn, prev(i))
            sCurr.Points.AddXY(mn, curr(i))
            totalPrev += prev(i)
            totalCurr += curr(i)
            If Math.Abs(prev(i)) > maxVal Then maxVal = Math.Abs(prev(i))
            If Math.Abs(curr(i)) > maxVal Then maxVal = Math.Abs(curr(i))
        Next
        sPrev.LegendText = prevYearLabel
        sCurr.LegendText = currYear.ToString()
        ' Ensure no point labels are shown
        For Each pt In sPrev.Points
            pt.Label = String.Empty
        Next
        For Each pt In sCurr.Points
            pt.Label = String.Empty
        Next

        If maxVal <= 0 Then maxVal = 1
        Dim yMax As Double = Math.Ceiling(maxVal * 1.1)
        Dim yInterval As Double = Math.Max(1, Math.Round(yMax / 10))

        With chartMonitor.ChartAreas(0)
            .AxisX.Interval = 1
            .AxisX.Title = "Mese"
            .AxisY.Title = "Totale"
            .AxisY.Minimum = 0
            .AxisY.Maximum = yMax
            .AxisY.Interval = yInterval
            .AxisY.LabelStyle.Format = "C0"
        End With
    End Sub
End Class
