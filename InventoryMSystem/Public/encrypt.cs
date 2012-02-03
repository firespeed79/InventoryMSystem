using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace InventoryMSystem
{
    public class Encrypter
    {
        public static string PublicEncryptKey = "12345678";
        public static string GetDecryptString(string encrytedText, string encryptKey)
        {
            if (null == encrytedText || null == encryptKey)
            {
                return null;
            }
            string strR = null;

            DESCryptoServiceProvider keyDecrypt = new DESCryptoServiceProvider();
            keyDecrypt.Key = ASCIIEncoding.ASCII.GetBytes(encryptKey);
            keyDecrypt.IV = ASCIIEncoding.ASCII.GetBytes(encryptKey);
            MatchCollection mc = Regex.Matches(encrytedText, @"(?i)[\da-f]{2}");
            List<byte> buf = new List<byte>();//填充到这个临时列表中
            //依次添加到列表中
            foreach (Match m in mc)
            {
                buf.Add(Byte.Parse(m.ToString(), System.Globalization.NumberStyles.HexNumber));
            }
            strR = Decrypt(buf.ToArray(), keyDecrypt);
            return strR;
        }
        public static string GetEncryptString(string plainText, string encryptKey)
        {
            if (null == plainText || null == encryptKey)
            {
                return null;
            }
            string strR = null;

            DESCryptoServiceProvider keyEncrypt = new DESCryptoServiceProvider();
            keyEncrypt.Key = ASCIIEncoding.ASCII.GetBytes(encryptKey);
            keyEncrypt.IV = ASCIIEncoding.ASCII.GetBytes(encryptKey);
            byte[] bytesBuf = Encrypt(plainText, keyEncrypt);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytesBuf)
            {
                sb.Append(b.ToString("X2"));
            }
            strR = sb.ToString();
            return strR;
        }
        // Encrypt the string.
        static byte[] Encrypt(string PlainText, SymmetricAlgorithm key)
        {
            // Create a memory stream.
            MemoryStream ms = new MemoryStream();

            // Create a CryptoStream using the memory stream and the 
            // CSP DES key.  
            CryptoStream encStream = new CryptoStream(ms, key.CreateEncryptor(), CryptoStreamMode.Write);

            // Create a StreamWriter to write a string
            // to the stream.
            StreamWriter sw = new StreamWriter(encStream);

            // Write the plaintext to the stream.
            sw.WriteLine(PlainText);

            // Close the StreamWriter and CryptoStream.
            sw.Close();
            encStream.Close();

            // Get an array of bytes that represents
            // the memory stream.
            byte[] buffer = ms.ToArray();

            // Close the memory stream.
            ms.Close();

            // Return the encrypted byte array.
            return buffer;
        }

        // Decrypt the byte array.
        static string Decrypt(byte[] CypherText, SymmetricAlgorithm key)
        {
            // Create a memory stream to the passed buffer.
            MemoryStream ms = new MemoryStream(CypherText);

            // Create a CryptoStream using the memory stream and the 
            // CSP DES key. 
            CryptoStream encStream = new CryptoStream(ms, key.CreateDecryptor(), CryptoStreamMode.Read);

            // Create a StreamReader for reading the stream.
            StreamReader sr = new StreamReader(encStream);

            // Read the stream as a string.
            string val = sr.ReadLine();

            // Close the streams.
            sr.Close();
            encStream.Close();
            ms.Close();

            return val;
        }
    }
}
