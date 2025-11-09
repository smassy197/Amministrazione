Imports System.Drawing
Imports Microsoft.Web.WebView2.WinForms

Public Class FormAnteprimaDocumento
    Private percorso As String

    Public Sub New(percorso As String)
        Console.WriteLine("[DEBUG] Inizializzazione FormAnteprimaDocumento. Percorso: " & percorso)

        InitializeComponent()
        Me.percorso = percorso
        Me.Text = "Anteprima documento"
        Me.Width = 800
        Me.Height = 600

        MostraAnteprima()
    End Sub

    Private Sub MostraAnteprima()
        Dim ext As String = IO.Path.GetExtension(percorso).ToLower()
        Console.WriteLine("[DEBUG] Estensione file: " & ext)

        If ext = ".pdf" Then
            Console.WriteLine("[DEBUG] Anteprima PDF.")
            Try
                Dim webView As New Microsoft.Web.WebView2.WinForms.WebView2()
                webView.Dock = DockStyle.Fill
                webView.Source = New Uri(percorso)
                Me.Controls.Add(webView)
            Catch ex As Exception
                Console.WriteLine("[DEBUG] Errore anteprima PDF: " & ex.Message)
                Dim lbl As New Label()
                lbl.Text = "Errore anteprima PDF: " & ex.Message
                lbl.Dock = DockStyle.Fill
                lbl.TextAlign = ContentAlignment.MiddleCenter
                Me.Controls.Add(lbl)
            End Try

        ElseIf ext = ".jpg" OrElse ext = ".jpeg" OrElse ext = ".png" OrElse ext = ".bmp" Then
            Console.WriteLine("[DEBUG] Anteprima immagine.")
            Try
                Dim pb As New PictureBox()
                pb.Dock = DockStyle.Fill
                pb.SizeMode = PictureBoxSizeMode.Zoom
                pb.Image = Image.FromFile(percorso)
                Me.Controls.Add(pb)
            Catch ex As Exception
                Console.WriteLine("[DEBUG] Errore anteprima immagine: " & ex.Message)
                Dim lbl As New Label()
                lbl.Text = "Errore anteprima immagine: " & ex.Message
                lbl.Dock = DockStyle.Fill
                lbl.TextAlign = ContentAlignment.MiddleCenter
                Me.Controls.Add(lbl)
            End Try

        ElseIf ext = ".txt" Then
            Console.WriteLine("[DEBUG] Anteprima file di testo.")
            Try
                Dim tb As New TextBox()
                tb.Multiline = True
                tb.ReadOnly = True
                tb.Dock = DockStyle.Fill
                tb.Text = IO.File.ReadAllText(percorso)
                Me.Controls.Add(tb)
            Catch ex As Exception
                Console.WriteLine("[DEBUG] Errore anteprima testo: " & ex.Message)
                Dim lbl As New Label()
                lbl.Text = "Errore anteprima testo: " & ex.Message
                lbl.Dock = DockStyle.Fill
                lbl.TextAlign = ContentAlignment.MiddleCenter
                Me.Controls.Add(lbl)
            End Try

        Else
            Console.WriteLine("[DEBUG] Nessuna anteprima disponibile per: " & ext)
            Dim lbl As New Label()
            lbl.Text = "Nessuna anteprima disponibile"
            lbl.Dock = DockStyle.Fill
            lbl.TextAlign = ContentAlignment.MiddleCenter
            Me.Controls.Add(lbl)
        End If
    End Sub


    Private Sub FormAnteprimaDocumento_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class