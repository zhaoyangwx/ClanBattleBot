
Imports System.Text
Imports Newtonsoft.Json

<Serializable>
Public Class BotManager
    Public Groups As New List(Of ClanBattle)
    Public AdminQQ As New List(Of String)
    Public Sub Init()
        For Each g As ClanBattle In Groups
            AddHandler g.SendMessage, Sub(Msg As String)
                                          If Msg.Length > 4000 Then Msg = $"消息过长，只保留前4000字{vbCrLf}" & Msg.Substring(0, 4000)
                                          Msg = JsonConvert.SerializeObject(Msg)
                                          Msg = Msg.Substring(1, Msg.Length - 2)

                                          RaiseEvent SendMessage($"{{""action"":""send_group_msg"",""params"":{{""group_id"":""{g.Group_ID}"",""message"":""{Msg}""}}}}")
                                      End Sub
            g.Init()
        Next
    End Sub

    Public Event SendMessage(msg As String)
    Public Sub ReplyMsg(msg As String, Source As CQMessage)
        If Source.message_type = "group" Then
            RaiseEvent SendMessage($"{{""action"":""send_group_msg"",""params"":{{""group_id"":""{Source.group_id}"",""message"":""{msg}""}}}}")
        Else
            RaiseEvent SendMessage($"{{""action"":""send_private_msg"",""params"":{{""user_id"":""{Source.user_id}"",""group_id"":""{Source.group_id}"",""message"":""{msg}""}}}}")
        End If
    End Sub
    Public Function CheckAdmin(MsgObj As CQMessage) As Boolean
        If Not AdminQQ.Contains(MsgObj.user_id.ToString) AndAlso AdminQQ.Count > 0 Then
            ReplyMsg($"无权操作，需要超管权限", MsgObj)
            Return False
        End If
        Return True
    End Function
    Public Sub ProcessMessage(msg As String)
        If msg.StartsWith("{""post_type"":""message""") Then
            msg = msg.Replace("：", ":")
            Dim MsgObj As CQMessage = CQMessage.FromJSON(msg)
            MsgObj.raw_message = Net.WebUtility.HtmlDecode(MsgObj.raw_message)
            Dim msgraw As String = MsgObj.raw_message
            If MsgObj IsNot Nothing Then
                'Public Msg
                If msgraw.StartsWith("echo") Then
                    ReplyMsg(msgraw.Substring(4).TrimStart(), MsgObj)
                    Exit Sub
                ElseIf msgraw.StartsWith("设置超管") Then
                    SetSuperAdmin(MsgObj)
                    Exit Sub
                ElseIf msgraw.StartsWith("取消超管") Then
                    RemoveSuperAdmin(MsgObj)
                    Exit Sub
                ElseIf msgraw.StartsWith("超管列表") Then
                    Dim s As New StringBuilder
                    For Each su As String In AdminQQ
                        s.AppendLine(su)
                    Next
                    ReplyMsg(s.ToString(), MsgObj)
                    Exit Sub
                ElseIf msgraw.StartsWith("设置保存") Then
                    Save(Application.StartupPath & "\" & "botdata.xml")
                    ReplyMsg($"Bot数据库已保存", MsgObj)
                    Exit Sub
                ElseIf msgraw.StartsWith("启用会战") Then
                    CheckInitClanbattle(MsgObj)
                    Exit Sub
                ElseIf msgraw = "指令列表" Or msgraw = "指令" Or msgraw = "帮助" Then
                    ReplyMsg("===== BOT管理指令 =====
*设置超管：将指定用户设置为超管，后面跟qq号或者at某人；如果超管列表为空则无需权限直接添加
*取消超管：取消指定用户超管权限，后面跟qq号或者at某人（必须）
设置保存：保存bot数据库
启用会战：为当前群建立会战档案，如果已存在则启用并更新超管列表到群管列表

*：需要超管权限
==========", MsgObj)
                End If

                If MsgObj.message_type = "group" Then
                    For Each g As ClanBattle In Groups
                        If g.Group_ID = MsgObj.group_id.ToString AndAlso g.Enabled Then
                            'Group Msg 
                            g.ProcessMessage(msgraw, MsgObj)
                            Exit Sub
                        End If
                    Next
                Else
                    'Private Msg

                End If
            End If
        End If
    End Sub
    Public Sub SetSuperAdmin(msgObj As CQMessage)
        Dim msgraw As String = msgObj.raw_message
        If Not CheckAdmin(msgObj) Then Exit Sub
        Dim param As String = MsgUtils.ParseCQatParam(msgraw, 4)
        If param = "" Then param = msgObj.user_id.ToString
        If Not AdminQQ.Contains(param) Then
            AdminQQ.Add(param)
            ReplyMsg($"{param}设置为超级管理员", msgObj)
        Else
            ReplyMsg($"{param}已经是超级管理员", msgObj)
        End If
    End Sub
    Public Sub RemoveSuperAdmin(msgObj As CQMessage)
        Dim msgraw As String = msgObj.raw_message
        If Not CheckAdmin(msgObj) Then Exit Sub
        Dim param As String = MsgUtils.ParseCQatParam(msgraw, 4)
        If AdminQQ.Contains(param) Then
            AdminQQ.Remove(param)
            ReplyMsg($"{param}取消超级管理员", msgObj)
        Else
            ReplyMsg($"{param}不是超级管理员", msgObj)
        End If
    End Sub
    Public Sub CheckInitClanbattle(msgObj As CQMessage)
        Dim gCurr As ClanBattle = Nothing
        For Each g As ClanBattle In Groups
            If g.Group_ID = msgObj.group_id.ToString() Then
                gCurr = g
                Exit For
            End If
        Next
        If gCurr IsNot Nothing Then
            For Each adm As String In AdminQQ
                If Not gCurr.GroupAdmin.Contains(adm) Then gCurr.GroupAdmin.Add(adm)
            Next
            gCurr.Enabled = True
            ReplyMsg($"{msgObj.group_id}已存在，会战功能已启用，超管权限已刷新", msgObj)
        Else
            Dim g As New ClanBattle With {.Group_ID = msgObj.group_id}
            For Each adm As String In AdminQQ
                If Not g.GroupAdmin.Contains(adm) Then g.GroupAdmin.Add(adm)
            Next
            g.GroupAdmin.Add(msgObj.user_id.ToString)
            Groups.Add(g)
            AddHandler g.SendMessage, Sub(Msg As String)
                                          Msg = JsonConvert.SerializeObject(Msg)
                                          Msg = Msg.Substring(1, Msg.Length - 2)
                                          RaiseEvent SendMessage($"{{""action"":""send_group_msg"",""params"":{{""group_id"":""{g.Group_ID}"",""message"":""{Msg}""}}}}")
                                      End Sub
            g.Init()
            ReplyMsg($"{msgObj.group_id}初始化完成，已将{msgObj.user_id}以及全部超管设置为群管，请加入公会成员", msgObj)
        End If
    End Sub

    Public Sub Save(FileName As String)
        Dim writer As New System.Xml.Serialization.XmlSerializer(GetType(BotManager))
        Dim ms As New IO.FileStream(FileName, IO.FileMode.Create)
        Dim t As IO.TextWriter = New IO.StreamWriter(ms, New System.Text.UTF8Encoding(False))
        writer.Serialize(t, Me)
        ms.Close()
    End Sub
    Public Shared Function Load(FileName As String) As BotManager
        Dim reader As New System.Xml.Serialization.XmlSerializer(GetType(BotManager))
        Dim t As IO.TextReader = New IO.StringReader(My.Computer.FileSystem.ReadAllText(FileName))
        Dim result As BotManager = CType(reader.Deserialize(t), BotManager)
        Return result
    End Function
End Class
