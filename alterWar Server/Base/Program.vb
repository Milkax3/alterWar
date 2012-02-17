Namespace alterWar.Base
    Public Class Program

        Private _LoginServer As alterWar.LoginServer.Login
        Public ReadOnly Property LoginServer() As alterWar.LoginServer.Login
            Get
                Return _LoginServer
            End Get
        End Property

        Private _GameServer As alterWar.GameServer.Game
        Public ReadOnly Property GameServer() As alterWar.GameServer.Game
            Get
                Return _GameServer
            End Get
        End Property

        Private _MySql As MySqlClass
        Public ReadOnly Property MySql() As MySqlClass
            Get
                Return _MySql
            End Get
        End Property

        Private _Crypter As alterWar.PacketSystem.Crypter
        Public ReadOnly Property Crypter() As alterWar.PacketSystem.Crypter
            Get
                Return _Crypter
            End Get
        End Property

        Sub Run()
            Try
                Globals.Program = Me
                Globals.LevelManager = New LevelManager("levels.dat")

                Config = New ConfigClass("Server")
                Config.ReadIni()

                _MySql = New alterWar.Base.MySqlClass(Config.GetConfig("SQL_IP"), Config.GetConfig("SQL_USERNAME"), Config.GetConfig("SQL_PORT"), Config.GetConfig("SQL_PASSWORD"), Config.GetConfig("SQL_DATABASE"))
                _Crypter = New alterWar.PacketSystem.Crypter()
                _LoginServer = New alterWar.LoginServer.Login()
                _GameServer = New alterWar.GameServer.Game()
            Catch ex As Exception
                Log(LogStyle.Error, "Root error")
                Log(LogStyle.Info, ex.Message)
                Console.Read()
            End Try
        End Sub
    End Class

End Namespace
