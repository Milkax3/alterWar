Imports System.Net.Sockets
Imports System.Net
Imports System.IO

Module Module1

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

    Private ListenerSocket As Socket
    Private Buffer(65535) As Byte
    Private Crypter As Decrypter
    Private LoggerStream As StreamWriter
    Private LoggerStreamUDP As StreamWriter
    Private SettingsD As Dictionary(Of Integer, Settings)

    Private Const IOC_VENDOR As Integer = &H18000000
    Private Const IOC_IN As Integer = -2147483648
    Private Const SIO_RCVALL As Integer = IOC_IN Or IOC_VENDOR Or 1

    Private Function ByteArrayToString(ByVal i As Byte()) As String
        Return New Decrypter().UnCryptS(i, True)
    End Function

    Sub Main()
        Log("Startup", "Initializing...")
        LoggerStream = IO.File.CreateText(Path.Combine(Environment.CurrentDirectory, "log" & "." & Date.Now.Millisecond & "." & Date.Now.Hour & "." & Date.Now.Minute & "." & Date.Now.Second & "." & Date.Now.Month & "." & Date.Now.Day & ".log"))
        LoggerStreamUDP = IO.File.CreateText(Path.Combine(Environment.CurrentDirectory, "UDP.log" & "." & Date.Now.Millisecond & "." & Date.Now.Hour & "." & Date.Now.Minute & "." & Date.Now.Second & "." & Date.Now.Month & "." & Date.Now.Day & ".log"))
        SettingsD = New Dictionary(Of Integer, Settings)
        PutLog("Initialize", "Initializing the System...")
        'Skipping the Config ( im just working on it )
        'ReadConfig()
        Crypter = New Decrypter()
        ListenerSocket = New Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP)
        Log("Startup", "Binding to Local Endpoint")
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
                Log("Select", "You took " & input & ")")
            Else
                Log("Select", "You took " & input & ")")
            End If
        Else
            GoTo tryAgain
        End If
        PutLog("Initialize", "Listening on " & Addresses(input).ToString())
        ListenerSocket.Bind(New IPEndPoint(Addresses(input), 0))
        Log("Startup", "Attaching to SIO_RCVALL")
        ListenerSocket.IOControl(SIO_RCVALL, BitConverter.GetBytes(1), Nothing)
        Log("Startup", "Initialized... Now listening")
        ListenerSocket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, New AsyncCallback(AddressOf OnReceive), Nothing)
        KeepAlive()
    End Sub
    Sub ReadConfig()
        Dim FileLines() As String = IO.File.ReadAllLines(IO.Path.Combine(Environment.CurrentDirectory, "settings.conf"))
        For Each ln As String In FileLines

            If Not ln.StartsWith("#") Then
                Dim FirstTag As String = ln.Split(" ")(0)
                If FirstTag = "i:c->s" Then
                    Dim Setting As New Settings
                    Setting.Type = Settings.eType.SourceToDestination
                    Dim SecondTag As String = ln.Split(" ")(1)
                    Try
                        IPAddress.Parse(SecondTag)
                    Catch ex As Exception
                        PutLog("Error", "Couldnt parse ip address (client->server) in Config File: " & SecondTag)
                        Log("Error", "Error reading config (see log)")
                        Console.ReadLine()
                    End Try
                    If ln.Split(" ").Length > 2 Then
                        Dim CryptWR As Boolean = Boolean.Parse(ln.Split(" ")(2))
                        Setting.CryptWR = CryptWR
                    End If
                    If ln.Split(" ").Length > 3 Then
                        Dim OutTag As String = ln.Split(" ")(3).Replace("\s", " ")
                    End If

                End If
            End If
        Next
    End Sub
    Private Function ReadLine() As String
        Console.Write("             ")
        Return Console.ReadLine
    End Function

    Private Sub KeepAlive()
        Log("Info", "Keepin the console alive")
        While True
        End While
    End Sub
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
    Private Sub HandlePacket(ByVal p As Packet)
        If p.DestinationPort = 5330 AndAlso p.Protocol = Protocol.Tcp Then 'Client Packet (Login)
            Dim Uncrypted As String = Crypter.UnCrypt(p.GetDataBytes(), True)
            If Uncrypted = "" OrElse Uncrypted.Length = 4 Then Exit Sub
            Log("Login", "Client->Server")
            PutLog("Login Client->Server", Uncrypted)
        ElseIf p.DestinationPort = 5340 AndAlso p.Protocol = Protocol.Tcp Then 'Client Packet (Game)
            Dim Uncrypted As String = Crypter.UnCrypt(p.GetDataBytes(), False)
            If Uncrypted = "" OrElse Uncrypted.Length = 4 Then Exit Sub
            Log("Game", "Client->Server")
            PutLog("Game Client->Server", Uncrypted)
        ElseIf p.SourcePort = 5330 AndAlso p.Protocol = Protocol.Tcp Then 'Server Packet (Login)
            Dim Uncrypted As String = Crypter.UnCryptS(p.GetDataBytes(), True)
            If Uncrypted = "" OrElse Uncrypted.Length = 4 Then Exit Sub
            Log("Login", "Server->Client")
            PutLog("Login Server->Client", Uncrypted)
        ElseIf p.SourcePort = 5340 AndAlso p.Protocol = Protocol.Tcp Then  'Server Packet (Game)
            Dim Uncrypted As String = Crypter.UnCryptS(p.GetDataBytes(), False)
            If Uncrypted = "" OrElse Uncrypted.Length = 4 Then Exit Sub
            Log("Game", "Server->Client")
            PutLog("Game Server->Client", Uncrypted)

        ElseIf p.DestinationPort = 5350 AndAlso p.Protocol = Protocol.Udp Then ' Client Packet ( UDP )
            Log("UDP", "Client->Server")
            PutLogUDP("UDP Client->Server", p.Source & "->" & p.DestinationAddress & vbNewLine & p.GetDataBytesUDP().ToHexString())
        ElseIf p.SourcePort = 5350 AndAlso p.Protocol = Protocol.Udp Then ' Client Packet ( UDP )
            Log("UDP", "Server->Client")
            PutLogUDP("UDP Server->Client", p.Source & "->" & p.DestinationAddress & vbNewLine & p.GetDataBytesUDP().ToHexString())
        End If
    End Sub
    Public Function Resolve(ByVal addr As String) As String
        Return Dns.Resolve(addr).AddressList(0).ToString()
    End Function

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
    Public Sub PutLog(ByVal Pre As String, ByVal Data As String)
        LoggerStream.Write("[" & Pre & "]" & vbNewLine & vbNewLine & Data & vbNewLine & vbNewLine & vbNewLine)
        LoggerStream.Flush()
    End Sub
    Public Sub PutLogUDP(ByVal Pre As String, ByVal Data As String)
        LoggerStreamUDP.Write("[" & Pre & "]" & vbNewLine & vbNewLine & Data & vbNewLine & vbNewLine & vbNewLine)
        LoggerStreamUDP.Flush()
    End Sub


End Module
