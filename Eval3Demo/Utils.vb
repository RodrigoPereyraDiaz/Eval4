Public Class Utils
    Shared Function MsgBox(ByVal Prompt As Object, Optional ByVal Buttons As Microsoft.VisualBasic.MsgBoxStyle = 0, Optional ByVal Title As Object = Nothing) As Microsoft.VisualBasic.MsgBoxResult
        Return Microsoft.VisualBasic.MsgBox(Prompt, Buttons, Title)
    End Function


    Public Shared Sub ErrMsg(ByVal parent As Windows.Forms.Control, ByVal msg As String, ByVal ex As Exception)
        Microsoft.VisualBasic.MsgBox(msg, MsgBoxStyle.OkOnly, "Error")
    End Sub

End Class
