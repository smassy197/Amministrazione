Imports System.ComponentModel

Public Class SetDeadlineForm
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property Deadline As DateTime?

    Private Sub SetDeadlineForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Deadline.HasValue AndAlso Deadline.Value >= DateTimePickerDeadline.MinDate AndAlso Deadline.Value <= DateTimePickerDeadline.MaxDate Then
            DateTimePickerDeadline.Value = Deadline.Value
            DateTimePickerDeadline.Checked = True
        Else
            DateTimePickerDeadline.Value = DateTime.Now ' Imposta una data valida predefinita
            DateTimePickerDeadline.Checked = False
        End If
    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs) Handles BtnSave.Click
        If DateTimePickerDeadline.Checked Then
            Deadline = DateTimePickerDeadline.Value
        Else
            Deadline = Nothing
        End If
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub BtnRemoveDeadline_Click(sender As Object, e As EventArgs) Handles BtnRemoveDeadline.Click
        Deadline = Nothing
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub
End Class
