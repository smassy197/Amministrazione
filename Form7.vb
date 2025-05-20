Imports System.ComponentModel

Public Class FormTabella
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property NumeroRighe As Integer

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property NumeroColonne As Integer

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property ColoreBordo As Color = Color.Black ' Colore di default

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property SpessoreBordo As Integer = 1 ' Spessore di default

    Private Sub btnOk_Click(sender As Object, e As EventArgs) Handles btnOk.Click
        NumeroRighe = CInt(nudRighe.Value)
        NumeroColonne = CInt(nudColonne.Value)
        SpessoreBordo = CInt(nudSpessoreBordo.Value) ' Ottieni lo spessore del bordo
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub btnSelezionaColore_Click(sender As Object, e As EventArgs) Handles btnSelezionaColore.Click
        Dim colorDialog As New ColorDialog()
        If colorDialog.ShowDialog() = DialogResult.OK Then
            ColoreBordo = colorDialog.Color
            panelColore.BackColor = ColoreBordo ' Aggiorna il colore della Panel
        End If
    End Sub

    Private Sub FormTabella_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub


End Class



