Imports System.Net.Sockets
Imports System.Net
Imports System.IO

Module Base

    <Runtime.CompilerServices.Extension()> _
    Public Function ToHexString(ByVal i As Byte()) As String
        Dim final As String = String.Empty
        For Each c As Byte In i
            final &= c.ToString("X2") & " "
        Next
        Return final
    End Function

    Private Structure Settings
        Public Message As String
        Public CryptWR As Boolean
        Public Type As eType
        Public Enum eType
            SourceToDestination = 0
            DestinationToSource = 1
        End Enum
    End Structure

#Region "Objects"

    ''' <summary>
    ''' Socket to listen on
    ''' </summary>
    ''' <remarks></remarks>
    Private ListenerSocket As Socket

    ''' <summary>
    ''' Buffer for the packet
    ''' </summary>
    ''' <remarks></remarks>
    Private Buffer(65535) As Byte

    ''' <summary>
    ''' Decrypter for the packets
    ''' </summary>
    ''' <remarks></remarks>
    Private Crypter As Decrypter

    ''' <summary>
    ''' Logger stream for logging
    ''' </summary>
    ''' <remarks></remarks>
    Private LoggerStream As StreamWriter
    Private LoggerStreamUDP As StreamWriter
    Private LoggerStreamUDPOther As StreamWriter

    ''' <summary>
    ''' Consts
    ''' </summary>
    ''' <remarks></remarks>
    Private Const IOC_VENDOR As Integer = &H18000000
    Private Const IOC_IN As Integer = -2147483648
    Private Const SIO_RCVALL As Integer = IOC_IN Or IOC_VENDOR Or 1

    ''' <summary>
    ''' The IP Address to listen on
    ''' </summary>
    ''' <remarks></remarks>
    Private SelectedIPAddress As String

#End Region

    Private Function ByteArrayToString(ByVal i As Byte()) As String
        Return New Decrypter().UnCryptS(i, True)
    End Function

    Sub Main()
        Log("Initialize", "Supreme Logger starting...", ConsoleColor.Green)
        LoggerStream = IO.File.CreateText(Path.Combine(Environment.CurrentDirectory, "log" & "." & Date.Now.Millisecond & "." & Date.Now.Hour & "." & Date.Now.Minute & "." & Date.Now.Second & "." & Date.Now.Month & "." & Date.Now.Day & ".log"))
        LoggerStreamUDP = IO.File.CreateText(Path.Combine(Environment.CurrentDirectory, "UDP.log" & "." & Date.Now.Millisecond & "." & Date.Now.Hour & "." & Date.Now.Minute & "." & Date.Now.Second & "." & Date.Now.Month & "." & Date.Now.Day & ".log"))
        LoggerStreamUDPOther = IO.File.CreateText(Path.Combine(Environment.CurrentDirectory, "UDP.Other.log" & "." & Date.Now.Millisecond & "." & Date.Now.Hour & "." & Date.Now.Minute & "." & Date.Now.Second & "." & Date.Now.Month & "." & Date.Now.Day & ".log"))
        PutLog("Initialize", "Initializing the System...")
        Crypter = New Decrypter()
        ListenerSocket = New Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP)


        'Select a device to listen on
        Log("Select", "Select a Device to listen on", ConsoleColor.Cyan)
        Dim Addresses() As IPAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList
        Dim i As Integer = 0
        For Each Entry As IPAddress In Addresses
            Log("Select", i & ") " & Entry.ToString())
            i += 1
        Next Entry
tryAgain:
        Dim input As String = CInt(ReadLine())
        If IsNumeric(input) Then
            If input <= Addresses.Length Then
                Log("Select", "You took " & input & " [ " & Addresses(input).ToString() & " ]", ConsoleColor.Cyan)
                SelectedIPAddress = Addresses(input).ToString()
            Else
                GoTo tryAgain
            End If
        Else
            GoTo tryAgain
        End If



        PutLog("Initialize", "Listening on " & Addresses(input).ToString())
        ListenerSocket.Bind(New IPEndPoint(Addresses(input), 0))

        ListenerSocket.IOControl(SIO_RCVALL, BitConverter.GetBytes(1), Nothing)
        Log("Initialize", "Now listening...")

        ListenerSocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, New AsyncCallback(AddressOf OnReceive), Nothing)
        KeepAlive()
    End Sub

    ''' <summary>
    ''' Reads a formatted line
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ReadLine() As String
        Console.Write("             ")
        Return Console.ReadLine
    End Function

    ''' <summary>
    ''' Keeps the Console alive for the case that the Connections were closed
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub KeepAlive()
        While True
        End While
    End Sub

    ''' <summary>
    ''' Catching every packet and check it
    ''' </summary>
    ''' <param name="ar">...</param>
    ''' <remarks></remarks>
    Private Sub OnReceive(ByVal ar As IAsyncResult)
        Try
            Dim received As Integer = ListenerSocket.EndReceive(ar)
            Try
                Dim packet(received - 1) As Byte
                Array.Copy(Buffer, 0, packet, 0, received)
                Dim TruePacket As New Packet(packet)
                HandlePacket(TruePacket)
            Catch ex2 As Exception
            End Try
            ListenerSocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, New AsyncCallback(AddressOf OnReceive), Nothing)
        Catch ex1 As Exception
        End Try
    End Sub

    ''' <summary>
    ''' Handling an sniffed packet (eg. log it)
    ''' </summary>
    ''' <param name="p">The packet</param>
    ''' <remarks></remarks>
    Private Sub HandlePacket(ByVal p As Packet)
        If p.DestinationPort = 5330 AndAlso p.Protocol = Protocol.Tcp Then 'Client Packet (Login)
            Dim Uncrypted As String = Crypter.UnCrypt(p.GetDataBytes(), True)
            If Uncrypted = "" OrElse Uncrypted.Length = 4 Then Exit Sub
            Log("Login", "Client->Server", ConsoleColor.Gray)
            PutLog("Login Client->Server", p.Source & "->" & p.DestinationAddress & vbNewLine & Uncrypted)


        ElseIf p.DestinationPort = 5340 AndAlso p.Protocol = Protocol.Tcp Then 'Client Packet (Game)
            Dim Uncrypted As String = Crypter.UnCrypt(p.GetDataBytes(), False)
            If Uncrypted = "" OrElse Uncrypted.Length = 4 Then Exit Sub
            Log("Game", "Client->Server", ConsoleColor.Gray)
            PutLog("Game Client->Server", p.Source & "->" & p.DestinationAddress & vbNewLine & Uncrypted)


        ElseIf p.SourcePort = 5330 AndAlso p.Protocol = Protocol.Tcp Then 'Server Packet (Login)
            Dim Uncrypted As String = Crypter.UnCryptS(p.GetDataBytes(), True)
            If Uncrypted = "" OrElse Uncrypted.Length = 4 Then Exit Sub
            Log("Login", "Server->Client", ConsoleColor.Gray)
            PutLog("Login Server->Client", p.Source & "->" & p.DestinationAddress & vbNewLine & Uncrypted)


        ElseIf p.SourcePort = 5340 AndAlso p.Protocol = Protocol.Tcp Then  'Server Packet (Game)
            Dim Uncrypted As String = Crypter.UnCryptS(p.GetDataBytes(), False)
            If Uncrypted = "" OrElse Uncrypted.Length = 4 Then Exit Sub
            Log("Game", "Server->Client", ConsoleColor.Gray)
            PutLog("Game Server->Client", p.Source & "->" & p.DestinationAddress & vbNewLine & Uncrypted)


        ElseIf p.DestinationPort = 5350 AndAlso p.Protocol = Protocol.Udp Then ' Client Packet ( UDP )
            Log("UDP", "Client->Server", ConsoleColor.Gray)
            PutLogUDP("UDP Client->Server", p.Source & "->" & p.DestinationAddress & vbNewLine & p.GetDataBytesUDP().ToHexString())


        ElseIf p.SourcePort = 5350 AndAlso p.Protocol = Protocol.Udp Then ' Server Packet ( UDP )
            Log("UDP", "Server->Client", ConsoleColor.Gray)
            PutLogUDP("UDP Server->Client", p.Source & "->" & p.DestinationAddress & vbNewLine & p.GetDataBytesUDP().ToHexString())


        ElseIf p.Protocol = Protocol.Udp AndAlso (p.SourceAddress = SelectedIPAddress OrElse p.DestinationAddress = SelectedIPAddress) Then ' Other Packet ( UDP )
            PutLogUDPOther(p.Source.ToString() & " -> " & p.Destination.ToString(), p.GetDataBytesUDP().ToHexString())


        End If
    End Sub

    ''' <summary>
    ''' Resolves the DNS to an IP Address
    ''' </summary>
    ''' <param name="addr">Hostname (DNS)</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Resolve(ByVal addr As String) As String
        Return Dns.GetHostEntry(addr).AddressList(0).ToString()
    End Function

    ''' <summary>
    ''' Loggs messages in the console
    ''' </summary>
    ''' <param name="Pre">Pre message (tagged in [])</param>
    ''' <param name="Message">Message</param>
    ''' <param name="C">Color of Log</param>
    ''' <remarks></remarks>
    Public Sub Log(ByVal Pre As String, ByVal Message As String, Optional ByVal C As ConsoleColor = ConsoleColor.White)
        Console.ForegroundColor = ConsoleColor.Yellow
        If Pre.Length < 9 Then
            For i As Integer = Pre.Length To 9
                Pre &= " "
            Next
        End If
        Console.Write("[" & Pre & "] ")
        Console.ForegroundColor = C
        Console.Write(Message)
        Console.Write(vbNewLine)
    End Sub

    ''' <summary>
    ''' Writes log into default log file
    ''' </summary>
    ''' <param name="Pre">Pre message (tagged in [])</param>
    ''' <param name="Data">Message</param>
    ''' <remarks></remarks>
    ''' 
    Public Sub PutLog(ByVal Pre As String, ByVal Data As String)
        LoggerStream.Write("[" & Pre & "]" & vbNewLine & Data & vbNewLine & vbNewLine & vbNewLine & "=========================================" & vbNewLine & vbNewLine)
        LoggerStream.Flush()
    End Sub

    ''' <summary>
    ''' Writes log into udp log file
    ''' </summary>
    ''' <param name="Pre">Pre message (tagged in [])</param>
    ''' <param name="Data">Message</param>
    ''' <remarks></remarks>
    Public Sub PutLogUDP(ByVal Pre As String, ByVal Data As String)
        LoggerStreamUDP.Write("[" & Pre & "]" & vbNewLine & Data & vbNewLine & vbNewLine & vbNewLine & "=========================================" & vbNewLine & vbNewLine)
        LoggerStreamUDP.Flush()
    End Sub

    ''' <summary>
    ''' Writes log into other udp log file
    ''' </summary>
    ''' <param name="Pre">Pre message (tagged in [])</param>
    ''' <param name="Data">Message</param>
    ''' <remarks></remarks>
    Public Sub PutLogUDPOther(ByVal Pre As String, ByVal Data As String)
        LoggerStreamUDPOther.Write("[" & Pre & "]" & vbNewLine & Data & vbNewLine & vbNewLine & vbNewLine & "=========================================" & vbNewLine & vbNewLine)
        LoggerStreamUDPOther.Flush()
    End Sub


End Module
