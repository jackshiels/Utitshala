using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Utitshala.Services
{
    /// <summary>
    /// Returns hash values for specific services that require randomised strings.
    /// </summary>
    public class SecurityHash
    {
        public SecurityHash() { }

        /// <summary>
        /// Returns a hash value for passwords or confirmation codes.
        /// </summary>
        /// <returns>A hashed string value.</returns>
        public string Hasher()
        {
            var SHA = SHA256.Create();
            byte[] toHash = SHA.ComputeHash(Encoding.UTF8.GetBytes(DateTime.Now.ToString()));
            string hash = BitConverter.ToString(toHash).Replace("-", String.Empty);
            hash = hash.Substring(0, 8).ToLower();

            return hash;
        }
    }
}