Imports System.Runtime.InteropServices
Module PhoneCommandAPI
#Region "PhoneCommand"

    Private Const LibPhoneCommand As String = "DeviceApi\PhoneCommand.dll"

    <DllImport(LibPhoneCommand, CallingConvention:=CallingConvention.Cdecl)>
    Public Function SP_CreatePhone(pLogUtil As IntPtr) As SP_HANDLE
    End Function

    <DllImport(LibPhoneCommand, CallingConvention:=CallingConvention.Cdecl)>
    Public Function SP_BeginPhoneTest(hDiagPhone As SP_HANDLE, ByRef pOpenArgument As CHANNEL_ATTRIBUTE) As Integer
    End Function

    <DllImport(LibPhoneCommand, CallingConvention:=CallingConvention.Cdecl)>
    Public Sub SP_EndPhoneTest(hDiagPhone As SP_HANDLE)
    End Sub
    <DllImport(LibPhoneCommand, CallingConvention:=CallingConvention.Cdecl)>
    Public Sub SP_ReleasePhone(hDiagPhone As SP_HANDLE)
    End Sub

    <DllImport(LibPhoneCommand, CallingConvention:=CallingConvention.Cdecl)>
    Public Function SP_RestartPhone(hDiagPhone As SP_HANDLE, ePhoneMode As RM_MODE_ENUM) As Integer
    End Function

    <DllImport(LibPhoneCommand, CallingConvention:=CallingConvention.Cdecl)>
    Public Function SP_ReadSN(
        ByVal hDiagPhone As SP_HANDLE,
        ByVal sn1 As Boolean,
        ByVal pSN As IntPtr,
        ByVal uLen As UInteger
    ) As Integer
    End Function

    <DllImport(LibPhoneCommand, CallingConvention:=CallingConvention.Cdecl)>
    Public Function SP_SendATCommand(
        ByVal hDiagPhone As SP_HANDLE,
        ByVal lpATCommand As Byte(),
        ByVal bWantReply As Boolean,
        ByVal lpReplyString As Byte(),
        ByVal ulReplyStringLen As UInteger,
        ByRef pulResponseLen As UInteger,
        ByVal ulTimeOut As UInteger
    ) As Integer
    End Function

    ' Deklarasi fungsi SP_ReadNV
    <DllImport(LibPhoneCommand, CallingConvention:=CallingConvention.Cdecl)>
    Public Function SP_GetAPVersion(
        ByVal hDiagPhone As SP_HANDLE,
        ByVal lpBuff As IntPtr,
        ByVal ulBuffLen As UInteger
    ) As Integer
    End Function

    <DllImport(LibPhoneCommand, CallingConvention:=CallingConvention.Cdecl)>
    Public Function SP_LoadProductData(hDiagPhone As SP_HANDLE, ImeiNvID As Byte(), Output As Byte()) As Integer
    End Function

    <DllImport(LibPhoneCommand, CallingConvention:=CallingConvention.Cdecl)>
    Public Function SP_btLoadAddr(hDiagPhone As SP_HANDLE, BTAddr As Byte()) As Integer
    End Function

    <DllImport(LibPhoneCommand, CallingConvention:=CallingConvention.Cdecl)>
    Public Function SP_wifiLoadAddr(hDiagPhone As SP_HANDLE, WIFIAddr As Byte()) As Integer
    End Function

    <DllImport(LibPhoneCommand, CallingConvention:=CallingConvention.Cdecl)>
    Public Function SP_SendAndRecvDiagPackage(
                                             hDiagPhone As SP_HANDLE,
                                             lpValue As Byte(),
                                             nbrOfBytesToWrite As ULong
                                             ) As Integer
    End Function
    <DllImport(LibPhoneCommand, CallingConvention:=CallingConvention.Cdecl)>
    Public Function SP_Write(hDiagPhone As SP_HANDLE, lpValue As Byte(), nbrOfBytesToWrite As ULong) As Integer
    End Function

    <DllImport(LibPhoneCommand, CallingConvention:=CallingConvention.Cdecl)>
    Public Function SP_Read(hDiagPhone As SP_HANDLE, lpValue As Byte(), nbrOfBytesToRead As ULong, Optional ulTimeOut As ULong = 1000) As Integer
    End Function

    <DllImport(LibPhoneCommand, CallingConvention:=CallingConvention.Cdecl)>
    Public Function SP_ReadNV(
        ByVal hDiagPhone As SP_HANDLE,
        ByVal uNvID As UShort,
        ByVal lpData As IntPtr,
        ByVal ulDataLen As UInteger,
        ByRef pulDataLen As UInteger
    ) As Integer
    End Function

    Public Structure SP_HANDLE
        Public Value As IntPtr
    End Structure

    Public Enum RM_MODE_ENUM
        RM_NORMAL_MODE = &H0
    End Enum

    Public Enum CHANNEL_TYPE
        CHANNEL_TYPE_COM = 0
        CHANNEL_TYPE_SOCKET = 1
        CHANNEL_TYPE_FILE = 2
        CHANNEL_TYPE_USBMON = 3
    End Enum

    <StructLayout(LayoutKind.Sequential)>
    Public Structure CHANNEL_ATTRIBUTE
        Public ChannelType As CHANNEL_TYPE
        Public Com As COM_CHANNEL_ATTRIBUTE
        Public Socket As SOCKET_CHANNEL_ATTRIBUTE
        Public File As FILE_CHANNEL_ATTRIBUTE
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Public Structure COM_CHANNEL_ATTRIBUTE
        Public dwPortNum As UInteger
        Public dwBaudRate As UInteger
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Public Structure SOCKET_CHANNEL_ATTRIBUTE
        Public dwPort As UInteger
        Public dwIP As UInteger
        Public dwFlag As UInteger
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Public Structure FILE_CHANNEL_ATTRIBUTE
        Public dwPackSize As UInteger
        Public dwPackFreq As UInteger
        Public pFilePath As String
    End Structure

    <DllImport(LibPhoneCommand, CallingConvention:=CallingConvention.Cdecl)>
    Public Function SP_SaveProductData(
        ByVal hDiagPhone As SP_HANDLE,
        ByVal ImeiNvID As Byte(),
        ByVal imei As String
    ) As Integer
    End Function

    <DllImport(LibPhoneCommand, CallingConvention:=CallingConvention.Cdecl)>
    Public Function SP_PowerOff(hDiagPhone As SP_HANDLE) As Integer
    End Function

    Public Enum CUSTOMER_PHONE_STATE_OPER
        CUSTOMER_DISK_FORMAT = 0
        CUSTOMER_FACTORY_RESET
        CUSTOMER_TCARD_CLEAR
    End Enum

    Public Const MAX_IMEI_STR_LENGTH As Integer = 16
    Public Const MAX_IMEI_NV_LENGTH As Integer = 8
    Public Const MAX_BT_ADDR_STR_LENGTH As Integer = 13
    Public Const MAX_BT_ADDR_NV_LENGTH As Integer = 6
    Public Const MAX_WIFI_ADDR_STR_LENGTH As Integer = 13
    Public Const MAX_WIFI_ADDR_NV_LENGTH As Integer = 6
    Public Const NVID_IMEI1 As UShort = &H5
    Public Const NVID_IMEI2 As UShort = &H179

#End Region


#Region "Diag Channel"
    Private Const LibDiagChannel As String = "DeviceApi\Channel9.dll"


    <DllImport(LibDiagChannel, CallingConvention:=CallingConvention.Cdecl)>
    Public Sub terminate()
    End Sub


#End Region
End Module
