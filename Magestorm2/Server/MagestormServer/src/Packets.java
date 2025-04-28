import java.nio.ByteBuffer;
import java.nio.charset.StandardCharsets;
import java.util.ArrayList;
import java.util.Collection;

public class Packets {

    private static final byte[] AccountCreated_Bytes = new byte[]{OpCode_Send.AccountCreated};
    private static final byte[] CreationFailed_Bytes = new byte[]{OpCode_Send.CreationFailed};
    private static final byte[] AccountExistsPacket_Bytes = new byte[]{OpCode_Send.AccountAlreadyExists};
    private static final byte[] AlreadyLoggedInPacket_Bytes = new byte[]{OpCode_Send.AlreadyLoggedIn};
    private static final byte[] LoginFailedPacket_Bytes = new byte[]{OpCode_Send.LogInFailed};
    private static final byte[] ProhibitedLanguagePacket_Bytes = new byte[]{OpCode_Send.ProhibitedLanguage};
    private static final byte[] CharacterExistsPacket_Bytes = new byte[]{OpCode_Send.CharacterExists};
    private static final byte[] InactivityDisconnect_Bytes = new byte[]{OpCode_Send.InactivityDisconnect};
    private static final byte[] MatchAlreadyCreated_Bytes = new byte[]{OpCode_Send.MatchAlreadyCreated};
    private static final byte[] MatchLimitReached_Bytes = new byte[]{OpCode_Send.MatchLimitReached};
    private static final byte[] MatchStillHasPlayers_Bytes = new byte[]{OpCode_Send.MatchStillHasPlayers};
    private static final byte[] BannedForCheating_Bytes = new byte[]{OpCode_Send.BannedForCheating};
    private static final byte[] BannedForBehavior_Bytes = new byte[]{OpCode_Send.BannedForBehavior};

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

    public static byte[] CharacterDeletedPacket(int characterID) {
        byte[] toEncrypt = new byte[5];
        toEncrypt[0] = OpCode_Send.CharacterDeleted;
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
        toSend[0] = OpCode_Send.MatchData;
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
        byte[] characterBytes = Database.GetCharactersForAccount(accountID); // this includes the name length as the first byte
        Main.LogMessage("Character bytes retrieved: " + characterBytes.length);
        byte[] toSend;
        if(characterBytes.length > 0){
            toSend = new byte[1 + 4 + 8 + characterBytes.length];
            System.arraycopy(characterBytes, 0, toSend, 13, characterBytes.length);
        }
        else{
            toSend = new byte[1 + 4 + 8];
        }
        toSend[0] = OpCode_Send.LogInSucceeded;
        byte[] accountBytes = ByteUtils.IntToByteArray(accountID);
        byte[] timeBytes = ByteUtils.LongToByteArray(System.currentTimeMillis());
        System.arraycopy(accountBytes, 0, toSend, 1, 4);
        System.arraycopy(timeBytes, 0, toSend, 5, 8);
        return Cryptographer.Encrypt(toSend);
    }

    public static byte[] CharacterCreatedPacket(int characterID, byte classCode, String charname, byte[] stats){
        byte[] nameBytes = charname.getBytes(StandardCharsets.UTF_8);
        byte nameLength = (byte)nameBytes.length;
        byte[] toReturn = new byte[1 + 1 + 1 + 4 + 6 + nameLength];
        toReturn[0] = OpCode_Send.CharacterCreated;
        toReturn[1] = classCode;
        toReturn[2] = nameLength;
        byte[] idBytes = ByteUtils.IntToByteArray(characterID);
        System.arraycopy(idBytes,0,toReturn, 3, 4);
        System.arraycopy(stats, 0, toReturn, 7, 6);
        System.arraycopy(nameBytes,0,toReturn,13,nameLength);
        return Cryptographer.Encrypt(toReturn);
    }

    public static byte[] RemovedFromServerPacket(byte reasonCode){
        return Cryptographer.Encrypt(new byte[]{OpCode_Send.RemovedFromServer, reasonCode});
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
