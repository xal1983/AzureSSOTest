Imports Microsoft.Identity.Client

Public Class SSOHelper

    Private app As IPublicClientApplication
    Private ClientID As String = "b1b4ed2c-1bde-41f6-850b-f1fc263f20f3"
    Private Authority As String = "https://login.microsoftonline.com/softwarethinking.com"
    Public Sub New()
        app = PublicClientApplicationBuilder.Create(ClientID).
            WithDefaultRedirectUri().
            Build()
    End Sub
    Public Async Function AcquireToken() As Task(Of AuthInfo)
        Dim scopes() As String = {"user.read"}
        Dim accounts = Await app.GetAccountsAsync()
        Dim result As AuthenticationResult = Nothing
        Dim interactiveRequired As Boolean
        Try
            result = Await app.AcquireTokenSilent(scopes, accounts.FirstOrDefault()).WithAuthority(Authority).ExecuteAsync()
        Catch ex As MsalUiRequiredException
            interactiveRequired = True
        End Try
        If interactiveRequired Then
            result = Await app.AcquireTokenInteractive(scopes).WithAuthority(Authority).ExecuteAsync()
        End If
        Return New AuthInfo() With {
            .AccessToken = result.AccessToken,
            .Username = result.Account.Username
            }

    End Function



End Class

Public Class AuthInfo
    Public Property AccessToken As String
    Public Property Username As String
End Class
