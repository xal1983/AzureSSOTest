'Imports System.IdentityModel.Claims
Imports System.Net
Imports Microsoft.AspNet.Identity
Imports Microsoft.AspNet.Identity.Owin
Imports Microsoft.Owin
Imports Microsoft.Owin.Security
Imports Microsoft.Owin.Security.Cookies
Imports Microsoft.Owin.Security.OpenIdConnect
Imports System.Threading.Tasks
Imports System.Security.Claims

<AllowAnonymous()>
Public Class AccountController
    Inherits System.Web.Mvc.Controller

    Function LogOn(returnUrl As String) As ActionResult
        ViewData("ReturnUrl") = returnUrl
        Return View()
    End Function

    <ValidateInput(False)>
    <HttpPost()>
    Function LogOn(ByVal model As LoginModel, ByVal returnUrl As String) As ActionResult
        If ModelState.IsValid AndAlso model.UserName = "testuser" AndAlso model.Password = "test" Then

            ''Validate username / password, if valid, do the following:

            CreateClaimsIdentity(HttpContext.GetOwinContext(), model.UserName)
            If String.IsNullOrWhiteSpace(returnUrl) Then
                returnUrl = "~/"
            End If
            Return Redirect(returnUrl)
        End If

        ' If we got this far, something failed, redisplay form
        Return View(model)
    End Function


    Public Shared Sub CreateClaimsIdentity(context As IOwinContext, username As String)
        Dim claims = New List(Of Claim)()

        'create *required* claims
        claims.Add(New Claim(ClaimTypes.NameIdentifier, username))
        claims.Add(New Claim(ClaimTypes.Name, username))

        '// serialized AppUserState object
        'claims.Add(New Claim("userState", appUserState.ToString()));

        'Dim identity = New ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie)

        Dim identity = New ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationType)
        context.Authentication.SignIn(New AuthenticationProperties() With
        {
            .AllowRefresh = True,
            .IsPersistent = True,
            .ExpiresUtc = DateTime.UtcNow.AddDays(7)
        }, identity)
    End Sub
    Private ReadOnly Property AuthenticationManager As IAuthenticationManager
        Get
            Return HttpContext.GetOwinContext().Authentication
        End Get
    End Property


    Public Function LogOff() As ActionResult
        'If user is signed in with office 365
        If User.Identity.Name.IndexOf("@") >= 0 Then
            AuthenticationManager.SignOut(OpenIdConnectAuthenticationDefaults.AuthenticationType)
        Else
            AuthenticationManager.SignOut(CookieAuthenticationDefaults.AuthenticationType)
        End If
        Return RedirectToAction("Index", "Home")
    End Function


    <AcceptVerbs(HttpVerbs.Get Or HttpVerbs.Post)>
    Sub ExternalLogOn(returnUrl As String)
        If Not Request.IsAuthenticated Then
            HttpContext.GetOwinContext().Authentication.Challenge(
                 New AuthenticationProperties,
                    OpenIdConnectAuthenticationDefaults.AuthenticationType)
        Else
            Response.Redirect(returnUrl)
        End If
    End Sub

End Class
