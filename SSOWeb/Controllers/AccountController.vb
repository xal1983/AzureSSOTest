Imports Microsoft.Owin.Security
Imports Microsoft.Owin.Security.OpenIdConnect

Public Class AccountController
    Inherits System.Web.Mvc.Controller

    Function LogOn() As ActionResult
        Return View()
    End Function

    <ValidateInput(False)>
    <HttpPost()>
    Function LogOn(ByVal model As LoginModel, ByVal returnUrl As String) As ActionResult
        If ModelState.IsValid Then
            Try
                If Membership.ValidateUser(model.UserName, model.Password) Then
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe)
                    If Url.IsLocalUrl(returnUrl) AndAlso returnUrl.Length > 1 AndAlso returnUrl.StartsWith("/") _
               AndAlso Not returnUrl.StartsWith("//") AndAlso Not returnUrl.StartsWith("/\\") Then
                        Return Redirect(returnUrl)
                    Else
                        Return RedirectToAction("Index", "Home")
                    End If
                Else
                    ModelState.AddModelError("", "The user name or password provided is incorrect.")
                End If
            Catch ex As Exception
                ModelState.AddModelError("", ex.Message)
            End Try
        End If

        ' If we got this far, something failed, redisplay form
        Return View(model)
    End Function
    <HttpPost()>
    Function ExternalLogOn() As ActionResult
        If Not Request.IsAuthenticated Then
            HttpContext.GetOwinContext().Authentication.Challenge(
                 New AuthenticationProperties With {.RedirectUri = "/"},
                    OpenIdConnectAuthenticationDefaults.AuthenticationType)
        End If
    End Function
    Function ExtAuth() As ActionResult
        Return View()
    End Function


End Class
