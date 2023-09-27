Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Windows.Forms
Imports System.Xml
Imports Microsoft.VisualBasic

Public Class PACExtractor
    Public Shared pacfile As String = Nothing
    Public Shared outputDir As String = Nothing
    Public Shared xmlfile As String = Nothing
    Public Shared debug As Boolean = False
    Public Shared PAC_HEADER As New Dictionary(Of String, Object)() From {
            {"szVersion", ""},
            {"dwHiSize", 0},
            {"dwLoSize", 0},
            {"productName", ""},
            {"firmwareName", ""},
            {"partitionCount", 0},
            {"partitionsListStart", 0},
            {"dwMode", 0},
            {"dwFlashType", 0},
            {"dwNandStrategy", 0},
            {"dwIsNvBackup", 0},
            {"dwNandPageType", 0},
            {"szPrdAlias", ""},
            {"dwOmaDmProductFlag", 0},
            {"dwIsOmaDM", 0},
            {"dwIsPreload", 0},
            {"dwReserved", 0},
            {"dwMagic", 0},
            {"wCRC1", 0},
            {"wCRC2", 0}
        }

    Public Shared FILE_HEADER As New Dictionary(Of String, Object)() From {
            {"length", 0},
            {"partitionName", ""},
            {"fileName", ""},
            {"szFileName", ""},
            {"hiPartitionSize", 0},
            {"hiDataOffset", 0},
            {"loPartitionSize", 0},
            {"nFileFlag", 0},
            {"nCheckFlag", 0},
            {"loDataOffset", 0},
            {"dwCanOmitFlag", 0},
            {"dwAddrNum", 0},
            {"dwAddr", 0},
            {"dwReserved", 0}
        }

    Public Shared Sub Abort(msg As String)
        Console.WriteLine(msg)
        MsgBox(msg)
        Return
    End Sub

    Public Shared Function GetString(bytes() As Byte) As String
        Return Encoding.Unicode.GetString(bytes).TrimEnd(ControlChars.NullChar)
    End Function

    Public Shared Sub PrintP(name As String, value As Object)
        Console.WriteLine($"{name,-13} = {value}")
    End Sub

    Public Shared Sub PrintPacHeader(pacHeader As Dictionary(Of String, Object))
#Region "Logs"
        RichLogs("Firmware Name " & vbTab & vbTab & ": ", Color.Black, True, False)
        RichLogs($"{pacHeader("firmwareName")}", Color.Black, True, True)

        RichLogs("Firmware Product " & vbTab & ": ", Color.Black, True, False)
        RichLogs($"{pacHeader("productName")}", Color.Black, True, True)

        RichLogs("Firmware Version " & vbTab & ": ", Color.Black, True, False)
        RichLogs($"{pacHeader("szVersion")}", Color.Black, True, True)
#End Region

        PrintP("Version", pacHeader("szVersion"))
        If Convert.ToInt32(pacHeader("dwHiSize")) = &H0 Then
            RichLogs("Firmware Size " & vbTab & vbTab & ": ", Color.Black, True, False)
            RichLogs($"{GetFileSizes(pacHeader("dwLoSize"))}", Color.Black, True, True)

            PrintP("Size", pacHeader("dwLoSize"))
        Else
            RichLogs("Firmware Size " & vbTab & ": ", Color.Black, True, False)
            RichLogs($"{GetFileSizes(Convert.ToInt64(pacHeader("dwHiSize")) + Convert.ToInt64(pacHeader("dwLoSize")))}", Color.Black, True, True)

            PrintP("Size", Convert.ToInt64(pacHeader("dwHiSize")) + Convert.ToInt64(pacHeader("dwLoSize")))
        End If

        PrintP("PrdName", pacHeader("productName"))
        PrintP("FirmwareName", pacHeader("firmwareName"))
        PrintP("FileCount", pacHeader("partitionCount"))
        PrintP("FileOffset", pacHeader("partitionsListStart"))
        PrintP("Mode", pacHeader("dwMode"))
        PrintP("FlashType", pacHeader("dwFlashType"))
        PrintP("NandStrategy", pacHeader("dwNandStrategy"))
        PrintP("IsNvBackup", pacHeader("dwIsNvBackup"))
        PrintP("NandPageType", pacHeader("dwNandPageType"))
        PrintP("PrdAlias", pacHeader("szPrdAlias"))
        PrintP("OmaDmPrdFlag", pacHeader("dwOmaDmProductFlag"))
        PrintP("IsOmaDM", pacHeader("dwIsOmaDM"))
        PrintP("IsPreload", pacHeader("dwIsPreload"))
        PrintP("Magic", "0x" & (Convert.ToInt64(pacHeader("dwMagic"))).ToString("X").ToLower())
        PrintP("CRC1", pacHeader("wCRC1"))
        PrintP("CRC2", pacHeader("wCRC2"))
        Console.WriteLine()
    End Sub

    Public Shared Function ParsePacHeader(reader As BinaryReader, pacfile As String, debug As Boolean) As Dictionary(Of String, Object)
        Dim pacHeader As New Dictionary(Of String, Object)(PAC_HEADER)

        Dim pacHeaderBytes() As Byte = reader.ReadBytes(Marshal.SizeOf(GetType(PacHeaderStruct)))
        Dim pacHeaderHandle As GCHandle = GCHandle.Alloc(pacHeaderBytes, GCHandleType.Pinned)
        Dim pacHeaderStruct As PacHeaderStruct = DirectCast(Marshal.PtrToStructure(pacHeaderHandle.AddrOfPinnedObject(), GetType(PacHeaderStruct)), PacHeaderStruct)
        pacHeaderHandle.Free()

        pacHeader("szVersion") = GetString(pacHeaderStruct.szVersion)
        pacHeader("dwHiSize") = pacHeaderStruct.dwHiSize
        pacHeader("dwLoSize") = pacHeaderStruct.dwLoSize
        pacHeader("productName") = GetString(pacHeaderStruct.productName)
        pacHeader("firmwareName") = GetString(pacHeaderStruct.firmwareName)
        pacHeader("partitionCount") = pacHeaderStruct.partitionCount
        pacHeader("partitionsListStart") = pacHeaderStruct.partitionsListStart
        pacHeader("dwMode") = pacHeaderStruct.dwMode
        pacHeader("dwFlashType") = pacHeaderStruct.dwFlashType
        pacHeader("dwNandStrategy") = pacHeaderStruct.dwNandStrategy
        pacHeader("dwIsNvBackup") = pacHeaderStruct.dwIsNvBackup
        pacHeader("dwNandPageType") = pacHeaderStruct.dwNandPageType
        pacHeader("szPrdAlias") = GetString(pacHeaderStruct.szPrdAlias)
        pacHeader("dwOmaDmProductFlag") = pacHeaderStruct.dwOmaDmProductFlag
        pacHeader("dwIsOmaDM") = pacHeaderStruct.dwIsOmaDM
        pacHeader("dwIsPreload") = pacHeaderStruct.dwIsPreload
        pacHeader("dwReserved") = pacHeaderStruct.dwReserved
        pacHeader("dwMagic") = pacHeaderStruct.dwMagic
        pacHeader("wCRC1") = pacHeaderStruct.wCRC1
        pacHeader("wCRC2") = pacHeaderStruct.wCRC2

        If debug Then
            RichLogs("", Color.Black, True, True)
            RichLogs("Firmware Information", Color.Black, True, True)
            RichLogs("=============================================================================", Color.Black, True, True)

            PrintPacHeader(pacHeader)
        End If

        If DirectCast(pacHeader("szVersion"), String) <> "BP_R1.0.0" AndAlso DirectCast(pacHeader("szVersion"), String) <> "BP_R2.0.1" Then
            Abort("Unsupported PAC version")
        End If

        Dim dwHiSize As ULong = DirectCast(pacHeader("dwHiSize"), UInteger)
        Dim dwLoSize As ULong = DirectCast(pacHeader("dwLoSize"), UInteger)

        Dim dwSize As ULong = (dwHiSize << 32) + dwLoSize
        Dim fileInfo As New FileInfo(pacfile)
        If dwSize <> CULng(fileInfo.Length) Then
            Abort("Bin packet's size is not correct")
        End If

        Return pacHeader
    End Function

    Public Shared Sub PrintFileHeader(fileHeader As Dictionary(Of String, Object))
        PrintP("Size", fileHeader("length"))
        PrintP("FileID", fileHeader("partitionName"))
        PrintP("FileName", fileHeader("fileName"))
        If Convert.ToInt64(fileHeader("hiPartitionSize")) = &H0 Then
            PrintP("FileSize", fileHeader("loPartitionSize"))
        Else
            PrintP("FileSize", Convert.ToInt64(fileHeader("hiPartitionSize")) + Convert.ToInt64(fileHeader("loPartitionSize")))
        End If
        PrintP("FileFlag", fileHeader("nFileFlag"))
        PrintP("CheckFlag", fileHeader("nCheckFlag"))
        If Convert.ToInt64(fileHeader("hiDataOffset")) = &H0 Then
            PrintP("DataOffset", fileHeader("loDataOffset"))
        Else
            PrintP("DataOffset", Convert.ToInt64(fileHeader("hiDataOffset")) + Convert.ToInt64(fileHeader("loDataOffset")))
        End If
        PrintP("CanOmitFlag", fileHeader("dwCanOmitFlag"))
        Console.WriteLine()
    End Sub

    Public Shared Sub ParseFiles(reader As BinaryReader, fileHeaders As List(Of Dictionary(Of String, Object)), debug As Boolean)
        Dim fileHeader As New Dictionary(Of String, Object)(FILE_HEADER)

        Dim fileHeaderBytes() As Byte = reader.ReadBytes(Marshal.SizeOf(GetType(FileHeaderStruct)))
        Dim fileHeaderHandle As GCHandle = GCHandle.Alloc(fileHeaderBytes, GCHandleType.Pinned)
        Dim fileHeaderStruct As FileHeaderStruct = DirectCast(Marshal.PtrToStructure(fileHeaderHandle.AddrOfPinnedObject(), GetType(FileHeaderStruct)), FileHeaderStruct)
        fileHeaderHandle.Free()

        Dim nFileFlag As UShort = 0

        If fileHeaderStruct.nFileFlag > 0 Then
            nFileFlag = 1
        End If

        Dim nCheckFlag As UShort = 0

        If fileHeaderStruct.nCheckFlag > 0 Then
            nCheckFlag = 1
        End If

        Dim loDataOffset As UInteger = 0
        Dim hiDataOffset As UInteger = 0
        If fileHeaderStruct.hiDataOffset > 0 Then
            loDataOffset = 0
            hiDataOffset = fileHeaderStruct.hiDataOffset
        End If

        fileHeader("length") = fileHeaderStruct.length
        fileHeader("partitionName") = GetString(fileHeaderStruct.partitionName)
        fileHeader("fileName") = GetString(fileHeaderStruct.fileName)
        fileHeader("szFileName") = GetString(fileHeaderStruct.szFileName)
        fileHeader("hiPartitionSize") = fileHeaderStruct.hiPartitionSize
        fileHeader("entahSize1") = fileHeaderStruct.entahSize1
        fileHeader("entahSize2") = fileHeaderStruct.entahSize2
        fileHeader("hiDataOffset") = hiDataOffset
        fileHeader("loPartitionSize") = fileHeaderStruct.loPartitionSize
        fileHeader("nFileFlag") = nFileFlag
        fileHeader("nCheckFlag") = nCheckFlag
        fileHeader("loDataOffset") = loDataOffset
        fileHeader("dwCanOmitFlag") = fileHeaderStruct.dwCanOmitFlag
        fileHeader("dwAddrNum") = fileHeaderStruct.dwAddrNum
        fileHeader("dwAddr") = fileHeaderStruct.dwAddr
        fileHeader("dwReserved") = fileHeaderStruct.dwReserved

        If Convert.ToInt32(fileHeader("length")) <> Marshal.SizeOf(GetType(FileHeaderStruct)) Then
            Console.WriteLine("Unknown Partition Header format found")
        End If

        If debug Then
            PrintFileHeader(fileHeader)
        End If

        fileHeaders.Add(fileHeader)
    End Sub

    Public Shared Sub UnpackPacFile(pacfile As String, outputDir As String, debug As Boolean)
        Using reader As New BinaryReader(File.Open(pacfile, FileMode.Open))
            Dim pacHeader As Dictionary(Of String, Object) = ParsePacHeader(reader, pacfile, debug)

            Dim partitionCount As Integer = Convert.ToInt32(pacHeader("partitionCount"))
            Dim partitionsListStart As Integer = Convert.ToInt32(pacHeader("partitionsListStart"))

            reader.BaseStream.Seek(partitionsListStart, SeekOrigin.Begin)

            Dim fileHeaders As New List(Of Dictionary(Of String, Object))()

            For i As Integer = 0 To partitionCount - 1
                ParseFiles(reader, fileHeaders, debug)
                ProcessBar1(i, partitionCount - 1)
                Delay(0.1)
            Next i

            For Each fileHeader As Dictionary(Of String, Object) In fileHeaders
                Dim partitionName As String = DirectCast(fileHeader("partitionName"), String)
                Dim fileName As String = DirectCast(fileHeader("fileName"), String)
                Dim loDataOffset As ULong = DirectCast(fileHeader("loDataOffset"), UInteger)
                Dim hiDataOffset As ULong = DirectCast(fileHeader("hiDataOffset"), UInteger)
                Dim loPartitionSize As ULong = DirectCast(fileHeader("loPartitionSize"), UInteger)
                Dim hiPartitionSize As ULong = DirectCast(fileHeader("hiPartitionSize"), UInteger)

                If (hiDataOffset + loDataOffset > 0) Then

                    If Equals(partitionName.ToLower(), "fdl") OrElse Equals(partitionName.ToLower(), "fdl1") Then
                        Dim dataOffset As ULong = hiDataOffset + loDataOffset
                        Dim partitionSize As ULong = hiPartitionSize + loPartitionSize

                        reader.BaseStream.Seek(CLng(dataOffset), SeekOrigin.Begin)
                        Dim fileData() As Byte = reader.ReadBytes(CInt(partitionSize))
                        GetFDLFile(outputDir & "\" & fileName, fileData)
                        Main.SharedUI.TxtFDL1.Invoke(CType(Sub() Main.SharedUI.TxtFDL1.Text = outputDir & "\" & fileName, Action))
                    ElseIf Equals(partitionName.ToLower(), "fdl2") Then
                        Dim dataOffset As ULong = hiDataOffset + loDataOffset
                        Dim partitionSize As ULong = hiPartitionSize + loPartitionSize

                        reader.BaseStream.Seek(CLng(dataOffset), SeekOrigin.Begin)
                        Dim fileData() As Byte = reader.ReadBytes(CInt(partitionSize))
                        GetFDLFile(outputDir & "\" & fileName, fileData)
                        Main.SharedUI.TxtFDL2.Invoke(CType(Sub() Main.SharedUI.TxtFDL2.Text = outputDir & "\" & fileName, Action))
                    End If

                    If Not fileName.Contains(".xml") AndAlso Not partitionName.Contains("FDL") AndAlso Not partitionName.Contains("NV_") Then
                        Main.SharedUI.DataView.Invoke(CType(Sub() Main.SharedUI.DataView.Rows.Add(
                                                           True,
                                                           partitionName,
                                                           GetPartitionNames(partitionName),
                                                           hiDataOffset + loDataOffset,
                                                           hiPartitionSize + loPartitionSize,
                                                           "",
                                                           fileName
                                                           ), Action))
                    End If

                    If fileName.Contains("xml") Then
                        Dim dataOffset As ULong = hiDataOffset + loDataOffset
                        Dim partitionSize As ULong = hiPartitionSize + loPartitionSize

                        reader.BaseStream.Seek(CLng(dataOffset), SeekOrigin.Begin)
                        Dim fileData() As Byte = reader.ReadBytes(CInt(partitionSize))
                        File.WriteAllBytes(outputDir & "\" & fileName, fileData)
                        xmlfile = outputDir & "\" & fileName
                        ScanXMLFile(Encoding.UTF8.GetString(fileData))
                    End If
                End If

            Next
            'ExtractFiles(reader, fileHeaders, outputDir)
        End Using
    End Sub

    Public Shared Sub ExtractFiles(reader As BinaryReader, fileHeaders As List(Of Dictionary(Of String, Object)), outputDir As String)
        For Each fileHeader As Dictionary(Of String, Object) In fileHeaders
            Dim fileName As String = DirectCast(fileHeader("fileName"), String)
            Dim loDataOffset As ULong = DirectCast(fileHeader("loDataOffset"), UInteger)
            Dim hiDataOffset As ULong = DirectCast(fileHeader("hiDataOffset"), UInteger)
            Dim loPartitionSize As ULong = DirectCast(fileHeader("loPartitionSize"), UInteger)
            Dim hiPartitionSize As ULong = DirectCast(fileHeader("hiPartitionSize"), UInteger)

            If Not String.IsNullOrEmpty(fileName) Then
                Dim dataOffset As ULong = hiDataOffset + loDataOffset
                Dim partitionSize As ULong = hiPartitionSize + loPartitionSize

                reader.BaseStream.Seek(CLng(dataOffset), SeekOrigin.Begin)
                Dim fileData() As Byte = reader.ReadBytes(CInt(partitionSize))

                Dim outputPath As String = Path.Combine(outputDir, fileName)
                Console.WriteLine(outputPath & " Data Offset : " & dataOffset & " Partition Size : " & partitionSize)
                If File.Exists(outputPath) Then
                    File.Delete(outputPath)
                End If
                File.WriteAllBytes(outputPath, fileData)
            End If

        Next fileHeader
    End Sub

    Public Shared Function ExtractPacData(StartSector As ULong, EndSector As ULong) As Byte()
        Using reader As New BinaryReader(File.Open(Main.SharedUI.TxtPacFirmware.Text, FileMode.Open))
            reader.BaseStream.Seek(CLng(StartSector), SeekOrigin.Begin)
            Return reader.ReadBytes(CInt(EndSector))
        End Using
    End Function

    Public Shared Sub GetFDLFile(outputPath As String, fileData As Byte())
        File.WriteAllBytes(outputPath, fileData)
    End Sub
    Public Shared Sub ScanXMLFile(XMLData As String)
        Dim FDLBase As Integer = 0

        Dim xmlDoc As New XmlDocument()
        xmlDoc.LoadXml(XMLData)

        Dim schemeNode As XmlNode = xmlDoc.SelectSingleNode("/BMAConfig/SchemeList/Scheme")
        Dim schemeName As String = schemeNode.Attributes("name").Value

        Dim fileNodes As XmlNodeList = schemeNode.SelectNodes("File")

        For Each fileNode As XmlNode In fileNodes
            Dim id As String = fileNode.SelectSingleNode("ID").InnerText
            Dim idAlias As String = fileNode.SelectSingleNode("IDAlias").InnerText
            Dim fileType As String = fileNode.SelectSingleNode("Type").InnerText

            Dim blockNode As XmlNode = fileNode.SelectSingleNode("Block")

            Dim blockId As String = ""

            If blockNode.Attributes("id") IsNot Nothing Then
                blockId = blockNode.Attributes("id").Value
            End If

            Dim baseAddress As String = blockNode.SelectSingleNode("Base").InnerText
            Dim size As String = blockNode.SelectSingleNode("Size").InnerText
            Dim flag As String = fileNode.SelectSingleNode("Flag").InnerText
            Dim checkFlag As String = fileNode.SelectSingleNode("CheckFlag").InnerText
            Dim description As String = fileNode.SelectSingleNode("Description").InnerText

            Console.WriteLine($"ID: {id}")
            Console.WriteLine($"IDAlias: {idAlias}")
            Console.WriteLine($"Type: {fileType}")
            Console.WriteLine($"Block ID: {blockId}")
            Console.WriteLine($"Base Address: {baseAddress}")
            Console.WriteLine($"Size: {size}")
            Console.WriteLine($"Flag: {flag}")
            Console.WriteLine($"CheckFlag: {checkFlag}")
            Console.WriteLine($"Description: {description}")
            Console.WriteLine()

            Main.SharedUI.DataView.Invoke(New EventHandler(Sub()
                                                               For Each item As DataGridViewRow In Main.SharedUI.DataView.Rows
                                                                   If item.Cells(Main.SharedUI.DataView.Columns(1).Index).Value = id Then
                                                                       item.Cells(Main.SharedUI.DataView.Columns(2).Index).Value = blockId
                                                                   End If
                                                               Next
                                                           End Sub))

            If idAlias = "FDL1" Then
                Main.SharedUI.TxtFDL1Address.Invoke(CType(Sub() Main.SharedUI.TxtFDL1Address.Text = baseAddress, Action))
            ElseIf idAlias = "FDL2" Then
                Main.SharedUI.TxtFDL2Address.Invoke(CType(Sub() Main.SharedUI.TxtFDL2Address.Text = baseAddress, Action))
            End If

        Next

        Dim xr1 As XmlTextReader
        xr1 = New XmlTextReader(New StringReader(XMLData))
        Do While xr1.Read()
            If xr1.NodeType = XmlNodeType.Element AndAlso xr1.Name = "Partition" Then
                Dim Partition As String = xr1.GetAttribute("id")
                Dim Size As String = xr1.GetAttribute("size")

                Main.SharedUI.DataView.Invoke(New EventHandler(Sub()
                                                                   For Each item As DataGridViewRow In Main.SharedUI.DataView.Rows
                                                                       If item.Cells(Main.SharedUI.DataView.Columns(2).Index).Value = Partition Then
                                                                           If Not Size = "0xFFFFFFFF" Then
                                                                               item.Cells(Main.SharedUI.DataView.Columns(5).Index).Value = Size & "MB"
                                                                           Else
                                                                               item.Cells(Main.SharedUI.DataView.Columns(5).Index).Value = "0xFFFFFFFF"
                                                                           End If
                                                                       ElseIf item.Cells(Main.SharedUI.DataView.Columns(2).Index).Value = "uboot" Then
                                                                           item.Cells(Main.SharedUI.DataView.Columns(5).Index).Value = "1MB"
                                                                       ElseIf item.Cells(Main.SharedUI.DataView.Columns(2).Index).Value = "splloader" Then
                                                                           item.Cells(Main.SharedUI.DataView.Columns(5).Index).Value = "1MB"
                                                                       End If
                                                                    Next
                                                                End Sub))

                Console.WriteLine("Partition Name :" & Partition & " Size : " & Size)
            End If
        Loop
    End Sub
    Public Shared Function GetPartitionNames(Partition As String) As String
        Return Partition.ToLower()
    End Function
    Public Shared Sub StartExtraction(args() As String)
        If args.Length < 2 Then
            Console.WriteLine("Usage: PacFileUnpacker <pacfile> <outputdir> [-debug]")
            Return
        End If

        pacfile = args(0)
        outputDir = args(1)
        debug = False

        If args.Length >= 3 AndAlso args(2) = "-debug" Then
            debug = True
        End If

        UnpackPacFile(pacfile, outputDir, debug)
    End Sub
End Class

<StructLayout(LayoutKind.Sequential, Pack:=1)>
Friend Structure PacHeaderStruct
    <MarshalAs(UnmanagedType.ByValArray, SizeConst:=44)>
    Public szVersion() As Byte
    Public dwHiSize As UInteger
    Public dwLoSize As UInteger
    <MarshalAs(UnmanagedType.ByValArray, SizeConst:=512)>
    Public productName() As Byte
    <MarshalAs(UnmanagedType.ByValArray, SizeConst:=512)>
    Public firmwareName() As Byte
    Public partitionCount As UInteger
    Public partitionsListStart As UInteger
    Public dwMode As UInteger
    Public dwFlashType As UInteger
    Public dwNandStrategy As UInteger
    Public dwIsNvBackup As UInteger
    Public dwNandPageType As UInteger
    <MarshalAs(UnmanagedType.ByValArray, SizeConst:=996)>
    Public szPrdAlias() As Byte
    Public dwOmaDmProductFlag As UInteger
    Public dwIsOmaDM As UInteger
    Public dwIsPreload As UInteger
    Public dwReserved As UInteger
    Public dwMagic As UInteger
    Public wCRC1 As UShort
    Public wCRC2 As UShort
    <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=60)>
    Public reservedData As String
End Structure

<StructLayout(LayoutKind.Sequential, Pack:=1)>
Friend Structure FileHeaderStruct
    Public length As UInteger
    <MarshalAs(UnmanagedType.ByValArray, SizeConst:=512)>
    Public partitionName() As Byte
    <MarshalAs(UnmanagedType.ByValArray, SizeConst:=512)>
    Public fileName() As Byte
    <MarshalAs(UnmanagedType.ByValArray, SizeConst:=512)>
    Public szFileName() As Byte
    Public hiPartitionSize As UInteger
    Public entahSize1 As UInteger
    Public entahSize2 As UInteger
    Public hiDataOffset As UInteger
    Public loPartitionSize As UInteger
    Public nFileFlag As UShort
    Public nCheckFlag As UShort
    Public loDataOffset As UInteger
    Public dwCanOmitFlag As UInteger
    Public dwAddrNum As UInteger
    Public dwAddr As UInteger
    Public dwReserved As UInteger
    <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=996)>
    Public reservedData As String
End Structure

