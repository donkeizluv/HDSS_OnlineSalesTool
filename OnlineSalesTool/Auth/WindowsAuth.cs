using System;
using System.DirectoryServices.Protocols;

namespace OnlineSalesTool.Auth
{
    static public class WindowsAuth
    {
        public static bool Validate_Principal(string userName, string pwd, string domain)
        {
            using (var pc = new System.DirectoryServices.AccountManagement
                .PrincipalContext(System.DirectoryServices.AccountManagement.ContextType.Domain, domain))
            {
                // validate the credentials
                return pc.ValidateCredentials(userName, pwd);
            }
        }

        //Pretty fast
        public static bool Validate_Principal2(string username, string password, string domain)
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
            catch (LdapException e)
            {
                return false;
            }
        }
    }
}
