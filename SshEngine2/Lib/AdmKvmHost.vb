Imports BackendAPI.Model
Imports Microsoft.EntityFrameworkCore

Namespace Kvm
    Public Class AdmKvmHost
        Property i As Integer
        Property ServerName As String
        Property OsVersion As String
        Property KvmVersion As String
        Property UserName As String
        Property Password As String
        Property CpuModel As String
        Property CpuCount As Integer
        Property CpuSocket As Integer
        Property CorePerSocket As Integer
        Property ThreadPerSocket As Integer
        Property NumaCell As Integer
        Property MemorySize As Long
        Property MainServerIP As String
        Property Location As String
        Property MonthPrice As Decimal
        Property Comment As String
        Property LastUpdate As DateTime
        Public Shared Function ReadAdmKvmHostList(_DB As ApplicationDbContext) As Tuple(Of List(Of AdmKvmHost), Exception)
            Dim AdmKvmHostList As New List(Of AdmKvmHost)
            _DB.Database.OpenConnection
            Dim CMD1 = _DB.Database.GetDbConnection().CreateCommand()
            Try
                CMD1.CommandText = $"select * from `cryptochestmax`.`KvmHost`;"
                Dim RDR1 = CMD1.ExecuteReader
                While RDR1.Read
                    AdmKvmHostList.Add(New AdmKvmHost With {
                                .i = CInt(RDR1("i")),
                                .ServerName = RDR1("ServerName"),
                                .OsVersion = RDR1("OsVersion"),
                                .KvmVersion = RDR1("KvmVersion"),
                                .UserName = RDR1("UserName"),
                                .Password = Text.UTF8Encoding.UTF8.GetString(RDR1("Password")),
                                .CpuModel = RDR1("CpuModel"),
                                .CpuCount = RDR1("CpuCount"),
                                .CpuSocket = RDR1("CpuSocket"),
                                .CorePerSocket = RDR1("CorePerSocket"),
                                .ThreadPerSocket = RDR1("ThreadPerSocket"),
                                .NumaCell = RDR1("NumaCell"),
                                .MemorySize = RDR1("MemorySize"),
                                .MainServerIP = RDR1("MainServerIP"),
                                .MonthPrice = RDR1("MonthPrice"),
                                .Location = RDR1("Location"),
                                .Comment = If(IsDBNull(RDR1("Comment")), "", RDR1("Comment")),
                                .LastUpdate = CDate(RDR1("LastUpdate"))
                                          })
                End While
                RDR1.Close()
                Return New Tuple(Of List(Of AdmKvmHost), Exception)(AdmKvmHostList, Nothing)
            Catch ex As Exception
                Debug.WriteLine(CMD1.CommandText & vbCrLf & ex.Message)
                Return New Tuple(Of List(Of AdmKvmHost), Exception)(Nothing, ex)
            End Try
        End Function

        Public Shared Function ReadAdmKvmConnectionInfo(_DB As ApplicationDbContext, ServerI As Integer, ServerDecryptPass As String) As Tuple(Of AdmKvmHost, Exception)
            Dim AdmKvmHostList As New List(Of AdmKvmHost)
            _DB.Database.OpenConnection
            Dim CMD1 = _DB.Database.GetDbConnection().CreateCommand()
            Try
                CMD1.CommandText = $"select `MainServerIP`,`UserName`,AES_DECRYPT(`Password`,'{ServerDecryptPass}') as Password from `cryptochestmax`.`KvmHost` where I={ServerI};"
                Dim RDR1 = CMD1.ExecuteReader
                While RDR1.Read
                    AdmKvmHostList.Add(New AdmKvmHost With {
                               .UserName = RDR1("UserName"),
                               .Password = Text.UTF8Encoding.UTF8.GetString(RDR1("Password")),
                               .MainServerIP = RDR1("MainServerIP")
                                         })
                End While
                RDR1.Close()
                Return New Tuple(Of AdmKvmHost, Exception)(AdmKvmHostList(0), Nothing)
            Catch ex As Exception
                Debug.WriteLine(CMD1.CommandText & vbCrLf & ex.Message)
                Return New Tuple(Of AdmKvmHost, Exception)(Nothing, ex)
            End Try
        End Function
    End Class


End Namespace