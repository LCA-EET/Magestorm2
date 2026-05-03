import java.util.concurrent.ConcurrentHashMap;

public class ThreadMonitor  {
    private int _nextThreadID;
    private final ConcurrentHashMap<Integer, RegisteredThread> _threads;

    public ThreadMonitor(){
        _threads = new ConcurrentHashMap<>();
    }

    public int RegisterThread(RegisteredThread thread){
        _nextThreadID++;
        _threads.put(_nextThreadID, thread);
        return _nextThreadID;
    }

    public void DeregisterThread(int threadID){
        _threads.remove(threadID);
    }

    public void InterruptAllThreads(){
        for(RegisteredThread thread : _threads.values()){
            try{
                thread.interrupt();
                System.out.println("Interrupted thread " + thread.GetThreadID());
            }
            catch(Exception ex){
                System.out.println("Failed to interrupt thread " + thread.GetThreadID() + ". " + ex.getMessage());
            }
        }
    }
}
