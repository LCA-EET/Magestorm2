public class Vector3 {

    private final byte[] _position;

    public Vector3(){
        _position = new byte[12];
    }

    public void SetCoordinates(byte[] decrypted, int index){
        System.arraycopy(decrypted, index, _position, 0, _position.length);
    }

    public byte[] GetPosition(){
        return _position;
    }


}
