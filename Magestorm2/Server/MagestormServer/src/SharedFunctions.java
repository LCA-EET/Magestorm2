import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;
import java.util.ArrayList;
import java.util.Collection;
import java.util.Random;

public class SharedFunctions {


    private static Random _random;
    public static void Initialize(){
        _random = new Random();
    }
    public static float GetRandomFloat(){
        return _random.nextFloat();
    }
    public static String GetDateTimeString(){
        LocalDateTime now = LocalDateTime.now();
        DateTimeFormatter formatter = DateTimeFormatter.ofPattern("yyyy-MM-dd HH:mm:ss");
        return now.format(formatter);
    }
    public static void FillEffects(long number, Collection<Byte> collection){
        boolean[] converted = ByteUtils.LongToBoolArray(number);
        for(byte b = 0; b < converted.length; b++){
            if(converted[b]){
                collection.add(b);
            }
        }
    }
    public static boolean EffectApplied(float minChance, float maxChance, byte statCode, MatchCharacter target, MatchCharacter caster){
        Main.LogMessage("Effect stat code: " + statCode);
        byte casterStat = caster.GetStatistic(statCode);
        Main.LogMessage("Caster stat: " + casterStat);
        byte targetStat = target.GetStatistic(statCode);
        Main.LogMessage("Target stat: " + targetStat);
        byte difference = (byte) (casterStat - targetStat);
        Main.LogMessage("Difference: " + difference);
        float chance = (50 + (difference * 10)) / 100.0f;
        Main.LogMessage("Chance of effect: " + chance);
        if(chance > 0.9f){
            chance = 0.9f;
        }
        if(chance < 0.1f){
            chance = 0.1f;
        }
        return chance > SharedFunctions.GetRandomFloat();
    }
    public static boolean HandleTeamMessage(byte[] decrypted, InGamePacketProcessor proc, Match owner, RemoteClient remote){
        byte teamID = decrypted[2];
        int messageLength = ByteUtils.ExtractInt(decrypted, 3);
        String messageString = ByteUtils.BytesToUTF8(decrypted, 7, messageLength);
        if(ProfanityChecker.ContainsProhibitedLanguage(messageString)){
            proc.EnqueueForSend(Packets.ProhibitedLanguagePacket(InGame_Send.ProhibitedLanguage),
                    remote);
        }
        else{
            MatchCharacter sender = owner.GetMatchCharacter(decrypted[1]);
            if(sender != null) {
                Main.LogChat(sender, messageString, owner._matchID);
                if (sender.GetTeamID() == teamID) {
                    proc.EnqueueForSend(Packets.MessagePacket(decrypted, InGame_Send.TeamMessage),
                            owner.GetMatchTeam(teamID).GetRemoteClients());
                } else {
                    ArrayList<RemoteClient> recipients = new ArrayList<RemoteClient>();
                    recipients.add(remote);
                    recipients.addAll(owner.GetMatchTeam(teamID).GetRemoteClients());
                    proc.EnqueueForSend(Packets.MessagePacket(decrypted, InGame_Send.TeamMessage),
                            recipients);
                }
            }
        }
        return true;
    }
}
