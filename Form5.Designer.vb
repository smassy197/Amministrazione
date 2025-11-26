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
        buttonOpenDatabaseFolder = New Button()
        buttonHighlight = New Button()
        buttonFont = New Button()
        btnCreaTabella = New Button()
        btnVaiAlDesktop = New Button()
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
        menuStrip1.Location = New Point(10, 12)
        menuStrip1.Name = "menuStrip1"
        menuStrip1.Padding = New Padding(5, 2, 0, 2)
        menuStrip1.Size = New Size(19, 24)
        menuStrip1.TabIndex = 0
        menuStrip1.Text = "MenuStrip2"
        ' 
        ' ToolStripMenuItem1
        ' 
        ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        ToolStripMenuItem1.Size = New Size(12, 20)
        ' 
        ' listBoxFiles
        ' 
        listBoxFiles.FormattingEnabled = True
        listBoxFiles.Location = New Point(921, 35)
        listBoxFiles.Margin = New Padding(3, 2, 3, 2)
        listBoxFiles.Name = "listBoxFiles"
        listBoxFiles.Size = New Size(155, 199)
        listBoxFiles.TabIndex = 1
        ' 
        ' buttonLoadFile
        ' 
        buttonLoadFile.Location = New Point(994, 264)
        buttonLoadFile.Margin = New Padding(3, 2, 3, 2)
        buttonLoadFile.Name = "buttonLoadFile"
        buttonLoadFile.Size = New Size(82, 22)
        buttonLoadFile.TabIndex = 2
        buttonLoadFile.Text = "Carica"
        buttonLoadFile.UseVisualStyleBackColor = True
        ' 
        ' buttonDocuments
        ' 
        buttonDocuments.Location = New Point(994, 238)
        buttonDocuments.Margin = New Padding(3, 2, 3, 2)
        buttonDocuments.Name = "buttonDocuments"
        buttonDocuments.Size = New Size(82, 22)
        buttonDocuments.TabIndex = 3
        buttonDocuments.Text = "Visualizza"
        buttonDocuments.UseVisualStyleBackColor = True
        ' 
        ' buttonSave
        ' 
        buttonSave.Location = New Point(994, 290)
        buttonSave.Margin = New Padding(3, 2, 3, 2)
        buttonSave.Name = "buttonSave"
        buttonSave.Size = New Size(82, 22)
        buttonSave.TabIndex = 4
        buttonSave.Text = "Salva"
        buttonSave.UseVisualStyleBackColor = True
        ' 
        ' buttonDeleteFile
        ' 
        buttonDeleteFile.Location = New Point(994, 316)
        buttonDeleteFile.Margin = New Padding(3, 2, 3, 2)
        buttonDeleteFile.Name = "buttonDeleteFile"
        buttonDeleteFile.Size = New Size(82, 22)
        buttonDeleteFile.TabIndex = 5
        buttonDeleteFile.Text = "Elimina"
        buttonDeleteFile.UseVisualStyleBackColor = True
        ' 
        ' richTextBoxDocument
        ' 
        richTextBoxDocument.Location = New Point(10, 35)
        richTextBoxDocument.Margin = New Padding(3, 2, 3, 2)
        richTextBoxDocument.Name = "richTextBoxDocument"
        richTextBoxDocument.Size = New Size(906, 486)
        richTextBoxDocument.TabIndex = 6
        richTextBoxDocument.Text = ""
        ' 
        ' textBoxFilePath
        ' 
        textBoxFilePath.Location = New Point(10, 524)
        textBoxFilePath.Margin = New Padding(3, 2, 3, 2)
        textBoxFilePath.Name = "textBoxFilePath"
        textBoxFilePath.Size = New Size(906, 23)
        textBoxFilePath.TabIndex = 7
        ' 
        ' buttonOpenDatabaseFolder
        ' 
        buttonOpenDatabaseFolder.BackgroundImage = CType(resources.GetObject("buttonOpenDatabaseFolder.BackgroundImage"), Image)
        buttonOpenDatabaseFolder.Location = New Point(921, 238)
        buttonOpenDatabaseFolder.Margin = New Padding(3, 2, 3, 2)
        buttonOpenDatabaseFolder.Name = "buttonOpenDatabaseFolder"
        buttonOpenDatabaseFolder.Size = New Size(41, 33)
        buttonOpenDatabaseFolder.TabIndex = 10
        buttonOpenDatabaseFolder.UseVisualStyleBackColor = True
        ' 
        ' buttonHighlight
        ' 
        buttonHighlight.Location = New Point(293, 9)
        buttonHighlight.Margin = New Padding(3, 2, 3, 2)
        buttonHighlight.Name = "buttonHighlight"
        buttonHighlight.Size = New Size(35, 22)
        buttonHighlight.TabIndex = 11
        buttonHighlight.Text = "Ev"
        buttonHighlight.UseVisualStyleBackColor = True
        ' 
        ' buttonFont
        ' 
        buttonFont.Location = New Point(333, 9)
        buttonFont.Margin = New Padding(3, 2, 3, 2)
        buttonFont.Name = "buttonFont"
        buttonFont.Size = New Size(35, 22)
        buttonFont.TabIndex = 12
        buttonFont.Text = "Fo"
        buttonFont.UseVisualStyleBackColor = True
        ' 
        ' btnCreaTabella
        ' 
        btnCreaTabella.Location = New Point(374, 9)
        btnCreaTabella.Margin = New Padding(3, 2, 3, 2)
        btnCreaTabella.Name = "btnCreaTabella"
        btnCreaTabella.Size = New Size(35, 22)
        btnCreaTabella.TabIndex = 13
        btnCreaTabella.Text = "Ta"
        btnCreaTabella.UseVisualStyleBackColor = True
        ' 
        ' btnVaiAlDesktop
        ' 
        btnVaiAlDesktop.Location = New Point(921, 275)
        btnVaiAlDesktop.Margin = New Padding(3, 2, 3, 2)
        btnVaiAlDesktop.Name = "btnVaiAlDesktop"
        btnVaiAlDesktop.Size = New Size(41, 22)
        btnVaiAlDesktop.TabIndex = 14
        btnVaiAlDesktop.Text = "De"
        btnVaiAlDesktop.UseVisualStyleBackColor = True
        ' 
        ' btnUndo
        ' 
        btnUndo.Location = New Point(500, 9)
        btnUndo.Margin = New Padding(3, 2, 3, 2)
        btnUndo.Name = "btnUndo"
        btnUndo.Size = New Size(35, 22)
        btnUndo.TabIndex = 16
        btnUndo.Text = "u"
        btnUndo.UseVisualStyleBackColor = True
        ' 
        ' btnRedo
        ' 
        btnRedo.Location = New Point(541, 9)
        btnRedo.Margin = New Padding(3, 2, 3, 2)
        btnRedo.Name = "btnRedo"
        btnRedo.Size = New Size(35, 22)
        btnRedo.TabIndex = 17
        btnRedo.Text = "r"
        btnRedo.UseVisualStyleBackColor = True
        ' 
        ' Form5
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1087, 554)
        Controls.Add(btnRedo)
        Controls.Add(btnUndo)
        Controls.Add(btnVaiAlDesktop)
        Controls.Add(btnCreaTabella)
        Controls.Add(buttonFont)
        Controls.Add(buttonHighlight)
        Controls.Add(buttonOpenDatabaseFolder)
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
        Margin = New Padding(3, 2, 3, 2)
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
    Friend WithEvents buttonOpenDatabaseFolder As Button
    Friend WithEvents buttonHighlight As Button
    Friend WithEvents ToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents buttonFont As Button
    Friend WithEvents btnCreaTabella As Button
    Friend WithEvents btnVaiAlDesktop As Button
    Friend WithEvents btnUndo As Button
    Friend WithEvents btnRedo As Button
End Class
