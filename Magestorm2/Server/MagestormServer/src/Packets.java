import java.nio.charset.StandardCharsets;
import java.util.ArrayList;
import java.util.Collection;

public class Packets {

    private static final byte[] AccountCreated_Bytes = new byte[]{Pregame_Send.AccountCreated};
    private static final byte[] CreationFailed_Bytes = new byte[]{Pregame_Send.CreationFailed};
    private static final byte[] AccountExistsPacket_Bytes = new byte[]{Pregame_Send.AccountAlreadyExists};
    private static final byte[] AlreadyLoggedInPacket_Bytes = new byte[]{Pregame_Send.AlreadyLoggedIn};
    private static final byte[] LoginFailedPacket_Bytes = new byte[]{Pregame_Send.LogInFailed};
    private static final byte[] CharacterExistsPacket_Bytes = new byte[]{Pregame_Send.CharacterExists};
    private static final byte[] InactivityDisconnect_Bytes = new byte[]{Pregame_Send.InactivityDisconnect};
    private static final byte[] MatchAlreadyCreated_Bytes = new byte[]{Pregame_Send.MatchAlreadyCreated};
    private static final byte[] MatchLimitReached_Bytes = new byte[]{Pregame_Send.MatchLimitReached};
    private static final byte[] MatchStillHasPlayers_Bytes = new byte[]{Pregame_Send.MatchStillHasPlayers};
    private static final byte[] BannedForCheating_Bytes = new byte[]{Pregame_Send.BannedForCheating};
    private static final byte[] BannedForBehavior_Bytes = new byte[]{Pregame_Send.BannedForBehavior};
    private static final byte[] MatchIsFull_Bytes = new byte[]{Pregame_Send.MatchIsFullPacket};
    private static final byte[] AcknowledgeSubscription_Bytes = new byte[]{Pregame_Send.AcknowledgeSubscription};



    public static byte[] AcknowledgeSubscriptionPacket(){return Cryptographer.Encrypt(AcknowledgeSubscription_Bytes);}
    public static byte[] MessagePacket(byte[] decrypted, byte opCode){
        decrypted[0] = opCode;
        return Cryptographer.Encrypt(decrypted);
    }
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

    public static byte[] ProhibitedLanguagePacket(byte opCode){ return Cryptographer.Encrypt(new byte[]{opCode});}

    public static byte[] CharacterExistsPacket(){ return Cryptographer.Encrypt(CharacterExistsPacket_Bytes);}

    public static byte[] MatchAlreadyCreatedPacket() {return Cryptographer.Encrypt(MatchAlreadyCreated_Bytes);}

    public static byte[] MatchLimitReachedPacket() {return Cryptographer.Encrypt(MatchLimitReached_Bytes);}

    public static byte[] LevelListPacket(){return Cryptographer.Encrypt(GameServer.LevelList());}

    public static byte[] MatchIsFullPacket(){
        return Cryptographer.Encrypt(MatchIsFull_Bytes);
    }

    public static byte[] AllShrineHealthPacket(byte chaos, byte balance, byte order){
        byte[] toEncrypt = new byte[4];
        toEncrypt[0] = InGame_Send.AllShrineHealth;
        toEncrypt[1] = chaos;
        toEncrypt[2] = balance;
        toEncrypt[3] = order;
        return Cryptographer.Encrypt(toEncrypt);
    }

    public static byte[] ShrineHealthPacket(byte health, byte teamID){
        byte[] toEncrypt = new byte[3];
        toEncrypt[0] = InGame_Send.ShrineHealth;
        toEncrypt[1] = teamID;
        toEncrypt[2] = health;
        return Cryptographer.Encrypt(toEncrypt);
    }
    public static byte[] ObjectStateChangePacket(byte objectID, byte state){
        byte[] toEncrypt = new byte[3];
        toEncrypt[0] = InGame_Send.ObjectStateChange;
        toEncrypt[1] = objectID;
        toEncrypt[2] = state;
        return Cryptographer.Encrypt(toEncrypt);
    }

    public static byte[] DeathMatchEntryPacket(byte sceneID, byte teamID, byte playerID, int port, byte matchID, byte matchType){
        byte[] poolData = ((DeathMatch)MatchManager.GetMatch(matchID)).GetPoolManager().GetPoolBiasData();
        byte[] toEncrypt = new byte[9 + poolData.length];
        toEncrypt[0] = Pregame_Send.MatchEntryPacket;
        toEncrypt[1] = sceneID;
        toEncrypt[2] = teamID;
        toEncrypt[3] = playerID;
        toEncrypt[4] = matchType;
        System.arraycopy(ByteUtils.IntToByteArray(port), 0, toEncrypt, 5, 4);
        System.arraycopy(poolData, 0, toEncrypt, 9, poolData.length);
        return Cryptographer.Encrypt(toEncrypt);
    }

    public static byte[] NameCheckResults(byte isUsed){
        byte[] toEncrypt = new byte[2];
        toEncrypt[0] = Pregame_Send.NameCheckResult;
        toEncrypt[1] = isUsed;
        return Cryptographer.Encrypt(toEncrypt);
    }
    public static byte[] MatchDetailsPacket(Match match){
        return Cryptographer.Encrypt(match.PlayersInMatch(Pregame_Send.MatchDetails));
    }

    public static byte[] CharacterDeletedPacket(int characterID) {
        byte[] toEncrypt = new byte[5];
        toEncrypt[0] = Pregame_Send.CharacterDeleted;
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
        toSend[0] = Pregame_Send.MatchData;
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
        toSend[0] = Pregame_Send.LogInSucceeded;
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
        toReturn[0] = Pregame_Send.CharacterCreated;
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
        return Cryptographer.Encrypt(new byte[]{Pregame_Send.RemovedFromServer, reasonCode});
    }



    /////////////////////// IN-GAME PACKETS ////////////////////////
    private static final byte[] MatchEnded_Bytes = new byte[]{InGame_Send.MatchEnded};
    private static final byte[] InactivityWarning_Bytes = new byte[]{InGame_Send.InactivityWarning};
    private static final byte[] PoolBiasFailure_Bytes = new byte[]{InGame_Send.PoolBiasFailure};

    public static byte[] MatchEndedPacket(){return Cryptographer.Encrypt(MatchEnded_Bytes);}
    public static byte[] InactivityWarningPacket(){ return Cryptographer.Encrypt(InactivityWarning_Bytes);}
    public static byte[] PoolBiasFailurePacket(){ return Cryptographer.Encrypt(PoolBiasFailure_Bytes);}

    public static byte[] PoolBiasPacket(byte poolID, byte bias, byte teamID, byte biaserID){
        byte[] toEncrypt = new byte[5];
        toEncrypt[0] = InGame_Send.PoolBiased;
        toEncrypt[1] = poolID;
        toEncrypt[2] = bias;
        toEncrypt[3] = teamID;
        toEncrypt[4] = biaserID;
        return Cryptographer.Encrypt(toEncrypt);
    }

    public static byte[] PlayersLeftMatchPacket(ArrayList<MatchCharacter> departed){
        byte[] toEncrypt = new byte[2 + departed.size()];
        toEncrypt[0] = InGame_Send.PlayerLeftMatch;
        toEncrypt[1] = (byte)departed.size();
        int index = 2;
        for(MatchCharacter mc : departed){
            toEncrypt[index] = mc.GetIDinMatch();
            index++;
        }
        return Cryptographer.Encrypt(toEncrypt);
    }

    public static byte[] PlayerLeftMatchPacket(byte playerID){
        byte[] toEncrypt = new byte[3];
        toEncrypt[0] = InGame_Send.PlayerLeftMatch;
        toEncrypt[1] = 1;
        toEncrypt[2] = playerID;
        //toEncrypt[3] = teamID;
        return Cryptographer.Encrypt(toEncrypt);
    }

    public static byte[] PlayerJoinedMatchPacket(byte[] INLCTA){
        byte[] toEncrypt = new byte[1 + INLCTA.length];
        toEncrypt[0] = InGame_Send.PlayerJoinedMatch;
        System.arraycopy(INLCTA, 0, toEncrypt, 1, INLCTA.length);
        return Cryptographer.Encrypt(toEncrypt);
    }
    public static byte[] PlayerDataPacket(byte[] dataForPlayer){
        byte[] toEncrypt = new byte[dataForPlayer.length + 1];
        toEncrypt[0] = InGame_Send.PlayerData;
        System.arraycopy(dataForPlayer, 0, toEncrypt, 1, dataForPlayer.length);
        return Cryptographer.Encrypt(toEncrypt);
    }

    public static byte[] TimedObjectExpirationPacket(ArrayList<Byte> expired){
        byte[] toEncrypt = new byte[1 + expired.size()];
        toEncrypt[0] = InGame_Send.TimedObjectExpired;
        Byte[] expiredArray = expired.toArray(new Byte[0]);
        System.arraycopy(expiredArray, 0, toEncrypt, 1, expiredArray.length );
        return Cryptographer.Encrypt(toEncrypt);
    }

    /////////////////////// SHARED //////////////////////

    public static byte[] ExtractBytes(byte[] decrypted, int index, int length){
        byte[] toReturn = new byte[length];
        System.arraycopy(decrypted, index, toReturn, 0, length);
        return toReturn;
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
}
