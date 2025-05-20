<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormTabella
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormTabella))
        nudRighe = New NumericUpDown()
        nudColonne = New NumericUpDown()
        Righe = New Label()
        Colonne = New Label()
        btnOk = New Button()
        colori = New Label()
        btnSelezionaColore = New Button()
        panelColore = New Panel()
        nudSpessoreBordo = New NumericUpDown()
        Label1 = New Label()
        CType(nudRighe, ComponentModel.ISupportInitialize).BeginInit()
        CType(nudColonne, ComponentModel.ISupportInitialize).BeginInit()
        CType(nudSpessoreBordo, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' nudRighe
        ' 
        nudRighe.Location = New Point(108, 51)
        nudRighe.Name = "nudRighe"
        nudRighe.Size = New Size(48, 27)
        nudRighe.TabIndex = 0
        ' 
        ' nudColonne
        ' 
        nudColonne.Location = New Point(108, 110)
        nudColonne.Name = "nudColonne"
        nudColonne.Size = New Size(48, 27)
        nudColonne.TabIndex = 1
        ' 
        ' Righe
        ' 
        Righe.AutoSize = True
        Righe.Location = New Point(24, 53)
        Righe.Name = "Righe"
        Righe.Size = New Size(47, 20)
        Righe.TabIndex = 2
        Righe.Text = "Righe"
        ' 
        ' Colonne
        ' 
        Colonne.AutoSize = True
        Colonne.Location = New Point(24, 112)
        Colonne.Name = "Colonne"
        Colonne.Size = New Size(64, 20)
        Colonne.TabIndex = 3
        Colonne.Text = "Colonne"
        ' 
        ' btnOk
        ' 
        btnOk.Location = New Point(348, 235)
        btnOk.Name = "btnOk"
        btnOk.Size = New Size(94, 29)
        btnOk.TabIndex = 4
        btnOk.Text = "Crea"
        btnOk.UseVisualStyleBackColor = True
        ' 
        ' colori
        ' 
        colori.AutoSize = True
        colori.Location = New Point(24, 172)
        colori.Name = "colori"
        colori.Size = New Size(53, 20)
        colori.TabIndex = 6
        colori.Text = "Colore"
        ' 
        ' btnSelezionaColore
        ' 
        btnSelezionaColore.Location = New Point(108, 168)
        btnSelezionaColore.Name = "btnSelezionaColore"
        btnSelezionaColore.Size = New Size(84, 29)
        btnSelezionaColore.TabIndex = 7
        btnSelezionaColore.Text = "Seleziona"
        btnSelezionaColore.UseVisualStyleBackColor = True
        ' 
        ' panelColore
        ' 
        panelColore.Location = New Point(217, 170)
        panelColore.Name = "panelColore"
        panelColore.Size = New Size(23, 25)
        panelColore.TabIndex = 8
        ' 
        ' nudSpessoreBordo
        ' 
        nudSpessoreBordo.Location = New Point(108, 217)
        nudSpessoreBordo.Name = "nudSpessoreBordo"
        nudSpessoreBordo.Size = New Size(48, 27)
        nudSpessoreBordo.TabIndex = 9
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(24, 219)
        Label1.Name = "Label1"
        Label1.Size = New Size(68, 20)
        Label1.TabIndex = 10
        Label1.Text = "Spessore"
        ' 
        ' FormTabella
        ' 
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(454, 276)
        Controls.Add(Label1)
        Controls.Add(nudSpessoreBordo)
        Controls.Add(panelColore)
        Controls.Add(btnSelezionaColore)
        Controls.Add(colori)
        Controls.Add(btnOk)
        Controls.Add(Colonne)
        Controls.Add(Righe)
        Controls.Add(nudColonne)
        Controls.Add(nudRighe)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        Name = "FormTabella"
        Text = "Tabella"
        CType(nudRighe, ComponentModel.ISupportInitialize).EndInit()
        CType(nudColonne, ComponentModel.ISupportInitialize).EndInit()
        CType(nudSpessoreBordo, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents nudRighe As NumericUpDown
    Friend WithEvents nudColonne As NumericUpDown
    Friend WithEvents Righe As Label
    Friend WithEvents Colonne As Label
    Friend WithEvents btnOk As Button
    Friend WithEvents colori As Label
    Friend WithEvents btnSelezionaColore As Button
    Friend WithEvents panelColore As Panel
    Friend WithEvents nudSpessoreBordo As NumericUpDown
    Friend WithEvents Label1 As Label
End Class
