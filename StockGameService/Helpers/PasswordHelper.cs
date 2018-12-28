using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Capstone
{
    public class PasswordHelper
    {
        private string _password;
        private static int _workFactor = 200;
        private static int _saltSize = 16;

        public string Salt { get; private set; }
        public string Hash { get; private set; }

        //Use when Registering a new user
        public PasswordHelper(string password)
        {
            this._password = password;
            GenerateSalt();
            GenerateHash();
        }

        //Use this when verifying an existing user
        public PasswordHelper(string password, string salt)
        {
            this._password = password;
            this.Salt = salt;
            GenerateHash();
        }

        public bool Verify(string hash)
        {
            return Hash == hash;
        }

        #region Private methods

        //generates own salt
        private void GenerateSalt()
        {
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(_password, _saltSize, _workFactor);
            Salt = GetSalt(rfc);
        }

        private void GenerateHash()
        {
            Rfc2898DeriveBytes rfc = HashPasswordWithPBKDF2(_password, Salt);
            Hash = GetHash(rfc);
        }

        private static Rfc2898DeriveBytes HashPasswordWithPBKDF2(string password, string salt)
        {
            // Creates the crypto service provider and provides the salt - usually used to check for a password match
            Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt), _workFactor);

            return rfc2898DeriveBytes;
        }

        private static string GetHash(Rfc2898DeriveBytes rfc)
        {
            return Convert.ToBase64String(rfc.GetBytes(20));
        }

        private static string GetSalt(Rfc2898DeriveBytes rfc)
        {
            return Convert.ToBase64String(rfc.Salt);
        }

        #endregion
    }
}
