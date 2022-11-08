Imports System.Diagnostics.Metrics
Imports System.Threading

Public Class SshOutputLine
    Public Shared Property LineCount As Integer

    Public Sub New(ByVal Line As String, ByVal IsErrorLine As Boolean, SshOutput As List(Of String), SshErrMsg As List(Of String), IpAddr As String)
        'Debug.WriteLine($"Line Ready ({Now}): {Line}")
        Interlocked.Increment(LineCount)
        Debug.WriteLine($"{IpAddr} Line Ready ({Now}): LineCount={LineCount} : ThreadId={Thread.CurrentThread.ManagedThreadId}")

        If IsErrorLine Then
            SshErrMsg.Add(Line)
        Else
            SshOutput.Add(Line)
        End If
    End Sub
End Class
