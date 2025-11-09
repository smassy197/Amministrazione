Imports System.ComponentModel
Imports System.IO
Imports System.Text
Imports Microsoft.Data.Sqlite
Imports DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing
Imports DocumentFormat.OpenXml.Packaging
Imports System.Runtime.InteropServices

Public Class Form5
    Private WithEvents fileToolStripMenuItem As ToolStripMenuItem
    Private WithEvents modificaToolStripMenuItem As ToolStripMenuItem
    Private WithEvents nuovoToolStripMenuItem As ToolStripMenuItem
    Private WithEvents apriToolStripMenuItem As ToolStripMenuItem
    Private WithEvents salvaConNomeToolStripMenuItem As ToolStripMenuItem
    Private WithEvents salvaToolStripMenuItem As ToolStripMenuItem
    Private WithEvents inserisciToolStripMenuItem As ToolStripMenuItem
    Private WithEvents importaDatiToolStripMenuItem As ToolStripMenuItem
    Private WithEvents daEsternoToolStripMenuItem As ToolStripMenuItem
    Private WithEvents daDatabaseToolStripMenuItem As ToolStripMenuItem
    Private WithEvents contextMenu As New ContextMenuStrip()
    Private WithEvents toolStripMenuItemIncolla As New ToolStripMenuItem()



    ' Path to your SQLite database file
    Dim databasePath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "amministrazione.db")
    Dim connString As String = "Data Source=" & databasePath

    ' Variabile per tenere traccia del percorso del file aperto
    Private currentFilePath As String = String.Empty
    'Private currentFolderPath As String





    Private Sub Form5_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' ApriConsole()
        InitializeComponents()
        InitializeFileList()
        richTextBoxDocument.DetectUrls = True ' Abilita il rilevamento dei link

    End Sub




    Private Sub InitializeComponents()
        ' Configurazione dei menu
        Me.fileToolStripMenuItem = New ToolStripMenuItem()
        Me.modificaToolStripMenuItem = New ToolStripMenuItem()
        Me.inserisciToolStripMenuItem = New ToolStripMenuItem()
        Me.nuovoToolStripMenuItem = New ToolStripMenuItem()
        Me.apriToolStripMenuItem = New ToolStripMenuItem()
        Me.salvaConNomeToolStripMenuItem = New ToolStripMenuItem()
        Me.salvaToolStripMenuItem = New ToolStripMenuItem()


        contextMenu = New ContextMenuStrip()
        toolStripMenuItemIncolla = New ToolStripMenuItem("Incolla")

        ' Aggiungi l'elemento Incolla al menu contestuale
        contextMenu.Items.Add(toolStripMenuItemIncolla)

        ' Configurazione menu "File"
        Me.fileToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {Me.nuovoToolStripMenuItem, Me.apriToolStripMenuItem, Me.salvaToolStripMenuItem, Me.salvaConNomeToolStripMenuItem})
        Me.fileToolStripMenuItem.Name = "fileToolStripMenuItem"
        Me.fileToolStripMenuItem.Size = New Size(37, 20)
        Me.fileToolStripMenuItem.Text = "File"

        ' Configurazione menu "Modifica"
        Me.modificaToolStripMenuItem.DropDownItems.AddRange(New ToolStripItem() {Me.toolStripMenuItemIncolla})
        Me.modificaToolStripMenuItem.Name = "modificaToolStripMenuItem"
        Me.modificaToolStripMenuItem.Size = New Size(60, 20)
        Me.modificaToolStripMenuItem.Text = "Modifica"

        ' Configurazione menu "Inserisci"
        Me.inserisciToolStripMenuItem.Name = "inserisciToolStripMenuItem"
        Me.inserisciToolStripMenuItem.Size = New Size(60, 20)
        Me.inserisciToolStripMenuItem.Text = "Inserisci"

        ' Creazione della voce di menu per l'elenco puntato
        Dim elencoPuntatoToolStripMenuItem As New ToolStripMenuItem()
        elencoPuntatoToolStripMenuItem.Name = "elencoPuntatoToolStripMenuItem"
        elencoPuntatoToolStripMenuItem.Text = "Elenco Puntato"
        AddHandler elencoPuntatoToolStripMenuItem.Click, AddressOf Me.elencoPuntatoToolStripMenuItem_Click

        ' Aggiungi la voce di menu al menu "Inserisci"
        Me.inserisciToolStripMenuItem.DropDownItems.Add(elencoPuntatoToolStripMenuItem)

        ' Configurazione voce "Nuovo"
        Me.nuovoToolStripMenuItem.Name = "nuovoToolStripMenuItem"
        Me.nuovoToolStripMenuItem.Size = New Size(152, 22)
        Me.nuovoToolStripMenuItem.Text = "Nuovo"
        AddHandler Me.nuovoToolStripMenuItem.Click, AddressOf Me.nuovoToolStripMenuItem_Click

        ' Configurazione voce "Apri"
        Me.apriToolStripMenuItem.Name = "apriToolStripMenuItem"
        Me.apriToolStripMenuItem.Size = New Size(152, 22)
        Me.apriToolStripMenuItem.Text = "Apri"
        AddHandler Me.apriToolStripMenuItem.Click, AddressOf Me.apriToolStripMenuItem_Click

        ' Configurazione voce "Salva"
        Me.salvaToolStripMenuItem.Name = "salvaToolStripMenuItem"
        Me.salvaToolStripMenuItem.Size = New Size(152, 22)
        Me.salvaToolStripMenuItem.Text = "Salva"
        AddHandler Me.salvaToolStripMenuItem.Click, AddressOf Me.salvaToolStripMenuItem_Click

        ' Configurazione voce "Salva con nome"
        Me.salvaConNomeToolStripMenuItem.Name = "salvaConNomeToolStripMenuItem"
        Me.salvaConNomeToolStripMenuItem.Size = New Size(152, 22)
        Me.salvaConNomeToolStripMenuItem.Text = "Salva con nome"
        AddHandler Me.salvaConNomeToolStripMenuItem.Click, AddressOf Me.salvaConNomeToolStripMenuItem_Click

        ' Aggiungi i menu alla barra dei menu principale
        Me.menuStrip1.Items.AddRange(New ToolStripItem() {Me.fileToolStripMenuItem, Me.inserisciToolStripMenuItem, Me.modificaToolStripMenuItem})
    End Sub

    Private Sub InitializeFileList()

        ' Imposta il percorso alla cartella dell'applicazione
        currentFilePath = Path.GetDirectoryName(databasePath)

        ' Verifica se la directory esiste; se non esiste, la crea
        If Not Directory.Exists(currentFilePath) Then
            Directory.CreateDirectory(currentFilePath)
        End If

        ' Carica i file e le cartelle dalla cartella corrente
        LoadFileList(currentFilePath)
    End Sub



    Private Sub LoadFileList(folderPath As String)
        ' Aggiorna il percorso della cartella corrente
        currentFilePath = folderPath

        ' Ottieni le cartelle
        Dim directories As String() = Directory.GetDirectories(folderPath)
        ' Ottieni i file
        Dim files As String() = Directory.GetFiles(folderPath, "*.*").Where(Function(f) f.EndsWith(".txt") OrElse f.EndsWith(".rtf")).ToArray()

        listBoxFiles.Items.Clear()

        ' Aggiungi la possibilità di tornare indietro
        If folderPath <> Path.GetPathRoot(folderPath) Then
            listBoxFiles.Items.Add("..") ' Rappresenta la cartella superiore
        End If

        ' Aggiungi le cartelle
        For Each dir As String In directories
            listBoxFiles.Items.Add(New IO.DirectoryInfo(dir).Name)
        Next

        ' Aggiungi i file
        For Each file As String In files
            listBoxFiles.Items.Add(Path.GetFileName(file))
        Next
    End Sub

    Private Sub listBoxFiles_SelectedIndexChanged(sender As Object, e As EventArgs) Handles listBoxFiles.SelectedIndexChanged
        ' Controlla se un file è stato selezionato nella ListBox
        If listBoxFiles.SelectedItem IsNot Nothing Then
            ' Ottieni il percorso completo della cartella in cui si trovano i file
            Dim fileName As String = listBoxFiles.SelectedItem.ToString()
            Dim filePath As String = Path.Combine(currentFilePath, fileName)

            ' Visualizza il percorso completo nella TextBox
            textBoxFilePath.Text = filePath
        Else
            textBoxFilePath.Text = String.Empty ' Pulisci la TextBox se nulla è selezionato
        End If
    End Sub



    Private Sub listBoxFiles_DoubleClick(sender As Object, e As EventArgs) Handles listBoxFiles.DoubleClick
        If listBoxFiles.SelectedItem IsNot Nothing Then
            Dim selectedItem As String = listBoxFiles.SelectedItem.ToString()

            ' Controlla se l'elemento selezionato è ".." per tornare indietro
            If selectedItem = ".." Then
                ' Controlla se currentFilePath non è vuoto
                If Not String.IsNullOrEmpty(currentFilePath) Then
                    Dim parentDirectory As DirectoryInfo = Directory.GetParent(currentFilePath)
                    If parentDirectory IsNot Nothing Then
                        LoadFileList(parentDirectory.FullName)
                    Else
                        MessageBox.Show("Non è possibile tornare alla cartella superiore.")
                    End If
                Else
                    MessageBox.Show("Il percorso corrente non è valido.")
                End If
            Else
                ' Controlla se è una cartella
                Dim selectedFolderPath As String = Path.Combine(currentFilePath, selectedItem)
                If Directory.Exists(selectedFolderPath) Then
                    LoadFileList(selectedFolderPath) ' Carica i file della cartella selezionata
                End If
            End If
        End If
    End Sub


    ' Gestore per la creazione di un nuovo documento
    Private Sub nuovoToolStripMenuItem_Click(sender As Object, e As EventArgs)
        richTextBoxDocument.Clear()
        currentFilePath = String.Empty
    End Sub
    Private Sub InitializeRichBox()
        richTextBoxDocument.Clear()
        currentFilePath = String.Empty
    End Sub


    ' Gestore per aprire un file di testo
    Private Sub apriToolStripMenuItem_Click(sender As Object, e As EventArgs)
        If richTextBoxDocument.Modified Then ' Controlla se ci sono modifiche non salvate
            Dim result As DialogResult = MessageBox.Show("Hai delle modifiche non salvate. Vuoi salvare prima di caricare un nuovo file?", "Salva file", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning)

            If result = DialogResult.Yes Then
                ' Chiama il metodo di salvataggio
                buttonSave_Click(sender, e)
            ElseIf result = DialogResult.Cancel Then
                ' Annulla il caricamento del nuovo file
                Return
            End If
        End If

        ' Carica il nuovo file come prima
        Using openFileDialog As New OpenFileDialog()
            openFileDialog.Filter = "File di testo (*.txt)|*.txt|File RTF (*.rtf)|*.rtf|Tutti i file (*.*)|*.*"

            If openFileDialog.ShowDialog() = DialogResult.OK Then
                currentFilePath = openFileDialog.FileName
                richTextBoxDocument.Clear()

                Try
                    If currentFilePath.EndsWith(".rtf") Then
                        ' Carica il file RTF nella RichTextBox mantenendo la formattazione
                        richTextBoxDocument.LoadFile(currentFilePath, RichTextBoxStreamType.RichText)
                    ElseIf currentFilePath.EndsWith(".txt") Then
                        ' Carica il file di testo nella RichTextBox
                        richTextBoxDocument.Text = File.ReadAllText(currentFilePath)
                    Else
                        MessageBox.Show("Tipo di file non supportato.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                Catch ex As Exception
                    MessageBox.Show("Errore durante l'apertura del file: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        End Using
    End Sub

    ' Gestore per salvare il file con un nuovo nome
    ' Gestore per il salvataggio del file corrente
    Private Sub salvaToolStripMenuItem_Click(sender As Object, e As EventArgs)
        If String.IsNullOrEmpty(currentFilePath) Then
            ' Se non c'è un file esistente, mostra la finestra di dialogo "Salva con nome"
            salvaConNomeToolStripMenuItem_Click(sender, e)
        Else
            ' Altrimenti, salva direttamente il file esistente
            SaveFile(currentFilePath)
        End If
    End Sub

    Private Sub salvaConNomeToolStripMenuItem_Click(sender As Object, e As EventArgs)
        ' Mostra la finestra di dialogo "Salva con nome"
        Using saveFileDialog As New SaveFileDialog()
            saveFileDialog.Filter = "File di testo (*.txt)|*.txt|File RTF (*.rtf)|*.rtf|Tutti i file (*.*)|*.*"

            If saveFileDialog.ShowDialog() = DialogResult.OK Then
                currentFilePath = saveFileDialog.FileName

                Try
                    SaveFile(currentFilePath)
                Catch ex As Exception
                    MessageBox.Show("Errore durante il salvataggio del file: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        End Using
    End Sub



    ' Gestore per il menu contestuale "Incolla"
    Private Sub toolStripMenuItemIncolla_Click(sender As Object, e As EventArgs) Handles toolStripMenuItemIncolla.Click
        If Clipboard.ContainsText() Then
            richTextBoxDocument.Paste()
        End If
    End Sub

    Private Sub elencoPuntatoToolStripMenuItem_Click(sender As Object, e As EventArgs)
        ' Chiede all'utente di scegliere il tipo di elenco
        Dim scelta As String = InputBox("Scegli il tipo di elenco: 1 = Punto, 2 = Numerato, 3 = Lettere", "Seleziona Elenco")

        ' Determina il simbolo in base alla scelta
        Dim simboloElenco As String = ""
        Select Case scelta
            Case "1"
                simboloElenco = "•"
            Case "2"
                simboloElenco = "1."
            Case "3"
                simboloElenco = "a."
            Case Else
                simboloElenco = "•"
        End Select

        ' Inserisce il simbolo scelto all'inizio di ogni riga
        richTextBoxDocument.SelectedText = simboloElenco & " " & richTextBoxDocument.SelectedText.Replace(vbCrLf, vbCrLf & simboloElenco & " ")
    End Sub


    Private Sub richTextBoxDocument_KeyDown(sender As Object, e As KeyEventArgs) Handles richTextBoxDocument.KeyDown
        ' Verifica se è stato premuto "Invio"
        If e.KeyCode = Keys.Enter Then
            ' Ottiene la riga corrente e il testo della riga
            Dim currentLine As Integer = richTextBoxDocument.GetLineFromCharIndex(richTextBoxDocument.SelectionStart)
            Dim lineText As String = richTextBoxDocument.Lines(currentLine).Trim()

            ' Variabili per simbolo e prossimo valore dell'elenco
            Dim nextSymbol As String = ""
            Dim match As System.Text.RegularExpressions.Match

            ' Gestisce l'elenco puntato
            If lineText.StartsWith("• ") Then
                nextSymbol = "• "

                ' Gestisce l'elenco numerato (es. "1. ")
            ElseIf System.Text.RegularExpressions.Regex.IsMatch(lineText, "^\d+\.\s") Then
                match = System.Text.RegularExpressions.Regex.Match(lineText, "^(\d+)\.")
                If match.Success Then
                    Dim currentNumber As Integer = Integer.Parse(match.Groups(1).Value)
                    nextSymbol = (currentNumber + 1).ToString() & ". "
                End If

                ' Gestisce l'elenco alfabetico (es. "a. ")
            ElseIf System.Text.RegularExpressions.Regex.IsMatch(lineText, "^[a-zA-Z]\.\s") Then
                match = System.Text.RegularExpressions.Regex.Match(lineText, "^([a-zA-Z])\.")
                If match.Success Then
                    Dim currentLetter As Char = Char.Parse(match.Groups(1).Value)
                    nextSymbol = Chr(Asc(currentLetter) + 1) & ". "
                End If
            End If

            ' Aggiunge il simbolo dell'elenco alla nuova riga se esiste un simbolo definito
            If Not String.IsNullOrEmpty(nextSymbol) Then
                richTextBoxDocument.SelectedText = vbCrLf & nextSymbol
                e.Handled = True ' Blocca l'Invio di default
            End If
        End If
    End Sub

    Private Sub buttonLoadFile_Click(sender As Object, e As EventArgs) Handles buttonLoadFile.Click
        If richTextBoxDocument.Modified Then ' Controlla se ci sono modifiche non salvate
            Dim result As DialogResult = MessageBox.Show("Hai delle modifiche non salvate. Vuoi salvare prima di caricare un nuovo file?", "Salva file", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning)

            If result = DialogResult.Yes Then
                ' Chiama il metodo di salvataggio
                buttonSave_Click(sender, e)
            ElseIf result = DialogResult.Cancel Then
                ' Annulla il caricamento del nuovo file
                Return
            End If
        End If

        ' Carica il nuovo file come prima
        Dim openFileDialog As New OpenFileDialog()
        openFileDialog.Filter = "File di testo (*.txt)|*.txt|File RTF (*.rtf)|*.rtf|Tutti i file (*.*)|*.*"

        If openFileDialog.ShowDialog() = DialogResult.OK Then
            currentFilePath = openFileDialog.FileName
            richTextBoxDocument.Clear()

            If currentFilePath.EndsWith(".rtf") Then
                ' Carica il file RTF nella RichTextBox mantenendo la formattazione
                richTextBoxDocument.LoadFile(currentFilePath, RichTextBoxStreamType.RichText)
            ElseIf currentFilePath.EndsWith(".txt") Then
                ' Carica il file di testo nella RichTextBox
                richTextBoxDocument.Text = File.ReadAllText(currentFilePath)
            Else
                MessageBox.Show("Tipo di file non supportato.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
        End If
    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If richTextBoxDocument.Modified Then ' Controlla se ci sono modifiche non salvate
            Dim result As DialogResult = MessageBox.Show("Hai delle modifiche non salvate. Vuoi salvare prima di uscire?", "Salva file", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning)

            If result = DialogResult.Yes Then
                ' Chiama il metodo di salvataggio
                buttonSave_Click(sender, e)
            ElseIf result = DialogResult.Cancel Then
                ' Annulla la chiusura della finestra
                e.Cancel = True
            End If
        End If
    End Sub


    Private Sub buttonDocuments_Click(sender As Object, e As EventArgs) Handles buttonDocuments.Click
        ' Controlla se un file è stato selezionato nella ListBoxFiles
        If listBoxFiles.SelectedItem IsNot Nothing Then
            ' Ottieni il nome del file selezionato
            Dim fileName As String = listBoxFiles.SelectedItem.ToString()

            ' Usa il percorso completo dalla TextBox
            Dim filePath As String = textBoxFilePath.Text ' Assicurati che questa TextBox contenga il percorso corretto

            ' Controlla se il file esiste
            If Not File.Exists(filePath) Then
                MessageBox.Show("Il file non esiste: " & filePath, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Try
                ' Controlla l'estensione del file per determinare come aprirlo
                If filePath.EndsWith(".rtf") Then
                    ' Carica il contenuto del file RTF nella RichTextBox
                    richTextBoxDocument.LoadFile(filePath, RichTextBoxStreamType.RichText)
                ElseIf filePath.EndsWith(".txt") Then
                    ' Carica il contenuto del file di testo nella RichTextBox
                    richTextBoxDocument.Text = IO.File.ReadAllText(filePath)
                Else
                    MessageBox.Show("Tipo di file non supportato.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
            Catch ex As Exception
                MessageBox.Show("Errore durante l'apertura del file: " & ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        Else
            MessageBox.Show("Seleziona un file dalla lista prima di continuare.", "Nessun file selezionato", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    ' Metodo per salvare il contenuto della RichTextBox in un file
    Private Sub SaveFile(filePath As String)
        Try
            ' Controlla se la RichTextBox è completamente vuota
            If String.IsNullOrWhiteSpace(richTextBoxDocument.Text) Then
                MessageBox.Show("La RichTextBox è vuota. Si prega di selezionare un percorso per il file.")

                ' Crea una finestra di dialogo per il salvataggio del file
                Using saveFileDialog As New SaveFileDialog()
                    saveFileDialog.Filter = "File di testo (*.txt)|*.txt|Documento RTF (*.rtf)|*.rtf"
                    saveFileDialog.Title = "Salva File"

                    ' Mostra la finestra di dialogo e controlla se l'utente ha premuto OK
                    If saveFileDialog.ShowDialog() = DialogResult.OK Then
                        filePath = saveFileDialog.FileName
                    Else
                        MessageBox.Show("Salvataggio annullato.")
                        Return ' Esci dalla subroutine se l'utente annulla
                    End If
                End Using
            End If

            ' Controlla se il percorso contiene un'estensione di file
            ' Se non c'è, usa il percorso del file visualizzato nella textBoxFilePath
            If Not filePath.EndsWith(".txt") AndAlso Not filePath.EndsWith(".rtf") Then
                filePath = textBoxFilePath.Text ' Usa il percorso completo dalla textBoxFilePath
                Console.WriteLine("Percorso del file aggiornato: " & filePath)
            End If

            ' Controlla l'estensione del file per determinare come salvarlo
            If filePath.EndsWith(".rtf") Then
                ' Salva il contenuto come file RTF
                Console.WriteLine("Salvataggio come documento RTF.")
                ' Salva il contenuto della RichTextBox in formato RTF
                richTextBoxDocument.SaveFile(filePath, RichTextBoxStreamType.RichText)
            ElseIf filePath.EndsWith(".txt") Then
                ' Salva il contenuto come file di testo
                Console.WriteLine("Salvataggio come file di testo.")
                File.WriteAllText(filePath, richTextBoxDocument.Text)
            Else
                Console.WriteLine("Tipo di file non supportato.")
            End If

            ' Imposta Modified a False dopo il salvataggio
            richTextBoxDocument.Modified = False
            Console.WriteLine("File salvato con successo.")
            MessageBox.Show("File salvato con successo.")

        Catch ex As Exception
            ' Mostra un messaggio di errore se il salvataggio fallisce
            Console.WriteLine("Errore durante il salvataggio del file: " & ex.Message)
            MessageBox.Show("Errore durante il salvataggio del file: " & ex.Message)
        End Try

        ' LoadFileList(currentFilePath)
    End Sub


    ' Gestore per il salvataggio del file corrente
    Private Sub buttonSave_Click(sender As Object, e As EventArgs) Handles buttonSave.Click
        ' Controlla se il file corrente ha un percorso associato (cioè se è un file esistente)
        If String.IsNullOrEmpty(currentFilePath) Then
            ' Se il file è nuovo, mostra la finestra di dialogo "Salva con nome"
            salvaConNomeToolStripMenuItem_Click(sender, e)
        Else
            ' Se il file esiste già, salvalo direttamente
            SaveFile(currentFilePath)
        End If
    End Sub

    ' Aggiungi questo codice all'interno della tua classe Form5
    <DllImport("kernel32.dll")>
    Private Shared Function AllocConsole() As Boolean
    End Function

    Private Sub ApriConsole()
        AllocConsole()
    End Sub

    Private Sub buttonDeleteFile_Click(sender As Object, e As EventArgs) Handles buttonDeleteFile.Click
        Dim filePath As String = textBoxFilePath.Text

        If File.Exists(filePath) Then
            Dim result As DialogResult = MessageBox.Show("Sei sicuro di voler eliminare il file?", "Conferma Eliminazione", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

            If result = DialogResult.Yes Then
                Try
                    File.Delete(filePath)
                    Console.WriteLine("File eliminato con successo: " & filePath)
                    MessageBox.Show("File eliminato con successo: " & filePath)

                Catch ex As Exception
                    Console.WriteLine("Errore durante l'eliminazione del file: " & ex.Message)
                    MessageBox.Show("Errore durante il salvataggio del file: " & ex.Message)
                End Try
            End If
        Else
            Console.WriteLine("File non trovato: " & filePath)
            Console.WriteLine("File non trovato: " & filePath)
        End If
        ' LoadFileList(currentFilePath)
        ' richTextBoxDocument.Clear()
    End Sub

    Private Sub btnForm1_Click(sender As Object, e As EventArgs) Handles btnForm1.Click
        Dim nuovaForm As New Form1
        nuovaForm.Show()
        Me.Close() ' Chiude la form corrente
    End Sub

    Private Sub bntForm3_Click(sender As Object, e As EventArgs) Handles bntForm3.Click
        Dim nuovaForm As New Form3
        nuovaForm.Show()
        Me.Close() ' Chiude la form corrente
    End Sub

    Private Sub buttonOpenDatabaseFolder_Click(sender As Object, e As EventArgs) Handles buttonOpenDatabaseFolder.Click
        ' Percorso della cartella del database
        Dim databaseFolderPath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione")

        ' Verifica se la cartella esiste
        If Directory.Exists(databaseFolderPath) Then
            ' Carica il contenuto della cartella nella ListBox
            LoadFileList(databaseFolderPath)
        Else
            MessageBox.Show("La cartella del database non esiste.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub btnVaiAlDesktop_Click(sender As Object, e As EventArgs) Handles btnVaiAlDesktop.Click
        Dim desktopPath As String = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
        LoadFileList(desktopPath)
    End Sub



    Private Sub buttonHighlight_Click(sender As Object, e As EventArgs) Handles buttonHighlight.Click
        ToggleHighlight()
    End Sub

    Private Sub ToggleHighlight()
        If richTextBoxDocument.SelectionLength > 0 Then
            Dim currentColor As Color = richTextBoxDocument.SelectionBackColor
            Dim newColor As Color = If(currentColor = Color.Yellow, Color.White, Color.Yellow)
            richTextBoxDocument.SelectionBackColor = newColor
        End If
    End Sub

    Private Sub buttonFont_Click(sender As Object, e As EventArgs) Handles buttonFont.Click
        Using fontDialog As New FontDialog()
            fontDialog.Font = richTextBoxDocument.SelectionFont ' Usa il font della selezione corrente
            fontDialog.ShowColor = True ' Abilita la scelta del colore

            If fontDialog.ShowDialog() = DialogResult.OK Then
                ' Applica font e colore solo alla selezione
                richTextBoxDocument.SelectionFont = fontDialog.Font
                richTextBoxDocument.SelectionColor = fontDialog.Color
            End If
        End Using
    End Sub

    Private Sub btnCreaTabella_Click(sender As Object, e As EventArgs) Handles btnCreaTabella.Click
        Using formTabella As New FormTabella()
            If formTabella.ShowDialog() = DialogResult.OK Then
                Dim righe As Integer = formTabella.NumeroRighe
                Dim colonne As Integer = formTabella.NumeroColonne
                Dim coloreBordo As Color = formTabella.ColoreBordo
                Dim spessoreBordo As Integer = formTabella.SpessoreBordo

                ' Inizializza testoCelle se non è ancora impostato
                Dim testoCelle As New List(Of List(Of String))
                For i As Integer = 0 To righe - 1
                    Dim riga As New List(Of String)
                    For j As Integer = 0 To colonne - 1
                        riga.Add("") ' Cella vuota di default
                    Next
                    testoCelle.Add(riga)
                Next

                ' Genera la tabella in RTF
                Dim rtfTable As String = CreaTabellaRTF(righe, colonne, coloreBordo, spessoreBordo, testoCelle)
                richTextBoxDocument.SelectedRtf = rtfTable
            Else
                MessageBox.Show("Inserisci un numero valido di righe e colonne.", "Input non valido")
            End If
        End Using
    End Sub

    Private Function CreaTabellaRTF(righe As Integer, colonne As Integer, coloreBordo As Color, spessoreBordo As Integer, testoCelle As List(Of List(Of String))) As String
        Dim rtfTable As New System.Text.StringBuilder()
        Dim spessoreInPunti As Integer = CInt(spessoreBordo * 72 / 96)

        ' Larghezza della pagina in twips (12240 twips = 8.5 pollici a 1440 twips/pollice)
        Dim pageWidth As Integer = 12240
        ' Margini in twips (2 cm = circa 1134 twips)
        Dim marginLeft As Integer = 1134 ' Margine sinistro
        Dim marginRight As Integer = 1134 ' Margine destro
        ' Calcola la larghezza disponibile per la tabella
        Dim availableWidth As Integer = pageWidth - marginLeft - marginRight
        ' Calcola la larghezza uniforme per ogni colonna
        Dim colWidth As Integer = availableWidth \ colonne

        ' Inizio documento RTF con tabella dei colori
        rtfTable.Append("{\rtf1\ansi\deff0{\colortbl ;\red" & coloreBordo.R & "\green" & coloreBordo.G & "\blue" & coloreBordo.B & ";}")

        ' Creazione delle righe e celle
        For i As Integer = 0 To righe - 1
            rtfTable.Append("\trowd\trgaph108") ' Inizio riga con gap tra celle
            For j As Integer = 0 To colonne - 1
                Dim colPos As Integer = (j + 1) * colWidth ' Posizione della cella

                ' Imposta i bordi per ciascuna cella
                rtfTable.AppendFormat("\clbrdrl\brdrs\brdrcf1\brdrw{0}", spessoreInPunti) ' Bordo sinistro
                rtfTable.AppendFormat("\clbrdrr\brdrs\brdrcf1\brdrw{0}", spessoreInPunti) ' Bordo destro
                rtfTable.AppendFormat("\clbrdrt\brdrs\brdrcf1\brdrw{0}", spessoreInPunti) ' Bordo superiore
                rtfTable.AppendFormat("\clbrdrb\brdrs\brdrcf1\brdrw{0}", spessoreInPunti) ' Bordo inferiore
                rtfTable.AppendFormat("\cellx{0}", colPos) ' Posizione della cella
            Next

            ' Inserimento del testo nelle celle
            For j As Integer = 0 To colonne - 1
                Dim cellText As String = If(i < testoCelle.Count AndAlso j < testoCelle(i).Count, testoCelle(i)(j), "")
                rtfTable.Append("\intbl " & cellText & "\cell")
            Next
            rtfTable.Append("\row") ' Fine riga
        Next

        rtfTable.Append("}")
        Return rtfTable.ToString()
    End Function

    'link-------------------
    ' Variabili globali per memorizzare il percorso del link e l'inizio della selezione
    Private linkStart As Integer = -1
    Private linkTextLength As Integer = 0
    Private percorsoLink As String = String.Empty
    Private cartellaDocumenti As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Amministrazione", "Documenti")

    Private Sub btnInserisciLink_Click(sender As Object, e As EventArgs) Handles btnInserisciLink.Click
        ' Usa OpenFileDialog per scegliere il file
        Dim openFileDialog As New OpenFileDialog()
        openFileDialog.Title = "Seleziona il file da collegare"
        openFileDialog.InitialDirectory = cartellaDocumenti ' Imposta la cartella iniziale

        If openFileDialog.ShowDialog() = DialogResult.OK Then
            ' Ottieni il percorso completo del file
            Dim percorsoCompleto As String = openFileDialog.FileName

            ' Controlla se il percorso contiene spazi
            If percorsoCompleto.Contains(" ") Then
                MessageBox.Show("Il percorso non deve avere spazi: " & percorsoCompleto)
                Return
            End If

            ' Aggiungi 'file:///' all'inizio del percorso
            Dim linkText As String = "file:///" & percorsoCompleto.Replace("\", "/")

            ' Visualizza il percorso nella console
            Console.WriteLine("Percorso del file selezionato: " & linkText)

            ' Imposta la posizione di inizio del link
            linkStart = richTextBoxDocument.TextLength
            richTextBoxDocument.AppendText(linkText & vbCrLf)
            linkTextLength = linkText.Length

            ' Sottolinea il testo del link
            richTextBoxDocument.Select(linkStart, linkTextLength)
            richTextBoxDocument.SelectionFont = New Font(richTextBoxDocument.Font, FontStyle.Underline)
            richTextBoxDocument.DeselectAll()
        End If
    End Sub


    Private Sub richTextBoxDocument_LinkClicked(sender As Object, e As LinkClickedEventArgs) Handles richTextBoxDocument.LinkClicked
        Try
            ' Rimuove 'file:///' dal percorso
            Dim percorsoCompleto As String = e.LinkText.Replace("file:///", "").Replace("/", "\")

            ' Messaggio in console per il debug
            Console.WriteLine("Tentativo di apertura del file: " & percorsoCompleto)

            ' Verifica se il file esiste
            If Not System.IO.File.Exists(percorsoCompleto) Then
                MessageBox.Show("Il file non esiste: " & percorsoCompleto)
                Return
            End If

            ' Assicurati di racchiudere il percorso in virgolette per gestire spazi
            Dim processStartInfo As New ProcessStartInfo()
            processStartInfo.FileName = percorsoCompleto
            processStartInfo.UseShellExecute = True

            ' Avvia il processo
            Process.Start(processStartInfo)
        Catch ex As Exception
            MessageBox.Show("Errore nell'aprire il file: " & ex.Message)
            Console.WriteLine("Errore nell'aprire il file: " & ex.Message)
        End Try
    End Sub

    'fine link----------------

    Private Sub btnUndo_Click(sender As Object, e As EventArgs) Handles btnUndo.Click
        If richTextBoxDocument.CanUndo Then
            richTextBoxDocument.Undo()
        End If
        AggiornaPulsantiUndoRedo()
    End Sub

    Private Sub btnRedo_Click(sender As Object, e As EventArgs) Handles btnRedo.Click
        If richTextBoxDocument.CanRedo Then
            richTextBoxDocument.Redo()
        End If
        AggiornaPulsantiUndoRedo()
    End Sub

    Private Sub richTextBoxDocument_TextChanged(sender As Object, e As EventArgs) Handles richTextBoxDocument.TextChanged
        AggiornaPulsantiUndoRedo()
    End Sub

    Private Sub richTextBoxDocument_SelectionChanged(sender As Object, e As EventArgs) Handles richTextBoxDocument.SelectionChanged
        AggiornaPulsantiUndoRedo()
    End Sub

    Private Sub AggiornaPulsantiUndoRedo()
        btnUndo.Enabled = richTextBoxDocument.CanUndo
        btnRedo.Enabled = richTextBoxDocument.CanRedo
    End Sub


End Class