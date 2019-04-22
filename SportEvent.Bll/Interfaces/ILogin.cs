using SportEvent.Bll.Model;

namespace SportEvent.Bll.Interfaces
{
    public interface ILogin
    {
        string BuildToken(string aduser);
        bool Authenticate(LoginModel login);
    }
}
