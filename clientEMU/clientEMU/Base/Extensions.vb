Namespace Base
    Public Module Extensions
        <Runtime.CompilerServices.Extension()> _
        Public Function FindID(ByVal L As List(Of Classes.cServer), ByVal ID As Integer) As Classes.cServer
            Dim Found As Classes.cServer = New Classes.cServer(-1, "unknown", "0.0.0.0", "0")
            For Each E As Classes.cServer In L
                If E.ServerID = ID Then
                    Found = E
                    Exit For
                End If
            Next
            Return Found
        End Function

        <Runtime.CompilerServices.Extension()> _
        Public Function ArrayToString(ByVal Bytes As Byte()) As String
            Dim final As String = String.Empty
            For Each c As Byte In Bytes
                final &= c.ToString("X2") & " "
            Next
            final = final.Substring(0, final.Length - 1)
            Return final
        End Function
    End Module
End Namespace
