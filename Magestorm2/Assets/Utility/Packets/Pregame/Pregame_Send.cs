using System.Runtime.CompilerServices;
using Unity.Collections;

public static class Pregame_Send
{
    public const byte LogIn = 1,
                        CreateAccount = 2,
                        CreateCharacter = 3,
                        LogOut = 4,
                        DeleteCharacter = 5,
                        SubscribeToMatches = 6,
                        UnsubscribeFromMatches = 7,
                        CreateMatch = 8,
                        DeleteMatch = 9,
                        RequestLevelsList = 10,
                        RequestMatchDetails = 11,
                        NameCheck = 12,
                        UpdateAppearance = 13,
                        JoinMatch = 14,
                        RequestMatchList = 15;
}
        