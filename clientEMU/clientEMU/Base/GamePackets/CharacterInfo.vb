Namespace Base.Packets.GamePackets
    Public Class CharacterInfo
        Inherits PacketBase

        Sub New(ByVal _Data As Classes.cUserData)
            MyBase.New(25088)
            AddBlock(_Data.UserID)
            AddBlock(-1)
            AddBlock(_Data.Username)
            AddBlock(_Data.Nickname)
            AddBlock(0)
            AddBlock(28)
            AddBlock(0)
            AddBlock(1)
            AddBlock(-1)
            AddBlock(-1)
            AddBlock(-1)
            AddBlock(-1)
            AddBlock(-1)
            AddBlock(0)
            AddBlock("1.36871")
            AddBlock("dnjfhr^")
        End Sub
    End Class
End Namespace
