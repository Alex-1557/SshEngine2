Imports System.Diagnostics.Metrics
Imports System.Threading
Imports BackendAPI.Notification
Imports BackendAPI.Vm

Public Class SshOutputLine2
    Public Shared Property LineCount As Integer

    Public Sub New(ByVal Line As String, ByVal IsErrorLine As Boolean, IpAddr As String, ByVal NewLine As EventHandler(Of KeyValuePair(Of Integer, String)), ByVal ErrLine As EventHandler(Of KeyValuePair(Of Integer, String)), ByVal RefToClass As VmBashAsync2)
        Debug.WriteLine($"Line Ready ({Now}): {Line}")
        Interlocked.Increment(LineCount)
        Debug.WriteLine($"{IpAddr} Line Ready ({Now}): LineCount={LineCount} : ThreadId={Thread.CurrentThread.ManagedThreadId}")

        If IsErrorLine Then
            RefToClass.SshErrMsg.TryAdd(LineCount, Line)
            ErrLine.Invoke(RefToClass, New KeyValuePair(Of Integer, String)(LineCount, Line))
        Else
            RefToClass.SshOutput.TryAdd(LineCount, Line)
            NewLine.Invoke(RefToClass, New KeyValuePair(Of Integer, String)(LineCount, Line))
        End If
    End Sub
End Class
