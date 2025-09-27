import javax.xml.crypto.Data;
import java.io.File;
import java.io.FileNotFoundException;
import java.net.InetAddress;
import java.util.Scanner;

public class ServerParams {
    public static String ExecutionDirectory;
    public static String LogFilePath;
    public static String ErrorFilePath;
    public static String EmailCredsPath;
    public static int ListeningPort;

    public static void LoadParams(String paramFilePath){
        ExecutionDirectory = System.getProperty("user.dir");
        System.out.println("Loading parameters from " + paramFilePath);
        System.out.println("Time since epoch: " + System.currentTimeMillis());
        File paramFile = new File(paramFilePath);
        Scanner paramScanner = null;
        try {
            paramScanner = new Scanner(paramFile);
            ListeningPort = Integer.parseInt(paramScanner.nextLine());
            Database.Init(paramScanner.nextLine(), paramScanner.nextLine(),
                    paramScanner.nextLine(), paramScanner.nextLine());
            EmailCredsPath = paramScanner.nextLine();
            ProfanityChecker.Init(paramScanner.nextLine());
            ErrorFilePath = paramScanner.nextLine();
            LogFilePath = paramScanner.nextLine();
            System.out.println("Log file: " + LogFilePath);
            System.out.println("Error file: " + ErrorFilePath);
            Main.InitLog();
        } catch (FileNotFoundException e) {
            throw new RuntimeException(e);
        }
    }
}
