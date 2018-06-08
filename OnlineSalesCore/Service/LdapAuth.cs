using System.DirectoryServices.Protocols;

namespace OnlineSalesCore.Service
{
    public class LdapAuth : ILdapAuth
    {
        public bool Validate(string username, string password, string domain)
        {
             try
            {
                //string userdn;
                using (var lconn = new LdapConnection(new LdapDirectoryIdentifier(domain)))
                {
                    lconn.Bind(new System.Net.NetworkCredential(username, password, domain));
                    return true;
                }
            }
            catch (LdapException)
            {
                return false;
            }
        }
    }
}
