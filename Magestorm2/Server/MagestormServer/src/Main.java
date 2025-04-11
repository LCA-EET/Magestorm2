import javax.xml.crypto.Data;
import java.io.FileNotFoundException;
import java.nio.charset.Charset;
import java.nio.charset.StandardCharsets;

public class Main {
    public static Charset charset = StandardCharsets.UTF_8;
    private static Log _serverLog;
    public static Emailer Mailer;
    public static boolean Running = true;

    public static void main(String args[]) throws FileNotFoundException {
        String paramFilePath = args[0];
        ServerParams.LoadParams(paramFilePath);
        Mailer = new Emailer(ServerParams.EmailCredsPath);

        new Thread(_serverLog).start();
        Cryptographer.GenerateKeyAndIV();
        if(Database.TestDBConnection()){
            Database.UpdateServerInfo();
            GameServer.init();
        }
    }
    public static void InitLog(String logPath, String errorPath){
        _serverLog = new Log(ServerParams.LogFilePath, ServerParams.ErrorFilePath);
    }
    public static void LogMessage(String toLog){
        _serverLog.LogMessage(toLog);
    }
    public static void LogError(String toLog){
        _serverLog.LogError(toLog);
    }
}
