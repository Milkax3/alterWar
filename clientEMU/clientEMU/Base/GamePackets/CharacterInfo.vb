Namespace Base.Packets.GamePackets
    Public Class CharacterInfo
        Inherits PacketBase

        Sub New(ByVal _Data As Classes.cUserData, ByVal _PassCode As String)
            MyBase.New(25088)
            AddBlock(_Data.UserID)
            AddBlock(-1)
            AddBlock(_Data.Username)
            AddBlock(_Data.Nickname)
            AddBlock(1)
            AddBlock(26)
            AddBlock(910)
            AddBlock(1)
            AddBlock(-1)
            AddBlock(-1)
            AddBlock(-1)
            AddBlock(-1)
            AddBlock(-1)
            AddBlock(0)
            AddBlock(_PassCode)
            AddBlock("dnjfhr^")
        End Sub
    End Class
End Namespace
