import javax.xml.crypto.Data;
import java.io.File;
import java.io.FileNotFoundException;
import java.net.InetAddress;
import java.util.Scanner;

public class ServerParams {
    public static String ExecutionDirectory;
    public static String LogFilePath;
    public static String ErrorFilePath;
    public static int ListeningPort;

    public static void LoadParams(){
        ExecutionDirectory = System.getProperty("user.dir");
        LogFilePath = ExecutionDirectory + "/log.txt";
        ErrorFilePath = ExecutionDirectory + "/error.txt";
        File paramFile = new File(ExecutionDirectory + "/serverparams.txt");
        Scanner paramScanner = null;
        try {
            paramScanner = new Scanner(paramFile);
            ListeningPort = Integer.parseInt(paramScanner.nextLine());
            Database.Init(paramScanner.nextLine(), paramScanner.nextLine(), paramScanner.nextLine());

        } catch (FileNotFoundException e) {
            throw new RuntimeException(e);
        }

    }
}
