using Windows.Security.Credentials;
using System.Linq;

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

        public void WriteSecret(string container, string key, string secret)
        {
            if (_vault.RetrieveAll().Any(x => x.Resource == container && x.UserName == key))
            {
                var credential = _vault.Retrieve(container, key);
                credential.RetrievePassword();
                credential.Password = secret;
            }
            else
            {
                var credential = new PasswordCredential(container, key, secret);
                _vault.Add(credential);
            }
        }
    }
}
