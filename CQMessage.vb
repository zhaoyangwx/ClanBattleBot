
Imports Newtonsoft.Json

Public Class CQMessage
    Public Property post_type As String
    Public Property message_type As String
    Public Property time As Long
    Public Property self_id As Long
    Public Property sub_type As String
    Public Property message As String
    Public Property message_seq As Long
    Public Property raw_message As String
    Public Property sender As senderInfo
    Public Class senderInfo
        Public Property age As Long
        Public Property area As String
        Public Property card As String
        Public Property level As String
        Public Property nickname As String
        Public Property role As String
        Public Property sex As String
        Public Property title As String
        Public Property user_id As Long

    End Class
    Public Property message_id As Long
    Public Property anonymous As Object
    Public Property font As Long
    Public Property group_id As Long
    Public Property user_id As Long
    Public Function GetJSON() As String
        Return JsonConvert.SerializeObject(Me)
    End Function
    Public Shared Function FromJSON(JSONText As String) As CQMessage
        Return JsonConvert.DeserializeObject(Of CQMessage)(JSONText)
    End Function

End Class
Public Class MsgUtils
    Public Shared Function ParseCQatParam(msgraw As String, PrefixLen As Long) As String
        Dim param As String = msgraw.Substring(PrefixLen).TrimStart(" ")
        If param.StartsWith("[CQ:at,qq=") Then
            param = param.Substring(10)
            For i As Long = 0 To param.Length - 1
                If param(i) = "]" Then
                    param = param.Substring(0, i)
                    Exit For
                End If
            Next
        End If
        For i As Long = 0 To param.Length - 1
            If Not IsNumeric(param(i)) Then
                param = param.Substring(0, i)
                Exit For
            End If
        Next
        Return param
    End Function
    Public Shared Function CQAt(qqid As String) As String
        Return $"[CQ:at,qq={qqid}]"
    End Function
    Public Shared Function IsNumeric(c As Char) As Boolean
        Return Asc(c) >= Asc("0") AndAlso Asc(c) <= Asc("9")
    End Function
    Public Shared Function ReadNum(s As String) As String()
        Dim i As Long = 0
        For i = 0 To s.Length - 1
            If Not IsNumeric(s(i)) Then Exit For
        Next
        Dim s0 As String = "", s1 As String = ""
        If i > 0 Then s0 = s.Substring(0, i)
        If i < s.Length Then s1 = s.Substring(i)
        Return {s0, s1}
    End Function
    Public Shared Function GetBehalf(s As String) As String()
        If s = "" Then Return {"", s}
        If Not s.Contains("[CQ:at,qq=") Then Return {"", s}
        Try
            Dim i As Long = s.IndexOf("[CQ:at,qq=") + 10
            Dim rnresult As String() = ReadNum(s.Substring(i))
            Return {rnresult(0), s.Substring(0, i - 10) & rnresult(1).Substring(1)}
        Catch ex As Exception
            Return {"", s}
        End Try

    End Function
End Class
