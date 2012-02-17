Namespace Base.Packets.LoginPackets
    Public Class LoginPacket
        Inherits PacketBase

        Sub New(ByVal Username As String, ByVal Password As String)
            MyBase.New(4352)
            AddBlock(New Random().Next(111111111, 999999999))
            AddBlock(0)
            AddBlock(Username)
            AddBlock(Password)
            AddBlock("xl3ckmyx")
            AddBlock(0)
            AddBlock(0)
        End Sub
    End Class
End Namespace
