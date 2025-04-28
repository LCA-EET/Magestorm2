public class AntiCheat {
    public static boolean CheckStats(byte[] stats, RemoteClient rc, int accountID){
        boolean isCheating = false;
        int total = 0;
        for(byte stat : stats){
            if(stat > 20 || stat < 10){
                isCheating = true;
                break;
            }
            else{
                total += stat;
            }
        }
        if(total > 90){
            isCheating = true;
        }
        if(isCheating){
            Database.BanAccount(accountID);
            GameServer.EnqueueForSend(Packets.BannedForCheatingPacket(), rc);
            Main.LogMessage("Account banned for stat hacking: " + accountID);
        }
        return isCheating;
    }
}
