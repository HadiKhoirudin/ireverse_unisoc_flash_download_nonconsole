Module MyDisplay
    Public Sub RichLogs(msg As String, colour As Color, isBold As Boolean, Optional NextLine As Boolean = False)
        Main.SharedUI.Logs.Invoke(Sub()
                                      Main.SharedUI.Logs.SelectionStart = Main.SharedUI.Logs.Text.Length
                                      Dim selectionColor As Color = Main.SharedUI.Logs.SelectionColor
                                      Main.SharedUI.Logs.SelectionColor = colour
                                      If isBold Then
                                          Main.SharedUI.Logs.SelectionFont = New Font(Main.SharedUI.Logs.Font, FontStyle.Bold)
                                      Else
                                          Main.SharedUI.Logs.SelectionFont = New Font(Main.SharedUI.Logs.Font, FontStyle.Regular)
                                      End If
                                      Main.SharedUI.Logs.AppendText(msg)
                                      Main.SharedUI.Logs.SelectionColor = selectionColor
                                      If NextLine Then
                                          If Main.SharedUI.Logs.TextLength > 0 Then
                                              Main.SharedUI.Logs.AppendText(vbCrLf)
                                          End If
                                      End If
                                  End Sub)
    End Sub

    Public Sub RtbClear()
        Main.SharedUI.Logs.Invoke(Sub()
                                      Main.SharedUI.Logs.Clear()
                                  End Sub)
    End Sub
    Public Function USBSearchPort() As Boolean
        Dim Flag As Boolean = False
        Main.SharedUI.ComboPort.Invoke(Sub()
                                           If Not Main.SharedUI.ComboPort.Text.Contains("SPRD") Then
                                               SetWaktu()

                                               For i As Integer = 0 To WaktuCari

                                                   Delay(1)

                                                   If Main.SharedUI.ComboPort.Text.Contains("SPRD") Then
                                                       RichLogs("Found", Color.Lime, True, True)
                                                       RichLogs("", Color.Black, True, True)
                                                       Flag = True
                                                       SetTimer(0)
                                                       Exit For
                                                   End If

                                                   SetTimer(WaktuCari - i)
                                               Next i

                                           Else
                                               RichLogs("Found", Color.Lime, True, True)
                                               RichLogs("", Color.Black, True, True)
                                               Flag = True
                                           End If

                                           If Flag Then
                                               PortCom = Main.SharedUI.ComboPort.Text.Substring(Main.SharedUI.ComboPort.Text.LastIndexOf("(")).Replace("COM", "").Replace("(", "").Replace(")", "")
                                           End If

                                       End Sub)
        Return Flag
    End Function
End Module
