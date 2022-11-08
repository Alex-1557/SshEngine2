Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports Renci.SshNet

Module SshCommandExtensions
    <Extension()>
    Async Function ExecuteAsync(ByVal SshCommand As SshCommand, ByVal OutputLine As IProgress(Of SshOutputLine), ByVal CTX As CancellationToken, SshOutput As List(Of String), SshErrMsg As List(Of String), IpAddr As String) As Task
        Dim AsyncResult As IAsyncResult = SshCommand.BeginExecute()
        Dim StdoutSR = New StreamReader(SshCommand.OutputStream)
        Dim StderrSR = New StreamReader(SshCommand.ExtendedOutputStream)
        While Not AsyncResult.IsCompleted
            'Debug.Print($"#1 {AsyncResult.IsCompleted} {Now} {String.Join(".", SshOutput)}")
            ' Debug.Print($"#1 {IpAddr} {AsyncResult.IsCompleted} {Now} ")
            Await Progress(SshCommand, StdoutSR, StderrSR, OutputLine, CTX, SshOutput, SshErrMsg, IpAddr)
            Thread.Yield()
            'Debug.Print($"#2 {AsyncResult.IsCompleted} {Now} {String.Join(".", SshOutput)}")
            'Debug.Print($"#2 {IpAddr} {AsyncResult.IsCompleted} {Now} ")
        End While
        'Debug.Print($"#3 {AsyncResult.IsCompleted} {Now} {String.Join(".", SshOutput)}")
        'Debug.Print($"#3 {IpAddr} {AsyncResult.IsCompleted} {Now} ")
        SshCommand.EndExecute(AsyncResult)
        Await Progress(SshCommand, StdoutSR, StderrSR, OutputLine, CTX, SshOutput, SshErrMsg, IpAddr)
        'Debug.Print($"#4 {AsyncResult.IsCompleted} {Now} {String.Join(".", SshOutput)}")
        'Debug.Print($"#4 {IpAddr} {AsyncResult.IsCompleted} {Now} ")
    End Function

    Private Async Function Progress(ByVal SshCommand As SshCommand, ByVal StdoutSR As TextReader, ByVal StderrSR As TextReader, ByVal OutputLine As IProgress(Of SshOutputLine), ByVal CTX As CancellationToken, SshOutput As List(Of String), SshErrMsg As List(Of String), IpAddr As String) As Task
        If CTX.IsCancellationRequested Then SshCommand.CancelAsync()
        CTX.ThrowIfCancellationRequested()
        Await OutProgress(StdoutSR, OutputLine, SshOutput, SshErrMsg, IpAddr)
        Await ErrProgress(StderrSR, OutputLine, SshOutput, SshErrMsg, IpAddr)
    End Function

    Private Async Function OutProgress(ByVal StdoutSR As TextReader, ByVal StdoutProgress As IProgress(Of SshOutputLine), SshOutput As List(Of String), SshErrMsg As List(Of String), IpAddr As String) As Task
        Dim StdoutLine = Await StdoutSR.ReadToEndAsync()
        If Not String.IsNullOrEmpty(StdoutLine) Then StdoutProgress.Report(New SshOutputLine(Line:=StdoutLine, IsErrorLine:=False, SshOutput, SshErrMsg, IpAddr))
    End Function

    Private Async Function ErrProgress(ByVal StderrSR As TextReader, ByVal stderrProgress As IProgress(Of SshOutputLine), SshOutput As List(Of String), SshErrMsg As List(Of String), IpAddr As String) As Task
        Dim StderrLine = Await StderrSR.ReadToEndAsync()
        If Not String.IsNullOrEmpty(StderrLine) Then stderrProgress.Report(New SshOutputLine(Line:=StderrLine, IsErrorLine:=True, SshOutput, SshErrMsg, IpAddr))
    End Function
End Module


