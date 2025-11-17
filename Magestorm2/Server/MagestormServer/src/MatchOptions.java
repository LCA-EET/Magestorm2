import java.util.ArrayList;

public class MatchOptions {
    public static final int FastRegen = 0;
    public static final int NoSolidWalls = 1;
    public static final int AntiStack = 2;
    public static final int NoResurrection = 3;
    public static final int NoHealOther = 4;

    private final ArrayList<Boolean> _options;

    public MatchOptions(byte[] options){

        _options = ByteUtils.ByteArrayToBits(options);
        for(int i = 0; i < _options.size(); i++){
            Main.LogMessage(i + ": " + _options.get(i).toString());
        }
    }

    public boolean IsOptionSet(int index){
        return _options.get(index);
    }
}
