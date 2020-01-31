Public Class Form1
    Dim helper As SSOHelper
    Public Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        helper = New SSOHelper()
    End Sub
    Private Async Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Button1.Enabled = False
        Label1.Text = "Signing in..."
        Try
            Await SignIn()
        Catch ex As Exception
            Label1.Text = "Failed"
            MessageBox.Show(ex.ToString())
        Finally
            Button1.Enabled = True
        End Try
    End Sub

    Public Async Function SignIn() As Task
        Dim r = Await helper.AcquireToken()
        Label1.Text = r.Username
    End Function
End Class
