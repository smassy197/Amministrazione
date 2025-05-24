<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form3
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form3))
        TextBoxAnno = New TextBox()
        ComboBoxTipoPagamento = New ComboBox()
        TextBoxImporto = New TextBox()
        ComboBoxNote = New ComboBox()
        Label1 = New Label()
        Label2 = New Label()
        Label3 = New Label()
        Label4 = New Label()
        dgvDocumenti = New DataGridView()
        MonthCalendar1 = New MonthCalendar()
        btnOpenForm3 = New Button()
        btnSearch = New Button()
        btnReset = New Button()
        Button1 = New Button()
        rbtnUtente1 = New CheckBox()
        rbtnUtente2 = New CheckBox()
        rbtnUtente3 = New CheckBox()
        TimerScadenze = New Timer(components)
        lblNotifica = New Label()
        picNotifica = New PictureBox()
        btnInserimentoGuidato = New Button()
        btnExport = New Button()
        dtpEndDate = New DateTimePicker()
        dtpStartDate = New DateTimePicker()
        CType(dgvDocumenti, ComponentModel.ISupportInitialize).BeginInit()
        CType(picNotifica, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' TextBoxAnno
        ' 
        TextBoxAnno.BackColor = Color.FromArgb(CByte(224), CByte(224), CByte(224))
        TextBoxAnno.Font = New Font("Segoe UI", 12F)
        TextBoxAnno.Location = New Point(494, 22)
        TextBoxAnno.Name = "TextBoxAnno"
        TextBoxAnno.Size = New Size(303, 34)
        TextBoxAnno.TabIndex = 2
        ' 
        ' ComboBoxTipoPagamento
        ' 
        ComboBoxTipoPagamento.BackColor = Color.FromArgb(CByte(224), CByte(224), CByte(224))
        ComboBoxTipoPagamento.Font = New Font("Segoe UI", 12F)
        ComboBoxTipoPagamento.FormattingEnabled = True
        ComboBoxTipoPagamento.Location = New Point(494, 62)
        ComboBoxTipoPagamento.Name = "ComboBoxTipoPagamento"
        ComboBoxTipoPagamento.Size = New Size(303, 36)
        ComboBoxTipoPagamento.TabIndex = 3
        ' 
        ' TextBoxImporto
        ' 
        TextBoxImporto.Font = New Font("Segoe UI", 12F)
        TextBoxImporto.Location = New Point(494, 104)
        TextBoxImporto.Name = "TextBoxImporto"
        TextBoxImporto.Size = New Size(303, 34)
        TextBoxImporto.TabIndex = 4
        ' 
        ' ComboBoxNote
        ' 
        ComboBoxNote.BackColor = Color.FromArgb(CByte(224), CByte(224), CByte(224))
        ComboBoxNote.Font = New Font("Segoe UI", 12F)
        ComboBoxNote.FormattingEnabled = True
        ComboBoxNote.Location = New Point(494, 144)
        ComboBoxNote.Name = "ComboBoxNote"
        ComboBoxNote.Size = New Size(303, 36)
        ComboBoxNote.TabIndex = 8
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Font = New Font("Segoe UI", 12F)
        Label1.Location = New Point(301, 29)
        Label1.Name = "Label1"
        Label1.Size = New Size(183, 28)
        Label1.TabIndex = 9
        Label1.Text = "Anno di riferimento"
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Font = New Font("Segoe UI", 12F)
        Label2.Location = New Point(305, 70)
        Label2.Name = "Label2"
        Label2.Size = New Size(179, 28)
        Label2.TabIndex = 10
        Label2.Text = "Tipo di pagamento"
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Font = New Font("Segoe UI", 12F)
        Label3.Location = New Point(400, 110)
        Label3.Name = "Label3"
        Label3.Size = New Size(84, 28)
        Label3.TabIndex = 11
        Label3.Text = "Importo"
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.Font = New Font("Segoe UI", 12F)
        Label4.Location = New Point(428, 150)
        Label4.Name = "Label4"
        Label4.Size = New Size(56, 28)
        Label4.TabIndex = 12
        Label4.Text = "Note"
        ' 
        ' dgvDocumenti
        ' 
        dgvDocumenti.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvDocumenti.Dock = DockStyle.Bottom
        dgvDocumenti.Location = New Point(0, 286)
        dgvDocumenti.Name = "dgvDocumenti"
        dgvDocumenti.RowHeadersWidth = 51
        dgvDocumenti.Size = New Size(1304, 364)
        dgvDocumenti.TabIndex = 14
        ' 
        ' MonthCalendar1
        ' 
        MonthCalendar1.Location = New Point(18, 29)
        MonthCalendar1.Name = "MonthCalendar1"
        MonthCalendar1.TabIndex = 18
        ' 
        ' btnOpenForm3
        ' 
        btnOpenForm3.Font = New Font("Segoe UI", 13.8F)
        btnOpenForm3.Location = New Point(400, 199)
        btnOpenForm3.Name = "btnOpenForm3"
        btnOpenForm3.Size = New Size(92, 59)
        btnOpenForm3.TabIndex = 25
        btnOpenForm3.Text = "Conti"
        btnOpenForm3.UseVisualStyleBackColor = True
        ' 
        ' btnSearch
        ' 
        btnSearch.Location = New Point(821, 120)
        btnSearch.Name = "btnSearch"
        btnSearch.Size = New Size(94, 29)
        btnSearch.TabIndex = 26
        btnSearch.Text = "Ricerca"
        btnSearch.UseVisualStyleBackColor = True
        ' 
        ' btnReset
        ' 
        btnReset.Location = New Point(821, 153)
        btnReset.Name = "btnReset"
        btnReset.Size = New Size(94, 29)
        btnReset.TabIndex = 30
        btnReset.Text = "Reset"
        btnReset.UseVisualStyleBackColor = True
        ' 
        ' Button1
        ' 
        Button1.Font = New Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Button1.Location = New Point(498, 199)
        Button1.Name = "Button1"
        Button1.Size = New Size(92, 59)
        Button1.TabIndex = 31
        Button1.Text = "Fogli"
        Button1.UseVisualStyleBackColor = True
        ' 
        ' rbtnUtente1
        ' 
        rbtnUtente1.AutoSize = True
        rbtnUtente1.Location = New Point(821, 29)
        rbtnUtente1.Name = "rbtnUtente1"
        rbtnUtente1.Size = New Size(94, 24)
        rbtnUtente1.TabIndex = 39
        rbtnUtente1.Text = "Personale"
        rbtnUtente1.UseVisualStyleBackColor = True
        ' 
        ' rbtnUtente2
        ' 
        rbtnUtente2.AutoSize = True
        rbtnUtente2.Location = New Point(821, 59)
        rbtnUtente2.Name = "rbtnUtente2"
        rbtnUtente2.Size = New Size(62, 24)
        rbtnUtente2.TabIndex = 40
        rbtnUtente2.Text = "Casa"
        rbtnUtente2.UseVisualStyleBackColor = True
        ' 
        ' rbtnUtente3
        ' 
        rbtnUtente3.AutoSize = True
        rbtnUtente3.Location = New Point(821, 90)
        rbtnUtente3.Name = "rbtnUtente3"
        rbtnUtente3.Size = New Size(64, 24)
        rbtnUtente3.TabIndex = 41
        rbtnUtente3.Text = "Altro"
        rbtnUtente3.UseVisualStyleBackColor = True
        ' 
        ' TimerScadenze
        ' 
        TimerScadenze.Enabled = True
        ' 
        ' lblNotifica
        ' 
        lblNotifica.AutoSize = True
        lblNotifica.Location = New Point(1236, 21)
        lblNotifica.Name = "lblNotifica"
        lblNotifica.Size = New Size(53, 20)
        lblNotifica.TabIndex = 46
        lblNotifica.Text = "Label7"
        ' 
        ' picNotifica
        ' 
        picNotifica.Image = CType(resources.GetObject("picNotifica.Image"), Image)
        picNotifica.Location = New Point(1196, 12)
        picNotifica.Name = "picNotifica"
        picNotifica.Size = New Size(39, 41)
        picNotifica.TabIndex = 47
        picNotifica.TabStop = False
        ' 
        ' btnInserimentoGuidato
        ' 
        btnInserimentoGuidato.Font = New Font("Segoe UI", 13.8F)
        btnInserimentoGuidato.Location = New Point(958, 29)
        btnInserimentoGuidato.Name = "btnInserimentoGuidato"
        btnInserimentoGuidato.Size = New Size(194, 153)
        btnInserimentoGuidato.TabIndex = 48
        btnInserimentoGuidato.Text = "Inserimento Guidato"
        btnInserimentoGuidato.UseVisualStyleBackColor = True
        ' 
        ' btnExport
        ' 
        btnExport.Location = New Point(857, 199)
        btnExport.Name = "btnExport"
        btnExport.Size = New Size(75, 59)
        btnExport.TabIndex = 51
        btnExport.Text = "Esporta"
        btnExport.UseVisualStyleBackColor = True
        ' 
        ' dtpEndDate
        ' 
        dtpEndDate.Location = New Point(601, 231)
        dtpEndDate.Name = "dtpEndDate"
        dtpEndDate.Size = New Size(250, 27)
        dtpEndDate.TabIndex = 50
        ' 
        ' dtpStartDate
        ' 
        dtpStartDate.Location = New Point(601, 199)
        dtpStartDate.Name = "dtpStartDate"
        dtpStartDate.Size = New Size(250, 27)
        dtpStartDate.TabIndex = 49
        ' 
        ' Form3
        ' 
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1304, 650)
        Controls.Add(btnExport)
        Controls.Add(dtpEndDate)
        Controls.Add(dtpStartDate)
        Controls.Add(btnInserimentoGuidato)
        Controls.Add(picNotifica)
        Controls.Add(lblNotifica)
        Controls.Add(rbtnUtente3)
        Controls.Add(rbtnUtente2)
        Controls.Add(rbtnUtente1)
        Controls.Add(Button1)
        Controls.Add(btnReset)
        Controls.Add(btnSearch)
        Controls.Add(btnOpenForm3)
        Controls.Add(MonthCalendar1)
        Controls.Add(dgvDocumenti)
        Controls.Add(Label4)
        Controls.Add(Label3)
        Controls.Add(Label2)
        Controls.Add(Label1)
        Controls.Add(ComboBoxNote)
        Controls.Add(TextBoxImporto)
        Controls.Add(ComboBoxTipoPagamento)
        Controls.Add(TextBoxAnno)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        Name = "Form3"
        Text = "Documenti"
        CType(dgvDocumenti, ComponentModel.ISupportInitialize).EndInit()
        CType(picNotifica, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents btnInserisciDati As Button
    Friend WithEvents TextBoxAnno As TextBox
    Friend WithEvents ComboBoxTipoPagamento As ComboBox
    Friend WithEvents TextBoxImporto As TextBox
    Friend WithEvents ComboBoxNote As ComboBox
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents dgvDocumenti As DataGridView
    Friend WithEvents MonthCalendar1 As MonthCalendar
    Friend WithEvents btnOpenForm3 As Button
    Friend WithEvents btnSearch As Button
    Friend WithEvents txtKeywordSearch As TextBox
    Friend WithEvents btnReset As Button
    Friend WithEvents Button1 As Button
    Friend WithEvents rbtnUtente1 As CheckBox
    Friend WithEvents rbtnUtente2 As CheckBox
    Friend WithEvents rbtnUtente3 As CheckBox
    Friend WithEvents TimerScadenze As Timer
    Friend WithEvents lblNotifica As Label
    Friend WithEvents picNotifica As PictureBox
    Friend WithEvents Button2 As Button
    Friend WithEvents btnInserimentoGuidato As Button
    Friend WithEvents btnExport As Button
    Friend WithEvents dtpEndDate As DateTimePicker
    Friend WithEvents dtpStartDate As DateTimePicker
End Class
