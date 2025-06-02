public class Vector3 {

    private final byte[] _position;

    public Vector3(){
        _position = new byte[12];
    }

    public void SetCoordinate(byte[] decrypted, int index, byte offset){
        for(int i = 0; i < 4; i++){
            _position[i + offset] = decrypted[index + i];
        }
    }

    public byte[] GetPosition(){
        return _position;
    }


}
