Imports System.ComponentModel

Public Class FormTabella
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property NumeroRighe As Integer

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property NumeroColonne As Integer

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property ColoreBordo As Color = Color.Black

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property SpessoreBordo As Integer = 1

    Private Sub FormTabella_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Console.WriteLine("[DEBUG] FormTabella caricato.")
    End Sub

    Private Sub btnOk_Click(sender As Object, e As EventArgs) Handles btnOk.Click
        NumeroRighe = CInt(nudRighe.Value)
        NumeroColonne = CInt(nudColonne.Value)
        SpessoreBordo = CInt(nudSpessoreBordo.Value)

        Console.WriteLine($"[DEBUG] btnOk_Click → Righe: {NumeroRighe}, Colonne: {NumeroColonne}, Spessore: {SpessoreBordo}")

        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub btnSelezionaColore_Click(sender As Object, e As EventArgs) Handles btnSelezionaColore.Click
        Dim colorDialog As New ColorDialog()
        If colorDialog.ShowDialog() = DialogResult.OK Then
            ColoreBordo = colorDialog.Color
            panelColore.BackColor = ColoreBordo
            Console.WriteLine("[DEBUG] Colore bordo selezionato: " & ColoreBordo.ToString())
        Else
            Console.WriteLine("[DEBUG] Selezione colore annullata.")
        End If
    End Sub
End Class




