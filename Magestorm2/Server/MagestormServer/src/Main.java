import java.io.FileNotFoundException;
import java.nio.charset.Charset;
import java.nio.charset.StandardCharsets;
import java.util.Scanner;

public class Main {
    public static Charset charset = StandardCharsets.UTF_8;
    private static Log _serverLog;
    public static Emailer Mailer;
    public static ThreadMonitor ThreadMonitor;
    public static AsyncDBUpdater AsyncDBUpdater;
    public static boolean Running = true;
    public static boolean Debug = false;
    public static void main(String args[]) throws FileNotFoundException {
        ThreadMonitor = new ThreadMonitor();
        SharedFunctions.Initialize();
        String paramFilePath = args[0];
        ServerParams.LoadParams(paramFilePath);
        Mailer = new Emailer(ServerParams.EmailCredsPath);
        new RegisteredThread(_serverLog).start();
        Main.LogMessage("Pregame Inactivity Timeout: " + ServerParams.PregameInactivity);
        Main.LogMessage("Ingame Inactivity Timeout: " + ServerParams.IngameInactivityDisconnect);

        Cryptographer.GenerateKeyAndIV();
        if(Database.TestDBConnection()){
            Database.UpdateServerInfo();
            GameServer.init();
            AsyncDBUpdater = new AsyncDBUpdater();
            ProcessCommands();
        }
        else{
            Main.LogError("Exiting due to a failure to access the database.");
            System.exit(0);
        }
    }
    private static void ProcessCommands(){
        try (Scanner scanner = new Scanner(System.in)) {
            while(Main.Running){
                System.out.print(">: ");
                String command = scanner.nextLine();
                switch(command){
                    case "lc":
                        ProcessListRCCommand();
                        break;
                    case "exit":
                    case "ts":
                    case "terminateserver":
                        GameServer.TerminateServer();
                        break;
                    case "lt":
                        ThreadMonitor.PrintActiveThreads();
                        break;
                }
            }
        }
    }

    private static void ProcessListRCCommand(){
        System.out.println("Pregame Clients");
        System.out.println("===============");
        System.out.println(RemoteClientManager.PregameClients().toString());
        System.out.println("Ingame Clients");
        System.out.println("===============");
        System.out.println(MatchManager.GetMatches().toString());
    }
    public static void InitLog(){
        _serverLog = new Log(ServerParams.LogFilePath, ServerParams.ErrorFilePath,
                ServerParams.DebugFilePath, ServerParams.ChatFilePath);
    }
    public static void LogMessage(String toLog){
        _serverLog.LogMessage(toLog);
    }
    public static void LogChat(MatchCharacter sender, String message, Number matchID){
        _serverLog.LogChat(sender, message, matchID);
    }
    public static void LogDebug(String toLog){
        _serverLog.LogDebug(toLog);
    }
    public static void LogError(String toLog){
        _serverLog.LogError(toLog);
    }
    public static void LogStackTrace(Exception ex){
        StackTraceElement[] toPrint = ex.getStackTrace();
        for(StackTraceElement element : toPrint){
            Main.LogError(element.toString());
        }
    }
}
