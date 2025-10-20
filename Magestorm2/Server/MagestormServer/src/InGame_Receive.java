public class InGame_Receive {
    public static final byte JoinedMatch = 1;
    public static final byte RequestPlayerData = 2;

    public static final byte ChangedObjectState = 3;
    public static final byte FetchShrineHealth = 4;

    public static final byte DirectMessage = 5;
    public static final byte TeamMessage = 6;
    public static final byte BroadcastMessage = 7;
    public static final byte LeaveMatch = 8;
    public static final byte InactivityCheckResponse = 9;
    public static final byte BiasPool = 10;
    public static final byte QuitGame = 11;
    public static final byte AdjustShrineHealth = 12;
    public static final byte FlagCaptured = 13;
    public static final byte FlagReturned = 14;
    public static final byte FlagTaken = 15;

    public static final byte HitPlayer = 17;
    public static final byte CastSpell = 18;
}
