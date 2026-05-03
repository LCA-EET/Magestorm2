public class LogEvent {
    private final String _logText;
    private final byte _logID;

    public LogEvent(byte logID, String text){
        _logText = text;
        _logID = logID;
    }

    public byte GetLogID(){
        return _logID;
    }

    public String GetEventText(){
        return _logText;
    }
}
