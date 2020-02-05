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

Public Class AccountController
    Inherits System.Web.Mvc.Controller

    Function LogOn() As ActionResult
        Return View()
    End Function

    <ValidateInput(False)>
    <HttpPost()>
    Function LogOn(ByVal model As LoginModel, ByVal returnUrl As String) As ActionResult
        If ModelState.IsValid Then

            ''Validate username / password, if valid, do the following:

            Dim claims = New List(Of Claim)()

            'create *required* claims
            claims.Add(New Claim(ClaimTypes.NameIdentifier, model.UserName))
            claims.Add(New Claim(ClaimTypes.Name, model.UserName))

            '// serialized AppUserState object
            'claims.Add(New Claim("userState", appUserState.ToString()));

            Dim identity = New ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie)


            AuthenticationManager.SignIn(New AuthenticationProperties() With
            {
                .AllowRefresh = True,
                .IsPersistent = True,
                .ExpiresUtc = DateTime.UtcNow.AddDays(7)
            }, identity)

        End If

        ' If we got this far, something failed, redisplay form
        Return View(model)
    End Function

    Private ReadOnly Property AuthenticationManager As IAuthenticationManager
        Get
            Return HttpContext.GetOwinContext().Authentication
        End Get
    End Property


    Public Sub LogOff()
        AuthenticationManager.SignOut(
            OpenIdConnectAuthenticationDefaults.AuthenticationType,
            CookieAuthenticationDefaults.AuthenticationType,
            DefaultAuthenticationTypes.ApplicationCookie,
            DefaultAuthenticationTypes.ExternalCookie)
    End Sub

    <HttpPost()>
    Sub ExternalLogOn()
        If Not Request.IsAuthenticated Then
            HttpContext.GetOwinContext().Authentication.Challenge(
                 New AuthenticationProperties With {.RedirectUri = "/"},
                    OpenIdConnectAuthenticationDefaults.AuthenticationType)
        End If
    End Sub
    <AllowAnonymous>
    Function ExtAuth() As ActionResult



        Return View()
    End Function


    '<AllowAnonymous>
    'Public Async Function ExternalLoginCallback(returnUrl As String, urlHash As String) As Task(Of ActionResult)

    '    Dim loginInfo = Await AuthenticationManager.GetExternalLoginInfoAsync()
    '    If loginInfo Is Nothing Then

    '        Return RedirectToAction("LogOn")
    '    End If

    '    Dim claims = New List(Of Claim)()
    '    claims.Add(New Claim(ClaimTypes.Sid, "Office365"))

    '    'Sign in the user with this external login provider if the user already has a login
    '    Dim User = Await UserManager.FindAsync(loginInfo.Login)
    '    If User Is Nothing Then

    '        User = Await UserManager.FindByNameAsync(loginInfo.DefaultUserName)

    '        If User Is Nothing Then

    '            If User.AllowExternalLogin = False Then
    '                ModelState.AddModelError("", String.Format("User {0} not allowed to authenticate with Office 365.", loginInfo.DefaultUserName))
    '                Return View("Login")
    '            End If
    '            Dim result = Await UserManager.AddLoginAsync(User.Id, loginInfo.Login)

    '            If result.Succeeded Then
    '                                    {
    '                If claims IsNot Nothing Then
    '                                            {
    '                    Dim userIdentity = Await User.GenerateUserIdentityAsync(UserManager)
    '                    userIdentity.AddClaims(claims)
    '                End If
    '                Await SignInManager.SignInAsync(User, isPersistent:=False, rememberBrowser:=False)
    '            End If
    '            Return RedirectToLocal(returnUrl)

    '        Else

    '            ModelState.AddModelError("", String.Format("User {0} not found.", loginInfo.DefaultUserName))
    '            Return View("Login")
    '        End If

    '    Else


    '        If User.AllowExternalLogin = False Then
    '            ModelState.AddModelError("", String.Format("User {0} not allowed to authenticate with Office 365.", loginInfo.DefaultUserName))
    '            Return View("Login")
    '        End If

    '        If claims IsNot Nothing Then
    '                                            {
    '           Dim userIdentity = Await User.GenerateUserIdentityAsync(UserManager)
    '            userIdentity.AddClaims(claims)
    '        End If
    '        Await SignInManager.SignInAsync(User, isPersistent:=False, rememberBrowser:=False)
    '        Return RedirectToLocal(returnUrl)
    '    End If
    'End Function
End Class
