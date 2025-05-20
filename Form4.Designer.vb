<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormChart
    Inherits System.Windows.Forms.Form

    'Form esegue l'override del metodo Dispose per pulire l'elenco dei componenti.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Richiesto da Progettazione Windows Form
    Private components As System.ComponentModel.IContainer

    'NOTA: la procedura che segue è richiesta da Progettazione Windows Form
    'Può essere modificata in Progettazione Windows Form.  
    'Non modificarla mediante l'editor del codice.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim ChartArea1 As System.Windows.Forms.DataVisualization.Charting.ChartArea = New DataVisualization.Charting.ChartArea()
        Dim Legend1 As System.Windows.Forms.DataVisualization.Charting.Legend = New DataVisualization.Charting.Legend()
        Dim Series1 As System.Windows.Forms.DataVisualization.Charting.Series = New DataVisualization.Charting.Series()
        Dim ChartArea2 As System.Windows.Forms.DataVisualization.Charting.ChartArea = New DataVisualization.Charting.ChartArea()
        Dim Legend2 As System.Windows.Forms.DataVisualization.Charting.Legend = New DataVisualization.Charting.Legend()
        Dim Series2 As System.Windows.Forms.DataVisualization.Charting.Series = New DataVisualization.Charting.Series()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormChart))
        btnCalculate = New Button()
        Chart1 = New DataVisualization.Charting.Chart()
        lblPatrimonio = New Label()
        lblSaldo = New Label()
        lstConto = New ListBox()
        MonthCalendar1 = New MonthCalendar()
        lstConto2 = New ListBox()
        lblTotalSpese = New Label()
        DataGridView1 = New DataGridView()
        Chart3 = New DataVisualization.Charting.Chart()
        DataGridView2 = New DataGridView()
        cmbAnno = New ComboBox()
        CType(Chart1, ComponentModel.ISupportInitialize).BeginInit()
        CType(DataGridView1, ComponentModel.ISupportInitialize).BeginInit()
        CType(Chart3, ComponentModel.ISupportInitialize).BeginInit()
        CType(DataGridView2, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' btnCalculate
        ' 
        btnCalculate.Location = New Point(178, 231)
        btnCalculate.Name = "btnCalculate"
        btnCalculate.Size = New Size(103, 80)
        btnCalculate.TabIndex = 30
        btnCalculate.Text = "Cerca"
        btnCalculate.UseVisualStyleBackColor = True
        ' 
        ' Chart1
        ' 
        ChartArea1.Name = "ChartArea1"
        Chart1.ChartAreas.Add(ChartArea1)
        Legend1.Name = "Legend1"
        Chart1.Legends.Add(Legend1)
        Chart1.Location = New Point(481, 12)
        Chart1.Name = "Chart1"
        Series1.ChartArea = "ChartArea1"
        Series1.Legend = "Legend1"
        Series1.Name = "Series1"
        Chart1.Series.Add(Series1)
        Chart1.Size = New Size(630, 385)
        Chart1.TabIndex = 31
        Chart1.Text = "Chart1"
        ' 
        ' lblPatrimonio
        ' 
        lblPatrimonio.AutoSize = True
        lblPatrimonio.Location = New Point(178, 357)
        lblPatrimonio.Name = "lblPatrimonio"
        lblPatrimonio.Size = New Size(53, 20)
        lblPatrimonio.TabIndex = 33
        lblPatrimonio.Text = "Label6"
        ' 
        ' lblSaldo
        ' 
        lblSaldo.AutoSize = True
        lblSaldo.Location = New Point(178, 377)
        lblSaldo.Name = "lblSaldo"
        lblSaldo.Size = New Size(53, 20)
        lblSaldo.TabIndex = 32
        lblSaldo.Text = "Label6"
        ' 
        ' lstConto
        ' 
        lstConto.FormattingEnabled = True
        lstConto.Location = New Point(12, 232)
        lstConto.Name = "lstConto"
        lstConto.Size = New Size(154, 544)
        lstConto.TabIndex = 34
        ' 
        ' MonthCalendar1
        ' 
        MonthCalendar1.Location = New Point(12, 12)
        MonthCalendar1.MaxSelectionCount = 365
        MonthCalendar1.Name = "MonthCalendar1"
        MonthCalendar1.TabIndex = 35
        ' 
        ' lstConto2
        ' 
        lstConto2.FormattingEnabled = True
        lstConto2.Location = New Point(1117, 12)
        lstConto2.Name = "lstConto2"
        lstConto2.Size = New Size(712, 384)
        lstConto2.TabIndex = 37
        ' 
        ' lblTotalSpese
        ' 
        lblTotalSpese.AutoSize = True
        lblTotalSpese.Location = New Point(178, 337)
        lblTotalSpese.Name = "lblTotalSpese"
        lblTotalSpese.Size = New Size(130, 20)
        lblTotalSpese.TabIndex = 38
        lblTotalSpese.Text = "Totale selezionato"
        ' 
        ' DataGridView1
        ' 
        DataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridView1.Location = New Point(1329, 411)
        DataGridView1.Name = "DataGridView1"
        DataGridView1.RowHeadersWidth = 51
        DataGridView1.Size = New Size(500, 365)
        DataGridView1.TabIndex = 39
        ' 
        ' Chart3
        ' 
        ChartArea2.Name = "ChartArea1"
        Chart3.ChartAreas.Add(ChartArea2)
        Legend2.Name = "Legend1"
        Chart3.Legends.Add(Legend2)
        Chart3.Location = New Point(693, 411)
        Chart3.Name = "Chart3"
        Series2.ChartArea = "ChartArea1"
        Series2.Legend = "Legend1"
        Series2.Name = "Series1"
        Chart3.Series.Add(Series2)
        Chart3.Size = New Size(630, 365)
        Chart3.TabIndex = 40
        Chart3.Text = "Chart3"
        ' 
        ' DataGridView2
        ' 
        DataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridView2.Location = New Point(172, 411)
        DataGridView2.Name = "DataGridView2"
        DataGridView2.RowHeadersWidth = 51
        DataGridView2.Size = New Size(515, 365)
        DataGridView2.TabIndex = 41
        ' 
        ' cmbAnno
        ' 
        cmbAnno.FormattingEnabled = True
        cmbAnno.Location = New Point(704, 738)
        cmbAnno.Name = "cmbAnno"
        cmbAnno.Size = New Size(105, 28)
        cmbAnno.TabIndex = 42
        ' 
        ' FormChart
        ' 
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1841, 798)
        Controls.Add(cmbAnno)
        Controls.Add(DataGridView2)
        Controls.Add(Chart3)
        Controls.Add(DataGridView1)
        Controls.Add(lblTotalSpese)
        Controls.Add(lstConto2)
        Controls.Add(MonthCalendar1)
        Controls.Add(lstConto)
        Controls.Add(lblPatrimonio)
        Controls.Add(lblSaldo)
        Controls.Add(Chart1)
        Controls.Add(btnCalculate)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        Name = "FormChart"
        Text = "Ricerca"
        CType(Chart1, ComponentModel.ISupportInitialize).EndInit()
        CType(DataGridView1, ComponentModel.ISupportInitialize).EndInit()
        CType(Chart3, ComponentModel.ISupportInitialize).EndInit()
        CType(DataGridView2, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents btnCalculate As Button
    Friend WithEvents Chart1 As DataVisualization.Charting.Chart
    Friend WithEvents lblPatrimonio As Label
    Friend WithEvents lblSaldo As Label
    Friend WithEvents lstConto As ListBox
    Friend WithEvents MonthCalendar1 As MonthCalendar
    Friend WithEvents lstConto2 As ListBox
    Friend WithEvents lblTotalSpese As Label
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents Chart3 As DataVisualization.Charting.Chart
    Friend WithEvents DataGridView2 As DataGridView
    Friend WithEvents cmbAnno As ComboBox
End Class
