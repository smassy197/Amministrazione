<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form2
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form2))
        btnBackupDatabase = New Button()
        btnCaricaDatabase = New Button()
        btnSaveChanges = New Button()
        dgvSqliteSequence = New DataGridView()
        Panel1 = New Panel()
        btnCaricaConfigurazione = New Button()
        btnBackupConfigurazione = New Button()
        Label1 = New Label()
        Panel2 = New Panel()
        Label2 = New Label()
        Panel3 = New Panel()
        RichTextBoxLog = New RichTextBox()
        Label3 = New Label()
        CType(dgvSqliteSequence, ComponentModel.ISupportInitialize).BeginInit()
        Panel1.SuspendLayout()
        Panel2.SuspendLayout()
        Panel3.SuspendLayout()
        SuspendLayout()
        ' 
        ' btnBackupDatabase
        ' 
        btnBackupDatabase.Location = New Point(16, 75)
        btnBackupDatabase.Name = "btnBackupDatabase"
        btnBackupDatabase.Size = New Size(109, 51)
        btnBackupDatabase.TabIndex = 0
        btnBackupDatabase.Text = "Backup Database"
        btnBackupDatabase.UseVisualStyleBackColor = True
        ' 
        ' btnCaricaDatabase
        ' 
        btnCaricaDatabase.Location = New Point(131, 75)
        btnCaricaDatabase.Name = "btnCaricaDatabase"
        btnCaricaDatabase.Size = New Size(109, 51)
        btnCaricaDatabase.TabIndex = 1
        btnCaricaDatabase.Text = "Carica Database"
        btnCaricaDatabase.UseVisualStyleBackColor = True
        ' 
        ' btnSaveChanges
        ' 
        btnSaveChanges.Location = New Point(445, 109)
        btnSaveChanges.Name = "btnSaveChanges"
        btnSaveChanges.Size = New Size(94, 87)
        btnSaveChanges.TabIndex = 2
        btnSaveChanges.Text = "Save"
        btnSaveChanges.UseVisualStyleBackColor = True
        ' 
        ' dgvSqliteSequence
        ' 
        dgvSqliteSequence.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        dgvSqliteSequence.Location = New Point(3, 23)
        dgvSqliteSequence.Name = "dgvSqliteSequence"
        dgvSqliteSequence.RowHeadersWidth = 51
        dgvSqliteSequence.Size = New Size(436, 173)
        dgvSqliteSequence.TabIndex = 3
        ' 
        ' Panel1
        ' 
        Panel1.BorderStyle = BorderStyle.FixedSingle
        Panel1.Controls.Add(btnCaricaConfigurazione)
        Panel1.Controls.Add(btnBackupConfigurazione)
        Panel1.Controls.Add(Label1)
        Panel1.Controls.Add(btnCaricaDatabase)
        Panel1.Controls.Add(btnBackupDatabase)
        Panel1.Location = New Point(12, 12)
        Panel1.Name = "Panel1"
        Panel1.Size = New Size(262, 201)
        Panel1.TabIndex = 4
        ' 
        ' btnCaricaConfigurazione
        ' 
        btnCaricaConfigurazione.Location = New Point(131, 136)
        btnCaricaConfigurazione.Name = "btnCaricaConfigurazione"
        btnCaricaConfigurazione.Size = New Size(109, 51)
        btnCaricaConfigurazione.TabIndex = 9
        btnCaricaConfigurazione.Text = "Carica Configurazione"
        btnCaricaConfigurazione.UseVisualStyleBackColor = True
        ' 
        ' btnBackupConfigurazione
        ' 
        btnBackupConfigurazione.Location = New Point(16, 136)
        btnBackupConfigurazione.Name = "btnBackupConfigurazione"
        btnBackupConfigurazione.Size = New Size(109, 51)
        btnBackupConfigurazione.TabIndex = 8
        btnBackupConfigurazione.Text = "Backup Configurazione"
        btnBackupConfigurazione.UseVisualStyleBackColor = True
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(3, 0)
        Label1.Name = "Label1"
        Label1.Size = New Size(135, 20)
        Label1.TabIndex = 7
        Label1.Text = "Modifica Database"
        ' 
        ' Panel2
        ' 
        Panel2.BorderStyle = BorderStyle.FixedSingle
        Panel2.Controls.Add(Label2)
        Panel2.Controls.Add(dgvSqliteSequence)
        Panel2.Controls.Add(btnSaveChanges)
        Panel2.Location = New Point(280, 12)
        Panel2.Name = "Panel2"
        Panel2.Size = New Size(546, 201)
        Panel2.TabIndex = 5
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(3, 0)
        Label2.Name = "Label2"
        Label2.Size = New Size(182, 20)
        Label2.TabIndex = 8
        Label2.Text = "Modifia numero sequenze"
        ' 
        ' Panel3
        ' 
        Panel3.BorderStyle = BorderStyle.FixedSingle
        Panel3.Controls.Add(RichTextBoxLog)
        Panel3.Controls.Add(Label3)
        Panel3.Location = New Point(12, 219)
        Panel3.Name = "Panel3"
        Panel3.Size = New Size(814, 236)
        Panel3.TabIndex = 6
        ' 
        ' RichTextBoxLog
        ' 
        RichTextBoxLog.Location = New Point(3, 23)
        RichTextBoxLog.Name = "RichTextBoxLog"
        RichTextBoxLog.Size = New Size(804, 208)
        RichTextBoxLog.TabIndex = 10
        RichTextBoxLog.Text = ""
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Location = New Point(3, 0)
        Label3.Name = "Label3"
        Label3.Size = New Size(134, 20)
        Label3.TabIndex = 9
        Label3.Text = "Log dei documenti"
        ' 
        ' Form2
        ' 
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(838, 464)
        Controls.Add(Panel3)
        Controls.Add(Panel2)
        Controls.Add(Panel1)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximizeBox = False
        Name = "Form2"
        Text = "Manutenzione"
        CType(dgvSqliteSequence, ComponentModel.ISupportInitialize).EndInit()
        Panel1.ResumeLayout(False)
        Panel1.PerformLayout()
        Panel2.ResumeLayout(False)
        Panel2.PerformLayout()
        Panel3.ResumeLayout(False)
        Panel3.PerformLayout()
        ResumeLayout(False)
    End Sub

    Friend WithEvents btnBackupDatabase As Button
    Friend WithEvents btnCaricaDatabase As Button
    Friend WithEvents btnSaveChanges As Button
    Friend WithEvents dgvSqliteSequence As DataGridView
    Friend WithEvents Panel1 As Panel
    Friend WithEvents Panel2 As Panel
    Friend WithEvents Panel3 As Panel
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents RichTextBoxLog As RichTextBox
    Friend WithEvents btnBackupConfigurazione As Button
    Friend WithEvents btnCaricaConfigurazione As Button
End Class
