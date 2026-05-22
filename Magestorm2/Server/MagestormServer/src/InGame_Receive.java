public class InGame_Receive {
    public static final byte JoinedMatch = 1;
    public static final byte RequestPlayerData = 2;

    public static final byte ChangedObjectState = 3;
    public static final byte FetchShrineHealth = 4;

    public static final byte LeaveMatch = 5;

    public static final byte DirectMessage = 6;
    public static final byte BroadcastMessage = 7;
    public static final byte TeamMessage = 8;



    public static final byte InactivityCheckResponse = 9;
    public static final byte BiasPool = 10;
    public static final byte QuitGame = 11;
    public static final byte AdjustShrineHealth = 12;
    public static final byte FlagCaptured = 13;
    public static final byte FlagReturned = 14;
    public static final byte FlagTaken = 15;
    public static final byte HitPlayer = 16;
    //public static final byte CastSpell = 17;
    public static final byte ObjectStatus = 18;

    public static final byte FetchPlayer = 20;
    public static final byte LeyUpdate = 21;
    public static final byte Tap = 22;
    public static final byte PostureChange = 23;
    public static final byte Cast = 24;
    public static final byte ReportHit = 28;
    public static final byte PlayerMoved = 29;
    public static final byte Devour = 30;
    public static final byte ReportSplash = 31;
    public static final byte WallHit = 32;
    public static final byte RequestWallsAndSigils = 33;
    public static final byte LeaderboardRequest = 34;
    public static final byte UpdateSlotting = 35;
    public static final byte ReportResistableHit = 36;
    public static final byte ReportHitByWall = 37;
    public static final byte TriggeredSigil = 38;
}
