<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class mainForm
    Inherits System.Windows.Forms.Form

    'Form 重写 Dispose，以清理组件列表。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意: 以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.OpenBtn = New System.Windows.Forms.Button()
        Me.urlInput = New System.Windows.Forms.TextBox()
        Me.CloseBtn = New System.Windows.Forms.Button()
        Me.msgInputBox = New System.Windows.Forms.TextBox()
        Me.SendBtn = New System.Windows.Forms.Button()
        Me.msgListbox = New System.Windows.Forms.TextBox()
        Me.ButtonLoad = New System.Windows.Forms.Button()
        Me.ButtonSave = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'OpenBtn
        '
        Me.OpenBtn.Location = New System.Drawing.Point(12, 12)
        Me.OpenBtn.Name = "OpenBtn"
        Me.OpenBtn.Size = New System.Drawing.Size(75, 23)
        Me.OpenBtn.TabIndex = 0
        Me.OpenBtn.Text = "连接"
        Me.OpenBtn.UseVisualStyleBackColor = True
        '
        'urlInput
        '
        Me.urlInput.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.urlInput.Location = New System.Drawing.Point(93, 12)
        Me.urlInput.Name = "urlInput"
        Me.urlInput.Size = New System.Drawing.Size(442, 21)
        Me.urlInput.TabIndex = 1
        '
        'CloseBtn
        '
        Me.CloseBtn.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CloseBtn.Location = New System.Drawing.Point(541, 12)
        Me.CloseBtn.Name = "CloseBtn"
        Me.CloseBtn.Size = New System.Drawing.Size(75, 23)
        Me.CloseBtn.TabIndex = 3
        Me.CloseBtn.Text = "断开"
        Me.CloseBtn.UseVisualStyleBackColor = True
        '
        'msgInputBox
        '
        Me.msgInputBox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.msgInputBox.Location = New System.Drawing.Point(12, 324)
        Me.msgInputBox.Multiline = True
        Me.msgInputBox.Name = "msgInputBox"
        Me.msgInputBox.Size = New System.Drawing.Size(604, 128)
        Me.msgInputBox.TabIndex = 4
        '
        'SendBtn
        '
        Me.SendBtn.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SendBtn.Location = New System.Drawing.Point(541, 458)
        Me.SendBtn.Name = "SendBtn"
        Me.SendBtn.Size = New System.Drawing.Size(75, 23)
        Me.SendBtn.TabIndex = 5
        Me.SendBtn.Text = "发送"
        Me.SendBtn.UseVisualStyleBackColor = True
        '
        'msgListbox
        '
        Me.msgListbox.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.msgListbox.Location = New System.Drawing.Point(12, 41)
        Me.msgListbox.Multiline = True
        Me.msgListbox.Name = "msgListbox"
        Me.msgListbox.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.msgListbox.Size = New System.Drawing.Size(604, 277)
        Me.msgListbox.TabIndex = 6
        Me.msgListbox.Text = "go-cqhttp设置参考：" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "servers:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "  - ws:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "      # 正向WS服务器监听地址" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "      address: 0.0.0.0:18" &
    "088" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "      middlewares:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "        <<: *default"
        Me.msgListbox.WordWrap = False
        '
        'ButtonLoad
        '
        Me.ButtonLoad.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ButtonLoad.Location = New System.Drawing.Point(12, 458)
        Me.ButtonLoad.Name = "ButtonLoad"
        Me.ButtonLoad.Size = New System.Drawing.Size(75, 23)
        Me.ButtonLoad.TabIndex = 7
        Me.ButtonLoad.Text = "读取"
        Me.ButtonLoad.UseVisualStyleBackColor = True
        '
        'ButtonSave
        '
        Me.ButtonSave.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ButtonSave.Location = New System.Drawing.Point(93, 458)
        Me.ButtonSave.Name = "ButtonSave"
        Me.ButtonSave.Size = New System.Drawing.Size(75, 23)
        Me.ButtonSave.TabIndex = 8
        Me.ButtonSave.Text = "保存"
        Me.ButtonSave.UseVisualStyleBackColor = True
        '
        'mainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(628, 493)
        Me.Controls.Add(Me.ButtonSave)
        Me.Controls.Add(Me.ButtonLoad)
        Me.Controls.Add(Me.msgListbox)
        Me.Controls.Add(Me.SendBtn)
        Me.Controls.Add(Me.msgInputBox)
        Me.Controls.Add(Me.CloseBtn)
        Me.Controls.Add(Me.urlInput)
        Me.Controls.Add(Me.OpenBtn)
        Me.Name = "mainForm"
        Me.Text = "会战助手"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents OpenBtn As System.Windows.Forms.Button
    Friend WithEvents urlInput As System.Windows.Forms.TextBox
    Friend WithEvents CloseBtn As System.Windows.Forms.Button
    Friend WithEvents msgInputBox As System.Windows.Forms.TextBox
    Friend WithEvents SendBtn As System.Windows.Forms.Button
    Friend WithEvents msgListbox As System.Windows.Forms.TextBox
    Friend WithEvents ButtonLoad As Button
    Friend WithEvents ButtonSave As Button
End Class
