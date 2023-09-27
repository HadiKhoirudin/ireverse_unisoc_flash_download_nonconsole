<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Main
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.ComboPort = New System.Windows.Forms.ComboBox()
        Me.BtnStart = New System.Windows.Forms.Button()
        Me.UnisocWorker = New System.ComponentModel.BackgroundWorker()
        Me.LabelTimer = New System.Windows.Forms.Label()
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
        Me.ProgressBar2 = New System.Windows.Forms.ProgressBar()
        Me.GroupBoxFlash = New System.Windows.Forms.GroupBox()
        Me.CkFDL2 = New System.Windows.Forms.CheckBox()
        Me.CkFDL1 = New System.Windows.Forms.CheckBox()
        Me.BtnPACFirmware = New System.Windows.Forms.Button()
        Me.BtnFDL2 = New System.Windows.Forms.Button()
        Me.BtnFDL1 = New System.Windows.Forms.Button()
        Me.CkKeepCharge = New System.Windows.Forms.CheckBox()
        Me.CkAutoReboot = New System.Windows.Forms.CheckBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.BtnErase = New System.Windows.Forms.Button()
        Me.BtnReadPartition = New System.Windows.Forms.Button()
        Me.TxtFDL2Address = New System.Windows.Forms.TextBox()
        Me.TxtPacFirmware = New System.Windows.Forms.TextBox()
        Me.TxtFDL2 = New System.Windows.Forms.TextBox()
        Me.TxtFDL1Address = New System.Windows.Forms.TextBox()
        Me.TxtFDL1 = New System.Windows.Forms.TextBox()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.CkPartition = New System.Windows.Forms.CheckBox()
        Me.DataView = New System.Windows.Forms.DataGridView()
        Me.Ck = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.Column1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column2 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column3 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column4 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column5 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column6 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column7 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Logs = New System.Windows.Forms.RichTextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.CkFDLLoaded = New System.Windows.Forms.CheckBox()
        Me.RdSerialPort = New System.Windows.Forms.RadioButton()
        Me.RdDiagChannel = New System.Windows.Forms.RadioButton()
        Me.Rdlibusb = New System.Windows.Forms.RadioButton()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.ReceiverDataWorker = New System.ComponentModel.BackgroundWorker()
        Me.GroupBoxFlash.SuspendLayout()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        CType(Me.DataView, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ComboPort
        '
        Me.ComboPort.FormattingEnabled = True
        Me.ComboPort.Location = New System.Drawing.Point(37, 12)
        Me.ComboPort.Name = "ComboPort"
        Me.ComboPort.Size = New System.Drawing.Size(1076, 21)
        Me.ComboPort.TabIndex = 1
        '
        'BtnStart
        '
        Me.BtnStart.Location = New System.Drawing.Point(1006, 66)
        Me.BtnStart.Name = "BtnStart"
        Me.BtnStart.Size = New System.Drawing.Size(88, 28)
        Me.BtnStart.TabIndex = 2
        Me.BtnStart.Text = "Download"
        Me.BtnStart.UseVisualStyleBackColor = True
        '
        'UnisocWorker
        '
        Me.UnisocWorker.WorkerReportsProgress = True
        Me.UnisocWorker.WorkerSupportsCancellation = True
        '
        'LabelTimer
        '
        Me.LabelTimer.AutoSize = True
        Me.LabelTimer.Location = New System.Drawing.Point(12, 15)
        Me.LabelTimer.Name = "LabelTimer"
        Me.LabelTimer.Size = New System.Drawing.Size(19, 13)
        Me.LabelTimer.TabIndex = 5
        Me.LabelTimer.Text = "[  ]"
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Enabled = False
        Me.ProgressBar1.Location = New System.Drawing.Point(11, 473)
        Me.ProgressBar1.MarqueeAnimationSpeed = 0
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(1102, 16)
        Me.ProgressBar1.TabIndex = 6
        Me.ProgressBar1.Value = 100
        '
        'ProgressBar2
        '
        Me.ProgressBar2.Enabled = False
        Me.ProgressBar2.Location = New System.Drawing.Point(11, 488)
        Me.ProgressBar2.MarqueeAnimationSpeed = 0
        Me.ProgressBar2.Name = "ProgressBar2"
        Me.ProgressBar2.Size = New System.Drawing.Size(1102, 16)
        Me.ProgressBar2.TabIndex = 6
        Me.ProgressBar2.Value = 100
        '
        'GroupBoxFlash
        '
        Me.GroupBoxFlash.Controls.Add(Me.CkFDL2)
        Me.GroupBoxFlash.Controls.Add(Me.CkFDL1)
        Me.GroupBoxFlash.Controls.Add(Me.BtnPACFirmware)
        Me.GroupBoxFlash.Controls.Add(Me.BtnFDL2)
        Me.GroupBoxFlash.Controls.Add(Me.BtnFDL1)
        Me.GroupBoxFlash.Controls.Add(Me.CkKeepCharge)
        Me.GroupBoxFlash.Controls.Add(Me.CkAutoReboot)
        Me.GroupBoxFlash.Controls.Add(Me.Label4)
        Me.GroupBoxFlash.Controls.Add(Me.Label2)
        Me.GroupBoxFlash.Controls.Add(Me.Label3)
        Me.GroupBoxFlash.Controls.Add(Me.Label5)
        Me.GroupBoxFlash.Controls.Add(Me.Label1)
        Me.GroupBoxFlash.Controls.Add(Me.BtnErase)
        Me.GroupBoxFlash.Controls.Add(Me.BtnReadPartition)
        Me.GroupBoxFlash.Controls.Add(Me.BtnStart)
        Me.GroupBoxFlash.Controls.Add(Me.TxtFDL2Address)
        Me.GroupBoxFlash.Controls.Add(Me.TxtPacFirmware)
        Me.GroupBoxFlash.Controls.Add(Me.TxtFDL2)
        Me.GroupBoxFlash.Controls.Add(Me.TxtFDL1Address)
        Me.GroupBoxFlash.Controls.Add(Me.TxtFDL1)
        Me.GroupBoxFlash.Location = New System.Drawing.Point(12, 506)
        Me.GroupBoxFlash.Name = "GroupBoxFlash"
        Me.GroupBoxFlash.Size = New System.Drawing.Size(1101, 103)
        Me.GroupBoxFlash.TabIndex = 9
        Me.GroupBoxFlash.TabStop = False
        '
        'CkFDL2
        '
        Me.CkFDL2.AutoSize = True
        Me.CkFDL2.Checked = True
        Me.CkFDL2.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CkFDL2.Location = New System.Drawing.Point(1079, 48)
        Me.CkFDL2.Name = "CkFDL2"
        Me.CkFDL2.Size = New System.Drawing.Size(15, 14)
        Me.CkFDL2.TabIndex = 18
        Me.CkFDL2.UseVisualStyleBackColor = True
        '
        'CkFDL1
        '
        Me.CkFDL1.AutoSize = True
        Me.CkFDL1.Checked = True
        Me.CkFDL1.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CkFDL1.Location = New System.Drawing.Point(1079, 22)
        Me.CkFDL1.Name = "CkFDL1"
        Me.CkFDL1.Size = New System.Drawing.Size(15, 14)
        Me.CkFDL1.TabIndex = 18
        Me.CkFDL1.UseVisualStyleBackColor = True
        '
        'BtnPACFirmware
        '
        Me.BtnPACFirmware.Location = New System.Drawing.Point(450, 69)
        Me.BtnPACFirmware.Name = "BtnPACFirmware"
        Me.BtnPACFirmware.Size = New System.Drawing.Size(99, 23)
        Me.BtnPACFirmware.TabIndex = 17
        Me.BtnPACFirmware.Text = "Browse"
        Me.BtnPACFirmware.UseVisualStyleBackColor = True
        '
        'BtnFDL2
        '
        Me.BtnFDL2.Location = New System.Drawing.Point(450, 43)
        Me.BtnFDL2.Name = "BtnFDL2"
        Me.BtnFDL2.Size = New System.Drawing.Size(99, 23)
        Me.BtnFDL2.TabIndex = 17
        Me.BtnFDL2.Text = "Browse"
        Me.BtnFDL2.UseVisualStyleBackColor = True
        '
        'BtnFDL1
        '
        Me.BtnFDL1.Location = New System.Drawing.Point(450, 16)
        Me.BtnFDL1.Name = "BtnFDL1"
        Me.BtnFDL1.Size = New System.Drawing.Size(99, 23)
        Me.BtnFDL1.TabIndex = 17
        Me.BtnFDL1.Text = "Browse"
        Me.BtnFDL1.UseVisualStyleBackColor = True
        '
        'CkKeepCharge
        '
        Me.CkKeepCharge.AutoSize = True
        Me.CkKeepCharge.Checked = True
        Me.CkKeepCharge.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CkKeepCharge.Location = New System.Drawing.Point(724, 73)
        Me.CkKeepCharge.Name = "CkKeepCharge"
        Me.CkKeepCharge.Size = New System.Drawing.Size(88, 17)
        Me.CkKeepCharge.TabIndex = 16
        Me.CkKeepCharge.Text = "Keep Charge"
        Me.CkKeepCharge.UseVisualStyleBackColor = True
        '
        'CkAutoReboot
        '
        Me.CkAutoReboot.AutoSize = True
        Me.CkAutoReboot.Checked = True
        Me.CkAutoReboot.CheckState = System.Windows.Forms.CheckState.Checked
        Me.CkAutoReboot.Location = New System.Drawing.Point(632, 73)
        Me.CkAutoReboot.Name = "CkAutoReboot"
        Me.CkAutoReboot.Size = New System.Drawing.Size(86, 17)
        Me.CkAutoReboot.TabIndex = 16
        Me.CkAutoReboot.Text = "Auto Reboot"
        Me.CkAutoReboot.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(552, 48)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(74, 13)
        Me.Label4.TabIndex = 12
        Me.Label4.Text = "FDL2 Address"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(552, 21)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(74, 13)
        Me.Label2.TabIndex = 13
        Me.Label2.Text = "FDL1 Address"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(6, 48)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(33, 13)
        Me.Label3.TabIndex = 14
        Me.Label3.Text = "FDL2"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(6, 74)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(73, 13)
        Me.Label5.TabIndex = 15
        Me.Label5.Text = "PAC Firmware"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(6, 22)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(33, 13)
        Me.Label1.TabIndex = 15
        Me.Label1.Text = "FDL1"
        '
        'BtnErase
        '
        Me.BtnErase.Location = New System.Drawing.Point(818, 66)
        Me.BtnErase.Name = "BtnErase"
        Me.BtnErase.Size = New System.Drawing.Size(88, 28)
        Me.BtnErase.TabIndex = 2
        Me.BtnErase.Text = "Erase Partition"
        Me.BtnErase.UseVisualStyleBackColor = True
        '
        'BtnReadPartition
        '
        Me.BtnReadPartition.Location = New System.Drawing.Point(912, 66)
        Me.BtnReadPartition.Name = "BtnReadPartition"
        Me.BtnReadPartition.Size = New System.Drawing.Size(88, 28)
        Me.BtnReadPartition.TabIndex = 2
        Me.BtnReadPartition.Text = "Read Partition"
        Me.BtnReadPartition.UseVisualStyleBackColor = True
        '
        'TxtFDL2Address
        '
        Me.TxtFDL2Address.Location = New System.Drawing.Point(632, 45)
        Me.TxtFDL2Address.Name = "TxtFDL2Address"
        Me.TxtFDL2Address.Size = New System.Drawing.Size(441, 20)
        Me.TxtFDL2Address.TabIndex = 8
        Me.TxtFDL2Address.Text = "0x9F000000"
        '
        'TxtPacFirmware
        '
        Me.TxtPacFirmware.Location = New System.Drawing.Point(88, 71)
        Me.TxtPacFirmware.Name = "TxtPacFirmware"
        Me.TxtPacFirmware.Size = New System.Drawing.Size(356, 20)
        Me.TxtPacFirmware.TabIndex = 9
        '
        'TxtFDL2
        '
        Me.TxtFDL2.Location = New System.Drawing.Point(88, 45)
        Me.TxtFDL2.Name = "TxtFDL2"
        Me.TxtFDL2.Size = New System.Drawing.Size(356, 20)
        Me.TxtFDL2.TabIndex = 9
        Me.TxtFDL2.Text = "fdl2-sign.bin"
        '
        'TxtFDL1Address
        '
        Me.TxtFDL1Address.Location = New System.Drawing.Point(632, 19)
        Me.TxtFDL1Address.Name = "TxtFDL1Address"
        Me.TxtFDL1Address.Size = New System.Drawing.Size(441, 20)
        Me.TxtFDL1Address.TabIndex = 10
        Me.TxtFDL1Address.Text = "0x50003000"
        '
        'TxtFDL1
        '
        Me.TxtFDL1.Location = New System.Drawing.Point(88, 19)
        Me.TxtFDL1.Name = "TxtFDL1"
        Me.TxtFDL1.Size = New System.Drawing.Size(356, 20)
        Me.TxtFDL1.TabIndex = 11
        Me.TxtFDL1.Text = "fdl1-sign.bin"
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Location = New System.Drawing.Point(559, 39)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.TabControl1.RightToLeftLayout = True
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(554, 424)
        Me.TabControl1.TabIndex = 20
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.CkPartition)
        Me.TabPage1.Controls.Add(Me.DataView)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(546, 398)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Download Tool"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'CkPartition
        '
        Me.CkPartition.AutoSize = True
        Me.CkPartition.Location = New System.Drawing.Point(6, 6)
        Me.CkPartition.Name = "CkPartition"
        Me.CkPartition.Size = New System.Drawing.Size(15, 14)
        Me.CkPartition.TabIndex = 21
        Me.CkPartition.UseVisualStyleBackColor = True
        '
        'DataView
        '
        Me.DataView.AllowUserToAddRows = False
        Me.DataView.AllowUserToDeleteRows = False
        Me.DataView.BackgroundColor = System.Drawing.SystemColors.Window
        Me.DataView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataView.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Ck, Me.Column1, Me.Column2, Me.Column3, Me.Column4, Me.Column5, Me.Column6, Me.Column7})
        Me.DataView.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DataView.Location = New System.Drawing.Point(3, 3)
        Me.DataView.Name = "DataView"
        Me.DataView.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.DataView.RowHeadersVisible = False
        Me.DataView.Size = New System.Drawing.Size(540, 392)
        Me.DataView.TabIndex = 20
        '
        'Ck
        '
        Me.Ck.HeaderText = ""
        Me.Ck.Name = "Ck"
        Me.Ck.Width = 20
        '
        'Column1
        '
        Me.Column1.HeaderText = "File IDs"
        Me.Column1.Name = "Column1"
        Me.Column1.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.Column1.Width = 80
        '
        'Column2
        '
        Me.Column2.HeaderText = "Partitions"
        Me.Column2.Name = "Column2"
        Me.Column2.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.Column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.Column2.Width = 80
        '
        'Column3
        '
        Me.Column3.HeaderText = "Start Sectors"
        Me.Column3.Name = "Column3"
        Me.Column3.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.Column3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.Column3.Width = 80
        '
        'Column4
        '
        Me.Column4.HeaderText = "End Sectors"
        Me.Column4.Name = "Column4"
        Me.Column4.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.Column4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.Column4.Width = 80
        '
        'Column5
        '
        Me.Column5.HeaderText = "Partition Sizes"
        Me.Column5.Name = "Column5"
        Me.Column5.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.Column5.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.Column5.Width = 80
        '
        'Column6
        '
        Me.Column6.HeaderText = "Locations"
        Me.Column6.Name = "Column6"
        Me.Column6.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.Column6.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.Column6.Width = 800
        '
        'Column7
        '
        Me.Column7.HeaderText = ""
        Me.Column7.Name = "Column7"
        Me.Column7.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.Column7.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.Column7.Width = 5
        '
        'Logs
        '
        Me.Logs.Location = New System.Drawing.Point(12, 61)
        Me.Logs.Name = "Logs"
        Me.Logs.Size = New System.Drawing.Size(548, 402)
        Me.Logs.TabIndex = 0
        Me.Logs.Text = ""
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(12, 45)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(30, 13)
        Me.Label6.TabIndex = 15
        Me.Label6.Text = "Logs"
        '
        'CkFDLLoaded
        '
        Me.CkFDLLoaded.AutoSize = True
        Me.CkFDLLoaded.Location = New System.Drawing.Point(570, 40)
        Me.CkFDLLoaded.Name = "CkFDLLoaded"
        Me.CkFDLLoaded.Size = New System.Drawing.Size(85, 17)
        Me.CkFDLLoaded.TabIndex = 21
        Me.CkFDLLoaded.Text = "FDL Loaded"
        Me.CkFDLLoaded.UseVisualStyleBackColor = True
        '
        'RdSerialPort
        '
        Me.RdSerialPort.AutoSize = True
        Me.RdSerialPort.Location = New System.Drawing.Point(398, 39)
        Me.RdSerialPort.Name = "RdSerialPort"
        Me.RdSerialPort.Size = New System.Drawing.Size(73, 17)
        Me.RdSerialPort.TabIndex = 22
        Me.RdSerialPort.Text = "Serial Port"
        Me.RdSerialPort.UseVisualStyleBackColor = True
        '
        'RdDiagChannel
        '
        Me.RdDiagChannel.AutoSize = True
        Me.RdDiagChannel.Checked = True
        Me.RdDiagChannel.Location = New System.Drawing.Point(303, 39)
        Me.RdDiagChannel.Name = "RdDiagChannel"
        Me.RdDiagChannel.Size = New System.Drawing.Size(89, 17)
        Me.RdDiagChannel.TabIndex = 23
        Me.RdDiagChannel.TabStop = True
        Me.RdDiagChannel.Text = "Diag Channel"
        Me.RdDiagChannel.UseVisualStyleBackColor = True
        '
        'Rdlibusb
        '
        Me.Rdlibusb.AutoSize = True
        Me.Rdlibusb.Location = New System.Drawing.Point(477, 39)
        Me.Rdlibusb.Name = "Rdlibusb"
        Me.Rdlibusb.Size = New System.Drawing.Size(83, 17)
        Me.Rdlibusb.TabIndex = 22
        Me.Rdlibusb.Text = "libusb-win32"
        Me.Rdlibusb.UseVisualStyleBackColor = True
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(227, 41)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(70, 13)
        Me.Label7.TabIndex = 15
        Me.Label7.Text = "Connection : "
        '
        'ReceiverDataWorker
        '
        Me.ReceiverDataWorker.WorkerReportsProgress = True
        Me.ReceiverDataWorker.WorkerSupportsCancellation = True
        '
        'Main
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Window
        Me.ClientSize = New System.Drawing.Size(1125, 621)
        Me.Controls.Add(Me.CkFDLLoaded)
        Me.Controls.Add(Me.RdDiagChannel)
        Me.Controls.Add(Me.Rdlibusb)
        Me.Controls.Add(Me.RdSerialPort)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.GroupBoxFlash)
        Me.Controls.Add(Me.ProgressBar2)
        Me.Controls.Add(Me.ProgressBar1)
        Me.Controls.Add(Me.LabelTimer)
        Me.Controls.Add(Me.ComboPort)
        Me.Controls.Add(Me.Logs)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label6)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.Name = "Main"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "iReverse Unisoc Flash Download Non-Console"
        Me.GroupBoxFlash.ResumeLayout(False)
        Me.GroupBoxFlash.PerformLayout()
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage1.PerformLayout()
        CType(Me.DataView, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Public WithEvents ComboPort As ComboBox
    Public WithEvents BtnStart As Button
    Public WithEvents UnisocWorker As System.ComponentModel.BackgroundWorker
    Public WithEvents LabelTimer As Label
    Public WithEvents ProgressBar1 As ProgressBar
    Public WithEvents ProgressBar2 As ProgressBar
    Friend WithEvents GroupBoxFlash As GroupBox
    Friend WithEvents BtnPACFirmware As Button
    Friend WithEvents BtnFDL2 As Button
    Friend WithEvents BtnFDL1 As Button
    Friend WithEvents CkKeepCharge As CheckBox
    Friend WithEvents CkAutoReboot As CheckBox
    Friend WithEvents Label4 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents Label1 As Label
    Public WithEvents TxtFDL2Address As TextBox
    Public WithEvents TxtPacFirmware As TextBox
    Public WithEvents TxtFDL2 As TextBox
    Public WithEvents TxtFDL1Address As TextBox
    Public WithEvents TxtFDL1 As TextBox
    Public WithEvents CkFDL2 As CheckBox
    Public WithEvents CkFDL1 As CheckBox
    Public WithEvents BtnReadPartition As Button
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents TabPage1 As TabPage
    Public WithEvents CkPartition As CheckBox
    Friend WithEvents DataView As DataGridView
    Friend WithEvents Ck As DataGridViewCheckBoxColumn
    Friend WithEvents Column1 As DataGridViewTextBoxColumn
    Friend WithEvents Column2 As DataGridViewTextBoxColumn
    Friend WithEvents Column3 As DataGridViewTextBoxColumn
    Friend WithEvents Column4 As DataGridViewTextBoxColumn
    Friend WithEvents Column5 As DataGridViewTextBoxColumn
    Friend WithEvents Column6 As DataGridViewTextBoxColumn
    Friend WithEvents Column7 As DataGridViewTextBoxColumn
    Public WithEvents Logs As RichTextBox
    Friend WithEvents Label6 As Label
    Friend WithEvents CkFDLLoaded As CheckBox
    Public WithEvents BtnErase As Button
    Public WithEvents RdSerialPort As RadioButton
    Public WithEvents RdDiagChannel As RadioButton
    Public WithEvents Rdlibusb As RadioButton
    Friend WithEvents Label7 As Label
    Public WithEvents ReceiverDataWorker As System.ComponentModel.BackgroundWorker
End Class
