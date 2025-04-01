using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Threading;
using System.Net;
public static class Game
{
    public static bool Running;

    public static void Init()
    {
        Running = true;
        Language.Init();
        LayerManager.Init();
        InputControls.Init();
        Teams.Init();
        IPAddress ip = IPAddress.Parse("192.168.1.93");
        new UDPListener(6000, new IPEndPoint(ip, 6000));
    }

   
}
