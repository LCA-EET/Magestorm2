import java.io.File;
import java.io.FileNotFoundException;
import java.util.Scanner;

public class ServerParams {
    public static String ExecutionDirectory;
    public static String LogFilePath;
    public static String ErrorFilePath;
    public static String EmailCredsPath;
    public static long InactivityWarning = 60000;
    public static long InactivityDisconnect = 120000;
    public static long PregameInactivity = 60000;
    public static byte TickInterval = 10;
    public static byte MaxMatches = 20;
    public static byte PollingFactor = 5;
    public static boolean SymmetricEncryption = false;
    public static short ListeningPort;

    public static void LoadParams(String paramFilePath){
        ExecutionDirectory = System.getProperty("user.dir");
        System.out.println("Loading parameters from " + paramFilePath);
        System.out.println("Time since epoch: " + System.currentTimeMillis());
        File paramFile = new File(paramFilePath);
        Scanner paramScanner = null;
        try {
            paramScanner = new Scanner(paramFile);
            ListeningPort = Short.parseShort(paramScanner.nextLine());
            Database.Init(paramScanner.nextLine(), paramScanner.nextLine(),
                    paramScanner.nextLine(), paramScanner.nextLine());
            EmailCredsPath = paramScanner.nextLine();
            ProfanityChecker.Init(paramScanner.nextLine());
            ErrorFilePath = paramScanner.nextLine();
            LogFilePath = paramScanner.nextLine();
            InactivityWarning = Long.parseLong(paramScanner.nextLine());
            InactivityDisconnect = Long.parseLong(paramScanner.nextLine());
            SymmetricEncryption = Boolean.parseBoolean(paramScanner.nextLine());
            PregameInactivity = Long.parseLong(paramScanner.nextLine());
            TickInterval = Byte.parseByte(paramScanner.nextLine());
            MaxMatches = Byte.parseByte(paramScanner.nextLine());
            PollingFactor = Byte.parseByte(paramScanner.nextLine());
            System.out.println("Log file: " + LogFilePath);
            System.out.println("Error file: " + ErrorFilePath);
            Main.InitLog();
        } catch (FileNotFoundException e) {
            throw new RuntimeException(e);
        }
    }
}
