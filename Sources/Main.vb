Imports UniFlash.USBFastConnect
Imports System.Runtime.InteropServices

Public Class Main

#Region "Disable Sleep"
    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
    Public Shared Function SetThreadExecutionState(ByVal esFlags As EXECUTION_STATE) As EXECUTION_STATE
    End Function

    Public Enum EXECUTION_STATE As UInteger
        ES_SYSTEM_REQUIRED = &H1
        ES_DISPLAY_REQUIRED = &H2
        ES_CONTINUOUS = &H80000000UI
    End Enum

    Public Shared Sub PreventSleep()
        SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS Or EXECUTION_STATE.ES_SYSTEM_REQUIRED Or EXECUTION_STATE.ES_DISPLAY_REQUIRED)
    End Sub

    Public Shared Sub AllowSleep()
        SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS)
    End Sub
#End Region

    Friend Shared SharedUI As Main
    Public Sub New()
        InitializeComponent()
        SharedUI = Me
        getcomInfo()
        PreventSleep()
        AddHandler UnisocWorker.DoWork, AddressOf UnisocWorker_DoWork
        AddHandler UnisocWorker.RunWorkerCompleted, AddressOf UnisocWorker_RunWorkerCompleted
        AddHandler ReceiverDataWorker.DoWork, AddressOf ReceiverDataWorker_DoWork
        AddHandler ReceiverDataWorker.RunWorkerCompleted, AddressOf ReceiverDataWorker_RunWorkerCompleted
        Console.WriteLine()
    End Sub
    Private Sub Main_Closing() Handles MyBase.FormClosing
        AllowSleep()
    End Sub

    Private Sub Logs_TextChanged(sender As Object, e As EventArgs) Handles Logs.TextChanged
        Logs.Invoke(Sub()
                        Logs.SelectionStart = Logs.SelectionStart
                        Logs.ScrollToCaret()
                    End Sub)
    End Sub

    Private Sub ComboPort_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboPort.SelectedIndexChanged
        If ComboPort.Text = "" Then
            CkFDLLoaded.Checked = False
        End If
    End Sub
    Private Sub CkFDLLoaded_CheckedChanged(sender As Object, e As EventArgs) Handles CkFDLLoaded.CheckedChanged
        If CkFDLLoaded.Checked Then
            BtnStart.Text = "Flash"
            WorkerMethod = "Flash"
        Else
            BtnStart.Text = "Download"
            WorkerMethod = "Download"
        End If
    End Sub
    Private Sub CkPartition_CheckedChanged(sender As Object, e As EventArgs) Handles CkPartition.CheckedChanged
        If CkPartition.CheckState = CheckState.Checked Then
            For Each item As DataGridViewRow In DataView.Rows
                For i As Integer = 0 To item.Cells.Count - 1
                    item.Cells(0).Value = True
                Next
            Next
        Else
            For Each item As DataGridViewRow In DataView.Rows
                For i As Integer = 0 To item.Cells.Count - 1
                    item.Cells(0).Value = False
                Next
            Next
        End If
    End Sub
    Private Sub Rdlibusb_CheckedChanged(sender As Object, e As EventArgs) Handles Rdlibusb.CheckedChanged
        If Rdlibusb.Checked Then
            RdDiagChannel.Checked = False
            RdSerialPort.Checked = False
            USBMethod = "libusb-win32"
        End If
    End Sub
    Private Sub RdSerialPort_CheckedChanged(sender As Object, e As EventArgs) Handles RdSerialPort.CheckedChanged
        If RdSerialPort.Checked Then
            RdDiagChannel.Checked = False
            Rdlibusb.Checked = False
            USBMethod = "Serial Port"
        End If
    End Sub
    Private Sub RdDiagChannel_CheckedChanged(sender As Object, e As EventArgs) Handles RdDiagChannel.CheckedChanged
        If RdDiagChannel.Checked Then
            Rdlibusb.Checked = False
            RdSerialPort.Checked = False
            USBMethod = "Diag Channel"
        End If
    End Sub

    Private Sub BtnStart_Click(sender As Object, e As EventArgs) Handles BtnStart.Click
        If Not UnisocWorker.IsBusy Then
            If Not CkFDLLoaded.Checked Then
                RtbClear()
                ProcessBar1(0)
                ProcessBar2(0)
                WorkerMethod = "Download"
                UnisocWorker.RunWorkerAsync()
                UnisocWorker.Dispose()
            Else

                Dim flag As Boolean

                For Each item As DataGridViewRow In DataView.Rows
                    If item.Cells(0).Value = True Then
                        flag = True
                    End If
                Next

                If flag Then

                    RtbClear()
                    ProcessBar1(0)
                    ProcessBar2(0)
                    WorkerMethod = "Flash"

                    StringXML = ""

                    StringXML = String.Concat(StringXML, "<?xml version=""1.0"" ?>" & vbCrLf & "")
                    StringXML = String.Concat(StringXML, "<Partitions>" & vbCrLf & "")


                    totalchecked = 0
                    For Each item As DataGridViewRow In DataView.Rows
                        If item.Cells(DataView.Columns(0).Index).Value = True Then
                            totalchecked += 1

                            StringXML = String.Concat(StringXML, String.Format("<Partition id=""{0}"" startsector=""{1}"" endsector=""{2}"" size=""{3}"" locations=""{4}""/>", New Object() {
                                            item.Cells(DataView.Columns(2).Index).Value.ToString(),                   'id   = partition
                                            item.Cells(DataView.Columns(3).Index).Value.ToString(),                   'id   = startsector
                                            item.Cells(DataView.Columns(4).Index).Value.ToString(),                   'id   = endsector
                                            item.Cells(DataView.Columns(5).Index).Value.ToString().Replace("B", ""),  'id   = partition size
                                            item.Cells(DataView.Columns(6).Index).Value.ToString()                    'size = file locations
                                            }),
                                            "" & vbCrLf & "")

                        End If
                    Next

                    StringXML = String.Concat(StringXML, "</Partitions>")
                End If

                UnisocWorker.RunWorkerAsync()
                UnisocWorker.Dispose()
            End If
        End If
    End Sub

    Private Sub BtnReadPartition_Click(sender As Object, e As EventArgs) Handles BtnReadPartition.Click
        If Not UnisocWorker.IsBusy Then
            Dim flag As Boolean

            For Each item As DataGridViewRow In DataView.Rows
                If item.Cells(0).Value = True Then
                    flag = True
                End If
            Next

            If flag Then
                Dim folderBrowserDialog As New FolderBrowserDialog() With
                                            {
                                            .ShowNewFolderButton = True
                                            }

                If folderBrowserDialog.ShowDialog() = DialogResult.OK Then

                    RtbClear()
                    ProcessBar1(0)
                    ProcessBar2(0)
                    WorkerMethod = "Read Partition"

                    foldersave = folderBrowserDialog.SelectedPath

                    StringXML = ""

                    StringXML = String.Concat(StringXML, "<?xml version=""1.0"" ?>" & vbCrLf & "")
                    StringXML = String.Concat(StringXML, "<Partitions>" & vbCrLf & "")


                    totalchecked = 0
                    For Each item As DataGridViewRow In DataView.Rows
                        If item.Cells(DataView.Columns(0).Index).Value = True Then
                            totalchecked += 1

                            StringXML = String.Concat(StringXML, String.Format("<Partition id=""{0}"" size=""{1}""/>", New Object() {
                                            item.Cells(DataView.Columns(2).Index).Value.ToString(),                   'id   = partition
                                            item.Cells(DataView.Columns(5).Index).Value.ToString().Replace("B", "")  'size = partition size
                                            }),
                                            "" & vbCrLf & "")

                        End If
                    Next

                    StringXML = String.Concat(StringXML, "</Partitions>")
                End If

                UnisocWorker.RunWorkerAsync()
                UnisocWorker.Dispose()
            End If

        End If
    End Sub
    Private Sub BtnErase_Click(sender As Object, e As EventArgs) Handles BtnErase.Click
        'set_chksum_type("add")
        'send_read("a", "20M")
        'If Not UnisocWorker.IsBusy Then
        '    RtbClear()
        '    ProcessBar1(0)
        '    ProcessBar2(0)
        '    WorkerMethod = "Parse"
        '    UnisocWorker.RunWorkerAsync()
        '    UnisocWorker.Dispose()
        'End If

        If Not UnisocWorker.IsBusy Then
            Dim flag As Boolean

            For Each item As DataGridViewRow In DataView.Rows
                If item.Cells(0).Value = True Then
                    flag = True
                End If
            Next

            If flag Then

                RtbClear()
                ProcessBar1(0)
                ProcessBar2(0)
                WorkerMethod = "Erase Partition"

                StringXML = ""

                StringXML = String.Concat(StringXML, "<?xml version=""1.0"" ?>" & vbCrLf & "")
                StringXML = String.Concat(StringXML, "<Partitions>" & vbCrLf & "")


                totalchecked = 0
                For Each item As DataGridViewRow In DataView.Rows
                    If item.Cells(DataView.Columns(0).Index).Value = True Then
                        totalchecked += 1

                        StringXML = String.Concat(StringXML, String.Format("<Partition id=""{0}"" size=""{1}""/>", New Object() {
                                        item.Cells(DataView.Columns(2).Index).Value.ToString(),                   'id   = partition
                                        item.Cells(DataView.Columns(5).Index).Value.ToString().Replace("B", "")  'size = partition size
                                        }),
                                        "" & vbCrLf & "")

                    End If
                Next

                StringXML = String.Concat(StringXML, "</Partitions>")
            End If

            UnisocWorker.RunWorkerAsync()
            UnisocWorker.Dispose()
        End If

    End Sub
    Private Sub BtnPACFirmware_Click(sender As Object, e As EventArgs) Handles BtnPACFirmware.Click
        If Not UnisocWorker.IsBusy Then
            Dim openFileDialog As New OpenFileDialog() With
                        {
                        .Title = "Select PAC Firmware",
                        .InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer),
                        .FileName = "*.*",
                        .Filter = "PAC Firmware |*.pac* ",
                        .FilterIndex = 2,
                        .RestoreDirectory = True
                        }
            If openFileDialog.ShowDialog() = DialogResult.OK Then
                DGVClear()
                RtbClear()
                ProcessBar1(0)
                WorkerMethod = "PAC Firmware"
                TxtPacFirmware.Text = openFileDialog.FileName
                Firmware = openFileDialog.FileName
                UnisocWorker.RunWorkerAsync()
                UnisocWorker.Dispose()
            End If

        End If
    End Sub

End Class
