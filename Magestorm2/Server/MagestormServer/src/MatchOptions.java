import java.util.ArrayList;

public class MatchOptions {
    private final ArrayList<Boolean> _options;
    public MatchOptions(byte options){
        _options = ByteUtils.ByteArrayToBits(new byte[]{options});
    }
    public boolean IsOptionSet(int index){
        return _options.get(index);
    }
}
