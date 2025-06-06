import java.nio.charset.StandardCharsets;
import java.util.ArrayList;
import java.util.Collection;

public class Packets {

    private static final byte[] AccountCreated_Bytes = new byte[]{Pregame_OpCode_Send.AccountCreated};
    private static final byte[] CreationFailed_Bytes = new byte[]{Pregame_OpCode_Send.CreationFailed};
    private static final byte[] AccountExistsPacket_Bytes = new byte[]{Pregame_OpCode_Send.AccountAlreadyExists};
    private static final byte[] AlreadyLoggedInPacket_Bytes = new byte[]{Pregame_OpCode_Send.AlreadyLoggedIn};
    private static final byte[] LoginFailedPacket_Bytes = new byte[]{Pregame_OpCode_Send.LogInFailed};
    private static final byte[] ProhibitedLanguagePacket_Bytes = new byte[]{Pregame_OpCode_Send.ProhibitedLanguage};
    private static final byte[] CharacterExistsPacket_Bytes = new byte[]{Pregame_OpCode_Send.CharacterExists};
    private static final byte[] InactivityDisconnect_Bytes = new byte[]{Pregame_OpCode_Send.InactivityDisconnect};
    private static final byte[] MatchAlreadyCreated_Bytes = new byte[]{Pregame_OpCode_Send.MatchAlreadyCreated};
    private static final byte[] MatchLimitReached_Bytes = new byte[]{Pregame_OpCode_Send.MatchLimitReached};
    private static final byte[] MatchStillHasPlayers_Bytes = new byte[]{Pregame_OpCode_Send.MatchStillHasPlayers};
    private static final byte[] BannedForCheating_Bytes = new byte[]{Pregame_OpCode_Send.BannedForCheating};
    private static final byte[] BannedForBehavior_Bytes = new byte[]{Pregame_OpCode_Send.BannedForBehavior};
    private static final byte[] MatchIsFull_Bytes = new byte[]{Pregame_OpCode_Send.MatchIsFullPacket};

    public static byte[] BannedForCheatingPacket() {return Cryptographer.Encrypt(BannedForCheating_Bytes);}
    public static byte[] BannedForBehaviorPacket() {return Cryptographer.Encrypt(BannedForBehavior_Bytes);}

    public static byte[] MatchStillHasPlayersPacket() {return Cryptographer.Encrypt(MatchStillHasPlayers_Bytes);}
    public static byte[] InactivityDisconnectPacket() { return Cryptographer.Encrypt(InactivityDisconnect_Bytes);}
    public static byte[] LoginFailedPacket(){
        return Cryptographer.Encrypt(LoginFailedPacket_Bytes);
    }

    public static byte[] AccountExistsPacket(){
        return Cryptographer.Encrypt(AccountExistsPacket_Bytes);
    }

    public static byte[] AccountCreatedPacket(){
        return Cryptographer.Encrypt(AccountCreated_Bytes);
    }

    public static byte[] CreationFailedPacket(){
        return Cryptographer.Encrypt(CreationFailed_Bytes);
    }

    public static byte[] AlreadyLoggedInPacket(){
        return Cryptographer.Encrypt(AlreadyLoggedInPacket_Bytes);
    }

    public static byte[] ProhibitedLanguagePacket(){ return Cryptographer.Encrypt(ProhibitedLanguagePacket_Bytes);}

    public static byte[] CharacterExistsPacket(){ return Cryptographer.Encrypt(CharacterExistsPacket_Bytes);}

    public static byte[] MatchAlreadyCreatedPacket() {return Cryptographer.Encrypt(MatchAlreadyCreated_Bytes);}

    public static byte[] MatchLimitReachedPacket() {return Cryptographer.Encrypt(MatchLimitReached_Bytes);}

    public static byte[] LevelListPacket(){return Cryptographer.Encrypt(GameServer.LevelList());}

    public static byte[] MatchIsFullPacket(){
        return Cryptographer.Encrypt(MatchIsFull_Bytes);
    }

    public static byte[] MatchEntryPacket(byte sceneID, byte teamID, byte playerID, int port){
        byte[] toEncrypt = new byte[8];
        toEncrypt[0] = Pregame_OpCode_Send.MatchEntryPacket;
        toEncrypt[1] = sceneID;
        toEncrypt[2] = teamID;
        toEncrypt[3] = playerID;
        System.arraycopy(ByteUtils.IntToByteArray(port), 0, toEncrypt, 4, 4);
        return Cryptographer.Encrypt(toEncrypt);
    }

    public static byte[] NameCheckResults(byte isUsed){
        byte[] toEncrypt = new byte[2];
        toEncrypt[0] = Pregame_OpCode_Send.NameCheckResult;
        toEncrypt[1] = isUsed;
        return Cryptographer.Encrypt(toEncrypt);
    }
    public static byte[] MatchDetailsPacket(Match match){
        return Cryptographer.Encrypt(match.PlayersInMatch(Pregame_OpCode_Send.MatchDetails));
    }

    public static byte[] CharacterDeletedPacket(int characterID) {
        byte[] toEncrypt = new byte[5];
        toEncrypt[0] = Pregame_OpCode_Send.CharacterDeleted;
        byte[] idBytes = ByteUtils.IntToByteArray(characterID);
        System.arraycopy(idBytes,0, toEncrypt, 1, 4);
        return Cryptographer.Encrypt(toEncrypt);
    }

    public static byte[] MatchDataPacket(Collection<Match> matches){
        ArrayList<byte[]> matchData = new ArrayList<>();
        int totalSize = 2;
        byte matchCount = 0;
        for(Match match : matches){
            byte[] matchBytes = match.ToByteArray();
            totalSize += matchBytes.length;
            matchData.add(matchBytes);
            matchCount++;
        }
        byte[] toSend = new byte[totalSize];
        toSend[0] = Pregame_OpCode_Send.MatchData;
        toSend[1] = matchCount;
        int index = 2;
        for (byte[] matchBytes : matchData){
            int length = matchBytes.length;
            System.arraycopy(matchBytes, 0, toSend, index, length);
            index += length;
        }
        return Cryptographer.Encrypt(toSend);
    }

    public static byte[] LoginSucceededPacket(int accountID){
        byte[] characterBytes = Database.GetCharactersOfAccount(accountID); // this includes the name length as the first byte
        //Main.LogMessage("Character bytes retrieved: " + characterBytes.length);
        byte[] toSend;
        if(characterBytes.length > 0){
            toSend = new byte[1 + 4 + 8 + characterBytes.length];
            System.arraycopy(characterBytes, 0, toSend, 13, characterBytes.length);
        }
        else{
            toSend = new byte[1 + 4 + 8];
        }
        toSend[0] = Pregame_OpCode_Send.LogInSucceeded;
        byte[] accountBytes = ByteUtils.IntToByteArray(accountID);
        byte[] timeBytes = ByteUtils.LongToByteArray(System.currentTimeMillis());
        System.arraycopy(accountBytes, 0, toSend, 1, 4);
        System.arraycopy(timeBytes, 0, toSend, 5, 8);
        return Cryptographer.Encrypt(toSend);
    }

    public static byte[] CharacterCreatedPacket(int characterID, byte classCode, String charname, byte[] stats, byte[] appearance){
        byte[] nameBytes = charname.getBytes(StandardCharsets.UTF_8);
        byte nameLength = (byte)nameBytes.length;
        byte[] toReturn = new byte[1 + 1 + 4 + 5 + 6 + 1 + nameLength];
        toReturn[0] = Pregame_OpCode_Send.CharacterCreated;
        toReturn[1] = classCode;
        byte[] idBytes = ByteUtils.IntToByteArray(characterID);
        System.arraycopy(idBytes,0,toReturn, 2, 4);
        System.arraycopy(appearance, 0, toReturn, 6, 5);
        System.arraycopy(stats, 0, toReturn, 11, 6);
        toReturn[17] = nameLength;
        System.arraycopy(nameBytes,0,toReturn,18,nameLength);
        return Cryptographer.Encrypt(toReturn);
    }

    public static byte[] RemovedFromServerPacket(byte reasonCode){
        return Cryptographer.Encrypt(new byte[]{Pregame_OpCode_Send.RemovedFromServer, reasonCode});
    }

    public static ArrayList<byte[]> ExtractBytes(byte[] decrypted, int firstIndex){
        ArrayList<byte[]> toReturn = new ArrayList<>();
        int index = firstIndex;
        byte[] lengths = new byte[firstIndex - 1];
        System.arraycopy(decrypted, 1,lengths,0,lengths.length);
        for(int i = 0; i < lengths.length; i++){
            byte[] toAdd = new byte[lengths[i]];
            System.arraycopy(decrypted, index, toAdd, 0, lengths[i]);
            index += lengths[i];
            toReturn.add(toAdd);
        }
        return toReturn;
    }



    public static byte[] ExtractBytes(byte[] decrypted, int index, int length){
        byte[] toReturn = new byte[length];
        System.arraycopy(decrypted, index, toReturn, 0, length);
        return toReturn;
    }

}
