using System;
using UnityEngine;

public static class TimeUtil
{
    private static long _serverTime;
    public static void SetServerTime(long serverTime)
    {
        _serverTime = serverTime;
    }
    public static long CurrentTime()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
    public static float MinutesLeft(long expirationTime)
    {
        long unixTimeMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        long delta = expirationTime - unixTimeMilliseconds;
        float minutesLeft = delta / 60000f;
        return minutesLeft;
    }

    public static string MinutesAndSecondsRemaining(long expirationTime)
    {
        float minutesLeft = MinutesLeft(expirationTime);
        int minutesRemaining = Mathf.FloorToInt(minutesLeft);
        string minutesString = "";
        if(minutesLeft < 10.0f)
        {
            minutesString = "0";
        }
        minutesString = minutesString + minutesRemaining.ToString();
        minutesLeft -= minutesRemaining;

        int secondsRemaining = Mathf.FloorToInt(minutesLeft * 60f);
        string secondsString = "";
        if(secondsRemaining < 10)
        {
            secondsString = "0";
        }
        secondsString = secondsString + secondsRemaining.ToString();

        return minutesString + ":" + secondsString;
    }
}
