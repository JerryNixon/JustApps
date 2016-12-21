using Windows.Security.Credentials;
using System.Linq;
using System.Diagnostics;

namespace Template10.Services.SecretService
{
    internal class SecretService
    {
        // https://msdn.microsoft.com/en-us/library/windows/apps/windows.security.credentials.passwordvault.aspx

        static PasswordVault _vault;

        static SecretService()
        {
            _vault = new PasswordVault();
        }

        public SecretService()
        {
            // empty
        }

        public string ReadSecret(string container, string key)
        {
            try
            {
                if (_vault.RetrieveAll().Any(x => x.Resource == container && x.UserName == key))
                {
                    try
                    {
                        var credential = _vault.Retrieve(container, key);
                        credential.RetrievePassword();
                        return credential.Password;
                    }
                    catch (System.Exception)
                    {
                        return string.Empty;
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (System.Exception ex)
            {
                Debugger.Break();
                throw;
            }
        }

        public void WriteSecret(string container, string key, string secret)
        {
            try
            {
                if (_vault.RetrieveAll().Any(x => x.Resource == container && x.UserName == key))
                {
                    var credential = _vault.Retrieve(container, key);
                    credential.RetrievePassword();
                    credential.Password = secret;
                }
                else if (!string.IsNullOrEmpty(secret))
                {
                    var credential = new PasswordCredential(container, key, secret);
                    _vault.Add(credential);
                }
            }
            catch (System.Exception ex)
            {
                Debugger.Break();
                throw;
            }
        }
    }
}
