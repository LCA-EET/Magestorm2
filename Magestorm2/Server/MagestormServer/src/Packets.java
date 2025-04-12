import java.util.ArrayList;

public class Packets {

    private static final byte[] AccountCreated_Bytes = new byte[]{OpCode_Send.AccountCreated};
    private static final byte[] AccountCreationFailed_Bytes = new byte[]{OpCode_Send.AccountCreationFailed};
    private static final byte[] AccountExistsPacket_Bytes = new byte[]{OpCode_Send.AccountAlreadyExists};
    private static final byte[] AlreadyLoggedInPacket_Bytes = new byte[]{OpCode_Send.AlreadyLoggedIn};
    private static final byte[] LoginFailedPacket_Bytes = new byte[]{OpCode_Send.LogInFailed};
    private static final byte[] LoginSucceededPacket_Bytes = new byte[]{OpCode_Send.LogInSucceeded};
    private static final byte[] ProhibitedLanguagePacket_Bytes = new byte[]{OpCode_Send.ProhibitedLanguage};

    public static byte[] LoginFailedPacket(){
        return Cryptographer.Encrypt(LoginFailedPacket_Bytes);
    }

    public static byte[] LoginSucceededPacket(){
        return Cryptographer.Encrypt(LoginSucceededPacket_Bytes);
    }

    public static byte[] AccountExistsPacket(){
        return Cryptographer.Encrypt(AccountExistsPacket_Bytes);
    }

    public static byte[] AccountCreatedPacket(){
        return Cryptographer.Encrypt(AccountCreated_Bytes);
    }

    public static byte[] AccountCreationFailedPacket(){
        return Cryptographer.Encrypt(AccountCreationFailed_Bytes);
    }

    public static byte[] AlreadyLoggedInPacket(){
        return Cryptographer.Encrypt(AlreadyLoggedInPacket_Bytes);
    }

    public static byte[] ProhibitedLanguagePacket(){
        return Cryptographer.Encrypt(ProhibitedLanguagePacket_Bytes);
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
}
