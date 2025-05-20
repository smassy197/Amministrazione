<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form5
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form5))
        menuStrip1 = New MenuStrip()
        ToolStripMenuItem1 = New ToolStripMenuItem()
        listBoxFiles = New ListBox()
        buttonLoadFile = New Button()
        buttonDocuments = New Button()
        buttonSave = New Button()
        buttonDeleteFile = New Button()
        richTextBoxDocument = New RichTextBox()
        textBoxFilePath = New TextBox()
        btnForm1 = New Button()
        bntForm3 = New Button()
        buttonOpenDatabaseFolder = New Button()
        buttonHighlight = New Button()
        buttonFont = New Button()
        btnCreaTabella = New Button()
        btnVaiAlDesktop = New Button()
        btnInserisciLink = New Button()
        btnUndo = New Button()
        btnRedo = New Button()
        menuStrip1.SuspendLayout()
        SuspendLayout()
        ' 
        ' menuStrip1
        ' 
        menuStrip1.Anchor = AnchorStyles.None
        menuStrip1.Dock = DockStyle.None
        menuStrip1.ImageScalingSize = New Size(20, 20)
        menuStrip1.Items.AddRange(New ToolStripItem() {ToolStripMenuItem1})
        menuStrip1.Location = New Point(12, 16)
        menuStrip1.Name = "menuStrip1"
        menuStrip1.Size = New Size(172, 28)
        menuStrip1.TabIndex = 0
        menuStrip1.Text = "MenuStrip2"
        ' 
        ' ToolStripMenuItem1
        ' 
        ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        ToolStripMenuItem1.Size = New Size(14, 24)
        ' 
        ' listBoxFiles
        ' 
        listBoxFiles.FormattingEnabled = True
        listBoxFiles.Location = New Point(1053, 47)
        listBoxFiles.Name = "listBoxFiles"
        listBoxFiles.Size = New Size(177, 264)
        listBoxFiles.TabIndex = 1
        ' 
        ' buttonLoadFile
        ' 
        buttonLoadFile.Location = New Point(1136, 352)
        buttonLoadFile.Name = "buttonLoadFile"
        buttonLoadFile.Size = New Size(94, 29)
        buttonLoadFile.TabIndex = 2
        buttonLoadFile.Text = "Carica"
        buttonLoadFile.UseVisualStyleBackColor = True
        ' 
        ' buttonDocuments
        ' 
        buttonDocuments.Location = New Point(1136, 317)
        buttonDocuments.Name = "buttonDocuments"
        buttonDocuments.Size = New Size(94, 29)
        buttonDocuments.TabIndex = 3
        buttonDocuments.Text = "Visualizza"
        buttonDocuments.UseVisualStyleBackColor = True
        ' 
        ' buttonSave
        ' 
        buttonSave.Location = New Point(1136, 387)
        buttonSave.Name = "buttonSave"
        buttonSave.Size = New Size(94, 29)
        buttonSave.TabIndex = 4
        buttonSave.Text = "Salva"
        buttonSave.UseVisualStyleBackColor = True
        ' 
        ' buttonDeleteFile
        ' 
        buttonDeleteFile.Location = New Point(1136, 422)
        buttonDeleteFile.Name = "buttonDeleteFile"
        buttonDeleteFile.Size = New Size(94, 29)
        buttonDeleteFile.TabIndex = 5
        buttonDeleteFile.Text = "Elimina"
        buttonDeleteFile.UseVisualStyleBackColor = True
        ' 
        ' richTextBoxDocument
        ' 
        richTextBoxDocument.Location = New Point(12, 47)
        richTextBoxDocument.Name = "richTextBoxDocument"
        richTextBoxDocument.Size = New Size(1035, 646)
        richTextBoxDocument.TabIndex = 6
        richTextBoxDocument.Text = ""
        ' 
        ' textBoxFilePath
        ' 
        textBoxFilePath.Location = New Point(12, 699)
        textBoxFilePath.Name = "textBoxFilePath"
        textBoxFilePath.Size = New Size(1035, 27)
        textBoxFilePath.TabIndex = 7
        ' 
        ' btnForm1
        ' 
        btnForm1.Location = New Point(1136, 508)
        btnForm1.Name = "btnForm1"
        btnForm1.Size = New Size(94, 29)
        btnForm1.TabIndex = 8
        btnForm1.Text = "Conti"
        btnForm1.UseVisualStyleBackColor = True
        ' 
        ' bntForm3
        ' 
        bntForm3.Location = New Point(1136, 541)
        bntForm3.Name = "bntForm3"
        bntForm3.Size = New Size(94, 29)
        bntForm3.TabIndex = 9
        bntForm3.Text = "Documenti"
        bntForm3.UseVisualStyleBackColor = True
        ' 
        ' buttonOpenDatabaseFolder
        ' 
        buttonOpenDatabaseFolder.BackgroundImage = CType(resources.GetObject("buttonOpenDatabaseFolder.BackgroundImage"), Image)
        buttonOpenDatabaseFolder.Location = New Point(1053, 317)
        buttonOpenDatabaseFolder.Name = "buttonOpenDatabaseFolder"
        buttonOpenDatabaseFolder.Size = New Size(47, 44)
        buttonOpenDatabaseFolder.TabIndex = 10
        buttonOpenDatabaseFolder.UseVisualStyleBackColor = True
        ' 
        ' buttonHighlight
        ' 
        buttonHighlight.Location = New Point(335, 12)
        buttonHighlight.Name = "buttonHighlight"
        buttonHighlight.Size = New Size(40, 30)
        buttonHighlight.TabIndex = 11
        buttonHighlight.Text = "Ev"
        buttonHighlight.UseVisualStyleBackColor = True
        ' 
        ' buttonFont
        ' 
        buttonFont.Location = New Point(381, 12)
        buttonFont.Name = "buttonFont"
        buttonFont.Size = New Size(40, 30)
        buttonFont.TabIndex = 12
        buttonFont.Text = "Fo"
        buttonFont.UseVisualStyleBackColor = True
        ' 
        ' btnCreaTabella
        ' 
        btnCreaTabella.Location = New Point(427, 12)
        btnCreaTabella.Name = "btnCreaTabella"
        btnCreaTabella.Size = New Size(40, 30)
        btnCreaTabella.TabIndex = 13
        btnCreaTabella.Text = "Ta"
        btnCreaTabella.UseVisualStyleBackColor = True
        ' 
        ' btnVaiAlDesktop
        ' 
        btnVaiAlDesktop.Location = New Point(1053, 367)
        btnVaiAlDesktop.Name = "btnVaiAlDesktop"
        btnVaiAlDesktop.Size = New Size(47, 29)
        btnVaiAlDesktop.TabIndex = 14
        btnVaiAlDesktop.Text = "De"
        btnVaiAlDesktop.UseVisualStyleBackColor = True
        ' 
        ' btnInserisciLink
        ' 
        btnInserisciLink.Location = New Point(473, 12)
        btnInserisciLink.Name = "btnInserisciLink"
        btnInserisciLink.Size = New Size(40, 30)
        btnInserisciLink.TabIndex = 15
        btnInserisciLink.Text = "Li"
        btnInserisciLink.UseVisualStyleBackColor = True
        ' 
        ' btnUndo
        ' 
        btnUndo.Location = New Point(572, 12)
        btnUndo.Name = "btnUndo"
        btnUndo.Size = New Size(40, 30)
        btnUndo.TabIndex = 16
        btnUndo.Text = "u"
        btnUndo.UseVisualStyleBackColor = True
        ' 
        ' btnRedo
        ' 
        btnRedo.Location = New Point(618, 12)
        btnRedo.Name = "btnRedo"
        btnRedo.Size = New Size(40, 30)
        btnRedo.TabIndex = 17
        btnRedo.Text = "r"
        btnRedo.UseVisualStyleBackColor = True
        ' 
        ' Form5
        ' 
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1242, 738)
        Controls.Add(btnRedo)
        Controls.Add(btnUndo)
        Controls.Add(btnInserisciLink)
        Controls.Add(btnVaiAlDesktop)
        Controls.Add(btnCreaTabella)
        Controls.Add(buttonFont)
        Controls.Add(buttonHighlight)
        Controls.Add(buttonOpenDatabaseFolder)
        Controls.Add(bntForm3)
        Controls.Add(btnForm1)
        Controls.Add(textBoxFilePath)
        Controls.Add(richTextBoxDocument)
        Controls.Add(buttonDeleteFile)
        Controls.Add(buttonSave)
        Controls.Add(buttonDocuments)
        Controls.Add(buttonLoadFile)
        Controls.Add(listBoxFiles)
        Controls.Add(menuStrip1)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MainMenuStrip = menuStrip1
        MaximizeBox = False
        Name = "Form5"
        Text = "Fogli"
        menuStrip1.ResumeLayout(False)
        menuStrip1.PerformLayout()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents menuStrip1 As MenuStrip
    Friend WithEvents listBoxFiles As ListBox
    Friend WithEvents buttonLoadFile As Button
    Friend WithEvents buttonDocuments As Button
    Friend WithEvents buttonSave As Button
    Friend WithEvents buttonDeleteFile As Button
    Friend WithEvents richTextBoxDocument As RichTextBox
    Friend WithEvents textBoxFilePath As TextBox
    Friend WithEvents btnForm1 As Button
    Friend WithEvents bntForm3 As Button
    Friend WithEvents buttonOpenDatabaseFolder As Button
    Friend WithEvents buttonHighlight As Button
    Friend WithEvents ToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents buttonFont As Button
    Friend WithEvents btnCreaTabella As Button
    Friend WithEvents btnVaiAlDesktop As Button
    Friend WithEvents btnInserisciLink As Button
    Friend WithEvents btnUndo As Button
    Friend WithEvents btnRedo As Button
End Class
