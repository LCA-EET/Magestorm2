import java.io.FileNotFoundException;
import java.nio.charset.Charset;
import java.nio.charset.StandardCharsets;

public class Main {
    public static Charset charset = StandardCharsets.UTF_8;
    private static Log _serverLog;
    public static boolean Running = true;

    public static void main(String args[]) throws FileNotFoundException {
        ServerParams.LoadParams();
        _serverLog = new Log(ServerParams.LogFilePath, ServerParams.ErrorFilePath);
        new Thread(_serverLog).start();
        Database.TestDBConnection();
        GameServer gameServer = new GameServer();
        gameServer.start();
    }
    public static void LogMessage(String toLog){
        _serverLog.LogMessage(toLog);
    }
    public static void LogError(String toLog){
        _serverLog.LogError(toLog);
    }
}
