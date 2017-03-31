Imports System.Data.SQLite
Module globalvars
    Public con As SQLiteConnection
    Public Sub connect()
        con = New SQLiteConnection("Data Source=classes.db;")
    End Sub


    Public Function sqlexec(ByVal query As String) As Boolean
        Try
            connect()
            Dim cmd As SQLiteCommand
            con.Open()
            cmd = con.CreateCommand
            cmd.CommandText = query
            cmd.ExecuteNonQuery()
            con.Close()
            Return True
        Catch ex As Exception
            Return False
        End Try
        Return False

    End Function

    Public Function getOneValue(ByVal query As String) As String
        Try
            Dim DT As New DataTable()
            Dim cmd As SQLiteCommand
            connect()
            con.Open()
            cmd = con.CreateCommand()
            Dim da = New SQLiteDataAdapter(query, con)
            da.Fill(DT)
            Return DT.Rows(0)(0).ToString
        Catch ex As Exception
            Return ex.Message
        End Try
        Return "error"
    End Function
    Public Function sqlTable(ByVal query As String) As DataTable
        Dim cmd As SQLiteCommand
        Dim da As SQLiteDataAdapter
        Try
            Dim DT As New DataTable()
            connect()
            con.Open()
            cmd = con.CreateCommand()
            da = New SQLiteDataAdapter(query, con)
            da.Fill(DT)
            Return DT
        Catch ex As Exception
            Return New DataTable
        End Try
    End Function
End Module
