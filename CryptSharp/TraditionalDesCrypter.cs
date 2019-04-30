﻿using CryptSharp.Internal;
using CryptSharp.Utility;
using System;
using System.Text.RegularExpressions;

namespace CryptSharp
{
    /// <summary>
    /// Traditional DES crypt.
    /// </summary>
    public class TraditionalDesCrypter : Crypter
    {
        const int MaxPasswordLength = 8;

        static readonly Regex _regex = new Regex(Regex, RegexOptions.CultureInvariant);

        static CrypterOptions _properties = new CrypterOptions()
        {
            { CrypterProperty.MaxPasswordLength, MaxPasswordLength }
        }.MakeReadOnly();

        /// <inheritdoc />
        public override string GenerateSalt(CrypterOptions options)
        {
            Check.Null("options", options);

            string salt;
            do { salt = Base64Encoding.UnixMD5.GetString(Security.GenerateRandomBytes(2)).Substring(0, 2); }
            while (FilterSalt(salt) != salt);
            return salt;
        }

        /// <inheritdoc />
        public override bool CanCrypt(string salt)
        {
            Check.Null("salt", salt);

            return _regex.IsMatch(salt);
        }

        /// <inheritdoc />
        public override string Crypt(byte[] password, string salt)
        {
            Check.Null("password", password);
            Check.Null("salt", salt);

            Match match = _regex.Match(salt);
            if (!match.Success) { throw Exceptions.Argument("salt", "Invalid salt."); }

            byte[] crypt = null, input = null;
            try
            {
                string saltString = FilterSalt(match.Groups["salt"].Value);

                input = new byte[8];
                int length = ByteArray.NullTerminatedLength(password, input.Length);
                Array.Copy(password, input, Math.Min(length, input.Length));

                // DES Crypt ignores the high bit of every byte.
                for (int n = 0; n < 8; n++) { input[n] <<= 1; }
                using (DesCipher cipher = DesCipher.Create(input))
                {
                    int saltValue =
                        Base64Encoding.UnixCrypt.GetValue(saltString[0]) << 0 |
                        Base64Encoding.UnixCrypt.GetValue(saltString[1]) << 6;

                    crypt = new byte[8];
                    cipher.Crypt(crypt, 0, 25, saltValue);
                }

                return saltString + Base64Encoding.UnixCrypt.GetString(crypt);
            }
            finally
            {
                Security.Clear(crypt);
                Security.Clear(input);
            }
        }

        // NOTE: While debugging test vectors (actually, when trying to eliminate the effects of salting to determine why
        //       my implementation wasn't matching...) I discovered PHP crypt() replaces a zero salt with a one-salt.
        //       I'll do the same for compatibility's sake, since really, DES support is *entirely* for compatibility's sake...
        static string FilterSalt(string salt)
        {
            return salt == ".." ? "/." : salt;
        }

        /// <inheritdoc />
        public override CrypterOptions Properties
        {
            get { return _properties; }
        }

        static string Regex
        {
            get { return @"\A(?<salt>[A-Za-z0-9./]{2})(?<hash>[A-Za-z0-9./]{11})?\z"; }
        }
    }
}