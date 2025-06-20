public enum OpCode_Receive : byte
{
    //{ InGame
        PlayersInMatch = 0,
        PlayerData = 1,
        ShrineHealth = 2,
        ObjectData = 3,
        ObjectStateChange = 4,
        AllShrineHealth = 5,
    //}
    //{ Pregame
        LogInSucceeded = 1,
        LogInFailed = 2,
        AccountCreated = 3,
        CreationFailed = 4,
        AccountAlreadyExists = 5,
        ProhibitedLanguage = 6,
        AlreadyLoggedIn = 7,
        RemovedFromServer = 8,
        CharacterExists = 9,
        CharacterCreated = 10,
        InactivityDisconnect = 11,
        CharacterDeleted = 12,
        MatchData = 13,
        MatchAlreadyCreated = 14,
        MatchLimitReached = 15,
        MatchStillHasPlayers = 16,
        LevelsList = 17,
        BannedForCheating = 18,
        BannedForBehavior = 19,
        MatchDetails = 20,
        NameCheckResult = 21,
        MatchEntryPacket = 22,
        MatchIsFullPacket = 23
    //}
}
