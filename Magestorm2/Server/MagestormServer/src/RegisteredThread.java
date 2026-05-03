public class RegisteredThread extends Thread{
    private int _threadID;
    protected boolean _terminated;

    public RegisteredThread(){
        super();
    }

    public RegisteredThread(Runnable toRun){
        super(toRun);
        _terminated = false;
    }
    public int GetThreadID(){
        return _threadID;
    }

    public boolean IsTerminated(){
        return _terminated;
    }

    protected void Deregister(){
        Main.ThreadMonitor.DeregisterThread(_threadID);
    }

    @Override public void start(){
        _threadID = Main.ThreadMonitor.RegisterThread(this);
        super.start();
    }
}
