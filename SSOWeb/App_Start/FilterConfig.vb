Imports System.Security.Principal
Imports System.Web
Imports System.Web.Mvc

Public Module FilterConfig
    Public Sub RegisterGlobalFilters(ByVal filters As GlobalFilterCollection)
        filters.Add(New HandleErrorAttribute())
        filters.Add(New CMAuthorizeAttribute)
    End Sub

    Public Class CMAuthorizeAttribute
        Inherits AuthorizeAttribute
        Public Overrides Sub OnAuthorization(filterContext As AuthorizationContext)
            MyBase.OnAuthorization(filterContext)
            Dim iden = filterContext.RequestContext.HttpContext.User.Identity

            If filterContext.RequestContext.HttpContext.User.Identity.IsAuthenticated Then
                Dim signInAs = iden.Name
                If String.IsNullOrWhiteSpace(signInAs) AndAlso TypeOf iden Is System.Security.Claims.ClaimsIdentity Then
                    Dim ci = CType(iden, System.Security.Claims.ClaimsIdentity)
                    signInAs = ci.Claims.FirstOrDefault(Function(o) o.Type = "preferred_username")?.Value
                End If
                'perform real principal "WebAuthenticatedLogin" 
                filterContext.RequestContext.HttpContext.User = New CustomPrincipal(New CustomIdentity(signInAs))
            End If

        End Sub
    End Class

    Public Class CustomPrincipal
        Inherits System.Security.Principal.GenericPrincipal
        Public Sub New(identity As System.Security.Principal.IIdentity)
            MyBase.New(identity, Nothing)
        End Sub
    End Class
    Public Class CustomIdentity
        Implements System.Security.Principal.IIdentity
        Public Sub New(name As String)
            Me.Name = name
        End Sub

        Public ReadOnly Property Name As String Implements IIdentity.Name


        Public ReadOnly Property AuthenticationType As String Implements IIdentity.AuthenticationType
            Get
                Return "Custom"
            End Get
        End Property

        Public ReadOnly Property IsAuthenticated As Boolean Implements IIdentity.IsAuthenticated
            Get
                Return True
            End Get
        End Property
    End Class
End Module