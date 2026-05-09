import java.util.ArrayList;

public class MatchOptions {


    private final ArrayList<Boolean> _options;

    public MatchOptions(byte options){

        _options = ByteUtils.ByteArrayToBits(new byte[]{options});
        /*
        for(int i = 0; i < _options.size(); i++){
            Main.LogMessage(i + ": " + _options.get(i).toString());
        }
        */
    }

    public boolean IsOptionSet(int index){
        return _options.get(index);
    }
}
