using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Helpers.Extensions
{
    public static class Strings
    {
        public static int? ToNullableInt32(this string s)
        {
            int i;
            if (Int32.TryParse(s, out i)) return i;
            return null;
        }

        public static bool IsNotZero(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return false;
            }
            if (s == "0")
            {
                return false;
            }
            return true;
        }

        public static bool EqualIgnoreCase(this String str1, string str2)
        {
            if (string.IsNullOrWhiteSpace(str1) || string.IsNullOrWhiteSpace(str2))
                return false;
            return string.Equals(str1, str2, StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool IsEmpty(this String str1)
        {
            return string.IsNullOrWhiteSpace(str1);
        }

        public static string CalculateMD5Hash(this String input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public static bool IsNotEmpty(this String str1)
        {
            return !string.IsNullOrWhiteSpace(str1);
        }
    }
}
