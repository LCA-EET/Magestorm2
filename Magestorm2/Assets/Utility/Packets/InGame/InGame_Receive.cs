using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class InGame_Receive
{
    public const byte PlayersInMatch = 0,
                        PlayerData = 1,
                        ShrineHealth = 2,
                        ObjectData = 3,
                        ObjectStateChange = 4,
                        AllShrineHealth = 5,

                        DirectMessage = 6,
                        BroadcastMessage = 7,
                        TeamMessage = 8,

                        ProhibitedLanguage = 9,
                        PlayerLeftMatch = 10,
                        MatchEnded = 11,
                        TimedObjectExpired = 12,
                        RemovedFromMatch = 13,
                        InactivityWarning = 14,
                        PlayerJoinedMatch = 15,
                        PoolBiased = 16,
                        PoolBiasFailure = 17,
                        ShrineAdjusted = 18,
                        ShrineFailure = 19,
                        FlagCaptured = 20,
                        FlagReturned = 21,
                        FlagDropped = 22,
                        PostureChange = 23,
                        Cast = 24,
                        PlayerKilled = 25,
                        HMLUpdate = 26,
                        FlagTaken = 27,
                        UpdateLocation = 28,
                        PlayerMoved = 29,
                        HPandManaUpdate = 30,
                        HPUpdate = 31,
                        ManaUpdate = 32,
                        LeyUpdate = 33,
                        PlayerRevived = 34,
                        PlayerTapped = 35,

                        ApplyEffect = 37,
                        InactivityDisconnect = 39,
                        HitNotification = 40,
                        SendToValhalla = 41,
                        EffectsCancellation = 42,
                        WallExpired = 43,
                        WallRequestResponse = 44,
                        ExperienceUpdate = 45,
                        MatchScores = 46,
                        SpellResisted = 47,
                        SpawnVFXonPlayer = 48,
                        SigilExpired = 49,
                        SigilRequestResponse = 50,
                        ApplyForce = 51,
                        AllPlayerData = 52;
}
