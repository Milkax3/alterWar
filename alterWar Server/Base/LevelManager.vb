Namespace alterWar.Base
    Public Class LevelManager
        Private _LevelList As List(Of Int32)
        Public Function GetEXP(ByVal Level As Byte) As Int32
            Return _LevelList(Level)
        End Function
        Public Function GetLevel(ByVal Exp As Int32) As Byte
            Dim CurrentLevel As Integer = 0
            For Each e As Int32 In _LevelList
                If Exp >= e Then
                    CurrentLevel += 1
                End If
            Next
            Return CurrentLevel - 1
        End Function
        Sub New(ByVal _LevelFile As String)
            Dim Lines As String() = IO.File.ReadAllLines(_LevelFile)
            _LevelList = New List(Of Int32)
            For Each L As String In Lines
                _LevelList.Add(L)
            Next
        End Sub
    End Class
End Namespace