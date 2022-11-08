Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports BackendAPI.Notification
Imports BackendAPI.Vm
Imports Renci.SshNet

Module SshCommandExtensions2
    <Extension()>
    Async Function ExecuteAsync2(ByVal SshCommand As SshCommand, ByVal OutputLine As IProgress(Of SshOutputLine2), ByVal CTX As CancellationToken, IpAddr As String, ByVal NewLine As EventHandler(Of KeyValuePair(Of Integer, String)), ByVal ErrLine As EventHandler(Of KeyValuePair(Of Integer, String)), ByVal RefToClass As VmBashAsync2) As Task
        Dim AsyncResult As IAsyncResult = SshCommand.BeginExecute()
        Dim StdoutSR = New StreamReader(SshCommand.OutputStream)
        Dim StderrSR = New StreamReader(SshCommand.ExtendedOutputStream)
        While Not AsyncResult.IsCompleted
            'Debug.Print($"#1 {AsyncResult.IsCompleted} {Now} {String.Join(".", SshOutput)}")
            ' Debug.Print($"#1 {IpAddr} {AsyncResult.IsCompleted} {Now} ")
            Await Progress2(SshCommand, StdoutSR, StderrSR, OutputLine, CTX, IpAddr, NewLine, ErrLine, RefToClass)
            Thread.Yield()
            'Debug.Print($"#2 {AsyncResult.IsCompleted} {Now} {String.Join(".", SshOutput)}")
            'Debug.Print($"#2 {IpAddr} {AsyncResult.IsCompleted} {Now} ")
        End While
        'Debug.Print($"#3 {AsyncResult.IsCompleted} {Now} {String.Join(".", SshOutput)}")
        'Debug.Print($"#3 {IpAddr} {AsyncResult.IsCompleted} {Now} ")
        SshCommand.EndExecute(AsyncResult)
        Await Progress2(SshCommand, StdoutSR, StderrSR, OutputLine, CTX, IpAddr, NewLine, ErrLine, RefToClass)
        'Debug.Print($"#4 {AsyncResult.IsCompleted} {Now} {String.Join(".", SshOutput)}")
        'Debug.Print($"#4 {IpAddr} {AsyncResult.IsCompleted} {Now} ")
    End Function

    Private Async Function Progress2(ByVal SshCommand As SshCommand, ByVal StdoutSR As TextReader, ByVal StderrSR As TextReader, ByVal OutputLine As IProgress(Of SshOutputLine2), ByVal CTX As CancellationToken, IpAddr As String, ByVal NewLine As EventHandler(Of KeyValuePair(Of Integer, String)), ByVal ErrLine As EventHandler(Of KeyValuePair(Of Integer, String)), ByVal RefToClass As VmBashAsync2) As Task
        If CTX.IsCancellationRequested Then SshCommand.CancelAsync()
        CTX.ThrowIfCancellationRequested()
        Await OutProgress2(StdoutSR, OutputLine, IpAddr, NewLine, ErrLine, RefToClass)
        Await ErrProgress2(StderrSR, OutputLine, IpAddr, NewLine, ErrLine, RefToClass)
    End Function

    Private Async Function OutProgress2(ByVal StdoutSR As TextReader, ByVal StdoutProgress As IProgress(Of SshOutputLine2), IpAddr As String, ByVal NewLine As EventHandler(Of KeyValuePair(Of Integer, String)), ByVal ErrLine As EventHandler(Of KeyValuePair(Of Integer, String)), ByVal RefToClass As VmBashAsync2) As Task
        Dim StdoutLine = Await StdoutSR.ReadToEndAsync()
        If Not String.IsNullOrEmpty(StdoutLine) Then StdoutProgress.Report(New SshOutputLine2(Line:=StdoutLine, IsErrorLine:=False, IpAddr, NewLine, ErrLine, RefToClass))
    End Function

    Private Async Function ErrProgress2(ByVal StderrSR As TextReader, ByVal stderrProgress As IProgress(Of SshOutputLine2), IpAddr As String, ByVal NewLine As EventHandler(Of KeyValuePair(Of Integer, String)), ByVal ErrLine As EventHandler(Of KeyValuePair(Of Integer, String)), ByVal RefToClass As VmBashAsync2) As Task
        Dim StderrLine = Await StderrSR.ReadToEndAsync()
        If Not String.IsNullOrEmpty(StderrLine) Then stderrProgress.Report(New SshOutputLine2(Line:=StderrLine, IsErrorLine:=True, IpAddr, NewLine, ErrLine, RefToClass))
    End Function
End Module


