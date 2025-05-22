<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormInserimentoGuidato
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
        components = New ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormInserimentoGuidato))
        dgvDocumenti = New DataGridView()
        btnAggiungiDocumento = New Button()
        txtUtente = New TextBox()
        Label1 = New Label()
        Label2 = New Label()
        txtTipoPagamento = New TextBox()
        btnApriFile = New Button()
        btnElimina = New Button()
        btnModifica = New Button()
        TimerScadenze = New Timer(components)
        lblNotifica = New Label()
        picNotifica = New PictureBox()
        lstRiepilogoScadenze = New ListBox()
        CType(dgvDocumenti, ComponentModel.ISupportInitialize).BeginInit()
        CType(picNotifica, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' dgvDocumenti
        ' 
        dgvDocumenti.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvDocumenti.Location = New Point(13, 311)
        dgvDocumenti.Margin = New Padding(4)
        dgvDocumenti.Name = "dgvDocumenti"
        dgvDocumenti.RowHeadersWidth = 51
        dgvDocumenti.Size = New Size(1477, 421)
        dgvDocumenti.TabIndex = 0
        ' 
        ' btnAggiungiDocumento
        ' 
        btnAggiungiDocumento.Location = New Point(1244, 185)
        btnAggiungiDocumento.Margin = New Padding(4)
        btnAggiungiDocumento.Name = "btnAggiungiDocumento"
        btnAggiungiDocumento.Size = New Size(246, 64)
        btnAggiungiDocumento.TabIndex = 1
        btnAggiungiDocumento.Text = "Aggiungi nuovo"
        btnAggiungiDocumento.UseVisualStyleBackColor = True
        ' 
        ' txtUtente
        ' 
        txtUtente.Location = New Point(103, 17)
        txtUtente.Margin = New Padding(4)
        txtUtente.Name = "txtUtente"
        txtUtente.Size = New Size(440, 34)
        txtUtente.TabIndex = 2
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Label1.Location = New Point(13, 20)
        Label1.Margin = New Padding(4, 0, 4, 0)
        Label1.Name = "Label1"
        Label1.Size = New Size(71, 28)
        Label1.TabIndex = 3
        Label1.Text = "Utente"
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Label2.Location = New Point(13, 91)
        Label2.Margin = New Padding(4, 0, 4, 0)
        Label2.Name = "Label2"
        Label2.Size = New Size(111, 28)
        Label2.TabIndex = 5
        Label2.Text = "Pagamento"
        ' 
        ' txtTipoPagamento
        ' 
        txtTipoPagamento.Location = New Point(132, 88)
        txtTipoPagamento.Margin = New Padding(4)
        txtTipoPagamento.Name = "txtTipoPagamento"
        txtTipoPagamento.Size = New Size(411, 34)
        txtTipoPagamento.TabIndex = 4
        ' 
        ' btnApriFile
        ' 
        btnApriFile.Location = New Point(612, 47)
        btnApriFile.Name = "btnApriFile"
        btnApriFile.Size = New Size(209, 52)
        btnApriFile.TabIndex = 6
        btnApriFile.Text = "Apri selezionato"
        btnApriFile.UseVisualStyleBackColor = True
        ' 
        ' btnElimina
        ' 
        btnElimina.Location = New Point(843, 47)
        btnElimina.Name = "btnElimina"
        btnElimina.Size = New Size(209, 52)
        btnElimina.TabIndex = 7
        btnElimina.Text = "Elimina selezionato"
        btnElimina.UseVisualStyleBackColor = True
        ' 
        ' btnModifica
        ' 
        btnModifica.Location = New Point(1069, 47)
        btnModifica.Name = "btnModifica"
        btnModifica.Size = New Size(209, 52)
        btnModifica.TabIndex = 8
        btnModifica.Text = "Modifica selezionato"
        btnModifica.UseVisualStyleBackColor = True
        ' 
        ' TimerScadenze
        ' 
        TimerScadenze.Enabled = True
        ' 
        ' lblNotifica
        ' 
        lblNotifica.AutoSize = True
        lblNotifica.Location = New Point(1069, 161)
        lblNotifica.Name = "lblNotifica"
        lblNotifica.Size = New Size(69, 28)
        lblNotifica.TabIndex = 9
        lblNotifica.Text = "Label3"
        ' 
        ' picNotifica
        ' 
        picNotifica.Image = CType(resources.GetObject("picNotifica.Image"), Image)
        picNotifica.Location = New Point(1023, 161)
        picNotifica.Name = "picNotifica"
        picNotifica.Size = New Size(40, 62)
        picNotifica.TabIndex = 10
        picNotifica.TabStop = False
        ' 
        ' lstRiepilogoScadenze
        ' 
        lstRiepilogoScadenze.FormattingEnabled = True
        lstRiepilogoScadenze.HorizontalScrollbar = True
        lstRiepilogoScadenze.Location = New Point(13, 161)
        lstRiepilogoScadenze.Name = "lstRiepilogoScadenze"
        lstRiepilogoScadenze.Size = New Size(993, 116)
        lstRiepilogoScadenze.TabIndex = 11
        ' 
        ' FormInserimentoGuidato
        ' 
        AutoScaleDimensions = New SizeF(11F, 28F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1509, 745)
        Controls.Add(lstRiepilogoScadenze)
        Controls.Add(picNotifica)
        Controls.Add(lblNotifica)
        Controls.Add(btnModifica)
        Controls.Add(btnElimina)
        Controls.Add(btnApriFile)
        Controls.Add(Label2)
        Controls.Add(txtTipoPagamento)
        Controls.Add(Label1)
        Controls.Add(txtUtente)
        Controls.Add(btnAggiungiDocumento)
        Controls.Add(dgvDocumenti)
        Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Margin = New Padding(4)
        MaximizeBox = False
        Name = "FormInserimentoGuidato"
        Text = "FormInserimentoGuidato"
        CType(dgvDocumenti, ComponentModel.ISupportInitialize).EndInit()
        CType(picNotifica, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents dgvDocumenti As DataGridView
    Friend WithEvents btnAggiungiDocumento As Button
    Friend WithEvents txtUtente As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents txtTipoPagamento As TextBox
    Friend WithEvents btnApriFile As Button
    Friend WithEvents btnElimina As Button
    Friend WithEvents btnModifica As Button
    Friend WithEvents TimerScadenze As Timer
    Friend WithEvents lblNotifica As Label
    Friend WithEvents picNotifica As PictureBox
    Friend WithEvents lstRiepilogoScadenze As ListBox
End Class
