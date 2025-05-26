using System.Runtime.CompilerServices;

public static class OpCode_Send
{
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
}
