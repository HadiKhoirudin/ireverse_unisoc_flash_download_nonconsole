Imports System.IO
Imports System.Text
Imports UniFlash.PortIO

Module uni
    Private Const SPRD_DEFAULT_TIMEOUT As Integer = 200
    Private boot_ver As Integer = 0

    Public MIDST_SIZE As Integer = 528
    Public logs_on As Boolean = False

    Private Function translate(data As Byte()) As Byte()
        Dim transdata As New List(Of Byte)()
        transdata.Add(HDLC_HEADER)
        For Each b As Byte In data
            If b = HDLC_HEADER Then
                transdata.Add(HDLC_ESCAPE)
                transdata.Add(&H5E)
            ElseIf b = HDLC_ESCAPE Then
                transdata.Add(HDLC_ESCAPE)
                transdata.Add(&H5D)
            Else
                transdata.Add(b)
            End If
        Next
        transdata.Add(&H7E)
        Return transdata.ToArray()
    End Function
    Public Function detranslate(data As Byte()) As Byte()
        If data.Length > 0 Then

            Dim lst As New List(Of Byte)(data)
            If lst(0) <> HDLC_HEADER Then
                Return Nothing
            End If
            lst.RemoveAt(0)
            lst.RemoveAt(lst.Count - 1)
            Dim i As Integer = 0
            Dim detransdata As New List(Of Byte)()
            While i <= (lst.Count - 1)
                If lst(i) = HDLC_ESCAPE AndAlso lst(i + 1) = &H5E Then
                    detransdata.Add(HDLC_HEADER)
                    i += 2
                ElseIf lst(i) = HDLC_ESCAPE AndAlso lst(i + 1) = &H5D Then
                    detransdata.Add(HDLC_ESCAPE)
                    i += 2
                Else
                    detransdata.Add(lst(i))
                    i += 1
                End If
            End While
            detransdata.RemoveAt(0)
            Return detransdata.ToArray()
        Else
            Return New Byte() {}
        End If
    End Function

    Public Function ExtractData(data As Byte()) As Byte()
        Dim startSequence() As Byte = {&H7E, &H0, &H93}
        Dim endSequence() As Byte = {&H7E}
        Dim result As New List(Of Byte)()

        Dim indexes As New List(Of Integer)()
        Dim i As Integer = 0
        While i < data.Length
            Dim index = FindSequence(data, i, startSequence)
            If index = -1 Then
                Exit While
            End If
            indexes.Add(index)
            i = index + startSequence.Length
        End While

        For j As Integer = 0 To indexes.Count - 1
            Dim startIndex As Integer = indexes(j) + startSequence.Length + 2
            Dim endIndex As Integer = data.Length

            If j < indexes.Count - 1 Then
                endIndex = indexes(j + 1) - endSequence.Length - 2
            End If

            For k As Integer = startIndex To endIndex - 1
                result.Add(data(k))
            Next
        Next

        Dim lst As New List(Of Byte)(result.ToArray())
        Dim h As Integer = 0
        Dim translate As New List(Of Byte)()
        While h <= (lst.Count - 1)
            If lst(h) = HDLC_ESCAPE AndAlso lst(h + 1) = &H5E Then
                translate.Add(HDLC_HEADER)
                h += 2
            ElseIf lst(h) = HDLC_ESCAPE AndAlso lst(h + 1) = &H5D Then
                translate.Add(HDLC_ESCAPE)
                h += 2
            Else
                translate.Add(lst(h))
                h += 1
            End If
        End While
        Dim DataClear As Byte() = translate.ToArray()
        Return TakeByte(DataClear, 0, DataClear.Length - 11)
    End Function

    Private Function FindSequence(data As Byte(), startIndex As Integer, sequence As Byte()) As Integer
        For i As Integer = startIndex To data.Length - sequence.Length
            Dim found As Boolean = True
            For j As Integer = 0 To sequence.Length - 1
                If data(i + j) <> sequence(j) Then
                    found = False
                    Exit For
                End If
            Next
            If found Then
                Return i
            End If
        Next
        Return -1
    End Function

    Public Function StrToSize(ByVal str As String) As ULong
        Dim shl As Integer = 0
        Dim n As ULong
        n = str.Replace("K", "").Replace("M", "").Replace("G", "")

        If str.EndsWith("K") Then
            shl = 10
        ElseIf str.EndsWith("M") Then
            shl = 20
        ElseIf str.EndsWith("G") Then
            shl = 30
        Else
            Throw New Exception("unknown size suffix")
        End If

        If shl <> 0 Then
            Dim tmp As Long = CLng(n)
            tmp >>= 63 - shl
            If tmp <> 0 AndAlso tmp <> -1 Then
                Throw New Exception("size overflow on multiply")
            End If
        End If

        Return n << shl
    End Function

    Private Function generate_packet(command As Integer, Optional data As Byte() = Nothing) As Byte()

        If command = BSL.CMD_CHECK_BAUD Then
            Return {BSL.CMD_CHECK_BAUD}
        End If

        Dim packet As New List(Of Byte)()
        packet.AddRange(parse_reverse(BitConverter.GetBytes(CType(command, UInt16))))

        If data IsNot Nothing AndAlso data.Length > 0 Then
            packet.AddRange(parse_reverse(BitConverter.GetBytes(CType(data.Length, UInt16))))
            packet.AddRange(data)
        Else
            packet.AddRange(BitConverter.GetBytes(CType(0, UInt16)))
        End If

        Dim chksum As Integer = calc_chksum(packet.ToArray())
        packet.AddRange(parse_reverse(BitConverter.GetBytes(CType(chksum, UInt16))))
        Dim transdata As Byte() = translate(packet.ToArray())
        Return transdata

    End Function

    Public Function parse_packet(packet As Byte()) As Tuple(Of Integer, Byte(), Boolean)
        Dim detranslatedPacket As Byte() = detranslate(packet)
        If detranslatedPacket Is Nothing Then
            Return New Tuple(Of Integer, Byte(), Boolean)(0, Nothing, False)
        End If
        Dim command As Integer = BitConverter.ToUInt16(detranslatedPacket, 0)
        Dim length As Integer = BitConverter.ToUInt16(detranslatedPacket, 2)
        Dim data As Byte() = detranslatedPacket
        Dim chksum As Integer = BitConverter.ToUInt16(detranslatedPacket, detranslatedPacket.Length - 2)
        Dim chksumMatch As Boolean = (calc_chksum(detranslatedPacket.Take(detranslatedPacket.Length - 2).ToArray()) = chksum)
        Return New Tuple(Of Integer, Byte(), Boolean)(command, data, chksumMatch)
    End Function

    Public Function send_data(data As Byte(), Optional timeout As Integer = Nothing) As Boolean
        If timeout = Nothing Then
            timeout = SPRD_DEFAULT_TIMEOUT
        End If
        Try

            If USBMethod = "libusb-win32" Then
                USBPortReadWrite(data)
            ElseIf USBMethod = "Serial Port" Then
                PortWrite(data)
            ElseIf USBMethod = "Diag Channel" Then
                ReadWriteDiag(data)
            End If

            Return read_ack()
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function send_checkbaud() As Boolean
        Return send_data(generate_packet(BSL.CMD_CHECK_BAUD))
    End Function

    Public Function send_connect() As Boolean
        Return send_data(generate_packet(BSL.CMD_CONNECT))
    End Function

    Public Function send_enable_flash() As Boolean
        Return send_data(generate_packet(BSL.CMD_ENABLE_WRITE_FLASH))
    End Function
    Public Function send_disable_transcode() As Boolean
        Return send_data(generate_packet(BSL.CMD_DISABLE_TRANSCODE))
    End Function

    Public Function send_start_fdl(addr As Integer, total_size As Integer) As Boolean
        Return send_data(generate_packet(
                                                 BSL.CMD_START_DATA,
                                                 parse_reverse(BitConverter.GetBytes(total_size).Concat(BitConverter.GetBytes(addr)).ToArray())
                                                 )
                                             )
    End Function

    Public Function send_start_flash(Partition As String, Optional size As String = "1M", Optional partitionsize As ULong = 0) As Boolean
        Dim asize As ULong

        If partitionsize > 0 Then
            asize = partitionsize
        Else
            asize = StrToSize(size)
        End If

        Dim Taken As Byte() = BitConverter.GetBytes(asize)

        Dim byteA As Byte() = Encoding.Unicode.GetBytes(Partition)
        Dim byteC As Byte() = TakeByte(Taken, 0, 4)

        Dim lengthA As Integer = byteA.Length
        Dim lengthC As Integer = byteC.Length

        Dim totalLength As Integer = 76
        Dim lengthB As Integer = totalLength - (lengthA + lengthC)

        Dim byteB(lengthB - 1) As Byte

        For i As Integer = 0 To lengthB - 1
            byteB(i) = 0
        Next

        Dim resultBytes As Byte() = New Byte(lengthA + lengthB + lengthC - 1) {}
        Array.Copy(byteA, 0, resultBytes, 0, lengthA)
        Array.Copy(byteB, 0, resultBytes, lengthA, lengthB)
        Array.Copy(byteC, 0, resultBytes, lengthA + lengthB, lengthC)

        Dim Tosend As Byte() = generate_packet(BSL.CMD_START_DATA, resultBytes)

        Return send_data(Tosend)
    End Function

    Public Function send_midst(data As Byte()) As Boolean
        isPartitionOperation = True
        Return send_data(generate_packet(BSL.CMD_MIDST_DATA, data))
    End Function

    Public Function send_end() As Boolean
        isPartitionOperation = False
        Return send_data(generate_packet(BSL.CMD_END_DATA))
    End Function

    Public Function send_exec() As Boolean
        Return send_data(generate_packet(BSL.CMD_EXEC_DATA))
    End Function

    Public Function send_read(Partition As String, size As String) As Boolean
        Dim asize As ULong = StrToSize(size)

        Dim Taken As Byte() = BitConverter.GetBytes(asize)

        Dim byteA As Byte() = Encoding.Unicode.GetBytes(Partition)
        Dim byteC As Byte() = TakeByte(Taken, 0, 4)

        Dim lengthA As Integer = byteA.Length
        Dim lengthC As Integer = byteC.Length

        Dim totalLength As Integer = 76
        Dim lengthB As Integer = totalLength - (lengthA + lengthC)

        Dim byteB(lengthB - 1) As Byte

        For i As Integer = 0 To lengthB - 1
            byteB(i) = 0
        Next

        Dim resultBytes As Byte() = New Byte(lengthA + lengthB + lengthC - 1) {}
        Array.Copy(byteA, 0, resultBytes, 0, lengthA)
        Array.Copy(byteB, 0, resultBytes, lengthA, lengthB)
        Array.Copy(byteC, 0, resultBytes, lengthA + lengthB, lengthC)

        Dim Tosend As Byte() = generate_packet(BSL.CMD_READ_START, resultBytes)

        Console.WriteLine(" ")
        Console.WriteLine(StrToSize(size) & " " & BitConverter.ToString(byteC).Replace("-", " "))
        Console.WriteLine("Read Partition Data : " & BitConverter.ToString(Tosend).Replace("-", " "))

        Return send_data(Tosend)

    End Function

    Public Function send_read_midst(total As Integer, len As Integer) As Boolean
        isPartitionOperation = True
        Dim a As Byte() = BitConverter.GetBytes(total)
        Dim b As Byte() = BitConverter.GetBytes(len)
        Dim tb As Byte() = TakeByte(a, 0, 4)
        Dim tl As Byte() = TakeByte(b, 0, 4)
        Dim Tosend As Byte() = generate_packet(BSL.CMD_READ_MIDST, tb.Concat(tl).ToArray())
        Return send_data(Tosend)
    End Function

    Public Function send_read_end() As Boolean
        isPartitionOperation = False
        Return send_data(generate_packet(BSL.CMD_READ_END))
    End Function

    Public Function send_erase(Partition As String, Optional size As String = "1M", Optional partitionsize As ULong = 0) As Boolean
        Dim asize As ULong

        If partitionsize > 0 Then
            asize = partitionsize
        Else
            asize = StrToSize(size)
        End If

        Dim Taken As Byte() = BitConverter.GetBytes(asize)

        Dim byteA As Byte() = Encoding.Unicode.GetBytes(Partition)
        Dim byteC As Byte() = TakeByte(Taken, 0, 4)

        Dim lengthA As Integer = byteA.Length
        Dim lengthC As Integer = byteC.Length

        Dim totalLength As Integer = 76
        Dim lengthB As Integer = totalLength - (lengthA + lengthC)

        Dim byteB(lengthB - 1) As Byte

        For i As Integer = 0 To lengthB - 1
            byteB(i) = 0
        Next

        Dim resultBytes As Byte() = New Byte(lengthA + lengthB + lengthC - 1) {}
        Array.Copy(byteA, 0, resultBytes, 0, lengthA)
        Array.Copy(byteB, 0, resultBytes, lengthA, lengthB)
        Array.Copy(byteC, 0, resultBytes, lengthA + lengthB, lengthC)

        Dim Tosend As Byte() = generate_packet(BSL.CMD_ERASE_FLASH, resultBytes)

        Console.WriteLine(" ")
        Console.WriteLine(StrToSize(size) & " " & BitConverter.ToString(byteC).Replace("-", " "))
        Console.WriteLine("Read Partition Data : " & BitConverter.ToString(Tosend).Replace("-", " "))

        Return send_data(Tosend)
    End Function

    Public Function send_reset() As Boolean
        Return send_data(generate_packet(BSL.CMD_NORMAL_RESET))
    End Function

    Public Function send_keepcharge() As Boolean
        Return send_data(generate_packet(BSL.CMD_KEEP_CHARGE))
    End Function

    Public Function read_ack() As Boolean
        Dim resp As Byte() = New Byte() {}

        If USBMethod = "libusb-win32" Then
            resp = readBuffer
        ElseIf USBMethod = "Serial Port" Then
            resp = PortRead()
        ElseIf USBMethod = "Diag Channel" Then
            resp = ChannelBuffer
            ChannelBuffer = New Byte(1024) {}
        End If

        If resp.Length > 0 Then

            Dim str As String = BitConverter.ToString(resp).Replace("-", " ")

            Dim tuple As Tuple(Of Integer, Byte(), Boolean) = parse_packet(resp)
            Dim response As Integer = tuple.Item1
            Dim Data As Byte() = tuple.Item2
            Dim chksumMatch As Boolean = tuple.Item3

            If Data IsNot Nothing Then
                If Data.Length = 8205 Then
                    Dim a As Byte() = TakeByte(Data, 2, 4097)
                    Dim b As Byte() = TakeByte(Data, 4106, 4097)
                    Dim c As Byte() = a.Concat(b).ToArray()
                    Console.WriteLine("Readed Double Data : ")
                    Console.WriteLine(BitConverter.ToString(c).Replace("-", " "))
                    Console.WriteLine(" ")
                    DataReadFlash = c
                Else
                    Console.WriteLine("Readed Data : " & Data.Length)
                    Console.WriteLine(BitConverter.ToString(Data).Replace("-", " "))
                    Console.WriteLine(" ")
                    DataReadFlash = TakeByte(Data, 3, Data.Length - 5)
                End If
            Else
                DataReadFlash = Nothing
            End If

            If response = BSL.REP_VER Then
                Dim version As String
                RichLogs("Boot version" & vbTab & ": ", Color.Black, True, False)

                If boot_ver > 0 Then
                    version = Encoding.UTF8.GetString(TakeByte(Data, 3, Data.Length - 1014))
                    boot_ver = 0
                Else
                    version = Encoding.UTF8.GetString(TakeByte(Data, 3, Data.Length - 986))
                End If

                If version IsNot Nothing Then
                    RichLogs(version, Color.DarkBlue, True, True)
                    Console.WriteLine("Boot version: " & version)
                Else
                    Console.WriteLine("Failed to read version.")
                End If

            ElseIf response = BSL.REP_ACK Then
                'Console.WriteLine("Response : ACK Received")
            ElseIf response = BSL.REP_READ_FLASH Then
                'Console.WriteLine("Response : Reading Flash Data")
            ElseIf response = BSL.REP_VERIFY_ERROR Then
                RichLogs("Response" & vbTab & ": Verify Error", Color.DarkRed, True, True)
                Return False
            ElseIf response = BSL.REP_INCOMPATIBLE_PARTITION Then
                send_enable_flash()
            ElseIf response = BSL.REP_DOWN_SIZE_ERROR Then
                RichLogs("Response" & vbTab & ": Download Size Error!", Color.DarkRed, True, True)
                Return False
            Else

            End If

        End If
        Return True
    End Function

    Public Function TakeByte(source As Byte(), start As Integer, length As Integer) As Byte()
        Return (From element In source Skip start Take length).ToArray
    End Function

    Public Function parse_reverse(data As Byte()) As Byte()
        Dim a As String = BitConverter.ToString(data).Replace("-", " ")
        Dim b As Byte() = StringToByteArray(ReverseBytes(a))
        Return b
    End Function

    Public Function ReverseBytes(ByVal value As String) As String
        Dim reversed As String = ""
        Dim val As String = value.Replace(" ", "").Replace("-", "")
        For i As Integer = val.Length - 2 To 0 Step -2
            reversed += val.Substring(i, 2)
        Next
        Return reversed
    End Function

    Public Function StringToByteArray(ByVal hex As String) As Byte()
        hex = hex.Replace(" ", "")
        Return Enumerable.Range(0, hex.Length).Where(Function(x) x Mod 2 = 0).[Select](Function(x) Convert.ToByte(hex.Substring(x, 2), 16)).ToArray()
    End Function

End Module

