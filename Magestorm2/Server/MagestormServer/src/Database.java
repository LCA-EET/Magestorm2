import java.sql.Connection;
import java.sql.DriverManager;

public class Database {
    private static String _userName;
    private static String _password;
    private static String _conn;

    public static void Init(String conn, String user, String pass){
        _conn = conn;
        _userName = user;
        _password = pass;
    }
    public static boolean TestDBConnection(){
        Main.LogMessage("Testing DB Connection.");
        try {
            Class.forName("com.mysql.cj.jdbc.Driver");
            Connection conn = DriverManager.getConnection(_conn, _userName, _password);
            conn.close();
            Main.LogMessage("DB Connection Test Successful.");
            return true;
        } catch (Exception e) {
            Main.LogError(e.getMessage());
        }
        Main.LogMessage("DB Connection Test Failed.");
        return false;
    }
}
