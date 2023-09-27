Module Checksum
    Private chksum_type As Integer = 0
    Private Const CHKSUM_TYPE_CRC16 As Integer = 1
    Private Const CHKSUM_TYPE_ADD As Integer = 2

    Private Const HDLC_HEADER As Byte = &H7E
    Private Const HDLC_ESCAPE As Byte = &H7D

    Private Const CHK_FIXZERO As Integer = 1
    Private Const CHK_ORIG As Integer = 2

    Public Sub TransCodeTest()
        Dim src() As Byte = StringToByteArray("00 01 00 08 9F 00 00 00 00 07 34 00")
        Dim len As Integer = src.Length
        Dim dst(len - 1) As Byte

        Dim transcodeMax As Integer = SpdTranscodeMax(src, len, 4)
        Console.WriteLine("Transcoded Max: " & transcodeMax)

        Dim crc As UInteger = SpdCrc16(0, src, CUInt(len))
        Console.WriteLine("CRC16: " & crc.ToString("X4"))

        Dim checksum As UInteger = SpdChecksum(0, src, len, CHK_ORIG)
        Console.WriteLine("Checksum: " & checksum.ToString("X4"))
    End Sub

    Public Sub set_chksum_type(type As String)
        If type = "crc16" Then
            chksum_type = CHKSUM_TYPE_CRC16
        ElseIf type = "add" Then
            chksum_type = CHKSUM_TYPE_ADD
        Else
            Console.WriteLine("Checksum type incorrect.")
        End If
    End Sub

    Public Function calc_chksum(data As Byte()) As Integer
        If chksum_type = CHKSUM_TYPE_CRC16 Then
            Dim c As UInteger = SpdCrc16(0, data, data.Length)
            'Console.WriteLine("CRC16 Checksum    : " & c.ToString("X4"))
            Return c
        ElseIf chksum_type = CHKSUM_TYPE_ADD Then
            Dim c As UInteger = Checksum.SpdChecksum(0, parse_reverse(data), data.Length, CHK_ORIG)
            'Console.WriteLine("SPD Checksum      : " & c.ToString("X4"))
            Return c
        Else
            Console.WriteLine("Error: Checksum type is incorrect.")
            Return 0
        End If
    End Function

    Public Function SpdTranscode(ByRef dst() As Byte, ByVal src() As Byte, ByVal len As Integer) As Integer
        Dim i, a, n As Integer
        n = 0
        For i = 0 To len - 1
            a = src(i)
            If a = HDLC_HEADER OrElse a = HDLC_ESCAPE Then
                If dst IsNot Nothing Then dst(n) = HDLC_ESCAPE
                n += 1
                a = a Xor &H20
            End If
            If dst IsNot Nothing Then dst(n) = CByte(a)
            n += 1
        Next
        Return n
    End Function

    Public Function SpdTranscodeMax(ByVal src() As Byte, ByVal len As Integer, ByVal n As Integer) As Integer
        Dim i, a As Integer
        For i = 0 To len - 1
            a = src(i)
            If a = HDLC_HEADER OrElse a = HDLC_ESCAPE Then
                a = 2
            Else
                a = 1
            End If
            If n < a Then Exit For
            n -= a
        Next
        Return i
    End Function

    Public Function SpdCrc16(ByVal crc As UInteger, ByVal src() As Byte, ByVal len As UInteger) As UInteger
        Dim s() As Byte = New Byte(len - 1) {}
        Buffer.BlockCopy(src, 0, s, 0, CInt(len))

        crc = crc And &HFFFFUI
        Dim i As Integer
        While len > 0
            crc = crc Xor (CType(s(i), UInteger) << 8)
            For j As Integer = 0 To 7
                crc = (crc << 1) Xor (If((crc >> 15) <> 0, &H11021UI, 0))
            Next
            len -= 1
            i += 1
        End While
        Return crc
    End Function

    Public Function SpdChecksum(crc As UInteger, src As Byte(), len As Integer, final As Integer) As UInteger
        Dim s As Byte() = parse_reverse(src)

        While len > 1
            crc += (CUInt(s(1)) << 8) Or CUInt(s(0))
            s = s.Skip(2).ToArray()
            len -= 2
        End While

        If len <> 0 Then
            crc += CUInt(s(0))
        End If

        If final <> 0 Then
            crc = (crc >> 16) + (crc And &HFFFF)
            crc += crc >> 16
            crc = Not crc And &HFFFF

            If len < final Then
                crc = (crc >> 8) Or ((crc And &HFF) << 8)
            End If
        End If

        Return crc
    End Function

End Module
