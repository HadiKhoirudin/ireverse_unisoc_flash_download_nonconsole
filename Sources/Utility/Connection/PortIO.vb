Imports System.IO
Imports System.IO.Ports
Imports System.Text
Imports System.Threading

Public Class PortIO
    Public Shared serialPort As SerialPort
    Public Shared PortCOMDiag As String = ""
    Public Shared resp() As Byte = New Byte(0) {}
    Public Shared DataReadFlash() As Byte = New Byte(0) {}

    Public Shared Sub PortOpen(PortCOMDiag As String)
        serialPort = New SerialPort("COM" & PortCOMDiag)
        serialPort.BaudRate = 115200
        serialPort.Parity = Parity.None
        serialPort.Handshake = Handshake.None
        serialPort.DataBits = 8
        serialPort.StopBits = StopBits.One
        serialPort.ReadBufferSize = 65536
        serialPort.WriteBufferSize = 65536
        serialPort.Open()
        serialPort.RtsEnable = True
        serialPort.DtrEnable = True
    End Sub

    Public Shared Sub PortClose()
        If serialPort.IsOpen Then
            serialPort.Close()
            serialPort.Dispose()
        End If
    End Sub

    Public Shared Function PortRead() As Byte()
        If Not serialPort.IsOpen Then
            Return New Byte(0) {}
        End If
        Dim numBytes As Integer = serialPort.BytesToRead
        Dim buffer(numBytes - 1) As Byte
        serialPort.BaseStream.ReadAsync(buffer, 0, numBytes)
        Return buffer
    End Function

    Private Shared Sub raiseAppSerialDataEvent(received() As Byte)
        Throw New NotImplementedException()
    End Sub

    Public Shared Sub PortWrite(request As Byte())
        If serialPort.IsOpen Then
            Thread.Sleep(11)
            serialPort.Write(request, 0, request.Length)

            If logs_on Then
                Dim writen As String = BitConverter.ToString(request).Replace("-", " ")
                If writen.Length > 40 Then
                    RichLogs("Written" & vbTab & vbTab & ": " & vbCrLf & writen, Color.DarkBlue, False, True)
                Else
                    RichLogs("Written" & vbTab & vbTab & ": " & writen, Color.DarkBlue, True, True)
                End If
            End If

        End If
    End Sub
    Public Shared Function MergeBytes(First As String, Data As Byte(), Last As String) As Byte()
        Dim b1 As Byte() = StringToByteArray(First)
        Dim b2 As Byte() = Data
        Dim b3 As Byte() = StringToByteArray(Last)
        Dim s = New MemoryStream()
        s.Write(b1, 0, b1.Length)
        s.Write(b2, 0, b2.Length)
        s.Write(b3, 0, b3.Length)
        Dim b4 = s.ToArray()
        Return b4
    End Function
    Public Shared Function StringToByteArray(ByVal hex As String) As Byte()
        hex = hex.Replace(" ", "")
        Return Enumerable.Range(0, hex.Length).Where(Function(x) x Mod 2 = 0).[Select](Function(x) Convert.ToByte(hex.Substring(x, 2), 16)).ToArray()
    End Function
    Public Shared Function TakeByte(source As Byte(), start As Integer, length As Integer) As Byte()
        Return (From element In source Skip start Take length).ToArray
    End Function
    Public Shared Function HexStringToAscii(hex As String) As String
        hex = hex.Replace(" ", "")
        Dim bytes(hex.Length \ 2 - 1) As Byte
        For i As Integer = 0 To bytes.Length - 1
            bytes(i) = Convert.ToByte(hex.Substring(i * 2, 2), 16)
        Next
        Dim asciiString As String = Encoding.ASCII.GetString(bytes)
        Return asciiString
    End Function
    Public Shared Function ReverseBytes(ByVal value As String) As String
        Dim reversed As String = ""
        For i As Integer = value.Length - 2 To 0 Step -2
            reversed += value.Substring(i, 2)
        Next
        Return reversed
    End Function
End Class
