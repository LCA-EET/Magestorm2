import java.io.FileNotFoundException;
import java.io.PrintWriter;
public class ServerMain{
    private static Log _serverLog;
    public static boolean Running = true;

    public static void main(String args[]) throws FileNotFoundException {
        _serverLog = new Log("F:/repo/Magestorm2/Magestorm2/Server/MagestormServer/out/production/MagestormServer/log.txt", "error.txt");
        new Thread(_serverLog).start();
        GameServer gameServer = new GameServer();
        gameServer.start();
        while(Running) {

        }
    }
    public static void LogMessage(String toLog){
        _serverLog.LogMessage(toLog);
    }
    public static void LogError(String toLog){
        _serverLog.LogError(toLog);
    }
}
