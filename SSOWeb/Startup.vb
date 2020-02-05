
Imports System.Security.Claims
Imports System.Security.Principal
Imports System.Threading.Tasks
Imports Microsoft.AspNet.Identity
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
        'app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType)
        app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie)

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
                .AuthorizationCodeReceived = Function(context)
                                                 Return Task.FromResult(0)
                                             End Function,
                .SecurityTokenValidated = Function(context)
                                              Return Task.FromResult(0)
                                          End Function,
                                .TokenResponseReceived = Function(context)
                                                             Return Task.FromResult(0)
                                                         End Function,
                    .RedirectToIdentityProvider =
                        Function(context)
                            If context.Request.Path.Value = "/Account/ExternalLogOn" OrElse (context.Request.Path.Value = "/Account/LogOff" AndAlso IsExternalUser(context.Request.User.Identity)) Then
                                '// This ensures that the address used for sign in And sign out Is picked up dynamically from the request
                                '// this allows you to deploy your app (to Azure Web Sites, for example)without having to change settings
                                '// Remember that the base URL of the address used here must be provisioned in Azure AD beforehand.
                                Dim appBaseUrl = context.Request.Scheme + "://" + context.Request.Host.Value + context.Request.PathBase.Value
                                context.ProtocolMessage.RedirectUri = appBaseUrl + "/Account/ExtAuth"
                                context.ProtocolMessage.PostLogoutRedirectUri = appBaseUrl + "/Account/ExtAuthSignOut"
                            Else
                                '//This Is to avoid being redirected to the microsoft login page when deep linking And Not logged in 
                                context.State = Microsoft.Owin.Security.Notifications.NotificationResultState.Skipped
                                context.HandleResponse()
                            End If
                            Return Task.FromResult(0)
                        End Function,
        .AuthenticationFailed = AddressOf OnAuthenticationFailed 'OpenIdConnectAuthenticationNotifications configures OWIN to send notification of failed authentications to OnAuthenticationFailed method
                }
            }
        )

        '.SecurityTokenReceived = Function(context)
        '                             Return Task.FromResult(0)
        '                         End Function,
        app.UseCookieAuthentication(New CookieAuthenticationOptions() With {
    .AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
    .LoginPath = New PathString("/Account/LogOn")
})

    End Sub
    Private Shared Function IsExternalUser(identity As IIdentity) As Boolean
        Dim iden = TryCast(identity, ClaimsIdentity)
        If iden IsNot Nothing AndAlso iden.IsAuthenticated Then
            Dim value = iden.FindFirstValue(ClaimTypes.Sid)
            If value IsNot Nothing AndAlso value = "Office365" Then
                Return True
            End If
        End If
        Return False
    End Function
    Private Function OnAuthenticationFailed(context As AuthenticationFailedNotification(Of OpenIdConnectMessage, OpenIdConnectAuthenticationOptions)) As Task
        context.HandleResponse()
        context.Response.Redirect("/?errormessage=" & context.Exception.Message)
        Return Task.FromResult(0)
    End Function
End Class
