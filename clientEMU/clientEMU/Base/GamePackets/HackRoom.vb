Namespace Base.Packets.GamePackets
    Public Class HackRoom
        Inherits PacketBase

        Sub New()
            MyBase.New(29440)
            AddBlock("PW_lmfao")
            AddBlock(1)
            AddBlock("lmfao")
            AddBlock(5)
            AddBlock(46)
            AddBlock(4)
            AddBlock(0)
            AddBlock(0)
            AddBlock(0)
            AddBlock(0)
            AddBlock(1)
            WriteLine("asdfmovie", MyBase.GetPacket())
        End Sub
    End Class
End Namespace

