import java.util.ArrayList;
import java.util.Random;

public class SharedFunctions {


    private static Random _random;
    public static void Initialize(){
        _random = new Random();
    }
    public static float GetRandomFloat(){
        return _random.nextFloat();
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
            if(owner.GetMatchCharacter(decrypted[1]).GetTeamID() == teamID){
                proc.EnqueueForSend(Packets.MessagePacket(decrypted, InGame_Send.TeamMessage),
                        owner.GetMatchTeam(teamID).GetRemoteClients());
            }
            else {
                ArrayList<RemoteClient> recipients = new ArrayList<RemoteClient>();
                recipients.add(remote);
                recipients.addAll(owner.GetMatchTeam(teamID).GetRemoteClients());
                proc.EnqueueForSend(Packets.MessagePacket(decrypted, InGame_Send.TeamMessage),
                        recipients);
            }
        }
        return true;
    }
}
