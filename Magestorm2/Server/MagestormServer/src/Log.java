import java.io.FileWriter;
import java.io.IOException;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;
import java.util.concurrent.ConcurrentLinkedQueue;

public class Log extends Thread{
    private final DateTimeFormatter _formatter;
    private final ConcurrentLinkedQueue<String> _debugQueue;
    private final ConcurrentLinkedQueue<String> _messageQueue;
    private final ConcurrentLinkedQueue<String> _errorQueue;
    private final FileWriter _logFileWriter, _errorFileWriter, _debugFileWriter;
    public Log(String logFile, String errorFile, String debugFile){
        _formatter = DateTimeFormatter.ofPattern("yyyy-MM-dd HH:mm:ss");
        try {
            _logFileWriter = new FileWriter(logFile, true);
            _errorFileWriter = new FileWriter(errorFile, true);
            _debugFileWriter = new FileWriter(debugFile, true);
        } catch (IOException e) {
            throw new RuntimeException(e);
        }
        _messageQueue = new ConcurrentLinkedQueue<>();
        _errorQueue = new ConcurrentLinkedQueue<>();
        _debugQueue = new ConcurrentLinkedQueue<>();
    }
    public void run(){
        Main.LogMessage("Log started.");
        Main.LogError("Log started.");
        Main.LogDebug("Log started.");
        while(Main.Running){
            try{
                Thread.sleep(1000);
                ProcessQueue(_messageQueue, _logFileWriter);
                ProcessQueue(_errorQueue, _errorFileWriter);
                ProcessQueue(_debugQueue, _debugFileWriter);
            }
            catch(Exception e){
                System.err.println(e.getMessage());
            }
        }
        try {
            _logFileWriter.close();
            _errorFileWriter.close();
        } catch (IOException e) {
            System.err.println(e.getMessage());
        }
    }
    public void WriteNow(){
        ProcessQueue(_messageQueue, _logFileWriter);
        ProcessQueue(_errorQueue, _errorFileWriter);
        ProcessQueue(_debugQueue, _debugFileWriter);
    }
    private void ProcessQueue(ConcurrentLinkedQueue toProcess, FileWriter writer){
        while(!toProcess.isEmpty()){
            String message = toProcess.remove().toString();
            try {
                writer.append(message);
            } catch (IOException e) {
                System.err.println(e.getMessage());
            }
        }
        try{
            writer.flush();
        }
        catch(Exception e){
            System.err.println(e.getMessage());
        }
    }
    private String FormatString(String toFormat){
        return "\n" + LocalDateTime.now().format(_formatter) + ": " + toFormat;
    }
    public void LogMessage(String toLog){
        _messageQueue.add(FormatString(toLog));
    }
    public void LogError(String toLog){
        _errorQueue.add(FormatString(toLog));
    }
    public void LogDebug(String toLog){
        _debugQueue.add(FormatString(toLog));
    }
}
