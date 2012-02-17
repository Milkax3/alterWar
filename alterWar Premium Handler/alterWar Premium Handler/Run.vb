Module Module1

    Sub Main()
        Dim DebugMode As Boolean = True
        Console.ForegroundColor = ConsoleColor.White
        If DebugMode = True Then GoTo EnterDebugMode
        Dim Arg As String() = Environment.GetCommandLineArgs()
        If Arg.Length < 4 Then
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("Argument Exception")
            Console.ForegroundColor = ConsoleColor.White
            Console.WriteLine("Run with "" mysqlhost mysqlusername mysqlpassword mysqldatabase"" in the Arguments!")
            Console.Read()
            Exit Sub
        End If
        Dim SqlHost As String = String.Empty
        SqlHost = Arg(1).Substring(1, Arg(1).Length - 1)
        Dim SqlName As String = String.Empty
        SqlName = Arg(2).Substring(1, Arg(2).Length - 1)
        Dim SqlPass As String = String.Empty
        SqlPass = Arg(3).Substring(1, Arg(3).Length - 1)
        Dim SqlData As String = String.Empty
        SqlData = Arg(4).Substring(1, Arg(4).Length - 1)
        If Arg.Length >= 4 Then
            Dim Base As New Base.Program()
            Base.Run(SqlHost, SqlName, SqlPass, SqlData)
        Else
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("Argument Exception")
            Console.ForegroundColor = ConsoleColor.White
            Console.WriteLine("Run with "" mysqlhost mysqlusername mysqlpassword mysqldatabase"" in the Arguments!")
            Console.Read()
        End If
        Exit Sub

EnterDebugMode:


        Dim BaseDebug As New Base.Program()
        BaseDebug.Run("localhost", "root", "", "wremu")
    End Sub

End Module
