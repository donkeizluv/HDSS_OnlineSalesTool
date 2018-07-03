namespace OnlineSalesCore.Services
{
    public interface IAPIAuth
    {
        bool Check(string sig, string guid);
        string Forge(string guid);
    }
}
