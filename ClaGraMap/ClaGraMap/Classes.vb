Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports MetroUI_Form.WinApi
Imports System.Drawing
Imports System.Data.SQLite
Public Class Classes

    Dim id As String
    Dim cmd As SQLiteCommand
    Dim da As SQLiteDataAdapter
    Dim DS As New DataSet()
#Region "windowbehavior"
    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()
        Me.FormBorderStyle = Windows.Forms.FormBorderStyle.None
        ' Add any initialization after the InitializeComponent() call.
    End Sub

    Private Const CS_DROPSHADOW As Integer = 131072

    Protected Overrides ReadOnly Property CreateParams() As System.Windows.Forms.CreateParams
        Get
            Dim cp As CreateParams = MyBase.CreateParams
            cp.ClassStyle = cp.ClassStyle Or CS_DROPSHADOW
            Return cp
        End Get
    End Property
#End Region
#Region " ClientAreaMove Handling "
    Const WM_NCHITTEST As Integer = &H84
    Const HTCLIENT As Integer = &H1
    Const HTCAPTION As Integer = &H2
    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
        Select Case m.Msg
            Case WM_NCHITTEST
                MyBase.WndProc(m)
                If m.Result = HTCLIENT Then m.Result = HTCAPTION
                'If m.Result.ToInt32 = HTCLIENT Then m.Result = IntPtr.op_Explicit(HTCAPTION) 'Try this in VS.NET 2002/2003 if the latter line of code doesn't do it... thx to Suhas for the tip.
            Case Else
                'Make sure you pass unhandled messages back to the default message handler.
                MyBase.WndProc(m)
        End Select
    End Sub
#End Region
    Private Sub Classes_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        loadClasses()
        loadsems()
        loadsubjs()
    End Sub
    Private Sub loadsubjs()
        Try
            Dim DT As New DataTable()
            connect()
            con.Open()
            cmd = con.CreateCommand()
            Dim Sql = "select * from subjects"
            da = New SQLiteDataAdapter(Sql, con)
            DS.Reset()
            da.Fill(DT)
            cboSubject.DataSource = DT
            cboSubject.DisplayMember = "subjectname"
            cboSubject.ValueMember = "subjid"
        Catch ex As Exception

        End Try
    End Sub
    Private Sub loadsems()
        Try
            Dim DT As New DataTable()
            connect()
            con.Open()
            cmd = con.CreateCommand()
            Dim Sql = "select * from semesters"
            da = New SQLiteDataAdapter(Sql, con)
            DS.Reset()
            da.Fill(DT)
            cboSem.DataSource = DT
            cboSem.DisplayMember = "semname"
            cboSem.ValueMember = "semid"
        Catch ex As Exception

        End Try

    End Sub
    Private Sub loadClasses()
        Try
            Dim DT As New DataTable()
            connect()
            con.Open()
            cmd = con.CreateCommand()
            Dim Sql = "select c.classid, j.subjid as Subject, c.classname as Class, s.semname as Semester from classes c inner join semesters s on s.semid = c.semid inner join subjects j on j.subjid=c.subjid"
            da = New SQLiteDataAdapter(Sql, con)
            DS.Reset()
            da.Fill(DT)
            DataGridView1.DataSource = DT
            DataGridView1.Columns(0).Width = 0
        Catch ex As Exception

        End Try
    End Sub
    Private Sub Button1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        shtdown.ShowDialog()
    End Sub

    Private Sub Button2_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Me.WindowState = FormWindowState.Minimized
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        If btnEdit.Enabled And Not id = "" Then
            btnEdit.Enabled = False
            clear()
            Exit Sub
        End If
        If txtClassname.Text.Trim = "" Then
            MsgBox("Please input a valid course name like 'BSIT IV-1' and select the proper semester.")
        Else
            add()
            clear()

        End If

    End Sub
    Private Sub add()
        If Not sqlexec("insert into classes(classname,semid,subjid) values('" + txtClassname.Text.Trim + "','" + cboSem.SelectedValue.ToString + "','" + cboSubject.SelectedValue.ToString + "')") Then
            MsgBox("failed to add. please try again.")
        End If
        loadClasses()
    End Sub

    Private Sub DataGridView1_SelectionChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DataGridView1.SelectionChanged

        If DataGridView1.SelectedRows.Count > 0 Then
            id = DataGridView1.SelectedRows(0).Cells(0).Value.ToString()

            Try
                Dim DT As New DataTable()
                connect()
                con.Open()
                cmd = con.CreateCommand()
                Dim Sql = "select * from classes where classid='" + id + "'"
                da = New SQLiteDataAdapter(Sql, con)
                DS.Reset()
                da.Fill(DT)
                txtClassname.Text = DT.Rows(0)("classname").ToString()
                cboSem.SelectedValue = DT.Rows(0)("semid").ToString()
                cboSubject.SelectedValue = DT.Rows(0)("subjid").ToString()
                btnEdit.Enabled = True
            Catch ex As Exception

            End Try
        End If
    End Sub
    Private Sub clear()
        loadClasses()
        loadsems()
        loadsubjs()
        id = ""
        txtClassname.Text = ""
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        clear()
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        If id = "" Then
            MsgBox("Select from the list first, then edit the data on the text boxes before clicking this button.")
        Else
            If DataGridView1.SelectedRows.Count > 0 And MsgBox("Are you SURE you want to DELETE the selected ITEM?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                sqlexec("delete from classes where classid '" + DataGridView1.SelectedRows(0).Cells(0).Value.ToString() + "'")
            End If
        End If
    End Sub

    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        Me.Close()
        MetroForm.Show()

    End Sub

    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button8.Click
        subjects.ShowDialog()
        loadsubjs()
    End Sub

    Private Sub cboSubject_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboSubject.SelectedIndexChanged

    End Sub

    Private Sub btnEdit_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEdit.Click
        If id = "" Then
            MsgBox("Select from the list first, then edit the data on the text boxes before clicking this button.")
        Else

            If txtClassname.Text.Trim = "" Then
                MsgBox("please fill up the text box with the proper values then try again.")
            Else
                If Not sqlexec("update classes set subjid='" + cboSubject.SelectedValue.ToString + "',classname='" + txtClassname.Text + "',semid='" + cboSem.SelectedValue.ToString + "' where classid='" + id + "'") Then
                    MsgBox("Failed to edit. please try again.")
                End If
                clear()
            End If
        End If
    End Sub
End Class