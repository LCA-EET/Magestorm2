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
                        TeamMessage = 7,
                        BroadcastMessage = 8,
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
                        ShrineFailure = 19;
}
