Imports BackendAPI.Model
Imports Microsoft.EntityFrameworkCore

Namespace Vm
    Public Class VmConnectionInfo

        Public Property Ip As String
        Public Property Login As String
        Public Property Pass As String

        Public Shared Function ReadVmConnectionInfo(_DB As ApplicationDbContext, VmName As String, VmConnectionDecryptPass As String) As Tuple(Of List(Of VmConnectionInfo), Exception)
            Dim AdmVMList As New List(Of VmConnectionInfo)
            _DB.Database.OpenConnection
            Dim CMD1 = _DB.Database.GetDbConnection().CreateCommand()
            Try
                CMD1.CommandText = $"Select `AdminLogin`, AES_DECRYPT(`AdminPassword`,'{VmConnectionDecryptPass}') as `Pass`, `Ip`  FROM `VM` join `VmIp` on `VM`.`i`=`VmIp`.`toVm` where `cryptochestmax`.`VM`.`Name`='{VmName}';"
                Dim RDR1 = CMD1.ExecuteReader
                While RDR1.Read
                    AdmVMList.Add(New VmConnectionInfo With {
                                .Login = RDR1("AdminLogin"),
                                .Pass = If(IsDBNull(RDR1("Pass")), "", Text.UTF8Encoding.UTF8.GetString(RDR1("Pass"))),
                                .Ip = RDR1("Ip")
                                })
                End While
                RDR1.Close()
                Return New Tuple(Of List(Of VmConnectionInfo), Exception)(AdmVMList, Nothing)
            Catch ex As Exception
                Debug.WriteLine(CMD1.CommandText & vbCrLf & ex.Message)
                Return New Tuple(Of List(Of VmConnectionInfo), Exception)(Nothing, ex)
            End Try
        End Function
    End Class
End Namespace

