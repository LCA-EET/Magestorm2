import java.nio.charset.StandardCharsets;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.util.Base64;

import static java.nio.charset.StandardCharsets.UTF_8;

public class Database {
    private static String _userName;
    private static String _password;
    private static String _conn;
    private static String _psk;

    public static void Init(String conn, String user, String pass, String psk){
        _conn = conn;
        _userName = user;
        _password = pass;
        _psk = psk;
    }
    public static boolean UpdateServerInfo(){
        String portNumber = "6000";

        String sql = "UPDATE serverinfo SET portnumber=?, encryptionkey=? WHERE id=0";
        Connection conn = DBConnection();
        try{
            PreparedStatement ps = conn.prepareStatement(sql);
            ps.setInt(1, 6000);
            Base64.getEncoder().encodeToString((Cryptographer.Key()));
            String key64 = Base64.getEncoder().encodeToString((Cryptographer.Key()));
            Main.LogMessage("key64: " + key64);
            ps.setString(2, key64);
            ps.addBatch();
            ps.executeBatch();
            conn.close();
            Main.LogMessage("Updated database with port and key information.");
            return true;
        }
        catch(Exception ex){
            Main.LogError("Failed to update database with port and key information: " + ex.getMessage());
        }
        return false;
    }
    public static boolean TestDBConnection(){
        Main.LogMessage("Testing DB Connection.");
        try {
            Connection conn = DBConnection();
            conn.close();
            Main.LogMessage("DB Connection Test Successful.");
            return true;
        } catch (Exception e) {
            Main.LogError(e.getMessage());
        }
        Main.LogMessage("DB Connection Test Failed.");
        return false;
    }
    private static Connection DBConnection(){
        try{
            Class.forName("com.mysql.cj.jdbc.Driver");
            return DriverManager.getConnection(_conn, _userName, _password);
        }
        catch(Exception e){

        }
        return null;
    }

}
