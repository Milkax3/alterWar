Imports MySql.Data.MySqlClient

Public Class LoginHandler
    Public Enum LoginErrorCodes As Integer
        Success = 1

        Unregistered = 72010
        InvalidPassword = 72020
        AlreadyLoggedIn = 72030

        EnterName = 74010
        EnterPassword = 74020
    End Enum

    Public Sub HandlePacket(ByVal D As Byte(), ByVal Client As PlayerClientLogin)
        Try
            Dim Packet As PacketBase = Nothing
            Packet = GetCrypter().UnCrypt(GetCrypter().BytesToString(D), True)
            HandlePacketSub(Packet, Client)
        Catch ex As Exception
            Log(LogStyle.GameServer, "(LoginServer) Client packet went wrong -> Disconnecting it")
            Client.Connection.Disconnect(True)
        End Try
    End Sub

    Private Sub HandlePacketSub(ByVal P As PacketBase, ByVal Client As PlayerClientLogin)
        Select Case P.OperationCode
            Case LOperationCodes.ClPatch
                HandlePacket_Patch(P, Client)
                Exit Select
            Case LOperationCodes.ClLogin
                HandlePacket_Login(P, Client)
                Exit Select
            Case Else
                Log(LogStyle.Network, "~ NEW [LOGIN] CLIENT PACKET ~")
                Log(LogStyle.Info, "Operation Code: " & P.OperationCode & " - Block Count & " & P.Blocks.Count)
                Log(LogStyle.Info, "Packet Content: ")
                Log(LogStyle.Info, P.OperationCode & " " & String.Join(" ", P.Blocks.ToArray))
        End Select
    End Sub

    Private Sub HandlePacket_Patch(ByVal P As PacketBase, ByVal Client As PlayerClientLogin)
        Client.Send(New lSvPatch())
    End Sub

    Private Sub HandlePacket_Login(ByVal P As PacketBase, ByVal Client As PlayerClientLogin)
        Dim UsernameT As String = P.GetBlockIndex(2)
        Dim PasswordT As String = P.GetBlockIndex(3)

        Dim Username As String = String.Empty
        Dim Password As String = String.Empty
        Dim Nickname As String = String.Empty

        Dim Online As Integer = 0
        Dim ID As Integer = 0
        Dim AccessLevel As Integer = 0

        Dim Registered As Boolean = False

        Dim ErrorMessage As Integer = 1
        Dim Query As MySqlDataReader = Nothing
        Query = GetMySql().ExecuteReader("SELECT * FROM `account` WHERE `name`='" & UsernameT & "'")
        While Query.Read()
            Username = Query.GetString("name")
            Password = Query.GetString("passwd")
            Nickname = Query.GetString("nickname")

            ID = Query.GetInt32("id")
            Online = Query.GetInt32("online")
            AccessLevel = Query.GetInt32("accesslevel")
            Registered = True
        End While
        Query.Close()

        Client.UserID = ID
        Client.Username = Username
        Client.Password = Password
        Client.Nickname = Nickname

        Log(LogStyle.LoginServer, "Login [" & ID & "] " & Username & ":" & Password & " ALevel #" & AccessLevel)

        If Password <> PasswordT Then
            ErrorMessage = LoginErrorCodes.InvalidPassword
        End If
        If Online = 1 Then
            ErrorMessage = LoginErrorCodes.AlreadyLoggedIn
        End If
        If Registered = False Then
            ErrorMessage = LoginErrorCodes.Unregistered
        End If

        If ErrorMessage = LoginErrorCodes.Success Then
            Client.Send(New lSvLogin(Client))
        Else
            Client.Send(New lSvLogin(ErrorMessage))
        End If

    End Sub


End Class