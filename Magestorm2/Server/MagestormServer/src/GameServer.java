public class GameServer extends Thread {
    private static UIPacketProcessor _loginProcessor;

    public static void init(){
       _loginProcessor = new UIPacketProcessor();
    }


}
