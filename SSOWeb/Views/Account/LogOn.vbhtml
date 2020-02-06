@ModelType SSOWeb.LoginModel
@Code
    ViewData("Title") = "LogOn"
End Code

<h2>LogOn</h2>

@Using Html.BeginForm("LogOn", "Account", New With {.ReturnUrl = ViewData("ReturnUrl")}, FormMethod.Post, New With {.class = "form-4"})
    @Html.AntiForgeryToken()
    @<div>
        <h1>Sign in <span>to your account</span></h1>
        <p>@Html.ValidationSummary(True)</p>
        <p class="field-username">
            <label for="UserName">Username or email</label>
            @Html.TextBoxFor(Function(m) m.UserName, New With {.placeholder = "Username", .required = "required"})
        </p>
        <p class="field-password">
            <label for="Password">Password</label>
            @Html.PasswordFor(Function(m) m.Password, New With {.placeholder = "Password", .required = "required"})
        </p>
        <p id="input-checkbox">
            @Html.CheckBoxFor(Function(m) m.RememberMe, New With {.placeholder = "Password"})
            <label for="RememberMe" style="display:inline;">Remember me</label>
        </p>
        <p>
            <input type="submit" name="submit" value="Sign in">
        </p>
        @if ViewData("ForgotPassword") Then
            @<p><a href="@Url.Action("Index", "ForgotPassword")" style="display: inherit; text-align:center;">Forgot Password?</a></p>
        End If

    </div>
End Using
@Using Html.BeginForm("ExternalLogOn", "Account", New With {.ReturnUrl = ViewData("ReturnUrl")}, FormMethod.Post)
    @<div>
    <input type="submit" value="Sign in with microsoft account"/>
    </div>
End Using