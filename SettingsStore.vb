Imports System.IO
Imports System.Text

Module SettingsStore
    ' Simple INI-like key=value loader and saver (no sections)
    Public Function ReadSettingsFromFile(filePath As String) As Dictionary(Of String, String)
        Dim result As New Dictionary(Of String, String)()
        If Not File.Exists(filePath) Then Return result

        For Each rawLine As String In File.ReadAllLines(filePath, Encoding.UTF8)
            Dim line = rawLine.Trim()
            If String.IsNullOrEmpty(line) Then Continue For
            If line.StartsWith("#") OrElse line.StartsWith(";") OrElse line.StartsWith("::") Then Continue For

            Dim idx = line.IndexOf("=")
            If idx <= 0 Then Continue For
            Dim key = line.Substring(0, idx).Trim()
            Dim value = line.Substring(idx + 1).Trim()
            If result.ContainsKey(key) Then
                result(key) = value
            Else
                result.Add(key, value)
            End If
        Next

        Return result
    End Function

    Public Sub WriteSettingsToFile(filePath As String, data As Dictionary(Of String, String))
        Dim sb As New StringBuilder()
        For Each kvp In data
            sb.AppendLine($"{kvp.Key}={kvp.Value}")
        Next
        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8)
    End Sub

    ' If a legacy Settings.dat exists and Settings.ini does not, migrate by renaming.
    Public Sub MigrateDatToIni(baseDirectory As String)
        Try
            Dim iniPath = Path.Combine(baseDirectory, "Settings.ini")
            Dim datPath = Path.Combine(baseDirectory, "Settings.dat")

            If File.Exists(datPath) AndAlso Not File.Exists(iniPath) Then
                ' Prefer atomic move; fall back to copy+delete if needed
                Try
                    File.Move(datPath, iniPath)
                Catch ex As Exception
                    ' Try copy & delete if move fails (e.g., across volumes)
                    File.Copy(datPath, iniPath)
                    File.Delete(datPath)
                End Try
            End If
        Catch
            ' Swallow migration errors; reading will attempt fallback where appropriate
        End Try
    End Sub
End Module
