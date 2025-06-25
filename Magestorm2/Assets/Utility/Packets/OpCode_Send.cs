using System.Runtime.CompilerServices;
using Unity.Collections;

public static class OpCode_Send
{
    //{ InGame
    public const byte JoinedMatch = 1;
    public const byte RequestPlayerData = 2;
    public const byte ChangedObjectState = 3;
    public const byte FetchShrineHealth = 4;
    public const byte DirectMessage = 5;
    public const byte TeamMessage = 6;
    public const byte BroadcastMessage = 7;
    //}

    //{ Pregame
    public const byte LogIn = 1;
        public const byte CreateAccount = 2;
        public const byte CreateCharacter = 3;
        public const byte LogOut = 4;
        public const byte DeleteCharacter = 5;
        public const byte SubscribeToMatches = 6;
        public const byte UnsubscribeFromMatches = 7;
        public const byte CreateMatch = 8;
        public const byte DeleteMatch = 9;
        public const byte RequestLevelsList = 10;
        public const byte RequestMatchDetails = 11;
        public const byte NameCheck = 12;
        public const byte UpdateAppearance = 13;
        public const byte JoinMatch = 14;
    //}


}
        