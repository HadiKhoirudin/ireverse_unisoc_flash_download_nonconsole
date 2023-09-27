Imports System.ComponentModel
Imports System.IO
Imports System.Xml
Imports UniFlash.PortIO

Module uni_worker
    Public Firmware As String = ""
    Public foldersave As String = ""
    Public PortCom As String = ""
    Public StringXML As String = ""
    Public WorkerMethod As String = ""
    Public USBMethod As String = "Diag Channel"
    Public Logs As String = ""

    Public isPartitionOperation As Boolean = False

    Public totalchecked As Integer = 0
    Public totaldo As Integer = 0

    Public Sub UnisocWorker_DoWork(sender As Object, e As DoWorkEventArgs)
        If WorkerMethod = "Download" Then

            If SearchDownloadPort() Then
                ConnectDownload()
            Else
                Return
            End If

            If Main.SharedUI.CkKeepCharge.Checked Then
                RichLogs("Keep Charge" & vbTab & ": OK ", Color.Black, True, True)
                send_keepcharge()
            End If

        ElseIf WorkerMethod = "Flash" Then
            GetFlashPartition()

            'Reset 

            If Main.SharedUI.CkAutoReboot.Checked Then
                RichLogs("Reboot" & vbTab & vbTab & ": OK ", Color.Black, True, True)
                send_reset()
            End If

        ElseIf WorkerMethod = "Read Partition" Then
            GetReadPartition()

            'Reset 

            If Main.SharedUI.CkAutoReboot.Checked Then
                RichLogs("Reboot" & vbTab & vbTab & ": OK ", Color.Black, True, True)
                send_reset()
            End If

        ElseIf WorkerMethod = "Erase Partition" Then
            GetErasePartition()

            'Reset 

            If Main.SharedUI.CkAutoReboot.Checked Then
                RichLogs("Reboot" & vbTab & vbTab & ": OK ", Color.Black, True, True)
                send_reset()
            End If

        ElseIf WorkerMethod = "Parse" Then

            'Logs = "Log/UartComm_COM_141_2023_09_26_Rd.bin"
            'Dim Data As Byte() = File.ReadAllBytes("Log/UartComm_COM_141_2023_09_26_Rd.bin")
            'File.WriteAllBytes("Log/boot-bak.img", ExtractData(Data))

            'Dim Data As Byte() = PACExtractor.ExtractPacData(73330112, 20971520)
            'File.WriteAllBytes("Log/boot.img", Data)

        ElseIf WorkerMethod = "PAC Firmware" Then
            If Not Directory.Exists(Path.GetDirectoryName(Main.SharedUI.TxtPacFirmware.Text) & "\ImageFiles") Then
                Directory.CreateDirectory(Path.GetDirectoryName(Main.SharedUI.TxtPacFirmware.Text) & "\ImageFiles")
            End If
            Dim input() As String = {Main.SharedUI.TxtPacFirmware.Text, Path.GetDirectoryName(Main.SharedUI.TxtPacFirmware.Text) & "\ImageFiles", "-debug"}
            PACExtractor.StartExtraction(input)
        End If
    End Sub

    Public Function SearchDownloadPort() As Boolean
        RichLogs("Searching USB SPRD Port Device... ", Color.Black, True, False)
        Dim Flag As Boolean = False
        If USBMethod = "Diag Channel" OrElse USBMethod = "Serial Port" Then
            If USBSearchPort() Then
                Flag = True
            End If
        ElseIf USBMethod = "libusb-win32" Then
            If USBWait() Then
                Flag = True
            End If
        End If
        Return Flag
    End Function

    Public Sub ConnectDownload()
        Dim fdl1 As Byte() = Nothing
        Dim fdl1_len As Integer = 0
        Dim fdl1_skip As Integer = 0

        If Main.SharedUI.CkFDL1.Checked Then
            fdl1 = File.ReadAllBytes(Main.SharedUI.TxtFDL1.Text)
            fdl1_len = File.ReadAllBytes(Main.SharedUI.TxtFDL1.Text).Length
            Console.WriteLine("Jumlah Length FDL1 : " & fdl1_len)
        End If

        Dim fdl2 As Byte() = Nothing
        Dim fdl2_len As Integer = 0
        Dim fdl2_skip As Integer = 0

        If Main.SharedUI.CkFDL2.Checked Then
            fdl2 = File.ReadAllBytes(Main.SharedUI.TxtFDL2.Text)
            fdl2_len = File.ReadAllBytes(Main.SharedUI.TxtFDL2.Text).Length
            Console.WriteLine("Jumlah Length FDL2 : " & fdl2_len)
        End If

        If USBMethod = "libusb-win32" Then
            USBConnect()
        ElseIf USBMethod = "Serial Port" Then
            PortOpen(PortCom)
        ElseIf USBMethod = "Diag Channel" Then
            DiagConnect(PortCom)
        End If

        set_chksum_type("crc16")

        RichLogs("Send connect" & vbTab & ": ", Color.Black, True, False)
        RichLogs("Connect command sent", Color.Black, True, True)
        If send_checkbaud() Then

            If send_connect() Then
#Region "send_file C++ FDL1"
                If Main.SharedUI.CkFDL1.Checked Then
                    Delay(1)
                    send_start_fdl(Convert.ToInt32(Main.SharedUI.TxtFDL1Address.Text.Replace("0x", ""), 16), fdl1_len)

                    RichLogs("Sending FDL1    : ", Color.Black, True, False)

                    While (fdl1_len > 0)

                        ProcessBar1(fdl1_skip, fdl1.Length)

                        If fdl1_len > MIDST_SIZE Then
                            send_midst(TakeByte(fdl1, fdl1_skip, MIDST_SIZE))

                            fdl1_len -= MIDST_SIZE
                            fdl1_skip += MIDST_SIZE
                        Else
                            send_midst(TakeByte(fdl1, fdl1_skip, fdl1_len))

                            fdl1_len = 0
                        End If

                    End While

                    send_end()
                    send_exec()
                    RichLogs("Done", Color.Purple, True, True)

                    send_connect()

                    If Main.SharedUI.CkFDL2.Checked Then
                        ProcessBar2(100, 200)
                    Else
                        ProcessBar2(100, 100)
                    End If

                End If
#End Region

#Region "send_file C++ FDL2"
                If Main.SharedUI.CkFDL2.Checked Then

                    send_connect()

                    set_chksum_type("add")

                    send_start_fdl(Convert.ToInt32(Main.SharedUI.TxtFDL2Address.Text.Replace("0x", ""), 16), fdl2_len)

                    RichLogs("Sending FDL2    : ", Color.Black, True, False)

                    While (fdl2_len > 0)

                        ProcessBar1(fdl2_skip, fdl2.Length)

                        If fdl2_len > MIDST_SIZE Then
                            send_midst(TakeByte(fdl2, fdl2_skip, MIDST_SIZE))
                            fdl2_len -= MIDST_SIZE
                            fdl2_skip += MIDST_SIZE
                        Else
                            send_midst(TakeByte(fdl2, fdl2_skip, fdl2_len))
                            fdl2_len = 0
                        End If


                    End While

                    send_end()
                    send_exec()

                    RichLogs("Done", Color.Purple, True, True)
                    ProcessBar2(200, 200)
                    Main.SharedUI.CkFDLLoaded.Invoke(CType(Sub() Main.SharedUI.CkFDLLoaded.Checked = True, Action))
                End If
#End Region

            Else
                Console.WriteLine("Failed to send ping.")
            End If

        Else
            Console.WriteLine("Failed to send ping.")
        End If

        If USBMethod = "Diag Channel" Then
            DiagClose()
            If File.Exists(Logs) Then
                Delay(1)
                File.Delete(Logs)
            End If
        End If

    End Sub
    Public Sub GetFlashPartition()
        Console.WriteLine(StringXML)

        Dim doprosess As Integer = 0
        totaldo = totalchecked
        Dim xr1 As XmlTextReader

        xr1 = New XmlTextReader(New StringReader(StringXML))

        Do While xr1.Read()
            If xr1.NodeType = XmlNodeType.Element AndAlso xr1.Name = "Partition" Then
                Dim partition = xr1.GetAttribute("id")
                Dim startsector = xr1.GetAttribute("startsector")
                Dim endsector = xr1.GetAttribute("endsector")
                Dim size = xr1.GetAttribute("size")
                Dim location = xr1.GetAttribute("location")

                FlashPartition(partition, startsector, endsector, size, location)
                doprosess += 1

                ProcessBar2(doprosess, totaldo)
            End If
        Loop

    End Sub

    Public Sub FlashPartition(partition As String, startsector As ULong, endsector As ULong, size As String, location As String)

        If USBMethod = "Diag Channel" Then
            DiagConnect(PortCom)
        End If

        Dim PartitionData As Byte()
        Dim PartitionData_len As Long
        Dim PartitionData_writen As Long

        RichLogs("Flashing Partition " & partition & " : ", Color.Black, True, False)

        set_chksum_type("add")

        send_enable_flash()

        If File.Exists(location) Then
            PartitionData = File.ReadAllBytes(location)
            PartitionData_len = PartitionData.Length
            If PartitionData_len > StrToSize(size) Then
                RichLogs("Failed! File size overflow.", Color.Red, True, True)
                Return
            Else
                send_start_flash(partition, Nothing, PartitionData_len)
            End If
        Else
            PartitionData = PACExtractor.ExtractPacData(startsector, endsector)
            PartitionData_len = PartitionData.Length
            Delay(2)
            send_start_flash(partition, size)
        End If

        While (PartitionData_len > 0)

            ProcessBar1(PartitionData_writen, PartitionData_len)

            If PartitionData_len > 4096 Then
                send_midst(TakeByte(PartitionData, PartitionData_writen, 4096))

                PartitionData_len -= 4096
                PartitionData_writen += 4096
            Else
                send_midst(TakeByte(PartitionData, PartitionData_writen, PartitionData_len))

                PartitionData_len = 0
            End If

        End While

        send_end()

        If USBMethod = "Diag Channel" Then
            DiagClose()
            If File.Exists(Logs) Then
                Delay(1)
                File.Delete(Logs)
            End If
        End If

    End Sub

    Public Sub GetReadPartition()
        Console.WriteLine(StringXML)

        Dim doprosess As Integer = 0
        totaldo = totalchecked
        Dim xr1 As XmlTextReader

        xr1 = New XmlTextReader(New StringReader(StringXML))

        Do While xr1.Read()
            If xr1.NodeType = XmlNodeType.Element AndAlso xr1.Name = "Partition" Then
                Dim partition = xr1.GetAttribute("id")
                Dim size = xr1.GetAttribute("size")

                ReadPartition(partition, size)
                doprosess += 1

                ProcessBar2(doprosess, totaldo)
            End If
        Loop

    End Sub

    Public Sub ReadPartition(partition As String, size As String)
        Console.WriteLine("Partition Name : " & partition & " Partition size : " & size)

        RichLogs("Reading Partition " & partition & " : ", Color.Black, True, False)

        set_chksum_type("add")

        send_enable_flash()

        send_read(partition, size)

        If USBMethod = "Diag Channel" Then
            ReadPartitionChannel(partition, size)
        Else

            Dim stream As New FileStream(foldersave & "\" & partition & ".img", FileMode.Append, FileAccess.Write)
            Using stream
                Dim buffer As Byte() = New Byte(4096) {}

                Dim i As Integer = 0
                Dim BYTES_TO_READ As Long = StrToSize(size) 'Partition Size
                Dim bytesRead As Long = 4096
                Dim fileOffset As Long = 0
                Do
                    fileOffset = bytesRead * i

                    send_read_midst(bytesRead, fileOffset)

                    buffer = DataReadFlash

                    If fileOffset = BYTES_TO_READ - bytesRead Then

                        If buffer IsNot Nothing Then
                            stream.Write(buffer, 0, buffer.Length)
                            Console.WriteLine("Buffer Data : " & buffer.Length)
                        End If

                        ProcessBar1(100)
                        stream.Flush()
                        stream.Close()

                        send_read_end()
                        Exit Do
                    End If

                    If buffer IsNot Nothing Then
                        stream.Write(buffer, 0, buffer.Length)
                        Console.WriteLine("Buffer Data : " & buffer.Length)
                    End If

                    ProcessBar1(fileOffset, BYTES_TO_READ)
                    fileOffset += bytesRead
                    i += 1
                Loop

            End Using

            RichLogs("OK", Color.Lime, True, True)
        End If
    End Sub
    Public Sub GetErasePartition()
        Console.WriteLine(StringXML)

        Dim doprosess As Integer = 0
        totaldo = totalchecked
        Dim xr1 As XmlTextReader

        xr1 = New XmlTextReader(New StringReader(StringXML))

        Do While xr1.Read()
            If xr1.NodeType = XmlNodeType.Element AndAlso xr1.Name = "Partition" Then
                Dim partition = xr1.GetAttribute("id")
                Dim size = xr1.GetAttribute("size")

                ErasePartition(partition, size)
                doprosess += 1

                ProcessBar2(doprosess, totaldo)
            End If
        Loop

    End Sub

    Public Sub ErasePartition(partition As String, size As String)
        If USBMethod = "Diag Channel" Then
            DiagConnect(PortCom)
        End If

        Console.WriteLine("Partition Name : " & partition & " Partition size : " & size)

        RichLogs("Erasing Partition " & partition & " : ", Color.Black, True, False)

        set_chksum_type("add")

        send_enable_flash()

        send_erase(partition, size)

        RichLogs("OK", Color.Lime, True, True)

        If USBMethod = "Diag Channel" Then
            DiagClose()
            If File.Exists(Logs) Then
                Delay(1)
                File.Delete(Logs)
            End If
        End If
    End Sub
    Public Sub UnisocWorker_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs)
        WorkerMethod = ""
        RichLogs("", Color.Black, True, True)
        RichLogs("_____________________________________________________________________________", Color.Black, True, True)
        RichLogs("Progress is completed", Color.Black, True, True)
    End Sub


    Public Sub ReceiverDataWorker_DoWork(sender As Object, e As DoWorkEventArgs)
        ReadTask()
    End Sub

    Public Sub ReceiverDataWorker_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs)

    End Sub
End Module
