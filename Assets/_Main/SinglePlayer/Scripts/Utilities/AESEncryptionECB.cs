using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

public class AESEncryptionECB
{
    public static string Encrypt(string message)
    {
        UTF8Encoding utf8 = new UTF8Encoding();
        AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
        aes.BlockSize = 128;
        aes.KeySize = 256;
        aes.Key = UTF8Encoding.UTF8.GetBytes(Constants.AES_KEY);
        aes.Mode = CipherMode.ECB;
        aes.Padding = PaddingMode.PKCS7;
        byte[] data = Encoding.UTF8.GetBytes(message);
        using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
        {
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            byte[] bytes = utf8.GetBytes(message);
            cs.Write(bytes, 0, bytes.Length);
            cs.FlushFinalBlock();
            ms.Position = 0;
            bytes = new byte[ms.Length];
            ms.Read(bytes, 0, bytes.Length);
            return Convert.ToBase64String(bytes);
        }
    }

    public static bool IsBase64String(string input)
    {
        try
        {
            byte[] data = Convert.FromBase64String(input);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    public static bool IsAes256Encrypted(string input)
    {
        if (IsBase64String(input))
        {
            try
            {
                byte[] encryptedData = Convert.FromBase64String(input);

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Encoding.UTF8.GetBytes(Constants.AES_KEY);
                    aesAlg.Mode = CipherMode.ECB;
                    aesAlg.Padding = PaddingMode.PKCS7;
                    aesAlg.IV = new byte[16]; // IV should be known for decryption

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    byte[] decryptedData = decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);

                    // Check if the decrypted data is a valid UTF-8 string
                    string decryptedString = Encoding.UTF8.GetString(decryptedData);

                    // You can add more checks here if needed

                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        return false;
    }

    public static string Decrypt(string encryptedText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(Constants.AES_KEY);
            aesAlg.Mode = CipherMode.ECB;
            aesAlg.Padding = PaddingMode.PKCS7;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            byte[] cipherBytes = Convert.FromBase64String(encryptedText);

            using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }
}