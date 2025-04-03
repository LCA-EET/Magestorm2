using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Threading;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System;
using UnityEngine.InputSystem.Controls;
using System.Text;
public static class Game
{
    public static bool Running;
    private static int _portNumber;
    private static byte[] _key;
    private static byte[] _iv;

    public static void Init()
    {
        Running = true;
        Debug.Log("IsLittleEndian:  " +  BitConverter.IsLittleEndian);
        FetchServerInfo();
        Language.Init();
        LayerManager.Init();
        InputControls.Init();
        Teams.Init();
        IPAddress ip = IPAddress.Parse("192.168.1.93");
        new UDPListener(6000, new IPEndPoint(ip, 6000));
    }
    private static int ComputeChecksum(byte[] data)
    {
        int toReturn = 0;
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] > 127)
            {
                toReturn += data[i] - 256;
                // In Java, the first bit of the byte is the sign byte. Java bytes range from -128 to 127. The bytes in C# in contrast are unsigned, so the first bit adds 2^7 (128) to the total. So, I have to subtract 128 * 2 for the checksum on the client (C#) to match that of the server (Java).
            }
            else
            {
                toReturn += data[i];
            }
        }
        return toReturn;
    }
    private static bool FetchServerInfo()
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                client.DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue
                {
                    NoCache = true
                };
                Task<string> t = client.GetStringAsync("https://www.fosiemods.net/ms2.php");
                string returned = t.Result;
                string[] returnedArray = returned.Split("<br>");
                _portNumber = int.Parse(returnedArray[0]);
                string key64 = returnedArray[1];
                string iv64 = returnedArray[2];
                Debug.Log("key64: " + key64);
                Debug.Log("iv64: " + iv64);
                _key = Convert.FromBase64String(key64);
                _iv = Convert.FromBase64String(iv64);
                Debug.Log("Key checksum: " + ComputeChecksum(_key) + ", Key Length: " + _key.Length);
                Debug.Log("IV checksum: " + ComputeChecksum(_iv) + ", IV Length: " + _iv.Length);
                return true;
            }
            catch (Exception e)
            {

            }
        }
        return false;
    }
}
