<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SetDeadlineForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SetDeadlineForm))
        DateTimePickerDeadline = New DateTimePicker()
        BtnSave = New Button()
        BtnRemoveDeadline = New Button()
        SuspendLayout()
        ' 
        ' DateTimePickerDeadline
        ' 
        DateTimePickerDeadline.CustomFormat = "dd/MM/yyyy HH:mm"
        DateTimePickerDeadline.Format = DateTimePickerFormat.Custom
        DateTimePickerDeadline.Location = New Point(133, 103)
        DateTimePickerDeadline.Name = "DateTimePickerDeadline"
        DateTimePickerDeadline.ShowUpDown = True
        DateTimePickerDeadline.Size = New Size(250, 27)
        DateTimePickerDeadline.TabIndex = 0
        ' 
        ' BtnSave
        ' 
        BtnSave.Location = New Point(289, 188)
        BtnSave.Name = "BtnSave"
        BtnSave.Size = New Size(94, 29)
        BtnSave.TabIndex = 1
        BtnSave.Text = "Save"
        BtnSave.UseVisualStyleBackColor = True
        ' 
        ' BtnRemoveDeadline
        ' 
        BtnRemoveDeadline.Location = New Point(133, 188)
        BtnRemoveDeadline.Name = "BtnRemoveDeadline"
        BtnRemoveDeadline.Size = New Size(94, 29)
        BtnRemoveDeadline.TabIndex = 2
        BtnRemoveDeadline.Text = "Remove"
        BtnRemoveDeadline.UseVisualStyleBackColor = True
        ' 
        ' SetDeadlineForm
        ' 
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(527, 335)
        Controls.Add(BtnRemoveDeadline)
        Controls.Add(BtnSave)
        Controls.Add(DateTimePickerDeadline)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        Name = "SetDeadlineForm"
        Text = "Scadenza"
        ResumeLayout(False)
    End Sub

    Friend WithEvents DateTimePickerDeadline As DateTimePicker
    Friend WithEvents BtnSave As Button
    Friend WithEvents BtnRemoveDeadline As Button
End Class
