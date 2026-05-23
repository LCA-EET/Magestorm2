import java.io.FileWriter;
import java.io.IOException;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;
import java.util.concurrent.LinkedBlockingQueue;

public class Log extends RegisteredThread{
    private final DateTimeFormatter _formatter;
    private final LinkedBlockingQueue<LogEvent> _eventQueue;
    private final FileWriter _logFileWriter, _errorFileWriter, _debugFileWriter, _chatFileWriter;
    public Log(String logFile, String errorFile, String debugFile, String chatFile){
        _formatter = DateTimeFormatter.ofPattern("yyyy-MM-dd HH:mm:ss");
        try {
            _logFileWriter = new FileWriter(logFile, true);
            _errorFileWriter = new FileWriter(errorFile, true);
            _debugFileWriter = new FileWriter(debugFile, true);
            _chatFileWriter = new FileWriter(chatFile, true);
        } catch (IOException e) {
            throw new RuntimeException(e);
        }
        _eventQueue = new LinkedBlockingQueue<>();
    }
    public void run(){
        Register("Log");
        Main.LogMessage("Log started.");
        Main.LogError("Log started.");
        Main.LogDebug("Log started.");
        while(!_terminated){
            try{
                ProcessQueue(); // blocking
            }
            catch(Exception e){
                System.err.println(e.getMessage());
            }
        }
        try {
            _logFileWriter.close();
            _errorFileWriter.close();
            _debugFileWriter.close();
        } catch (IOException e) {
            System.err.println(e.getMessage());
        }
        Deregister();
    }
    private void ProcessQueue(){
        try{
            LogEvent event = _eventQueue.take();
            byte logID = event.GetLogID();
            switch(logID){
                case ControlCodes.LogID_Main:
                    _logFileWriter.append(event.GetEventText());
                    _logFileWriter.flush();
                    break;
                case ControlCodes.LogID_Debug:
                    _debugFileWriter.append(event.GetEventText());
                    _debugFileWriter.flush();
                    break;
                case ControlCodes.LogID_Error:
                    _errorFileWriter.append(event.GetEventText());
                    _errorFileWriter.flush();
                    break;
                case ControlCodes.LogID_Chat:
                    _chatFileWriter.append(event.GetEventText());
                    _chatFileWriter.flush();
                    break;
            }
        }
        catch(Exception e){
            System.err.println(e.getMessage());
        }
    }
    private String FormatString(String toFormat){
        return "\n" + LocalDateTime.now().format(_formatter) + ": " + toFormat;
    }
    public void LogMessage(String toLog){
        _eventQueue.add(new LogEvent(ControlCodes.LogID_Main, FormatString(toLog)));
    }
    public void LogError(String toLog){
        _eventQueue.add(new LogEvent(ControlCodes.LogID_Error, FormatString(toLog)));
    }
    public void LogDebug(String toLog){
        _eventQueue.add(new LogEvent(ControlCodes.LogID_Debug, FormatString(toLog)));
    }
    public void LogChat(MatchCharacter sender, String message, Number matchID){
        String toLog = "M" + matchID + ", " + sender.GetCharacterName() + " (A" + sender.PC().GetAccountID()
                +", C" + sender.GetCharacterID() + "): " + message;
        _eventQueue.add(new LogEvent(ControlCodes.LogID_Chat, FormatString(toLog)));
    }
}
