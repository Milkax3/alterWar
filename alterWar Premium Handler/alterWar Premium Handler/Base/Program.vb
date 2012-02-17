Namespace Base
    Public Class Program

        Private _MySQL As MySqlClass
        Private _Levels As LevelManager
        Private _RefreshPremiumThreads As Threading.Thread

        Public Sub Run(ByVal _MySqlHost As String, ByVal _MySqlUsername As String, ByVal _MySqlPassword As String, ByVal _MySqlDatabase As String)
            Console.ForegroundColor = ConsoleColor.Yellow
            Console.WriteLine("alterWar Premium Server")
            Console.WriteLine("  This will update all Premium Times in")
            Console.WriteLine("  the Database every 60 seconds!")
            Console.ForegroundColor = ConsoleColor.Green
            Console.WriteLine("---------------------------------------------------")
            Console.ForegroundColor = ConsoleColor.White
            Console.WriteLine("Connecting to '" & _MySqlHost & "' with '" & _MySqlUsername & "':'" & _MySqlPassword & "' to Database '" & _MySqlDatabase & "'")
            _MySQL = New MySqlClass(_MySqlHost, _MySqlUsername, 3306, _MySqlPassword, _MySqlDatabase)
            Console.WriteLine("Successfully connected!")
            Console.WriteLine("")
            _RefreshPremiumThreads = New Threading.Thread(AddressOf PremiumRefresh)
            _RefreshPremiumThreads.Start()
        End Sub

        Private Sub PremiumRefresh()
            Dim sw As New Stopwatch()
            While True
                sw.Reset()
                sw.Start()
                RefreshAllPremium()
                sw.Stop()
                Console.WriteLine("Premium refreshed in " & sw.ElapsedMilliseconds & "ms")
                sw.Reset()
                sw.Start()
                Dim TotalItemsRemoved As Integer = RefreshAllItems()
                sw.Stop()
                Console.WriteLine("Inventory updated [total Out: " & TotalItemsRemoved & "] in " & sw.ElapsedMilliseconds & "ms")
                Threading.Thread.Sleep(60000)
            End While
        End Sub
        Private Function RefreshAllItems()
            Dim TotalItemsRemoved As Integer = -1
            Try
                Dim Query As MySql.Data.MySqlClient.MySqlDataReader = _MySQL.ExecuteReader("SELECT `id`,`expireDate` FROM `inventory`")
                Dim UpdateList As New List(Of String)
                While Query.Read()
                    Dim _ID As Long = 0
                    _ID = Query.GetInt64("id")
                    Dim _ExpireDate As Long = 0
                    _ExpireDate = Query.GetInt64("expireDate")
                    If _ExpireDate < Utils.CurrentTime() Then
                        UpdateList.Add("DELETE FROM `wremu`.`inventory` WHERE `inventory`.`id` = '" & _ID & "'")
                    End If
                End While
                Query.Close()

                For Each Update As String In UpdateList
                    _MySQL.Execute(Update)
                Next
                TotalItemsRemoved = UpdateList.Count
            Catch ex As Exception
                Console.ForegroundColor = ConsoleColor.Red
                Console.WriteLine("Inventory refresh error!")
                Console.ForegroundColor = ConsoleColor.Yellow
                Console.Write(ex.ToString())
                Console.Read()
            End Try
            Return TotalItemsRemoved
        End Function
        Private Sub RefreshAllPremium()
            'Try
            Dim Query As MySql.Data.MySqlClient.MySqlDataReader = _MySQL.ExecuteReader("SELECT `id`,`Premium`,`PremiumTime`,`PremiumDate`,`PremiumLeft` FROM `account_detail`")
            Dim UpdateList As New List(Of String)

            While Query.Read()
                Dim _PremiumLeft As Integer = 0 '                               In Minutes, will be read by GameServer
                _PremiumLeft = Query.GetInt16("PremiumLeft")
                Dim _Premium As Byte = 0  '                                     0 None; 1 Bronce; 2 Silver; 3 Gold; 4 Platinum;
                _Premium = Byte.Parse(Query.GetInt32("Premium"))
                Dim _PremiumDate As String = String.Empty '                     Timestamp
                _PremiumDate = Query.GetValue(3)
                Dim _PremiumTime As Integer = 0 '                               In days
                _PremiumTime = Query.GetInt32("PremiumTime")
                Dim _UserID As Int64 = 0 '                                      To save later
                _UserID = Query.GetInt64("id")
                If _Premium > 0 Then
                    Dim tmpDate As DateTime = DateTime.Parse(_PremiumDate)
                    Dim premiumBegin As TimeSpan = New TimeSpan(tmpDate.Ticks) 'CURRENT_TIMESTAMP (timestamp)
                    Dim premiumTime As TimeSpan = New TimeSpan(CLng(CLng(_PremiumTime) * CLng(24) * CLng(60) * CLng(60) * CLng(10000000))) 'in days
                    Dim Difference As TimeSpan = New TimeSpan(Now.Ticks) - premiumBegin
                    Dim premiumTimeTotal As TimeSpan = premiumTime - Difference
                    Dim premiumTimeMinutes As Integer = premiumTimeTotal.TotalMinutes
                    If premiumTimeMinutes > 0 Then
                        _PremiumLeft = premiumTimeMinutes
                    Else
                        _PremiumTime = -1
                        _Premium = 0
                    End If
                Else
                    _PremiumLeft = -1
                End If
                UpdateList.Add("UPDATE `wremu`.`account_detail` SET `PremiumLeft` = '" & _PremiumLeft & "' WHERE `account_detail`.`id` = '" & _UserID & "';")
            End While
            Query.Close()

            For Each Update As String In UpdateList
                _MySQL.Execute(Update)
            Next
            'Catch ex As Exception
            '    Console.ForegroundColor = ConsoleColor.Red
            '    Console.WriteLine("Premium refresh error!")
            '    Console.ForegroundColor = ConsoleColor.Yellow
            '    Console.Write(ex.ToString())
            '    Console.Read()
            'End Try
        End Sub
    End Class
End Namespace
