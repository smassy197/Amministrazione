<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form6
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form6))
        dataGridViewResults = New DataGridView()
        comboBoxDocType = New ComboBox()
        comboBoxUser = New ComboBox()
        MonthCalendarDate = New MonthCalendar()
        CType(dataGridViewResults, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' dataGridViewResults
        ' 
        dataGridViewResults.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dataGridViewResults.Location = New Point(123, 228)
        dataGridViewResults.Name = "dataGridViewResults"
        dataGridViewResults.RowHeadersWidth = 51
        dataGridViewResults.Size = New Size(1181, 309)
        dataGridViewResults.TabIndex = 0
        ' 
        ' comboBoxDocType
        ' 
        comboBoxDocType.FormattingEnabled = True
        comboBoxDocType.Location = New Point(690, 95)
        comboBoxDocType.Name = "comboBoxDocType"
        comboBoxDocType.Size = New Size(151, 28)
        comboBoxDocType.TabIndex = 2
        ' 
        ' comboBoxUser
        ' 
        comboBoxUser.FormattingEnabled = True
        comboBoxUser.Location = New Point(921, 95)
        comboBoxUser.Name = "comboBoxUser"
        comboBoxUser.Size = New Size(151, 28)
        comboBoxUser.TabIndex = 3
        ' 
        ' MonthCalendarDate
        ' 
        MonthCalendarDate.Location = New Point(272, 9)
        MonthCalendarDate.Name = "MonthCalendarDate"
        MonthCalendarDate.TabIndex = 4
        ' 
        ' Form6
        ' 
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1423, 558)
        Controls.Add(MonthCalendarDate)
        Controls.Add(comboBoxUser)
        Controls.Add(comboBoxDocType)
        Controls.Add(dataGridViewResults)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Name = "Form6"
        Text = "Ricerca"
        CType(dataGridViewResults, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents dataGridViewResults As DataGridView
    Friend WithEvents comboBoxDocType As ComboBox
    Friend WithEvents comboBoxUser As ComboBox
    Friend WithEvents MonthCalendarDate As MonthCalendar
End Class
