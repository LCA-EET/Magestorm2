using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class InGame_Send
{
    public const byte JoinedMatch = 1,
                        RequestPlayerData = 2,
                        ChangedObjectState = 3,
                        FetchShrineHealth = 4,
                        DirectMessage = 5,
                        TeamMessage = 6,
                        BroadcastMessage = 7,
                        LeaveMatch = 8,
                        InactivityCheckResponse = 9,
                        BiasPool = 10,
                        QuitGame = 11,
                        AdjustShrine = 12,
                        FlagCaptured = 13,
                        FlagReturned = 14,
                        FlagTaken = 15,
                        HitPlayer = 16,
                        CastSpell = 17;
}
