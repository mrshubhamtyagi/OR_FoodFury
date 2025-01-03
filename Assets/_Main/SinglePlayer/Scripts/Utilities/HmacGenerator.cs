using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.Networking;

public class HmacGenerator
{
    public static void AddHmacAndTimestampToRequest(string data, UnityWebRequest www)
    {
        // Debug.Log(data);
        string timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        www.SetRequestHeader("Timestamp", timestamp.ToString());
        www.SetRequestHeader("HMAC", GenerateHmacSignature(data, timestamp));
    }
    private static string GenerateHmacSignature(string data, string timestamp)
    {
        string dataToSign = "";
        if (data == null)
        {
            dataToSign = $"{timestamp}";
        }
        else
        {
            dataToSign = $"{data}{timestamp}";

        }

        using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(Constants.HMAC_KEY)))
        {
            byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToSign));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }


}