Imports BackendAPI.Model
Imports BackendAPI.Notification
Imports BackendAPI.Services

Namespace Vm
    Public Class VmBashAsync2
        Inherits VmBash

        Public SshOutput As MyConcurrentDictionary(Of Integer, String)
        Public SshErrMsg As MyConcurrentDictionary(Of Integer, String)
        Public Sub New(_DB As ApplicationDbContext, _Aes As AesCryptor, VmName As String, VmConnectionDecryptPass As String)
            MyBase.New(_DB, _Aes, VmName, VmConnectionDecryptPass)
            SshErrMsg = New MyConcurrentDictionary(Of Integer, String)
            SshOutput = New MyConcurrentDictionary(Of Integer, String)
        End Sub

        Public Overloads Async Function Bash(BashCmd As String, CTX As Threading.CancellationTokenSource, ByVal NewLine As EventHandler(Of KeyValuePair(Of Integer, String)), ByVal ErrLine As EventHandler(Of KeyValuePair(Of Integer, String))) As Task(Of String)
            If SshClient.IsConnected Then
                Try
                    Dim Cmd1 = SshClient.CreateCommand(BashCmd)
                    Await Task.Run(Function() Cmd1.ExecuteAsync2(New Progress(Of SshOutputLine2), CTX.Token, VmHosts.Ip, NewLine, ErrLine, Me))
                    'Await Cmd1.ExecuteAsync(New Progress(Of SshOutputLine), CTX)
                Catch ex As Exception
                    Return ex.Message
                End Try
            Else
                Await Task.Run(Function() "not connected")
            End If
        End Function

        Public Shared Widening Operator CType(v As VmBashAsync2) As Task(Of Object)
            Throw New NotImplementedException()
        End Operator
    End Class
End Namespace