using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CryptoTrading.Services.Security
{
    public class Encryption
    {
        internal class Salt
        {
            string _Salt;
            string[] _SaltArr;
            static int[] _SaltLength = new int[] { 8, 12, 16, 20, 24 };

            public static Salt CreateNew(string hash)
            {
                Random rand = new Random((int)DateTime.Now.Ticks);
                int length = _SaltLength[rand.Next(0, 4)];
                string saltStr = hash.Substring(rand.Next(hash.Length - 1 - length), length);
                return new Salt(saltStr);
            }

            public static Salt CreateFromHashedPassword(ref string hash)
            {
                List<string> saltParts = new List<string>();
                int startpos = -1;
                int len = 0;
                for (int i = 0; i < hash.Length; i++)
                {
                    if (startpos > -1 && hash[i] == '$')
                    {
                        saltParts.Add(hash.Substring(startpos + 1, len));
                        hash = hash.Remove(startpos, len + 2);
                        i = i - (len + 1);
                        startpos = -1;
                        len = 0;
                    }
                    else if (hash[i] == '$')
                    {
                        startpos = i;
                    }
                    else if (startpos > -1)
                    {
                        len++;
                    }
                }
                return new Salt(string.Join(string.Empty, saltParts));
            }

            private Salt(string hash)
            {
                _Salt = hash;
                _SaltArr = _CreateSaltArr();
            }

            public string GetString()
            {
                return _Salt;
            }

            public override string ToString()
            {
                return GetString();
            }

            private string[] _CreateSaltArr()
            {
                int saltSplitLen = 4;
                string[] saltArr = new string[_Salt.Length / saltSplitLen];
                int n = 0;
                for (int i = 1; i <= _Salt.Length; i++)
                {
                    if (i % saltSplitLen == 0)
                    {
                        saltArr[n] = "$" + _Salt.Substring(i - saltSplitLen, saltSplitLen) + "$";
                        n++;
                    }
                }
                return saltArr;
            }

            public string MergeSaltWithHash(string hash)
            {
                int saltArrLen = _SaltArr.Length;
                Random rnd = new Random((int)DateTime.Now.Ticks);
                int maxLenSplit = (int)Math.Floor((decimal)hash.Length / saltArrLen);
                int[] positions = new int[saltArrLen];
                positions[0] = 0;
                hash = hash.Insert(positions[0], _SaltArr[0]);
                for (int i = 1; i < saltArrLen; i++)
                {
                    positions[i] = rnd.Next(positions[i - 1] + 1 + _SaltArr[i].Length, hash.Length - maxLenSplit * (saltArrLen - i) - 1);
                    hash = hash.Insert(positions[i], _SaltArr[i]);
                }
                return hash;
            }
        }

        private DateTime _StartTime = DateTime.Now;
        private string _TimeHash;
        private Salt _Salt;
        private string _PWHash;

        #region Public methods

        #region Static methods

        /// <summary>
        /// Creates an empty class. This is required if a new password will be hashed.
        /// </summary>
        /// <returns>Encryption class</returns>
        public static Encryption Initialize()
        {
            return new Encryption();
        }

        /// <summary>
        /// Initializes the class using a perviously used hash (this is required if a password validation will be performed)
        /// </summary>
        /// <param name="hash">Password hash for validation</param>
        /// <returns>Encryption class</returns>
        public static Encryption Initialize(string hash)
        {
            return new Encryption(hash);
        }

        public static byte[] EncryptAesCBC(byte[] inputData, string password)
        {
            using (MemoryStream strm = new MemoryStream())
            {
                using (Aes AES = Aes.Create())
                {
                    AES.Mode = CipherMode.CBC;
                    AES.Padding = PaddingMode.PKCS7;
                    AES.KeySize = 256;
                    //	AES.BlockSize = 256;
                    byte[] IV = AES.IV;
                    byte[] pw = _FormatPassword(password, AES.KeySize);

                    using (CryptoStream cStream = new CryptoStream(strm, AES.CreateEncryptor(pw, IV), CryptoStreamMode.Write))
                    {
                        cStream.Write(inputData, 0, inputData.Length);
                    }
                    byte[] encrypted = strm.ToArray();
                    byte[] result = new byte[IV.Length + encrypted.Length];
                    Buffer.BlockCopy(IV, 0, result, 0, IV.Length);
                    Buffer.BlockCopy(encrypted, 0, result, IV.Length, encrypted.Length);
                    return result;
                }
            }
        }

        public static string EncryptAesCBC(string inputData, string password)
        {
            return Convert.ToBase64String(EncryptAesCBC(Encoding.UTF8.GetBytes(inputData), password));
        }

        public static byte[] EncryptAesCBC<T>(T inputData, string password)
        {

            byte[] serializedObject = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(inputData));
            return EncryptAesCBC(serializedObject, password);
        }

        public static byte[] DecryptAesCBC(byte[] secret, string password)
        {
            byte[] iv = new byte[16]; //initial vector is 16 bytes
            byte[] encryptedContent = new byte[secret.Length - iv.Length]; //the rest should be encryptedcontent

            //Copy data to byte array
            Buffer.BlockCopy(secret, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(secret, iv.Length, encryptedContent, 0, encryptedContent.Length);

            using (MemoryStream strm = new MemoryStream())
            {
                using (Aes AES = Aes.Create())
                {
                    AES.Mode = CipherMode.CBC;
                    AES.Padding = PaddingMode.PKCS7;
                    AES.KeySize = 256;
                    //	AES.BlockSize = 256;

                    byte[] pw = _FormatPassword(password, AES.KeySize);

                    using (CryptoStream cs = new CryptoStream(strm, AES.CreateDecryptor(pw, iv), CryptoStreamMode.Write))
                    {
                        cs.Write(encryptedContent, 0, encryptedContent.Length);

                    }
                    return strm.ToArray();
                }
            }
        }

        public static byte[] DecryptAesCBC(string secret, string password)
        {
            return DecryptAesCBC(Convert.FromBase64String(secret), password);
        }

        public static T DecryptAesCBC<T>(byte[] inputData, string password)
        {
            byte[] decryptedData = DecryptAesCBC(inputData, password);
            return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(decryptedData));
        }

        #endregion Static methods

        /// <summary>
        /// Validate the password set in the parameter
        /// </summary>
        /// <param name="password">Value of password to be validated</param>
        /// <returns>True if the given password is valid</returns>
        public bool Validate(string password)
        {
            string hashed = _GenerateHashString(password + _Salt.GetString());
            return hashed == _PWHash;
        }

        /// <summary>
        /// Creates the hash of the password given in the first parameter
        /// </summary>
        /// <param name="password">This is the plaintext string which will be hashed</param>
        /// <returns>Hashed password</returns>
        public string EncyptPassword(string password)
        {
            string data = _GenerateHashString(_GenerateHash(password + _Salt.GetString()));
            return _Salt.MergeSaltWithHash(data);
        }

        #endregion Public methods

        #region Private methods

        private Encryption()
        {
            _TimeHash = _GenerateHashString(_StartTime.ToString());
            _Salt = Salt.CreateNew(_TimeHash);
        }

        private Encryption(string hash)
        {
            _Salt = Salt.CreateFromHashedPassword(ref hash);
            _PWHash = hash;
        }

        private byte[] _GenerateHash(string data)
        {
            SHA384 sha = SHA384.Create();
            return sha.ComputeHash(Encoding.UTF8.GetBytes(data));
        }

        private string _GenerateHashString(string data)
        {
            return _GenerateHashString(_GenerateHash(data));
        }

        private string _GenerateHashString(byte[] data)
        {
            return Convert.ToBase64String(data);
        }

        private static byte[] _FormatPassword(string inputPW, int keyLength)
        {
            byte[] pwArr = Encoding.UTF8.GetBytes(inputPW);
            int length = keyLength / 8;
            string newPw = "";

            int i = 0;
            while (newPw.Length < length)
            {
                newPw += inputPW[i++];
                if (i == inputPW.Length)
                {
                    i = 0;
                }
            }

            return Encoding.UTF8.GetBytes(newPw);
        }

        #endregion Private methods
    }
}
