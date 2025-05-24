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
        CType(DataGridView1, ComponentModel.ISupportInitialize).BeginInit()
        Panel1.SuspendLayout()
        SuspendLayout()
        ' 
        ' txtAdebito
        ' 
        txtAdebito.Font = New Font("Segoe UI", 13.8F)
        txtAdebito.Location = New Point(412, 114)
        txtAdebito.Name = "txtAdebito"
        txtAdebito.Size = New Size(125, 38)
        txtAdebito.TabIndex = 4
        ' 
        ' txtAcredito
        ' 
        txtAcredito.Font = New Font("Segoe UI", 13.8F)
        txtAcredito.Location = New Point(412, 168)
        txtAcredito.Name = "txtAcredito"
        txtAcredito.Size = New Size(125, 38)
        txtAcredito.TabIndex = 5
        ' 
        ' btnSave
        ' 
        btnSave.Font = New Font("Segoe UI", 10.8F)
        btnSave.Location = New Point(558, 114)
        btnSave.Name = "btnSave"
        btnSave.Size = New Size(104, 92)
        btnSave.TabIndex = 6
        btnSave.Text = "Salva"
        btnSave.UseVisualStyleBackColor = True
        ' 
        ' DataGridView1
        ' 
        DataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        DataGridView1.Dock = DockStyle.Bottom
        DataGridView1.Location = New Point(0, 346)
        DataGridView1.Name = "DataGridView1"
        DataGridView1.RowHeadersWidth = 51
        DataGridView1.Size = New Size(1260, 272)
        DataGridView1.TabIndex = 7
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(320, 37)
        Label1.Name = "Label1"
        Label1.Size = New Size(49, 20)
        Label1.TabIndex = 7
        Label1.Text = "Conto"
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Location = New Point(320, 132)
        Label3.Name = "Label3"
        Label3.Size = New Size(67, 20)
        Label3.TabIndex = 9
        Label3.Text = "A debito"
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.Location = New Point(320, 183)
        Label4.Name = "Label4"
        Label4.Size = New Size(70, 20)
        Label4.TabIndex = 10
        Label4.Text = "A credito"
        ' 
        ' Label5
        ' 
        Label5.AutoSize = True
        Label5.Location = New Point(320, 83)
        Label5.Name = "Label5"
        Label5.Size = New Size(86, 20)
        Label5.TabIndex = 11
        Label5.Text = "Descrizione"
        ' 
        ' cmbConto
        ' 
        cmbConto.Font = New Font("Segoe UI", 10.8F)
        cmbConto.FormattingEnabled = True
        cmbConto.Location = New Point(412, 24)
        cmbConto.Name = "cmbConto"
        cmbConto.Size = New Size(250, 33)
        cmbConto.TabIndex = 2
        ' 
        ' cmbDescrizione
        ' 
        cmbDescrizione.Font = New Font("Segoe UI", 10.8F)
        cmbDescrizione.FormattingEnabled = True
        cmbDescrizione.Location = New Point(412, 66)
        cmbDescrizione.Name = "cmbDescrizione"
        cmbDescrizione.Size = New Size(250, 33)
        cmbDescrizione.TabIndex = 3
        ' 
        ' cmbContoType
        ' 
        cmbContoType.FormattingEnabled = True
        cmbContoType.Location = New Point(696, 59)
        cmbContoType.Name = "cmbContoType"
        cmbContoType.Size = New Size(198, 28)
        cmbContoType.TabIndex = 15
        ' 
        ' dtpStartDate
        ' 
        dtpStartDate.Location = New Point(696, 93)
        dtpStartDate.Name = "dtpStartDate"
        dtpStartDate.Size = New Size(250, 27)
        dtpStartDate.TabIndex = 16
        ' 
        ' dtpEndDate
        ' 
        dtpEndDate.Location = New Point(696, 126)
        dtpEndDate.Name = "dtpEndDate"
        dtpEndDate.Size = New Size(250, 27)
        dtpEndDate.TabIndex = 17
        ' 
        ' btnCalculate
        ' 
        btnCalculate.Location = New Point(696, 162)
        btnCalculate.Name = "btnCalculate"
        btnCalculate.Size = New Size(70, 41)
        btnCalculate.TabIndex = 18
        btnCalculate.Text = "Cerca"
        btnCalculate.UseVisualStyleBackColor = True
        ' 
        ' lblSaldo
        ' 
        lblSaldo.AutoSize = True
        lblSaldo.Location = New Point(700, 238)
        lblSaldo.Name = "lblSaldo"
        lblSaldo.Size = New Size(53, 20)
        lblSaldo.TabIndex = 19
        lblSaldo.Text = "Label6"
        ' 
        ' lblPatrimonio
        ' 
        lblPatrimonio.AutoSize = True
        lblPatrimonio.Location = New Point(700, 269)
        lblPatrimonio.Name = "lblPatrimonio"
        lblPatrimonio.Size = New Size(53, 20)
        lblPatrimonio.TabIndex = 20
        lblPatrimonio.Text = "Label6"
        ' 
        ' Label6
        ' 
        Label6.AutoSize = True
        Label6.Location = New Point(696, 24)
        Label6.Name = "Label6"
        Label6.Size = New Size(57, 20)
        Label6.TabIndex = 21
        Label6.Text = "Ricerca"
        ' 
        ' btnShowAll
        ' 
        btnShowAll.Location = New Point(157, 238)
        btnShowAll.Name = "btnShowAll"
        btnShowAll.Size = New Size(130, 41)
        btnShowAll.TabIndex = 22
        btnShowAll.Text = "Mostra tutto"
        btnShowAll.UseVisualStyleBackColor = True
        ' 
        ' btnbackup
        ' 
        btnbackup.Location = New Point(1133, 122)
        btnbackup.Name = "btnbackup"
        btnbackup.Size = New Size(115, 29)
        btnbackup.TabIndex = 23
        btnbackup.Text = "Manutenzione"
        btnbackup.UseVisualStyleBackColor = True
        ' 
        ' btnOpenForm3
        ' 
        btnOpenForm3.Font = New Font("Segoe UI", 13.8F)
        btnOpenForm3.Location = New Point(1051, 51)
        btnOpenForm3.Name = "btnOpenForm3"
        btnOpenForm3.Size = New Size(197, 59)
        btnOpenForm3.TabIndex = 24
        btnOpenForm3.Text = "Documenti"
        btnOpenForm3.UseVisualStyleBackColor = True
        ' 
        ' MonthCalendar1
        ' 
        MonthCalendar1.Location = New Point(18, 24)
        MonthCalendar1.Name = "MonthCalendar1"
        MonthCalendar1.TabIndex = 25
        ' 
        ' btnResetSearch
        ' 
        btnResetSearch.Location = New Point(18, 238)
        btnResetSearch.Name = "btnResetSearch"
        btnResetSearch.Size = New Size(133, 41)
        btnResetSearch.TabIndex = 26
        btnResetSearch.Text = "Reset"
        btnResetSearch.UseVisualStyleBackColor = True
        ' 
        ' OpenChart
        ' 
        OpenChart.Location = New Point(848, 162)
        OpenChart.Name = "OpenChart"
        OpenChart.Size = New Size(69, 41)
        OpenChart.TabIndex = 27
        OpenChart.Text = "Grafici"
        OpenChart.UseVisualStyleBackColor = True
        ' 
        ' bntInfo
        ' 
        bntInfo.Location = New Point(1154, 157)
        bntInfo.Name = "bntInfo"
        bntInfo.Size = New Size(94, 29)
        bntInfo.TabIndex = 28
        bntInfo.Text = "Release"
        bntInfo.UseVisualStyleBackColor = True
        ' 
        ' RichTextBoxNota
        ' 
        RichTextBoxNota.Location = New Point(412, 221)
        RichTextBoxNota.Name = "RichTextBoxNota"
        RichTextBoxNota.Size = New Size(250, 82)
        RichTextBoxNota.TabIndex = 29
        RichTextBoxNota.Text = ""
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(320, 238)
        Label2.Name = "Label2"
        Label2.Size = New Size(42, 20)
        Label2.TabIndex = 30
        Label2.Text = "Note"
        ' 
        ' ProgressBar1
        ' 
        ProgressBar1.Location = New Point(9, 17)
        ProgressBar1.Name = "ProgressBar1"
        ProgressBar1.Size = New Size(436, 43)
        ProgressBar1.TabIndex = 31
        ProgressBar1.Visible = False
        ' 
        ' lblDownloadStatus
        ' 
        lblDownloadStatus.AutoSize = True
        lblDownloadStatus.Location = New Point(82, 63)
        lblDownloadStatus.Name = "lblDownloadStatus"
        lblDownloadStatus.Size = New Size(53, 20)
        lblDownloadStatus.TabIndex = 32
        lblDownloadStatus.Text = "Label7"
        lblDownloadStatus.Visible = False
        ' 
        ' Panel1
        ' 
        Panel1.Controls.Add(ProgressBar1)
        Panel1.Controls.Add(lblDownloadStatus)
        Panel1.Location = New Point(438, 380)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(456, 98)
        Panel1.TabIndex = 33
        Panel1.Visible = False
        ' 
        ' btnExport
        ' 
        btnExport.Location = New Point(772, 162)
        btnExport.Name = "btnExport"
        btnExport.Size = New Size(70, 41)
        btnExport.TabIndex = 34
        btnExport.Text = "Esporta"
        btnExport.UseVisualStyleBackColor = True
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1260, 618)
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
        MaximizeBox = False
        Name = "Form1"
        Text = "Amministrazione"
        CType(DataGridView1, ComponentModel.ISupportInitialize).EndInit()
        Panel1.ResumeLayout(False)
        Panel1.PerformLayout()
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
End Class
