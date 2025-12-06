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
    private static int FillInitialMatchEntryBytes(MatchCharacter mc, byte[] toFill, byte matchType, byte sceneID, byte teamID, byte matchID, int port){
        toFill[0] = Pregame_Send.MatchEntryPacket;
        toFill[1] = matchType;
        toFill[2] = sceneID;
        toFill[3] = mc.GetIDinMatch();
        toFill[4] = teamID;
        toFill[5] = matchID;
        System.arraycopy(ByteUtils.IntToByteArray(port), 0, toFill, 6, 4);
        System.arraycopy(ByteUtils.FloatToByteArray(mc.PC().GetMaxHP()), 0, toFill, 10, 4);
        System.arraycopy(ByteUtils.FloatToByteArray(mc.PC().GetMaxMana()), 0, toFill, 14, 4);
        toFill[18] = mc.PC().GetMaxStamina();
        return 19; // the next index;
    }
    public static byte[] DeathMatchEntryPacket(byte sceneID, byte teamID, MatchCharacter mc, int port, byte matchID,
                                               byte matchType){
        DeathMatch dm = (DeathMatch)MatchManager.GetMatch(matchID);
        byte[] poolData = dm.GetPoolManager().GetPoolBiasData();
        byte[] shrineData = dm.ReportAllShrineHealth();
        byte[] toEncrypt = new byte[19 + shrineData.length + poolData.length];
        int nextIndex = FillInitialMatchEntryBytes(mc, toEncrypt, matchType, sceneID, teamID, matchID, port);
        System.arraycopy(shrineData, 0, toEncrypt, nextIndex, shrineData.length);
        nextIndex += shrineData.length;
        System.arraycopy(poolData, 0, toEncrypt, nextIndex, poolData.length);
        return Cryptographer.Encrypt(toEncrypt);
    }

    public static byte[] CTFEntryPacket(byte sceneID, MatchCharacter mc, byte teamID, int port, byte matchID,
                                        byte matchType){
        CaptureTheFlag ctf = (CaptureTheFlag) MatchManager.GetMatch(matchID);
        byte[] flagBytes = ctf.FlagsStatus();
        byte[] scores = ctf.GetScores();
        byte[] poolBytes = ctf.GetPoolManager().GetPoolBiasData();
        byte[] toEncrypt = new byte[19 + scores.length + 1 + flagBytes.length + poolBytes.length];
        int nextIndex = FillInitialMatchEntryBytes(mc, toEncrypt, matchType, sceneID, teamID, matchID, port);
        System.arraycopy(scores, 0, toEncrypt, nextIndex, 3);
        nextIndex += 3;
        toEncrypt[nextIndex] = (byte)flagBytes.length;
        nextIndex+=1;
        System.arraycopy(flagBytes, 0, toEncrypt, nextIndex, flagBytes.length);
        nextIndex += flagBytes.length;
        System.arraycopy(poolBytes, 0, toEncrypt, nextIndex, poolBytes.length);
        return Cryptographer.Encrypt(toEncrypt);
    }

    public static byte[] FFAEntryPacket(byte sceneID, MatchCharacter mc, int port, byte matchType, byte matchID){
        byte[] toEncrypt = new byte[19];
        FillInitialMatchEntryBytes(mc, toEncrypt, matchType, sceneID, MatchTeam.Neutral, matchID, port);
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

    public static byte[] PostureChangePacket(byte[] decrypted){
        decrypted[0] = InGame_Send.PostureChange;
        return Cryptographer.Encrypt(decrypted);
    }
    public static byte[] PlayerTapped(byte playerID){
        return Cryptographer.Encrypt(new byte[]{InGame_Send.PlayerTapped, playerID});
    }
    public static byte[] PlayerRevivedPacket(byte revivedID, byte reviverID, float hp){
        byte[] toEncrypt = new byte[7];
        toEncrypt[0] = InGame_Send.PlayerRevived;
        toEncrypt[1] = revivedID;
        toEncrypt[2] = reviverID;
        System.arraycopy(ByteUtils.FloatToByteArray(hp), 0, toEncrypt, 3, 4);
        return Cryptographer.Encrypt(toEncrypt);
    }
    public static byte[] PlayerMovedPacket(byte[] decrypted){
        decrypted[0] = InGame_Send.PlayerMoved;
        return Cryptographer.Encrypt(decrypted);
    }

    public static byte[] ObjectStatusBytes(ArrayList<Byte> status){
        byte[] toEncrypt = new byte[1 + status.size()];
        toEncrypt[0] = InGame_Send.ObjectData;
        for(int i = 0; i < status.size(); i++){
            toEncrypt[i+1] = status.get(i);
        }
        return Cryptographer.Encrypt(toEncrypt);
    }

    public static byte[] TeamChatPacket(byte[] messageBytes, byte sender, byte teamID){
        int messageLength = messageBytes.length;
        byte[] messageLengthBytes = ByteUtils.IntToByteArray(messageLength);
        byte[] toEncrypt = new byte[1 + 1 + 1 + 4 + messageLength];
        toEncrypt[0] = InGame_Send.TeamMessage;
        toEncrypt[1] = sender;
        toEncrypt[2] = teamID;
        System.arraycopy(messageLengthBytes, 0, toEncrypt, 3, 4);
        System.arraycopy(messageBytes, 0, toEncrypt, 7, messageLength);
        return Cryptographer.Encrypt(toEncrypt);
    }

    public static byte[] RemovedFromMatchPacket(byte reasonCode){
        return Cryptographer.Encrypt(new byte[]{InGame_Send.RemovedFromMatch, reasonCode});
    }

    public static byte[] HPandManaUpdatePacket(float health, float mana){
        byte[] toEncrypt = new byte[9];
        toEncrypt[0] = InGame_Send.HPandManaUpdate;
        System.arraycopy(ByteUtils.FloatToByteArray(health),0,toEncrypt,1,4);
        System.arraycopy(ByteUtils.FloatToByteArray(mana),0,toEncrypt,5,4);
        return Cryptographer.Encrypt(toEncrypt);
    }

    public static byte[] HPorManaorLeyUpdatePacket(byte packetID, float value){
        byte[] toEncrypt = new byte[5];
        toEncrypt[0] = packetID;
        System.arraycopy(ByteUtils.FloatToByteArray(value),0,toEncrypt,1,4);
        return Cryptographer.Encrypt(toEncrypt);
    }

    public static byte[] SpellCastPacket(byte[] toEncrypt){
        toEncrypt[0] = InGame_Send.SpellCast;
        return Cryptographer.Encrypt(toEncrypt);
    }
    public static byte[] PlayerDamagedPacket(byte playerID, byte damageSourceID, float newHP){
        return null;
    }

    public static byte[] PlayerKilledPacket(byte playerKilled, byte killerID){
        return Cryptographer.Encrypt(new byte[]{InGame_Send.PlayerKilled, playerKilled, killerID});
    }

    public static byte[] FlagTakenPacket(byte flagID, byte playerID){
        return Cryptographer.Encrypt(new byte[]{InGame_Send.FlagTaken, flagID, playerID});
    }

    public static byte[] FlagDroppedPacket(byte playerID, byte[] flagBytes, byte killer){
        byte[] toEncrypt = new byte[1 + 1 + 1 + flagBytes.length];
        toEncrypt[0] = InGame_Send.FlagDropped;
        toEncrypt[1] = playerID;
        toEncrypt[2] = killer;
        System.arraycopy(flagBytes, 0, toEncrypt, 3, flagBytes.length);
        return Cryptographer.Encrypt(toEncrypt);
    }

    public static byte[] FlagReturnedPacket(byte flagReturned){
        return Cryptographer.Encrypt(new byte[]{InGame_Send.FlagReturned, flagReturned});
    }

    public static byte[] FlagCapturedPacket(byte capturingTeam, byte flagCaptured, byte capturedBy, byte scoreCapturer,
                                            byte scoreCaptured)
    {
        return Cryptographer.Encrypt(new byte[]{InGame_Send.FlagCaptured, capturingTeam, flagCaptured, capturedBy, scoreCapturer, scoreCaptured});
    }

    public static byte[] AllShrineHealthPacket(byte chaos, byte balance, byte order){
        return Cryptographer.Encrypt(new byte[]{InGame_Send.AllShrineHealth, chaos, balance, order});
    }

    public static byte[] ShrineAdjustmentPacket(byte health, byte shrineID, byte adjusterID){
        return Cryptographer.Encrypt(new byte[]{InGame_Send.ShrineAdjusted, shrineID, health, adjusterID});
    }
    public static byte[] ObjectStateChangePacket(byte objectID, byte state){
        return Cryptographer.Encrypt(new byte[]{InGame_Send.ObjectStateChange, objectID, state});
    }

    public static byte[] ShrineFailurePacket(byte shrineID){
        return Cryptographer.Encrypt(new byte[]{InGame_Send.ShrineFailure, shrineID});
    }

    public static byte[] PoolBiasPacket(byte poolID, byte bias, byte teamID, byte biaserID){
        return Cryptographer.Encrypt(new byte[]{InGame_Send.PoolBiased, poolID, bias, teamID, biaserID});
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
        return Cryptographer.Encrypt(new byte[]{InGame_Send.PlayerLeftMatch, 1, playerID});
    }

    public static byte[] PlayerDataPacket(byte[] INCTLA){
        byte[] toEncrypt = new byte[INCTLA.length + 1];
        toEncrypt[0] = InGame_Send.PlayerData;
        System.arraycopy(INCTLA, 0, toEncrypt, 1, INCTLA.length);
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
