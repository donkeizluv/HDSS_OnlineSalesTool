namespace OnlineSalesCore.Service
{
    public interface ILdapAuth
    {
       bool Validate(string username, string password, string domain);
    }
}
