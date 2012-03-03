Namespace Base.Packets
    Public Class Crypter
        Public Function UnCrypt(ByVal Str As String, ByVal IsLoginPacket As Boolean) As PacketBase()
            Dim tBytes As Byte() = StringToBytes(Str)
            Dim tBytesUncrypted As New List(Of Byte)
            For i As Integer = 0 To tBytes.Length - 1
                If IsLoginPacket Then
                    tBytesUncrypted.Add(Convert.ToByte(tBytes(i) Xor &H96))
                Else
                    tBytesUncrypted.Add(Convert.ToByte(Convert.ToByte(Convert.ToByte(tBytes(i) Xor &H96) Xor &HCF) Xor &H48))
                End If
            Next
            Dim TotalPackets As New List(Of PacketBase)
            Dim UncryptedString As String = BytesToString(tBytesUncrypted.ToArray())
            For Each subPacket As String In UncryptedString.Split(Chr(254))

                Dim Packet As New List(Of String)
                Dim tmpBlocks As New List(Of String)
                Dim tmpOperationCode As Integer

                For Each B As String In BytesToString(tBytesUncrypted.ToArray()).Split(" ")
                    Packet.Add(B)
                Next

                tmpBlocks = Packet
                tmpBlocks.RemoveAt(0) 'removing timestamp
                tmpOperationCode = Packet(0) 'caching operation code
                tmpBlocks.RemoveAt(0) 'remove the operation code, so we only have the blocks left

                Dim P As New PacketBase(tmpOperationCode)
                For Each B As String In tmpBlocks
                    P.AddBlock(B)
                Next

                TotalPackets.Add(P)
            Next
            Return totalPackets.ToArray()
        End Function
        Public Function Crypt(ByVal Packet As PacketBase, ByVal IsLoginPacket As Boolean) As String
            Dim tBytes As Byte() = StringToBytes(Packet.GetPacket)

            For i As Integer = 0 To tBytes.Length - 1
                tBytes(i) = Convert.ToByte(tBytes(i) Xor &HC3)
                If Not IsLoginPacket Then
                    tBytes(i) = Convert.ToByte(Convert.ToByte(tBytes(i) Xor 230) Xor &H60)
                End If
            Next

            Return BytesToString(tBytes)
        End Function

        Public Function StringToBytes(ByVal Input As String) As Byte()
            Return Text.Encoding.[Default].GetBytes(Input)
        End Function
        Public Function BytesToString(ByVal Input As Byte()) As String
            Return Text.Encoding.[Default].GetString(Input)
        End Function

    End Class
End Namespace