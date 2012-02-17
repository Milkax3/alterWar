Namespace alterWar.PacketSystem
    Public Class Crypter
        Public Function UnCrypt(ByVal Str As String, ByVal IsLoginPacket As Boolean) As PacketBase
            Dim tBytes As Byte() = StringToBytes(Str)
            Dim tBytesUncrypted As New List(Of Byte)
            For i As Integer = 0 To tBytes.Length - 1
                If IsLoginPacket Then
                    tBytesUncrypted.Add(Convert.ToByte(tBytes(i) Xor &HC3))
                Else
                    tBytesUncrypted.Add(Convert.ToByte(Convert.ToByte(Convert.ToByte(tBytes(i) Xor &HC3) Xor 230) Xor &H60))
                End If
            Next

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

            Return P
        End Function
        Public Function Crypt(ByVal Packet As PacketBase, ByVal IsLoginPacket As Boolean) As String
            Dim tBytes As Byte() = StringToBytes(Packet.GetPacket)

            For i As Integer = 0 To tBytes.Length - 1
                tBytes(i) = Convert.ToByte(tBytes(i) Xor &H96)
            Next

            If IsLoginPacket Then
                Return BytesToString(tBytes)
            Else
                Return Crypt2(BytesToString(tBytes))
            End If
        End Function
        Private Function Crypt2(ByVal Content As String) As String
            Dim tBytes As Byte() = StringToBytes(Content)

            For i As Integer = 0 To tBytes.Length - 1
                tBytes(i) = Convert.ToByte(Convert.ToByte(tBytes(i) Xor &HCF) Xor &H48)
            Next

            Return System.Text.Encoding.[Default].GetString(tBytes)
        End Function

        Public Function StringToBytes(ByVal Input As String) As Byte()
            Return Text.Encoding.[Default].GetBytes(Input)
        End Function
        Public Function BytesToString(ByVal Input As Byte()) As String
            Return Text.Encoding.[Default].GetString(Input)
        End Function
    End Class
End Namespace