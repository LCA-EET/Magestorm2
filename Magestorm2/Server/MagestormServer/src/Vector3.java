public class Vector3 {

    private final byte[] _bytes;

    public Vector3(){
        _bytes = new byte[12];
    }

    public Vector3(byte[] decrypted, int index){
        _bytes = new byte[12];
        Update(decrypted, index);
    }

    public byte[] GetBytes(){
        return _bytes;
    }

    public void Update(byte[] decrypted, int index){
        System.arraycopy(decrypted, index, _bytes, 0, _bytes.length);
    }

}
