Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Text
Imports System.Threading
Imports LibUsbDotNet
Imports LibUsbDotNet.LibUsb
Imports LibUsbDotNet.Main
Imports LibUsbDotNet.Info
Imports MonoLibUsb
Imports MonoLibUsb.Transfer
Imports Usb = MonoLibUsb.MonoUsbApi
Imports System.Runtime.InteropServices

Public Module LibUSBPort
    'USB\VID_1782&PID_4D00\5&2D3B1689&1&1
    Private Const USB_VID As Integer = &H1782
    Private Const USB_PID As Integer = &H4D00
    Private Const HEADER_SIZE As Integer = 4
    Private Const BLOCK_SIZE As Integer = 512 * 1024 ' 512 KB
    Private LastDataEventDate As DateTime = DateTime.Now


    Private devices As UsbDevice
    Private targetSerialNumber As String

    Public Property Timeout() As Integer = 3000


    'Public ecRead As ErrorCode
    'Public reader As UsbEndpointReader
    'Public writer As UsbEndpointWriter
    'Public ecWrite As ErrorCode

    'Public transferredOut As Integer
    'Public transferredIn As Integer
    'Private usbWriteTransfer As UsbTransfer
    'Private usbReadTransfer As UsbTransfer

    Public MaxPacket As Integer = 1024
    Public readBuffer As Byte() = New Byte(MaxPacket) {}



    Private Const MY_CONFIG As Integer = 1
    Private Const MY_EP_READ As Byte = &H85
    Private Const MY_EP_WRITE As Byte = &H6
    Private Const MY_INTERFACE As Integer = 0
    Private Const MY_PID As Short = &H4D00
    Private Const MY_VID As Short = &H1782
    Private device_handle As MonoUsbDeviceHandle = Nothing
    Private sessionHandle As MonoUsbSessionHandle = Nothing
    Private TEST_REST_DEVICE As Boolean = False
    Private Const MY_TIMEOUT As Integer = 2000

    Public Function USBWait() As Boolean
        SetWaktu()
        Dim counter As Integer = 0
        Dim TimeOut As Integer = WaktuCari
        Do

            SetTimer(WaktuCari - counter)

            Dim allDevices = devices.AllDevices

            If allDevices.Any(Function(x) x.Vid = USB_VID And x.Pid = USB_PID) Then

                Main.SharedUI.ComboPort.Invoke(Sub()
                                                   Main.SharedUI.ComboPort.Text = "SPRD U2S Diag (libusb-win32)"
                                               End Sub)

                Console.WriteLine("USB Found!")
                RichLogs("Found", Color.Lime, True, True)
                RichLogs("", Color.Black, True, True)
                SetTimer(0)
                Return True
            End If

            If counter = WaktuCari Then
                Console.WriteLine("Timeout error.")
                RichLogs("Not Found", Color.Red, True, True)
                Return False
            End If

            Delay(1)
            counter += 1

        Loop

        Return False
    End Function

    Public Sub USBConnect()
        'Try
        '    devices.ForceLibUsbWinBack = True
        '    Dim finder As UsbDeviceFinder

        '    If String.IsNullOrWhiteSpace(targetSerialNumber) Then
        '        finder = New UsbDeviceFinder(USB_VID, USB_PID)
        '    Else
        '        finder = New UsbDeviceFinder(USB_VID, USB_PID, targetSerialNumber)
        '    End If

        '    devices = UsbDevice.OpenUsbDevice(finder)

        '    If devices Is Nothing Then
        '        Console.WriteLine("No devices available.")
        '        Main.SharedUI.ComboPort.Invoke(Sub()
        '                                           Main.SharedUI.ComboPort.Text = ""
        '                                       End Sub)
        '        Return
        '    Else

        '        reader = devices.OpenEndpointReader(ReadEndpointID.Ep05, 65536)
        '        writer = devices.OpenEndpointWriter(WriteEndpointID.Ep06, EndpointType.Bulk)

        '        Dim wDev = TryCast(devices, IUsbDevice)

        '        If TypeOf wDev Is IUsbDevice Then
        '            wDev.SetConfiguration(1)
        '            wDev.ClaimInterface(0)
        '        End If

        '        'Int libusb_control_transfer(
        '        'libusb_device_handle * devh,
        '        'uint8_t bmRequestType, 0x21
        '        'uint8_t  bRequest, 34
        '        'uint16_t  wValue, 0x601
        '        'uint16_t  wIndex, 0
        '        'unsigned  Char * Data, NULL
        '        'uint16_t wLength, 0
        '        'unsigned int timeout
        '        ')
        '        '
        '        'Perform a
        '        'USB Control transfer.  Returns the actual number	of  bytes  transferred
        '        'On  success,  in	 the range from	And including zero up to And including
        '        'wLength.On  error  a  LIBUSB_ERROR  code  Is  returned,  for  example
        '        'LIBUSB_ERROR_TIMEOUT  If  the  transfer timed out, LIBUSB_ERROR_PIPE if
        '        'the Control request was Not supported, LIBUSB_ERROR_NO_DEVICE  if  the
        '        'device  has  been  disconnected	And another LIBUSB_ERROR code on other
        '        'failures.The LIBUSB_ERROR codes are all negative.

        '        'libusb_control_transfer(
        '        'IO -> dev_handle,
        '        '        0x21,
        '        '        34,
        '        '        0x601,
        '        '        0,
        '        '        NULL,
        '        '        0,
        '        '        IO -> Timeout);
        '        Dim buffer As Byte() = New Byte(0) {}
        '        Dim transferred As Integer
        '        Dim setup As UsbSetupPacket = New UsbSetupPacket(&H21, &H34, &H601, &H0, Nothing)
        '        Dim result As Boolean = devices.ControlTransfer(setup, buffer, buffer.Length, transferred)
        '        Console.WriteLine("Result = {0}, {1}", result, transferred)
        '        Console.WriteLine("Devices connected.")
        '    End If

        'Catch ex As Exception
        '    RichLogs(ex.ToString(), Color.Red, True, True)
        'End Try

        Dim r As Integer = 0
        sessionHandle = New MonoUsbSessionHandle()
        If sessionHandle.IsInvalid Then
            Throw New Exception("Invalid session handle.")
        End If

        Console.WriteLine("Opening Device..")
        Delay(0.1)
        device_handle = MonoUsbApi.OpenDeviceWithVidPid(sessionHandle, MY_VID, MY_PID)
        If device_handle Is Nothing OrElse device_handle.IsInvalid Then
            Console.WriteLine("Failed : Can't open device. Handle is invalid!")
            Return
        End If

        ' If TEST_REST_DEVICE = True, reset the device and re-open
        If TEST_REST_DEVICE Then
            Console.WriteLine("Reset device..")
            Delay(0.1)
            MonoUsbApi.ResetDevice(device_handle)
            device_handle.Close()
            device_handle = MonoUsbApi.OpenDeviceWithVidPid(sessionHandle, MY_VID, MY_PID)
            If device_handle Is Nothing OrElse device_handle.IsInvalid Then
                Console.WriteLine("Failed : Can't reset device. Handle is invalid!")
                Return
            End If
        End If

        ' Activate interface from kernel driver 
        Console.WriteLine("Activate kernel driver..")
        Delay(0.1)
        r = MonoUsbApi.KernelDriverActive(device_handle, MY_INTERFACE)
        If r <> 0 Then
            Console.WriteLine("Failed : Can't activate kernel driver.. " & MonoUsbApi.StrError(r))
            ' Detach interface from kernel driver 
            Console.WriteLine("Trying detach kernel driver..")
            Delay(0.1)
            r = MonoUsbApi.DetachKernelDriver(device_handle, MY_INTERFACE)
            If r <> 0 Then
                Console.WriteLine("Failed : Can't detach kernel driver.. " & MonoUsbApi.StrError(r))
            End If
            ' Activate interface from kernel driver 
            Console.WriteLine("Activate kernel driver..")
            Delay(0.1)
            r = MonoUsbApi.KernelDriverActive(device_handle, MY_INTERFACE)
            If r <> 0 Then
                ' Attach interface from kernel driver 
                Console.WriteLine("Failed : Trying to attach driver.. " & MonoUsbApi.StrError(r))
                r = MonoUsbApi.AttachKernelDriver(device_handle, MY_INTERFACE)
                If r <> 0 Then
                    Console.WriteLine("Failed : Kernel driver disabled.. " & MonoUsbApi.StrError(r))
                End If
            End If
        End If

        ' Set configuration
        Console.WriteLine("Set Config..")
        Delay(0.1)
        r = MonoUsbApi.SetConfiguration(device_handle, MY_CONFIG)
        If r <> 0 Then
            Console.WriteLine("Failed : Can't set config..")
            Return
        End If

        ' Claim interface
        Console.WriteLine("Set Interface..")
        Delay(0.1)
        r = MonoUsbApi.ClaimInterface(device_handle, MY_INTERFACE)
        If r <> 0 Then
            Console.WriteLine("Failed : Can't set interface.. " & MonoUsbApi.StrError(r))
            Return
        End If

        ' Control transfer
        Console.WriteLine("Set control transfer..")
        Delay(0.1)
        r = MonoUsbApi.ControlTransfer(device_handle, &H21, 34, &H601, 0, Nothing, 0, MY_TIMEOUT)
        If r <> 0 Then
            Console.WriteLine("Failed : Can't set control transfer.. " & MonoUsbApi.StrError(r))
            Return
        End If

        '0x21, 34, 0x601, 0, NULL, 0, io->timeout);
        ' Write test data
    End Sub

    Public Sub USBPortReadWrite(bytesToSend As Byte(), Optional timeout As Integer = 3000)
        'ecWrite = writer.SubmitAsyncTransfer(bytesToSend, 0, bytesToSend.Length, 1000, usbWriteTransfer)
        'If ecWrite <> ErrorCode.None Then
        '    usbReadTransfer.Dispose()
        '    Throw New Exception("Submit Async Write Failed.")
        'End If

        'readBuffer = New Byte(MaxPacket) {}

        'ecRead = reader.SubmitAsyncTransfer(readBuffer, 0, readBuffer.Length, 1500, usbReadTransfer)
        'If ecRead <> ErrorCode.None Then
        '    Throw New Exception("Submit Async Read Failed.")
        'End If

        'WaitHandle.WaitAll(New WaitHandle() {usbWriteTransfer.AsyncWaitHandle, usbReadTransfer.AsyncWaitHandle}, 3000, False)

        'If Not usbWriteTransfer.IsCompleted Then
        '    usbWriteTransfer.Cancel()
        'End If
        'If Not usbReadTransfer.IsCompleted Then
        '    usbReadTransfer.Cancel()
        'End If

        'ecWrite = usbWriteTransfer.Wait(transferredOut)
        'ecRead = usbReadTransfer.Wait(transferredIn)

        'usbWriteTransfer.Dispose()
        'usbReadTransfer.Dispose()

        'Console.Write("Read  : {0} Status: {1}", transferredIn, ecRead)
        'Console.WriteLine(vbTab & "Write : {0} Status: {1}", transferredOut, ecWrite)
        'Console.WriteLine("Data  :" & BitConverter.ToString(readBuffer).Replace("-", " "))
        Dim transferred As Integer
        Dim received As Integer
        MonoUsbApi.BulkTransfer(device_handle, MY_EP_WRITE, bytesToSend, bytesToSend.Length, transferred, timeout)
        Thread.Sleep(15)
        MonoUsbApi.BulkTransfer(device_handle, MY_EP_READ, readBuffer, readBuffer.Length, received, timeout)
    End Sub
    Private Function doBulkAsyncTransfer(dev_handle As MonoUsbDeviceHandle, endpoint As Byte, buffer As Byte(), length As Integer, ByRef transferred As Integer, timeout As Integer) As MonoUsbError
        transferred = 0
        Dim transfer As New MonoUsbTransfer(0)
        If transfer.IsInvalid Then
            Return MonoUsbError.ErrorNoMem
        End If

        Dim monoUsbTransferCallbackDelegate As MonoUsbTransferDelegate = AddressOf bulkTransferCB
        Dim userCompleted() As Integer = {0}
        Dim gcUserCompleted As GCHandle = GCHandle.Alloc(userCompleted, GCHandleType.Pinned)

        Dim e As MonoUsbError
        Dim gcBuffer As GCHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned)
        transfer.FillBulk(dev_handle, endpoint, gcBuffer.AddrOfPinnedObject(), length, monoUsbTransferCallbackDelegate, gcUserCompleted.AddrOfPinnedObject(), timeout)

        e = transfer.Submit()
        If CInt(e) < 0 Then
            transfer.Free()
            gcUserCompleted.Free()
            Return e
        End If
        Dim r As Integer
        Console.WriteLine("Transfer Submitted..")
        While userCompleted(0) = 0
            e = CType(r, MonoUsbError)
            If r < 0 Then
                If e = MonoUsbError.ErrorInterrupted Then
                    Continue While
                End If
                transfer.Cancel()
                While userCompleted(0) = 0
                    If Usb.HandleEvents(sessionHandle) < 0 Then
                        Exit While
                    End If
                End While
                transfer.Free()
                gcUserCompleted.Free()
                Return e
            End If
        End While

        transferred = transfer.ActualLength
        e = MonoUsbApi.MonoLibUsbErrorFromTransferStatus(transfer.Status)
        transfer.Free()
        gcUserCompleted.Free()
        Return e
    End Function

    ' This function originated from bulk_transfer_cb()
    ' in sync.c of the Libusb-1.0 source code.
    Private Sub bulkTransferCB(transfer As MonoUsbTransfer)
        Marshal.WriteInt32(transfer.PtrUserData, 1)
        ' caller interprets results and frees transfer
    End Sub
End Module