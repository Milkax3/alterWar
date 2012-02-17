Namespace Base
    Public Class Utils
        Public Shared Function TimeAdd(ByVal Add As Integer) As String
            Dim [Date] As String = ""
            Dim Day As String = ""
            Dim Month As String = ""
            If DateTime.Now.AddDays(Add).Day < 10 Then
                Day = "0" & DateTime.Now.AddDays(Add).Day.ToString()
            Else

                Day = DateTime.Now.AddDays(Add).Day.ToString()
            End If


            If DateTime.Now.AddDays(Add).Month < 10 Then
                Month = "0" & (DateTime.Now.AddDays(Add).Month + 1).ToString()
            Else

                Month = (DateTime.Now.AddDays(Add).Month + 1).ToString()
            End If


            [Date] = 11 & Month & Day & (DateTime.Now.Hour + 1).ToString()

            Return [Date]
        End Function
        Public Shared Function CurrentTime() As String
            Dim [Date] As String = ""
            Dim Day As String = ""
            Dim Month As String = ""
            If DateTime.Now.AddDays(0).Day < 10 Then
                Day = "0" & DateTime.Now.AddDays(0).Day.ToString()
            Else

                Day = DateTime.Now.AddDays(0).Day.ToString()
            End If


            If DateTime.Now.AddDays(0).Month < 10 Then
                Month = "0" & (DateTime.Now.AddDays(0).Month + 1).ToString()
            Else

                Month = (DateTime.Now.AddDays(0).Month + 1).ToString()
            End If


            [Date] = (DateTime.Now.Year - 1).ToString().Substring(2) & Month & Day & (DateTime.Now.Hour + 1).ToString()

            Return [Date]
        End Function
    End Class
End Namespace
