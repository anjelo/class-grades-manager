Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Data.SQLite
Public Class MetroForm


    Dim cmd As SQLiteCommand
    Dim da As SQLiteDataAdapter

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
    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Me.WindowState = FormWindowState.Minimized
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        shtdown.ShowDialog()
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Classes.Show()
        Me.Hide()
    End Sub

    Private Sub MetroForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        loadsems()
        loadsubjs()
        cboClass.DataSource = sqlTable("select * from classes where semid = '" + cboSem.SelectedValue.ToString + "' and subjit ='" + cboSubject.SelectedValue.ToString + "'")
        cboClass.DisplayMember = "classname"
        cboClass.ValueMember = "classid"
    End Sub
    Private Sub loadsubjs()
        Try
            Dim DT As New DataTable()
            connect()
            con.Open()
            cmd = con.CreateCommand()
            Dim Sql = "select * from subjects"
            da = New SQLiteDataAdapter(Sql, con)
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
            da.Fill(DT)
            cboSem.DataSource = DT
            cboSem.DisplayMember = "semname"
            cboSem.ValueMember = "semid"
        Catch ex As Exception

        End Try

    End Sub

    Private Sub cboSem_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboSem.SelectedIndexChanged
        loadclass()
    End Sub
    Private Sub loadclass()
        Try
            Dim DT As New DataTable()
            connect()
            con.Open()
            cmd = con.CreateCommand()
            Dim Sql = "select * from classes where semid = '" + cboSem.SelectedValue.ToString + "'"
            da = New SQLiteDataAdapter(Sql, con)
            da.Fill(DT)
            cboClass.DataSource = DT
            cboClass.DisplayMember = "classname"
            cboClass.ValueMember = "classid"
        Catch ex As Exception

        End Try

    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        Dim g As New Grades(cboClass.SelectedValue.ToString)
        g.Show()
        Me.Hide()

    End Sub

    Private Sub cboSubject_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboSubject.SelectedIndexChanged
        cboClass.DataSource = sqlTable("select * from classes where semid = '" + cboSem.SelectedValue.ToString + "' and subjit ='" + cboSubject.SelectedValue.ToString + "'")
        cboClass.DisplayMember = "classname"
        cboClass.ValueMember = "classid"
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click

    End Sub

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        subjects.ShowDialog()
    End Sub
    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        Classes.Show()
        Me.Hide()
    End Sub

    Private Sub Button4_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs)
        loadsems()
        loadsubjs()
        cboClass.DataSource = sqlTable("select * from classes where semid = '" + cboSem.SelectedValue.ToString + "' and subjit ='" + cboSubject.SelectedValue.ToString + "'")
        cboClass.DisplayMember = "classname"
        cboClass.ValueMember = "classid"
    End Sub
End Class