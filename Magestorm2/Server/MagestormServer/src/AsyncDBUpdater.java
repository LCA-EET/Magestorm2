import java.sql.Connection;
import java.util.concurrent.LinkedBlockingQueue;

public class AsyncDBUpdater extends RegisteredThread{
    private final LinkedBlockingQueue<AsyncDBUpdate> _toProcess;
    private final Connection _persistentConnection;
    public AsyncDBUpdater(){
        _toProcess = new LinkedBlockingQueue<>();
        _persistentConnection = Database.DBConnection();
        new RegisteredThread(this).start();
    }
    public void run(){
        while(!_terminated){
            try{
                _toProcess.take().ProcessRequest(_persistentConnection);
            }
            catch(InterruptedException ie){
                _terminated = true;
            }
            catch(Exception ex){
                Main.LogError("AsyncDBUpdater.run(): " + ex.getMessage());
                Main.LogStackTrace(ex);
            }
        }
        Deregister();
    }
    public void AddToQueue(AsyncDBUpdate update){
        _toProcess.add(update);
    }
}
