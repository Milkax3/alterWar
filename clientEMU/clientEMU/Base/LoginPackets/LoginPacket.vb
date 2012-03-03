Namespace Base.Packets.LoginPackets
    Public Class LoginPacket
        Inherits PacketBase

        Sub New(ByVal Username As String, ByVal Password As String)
            MyBase.New(4352)
            AddBlock("0000000001")
            AddBlock("0")
            AddBlock(Username)
            AddBlock(Password)
            AddBlock("0")
            AddBlock("0")
        End Sub
    End Class
End Namespace
