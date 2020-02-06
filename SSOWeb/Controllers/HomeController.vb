Public Class HomeController
    Inherits System.Web.Mvc.Controller
    <AllowAnonymous()>
    Function Index() As ActionResult
        Return View()
    End Function
    <Authorize()>
    Function About() As ActionResult
        ViewData("Message") = "Your application description page."
        Dim iden = HttpContext.GetOwinContext().Request.User.Identity
        ViewData("User") = iden.Name + " " + iden.GetType().Name
        Return View()
    End Function

    Function Contact() As ActionResult
        ViewData("Message") = "Your contact page."

        Return View()
    End Function
End Class
