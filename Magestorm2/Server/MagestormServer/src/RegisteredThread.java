public class RegisteredThread extends Thread implements Comparable<RegisteredThread>{
    private int _threadID;
    protected boolean _terminated;
    private String _descriptor, _formattedDT;
    public RegisteredThread(){
        super();
    }

    public RegisteredThread(Runnable toRun){
        super(toRun);
    }
    public int GetThreadID(){
        return _threadID;
    }

    public boolean IsTerminated(){
        return _terminated;
    }

    protected void Register(String desc){
        _formattedDT = SharedFunctions.GetDateTimeString();
        _threadID = Main.ThreadMonitor.RegisterThread(this);
        _terminated = false;
        _descriptor = desc;
        Main.LogMessage("Registered thread " + _threadID + " (" + _descriptor + ")");
    }

    protected void Deregister(){
        Main.ThreadMonitor.DeregisterThread(_threadID);
        Main.LogMessage("Deregistered thread " + _threadID + " (" + _descriptor + ")");
    }

    public String ToString(){
        return _threadID + ": " + _descriptor + ", started on " + _formattedDT;
    }

    @Override
    public int compareTo(RegisteredThread o) {
        return Integer.compare(_threadID, o.GetThreadID());
    }
}
