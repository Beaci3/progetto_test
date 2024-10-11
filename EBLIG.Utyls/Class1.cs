using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EBLIG.Utyls
{
    public class Crypto
    {
        public static string Encrypt(string plainText)
        {
            string chiave = "AxTYQWCvGTFRbgLL";
            string iv = "QWExcfTyUxxLOafO";

            RijndaelManaged rjm = new RijndaelManaged();
            rjm.KeySize = 128;
            rjm.BlockSize = 128;
            rjm.Key = ASCIIEncoding.ASCII.GetBytes(chiave);
            rjm.IV = ASCIIEncoding.ASCII.GetBytes(iv);
            Byte[] input = Encoding.UTF8.GetBytes(plainText);
            Byte[] output = rjm.CreateEncryptor().TransformFinalBlock(input, 0,
                input.Length);
            return Convert.ToBase64String(output).Replace(" ", "+");
        }

        public static string Decrypt(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "";
            }

            string chiave = "AxTYQWCvGTFRbgLL";
            string iv = "QWExcfTyUxxLOafO";

            RijndaelManaged rjm = new RijndaelManaged();
            rjm.KeySize = 128;
            rjm.BlockSize = 128;
            rjm.Key = ASCIIEncoding.ASCII.GetBytes(chiave);
            rjm.IV = ASCIIEncoding.ASCII.GetBytes(iv);
            try
            {
                value = value.Replace(" ", "+");
                Byte[] input = Convert.FromBase64String(value);
                Byte[] output = rjm.CreateDecryptor().TransformFinalBlock(input, 0,
                    input.Length);
                return Encoding.UTF8.GetString(output);
            }
            catch
            {
                return value;
            }
        }

    }
}
