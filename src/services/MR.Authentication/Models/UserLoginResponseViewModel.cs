namespace MR.Authentication.Models;

public class UserLoginResponseViewModel
{
    public string AccessToken { get; set; }
    public double ExpiresIn { get; set; }
    public UserTokenViewModel UserToken { get; set; }
}
