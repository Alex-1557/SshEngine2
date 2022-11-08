Imports BackendAPI.Model
Imports BackendAPI.Notification
Imports BackendAPI.Services

Namespace Vm
    Public Class VmBashAsync
        Inherits VmBash

        Public SshOutput As List(Of String)
        Public SshErrMsg As List(Of String)
        Public Sub New(_DB As ApplicationDbContext, _Aes As AesCryptor, VmName As String, VmConnectionDecryptPass As String)
            MyBase.New(_DB, _Aes, VmName, VmConnectionDecryptPass)
            SshErrMsg = New List(Of String)
            SshOutput = New List(Of String)
        End Sub

        Public Overloads Async Function Bash(BashCmd As String) As Task(Of String)
            Dim CTX = New Threading.CancellationTokenSource()
            If SshClient.IsConnected Then
                Try
                    Dim Cmd1 = SshClient.CreateCommand(BashCmd)
                    Await Task.Run(Function() Cmd1.ExecuteAsync(New Progress(Of SshOutputLine), CTX.Token, SshOutput, SshErrMsg, VmHosts.Ip))
                    'Await Cmd1.ExecuteAsync(New Progress(Of SshOutputLine), CTX)
                Catch ex As Exception
                    Return ex.Message
                End Try
            Else
                Await Task.Run(Function() "not connected")
            End If
        End Function

        Public Shared Widening Operator CType(v As VmBashAsync) As Task(Of Object)
            Throw New NotImplementedException()
        End Operator
    End Class
End Namespace