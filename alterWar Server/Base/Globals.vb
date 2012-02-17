Imports System.Net

Public Module Globals

    Public Enum LogStyle As Integer
        Debug = 0
        Info = 1
        Network = 2
        ISC = 3
        Sucess = 4
        [Error] = 5

        LoginServer = 6
        GameServer = 7
    End Enum

    <Runtime.InteropServices.DllImport("winmm.dll")> _
    Private Function timeGetTime() As Long
    End Function
    Public Function GetTime() As Long
        Return timeGetTime()
    End Function

    Private ServerList As Dictionary(Of Integer, ServerInfo)
    Public Config As alterWar.Base.ConfigClass
    Public Program As alterWar.Base.Program
    Public LevelManager As alterWar.Base.LevelManager

    Public Function GetGameServer() As alterWar.GameServer.Game
        Return Program.GameServer()
    End Function
    Public Function GetLoginServer()
        Return Program.LoginServer()
    End Function
    Public Function GetMySql() As alterWar.Base.MySqlClass
        Return Globals.Program.MySql()
    End Function
    Public Function GetCrypter() As alterWar.PacketSystem.Crypter
        Return Program.Crypter()
    End Function

    Public Sub Log(ByVal Style As LogStyle, ByVal Message As String, Optional ByVal DebugLevel As Integer = 0)
        If Config Is Nothing = False AndAlso DebugLevel >= Config.GetConfig("SERVER_DEBUGLEVEL") Then Exit Sub
        Select Case Style
            Case LogStyle.Debug
                Console.ForegroundColor = ConsoleColor.Gray
                Console.Write("[DEBUG]")
                Console.ForegroundColor = ConsoleColor.White
                Console.Write(Message)
            Case LogStyle.Error
                Console.ForegroundColor = ConsoleColor.Red
                Console.Write("[Error]")
                Console.ForegroundColor = ConsoleColor.Red
                Console.Write(Message)
            Case LogStyle.Sucess
                Console.ForegroundColor = ConsoleColor.Green
                Console.Write("[SUCCESS]")
                Console.ForegroundColor = ConsoleColor.White
                Console.Write(Message)
            Case LogStyle.Info
                Console.ForegroundColor = ConsoleColor.Yellow
                Console.Write("[INFO]")
                Console.ForegroundColor = ConsoleColor.White
                Console.Write(Message)
            Case LogStyle.ISC
                Console.ForegroundColor = ConsoleColor.Cyan
                Console.Write("[ISC]")
                Console.ForegroundColor = ConsoleColor.White
                Console.Write(Message)
            Case LogStyle.Network
                Console.ForegroundColor = ConsoleColor.Blue
                Console.Write("[NETWORK]")
                Console.ForegroundColor = ConsoleColor.White
                Console.Write(Message)
            Case LogStyle.LoginServer
                Console.ForegroundColor = ConsoleColor.Yellow
                Console.Write("[")
                Console.ForegroundColor = ConsoleColor.Cyan
                Console.Write("LOGIN")
                Console.ForegroundColor = ConsoleColor.Yellow
                Console.Write("]")
                Console.Write(Message)
            Case LogStyle.GameServer
                Console.ForegroundColor = ConsoleColor.Yellow
                Console.Write("[")
                Console.ForegroundColor = ConsoleColor.Cyan
                Console.Write("GAME")
                Console.ForegroundColor = ConsoleColor.Yellow
                Console.Write("]")
                Console.Write(Message)
        End Select
        Console.Write(vbNewLine)
        Console.ForegroundColor = ConsoleColor.White

        Threading.Thread.Sleep(10)
    End Sub
End Module