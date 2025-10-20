public class Vector3 {

    private final byte[] _position;

    public Vector3(){
        _position = new byte[12];
    }

    public Vector3(byte[] decrypted, int index){
        _position = new byte[12];
        System.arraycopy(decrypted, index, _position, 0, _position.length);
    }

    public byte[] GetPositionBytes(){
        return _position;
    }


}
