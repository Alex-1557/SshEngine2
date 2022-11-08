Imports BackendAPI.Model
Imports BackendAPI.Services

Namespace Kvm
    Public Class ServerBash
        Implements IDisposable

        Protected SshClient As Renci.SshNet.SshClient
        Protected KvmHosts As AdmKvmHost
        Protected DbRead As Tuple(Of AdmKvmHost, Exception)
        Private disposedValue As Boolean

        Public Sub New()
            'support other new in inherits class
        End Sub
        Public Sub New(_DB As ApplicationDbContext, _Aes As AesCryptor, ServerI As Integer, ServerDecryptPass As String)
            DbRead = AdmKvmHost.ReadAdmKvmConnectionInfo(_DB, ServerI, ServerDecryptPass)
        End Sub
        Public Function SSHServerConnect() As Tuple(Of Renci.SshNet.SshClient, Exception, Exception)
            Try
                KvmHosts = DbRead.Item1
                If KvmHosts Is Nothing Then
                    Return New Tuple(Of Renci.SshNet.SshClient, Exception, Exception)(SshClient, Nothing, DbRead.Item2)
                Else
                    SshClient = New Renci.SshNet.SshClient(KvmHosts.MainServerIP, KvmHosts.UserName, KvmHosts.Password)
                    AddHandler SshClient.HostKeyReceived, AddressOf SshClient_HostKeyReceived
                    SshClient.Connect()
                    Return New Tuple(Of Renci.SshNet.SshClient, Exception, Exception)(SshClient, Nothing, Nothing)
                End If
            Catch ex As Renci.SshNet.Common.SshAuthenticationException
                Debug.WriteLine(ex.Message & vbCrLf & KvmHosts.MainServerIP & vbCrLf & KvmHosts.UserName & vbCrLf & KvmHosts.Password)
                Return New Tuple(Of Renci.SshNet.SshClient, Exception, Exception)(Nothing, ex, Nothing)
            Catch ex As System.Net.Sockets.SocketException
                Debug.WriteLine(ex.Message & vbCrLf & KvmHosts.MainServerIP & vbCrLf & KvmHosts.UserName & vbCrLf & KvmHosts.Password)
                Return New Tuple(Of Renci.SshNet.SshClient, Exception, Exception)(Nothing, Nothing, ex)
            Catch ex As Exception
                Debug.WriteLine(ex.Message & vbCrLf & KvmHosts.MainServerIP & vbCrLf & KvmHosts.UserName & vbCrLf & KvmHosts.Password)
                Return New Tuple(Of Renci.SshNet.SshClient, Exception, Exception)(Nothing, Nothing, ex)
            End Try

        End Function
        Sub SshClient_HostKeyReceived(sender As Object, e As Renci.SshNet.Common.HostKeyEventArgs)
            Dim Sb As Text.StringBuilder = New System.Text.StringBuilder()
            For i As Integer = 0 To e.HostKey.Length - 1
                Sb.Append(e.HostKey(i).ToString("X2"))
            Next
            Debug.WriteLine(vbCrLf & TryCast(sender, Renci.SshNet.SshClient).ConnectionInfo.Host & ", " & Sb.ToString())
        End Sub

        Public Function Bash(BashCmd As String) As Tuple(Of Renci.SshNet.SshCommand, Exception, Boolean)
            If SshClient.IsConnected Then
                Try
                    Dim Out1 = SshClient.RunCommand(BashCmd)
                    Close()
                    Return New Tuple(Of Renci.SshNet.SshCommand, Exception, Boolean)(Out1, Nothing, True)
                Catch ex As Exception
                    Return New Tuple(Of Renci.SshNet.SshCommand, Exception, Boolean)(Nothing, ex, True)
                End Try
            Else
                Return New Tuple(Of Renci.SshNet.SshCommand, Exception, Boolean)(Nothing, Nothing, False)
            End If
        End Function
        Public Sub Close()
            If SshClient.IsConnected Then
                SshClient.Disconnect()
            End If
        End Sub
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects)
                    SshClient = Nothing
                    KvmHosts = Nothing
                    DbRead = Nothing
                End If
                ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
                ' TODO: set large fields to null
                disposedValue = True
            End If
        End Sub

        ' ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
        ' Protected Overrides Sub Finalize()
        '     ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        '     Dispose(disposing:=False)
        '     MyBase.Finalize()
        ' End Sub

        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
            Dispose(disposing:=True)
            GC.SuppressFinalize(Me)
        End Sub
    End Class

End Namespace


