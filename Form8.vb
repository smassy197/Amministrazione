Imports System.ComponentModel

Public Class SetDeadlineForm
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property Deadline As DateTime?

    Private Sub SetDeadlineForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Console.WriteLine("[DEBUG] Caricamento SetDeadlineForm. Deadline iniziale: " & If(Deadline.HasValue, Deadline.Value.ToString("yyyy-MM-dd"), "null"))

        If Deadline.HasValue AndAlso Deadline.Value >= DateTimePickerDeadline.MinDate AndAlso Deadline.Value <= DateTimePickerDeadline.MaxDate Then
            DateTimePickerDeadline.Value = Deadline.Value
            DateTimePickerDeadline.Checked = True
            Console.WriteLine("[DEBUG] Deadline impostata su DateTimePicker: " & Deadline.Value.ToString("yyyy-MM-dd"))
        Else
            DateTimePickerDeadline.Value = DateTime.Now
            DateTimePickerDeadline.Checked = False
            Console.WriteLine("[DEBUG] Deadline non valida o assente. Impostata data corrente.")
        End If
    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs) Handles BtnSave.Click
        If DateTimePickerDeadline.Checked Then
            Deadline = DateTimePickerDeadline.Value
            Console.WriteLine("[DEBUG] Deadline salvata: " & Deadline.Value.ToString("yyyy-MM-dd"))
        Else
            Deadline = Nothing
            Console.WriteLine("[DEBUG] Deadline disattivata.")
        End If

        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub BtnRemoveDeadline_Click(sender As Object, e As EventArgs) Handles BtnRemoveDeadline.Click
        Deadline = Nothing
        Console.WriteLine("[DEBUG] Deadline rimossa manualmente.")
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub
End Class
