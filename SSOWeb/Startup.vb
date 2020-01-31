
Imports System.Threading.Tasks
Imports Microsoft.IdentityModel.Protocols.OpenIdConnect
Imports Microsoft.IdentityModel.Tokens
Imports Microsoft.Owin
Imports Microsoft.Owin.Security
Imports Microsoft.Owin.Security.Cookies
Imports Microsoft.Owin.Security.Notifications
Imports Microsoft.Owin.Security.OpenIdConnect
Imports Owin

<Assembly: OwinStartup(GetType(Startup))>
Public Class Startup
    Private ClientID As String = "b1b4ed2c-1bde-41f6-850b-f1fc263f20f3"
    Private tenant As String = "softwarethinking.com"
    Private AuthorityFormatString As String = "https://login.microsoftonline.com/{0}/v2.0"
    Private redirecturi = "https://localhost:44390/Account/ExtAuth"
    Public Sub Configuration(app As IAppBuilder)

        Dim authority = String.Format(AuthorityFormatString, tenant)
        app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType)

        app.UseCookieAuthentication(New CookieAuthenticationOptions())
        app.UseOpenIdConnectAuthentication(
            New OpenIdConnectAuthenticationOptions With
            {
                .ClientId = ClientID, 'Sets the ClientId, authority, RedirectUri as obtained from web.config
                .Authority = authority,
                .RedirectUri = redirecturi,
                .PostLogoutRedirectUri = "https://localhost:44390/Account/ExtAuthSignOut", 'PostLogoutRedirectUri Is the page that users will be redirected To after sign-out. In this Case, it Is using the home page
                .Scope = OpenIdConnectScope.OpenIdProfile,
                .ResponseType = OpenIdConnectResponseType.IdToken, 'ResponseType Is set to request the id_token - which contains basic information about the signed-in user
                .TokenValidationParameters = New TokenValidationParameters() With
                {
                    .ValidateIssuer = False 'Allow all tenants and personal accounts
                },
                .Notifications = New OpenIdConnectAuthenticationNotifications With
                {
                    .AuthenticationFailed = AddressOf OnAuthenticationFailed 'OpenIdConnectAuthenticationNotifications configures OWIN to send notification of failed authentications to OnAuthenticationFailed method
                }
            }
        )
    End Sub
    Private Function OnAuthenticationFailed(context As AuthenticationFailedNotification(Of OpenIdConnectMessage, OpenIdConnectAuthenticationOptions)) As Task
        context.HandleResponse()
        context.Response.Redirect("/?errormessage=" & context.Exception.Message)
        Return Task.FromResult(0)
    End Function
End Class
