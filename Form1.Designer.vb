<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim ChartArea1 As System.Windows.Forms.DataVisualization.Charting.ChartArea = New DataVisualization.Charting.ChartArea()
        Dim Legend1 As System.Windows.Forms.DataVisualization.Charting.Legend = New DataVisualization.Charting.Legend()
        Dim Series1 As System.Windows.Forms.DataVisualization.Charting.Series = New DataVisualization.Charting.Series()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        txtAdebito = New TextBox()
        txtAcredito = New TextBox()
        btnSave = New Button()
        DataGridView1 = New DataGridView()
        Label1 = New Label()
        Label3 = New Label()
        Label4 = New Label()
        Label5 = New Label()
        cmbConto = New ComboBox()
        cmbDescrizione = New ComboBox()
        cmbContoType = New ComboBox()
        dtpStartDate = New DateTimePicker()
        dtpEndDate = New DateTimePicker()
        btnCalculate = New Button()
        lblSaldo = New Label()
        lblPatrimonio = New Label()
        Label6 = New Label()
        btnShowAll = New Button()
        btnbackup = New Button()
        btnOpenForm3 = New Button()
        MonthCalendar1 = New MonthCalendar()
        btnResetSearch = New Button()
        OpenChart = New Button()
        bntInfo = New Button()
        RichTextBoxNota = New RichTextBox()
        Label2 = New Label()
        ProgressBar1 = New ProgressBar()
        lblDownloadStatus = New Label()
        Panel1 = New Panel()
        btnExport = New Button()
        SqliteCommand1 = New Microsoft.Data.Sqlite.SqliteCommand()
        saldoattuale = New DataGridView()
        Chart1 = New DataVisualization.Charting.Chart()
        CType(DataGridView1, ComponentModel.ISupportInitialize).BeginInit()
        Panel1.SuspendLayout()
        CType(saldoattuale, ComponentModel.ISupportInitialize).BeginInit()
        CType(Chart1, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' txtAdebito
        ' 
        txtAdebito.Font = New Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        txtAdebito.Location = New Point(388, 102)
        txtAdebito.Margin = New Padding(3, 2, 3, 2)
        txtAdebito.Name = "txtAdebito"
        txtAdebito.Size = New Size(110, 33)
        txtAdebito.TabIndex = 4
        ' 
        ' txtAcredito
        ' 
        txtAcredito.Font = New Font("Segoe UI", 13.8F)
        txtAcredito.Location = New Point(388, 142)
        txtAcredito.Margin = New Padding(3, 2, 3, 2)
        txtAcredito.Name = "txtAcredito"
        txtAcredito.Size = New Size(110, 32)
        txtAcredito.TabIndex = 5
        ' 
        ' btnSave
        ' 
        btnSave.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        btnSave.Location = New Point(675, 34)
        btnSave.Margin = New Padding(3, 2, 3, 2)
        btnSave.Name = "btnSave"
        btnSave.Size = New Size(145, 61)
        btnSave.TabIndex = 6
        btnSave.Text = "Salva"
        btnSave.UseVisualStyleBackColor = True
        ' 
        ' DataGridView1
        ' 
        DataGridView1.Anchor = AnchorStyles.None
        DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        DataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridView1.Location = New Point(16, 229)
        DataGridView1.Margin = New Padding(3, 2, 3, 2)
        DataGridView1.Name = "DataGridView1"
        DataGridView1.RowHeadersWidth = 51
        DataGridView1.Size = New Size(1153, 397)
        DataGridView1.TabIndex = 7
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Font = New Font("Segoe UI", 12F)
        Label1.Location = New Point(283, 36)
        Label1.Name = "Label1"
        Label1.Size = New Size(52, 21)
        Label1.TabIndex = 7
        Label1.Text = "Conto"
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Font = New Font("Segoe UI", 12F)
        Label3.Location = New Point(283, 107)
        Label3.Name = "Label3"
        Label3.Size = New Size(68, 21)
        Label3.TabIndex = 9
        Label3.Text = "A debito"
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.Font = New Font("Segoe UI", 12F)
        Label4.Location = New Point(283, 145)
        Label4.Name = "Label4"
        Label4.Size = New Size(72, 21)
        Label4.TabIndex = 10
        Label4.Text = "A credito"
        ' 
        ' Label5
        ' 
        Label5.AutoSize = True
        Label5.Font = New Font("Segoe UI", 12F)
        Label5.Location = New Point(283, 70)
        Label5.Name = "Label5"
        Label5.Size = New Size(90, 21)
        Label5.TabIndex = 11
        Label5.Text = "Descrizione"
        ' 
        ' cmbConto
        ' 
        cmbConto.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        cmbConto.FormattingEnabled = True
        cmbConto.Location = New Point(388, 34)
        cmbConto.Margin = New Padding(3, 2, 3, 2)
        cmbConto.Name = "cmbConto"
        cmbConto.Size = New Size(281, 29)
        cmbConto.TabIndex = 2
        ' 
        ' cmbDescrizione
        ' 
        cmbDescrizione.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        cmbDescrizione.FormattingEnabled = True
        cmbDescrizione.Location = New Point(388, 66)
        cmbDescrizione.Margin = New Padding(3, 2, 3, 2)
        cmbDescrizione.Name = "cmbDescrizione"
        cmbDescrizione.Size = New Size(281, 29)
        cmbDescrizione.TabIndex = 3
        ' 
        ' cmbContoType
        ' 
        cmbContoType.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        cmbContoType.FormattingEnabled = True
        cmbContoType.Location = New Point(876, 35)
        cmbContoType.Margin = New Padding(3, 2, 3, 2)
        cmbContoType.Name = "cmbContoType"
        cmbContoType.Size = New Size(174, 29)
        cmbContoType.TabIndex = 15
        ' 
        ' dtpStartDate
        ' 
        dtpStartDate.Location = New Point(876, 69)
        dtpStartDate.Margin = New Padding(3, 2, 3, 2)
        dtpStartDate.Name = "dtpStartDate"
        dtpStartDate.Size = New Size(219, 23)
        dtpStartDate.TabIndex = 16
        ' 
        ' dtpEndDate
        ' 
        dtpEndDate.Location = New Point(876, 97)
        dtpEndDate.Margin = New Padding(3, 2, 3, 2)
        dtpEndDate.Name = "dtpEndDate"
        dtpEndDate.Size = New Size(219, 23)
        dtpEndDate.TabIndex = 17
        ' 
        ' btnCalculate
        ' 
        btnCalculate.Font = New Font("Segoe UI", 12F)
        btnCalculate.Location = New Point(876, 130)
        btnCalculate.Margin = New Padding(3, 2, 3, 2)
        btnCalculate.Name = "btnCalculate"
        btnCalculate.Size = New Size(61, 44)
        btnCalculate.TabIndex = 18
        btnCalculate.Text = "Cerca"
        btnCalculate.UseVisualStyleBackColor = True
        ' 
        ' lblSaldo
        ' 
        lblSaldo.AutoSize = True
        lblSaldo.Font = New Font("Segoe UI", 12F)
        lblSaldo.Location = New Point(613, 193)
        lblSaldo.Name = "lblSaldo"
        lblSaldo.Size = New Size(56, 21)
        lblSaldo.TabIndex = 19
        lblSaldo.Text = "Label6"
        ' 
        ' lblPatrimonio
        ' 
        lblPatrimonio.AutoSize = True
        lblPatrimonio.Font = New Font("Segoe UI", 12F)
        lblPatrimonio.Location = New Point(876, 193)
        lblPatrimonio.Name = "lblPatrimonio"
        lblPatrimonio.Size = New Size(56, 21)
        lblPatrimonio.TabIndex = 20
        lblPatrimonio.Text = "Label6"
        ' 
        ' Label6
        ' 
        Label6.AutoSize = True
        Label6.Font = New Font("Segoe UI", 12F)
        Label6.Location = New Point(876, 12)
        Label6.Name = "Label6"
        Label6.Size = New Size(60, 21)
        Label6.TabIndex = 21
        Label6.Text = "Ricerca"
        ' 
        ' btnShowAll
        ' 
        btnShowAll.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        btnShowAll.Location = New Point(137, 189)
        btnShowAll.Margin = New Padding(3, 2, 3, 2)
        btnShowAll.Name = "btnShowAll"
        btnShowAll.Size = New Size(114, 29)
        btnShowAll.TabIndex = 22
        btnShowAll.Text = "Mostra tutto"
        btnShowAll.UseVisualStyleBackColor = True
        ' 
        ' btnbackup
        ' 
        btnbackup.Location = New Point(1334, 12)
        btnbackup.Margin = New Padding(3, 2, 3, 2)
        btnbackup.Name = "btnbackup"
        btnbackup.Size = New Size(101, 43)
        btnbackup.TabIndex = 23
        btnbackup.Text = "Manutenzione"
        btnbackup.UseVisualStyleBackColor = True
        ' 
        ' btnOpenForm3
        ' 
        btnOpenForm3.Font = New Font("Segoe UI", 13.8F)
        btnOpenForm3.Location = New Point(1177, 11)
        btnOpenForm3.Margin = New Padding(3, 2, 3, 2)
        btnOpenForm3.Name = "btnOpenForm3"
        btnOpenForm3.Size = New Size(151, 44)
        btnOpenForm3.TabIndex = 24
        btnOpenForm3.Text = "Documenti"
        btnOpenForm3.UseVisualStyleBackColor = True
        ' 
        ' MonthCalendar1
        ' 
        MonthCalendar1.Location = New Point(16, 18)
        MonthCalendar1.Margin = New Padding(8, 7, 8, 7)
        MonthCalendar1.Name = "MonthCalendar1"
        MonthCalendar1.TabIndex = 25
        ' 
        ' btnResetSearch
        ' 
        btnResetSearch.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        btnResetSearch.Location = New Point(16, 189)
        btnResetSearch.Margin = New Padding(3, 2, 3, 2)
        btnResetSearch.Name = "btnResetSearch"
        btnResetSearch.Size = New Size(116, 29)
        btnResetSearch.TabIndex = 26
        btnResetSearch.Text = "Reset"
        btnResetSearch.UseVisualStyleBackColor = True
        ' 
        ' OpenChart
        ' 
        OpenChart.Font = New Font("Segoe UI", 12F)
        OpenChart.Location = New Point(1029, 130)
        OpenChart.Margin = New Padding(3, 2, 3, 2)
        OpenChart.Name = "OpenChart"
        OpenChart.Size = New Size(66, 44)
        OpenChart.TabIndex = 27
        OpenChart.Text = "Grafici"
        OpenChart.UseVisualStyleBackColor = True
        ' 
        ' bntInfo
        ' 
        bntInfo.Location = New Point(1441, 12)
        bntInfo.Margin = New Padding(3, 2, 3, 2)
        bntInfo.Name = "bntInfo"
        bntInfo.Size = New Size(82, 43)
        bntInfo.TabIndex = 28
        bntInfo.Text = "Release"
        bntInfo.UseVisualStyleBackColor = True
        ' 
        ' RichTextBoxNota
        ' 
        RichTextBoxNota.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        RichTextBoxNota.Location = New Point(504, 104)
        RichTextBoxNota.Margin = New Padding(3, 2, 3, 2)
        RichTextBoxNota.Name = "RichTextBoxNota"
        RichTextBoxNota.Size = New Size(165, 70)
        RichTextBoxNota.TabIndex = 29
        RichTextBoxNota.Text = ""
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Font = New Font("Segoe UI", 12F)
        Label2.Location = New Point(675, 102)
        Label2.Name = "Label2"
        Label2.Size = New Size(44, 21)
        Label2.TabIndex = 30
        Label2.Text = "Note"
        ' 
        ' ProgressBar1
        ' 
        ProgressBar1.Location = New Point(27, 38)
        ProgressBar1.Margin = New Padding(3, 2, 3, 2)
        ProgressBar1.Name = "ProgressBar1"
        ProgressBar1.Size = New Size(550, 46)
        ProgressBar1.TabIndex = 31
        ProgressBar1.Visible = False
        ' 
        ' lblDownloadStatus
        ' 
        lblDownloadStatus.AutoSize = True
        lblDownloadStatus.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        lblDownloadStatus.Location = New Point(98, 86)
        lblDownloadStatus.Name = "lblDownloadStatus"
        lblDownloadStatus.Size = New Size(56, 21)
        lblDownloadStatus.TabIndex = 32
        lblDownloadStatus.Text = "Label7"
        lblDownloadStatus.Visible = False
        ' 
        ' Panel1
        ' 
        Panel1.Controls.Add(ProgressBar1)
        Panel1.Controls.Add(lblDownloadStatus)
        Panel1.Location = New Point(285, 392)
        Panel1.Margin = New Padding(3, 2, 3, 2)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(606, 121)
        Panel1.TabIndex = 33
        Panel1.Visible = False
        ' 
        ' btnExport
        ' 
        btnExport.Font = New Font("Segoe UI", 12F)
        btnExport.Location = New Point(943, 130)
        btnExport.Margin = New Padding(3, 2, 3, 2)
        btnExport.Name = "btnExport"
        btnExport.Size = New Size(80, 44)
        btnExport.TabIndex = 34
        btnExport.Text = "Esporta"
        btnExport.UseVisualStyleBackColor = True
        ' 
        ' SqliteCommand1
        ' 
        SqliteCommand1.CommandTimeout = 30
        SqliteCommand1.Connection = Nothing
        SqliteCommand1.Transaction = Nothing
        SqliteCommand1.UpdatedRowSource = UpdateRowSource.None
        ' 
        ' saldoattuale
        ' 
        saldoattuale.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        saldoattuale.Location = New Point(1175, 229)
        saldoattuale.Name = "saldoattuale"
        saldoattuale.Size = New Size(461, 396)
        saldoattuale.TabIndex = 37
        ' 
        ' Chart1
        ' 
        ChartArea1.Name = "ChartArea1"
        Chart1.ChartAreas.Add(ChartArea1)
        Legend1.Name = "Legend1"
        Chart1.Legends.Add(Legend1)
        Chart1.Location = New Point(1177, 60)
        Chart1.Name = "Chart1"
        Series1.ChartArea = "ChartArea1"
        Series1.Legend = "Legend1"
        Series1.Name = "Series1"
        Chart1.Series.Add(Series1)
        Chart1.Size = New Size(459, 163)
        Chart1.TabIndex = 38
        Chart1.Text = "Chart1"
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1648, 637)
        Controls.Add(Chart1)
        Controls.Add(saldoattuale)
        Controls.Add(btnExport)
        Controls.Add(Panel1)
        Controls.Add(Label2)
        Controls.Add(RichTextBoxNota)
        Controls.Add(bntInfo)
        Controls.Add(OpenChart)
        Controls.Add(btnResetSearch)
        Controls.Add(MonthCalendar1)
        Controls.Add(btnOpenForm3)
        Controls.Add(btnbackup)
        Controls.Add(btnShowAll)
        Controls.Add(Label6)
        Controls.Add(lblPatrimonio)
        Controls.Add(lblSaldo)
        Controls.Add(btnCalculate)
        Controls.Add(dtpEndDate)
        Controls.Add(dtpStartDate)
        Controls.Add(cmbContoType)
        Controls.Add(cmbDescrizione)
        Controls.Add(cmbConto)
        Controls.Add(Label5)
        Controls.Add(Label4)
        Controls.Add(Label3)
        Controls.Add(Label1)
        Controls.Add(DataGridView1)
        Controls.Add(btnSave)
        Controls.Add(txtAcredito)
        Controls.Add(txtAdebito)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Margin = New Padding(3, 2, 3, 2)
        MaximizeBox = False
        Name = "Form1"
        Text = "Amministrazione"
        CType(DataGridView1, ComponentModel.ISupportInitialize).EndInit()
        Panel1.ResumeLayout(False)
        Panel1.PerformLayout()
        CType(saldoattuale, ComponentModel.ISupportInitialize).EndInit()
        CType(Chart1, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub
    Friend WithEvents txtAdebito As TextBox
    Friend WithEvents txtAcredito As TextBox
    Friend WithEvents btnSave As Button
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents Label1 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents cmbConto As ComboBox
    Friend WithEvents cmbDescrizione As ComboBox
    Friend WithEvents cmbContoType As ComboBox
    Friend WithEvents dtpStartDate As DateTimePicker
    Friend WithEvents dtpEndDate As DateTimePicker
    Friend WithEvents btnCalculate As Button
    Friend WithEvents lblSaldo As Label
    Friend WithEvents lblPatrimonio As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents btnShowAll As Button
    Friend WithEvents btnbackup As Button
    Friend WithEvents btnOpenForm3 As Button
    Friend WithEvents MonthCalendar1 As MonthCalendar
    Friend WithEvents btnResetSearch As Button
    Friend WithEvents btnShowGraph As Button
    Friend WithEvents OpenChart As Button
    Friend WithEvents bntInfo As Button
    Friend WithEvents RichTextBoxNota As RichTextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents ProgressBar1 As ProgressBar
    Friend WithEvents lblDownloadStatus As Label
    Friend WithEvents Panel1 As Panel
    Friend WithEvents btnExport As Button
    Friend WithEvents SqliteCommand1 As Microsoft.Data.Sqlite.SqliteCommand
    Friend WithEvents saldoattuale As DataGridView
    Friend WithEvents Chart1 As DataVisualization.Charting.Chart
End Class
