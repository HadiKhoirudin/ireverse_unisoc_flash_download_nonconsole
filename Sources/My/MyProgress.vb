Module MyProgress
    Public Watch As Stopwatch
    Public WaktuCari As Integer = 0
    Private DoubleBytes As Double
    Public Sub SetWaktu()
        WaktuCari = 60
        If Main.SharedUI.LabelTimer.InvokeRequired Then
            Main.SharedUI.LabelTimer.Invoke(CType(Sub() Main.SharedUI.LabelTimer.Visible = True, Action))
        Else
            Main.SharedUI.LabelTimer.Visible = True
        End If
    End Sub
    Public Sub SetTimer(Val As String)
        If Main.SharedUI.LabelTimer.InvokeRequired Then
            Main.SharedUI.LabelTimer.Invoke(CType(Sub() Main.SharedUI.LabelTimer.Text = Val, Action))
        Else
            Main.SharedUI.LabelTimer.Text = Val
        End If
    End Sub
    Public Sub DGVClear()
        If Main.SharedUI.DataView.InvokeRequired Then
            Main.SharedUI.DataView.Invoke(CType(Sub() Main.SharedUI.DataView.Rows.Clear(), Action))
        Else
            Main.SharedUI.DataView.Rows.Clear()
        End If
    End Sub
    Public Sub Delay(dblSecs As Double)
        Microsoft.VisualBasic.DateAndTime.Now.AddSeconds(0.0000115740740740741)
        Dim dateTime As Date = Microsoft.VisualBasic.DateAndTime.Now.AddSeconds(0.0000115740740740741)
        Dim dateTime1 As Date = dateTime.AddSeconds(dblSecs)
        While Date.Compare(Microsoft.VisualBasic.DateAndTime.Now, dateTime1) <= 0
            Windows.Forms.Application.DoEvents()
        End While
    End Sub

    Public Function GetButtonText(sender As Object) As String
        If TypeOf sender Is Button Then
            Dim btn As Button = DirectCast(sender, Button)
            Return btn.Text
        End If
        Return ""
    End Function


    Public Sub ProcessBar1(Process As Long, total As Long)
        Dim val As Integer = CInt(Math.Round(Process * 100L / total))
        If val > 99 Then
            val = 100
        End If
        Main.SharedUI.ProgressBar1.Invoke(CType(Sub() Main.SharedUI.ProgressBar1.Value = val, Action))
    End Sub

    Public Sub ProcessBar2(Process As Long, total As Long)
        Dim val As Integer = CInt(Math.Round(Process * 100L / total))
        If val > 99 Then
            val = 100
        End If
        Main.SharedUI.ProgressBar2.Invoke(CType(Sub() Main.SharedUI.ProgressBar2.Value = val, Action))
    End Sub

    Public Sub ProcessBar1(Process As Long)
        Dim val As Long = Process
        If val > 99 Then
            val = 100
        End If
        Main.SharedUI.ProgressBar1.Invoke(CType(Sub() Main.SharedUI.ProgressBar1.Value = val, Action))
    End Sub

    Public Sub ProcessBar2(Process As Long)
        Dim val As Long = Process
        If val > 99 Then
            val = 100
        End If
        Main.SharedUI.ProgressBar2.Invoke(CType(Sub() Main.SharedUI.ProgressBar2.Value = val, Action))
    End Sub
    Public Function GetFileSizes(TheSize As Long) As String
        Dim str As String = ""
        Try
            Dim num As Long = TheSize
            If num >= 1099511627776L Then
                DoubleBytes = TheSize / 1099511627776
                str = String.Concat(FormatNumber(DoubleBytes, 2, TriState.UseDefault, TriState.UseDefault, TriState.UseDefault), " TB")
            ElseIf num >= 1073741824L AndAlso num <= 1099511627775L Then
                DoubleBytes = TheSize / 1073741824
                str = String.Concat(FormatNumber(DoubleBytes, 2, TriState.UseDefault, TriState.UseDefault, TriState.UseDefault), " GB")
            ElseIf num >= 1048576L AndAlso num <= 1073741823L Then
                DoubleBytes = TheSize / 1048576
                str = String.Concat(FormatNumber(DoubleBytes, 2, TriState.UseDefault, TriState.UseDefault, TriState.UseDefault), " MB")
            ElseIf num >= 1024L AndAlso num <= 1048575L Then
                DoubleBytes = TheSize / 1024
                str = String.Concat(FormatNumber(DoubleBytes, 2, TriState.UseDefault, TriState.UseDefault, TriState.UseDefault), " KB")
            ElseIf num < 0L OrElse num > 1023L Then
                str = ""
            Else
                DoubleBytes = TheSize
                str = String.Concat(FormatNumber(DoubleBytes, 2, TriState.UseDefault, TriState.UseDefault, TriState.UseDefault), " bytes")
            End If
        Catch ex As Exception
            Console.WriteLine(ex.ToString())
        End Try
        Return str
    End Function
End Module
