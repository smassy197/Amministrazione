<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormNuovoDocumento
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormNuovoDocumento))
        btnSalva = New Button()
        DateTimePickerData = New DateTimePicker()
        TextBoxAnno = New TextBox()
        TextBoxImporto = New TextBox()
        TextBoxNote = New TextBox()
        Label1 = New Label()
        Label2 = New Label()
        Label3 = New Label()
        btnSfogliaFile = New Button()
        txtNomeFile = New TextBox()
        chkScadenzaAttiva = New CheckBox()
        DateTimePickerScadenza = New DateTimePicker()
        Label4 = New Label()
        txtTipoPagamento = New TextBox()
        Label5 = New Label()
        txtUtente = New TextBox()
        chkDaPagare = New CheckBox()
        chkPagato = New CheckBox()
        lblIndicatore = New Label()
        chkModificaFile = New CheckBox()
        btnApriDocumento = New Button()
        panelPreview = New Panel()
        SuspendLayout()
        ' 
        ' btnSalva
        ' 
        btnSalva.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        btnSalva.Location = New Point(619, 57)
        btnSalva.Name = "btnSalva"
        btnSalva.Size = New Size(154, 93)
        btnSalva.TabIndex = 0
        btnSalva.Text = "Salva"
        btnSalva.UseVisualStyleBackColor = True
        ' 
        ' DateTimePickerData
        ' 
        DateTimePickerData.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        DateTimePickerData.Location = New Point(29, 137)
        DateTimePickerData.Name = "DateTimePickerData"
        DateTimePickerData.Size = New Size(348, 34)
        DateTimePickerData.TabIndex = 1
        ' 
        ' TextBoxAnno
        ' 
        TextBoxAnno.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        TextBoxAnno.Location = New Point(29, 202)
        TextBoxAnno.Name = "TextBoxAnno"
        TextBoxAnno.Size = New Size(125, 34)
        TextBoxAnno.TabIndex = 2
        ' 
        ' TextBoxImporto
        ' 
        TextBoxImporto.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        TextBoxImporto.Location = New Point(29, 257)
        TextBoxImporto.Name = "TextBoxImporto"
        TextBoxImporto.Size = New Size(125, 34)
        TextBoxImporto.TabIndex = 3
        ' 
        ' TextBoxNote
        ' 
        TextBoxNote.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        TextBoxNote.Location = New Point(29, 317)
        TextBoxNote.Name = "TextBoxNote"
        TextBoxNote.Size = New Size(635, 34)
        TextBoxNote.TabIndex = 4
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Label1.Location = New Point(187, 202)
        Label1.Name = "Label1"
        Label1.Size = New Size(197, 28)
        Label1.TabIndex = 5
        Label1.Text = "Anno del documento"
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Label2.Location = New Point(187, 256)
        Label2.Name = "Label2"
        Label2.Size = New Size(84, 28)
        Label2.TabIndex = 6
        Label2.Text = "Importo"
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Label3.Location = New Point(678, 323)
        Label3.Name = "Label3"
        Label3.Size = New Size(56, 28)
        Label3.TabIndex = 7
        Label3.Text = "Note"
        ' 
        ' btnSfogliaFile
        ' 
        btnSfogliaFile.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        btnSfogliaFile.Location = New Point(29, 389)
        btnSfogliaFile.Name = "btnSfogliaFile"
        btnSfogliaFile.Size = New Size(94, 37)
        btnSfogliaFile.TabIndex = 8
        btnSfogliaFile.Text = "Sfoglia"
        btnSfogliaFile.UseVisualStyleBackColor = True
        ' 
        ' txtNomeFile
        ' 
        txtNomeFile.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        txtNomeFile.Location = New Point(146, 390)
        txtNomeFile.Name = "txtNomeFile"
        txtNomeFile.Size = New Size(443, 34)
        txtNomeFile.TabIndex = 9
        ' 
        ' chkScadenzaAttiva
        ' 
        chkScadenzaAttiva.AutoSize = True
        chkScadenzaAttiva.Location = New Point(619, 223)
        chkScadenzaAttiva.Name = "chkScadenzaAttiva"
        chkScadenzaAttiva.Size = New Size(150, 24)
        chkScadenzaAttiva.TabIndex = 46
        chkScadenzaAttiva.Text = "Imposta scadenza"
        chkScadenzaAttiva.UseVisualStyleBackColor = True
        ' 
        ' DateTimePickerScadenza
        ' 
        DateTimePickerScadenza.CustomFormat = "dd/MM/yyyy HH:mm"
        DateTimePickerScadenza.Format = DateTimePickerFormat.Custom
        DateTimePickerScadenza.Location = New Point(519, 253)
        DateTimePickerScadenza.Name = "DateTimePickerScadenza"
        DateTimePickerScadenza.ShowUpDown = True
        DateTimePickerScadenza.Size = New Size(250, 27)
        DateTimePickerScadenza.TabIndex = 47
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Label4.Location = New Point(12, 46)
        Label4.Margin = New Padding(4, 0, 4, 0)
        Label4.Name = "Label4"
        Label4.Size = New Size(111, 28)
        Label4.TabIndex = 51
        Label4.Text = "Pagamento"
        ' 
        ' txtTipoPagamento
        ' 
        txtTipoPagamento.Location = New Point(131, 43)
        txtTipoPagamento.Margin = New Padding(4)
        txtTipoPagamento.Name = "txtTipoPagamento"
        txtTipoPagamento.Size = New Size(411, 27)
        txtTipoPagamento.TabIndex = 50
        ' 
        ' Label5
        ' 
        Label5.AutoSize = True
        Label5.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Label5.Location = New Point(12, 11)
        Label5.Margin = New Padding(4, 0, 4, 0)
        Label5.Name = "Label5"
        Label5.Size = New Size(71, 28)
        Label5.TabIndex = 49
        Label5.Text = "Utente"
        ' 
        ' txtUtente
        ' 
        txtUtente.Location = New Point(102, 8)
        txtUtente.Margin = New Padding(4)
        txtUtente.Name = "txtUtente"
        txtUtente.Size = New Size(440, 27)
        txtUtente.TabIndex = 48
        ' 
        ' chkDaPagare
        ' 
        chkDaPagare.AutoSize = True
        chkDaPagare.Location = New Point(619, 165)
        chkDaPagare.Name = "chkDaPagare"
        chkDaPagare.Size = New Size(101, 24)
        chkDaPagare.TabIndex = 52
        chkDaPagare.Text = "Da pagare"
        chkDaPagare.UseVisualStyleBackColor = True
        ' 
        ' chkPagato
        ' 
        chkPagato.AutoSize = True
        chkPagato.Location = New Point(619, 193)
        chkPagato.Name = "chkPagato"
        chkPagato.Size = New Size(77, 24)
        chkPagato.TabIndex = 53
        chkPagato.Text = "Pagato"
        chkPagato.UseVisualStyleBackColor = True
        ' 
        ' lblIndicatore
        ' 
        lblIndicatore.AutoSize = True
        lblIndicatore.Font = New Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        lblIndicatore.Location = New Point(581, 11)
        lblIndicatore.Name = "lblIndicatore"
        lblIndicatore.Size = New Size(74, 28)
        lblIndicatore.TabIndex = 54
        lblIndicatore.Text = "Label6"
        ' 
        ' chkModificaFile
        ' 
        chkModificaFile.AutoSize = True
        chkModificaFile.Location = New Point(595, 399)
        chkModificaFile.Name = "chkModificaFile"
        chkModificaFile.Size = New Size(115, 24)
        chkModificaFile.TabIndex = 55
        chkModificaFile.Text = "Modifica file"
        chkModificaFile.UseVisualStyleBackColor = True
        ' 
        ' btnApriDocumento
        ' 
        btnApriDocumento.Location = New Point(716, 394)
        btnApriDocumento.Name = "btnApriDocumento"
        btnApriDocumento.Size = New Size(57, 29)
        btnApriDocumento.TabIndex = 56
        btnApriDocumento.Text = "Apri"
        btnApriDocumento.UseVisualStyleBackColor = True
        ' 
        ' panelPreview
        ' 
        panelPreview.BorderStyle = BorderStyle.FixedSingle
        panelPreview.Location = New Point(805, 25)
        panelPreview.Name = "panelPreview"
        panelPreview.Size = New Size(362, 398)
        panelPreview.TabIndex = 57
        ' 
        ' FormNuovoDocumento
        ' 
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1191, 450)
        Controls.Add(panelPreview)
        Controls.Add(btnApriDocumento)
        Controls.Add(chkModificaFile)
        Controls.Add(lblIndicatore)
        Controls.Add(chkPagato)
        Controls.Add(chkDaPagare)
        Controls.Add(Label4)
        Controls.Add(txtTipoPagamento)
        Controls.Add(Label5)
        Controls.Add(txtUtente)
        Controls.Add(DateTimePickerScadenza)
        Controls.Add(chkScadenzaAttiva)
        Controls.Add(txtNomeFile)
        Controls.Add(btnSfogliaFile)
        Controls.Add(Label3)
        Controls.Add(Label2)
        Controls.Add(Label1)
        Controls.Add(TextBoxNote)
        Controls.Add(TextBoxImporto)
        Controls.Add(TextBoxAnno)
        Controls.Add(DateTimePickerData)
        Controls.Add(btnSalva)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        Name = "FormNuovoDocumento"
        Text = "FormNuovoDocumento"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents btnSalva As Button
    Friend WithEvents DateTimePickerData As DateTimePicker
    Friend WithEvents TextBoxAnno As TextBox
    Friend WithEvents TextBoxImporto As TextBox
    Friend WithEvents TextBoxNote As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents btnSfogliaFile As Button
    Friend WithEvents txtNomeFile As TextBox
    Friend WithEvents chkScadenzaAttiva As CheckBox
    Friend WithEvents DateTimePickerScadenza As DateTimePicker
    Friend WithEvents Label4 As Label
    Friend WithEvents txtTipoPagamento As TextBox
    Friend WithEvents Label5 As Label
    Friend WithEvents txtUtente As TextBox
    Friend WithEvents chkDaPagare As CheckBox
    Friend WithEvents chkPagato As CheckBox
    Friend WithEvents lblIndicatore As Label
    Friend WithEvents chkModificaFile As CheckBox
    Friend WithEvents btnApriDocumento As Button
    Friend WithEvents panelPreview As Panel
End Class
