Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Threading

Module DiagChannel

    Dim logUtilPtr As IntPtr
    Dim hDiagPhone As SP_HANDLE
    Public ChannelBuffer As Byte() = New Byte(1024) {}

    Public Sub DiagConnect(PortCom As String)
        hDiagPhone = SP_CreatePhone(logUtilPtr)
        Dim openArgument As New CHANNEL_ATTRIBUTE()
        openArgument.ChannelType = CHANNEL_TYPE.CHANNEL_TYPE_COM
        openArgument.Com.dwPortNum = CUInt(PortCom)
        openArgument.Com.dwBaudRate = 115200 '115200
        Console.WriteLine("Begin Diag Channel : " & SP_BeginPhoneTest(hDiagPhone, openArgument))
        Logs = "Log/UartComm_COM_" & PortCom & "_" & Date.Today.Year & "_" & Date.Today.Month.ToString("00") & "_" & Date.Today.Day.ToString("00") & "_Rd.bin"
        Main.SharedUI.ReceiverDataWorker.RunWorkerAsync()
    End Sub

    Public Sub DiagClose()
        Main.SharedUI.ReceiverDataWorker.CancelAsync()
        Main.SharedUI.ReceiverDataWorker.Dispose()

        SP_EndPhoneTest(hDiagPhone)
        SP_ReleasePhone(hDiagPhone)
        Marshal.FreeHGlobal(logUtilPtr)
    End Sub

    Public Sub ReadWriteDiag(lpvalue As Byte())
        Thread.Sleep(15)
        SP_Write(hDiagPhone, lpvalue, lpvalue.Length)
    End Sub
    Public Sub ReadTask()
        Do
            If Not Main.SharedUI.ReceiverDataWorker.CancellationPending Then
                If Not isPartitionOperation Then
                    SP_Read(hDiagPhone, ChannelBuffer, 1024)
                End If
            Else
                Exit Do
                Return
            End If
        Loop
    End Sub

    Public Sub ReadPartitionChannel(partition As String, size As String)
        DiagConnect(PortCom)
        Dim i As Integer = 0
        Dim BYTES_TO_READ As Long = StrToSize(size) 'Partition Size 1MB
        Dim bytesRead As Long = 4096
        Dim fileOffset As Long = 0

        Do
            fileOffset = bytesRead * i


            If fileOffset = BYTES_TO_READ - bytesRead Then
                send_read_midst(bytesRead, fileOffset)
                ProcessBar1(100)
                send_read_end()
                Exit Do
            End If

            send_read_midst(bytesRead, fileOffset)
            ProcessBar1(fileOffset, BYTES_TO_READ)
            fileOffset += bytesRead
            i += 1
        Loop

        RichLogs("OK", Color.Lime, True, True)

        Delay(20)

        DiagClose()

        If File.Exists(Logs) Then
            RichLogs("Parsing " & partition & " : ", Color.Black, True, False)
            Dim Data As Byte() = File.ReadAllBytes(Logs)
            File.WriteAllBytes(foldersave & "/" & partition & ".img", ExtractData(Data))
            RichLogs("OK", Color.Lime, True, True)
            Delay(1)
            File.Delete(Logs)
        End If

    End Sub
    Public Sub ErasePartitionChannel(partition As String, size As String)
        Dim i As Integer = 0
        Dim BYTES_TO_READ As Long = StrToSize(size) 'Partition Size 1MB
        Dim bytesRead As Long = 4096
        Dim fileOffset As Long = 0

        Do
            fileOffset = bytesRead * i


            If fileOffset = BYTES_TO_READ - bytesRead Then
                send_read_midst(bytesRead, fileOffset)
                ProcessBar1(100)
                send_read_end()
                Exit Do
            End If

            send_read_midst(bytesRead, fileOffset)
            ProcessBar1(fileOffset, BYTES_TO_READ)
            fileOffset += bytesRead
            i += 1
        Loop

        Delay(20)
        DiagClose()

        RichLogs("OK", Color.Lime, True, True)

        If File.Exists(Logs) Then
            RichLogs("Parsing " & partition & " : ", Color.Black, True, False)
            Dim Data As Byte() = File.ReadAllBytes(Logs)
            File.WriteAllBytes(foldersave & "/" & partition & ".img", ExtractData(Data))
            RichLogs("OK", Color.Lime, True, True)
            Delay(1)
            File.Delete(Logs)
        End If

    End Sub
End Module
