Namespace Base.Packets.GamePackets
    Public Class RoomList
        Inherits PacketBase

        Sub New(ByVal _Page As Integer)
            MyBase.New(29184)
            AddBlock(_Page)
            AddBlock(0)
            AddBlock(0)
        End Sub
    End Class
End Namespace

