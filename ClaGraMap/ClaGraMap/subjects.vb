Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports MetroUI_Form.WinApi
Imports System.Drawing
Imports System.Data.SQLite
Public Class subjects
    Dim cmd As SQLiteCommand
    Dim da As SQLiteDataAdapter
    Dim id As String
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
    Private Sub subjects_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        loadsubjects()
        btnEdit.Enabled = False
        txtCode.Focus()
    End Sub
    Private Sub loadsubjects()
        Try
            Dim DT As New DataTable()
            connect()
            con.Open()
            cmd = con.CreateCommand()
            Dim Sql = "select subjid as 'Subject Code', subjectname as 'Subject Name' from subjects"
            da = New SQLiteDataAdapter(Sql, con)
            da.Fill(DT)
            DataGridView1.DataSource = DT
        Catch ex As Exception

        End Try
    End Sub

    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        Me.Close()
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        If btnEdit.Enabled And Not id = "" Then
            btnEdit.Enabled = False
            clear()
            Exit Sub
        End If
        If txtCode.Text.Trim = "" Or txtName.Text.Trim = "" Then
            MsgBox("please fill up the text boxes with the proper values then try again.")
        Else
            add()
            clear()
        End If
    End Sub
    Private Sub clear()
        txtCode.Text = ""
        txtName.Text = ""
        id = ""
    End Sub
    Private Sub add()
        If Not sqlexec("insert into subjects values('" + txtCode.Text.Trim + "','" + txtName.Text + "')") Then
            MsgBox("Failed to add. please try again.")
        End If
        loadsubjects()
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        loadsubjects()
    End Sub

    Private Sub DataGridView1_SelectionChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DataGridView1.SelectionChanged
        btnEdit.Enabled = True
        If DataGridView1.SelectedRows.Count > 0 Then
            id = DataGridView1.SelectedRows(0).Cells(0).Value.ToString()

            Try
                Dim DT As New DataTable()
                connect()
                con.Open()
                cmd = con.CreateCommand()
                Dim Sql = "select * from subjects where subjid='" + id + "'"
                da = New SQLiteDataAdapter(Sql, con)
                da.Fill(DT)
                txtCode.Text = DT.Rows(0)("subjid").ToString()
                txtName.Text = DT.Rows(0)("subjectname").ToString()

            Catch ex As Exception

            End Try
        End If
    End Sub

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEdit.Click
        If id = "" Then
            MsgBox("Select from the list first, then edit the data on the text boxes before clicking this button.")
        Else

            If txtCode.Text.Trim = "" Or txtName.Text.Trim = "" Then
                MsgBox("please fill up the text boxes with the proper values then try again.")
            Else
                If Not sqlexec("update subjects set subjid='" + txtCode.Text.Trim + "',subjectname='" + txtName.Text + "'") Then
                    MsgBox("Failed to edit. please try again.")
                End If
                loadsubjects()
            End If
        End If
    End Sub
End Class