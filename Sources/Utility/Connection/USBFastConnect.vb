Imports System.IO.Ports
Imports System.Management
Imports System.Runtime.InteropServices
Public Class USBFastConnect

#Region "COM"
    Public Shared listDevices As List(Of comInfo) = New List(Of comInfo)()
    Public Shared Ports As New SerialPort()
    Public Shared ReadOnly watch As Stopwatch = New Stopwatch()
    Public Shared delta As Long = 0
    Public Class comInfo
        Public Property name As String
        Public Property hwid As String
        Public Property comport As String
        Public Property type As Integer
    End Class
    Public Shared Sub getcomInfo()
        Dim deviceWatcher As ManagementEventWatcher = Nothing
        watch.Start()

        Call Task.Run(Sub()
                          Try
                              Dim query As New WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2 OR EventType = 3")
                              deviceWatcher = New ManagementEventWatcher(query)
                              AddHandler deviceWatcher.EventArrived, AddressOf DeviceEventArrived
                              deviceWatcher.Start()
                          Catch ex As Exception
                              Console.WriteLine(ex.Message)
                          End Try
                      End Sub)
    End Sub

    Public Shared Sub DeviceEventArrived(ByVal sender As Object, ByVal e As EventArrivedEventArgs)
        If watch.ElapsedMilliseconds - delta < 100 Then
            Return
        End If

        delta = watch.ElapsedMilliseconds

        UpdateList()
    End Sub

    Public Shared Sub UpdateList()
        Call Task.Run(Sub()
                          Dim list As List(Of comInfo) = New List(Of comInfo)()
                          Try
                              Dim managementObjectSearcher As ManagementObjectSearcher = New ManagementObjectSearcher("Select * From Win32_POTSModem Where Status=""OK""")
                              Try
                                  For Each item As ManagementBaseObject In managementObjectSearcher.[Get]()
                                      Dim managementObject As ManagementObject = CType(item, ManagementObject)
                                      Dim obj As comInfo = New comInfo With {
                                                                                 .comport = managementObject.GetPropertyValue(CStr("AttachedTo")).ToString().Replace("COM", "")
                                                                                }
                                      Dim propertyValue As Object = managementObject.GetPropertyValue("Name")
                                      Dim obj2 As String = If(propertyValue IsNot Nothing, propertyValue.ToString(), Nothing)
                                      Dim propertyValue2 As Object = managementObject.GetPropertyValue("AttachedTo")
                                      obj.name = obj2 & " (" & (If(propertyValue2 IsNot Nothing, propertyValue2.ToString(), Nothing)) & ")"
                                      obj.hwid = managementObject.GetPropertyValue(CStr("DeviceID")).ToString()
                                      obj.type = 0
                                      Dim comInfo As comInfo = obj
                                      If comInfo.hwid.Contains("6860") OrElse comInfo.hwid.Contains("PID_685D") AndAlso Not comInfo.hwid.Contains("A185D30") OrElse (comInfo.name.ToLower().Contains("samsung mobile") AndAlso comInfo.name.ToLower().Contains("usb modem")) Then
                                          comInfo.type = 1
                                      ElseIf comInfo.hwid.Contains("PID_685D") AndAlso comInfo.hwid.Contains("A185D30") Then
                                          comInfo.type = 2
                                      End If
                                      list.Add(comInfo)
                                  Next
                                  managementObjectSearcher.Dispose()
                              Catch value As COMException
                                  Console.WriteLine(value)
                              End Try
                              Dim managementObjectSearcher2 As ManagementObjectSearcher = New ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity Where Status=""OK""")
                              Try
                                  For Each item2 As ManagementObject In managementObjectSearcher2.[Get]()
                                      If item2("Caption") IsNot Nothing Then
                                          Dim text As String = item2(CStr("Name")).ToString()
                                          If (text.Contains("Serial Port") OrElse text.ToUpper.Contains("QUALCOMM") OrElse text.ToUpper.Contains("PRELOADER") OrElse text.ToUpper.Contains("MEDIATEK") OrElse text.ToUpper.Contains("SPRD") OrElse text.Contains("LGE") OrElse text.Contains("1.0")) AndAlso text.Contains("(COM") Then
                                              Dim text2 As String = item2(CStr("DeviceID")).ToString()
                                              Dim array = CType(item2("HardwareID"), String())
                                              Dim comInfo2 As comInfo = New comInfo With {
                                                                                              .name = item2(CStr("Name")).ToString(),
                                                                                              .hwid = If(array.Length = 0, Nothing, array(0)),
                                                                                              .comport = betweenStrings(item2(CStr("Name")).ToString(), "(COM", ")")
                                                                                             }
                                              If text2.ToLower().Contains("diagserd") AndAlso text2.ToLower().Contains("0002") Then
                                                  comInfo2.type = 3
                                              End If
                                              list.Add(comInfo2)
                                          End If
                                      End If
                                  Next
                                  managementObjectSearcher2.Dispose()
                              Catch value2 As Exception
                                  Console.WriteLine(value2.Message)
                              End Try

                              If list.Count <> listDevices.Count Then
                                  listDevices = list
                                  Try
                                      UpdatecomboPort(listDevices)
                                  Catch ex As Exception
                                      Console.WriteLine(ex.Message)
                                  End Try
                              End If

                          Catch value3 As Exception
                              Console.WriteLine(value3)
                          End Try
                      End Sub)
    End Sub

    Public Shared Sub UpdatecomboPort(ByVal list As List(Of comInfo))
        Dim regex = String.Empty
        If Main.SharedUI.ComboPort.InvokeRequired Then
            Main.SharedUI.ComboPort.Invoke(CType(Sub()
                                                     If list.Count < Main.SharedUI.ComboPort.Items.Count Then
                                                         Main.SharedUI.ComboPort.Text = Nothing
                                                         Main.SharedUI.ComboPort.AllowDrop = False
                                                     End If
                                                     Main.SharedUI.ComboPort.Items.Clear()
                                                     For Each item As comInfo In list
                                                         Dim text = ""
                                                         If item.type = 1 Then
                                                             text = "[MTP] "
                                                         ElseIf item.type = 2 Then
                                                             text = "[DLM] "
                                                         ElseIf item.type = 3 Then
                                                             text = "[DIAG] "
                                                         End If
                                                         Dim text2 As String = text & item.name
                                                         Main.SharedUI.ComboPort.Items.Add(text2)
                                                         If item.name.Contains("SAMSUNG") AndAlso String.IsNullOrEmpty(regex) Then
                                                             regex = text2
                                                         End If
                                                     Next
                                                     If Not String.IsNullOrEmpty(regex) Then
                                                         Main.SharedUI.ComboPort.SelectedItem = regex
                                                     ElseIf list.Count > 0 Then
                                                         Main.SharedUI.ComboPort.SelectedIndex = 0
                                                     End If
                                                 End Sub, MethodInvoker))
            Return
        End If
        If list.Count < Main.SharedUI.ComboPort.Items.Count Then
            Main.SharedUI.ComboPort.Text = Nothing
            Main.SharedUI.ComboPort.AllowDrop = False
        End If
        Main.SharedUI.ComboPort.Items.Clear()
        For Each item2 As comInfo In list
            Console.WriteLine(item2.name)
            Main.SharedUI.ComboPort.Items.Add(item2.name & " (COM" + item2.comport & ")")
            If item2.name.Contains("SAMSUNG") AndAlso String.IsNullOrEmpty(regex) Then
                regex = item2.name & " (COM" + item2.comport & ")"
            End If
        Next
        Main.SharedUI.ComboPort.SelectedItem = regex
    End Sub
    Public Shared Function GetCurrentDeviceIndex() As Integer
        Return Main.SharedUI.ComboPort.SelectedIndex
    End Function
    Public Shared Function betweenStrings(ByVal text As String, ByVal start As String, ByVal [end] As String) As String
        Dim num = text.IndexOf(start) + start.Length
        Dim num2 = text.IndexOf([end], num)
        If Equals([end], "") Then
            Return text.Substring(num)
        End If
        Return text.Substring(num, num2 - num)
    End Function
    Public Shared Function VID(ByVal stream As String) As String()
        Dim array = New String(1) {}
        Dim num = stream.IndexOf("VID_")
        Dim text = stream.Substring(num + 4)
        array(0) = text.Substring(0, 4)
        Dim num2 = stream.IndexOf("PID_")
        Dim text2 = stream.Substring(num2 + 4)
        array(1) = text2.Substring(0, 4)
        Return array
    End Function
    Public Shared Function FindNewDevice(ByVal oldDevices As List(Of comInfo)) As comInfo
        Try
            Dim stopwatch As Stopwatch = New Stopwatch()
            stopwatch.Start()
            Dim newDevices As New List(Of comInfo)
            While True
                If stopwatch.ElapsedMilliseconds <= 30000L Then
                    If listDevices.Count = 0 OrElse listDevices Is oldDevices Then
                        Continue While
                    End If
                    newDevices = listDevices.Where(Function(ByVal device As comInfo) oldDevices.All(Function(ByVal oldDevice As comInfo) oldDevice.comport IsNot device.comport)).ToList()
                    If newDevices.Count > 0 Then
                        For Each e As comInfo In newDevices
                            If e.name.ToUpper.Contains("SPRD") Then
                                Exit While
                            End If
                        Next
                    End If
                    Continue While
                End If
                Return Nothing
            End While
            Return newDevices(0)
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return Nothing
        End Try
    End Function
#End Region

End Class
