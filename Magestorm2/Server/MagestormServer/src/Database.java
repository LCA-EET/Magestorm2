import java.nio.charset.StandardCharsets;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
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
        try(Connection conn = DBConnection()){
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
        try(Connection conn = DBConnection()) {
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
    public static int AccountRecordCount(String username, String email){
        int toReturn = -1;
        String sql = "SELECT COUNT(*) AS recordCount FROM accounts WHERE (accountname=?) OR (email=?)";
        try(Connection conn = DBConnection()){
            PreparedStatement ps = conn.prepareStatement(sql);
            ps.setString(1, username );
            ps.setString(2, email );
            //ps.addBatch();
            ResultSet rs = ps.executeQuery();
            if(rs.next()){
                toReturn = rs.getInt("recordCount");
            }
            else{
                toReturn = 0;
            }
            conn.close();
            Main.LogMessage("Record Count " + toReturn);
        }
        catch(Exception e){
            Main.LogError("Database.AccountRecordCount: " + e.getMessage());
        }
        return toReturn;
    }
    public static boolean CreateAccount(String username, String hash, String email, long token){
        Main.LogMessage("Creating account: " + username + ", " + hash + ", " + email);
        String sql = "INSERT INTO accounts(accountname, hash, email, accountstatus, activationtoken) VALUES(?,?,?,?,?)";

        Main.LogMessage("Activation token: " + token);
        try(Connection conn = DBConnection()){
            PreparedStatement ps = conn.prepareStatement(sql);
            ps.setString(1, username );
            ps.setString(2, hash );
            ps.setString(3, email );
            ps.setByte(4, (byte)0);
            ps.setLong(5, token);
            ps.addBatch();
            ps.execute();
            return true;
        }
        catch(Exception e){
            Main.LogError("DB Account Creation Failure: " + e.getMessage());
        }
        return false;
    }

    public static Object[] ValidateCredentials(String username, String hash){
        Object[] toReturn = new Object[2];
        boolean validated = false;
        String sql = "SELECT id, hash FROM accounts WHERE accountname=?";
        int accountid = -1;
        try (Connection conn = DBConnection()){
            PreparedStatement ps = conn.prepareStatement(sql);
            ps.setString(1, username);
            ResultSet rs = ps.executeQuery();
            if(rs.next()){
                String dbHash = rs.getString("hash");
                if(dbHash.equals(hash)){
                    validated = true;
                    accountid = rs.getInt("id");
                }
            }
        }
        catch(Exception e){
            Main.LogError("Credential validation failure: " + e.getMessage());
        }
        toReturn[0] = validated;
        toReturn[1] = accountid;
        return toReturn;
    }
}
