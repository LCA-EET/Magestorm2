import java.nio.ByteBuffer;
import java.nio.charset.StandardCharsets;
import java.util.ArrayList;

public class Packets {

    private static final byte[] AccountCreated_Bytes = new byte[]{OpCode_Send.AccountCreated};
    private static final byte[] CreationFailed_Bytes = new byte[]{OpCode_Send.CreationFailed};
    private static final byte[] AccountExistsPacket_Bytes = new byte[]{OpCode_Send.AccountAlreadyExists};
    private static final byte[] AlreadyLoggedInPacket_Bytes = new byte[]{OpCode_Send.AlreadyLoggedIn};
    private static final byte[] LoginFailedPacket_Bytes = new byte[]{OpCode_Send.LogInFailed};
    private static final byte[] ProhibitedLanguagePacket_Bytes = new byte[]{OpCode_Send.ProhibitedLanguage};
    private static final byte[] CharacterExistsPacket_Bytes = new byte[]{OpCode_Send.CharacterExists};
    private static final byte[] InactivityDisconnect_Bytes = new byte[]{OpCode_Send.InactivityDisconnect};

    public static byte[] InactivityDisconnectPacket() { return Cryptographer.Encrypt(InactivityDisconnect_Bytes);}
    public static byte[] LoginFailedPacket(){
        return Cryptographer.Encrypt(LoginFailedPacket_Bytes);
    }

    public static byte[] AccountExistsPacket(){
        return Cryptographer.Encrypt(AccountExistsPacket_Bytes);
    }

    public static byte[] AccountCreatedPacket(){
        return Cryptographer.Encrypt(AccountCreated_Bytes);
    }

    public static byte[] CreationFailedPacket(){
        return Cryptographer.Encrypt(CreationFailed_Bytes);
    }

    public static byte[] AlreadyLoggedInPacket(){
        return Cryptographer.Encrypt(AlreadyLoggedInPacket_Bytes);
    }

    public static byte[] ProhibitedLanguagePacket(){ return Cryptographer.Encrypt(ProhibitedLanguagePacket_Bytes);}

    public static byte[] CharacterExistsPacket(){ return Cryptographer.Encrypt(CharacterExistsPacket_Bytes);}

    public static byte[] CharacterDeletedPacket(int characterID) {
        byte[] toEncrypt = new byte[5];
        toEncrypt[0] = OpCode_Send.CharacterDeleted;
        byte[] idBytes = Packets.IntToByteArray(characterID);
        System.arraycopy(idBytes,0, toEncrypt, 1, 4);
        return Cryptographer.Encrypt(toEncrypt);
    }

    public static byte[] LoginSucceededPacket(int accountID){
        byte[] characterBytes = Database.GetCharactersForAccount(accountID);
        Main.LogMessage("Character bytes retrieved: " + characterBytes.length);
        byte[] toSend;
        if(characterBytes.length > 0){
            toSend = new byte[5 + characterBytes.length];
            System.arraycopy(characterBytes, 0, toSend, 5, characterBytes.length);
        }
        else{
            toSend = new byte[5];
        }
        toSend[0] = OpCode_Send.LogInSucceeded;
        byte[] accountBytes = Packets.IntToByteArray(accountID);
        System.arraycopy(accountBytes, 0, toSend, 1, 4);
        return Cryptographer.Encrypt(toSend);
    }

    public static byte[] CharacterCreatedPacket(int characterID, byte classCode, String charname){
        byte nameLength = (byte)charname.length();
        byte[] toReturn = new byte[7 + nameLength];
        toReturn[0] = OpCode_Send.CharacterCreated;

        toReturn[1] = classCode;
        toReturn[2] = nameLength;
        byte[] idBytes = Packets.IntToByteArray(characterID);
        byte[] nameBytes = charname.getBytes(StandardCharsets.UTF_8);
        System.arraycopy(idBytes,0,toReturn, 3, 4);
        System.arraycopy(nameBytes,0,toReturn,7,nameLength);
        return Cryptographer.Encrypt(toReturn);
    }

    public static byte[] RemovedFromServerPacket(byte reasonCode){
        return Cryptographer.Encrypt(new byte[]{OpCode_Send.RemovedFromServer, reasonCode});
    }

    public static ArrayList<byte[]> ExtractBytes(byte[] decrypted, int firstIndex){
        ArrayList<byte[]> toReturn = new ArrayList<>();
        int index = firstIndex;
        byte[] lengths = new byte[firstIndex - 1];
        System.arraycopy(decrypted, 1,lengths,0,lengths.length);
        for(int i = 0; i < lengths.length; i++){
            byte[] toAdd = new byte[lengths[i]];
            System.arraycopy(decrypted, index, toAdd, 0, lengths[i]);
            index += lengths[i];
            toReturn.add(toAdd);
        }
        return toReturn;
    }

    public static int ExtractInt(byte[] decrypted, int index){
        return ByteBuffer.wrap(decrypted).getInt(index);
    }

    public static byte[] ExtractBytes(byte[] decrypted, int index, int length){
        byte[] toReturn = new byte[length];
        System.arraycopy(decrypted, index, toReturn, 0, length);
        return toReturn;
    }

    public static byte[] IntToByteArray(int value) {
        return new byte[] {
                (byte)(value >> 24),
                (byte)(value >> 16),
                (byte)(value >> 8),
                (byte)value};
    }
}
