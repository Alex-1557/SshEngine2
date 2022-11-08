Imports BackendAPI.Model
Imports BackendAPI.Services

Namespace Vm
    Public Class VmBash
        Inherits Kvm.ServerBash

        Public VmHosts As VmConnectionInfo
        Protected Shadows DbRead As Tuple(Of List(Of VmConnectionInfo), Exception)
        Public Sub New(_DB As ApplicationDbContext, _Aes As AesCryptor, VmName As String, VmConnectionDecryptPass As String)
            DbRead = VmConnectionInfo.ReadVmConnectionInfo(_DB, VmName, VmConnectionDecryptPass)
        End Sub
        Public Overloads Function SSHServerConnect() As Tuple(Of Renci.SshNet.SshClient, Exception, Exception)
            Try
                VmHosts = DbRead.Item1(0)
                If VmHosts Is Nothing Then
                    Return New Tuple(Of Renci.SshNet.SshClient, Exception, Exception)(SshClient, Nothing, DbRead.Item2)
                Else
                    SshClient = New Renci.SshNet.SshClient(DbRead.Item1(0).Ip, DbRead.Item1(0).Login, DbRead.Item1(0).Pass)
                    AddHandler SshClient.HostKeyReceived, AddressOf SshClient_HostKeyReceived
                    SshClient.Connect()
                    Return New Tuple(Of Renci.SshNet.SshClient, Exception, Exception)(SshClient, Nothing, Nothing)
                End If
            Catch ex As Renci.SshNet.Common.SshAuthenticationException
                Debug.WriteLine(ex.Message & vbCrLf & (DbRead.Item1(0).Ip & vbCrLf & DbRead.Item1(0).Login & vbCrLf & DbRead.Item1(0).Pass))
                Return New Tuple(Of Renci.SshNet.SshClient, Exception, Exception)(Nothing, ex, Nothing)
            Catch ex As System.Net.Sockets.SocketException
                Debug.WriteLine(ex.Message & vbCrLf & (DbRead.Item1(0).Ip & vbCrLf & DbRead.Item1(0).Login & vbCrLf & DbRead.Item1(0).Pass))
                Return New Tuple(Of Renci.SshNet.SshClient, Exception, Exception)(Nothing, Nothing, ex)
            Catch ex As Exception
                Debug.WriteLine(ex.Message & vbCrLf & (DbRead.Item1(0).Ip & vbCrLf & DbRead.Item1(0).Login & vbCrLf & DbRead.Item1(0).Pass))
                Return New Tuple(Of Renci.SshNet.SshClient, Exception, Exception)(Nothing, Nothing, ex)
            End Try

        End Function

    End Class
End Namespace
