Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports MetroUI_Form.WinApi
Imports System.Drawing
Imports System.Data.SQLite
Imports excel = Microsoft.Office.Interop.Excel

Public Class Grades
    Dim classid As String
    Dim bs = New BindingSource()

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
    Public Sub New(ByVal cid As String)
        SetStyle(ControlStyles.ResizeRedraw, True)

        InitializeComponent()
        classid = cid
    End Sub
    Private Sub Grades_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        loaddg()
        lblName.Text = getOneValue("select classname from classes where classid = '" + classid + "'")
    End Sub
    Dim dt As New DataTable
    Dim da As SQLiteDataAdapter
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        If MsgBox("Save your changes?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
            save()
        End If
        Me.Close()
        MetroForm.Show()
    End Sub
    Private Sub loaddg()

        DataGridView1.DataSource = bs
        Dim dt As New DataTable

        connect()
        Try
            da = New SQLiteDataAdapter("select * from students where classid ='" + classid + "'", con)
            Dim cb = New SQLiteCommandBuilder(da)
            da.Fill(dt)
            bs.DataSource = dt
            With DataGridView1
                .AutoGenerateColumns = True
                .AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells)
                .Columns("classid").ReadOnly = True
                .Columns("classid").Width = 0
                .Columns("stdid").Visible = False

            End With
        Catch ex As Exception

        End Try

    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        shtdown.ShowDialog()
    End Sub
    Public Sub datagridviewnr(ByRef dg As DataGridView)
        Try
            dg.Columns("classid").ReadOnly = False
            dg.Rows(dg.RowCount - 2).Cells("classid").Value = classid
            dg.Columns("classid").ReadOnly = True
        Catch ex As Exception

        End Try
    End Sub

    Private Sub save()
        Try
            DataGridView1.EndEdit()
            da.Update(CType(bs.DataSource, DataTable))
        Catch ex As Exception

        End Try

    End Sub

    Private Sub DataGridView1_UserAddedRow(ByVal sender As System.Object, ByVal e As System.Windows.Forms.DataGridViewRowEventArgs) Handles DataGridView1.UserAddedRow
        datagridviewnr(DataGridView1)
    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        save()
        connect()
        Dim dtx = New DataTable
        da = New SQLiteDataAdapter("select * from students where classid ='" + classid + "'", con)
        Dim cb = New SQLiteCommandBuilder(da)
        da.Fill(dtx)
        Dim oXL As excel.Application
        Dim oWB As excel.Workbook
        Dim oSheet As excel.Worksheet
        ' Dim oRange As excel.Range
        Dim savedialog As New SaveFileDialog
        savedialog.DefaultExt = "xls"
        savedialog.Filter = "Excel files (*.xls)|*.txt|All files (*.*)|*.*"
        If savedialog.ShowDialog = Windows.Forms.DialogResult.Cancel Then
            Exit Sub
        End If
        ' Start Excel and get Application object. 
        oXL = New excel.Application

        ' Set some properties 
        oXL.Visible = True
        oXL.DisplayAlerts = False

        ' Get a new workbook. 
        oWB = oXL.Workbooks.Add

        ' Get the active sheet 
        oSheet = DirectCast(oWB.ActiveSheet, excel.Worksheet)
        oSheet.Name = getOneValue("select classname from classes where classid = '" + classid + "'")

        ' Process the DataTable 
        'BE SURE TO CHANGE THIS LINE TO USE *YOUR* DATATABLE 
        'Dim dt As Data.DataTable =

        ' Create the data rows 
        Dim rowCount As Integer = 1
        For Each dr As DataRow In dtx.Rows
            rowCount += 1
            For i As Integer = 1 To dtx.Columns.Count
                ' Add the header the first time through 
                If rowCount = 2 Then
                    oSheet.Cells(1, i) = dtx.Columns(i - 1).ColumnName
                End If
                oSheet.Cells(rowCount, i) = dr.Item(i - 1).ToString
            Next
        Next

        ' Resize the columns 
        'oRange = oSheet.Range(oSheet.Cells(1, 1), _
        '         oSheet.Cells(rowCount, dt.Columns.Count))
        'oRange.EntireColumn.AutoFit()

        ' Save the sheet and close 
        oSheet = Nothing
        ' oRange = Nothing
        ' oWB.FileFormat=
        oWB.SaveAs(savedialog.FileName)
        oWB.Close()
        oWB = Nothing
        oXL.Quit()

        ' Clean up 
        ' NOTE: When in release mode, this does the trick 
        GC.WaitForPendingFinalizers()
        GC.Collect()
        GC.WaitForPendingFinalizers()
        GC.Collect()


    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Me.WindowState = FormWindowState.Maximized
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Me.WindowState = FormWindowState.Minimized
    End Sub

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        save()

    End Sub
End Class