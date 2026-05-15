import java.util.ArrayList;
import java.util.concurrent.ConcurrentHashMap;

public class ThreadMonitor  {
    private int _nextThreadID;
    private final ConcurrentHashMap<Integer, RegisteredThread> _threads;

    public ThreadMonitor(){
        _threads = new ConcurrentHashMap<>();
    }

    public int RegisterThread(RegisteredThread thread){
        int localID = _nextThreadID;
        _nextThreadID++;
        _threads.put(localID, thread);
        Main.LogMessage("Active threads: " + _threads.size());
        return localID;
    }

    public void DeregisterThread(int threadID){
        _threads.remove(threadID);
        Main.LogMessage("Active threads: " + _threads.size());
    }

    public void PrintActiveThreads(){
        System.out.println("Active threads: " + _threads.size());
        ArrayList<RegisteredThread> thds = new ArrayList<>(_threads.values());
        thds.sort(null);
        for(RegisteredThread thd : thds){
            System.out.println(thd.ToString());
        }
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
