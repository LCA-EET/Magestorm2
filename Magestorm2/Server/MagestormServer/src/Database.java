import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.util.ArrayList;
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
    public static void BanAccount(int accountID){
        String sql = "UPDATE accounts SET accountstatus = 2 WHERE (accoundID = ?)";
        try(Connection conn = DBConnection()){
            PreparedStatement ps = conn.prepareStatement(sql);
            ps.setInt(1, accountID);
            ps.execute();
        }
        catch(Exception e){
            Main.LogError("Database.BanAccount(): " + e.getMessage());
        }
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
            Main.LogMessage("Updated database with port and key information.");
            return true;
        }
        catch(Exception ex){
            Main.LogError("Database.UpdateServerInfo():" + " Failed to update database with port and key information: " + ex.getMessage());
        }
        return false;
    }
    public static boolean TestDBConnection(){
        Main.LogMessage("Testing DB Connection.");
        try(Connection conn = DBConnection()) {
            Main.LogMessage("DB Connection Test Successful.");
            return true;
        } catch (Exception e) {
            Main.LogError("Database.TestDBConnection(): " + e.getMessage());
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
            Main.LogError("Database.DBConnection(): " + e.getMessage());
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
            ResultSet rs = ps.executeQuery();
            if(rs.next()){
                toReturn = rs.getInt("recordCount");
            }
            else{
                toReturn = 0;
            }
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
            ps.execute();
            return true;
        }
        catch(Exception e){
            Main.LogError("DB Account Creation Failure: " + e.getMessage());
        }
        return false;
    }
    public static boolean SeeIfCharacterExists(String charname){
         boolean characterExists = true;
         String sql = "SELECT COUNT(*) as recordCount FROM characters WHERE charname=? AND charstatus=1";
         try(Connection conn = DBConnection()){
             PreparedStatement ps = conn.prepareStatement(sql);
             ps.setString(1, charname);
             ResultSet rs = ps.executeQuery();
             if(rs.next()){
                 int count = rs.getInt("recordCount");
                 if(count == 0){
                     characterExists = false;
                 }
             }
         }
         catch(Exception e){
             Main.LogError("Character name check failure: " + e.getMessage());
         }
         return characterExists;
    }
    public static boolean DeactivateCharacter(int characterID, int accountID){
        boolean toReturn = false;
        String sql = "UPDATE characters SET charstatus=0 WHERE id=? and accountid=?";
        try(Connection conn = DBConnection()){
            PreparedStatement ps = conn.prepareStatement(sql);
            ps.setInt(1, characterID);
            ps.setInt(2, accountID);
            ps.execute();
            toReturn = true;
        }
        catch(Exception e){
            Main.LogError("Deactivate character: " + e.getMessage());
        }
        return toReturn;
    }

    public static int AddCharacter(int accountID, String charname, byte classCode, byte[] stats, byte[] appearance){
        int charID = -1;
        String sql = "INSERT INTO characters(accountid, charname, charclass, charstatus, statstr, statdex, statcon, statint, statcha, statwis, appsex, appskin, apphair, appface, apphead, level) VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
        Main.LogMessage("Adding character " + charname + " to database.");
        try(Connection conn = DBConnection()){
            PreparedStatement ps = conn.prepareStatement(sql, PreparedStatement.RETURN_GENERATED_KEYS);
            ps.setInt(1, accountID);
            ps.setString(2, charname);
            ps.setByte(3, classCode);
            ps.setByte(4, CharacterStatus.Activated);
            ps.setByte(5, stats[0]);
            ps.setByte(6, stats[1]);
            ps.setByte(7, stats[2]);
            ps.setByte(8, stats[3]);
            ps.setByte(9, stats[4]);
            ps.setByte(10, stats[5]);
            ps.setByte(11, appearance[0]);
            ps.setByte(12, appearance[1]);
            ps.setByte(13, appearance[2]);
            ps.setByte(14, appearance[3]);
            ps.setByte(15, appearance[4]);
            ps.setByte(16, (byte)1);
            ps.execute();
            ResultSet rs = ps.getGeneratedKeys();
            if (rs.next()) {
                charID = rs.getInt(1);
            }
        }
        catch(Exception e){
            Main.LogError("Database.AddCharacter(): " + e.getMessage());
        }
        return charID;
    }
    private static PlayerCharacter GetCharacter(int accountID, int characterID, Connection conn){
        PlayerCharacter toReturn = null;
        String sql = "SELECT id, charname, charclass, statstr, statdex, statcon, statint, statcha, statwis, appsex, appskin, apphair, appface, apphead, level, experience FROM characters WHERE accountid = ? AND id = ?";
        try{
            PreparedStatement ps = conn.prepareStatement(sql);
            ps.setInt(1, accountID);
            ps.setInt(2, characterID);
            ResultSet rs = ps.executeQuery();
            while (rs.next()) {
                String characterName = rs.getString("charname");
                byte charClass = rs.getByte("charclass");
                Main.LogMessage("Fetched character: " + characterName);
                byte[] characterIDBytes = ByteUtils.IntToByteArray(characterID);
                byte[] nameBytes = characterName.getBytes(UTF_8);
                byte nameLength = (byte) nameBytes.length;
                byte[] fetched = new byte[22 + nameLength];
                System.arraycopy(characterIDBytes, 0, fetched, 0, 4);
                fetched[4] = charClass;
                byte strength = rs.getByte("statstr");
                byte dexterity = rs.getByte("statdex");
                byte constitution = rs.getByte("statcon");
                byte intellect = rs.getByte("statint");
                byte charisma = rs.getByte("statcha");
                byte wisdom = rs.getByte("statwis");
                byte appsex = rs.getByte("appsex");
                byte appskin = rs.getByte("appskin");
                byte apphair = rs.getByte("apphair");
                byte appface = rs.getByte("appface");
                byte apphead = rs.getByte("apphead");
                byte level = rs.getByte("level");
                int experience = rs.getInt("experience");
                fetched[5] = strength;
                fetched[6] = dexterity;
                fetched[7] = constitution;
                fetched[8] = intellect;
                fetched[9] = charisma;
                fetched[10] = wisdom;
                fetched[11] = appsex;
                fetched[12] = appskin;
                fetched[13] = apphair;
                fetched[14] = appface;
                fetched[15] = apphead;
                fetched[16] = level;
                byte[] experienceBytes = ByteUtils.IntToByteArray(experience);
                System.arraycopy(experienceBytes, 0,fetched, 17, 4);
                fetched[21] = nameLength;
                System.arraycopy(nameBytes, 0, fetched, 22, nameLength);
                toReturn = new PlayerCharacter(fetched, accountID);
            }
        }
        catch(Exception ex){
            Main.LogError("Database.GetCharacter(): " + ex.getMessage());
        }
        return toReturn;
    }
    public static byte CheckIfNameIsUsed(String toCheck)
    {
        String sql = "SELECT id FROM characters WHERE charname = ? AND charstatus = 1";
        byte toReturn = 0;
        try(Connection conn = DBConnection()){
            PreparedStatement ps = conn.prepareStatement(sql);
            ps.setString(1, toCheck);
            ResultSet rs = ps.executeQuery();
            while(rs.next()){
                toReturn = 1;
            }
        }
        catch(Exception e){
            toReturn = 2;
            Main.LogError("Database.CheckIfNameIsUsed(): " + e.getMessage());
        }
        return toReturn;
    }

    public static void UpdateCharacterAppearance(int characterID, byte[] appearanceBytes){
        try(Connection conn = DBConnection()){
            String sql = "UPDATE characters SET appsex = ?, appskin = ?, apphair = ?, appface = ?, apphead = ? WHERE id = ?";
            PreparedStatement ps = conn.prepareStatement(sql);
            for(int i = 0; i< appearanceBytes.length; i++){
                ps.setByte(i + 1, appearanceBytes[i]);
            }
            ps.setInt(6, characterID);
            ps.execute();
        }
        catch (Exception e){
            Main.LogError("Database.UpdateCharacterAppearance(): " + e.getMessage());
        }
    }

    public static byte[] GetCharactersOfAccount(int accountID){
        String sql = "SELECT id FROM characters WHERE accountid = ? AND charstatus = ?";
        ArrayList<byte[]> characterBytes = new ArrayList<>();
        int totalLength = 0;
        try(Connection conn = DBConnection()){
            PreparedStatement ps = conn.prepareStatement(sql);
            ps.setInt(1, accountID);
            ps.setByte(2, CharacterStatus.Activated);
            ResultSet rs = ps.executeQuery();
            while (rs.next()){
                int characterID = rs.getInt("id");
                PlayerCharacter character = CharacterManager.GetCharacter(characterID);
                if(character == null){
                    character = GetCharacter(accountID, characterID, conn);
                }
                if(character != null){
                    byte[] charBytes = character.GetCharacterBytes();
                    totalLength += charBytes.length;
                    characterBytes.add(charBytes);
                }
            }
        }
        catch(Exception e){
            Main.LogError("Database.GetCharactersOfAccount(): " + e.getMessage());
        }
        byte[] toReturn = new byte[1 + totalLength];
        toReturn[0] = (byte)characterBytes.size();
        int index = 1;
        for (byte[] toAdd : characterBytes) {
            System.arraycopy(toAdd, 0, toReturn, index, toAdd.length);
            index += toAdd.length;
        }
        return toReturn;
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
            Main.LogError("Database.ValidateCredentials(): " + e.getMessage());
        }
        toReturn[0] = validated;
        toReturn[1] = accountid;
        return toReturn;
    }

    public static byte[] GetLevelsList(byte status){
        byte[] toReturn = null;
        String sql = "SELECT id, scenename, maxplayers FROM levels WHERE status=?";
        try(Connection conn = DBConnection()){
            PreparedStatement ps = conn.prepareStatement(sql);
            ps.setByte(1, status);
            ResultSet rs = ps.executeQuery();
            ArrayList<byte[]> bytesReturned = new ArrayList<>();
            int totalLength = 0;
            while (rs.next()) {
                byte sceneID = rs.getByte("id");
                String sceneName = rs.getString("scenename");
                byte maxPlayers = rs.getByte("maxplayers");
                byte[] nameBytes = sceneName.getBytes(UTF_8);
                byte[] fetched = new byte[1 + 1 + 1 + nameBytes.length];
                fetched[0] = sceneID;
                fetched[1] = maxPlayers;
                fetched[2] = (byte)nameBytes.length;
                System.arraycopy(nameBytes, 0, fetched, 3, nameBytes.length);
                bytesReturned.add(fetched);
                totalLength += fetched.length;
            }
            toReturn = new byte[totalLength + 2];
            toReturn[0] = Pregame_OpCode_Send.LevelsList;
            toReturn[1] = (byte)bytesReturned.size();
            int index = 2;
            for(byte[] sceneData : bytesReturned){
                System.arraycopy(sceneData, 0, toReturn, index, sceneData.length);
                index += sceneData.length;
            }
        }
        catch(Exception e){
            Main.LogError("Database.GetScenes(): " + e.getMessage());
        }
        return toReturn;
    }
}
