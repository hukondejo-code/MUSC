Imports System.IO

Public Class Form2
    Private HelyiBeallitasok As New Dictionary(Of String, String)()
    Private TxtNev As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.dat")

    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' 1. Betöltjük a meglévő adatokat
        If File.Exists(TxtNev) Then
            For Each sor As String In File.ReadLines(TxtNev)
                If sor.Contains("=") AndAlso Not sor.StartsWith("::") AndAlso Not sor.Trim() = "" Then
                    Dim reszek = sor.Split("="c)
                    HelyiBeallitasok(reszek(0).Trim()) = reszek(1).Trim()
                End If
            Next
        End If

        ' 2. Frissítjük a címkéket a betöltött szoftvernevekkel
        CimkekFrissitese()
    End Sub

    ' Címkéket frissítő rutin
    Private Sub CimkekFrissitese()
        For i As Integer = 1 To 8
            Dim lblNev As String = "LblStatusz" & i
            Dim kulcsNev As String = "NEV_" & i

            Dim celLabel As Label = CType(Me.Controls.Find(lblNev, True).FirstOrDefault(), Label)

            If celLabel IsNot Nothing Then
                If HelyiBeallitasok.ContainsKey(kulcsNev) Then
                    celLabel.Text = "Selected: " & HelyiBeallitasok(kulcsNev) & ""
                    celLabel.ForeColor = Color.Green
                Else
                    celLabel.Text = "Not Defined!"
                    celLabel.ForeColor = Color.Red
                End If
            End If
        Next
    End Sub

    ' Közös tallózó rutin
    Private Sub Tallozas(index As Integer)
        Using ofd As New OpenFileDialog()
            ofd.Filter = "Executables (*.exe)|*.exe|All Filetypes (*.*)|*.*"
            ofd.Title = index & ". executable selected."

            If ofd.ShowDialog() = DialogResult.OK Then
                Dim teljesUtvonal As String = ofd.FileName
                Dim mappa As String = Path.GetDirectoryName(teljesUtvonal)
                Dim exeNev As String = Path.GetFileName(teljesUtvonal)
                Dim psNev As String = Path.GetFileNameWithoutExtension(teljesUtvonal)

                HelyiBeallitasok("NEV_" & index) = psNev
                HelyiBeallitasok("MAPPA_" & index) = mappa
                HelyiBeallitasok("EXE_" & index) = exeNev

                If Not HelyiBeallitasok.ContainsKey("VAR_" & index) Then
                    HelyiBeallitasok("VAR_" & index) = "2"
                End If

                ' Azonnal frissítjük a képernyőn a feliratot a tallózás után
                CimkekFrissitese()
                MessageBox.Show(exeNev & " executable defined.")
            End If
        End Using
    End Sub

    ' Gombok eseménykezelői
    Private Sub BtnTalloz1_Click(sender As Object, e As EventArgs) Handles BtnTalloz1.Click
        Tallozas(1)
    End Sub

    Private Sub BtnTalloz2_Click(sender As Object, e As EventArgs) Handles BtnTalloz2.Click
        Tallozas(2)
    End Sub

    Private Sub BtnTalloz3_Click(sender As Object, e As EventArgs) Handles BtnTalloz3.Click
        Tallozas(3)
    End Sub

    Private Sub BtnTalloz4_Click(sender As Object, e As EventArgs) Handles BtnTalloz4.Click
        Tallozas(4)
    End Sub

    Private Sub BtnTalloz5_Click(sender As Object, e As EventArgs) Handles BtnTalloz5.Click
        Tallozas(5)
    End Sub

    Private Sub BtnTalloz6_Click(sender As Object, e As EventArgs) Handles BtnTalloz6.Click
        Tallozas(6)
    End Sub

    Private Sub BtnTalloz7_Click(sender As Object, e As EventArgs) Handles BtnTalloz7.Click
        Tallozas(7)
    End Sub

    Private Sub BtnTalloz8_Click(sender As Object, e As EventArgs) Handles BtnTalloz8.Click
        Tallozas(8)
    End Sub

    ' Mentés gomb eseménye
    Private Sub BtnMentes_Click(sender As Object, e As EventArgs) Handles BtnMentes.Click
        Try
            Dim kiirandoSorok As New List(Of String)()
            For Each kulcs In HelyiBeallitasok.Keys
                kiirandoSorok.Add(kulcs & "=" & HelyiBeallitasok(kulcs))
            Next

            File.WriteAllLines(TxtNev, kiirandoSorok)
            ' -----------------------------------------------------------------
            ' ÚJ SOR: Frissítjük a Form1 beállításait és füleit az új adatokkal
            ' -----------------------------------------------------------------
            Form1.BeallitasokBetoltese()
            ' -----------------------------------------------------------------
            MessageBox.Show("Settings saved successfuly!", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Me.Close()
        Catch ex As Exception
            MessageBox.Show("Save error: " & ex.Message)
        End Try
    End Sub
End Class
