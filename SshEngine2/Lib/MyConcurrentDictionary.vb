Imports System.Collections.Concurrent
Imports System.Diagnostics.Eventing.Reader
Imports System.Text
Imports BackendAPI.KVM
Imports Microsoft.Data.SqlClient

Namespace Notification

    Public Class MyConcurrentDictionary(Of Tkey, T)
        Inherits ConcurrentDictionary(Of Tkey, T)

        Public Function PrintKeys() As String
            Return IIf(String.IsNullOrEmpty(String.Join(",", MyBase.Keys.ToList)), "No", String.Join(",", MyBase.Keys.ToList))
        End Function
        Public Function Print() As String
            If String.IsNullOrEmpty(String.Join(",", MyBase.Keys.ToList)) Then
                Return "No"
            Else
                Dim Out1 As New StringBuilder
                For i As Integer = 0 To MyBase.ToList.Count - 1
                    Out1.AppendLine($"{MyBase.Keys(i).ToString}:{MyBase.Values(i).ToString}")
                Next
                Return Out1.ToString
            End If
        End Function
    End Class
End Namespace