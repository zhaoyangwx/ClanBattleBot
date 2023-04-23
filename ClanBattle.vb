Imports System.Text
Imports Newtonsoft.Json

<Serializable>
Public Class ClanBattle
    Public Property Group_ID As String
    Public Property Enabled As Boolean = True
    Public Property GroupMember As New List(Of Member)
    Public Property GroupAdmin As New List(Of String)

    <Serializable>
    Public Class Member
        Public Property QQID As String
        Public Property NickName As String
        Public Property FullChance As Long = 3
        Public Property FragChance As Long = 0
        Public Property SLUsed As Boolean = False
        Public Property SLTime As Date
        Public ReadOnly Property ChanceSummary As String
            Get
                Dim res As String = ""
                If FullChance > 0 Then res &= $"{FullChance}整"
                If FragChance > 0 Then res &= $"{FragChance}补"
                Return res
            End Get
        End Property

    End Class
    Public Function GetMemberOrCreateTemp(qqid As String) As Member
        For Each m As Member In GroupMember
            If m.QQID = qqid Then Return m
        Next
        Reply($"{qqid}未加入公会，将作为临时成员不计入次数统计。请发送""加入公会""")
        Return New Member With {.QQID = qqid, .NickName = $"{qqid}(未加入)"}
    End Function
    Public Property Boss As New List(Of BossInfo)
    <Serializable>
    Public Class BossInfo
        Public Property Name As String
        Public Structure HPLimitItem
            Public Value As Long
            Public RoundLimit As Long
        End Structure
        Public Property MaxHP As New List(Of HPLimitItem)
        Public ReadOnly Property CurrentMaxHP As Long
            Get
                For i As Long = 0 To MaxHP.Count - 1
                    If MaxHP(i).RoundLimit >= CurrentRound Then
                        Return MaxHP(i).Value
                    End If
                Next
                Return -1
            End Get
        End Property

        Public Property CurrentHP As Long
        Public Property CurrentRound As Long
        <Xml.Serialization.XmlIgnore>
        Public ReadOnly Property CurrentStage As String
            Get
                Return GetStage(CurrentRound)
            End Get
        End Property
        Public Function GetStage(RoundNum) As String
            For i As Long = 0 To Math.Min(MaxHP.Count - 1, 25)
                If MaxHP(i).RoundLimit >= RoundNum Then
                    Return Chr(Asc("A") + i)
                End If
            Next
            Return "?"
        End Function

        Public Shared Function [Default](Name As String) As BossInfo
            Dim result As New BossInfo With {.Name = Name, .CurrentRound = 1}
            Select Case Name
                Case "1"
                    Name = "一王"
                Case "2"
                    Name = "二王"
                Case "3"
                    Name = "三王"
                Case "4"
                    Name = "四王"
                Case "5"
                    Name = "五王"
            End Select
            Select Case Name
                Case "一王"
                    result.MaxHP.Add(New HPLimitItem With {.RoundLimit = 3, .Value = 6000000})
                    result.MaxHP.Add(New HPLimitItem With {.RoundLimit = 10, .Value = 6000000})
                    result.MaxHP.Add(New HPLimitItem With {.RoundLimit = 34, .Value = 7000000})
                    result.MaxHP.Add(New HPLimitItem With {.RoundLimit = 44, .Value = 17000000})
                    result.MaxHP.Add(New HPLimitItem With {.RoundLimit = 9999, .Value = 85000000})
                    result.CurrentHP = 6000000
                Case "二王"
                    result.MaxHP.Add(New HPLimitItem With {.RoundLimit = 3, .Value = 8000000})
                    result.MaxHP.Add(New HPLimitItem With {.RoundLimit = 10, .Value = 8000000})
                    result.MaxHP.Add(New HPLimitItem With {.RoundLimit = 34, .Value = 9000000})
                    result.MaxHP.Add(New HPLimitItem With {.RoundLimit = 44, .Value = 18000000})
                    result.MaxHP.Add(New HPLimitItem With {.RoundLimit = 9999, .Value = 90000000})
                    result.CurrentHP = 8000000
                Case "三王"
                    result.MaxHP.Add(New HPLimitItem With {.RoundLimit = 3, .Value = 10000000})
                    result.MaxHP.Add(New HPLimitItem With {.RoundLimit = 10, .Value = 10000000})
                    result.MaxHP.Add(New HPLimitItem With {.RoundLimit = 34, .Value = 12000000})
                    result.MaxHP.Add(New HPLimitItem With {.RoundLimit = 44, .Value = 20000000})
                    result.MaxHP.Add(New HPLimitItem With {.RoundLimit = 9999, .Value = 95000000})
                    result.CurrentHP = 10000000
                Case "四王"
                    result.MaxHP.Add(New HPLimitItem With {.RoundLimit = 3, .Value = 12000000})
                    result.MaxHP.Add(New HPLimitItem With {.RoundLimit = 10, .Value = 12000000})
                    result.MaxHP.Add(New HPLimitItem With {.RoundLimit = 34, .Value = 15000000})
                    result.MaxHP.Add(New HPLimitItem With {.RoundLimit = 44, .Value = 21000000})
                    result.MaxHP.Add(New HPLimitItem With {.RoundLimit = 9999, .Value = 100000000})
                    result.CurrentHP = 12000000
                Case "五王"
                    result.MaxHP.Add(New HPLimitItem With {.RoundLimit = 3, .Value = 15000000})
                    result.MaxHP.Add(New HPLimitItem With {.RoundLimit = 10, .Value = 15000000})
                    result.MaxHP.Add(New HPLimitItem With {.RoundLimit = 34, .Value = 20000000})
                    result.MaxHP.Add(New HPLimitItem With {.RoundLimit = 44, .Value = 23000000})
                    result.MaxHP.Add(New HPLimitItem With {.RoundLimit = 9999, .Value = 110000000})
                    result.CurrentHP = 15000000
            End Select
            Return result
        End Function
        Public Sub ResetHP()
            For i As Long = 0 To MaxHP.Count - 1
                If MaxHP(i).RoundLimit >= CurrentRound Then
                    CurrentHP = MaxHP(i).Value
                    Exit Sub
                End If
            Next
        End Sub
    End Class
    Public Sub PushAppointment(BossNum As Long) '从1开始
        Dim pushList As New List(Of String)
        For Each q As QueueInfo In Queue
            If q.QName = $"预约表" Then
                For Each qm As QueueInfo.QItem In q.QMember
                    If qm.QInfo = $"预约{BossNum}" AndAlso Not pushList.Contains(qm.QMember.QQID) Then
                        pushList.Add(qm.QMember.QQID)
                    End If
                Next
                Exit For
            End If
        Next

        If pushList.Count > 0 Then
            Dim ReplyMsg As String = $"BOSS {BossNum} 已经可以挑战"
            For Each qid As String In pushList
                ReplyMsg &= $"[CQ:at,qq={qid}]"
            Next
            Reply(ReplyMsg)
        End If
    End Sub
    Public Sub UpdateBossStatus()
        Dim RoundMin As Long = Long.MaxValue
        Dim bossnum As Long = 0
        For Each b As BossInfo In Boss
            bossnum += 1
            If b.CurrentHP > 0 Then RoundMin = Math.Min(b.CurrentRound, RoundMin)
            If b.CurrentHP = 0 Then
                For Each q As QueueInfo In Queue
                    If q.QName = $"树{bossnum}" Then
                        q.QMember.Clear()
                        Exit For
                    End If
                Next
            End If
        Next
        Dim HPSum As Long = 0
        bossnum = 0
        For Each b As BossInfo In Boss
            bossnum += 1
            HPSum += b.CurrentHP
            If b.CurrentHP = 0 Then
                If b.CurrentRound + 1 - RoundMin > 1 Then Continue For
                b.CurrentRound += 1
                Dim NextStage As String = ""
                For Each bn As BossInfo In Boss
                    NextStage &= bn.CurrentStage
                Next
                If NextStage.Replace(NextStage.Substring(0, 1), "") <> "" Then
                    b.CurrentRound -= 1
                    Continue For
                Else
                    b.ResetHP()
                    PushAppointment(bossnum)
                    HPSum += b.CurrentHP
                End If
            End If
        Next
        If HPSum = 0 Then
            bossnum = 0
            For Each b As BossInfo In Boss
                bossnum += 1
                b.CurrentRound += 1
                b.ResetHP()
                PushAppointment(bossnum)
            Next
            For Each q As QueueInfo In Queue
                If q.QName = "薛定谔的树" Then
                    q.QMember.Clear()
                End If
            Next
        End If

    End Sub
    Public Sub ResetDate()
        For Each m As Member In GroupMember
            m.FullChance = 3
            m.FragChance = 0
            m.SLUsed = False
        Next
        'Reply($"日期已更新 {Last_Battle_Time.ToString("yyyyMMddHHmmss")}->{Now.ToString("yyyyMMddHHmmss")}")
        Last_Battle_Time = Now
    End Sub
    Public Property Last_Battle_Time As Date
    Public Sub ResetAll()
        ResetDate()
        Last_Save_MemberID = ""
        Generation = 0
        BattleEventNum += 1
        For Each b As BossInfo In Boss
            b.CurrentRound = 1
            b.ResetHP()
        Next
        For Each q As QueueInfo In Queue
            q.QMember.Clear()
        Next

    End Sub
    Public Function SetDamage(BossNum As Long, Damage As Long) As Boolean
        Dim SelectedBoss As BossInfo = Boss(BossNum)
        If SelectedBoss.CurrentHP <= Damage Then Return False
        Last_Battle_Time = Now
        Threading.Interlocked.Add(SelectedBoss.CurrentHP, -Damage)
        UpdateBossStatus()
        Return True
    End Function
    Public Function CheckQDefeat(QNum As Long)
        Dim ReplyString As New StringBuilder
        For Each q As QueueInfo In Queue
            If q.QName = $"树{QNum}" Then
                ReplyString.Append($"下树提醒")
                For Each qm As QueueInfo.QItem In q.QMember
                    ReplyString.Append(MsgUtils.CQAt(qm.QMember.QQID))
                Next
                Exit For
            End If
        Next
        If ReplyString.Length > 4 Then
            Reply(ReplyString.ToString())
        End If
    End Function
    Public Function SetDefeat(BossNum As Long) As Boolean
        Dim SelectedBoss As BossInfo = Boss(BossNum)
        If SelectedBoss.CurrentHP = 0 Then Return False
        Last_Battle_Time = Now
        CheckQDefeat(BossNum + 1)
        SelectedBoss.CurrentHP = 0
        UpdateBossStatus()
        Return True
    End Function
    Public Sub CheckDate()
        If Generation = 0 Then
            Last_Battle_Time = Now
            Exit Sub
        End If
        Dim CurrentDate As Date = Now
        If (CurrentDate - Last_Battle_Time).TotalHours >= 24 Then
            ResetDate()
            Exit Sub
        End If
        If CurrentDate.Day = Last_Battle_Time.Day Then
            If CurrentDate.Hour >= 5 And Last_Battle_Time.Hour < 5 Then
                ResetDate()
                Exit Sub
            End If
            Exit Sub
        End If
        If Last_Battle_Time.Hour < 5 Then
            ResetDate()
            Exit Sub
        End If
        If CurrentDate.Hour >= 5 Then
            ResetDate()
            Exit Sub
        End If
    End Sub
    Public Function GetBossJSON() As String
        Return JsonConvert.SerializeObject(Boss)
    End Function
    Public Shared Function SetBossFromJSON(JSONText As String) As List(Of BossInfo)
        Return JsonConvert.DeserializeObject(Of List(Of BossInfo))(JSONText)
    End Function

    Public Queue As New List(Of QueueInfo)
    <Serializable>
    Public Class QueueInfo
        Public Property QName As String
        Public QMember As New List(Of QItem)
        Public Class QItem
            Public Property QMember As Member
            Public Property QInfo As String
            Public Property Message As String
            Public Property CreateTime As Date = Now
            Public ReadOnly Property PassedTime As String
                Get
                    Dim tsum As TimeSpan = Now - CreateTime
                    Dim h As Long = Math.Floor(tsum.TotalHours)
                    Dim m As Long = tsum.Minutes
                    If h >= 24 Then Return "24小时+"
                    If h > 0 Then
                        Return $"{h}时{m}分"
                    Else
                        Return $"{m}分"
                    End If

                End Get
            End Property
        End Class
    End Class
    Public Property Generation As Long
    Public Property BattleEventNum As Long
    Public Class Snapshot
        Public Property Last_Save_MemberID As String
        Public Property Last_Battle_Time As Date
        Public Property GroupMember As List(Of Member)
        Public Property Boss As List(Of BossInfo)
        Public Property Queue As List(Of QueueInfo)
    End Class
    Public Property Last_Save_MemberID As String = ""
    Public Sub SaveSnapShot()
        Dim SnapshotPath As String = Application.StartupPath & "\" & "snapshot"
        If Not My.Computer.FileSystem.DirectoryExists(SnapshotPath) Then My.Computer.FileSystem.CreateDirectory(SnapshotPath)
        My.Computer.FileSystem.WriteAllText($"{SnapshotPath}\SnapShot_group{Group_ID}_battle{BattleEventNum}_gen{Generation}.json",
                                            JsonConvert.SerializeObject(New Snapshot With {.Boss = Boss,
                                            .GroupMember = GroupMember,
                                            .Queue = Queue,
                                            .Last_Save_MemberID = Last_Save_MemberID,
                                            .Last_Battle_Time = Last_Battle_Time}), False)
        Generation += 1
    End Sub
    Public Function RollBack() As Boolean
        Dim SnapshotPath As String = Application.StartupPath & "\" & "snapshot"
        If Not My.Computer.FileSystem.DirectoryExists(SnapshotPath) Then Return False
        Dim SFile As String = $"{SnapshotPath}\SnapShot_group{Group_ID}_battle{BattleEventNum}_gen{Generation - 1}.json"
        If Not My.Computer.FileSystem.FileExists(SFile) Then Return False
        Dim spoint As Snapshot = JsonConvert.DeserializeObject(Of Snapshot)(My.Computer.FileSystem.ReadAllText(SFile))
        If spoint Is Nothing Then Return False
        If spoint.Last_Save_MemberID Is Nothing Then Return False
        If spoint.Boss Is Nothing Then Return False
        If spoint.GroupMember Is Nothing Then Return False
        If spoint.Queue Is Nothing Then Return False
        Last_Save_MemberID = spoint.Last_Save_MemberID
        Last_Battle_Time = spoint.Last_Battle_Time
        Generation -= 1
        Boss = spoint.Boss
        GroupMember = spoint.GroupMember
        Queue = spoint.Queue
        Return True
    End Function
    Public Event SendMessage(msg As String)
    <Xml.Serialization.XmlIgnore>
    Private LastMsg As String = ""
    Private DupeMsgCount As Integer = 0
    Private LastMsgTime As Date
    Public Sub Reply(msg As String)
        If LastMsg = msg AndAlso (Now - LastMsgTime).TotalSeconds < 60 Then
            DupeMsgCount += 1
            Threading.Thread.Sleep(200 * DupeMsgCount)
            If DupeMsgCount >= 2 Then
                RaiseEvent SendMessage("检测到重复消息，请检查是否存在互相触发")
                Exit Sub
            End If
        Else
            DupeMsgCount = 0
        End If
        LastMsg = msg
        LastMsgTime = Now
        RaiseEvent SendMessage(msg)
    End Sub
    Public Function CheckGroupAdmin(sender As String) As Boolean
        If Not GroupAdmin.Contains(sender) Then
            Reply("需要群管权限，超管请输入启用会战刷新权限")
            Return False
        End If
        Return True
    End Function
    Public Sub Init()
        Dim replymsg As String = $"正在启动BOT{vbCrLf}"
        If Boss.Count = 0 Then
            replymsg &= ($"BOSS未设置，已设为默认值{vbCrLf}")
            Boss.Add(BossInfo.Default("一王"))
            Boss.Add(BossInfo.Default("二王"))
            Boss.Add(BossInfo.Default("三王"))
            Boss.Add(BossInfo.Default("四王"))
            Boss.Add(BossInfo.Default("五王"))
        End If
        If Queue.Count = 0 Then
            replymsg &= $"已重建队列{vbCrLf}"
            Queue.Add(New QueueInfo With {.QName = "预约表"})
            Queue.Add(New QueueInfo With {.QName = "树1"})
            Queue.Add(New QueueInfo With {.QName = "树2"})
            Queue.Add(New QueueInfo With {.QName = "树3"})
            Queue.Add(New QueueInfo With {.QName = "树4"})
            Queue.Add(New QueueInfo With {.QName = "树5"})
            Queue.Add(New QueueInfo With {.QName = "薛定谔的树"})
        End If
        replymsg &= $"成员数：{GroupMember.Count}{vbCrLf}"
        replymsg &= $"档案编号：{BattleEventNum}{vbCrLf}"
        replymsg &= $"队列统计：{vbCrLf}"
        For Each q As QueueInfo In Queue
            replymsg &= $"  {q.QName}：{q.QMember.Count};"
        Next
        replymsg &= vbCrLf
        replymsg &= BattleStat()
        Exit Sub
        Reply(replymsg)
    End Sub
    Public Sub ProcessMessage(msg As String, MsgObj As CQMessage)
        Try
            CheckDate()
            If msg.StartsWith("设置群管") Then
                SetAdmin(msg, MsgObj)
                Exit Sub
            ElseIf msg.StartsWith("取消群管") Then
                RemoveAdmin(msg, MsgObj)
                Exit Sub
            ElseIf msg.StartsWith("群管列表") OrElse msg.StartsWith("群管一览") Then
                Dim r As New StringBuilder
                r.AppendLine($"群{Group_ID}共有{GroupAdmin.Count}位管理员")
                For Each s As String In GroupAdmin
                    r.AppendLine(s)
                Next
                Reply(r.ToString)
                Exit Sub
            ElseIf msg.StartsWith("加入公会") Or msg.StartsWith("加入工会") Then
                JoinMember(msg, MsgObj)
                Exit Sub
            ElseIf msg.StartsWith("退出公会") Or msg.StartsWith("退出工会") Then
                RemoveMember(msg, MsgObj)
                Exit Sub
            ElseIf msg.StartsWith("查刀") Then
                MemberStat(msg, MsgObj, True)
                Exit Sub
            ElseIf msg.StartsWith("催刀") Then
                If Not CheckGroupAdmin(MsgObj.user_id) Then Exit Sub
                MemberStat(msg, MsgObj, True, True)
                Exit Sub
            ElseIf msg.StartsWith("成员列表") OrElse msg.StartsWith("成员一览") Then
                MemberStat(msg, MsgObj, False)
                Exit Sub
            ElseIf msg.StartsWith("改名") Then
                RenameNick(msg.Substring(2), MsgObj)
                Exit Sub
            ElseIf msg.StartsWith("状态") OrElse msg.StartsWith("进度") OrElse msg.StartsWith("查树") Then
                Reply(BattleStat)
                Exit Sub
            ElseIf msg.StartsWith("挂树") Then
                EnQueue(msg.Substring(2), "树", MsgObj)
                Exit Sub
            ElseIf msg.StartsWith("下树") Then
                DeQueue(msg.Substring(2), "树", MsgObj)
                Exit Sub
            ElseIf msg.ToLower.StartsWith("sl") Then
                SL(msg.Substring(2), MsgObj)
                Exit Sub
            ElseIf msg.ToLower.StartsWith("取消sl") Or msg.ToLower.StartsWith("撤销sl") Then
                SL("取消" & msg.Substring(4), MsgObj)
                Exit Sub
            ElseIf msg.StartsWith("进") Then
                If msg.StartsWith("进了") Then
                    EnQueue(msg.Substring(2), "进", MsgObj)
                Else
                    EnQueue(msg.Substring(1), "进", MsgObj)
                End If
                Exit Sub
            ElseIf msg = "预约表" Then
                Reply(BattleStat(True))
                Exit Sub
            ElseIf msg.StartsWith("查") Then
                ListAppoint(msg.Substring(1))
                Exit Sub
            ElseIf msg.StartsWith("预约成功") OrElse msg.StartsWith("预约表") Then
            ElseIf msg.StartsWith("预约失败") Then
            ElseIf msg.StartsWith("预约") Then
                EnQueue(msg.Substring(2), "预约", MsgObj)
                Exit Sub
            ElseIf msg.StartsWith("取消预约") Then
                DeQueue(msg.Substring(4), "预约", MsgObj)
                Exit Sub
            ElseIf msg.StartsWith("报刀") Or msg.StartsWith("尾刀") Then
                Reply(BDHelp())
                Exit Sub
            ElseIf msg.StartsWith("报") Then
                BattleDamage(msg.Substring(1), MsgObj)
            ElseIf msg.StartsWith("合刀") Then
                Hedao(msg.Substring(2), MsgObj)
            ElseIf msg = "撤销" Then
                If MsgObj.user_id.ToString() = Last_Save_MemberID OrElse GroupAdmin.Contains(MsgObj.user_id.ToString()) Then
                    If RollBack() Then
                        Reply($"已回滚至{Generation}代记录：{vbCrLf}{BattleStat()}")
                    Else
                        Reply($"回滚失败：记录不存在")
                    End If
                Else
                    Reply($"回滚失败：需要群管权限，或者上次报刀的qq号")
                End If
            ElseIf msg = "重置状态" Or msg = "切换档案" Then
                If Not CheckGroupAdmin(MsgObj.user_id) Then Exit Sub
                ResetAll()
                Reply($"已重置会战信息，当前档案{BattleEventNum}{vbCrLf}{BattleStat()}")
            ElseIf msg = "禁用会战" Then
                DisableClanBattle(MsgObj)
                Exit Sub
            ElseIf msg.ToUpper.StartsWith("BOSS参数") Then
                BossSetting(msg, MsgObj)
            ElseIf msg = "指令列表" Or msg = "指令" Or msg = "帮助" Then
                Reply(CMDHelp())
            End If
        Catch ex As Exception
            Reply(ex.ToString)
        End Try

    End Sub
    Public Sub SetAdmin(msg As String, MsgObj As CQMessage)
        Dim sender As String = MsgObj.user_id
        If Not CheckGroupAdmin(sender) Then Exit Sub
        Dim param As String = MsgUtils.ParseCQatParam(msg, 4)
        If param = "" Then param = sender
        If Not GroupAdmin.Contains(param) Then
            GroupAdmin.Add(param)
            Reply($"已将{param}设置为群管")
        Else
            Reply($"{param}已经是群管")
        End If
    End Sub
    Public Sub RemoveAdmin(msg As String, MsgObj As CQMessage)
        Dim sender As String = MsgObj.user_id
        If Not CheckGroupAdmin(sender) Then Exit Sub
        Dim param As String = MsgUtils.ParseCQatParam(msg, 4)
        If param = "" Then
            param = sender
        End If
        For Each m As String In GroupAdmin
            If m = param Then
                Reply($"{param}已取消群管")
                Exit Sub
            End If
        Next
        Reply($"{param}还不是群管")
    End Sub
    Public Sub JoinMember(msg As String, MsgObj As CQMessage)
        Dim sender As String = MsgObj.user_id
        Dim param As String = MsgUtils.ParseCQatParam(msg, 4)
        Dim nickname As String = param
        If param = "" Then
            param = sender
            nickname = MsgObj.sender.card
            If nickname = "" Then nickname = MsgObj.sender.nickname
        End If
        For Each m As Member In GroupMember
            If m.QQID = param Then
                If nickname <> param Then
                    m.NickName = nickname
                    Reply($"{nickname}已经是公会成员，备注更新成功")
                Else
                    Reply($"{nickname}已经是公会成员")
                End If
                Exit Sub
            End If
        Next
        GroupMember.Add(New Member With {.QQID = param, .NickName = nickname})
        Reply($"{nickname}已加入公会")
    End Sub
    Public Sub RemoveMember(msg As String, MsgObj As CQMessage)
        Dim sender As String = MsgObj.user_id
        Dim param As String = MsgUtils.ParseCQatParam(msg, 4)
        Dim nickname As String = param
        If param = "" Then
            param = sender
            nickname = MsgObj.sender.card
        Else
            If Not CheckGroupAdmin(sender) Then Exit Sub
        End If
        For i As Integer = 0 To GroupMember.Count - 1
            Dim m As Member = GroupMember(i)
            If m.QQID = param Then
                GroupMember.Remove(m)
                Reply($"{nickname}退出公会成功")
                Exit Sub
            End If
        Next
        Reply($"{nickname}还不是公会成员")
    End Sub
    Public Sub MemberStat(msg As String, MsgObj As CQMessage, Optional ByVal CheckBattle As Boolean = False, Optional ByVal Send_At As Boolean = False)
        Dim result As New StringBuilder
        result.AppendLine($"群{MsgObj.group_id}共有{GroupMember.Count}位公会成员")
        result.AppendLine()
        GroupMember.Sort(New Comparison(Of Member)(Function(a As Member, b As Member) As Long
                                                       If a.FullChance <> b.FullChance Then
                                                           Return b.FullChance.CompareTo(a.FullChance)
                                                       Else
                                                           Return b.FragChance.CompareTo(a.FragChance)
                                                       End If
                                                   End Function))
        If CheckBattle Then result.AppendLine("未出完刀的成员：")
        For Each m As Member In GroupMember
            Dim RespondNick As String = m.NickName.PadRight(12)
            If Send_At Then RespondNick = MsgUtils.CQAt(m.QQID)
            If Not CheckBattle OrElse m.FullChance + m.FragChance > 0 Then result.Append($"{RespondNick}")
            If Not CheckBattle OrElse m.FullChance + m.FragChance > 0 Then result.Append($" 剩余")
            If Not CheckBattle OrElse m.FullChance > 0 Then result.Append($"{m.FullChance}整")
            If Not CheckBattle OrElse m.FragChance > 0 Then result.Append($"{m.FragChance}补")
            result.AppendLine()
        Next
        Reply(result.ToString)
    End Sub
    Public Sub RenameNick(msg As String, MsgObj As CQMessage)
        Dim scq As String() = MsgUtils.GetBehalf(msg)
        Dim behalf As String = scq(0)
        If behalf <> MsgObj.user_id AndAlso Not CheckGroupAdmin(MsgObj.user_id) Then Exit Sub
        scq(1) = scq(1).TrimStart(" ").TrimEnd(" ")
        If scq(1).Length = 0 Then
            Reply("名字不能为空")
            Exit Sub
        End If
        If behalf = "" Then behalf = MsgObj.user_id
        For Each m As Member In GroupMember
            If m.QQID = behalf Then
                m.NickName = scq(1)
                Reply($"{m.QQID}={m.NickName}")
                Exit For
            End If
        Next
    End Sub
    Public Sub EnQueue(param As String, qType As String, MsgObj As CQMessage)
        Dim btest As String() = MsgUtils.GetBehalf(param)
        Dim behalf As String = btest(0)
        param = btest(1)
        param = param.TrimStart(" ")
        If behalf = "" Then behalf = MsgObj.user_id
        Dim digiTest As String() = MsgUtils.ReadNum(param)
        Dim qnum As Long = 0
        If digiTest(0) <> "" Then qnum = Long.Parse(digiTest(0))
        param = digiTest(1)
        param = param.TrimStart(" ")
        param = param.TrimStart(":")

        Dim QName As String
        If qnum <= 5 AndAlso qnum > 0 Then
            QName = $"树{qnum}"
        Else
            QName = "薛定谔的树"
        End If
        Select Case qType
            Case "树"
                ClearQMember(QName, behalf)
                For Each q As QueueInfo In Queue
                    If q.QName = QName Then
                        If param = "" Then param = $"挂树[CQ:face,id={(New Random).Next(0, 247)}]"
                        q.QMember.Add(New QueueInfo.QItem With {.QMember = GetMemberOrCreateTemp(behalf), .Message = param, .QInfo = QName})
                        Exit For
                    End If
                Next
                Reply($"已挂树{vbCrLf}{BattleStat()}")
            Case "进"
                ClearQMember(QName, behalf)
                ClearQMember(QName, behalf)
                Dim appomsg As String = POPAppointment(qnum, behalf)

                If param = "" Then param = appomsg
                If param <> "" Then param = $": {param}"
                For Each q As QueueInfo In Queue
                    If q.QName = QName Then
                        q.QMember.Add(New QueueInfo.QItem With {.QMember = GetMemberOrCreateTemp(behalf), .Message = $"在打{param}", .QInfo = QName})
                        Exit For
                    End If
                Next
                Reply($"已进刀{vbCrLf}{BattleStat()}")
            Case "预约"
                If qnum < 1 Or qnum > 5 Then
                    Reply("预约失败：不存在这样的BOSS")
                    Exit Sub
                End If

                QName = "预约表"
                For Each q As QueueInfo In Queue
                    If q.QName = QName Then
                        q.QMember.Add(New QueueInfo.QItem With {.QMember = GetMemberOrCreateTemp(behalf), .Message = param, .QInfo = $"预约{qnum}"})
                        Exit For
                    End If
                Next
                Reply($"{MsgUtils.CQAt(MsgObj.user_id)}预约成功")
        End Select
    End Sub
    Public Sub SL(param As String, MsgObj As CQMessage)
        Dim pdata As String() = MsgUtils.GetBehalf(param)
        Dim behalf As String = pdata(0)
        If behalf = "" Then behalf = MsgObj.user_id
        Dim cmd As String = pdata(1).Replace("？", "?").TrimStart
        Dim m As Member = Nothing
        For Each mb As Member In GroupMember
            If mb.QQID = behalf Then
                m = mb
                Exit For
            End If
        Next
        If m Is Nothing Then
            Reply($"{behalf}未加入公会，无SL记录")
            Exit Sub
        End If
        If cmd.StartsWith("?") Then
            If m.SLUsed Then
                Reply($"{m.NickName} SL已用，于{m.SLTime.ToString("yyyy-MM-dd HH:mm:ss")}")
            Else
                Reply($"{m.NickName} SL未用")
            End If
        ElseIf cmd.StartsWith("取消") Or cmd.StartsWith("撤销") Then
            If m.SLUsed Then
                m.SLUsed = False
                Reply($"{m.NickName} 已取消SL")
            Else
                Reply($"{m.NickName} 没有SL记录")
            End If

        ElseIf cmd = "" Then
            If m.SLUsed Then
                Reply($"错误：{m.NickName} SL已用，于{m.SLTime.ToString("yyyy-MM-dd HH:mm:ss")}")
            Else
                m.SLUsed = True
                m.SLTime = Now
                Reply($"{m.NickName} 已记录SL")
            End If

        End If
    End Sub
    Public Function ClearQMember(QName As String, QQID As String) As Boolean
        For Each q As QueueInfo In Queue
            If q.QName = QName Then
                Dim target As QueueInfo.QItem = Nothing
                For Each qm As QueueInfo.QItem In q.QMember
                    If qm.QMember.QQID = QQID Then
                        target = qm
                        Exit For
                    End If
                Next
                If target IsNot Nothing Then
                    q.QMember.Remove(target)
                    Return True
                Else
                    Return False
                End If
            End If
        Next
        Return False
    End Function
    Public Function POPAppointment(qnum As Long, qqid As String) As String
        For Each q As QueueInfo In Queue
            If q.QName = "预约表" Then
                For i As Long = 0 To q.QMember.Count - 1
                    Dim qm As QueueInfo.QItem = q.QMember(i)
                    If qm.QInfo = $"预约{qnum}" AndAlso qm.QMember.QQID.ToString = qqid Then
                        q.QMember.Remove(qm)
                        Return qm.Message
                    End If
                Next
            End If
        Next
        Return ""
    End Function
    Public Sub DeQueue(param As String, qType As String, MsgObj As CQMessage)
        Dim btest As String() = MsgUtils.GetBehalf(param)
        Dim behalf As String = btest(0)
        param = btest(1)
        param = param.TrimStart(" ")
        If behalf = "" Then behalf = MsgObj.user_id
        Dim digiTest As String() = MsgUtils.ReadNum(param)
        Dim qnum As Long = 0
        If digiTest(0) <> "" Then qnum = Long.Parse(digiTest(0))
        param = digiTest(1)
        param = param.TrimStart(" ")
        param = param.TrimStart(":")

        Dim QName As String
        If qnum <= 5 AndAlso qnum > 0 Then
            QName = $"树{qnum}"
        Else
            QName = "薛定谔的树"
        End If
        Select Case qType
            Case "树"
                If ClearQMember(QName, behalf) Then
                    Reply("已下树")
                Else
                    Reply($"您不在{QName}上")
                End If
            Case "预约"
                QName = "预约表"
                If ClearQMember(QName, behalf) Then
                    Reply("取消预约成功")
                Else
                    Reply("没有预约记录")
                End If


        End Select


    End Sub
    Public Function BDHelp() As String
        Return ($"示例：
报1整12345678
报1补123456
报1整收
报1补收")
    End Function
    Public Sub BattleDamage(msg As String, MsgObj As CQMessage)
        Dim btest As String() = MsgUtils.GetBehalf(msg)
        Dim behalf As String = btest(0)
        If behalf = "" Then behalf = MsgObj.user_id
        msg = btest(1)
        msg = msg.ToLower
        msg = msg.Replace(" ", "").Replace(":", "").Replace(",", "").Replace("-", "")
        msg = msg.Replace("w", "0000").Replace("万", "0000").Replace("亿", "00000000").Replace("e", "00000000").Replace("m", "000000").Replace("k", "000").Replace("千", "000")
        Dim parseBN As String() = MsgUtils.ReadNum(msg)

        If parseBN(0) = "" OrElse msg.Length <= 2 Then
            Reply($"指令格式有误{vbCrLf}{BDHelp()}")
            Exit Sub
        End If
        Dim bossnum As Long = Long.Parse(parseBN(0))
        If bossnum <= 0 OrElse bossnum > 5 Then
            Reply($"BOSS编号有误{vbCrLf}{BDHelp()}")
            Exit Sub
        End If
        bossnum -= 1
        Dim BDParam As String = parseBN(1).Substring(1)
        Dim currmem As Member = Nothing
        Dim NickName As String = behalf
        For Each m As Member In GroupMember
            If m.QQID = behalf Then
                currmem = m
                NickName = m.NickName
                Exit For
            End If
        Next
        If currmem Is Nothing Then Reply($"警告：未加入公会，但仍可使用报刀功能，不会计入次数！代报请使用@")
        Select Case parseBN(1)(0)
            Case "整"
                SaveSnapShot()
                For Each q As QueueInfo In Queue
                    If q.QName = $"树{bossnum + 1}" Then
                        For i As Long = 0 To q.QMember.Count - 1
                            Dim qm As QueueInfo.QItem = q.QMember(i)
                            If qm.QMember.QQID = behalf Then
                                q.QMember.Remove(qm)
                                Exit For
                            End If
                        Next
                        Exit For
                    End If
                Next
                If currmem IsNot Nothing AndAlso currmem.FullChance > 0 Then
                    currmem.FullChance -= 1
                ElseIf currmem IsNot Nothing Then
                    RollBack()
                    Reply($"{NickName} 报刀失败：今天完整刀次数已用完，请检查报刀指令")
                    Exit Sub
                End If
                If BDParam.StartsWith("收") Or BDParam.StartsWith("尾") Then
                    If currmem IsNot Nothing Then currmem.FragChance += 1
                    If SetDefeat(bossnum) Then
                        Dim MsgDBDInfo As String = ""
                        If currmem IsNot Nothing Then
                            MsgDBDInfo = $"今日第{3 - currmem.FullChance}刀，完整刀{vbCrLf}"
                        End If
                        Last_Save_MemberID = MsgObj.user_id
                        Reply($"{BattleStat()}{NickName} {MsgDBDInfo}击败了BOSS")
                        Exit Sub
                    Else
                        RollBack()
                        Reply($"{NickName} 报刀失败：BOSS已经被击败")
                        Exit Sub
                    End If
                Else
                    Dim dmg As String = MsgUtils.ReadNum(BDParam)(0)
                    If dmg = "" Then
                        RollBack()
                        Reply($"指令格式有误{vbCrLf}{BDHelp()}")
                        Exit Sub
                    End If
                    If SetDamage(bossnum, Long.Parse(dmg)) Then
                        Dim MsgDBDInfo As String = ""
                        If currmem IsNot Nothing Then
                            MsgDBDInfo = $"今日第{3 - currmem.FullChance}刀，完整刀{vbCrLf}"
                        End If
                        Last_Save_MemberID = MsgObj.user_id
                        Reply($"{BattleStat()}{NickName} {MsgDBDInfo}对BOSS造成{dmg}伤害")
                        Exit Sub
                    Else
                        RollBack()
                        Reply($"{NickName} 报刀失败：超出剩余血量，如击败请使用""收""")
                        Exit Sub
                    End If
                End If
            Case "补"
                SaveSnapShot()
                For Each q As QueueInfo In Queue
                    If q.QName = $"树{bossnum + 1}" Then
                        For i As Long = 0 To q.QMember.Count - 1
                            Dim qm As QueueInfo.QItem = q.QMember(i)
                            If qm.QMember.QQID = behalf Then
                                q.QMember.Remove(qm)
                                Exit For
                            End If
                        Next
                        Exit For
                    End If
                Next
                If currmem IsNot Nothing AndAlso currmem.FragChance > 0 Then
                    currmem.FragChance -= 1
                Else
                    RollBack()
                    Reply($"{NickName} 报刀失败：无剩余补偿刀次数，请检查报刀指令")
                    Exit Sub
                End If
                If BDParam.StartsWith("收") Or BDParam.StartsWith("尾") Then
                    If SetDefeat(bossnum) Then
                        Dim MsgDBDInfo As String = ""
                        If currmem IsNot Nothing Then
                            MsgDBDInfo = $"今日第{3 - currmem.FullChance}刀，补偿刀{vbCrLf}"
                        End If
                        Last_Save_MemberID = MsgObj.user_id
                        Reply($"{BattleStat()}{NickName} {MsgDBDInfo}击败了BOSS")
                        Exit Sub
                    Else
                        RollBack()
                        Reply($"{NickName} 报刀失败：BOSS已经被击败")
                        Exit Sub
                    End If
                Else
                    Dim dmg As String = MsgUtils.ReadNum(BDParam)(0)
                    If dmg = "" Then
                        RollBack()
                        Reply($"指令格式有误{vbCrLf}{BDHelp()}")
                        Exit Sub
                    End If
                    If SetDamage(bossnum, Long.Parse(dmg)) Then
                        Dim MsgDBDInfo As String = ""
                        If currmem IsNot Nothing Then
                            MsgDBDInfo = $"{NickName} 补偿刀已出，当前剩余{currmem.FragChance}刀补偿{vbCrLf}"
                        End If
                        Last_Save_MemberID = MsgObj.user_id
                        Reply($"{BattleStat()}{NickName} {MsgDBDInfo}对BOSS造成{dmg}伤害")
                        Exit Sub
                    Else
                        RollBack()
                        Reply($"{NickName} 报刀失败：超出剩余血量，如击败请使用""收""")
                        Exit Sub
                    End If
                End If
            Case Else
                Reply($"报刀类型有误{vbCrLf}{BDHelp()}")
                Exit Sub
        End Select
    End Sub
    Public Sub Hedao(msg As String, MsgObj As CQMessage)
        Dim paramList As New List(Of Long)
        msg = msg.ToLower
        msg = msg.Replace("w", "0000").Replace("万", "0000").Replace("亿", "00000000").Replace("e", "00000000").Replace("m", "000000").Replace("k", "000").Replace("千", "000")
        msg = msg & " "
        Dim stack As String = ""
        For i As Long = 0 To msg.Length - 1
            If MsgUtils.IsNumeric(msg(i)) Then
                stack &= msg(i)
            Else
                If stack <> "" Then
                    paramList.Add(Long.Parse(stack))
                    stack = ""
                End If
            End If
        Next
        Select Case paramList.Count
            Case 2
                Dim d1 As Long = paramList(0)
                Dim d2 As Long = paramList(1)
                If d1 >= d2 Then
                    Reply($"剩余血量：{d2}{vbCrLf}满补需要再垫：{Math.Ceiling(Math.Max(0, d2 - (1 - (89.01 - 20) / 90) * d1))}")
                Else
                    Reply($"剩余血量：{d2}{vbCrLf}第一刀 {d1}{vbCrLf}第二刀满补需要伤害 {Math.Ceiling((d2 - d1) * (1 / (1 - (89.01 - 20) / 90)))}")
                End If
            Case 3
                Dim d1 As Long = paramList(0)
                Dim d2 As Long = paramList(1)
                Dim d3 As Long = paramList(2)
                If d1 + d2 < d3 Then
                    Reply("醒醒！这两刀是打不死boss的")
                Else
                    Reply($"剩余血量：{d3}{vbCrLf}{d1 _
                        } 先出，另一刀可获得 {Math.Min(90, Math.Round((1 - (d3 - d1) / Math.Max(d2, 0.0000000001)) * 90 + 20, 2)) _
                        } 秒补偿刀{vbCrLf}{d2} 先出，另一刀可获得 {Math.Min(90, Math.Round((1 - (d3 - d2) / Math.Max(d1, 0.0000000001)) * 90 + 20, 2))} 秒补偿刀")
                End If
            Case 4
                Dim d1 As Long = paramList(0)
                Dim d2 As Long = paramList(1)
                Dim t As Long = paramList(2)
                Dim s As Long = paramList(3)
                If d1 + d2 < t Then
                    Reply("醒醒！这两刀是打不死boss的")
                Else
                    Reply($"{d1} 先出，另一刀{d2}/{s}可获得 {t}->{Math.Min(90, Math.Max(0, Math.Round((1 - (s - d1) / Math.Max(d2, 0.0000000001)) * (90 - t) + t + 20, 2)))} 秒补偿刀")
                End If
            Case Else
                Reply($"合刀参数：d1-完整刀伤害; d2-收尾刀伤害; t-收尾刀余时; s-完整刀前的BOSS初始血量{vbCrLf}
补偿时间t2=(1-(s-d1)/d2)*(90-t)+t+20向上取整{vbCrLf}
用法：{vbCrLf}
用法一：补偿时间比较，输入d1 & d2、s，t=0，求t2{vbCrLf}
    合刀 刀1伤害 刀2伤害 BOSS血量{vbCrLf}
用法二：计算第二刀满补所需伤害，输入d1、s，t=0，t2=20，求d2{vbCrLf}
    合刀 伤害 剩余血量{vbCrLf}
用法三：计算需要垫多少满补，输入d2、s-d1+x，t=0，t2=20，求x{vbCrLf}
    合刀 伤害 剩余血量{vbCrLf}
用法四：能一刀收有剩余时间，垫伤害凹补偿时间，输入d1、d2、t、s，求t2{vbCrLf}
    合刀 完整刀伤害 收尾刀伤害 收尾刀余时 BOSS初始血量")
                Exit Sub
        End Select
    End Sub
    Public Sub echo(msg As String, MsgObj As CQMessage)

    End Sub
    Public Sub BossSetting(msg As String, MsgObj As CQMessage)
        Dim sender As String = MsgObj.user_id
        Dim param As String = msg.Substring(6)
        If param = "" Then
            Dim replymsg As String = ""
            If Boss.Count = 0 Then
                replymsg = ($"BOSS未设置，已设为默认值{vbCrLf}")
                Boss.Add(BossInfo.Default("一王"))
                Boss.Add(BossInfo.Default("二王"))
                Boss.Add(BossInfo.Default("三王"))
                Boss.Add(BossInfo.Default("四王"))
                Boss.Add(BossInfo.Default("五王"))
            End If
            replymsg &= (GetBossJSON())
            Reply(replymsg)
        Else
            If Not CheckGroupAdmin(sender) Then Exit Sub
            Try
                Dim newbosslist As List(Of BossInfo) = SetBossFromJSON(param)
                If newbosslist IsNot Nothing Then
                    Boss = newbosslist
                    Reply($"BOSS参数已更新，当前BOSS数量：{Boss.Count}")
                Else
                    Reply("BOSS参数未更新")
                End If
            Catch ex As Exception
                Reply($"BOSS参数未更新{vbCrLf}{ex.ToString()}")
            End Try
        End If
    End Sub
    Public Function BattleStat(Optional ByVal yybOnly As Boolean = False) As String
        Dim result As New StringBuilder
        Dim FullRemain, FragRemain, TotalSum As Long
        For Each gm As Member In GroupMember
            FullRemain += gm.FullChance
            FragRemain += gm.FragChance
            TotalSum += 3
        Next
        result.AppendLine($"今天已出{(TotalSum - FullRemain - FragRemain / 2)}刀，剩{FullRemain}整{FragRemain}补")
        If Not yybOnly Then
            result.AppendLine("Boss状态：")
            Dim bossNum As Long = 0
            For Each b As BossInfo In Boss
                bossNum += 1
                Dim HPValue As String = ""
                If b.CurrentHP \ 10000 > 0 Then
                    HPValue &= b.CurrentHP \ 10000
                    HPValue &= "," & (b.CurrentHP Mod 10000).ToString.PadLeft(4, "0")
                Else
                    HPValue = b.CurrentHP
                End If
                If b.CurrentHP > 0 Then
                    HPValue &= $" ({Math.Round(b.CurrentHP / b.CurrentMaxHP * 100)}%)"
                Else
                    HPValue &= $" (不可挑战)"
                End If
                result.AppendLine($"{b.Name}: {b.CurrentRound,2}({b.CurrentStage}) 剩 {HPValue}")
                For Each q As QueueInfo In Queue
                    If q.QName = $"树{bossNum}" Then
                        For Each qi As QueueInfo.QItem In q.QMember
                            'Dim rest As String = qi.QMember.ChanceSummary
                            'If rest <> "" Then rest = $"余{rest}"
                            Dim rest As String = $"{qi.QMember.FullChance}|{qi.QMember.FragChance}"
                            If qi.Message <> "" Then rest = $"{rest}:{qi.Message}"
                            result.AppendLine($"┗{qi.QMember.NickName}({qi.PassedTime}){rest}")
                        Next
                    End If
                Next
                result.AppendLine()
            Next
            'result.AppendLine("队列信息：")
        End If
        Dim qNEsum As Long = 0
        For Each q As QueueInfo In Queue
            If q.QMember.Count = 0 Then
                Continue For
            End If
            If yybOnly AndAlso q.QName <> "预约表" Then Continue For
            If q.QName <> "预约表" AndAlso Not q.QName.StartsWith("树") Then result.AppendLine($"====={q.QName}=====")
            If q.QName = "预约表" Then
                q.QMember.Sort(New Comparison(Of QueueInfo.QItem)(
                               Function(a As QueueInfo.QItem, b As QueueInfo.QItem)
                                   If a.QInfo <> b.QInfo Then Return a.QInfo.CompareTo(b.QInfo)
                                   Return a.CreateTime.CompareTo(b.CreateTime)
                               End Function))
                Dim qIN As String = ""
                For Each qi As QueueInfo.QItem In q.QMember
                    If qIN <> qi.QInfo Then
                        qIN = qi.QInfo
                        result.AppendLine($"====={qIN}=====")
                        qNEsum += 1
                    End If
                    'Dim rest As String = qi.QMember.ChanceSummary
                    'If rest <> "" Then rest = $"余{rest}"
                    Dim rest As String = $"{qi.QMember.FullChance}|{qi.QMember.FragChance}"
                    If qi.Message <> "" Then rest = $"{rest}：{qi.Message}"
                    result.AppendLine($"【{qi.QMember.NickName}】{rest}")
                Next
            Else
                If q.QName = $"薛定谔的树" Then
                    For Each qi As QueueInfo.QItem In q.QMember
                        qNEsum += 1
                        Dim rest As String = qi.QMember.ChanceSummary
                        If rest <> "" Then rest = $"余{rest}"
                        If qi.Message <> "" Then rest = $"{rest}：{qi.Message}"
                        result.AppendLine($" ~ {qi.QMember.NickName}({qi.PassedTime}){rest}")
                    Next
                End If
            End If
        Next
        If qNEsum = 0 Then
            If yybOnly Then result.AppendLine("预约表为空")
        Else
            result.AppendLine("==============")
        End If
        Return result.ToString
    End Function
    Public Sub ListAppoint(msg As String)
        Dim result As New StringBuilder
        Dim param As String = MsgUtils.ReadNum(msg)(0)
        If param = "" Then Exit Sub
        For Each q As QueueInfo In Queue
            If q.QName = "预约表" Then
                Dim qms As New List(Of QueueInfo.QItem)
                For Each qm As QueueInfo.QItem In q.QMember
                    If qm.QInfo = $"预约{param}" Then
                        qms.Add(qm)
                    End If
                Next
                result.AppendLine($"=====预约{param}=====")
                If qms.Count = 0 Then
                    result.AppendLine("没有预约记录")
                Else
                    For Each qi As QueueInfo.QItem In qms
                        Dim rest As String = qi.QMember.ChanceSummary
                        If rest <> "" Then rest = $"余{rest}"
                        If qi.Message <> "" Then rest = $"{rest}：{qi.Message}"
                        result.AppendLine($"【{qi.QMember.NickName}】{rest}")
                    Next
                End If
                result.Append("==============")
                Reply(result.ToString())
                Exit Sub
            End If
        Next
    End Sub
    Public Sub DisableClanBattle(msgObj As CQMessage)
        Dim sender As String = msgObj.user_id
        If Not CheckGroupAdmin(sender) Then Exit Sub
        If Enabled Then
            Enabled = False
            Reply($"已禁用群{Group_ID}的会战功能")
        Else
            Reply($"操作失败：群{Group_ID}的会战功能已处于禁用状态")
        End If
    End Sub
    Public Function CMDHelp() As String
        Return "===== 会战指令 =====
*设置群管：将指定用户设置为群管，后面跟qq号或者at某人
*取消群管：取消自己或者指定用户群管权限
群管列表/群管一览
加入公会：将自己或者指定用户加入公会
退出公会：将自己或者*指定用户退出公会
*改名
查刀/*催刀
成员列表/成员一览
状态/进度
挂树x/进x/下树x/预约x/取消预约x：
  示例
    挂树1
    挂树1：寄了
    挂树1：1000W
    进1
    进1：一刀
    下树1
    预约1：500W
    取消预约1
报x整/补xxx：报刀
  示例
    报1整12345678
    报1补123456
    报1整收/尾
    报1补收/尾
SL/SL?/取消SL/撤销SL
合刀：合刀计算器
*撤销：撤销上一个报刀
*重置状态/切换档案：切换到新的档案
*禁用会战：关闭本群会战功能（不会删除档案）
*BOSS参数：设置BOSS血量上限、周目信息

*：需要群管权限
=========="
        '*状态修改：更改血量、周目等。先发送该命令获得状态信息，然后把修改后的状态信息放在命令后面作为参数再次发送以写入数据

    End Function
End Class