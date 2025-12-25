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
        chkScadenzaAttiva = New CheckBox()
        DateTimePickerScadenza = New DateTimePicker()
        Label4 = New Label()
        txtTipoPagamento = New TextBox()
        Label5 = New Label()
        txtUtente = New TextBox()
        chkDaPagare = New CheckBox()
        chkPagato = New CheckBox()
        lblIndicatore = New Label()
        panelPreview = New Panel()
        lstAllegati = New ListBox()
        btnAggiungiAllegato = New Button()
        btnRimuoviAllegato = New Button()
        btnAllegatoUp = New Button()
        btnAllegatoDown = New Button()
        chkSenzaAllegato = New CheckBox()
        SuspendLayout()
        ' 
        ' btnSalva
        ' 
        btnSalva.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        btnSalva.Location = New Point(542, 43)
        btnSalva.Margin = New Padding(3, 2, 3, 2)
        btnSalva.Name = "btnSalva"
        btnSalva.Size = New Size(135, 70)
        btnSalva.TabIndex = 0
        btnSalva.Text = "Salva"
        btnSalva.UseVisualStyleBackColor = True
        ' 
        ' DateTimePickerData
        ' 
        DateTimePickerData.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        DateTimePickerData.Location = New Point(25, 103)
        DateTimePickerData.Margin = New Padding(3, 2, 3, 2)
        DateTimePickerData.Name = "DateTimePickerData"
        DateTimePickerData.Size = New Size(305, 29)
        DateTimePickerData.TabIndex = 1
        ' 
        ' TextBoxAnno
        ' 
        TextBoxAnno.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        TextBoxAnno.Location = New Point(25, 152)
        TextBoxAnno.Margin = New Padding(3, 2, 3, 2)
        TextBoxAnno.Name = "TextBoxAnno"
        TextBoxAnno.Size = New Size(110, 29)
        TextBoxAnno.TabIndex = 2
        ' 
        ' TextBoxImporto
        ' 
        TextBoxImporto.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        TextBoxImporto.Location = New Point(25, 193)
        TextBoxImporto.Margin = New Padding(3, 2, 3, 2)
        TextBoxImporto.Name = "TextBoxImporto"
        TextBoxImporto.Size = New Size(110, 29)
        TextBoxImporto.TabIndex = 3
        ' 
        ' TextBoxNote
        ' 
        TextBoxNote.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        TextBoxNote.Location = New Point(25, 238)
        TextBoxNote.Margin = New Padding(3, 2, 3, 2)
        TextBoxNote.Name = "TextBoxNote"
        TextBoxNote.Size = New Size(598, 29)
        TextBoxNote.TabIndex = 4
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Label1.Location = New Point(164, 152)
        Label1.Name = "Label1"
        Label1.Size = New Size(155, 21)
        Label1.TabIndex = 5
        Label1.Text = "Anno del documento"
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Label2.Location = New Point(164, 192)
        Label2.Name = "Label2"
        Label2.Size = New Size(66, 21)
        Label2.TabIndex = 6
        Label2.Text = "Importo"
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Label3.Location = New Point(629, 246)
        Label3.Name = "Label3"
        Label3.Size = New Size(44, 21)
        Label3.TabIndex = 7
        Label3.Text = "Note"
        ' 
        ' chkScadenzaAttiva
        ' 
        chkScadenzaAttiva.AutoSize = True
        chkScadenzaAttiva.Location = New Point(542, 167)
        chkScadenzaAttiva.Margin = New Padding(3, 2, 3, 2)
        chkScadenzaAttiva.Name = "chkScadenzaAttiva"
        chkScadenzaAttiva.Size = New Size(120, 19)
        chkScadenzaAttiva.TabIndex = 46
        chkScadenzaAttiva.Text = "Imposta scadenza"
        chkScadenzaAttiva.UseVisualStyleBackColor = True
        ' 
        ' DateTimePickerScadenza
        ' 
        DateTimePickerScadenza.CustomFormat = "dd/MM/yyyy HH:mm"
        DateTimePickerScadenza.Format = DateTimePickerFormat.Custom
        DateTimePickerScadenza.Location = New Point(454, 190)
        DateTimePickerScadenza.Margin = New Padding(3, 2, 3, 2)
        DateTimePickerScadenza.Name = "DateTimePickerScadenza"
        DateTimePickerScadenza.ShowUpDown = True
        DateTimePickerScadenza.Size = New Size(219, 23)
        DateTimePickerScadenza.TabIndex = 47
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Label4.Location = New Point(10, 34)
        Label4.Margin = New Padding(4, 0, 4, 0)
        Label4.Name = "Label4"
        Label4.Size = New Size(88, 21)
        Label4.TabIndex = 51
        Label4.Text = "Pagamento"
        ' 
        ' txtTipoPagamento
        ' 
        txtTipoPagamento.Location = New Point(115, 32)
        txtTipoPagamento.Margin = New Padding(4, 3, 4, 3)
        txtTipoPagamento.Name = "txtTipoPagamento"
        txtTipoPagamento.Size = New Size(360, 23)
        txtTipoPagamento.TabIndex = 50
        ' 
        ' Label5
        ' 
        Label5.AutoSize = True
        Label5.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Label5.Location = New Point(10, 8)
        Label5.Margin = New Padding(4, 0, 4, 0)
        Label5.Name = "Label5"
        Label5.Size = New Size(56, 21)
        Label5.TabIndex = 49
        Label5.Text = "Utente"
        ' 
        ' txtUtente
        ' 
        txtUtente.Location = New Point(89, 6)
        txtUtente.Margin = New Padding(4, 3, 4, 3)
        txtUtente.Name = "txtUtente"
        txtUtente.Size = New Size(386, 23)
        txtUtente.TabIndex = 48
        ' 
        ' chkDaPagare
        ' 
        chkDaPagare.AutoSize = True
        chkDaPagare.Location = New Point(542, 124)
        chkDaPagare.Margin = New Padding(3, 2, 3, 2)
        chkDaPagare.Name = "chkDaPagare"
        chkDaPagare.Size = New Size(79, 19)
        chkDaPagare.TabIndex = 52
        chkDaPagare.Text = "Da pagare"
        chkDaPagare.UseVisualStyleBackColor = True
        ' 
        ' chkPagato
        ' 
        chkPagato.AutoSize = True
        chkPagato.Location = New Point(542, 145)
        chkPagato.Margin = New Padding(3, 2, 3, 2)
        chkPagato.Name = "chkPagato"
        chkPagato.Size = New Size(63, 19)
        chkPagato.TabIndex = 53
        chkPagato.Text = "Pagato"
        chkPagato.UseVisualStyleBackColor = True
        ' 
        ' lblIndicatore
        ' 
        lblIndicatore.AutoSize = True
        lblIndicatore.Font = New Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        lblIndicatore.Location = New Point(508, 8)
        lblIndicatore.Name = "lblIndicatore"
        lblIndicatore.Size = New Size(60, 21)
        lblIndicatore.TabIndex = 54
        lblIndicatore.Text = "Label6"
        ' 
        ' panelPreview
        ' 
        panelPreview.BorderStyle = BorderStyle.FixedSingle
        panelPreview.Location = New Point(704, 19)
        panelPreview.Margin = New Padding(3, 2, 3, 2)
        panelPreview.Name = "panelPreview"
        panelPreview.Size = New Size(399, 442)
        panelPreview.TabIndex = 57
        ' 
        ' lstAllegati
        ' 
        lstAllegati.FormattingEnabled = True
        lstAllegati.ItemHeight = 15
        lstAllegati.Location = New Point(115, 293)
        lstAllegati.Name = "lstAllegati"
        lstAllegati.Size = New Size(508, 169)
        lstAllegati.TabIndex = 58
        ' 
        ' btnAggiungiAllegato
        ' 
        btnAggiungiAllegato.Location = New Point(23, 293)
        btnAggiungiAllegato.Name = "btnAggiungiAllegato"
        btnAggiungiAllegato.Size = New Size(86, 63)
        btnAggiungiAllegato.TabIndex = 59
        btnAggiungiAllegato.Text = "Aggiungi allegato"
        btnAggiungiAllegato.UseVisualStyleBackColor = True
        ' 
        ' btnRimuoviAllegato
        ' 
        btnRimuoviAllegato.Location = New Point(23, 362)
        btnRimuoviAllegato.Name = "btnRimuoviAllegato"
        btnRimuoviAllegato.Size = New Size(86, 63)
        btnRimuoviAllegato.TabIndex = 60
        btnRimuoviAllegato.Text = "Rimuovi allegato"
        btnRimuoviAllegato.UseVisualStyleBackColor = True
        ' 
        ' btnAllegatoUp
        ' 
        btnAllegatoUp.Location = New Point(629, 320)
        btnAllegatoUp.Name = "btnAllegatoUp"
        btnAllegatoUp.Size = New Size(48, 49)
        btnAllegatoUp.TabIndex = 61
        btnAllegatoUp.Text = "Su"
        btnAllegatoUp.UseVisualStyleBackColor = True
        ' 
        ' btnAllegatoDown
        ' 
        btnAllegatoDown.Location = New Point(629, 375)
        btnAllegatoDown.Name = "btnAllegatoDown"
        btnAllegatoDown.Size = New Size(48, 49)
        btnAllegatoDown.TabIndex = 62
        btnAllegatoDown.Text = "Giù"
        btnAllegatoDown.UseVisualStyleBackColor = True
        ' 
        ' chkSenzaAllegato
        ' 
        chkSenzaAllegato.AutoSize = True
        chkSenzaAllegato.Location = New Point(25, 431)
        chkSenzaAllegato.Name = "chkSenzaAllegato"
        chkSenzaAllegato.Size = New Size(85, 19)
        chkSenzaAllegato.TabIndex = 63
        chkSenzaAllegato.Text = "no allegato"
        chkSenzaAllegato.UseVisualStyleBackColor = True
        ' 
        ' FormNuovoDocumento
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1114, 474)
        Controls.Add(chkSenzaAllegato)
        Controls.Add(btnAllegatoDown)
        Controls.Add(btnAllegatoUp)
        Controls.Add(btnRimuoviAllegato)
        Controls.Add(btnAggiungiAllegato)
        Controls.Add(lstAllegati)
        Controls.Add(panelPreview)
        Controls.Add(lblIndicatore)
        Controls.Add(chkPagato)
        Controls.Add(chkDaPagare)
        Controls.Add(Label4)
        Controls.Add(txtTipoPagamento)
        Controls.Add(Label5)
        Controls.Add(txtUtente)
        Controls.Add(DateTimePickerScadenza)
        Controls.Add(chkScadenzaAttiva)
        Controls.Add(Label3)
        Controls.Add(Label2)
        Controls.Add(Label1)
        Controls.Add(TextBoxNote)
        Controls.Add(TextBoxImporto)
        Controls.Add(TextBoxAnno)
        Controls.Add(DateTimePickerData)
        Controls.Add(btnSalva)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Margin = New Padding(3, 2, 3, 2)
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
    Friend WithEvents chkScadenzaAttiva As CheckBox
    Friend WithEvents DateTimePickerScadenza As DateTimePicker
    Friend WithEvents Label4 As Label
    Friend WithEvents txtTipoPagamento As TextBox
    Friend WithEvents Label5 As Label
    Friend WithEvents txtUtente As TextBox
    Friend WithEvents chkDaPagare As CheckBox
    Friend WithEvents chkPagato As CheckBox
    Friend WithEvents lblIndicatore As Label
    Friend WithEvents panelPreview As Panel
    Friend WithEvents lstAllegati As ListBox
    Friend WithEvents btnAggiungiAllegato As Button
    Friend WithEvents btnRimuoviAllegato As Button
    Friend WithEvents btnAllegatoUp As Button
    Friend WithEvents btnAllegatoDown As Button
    Friend WithEvents chkSenzaAllegato As CheckBox
End Class
