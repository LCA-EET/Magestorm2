import java.util.concurrent.LinkedBlockingQueue;

public class AsyncRequestProcessor extends RegisteredThread {
    protected UDPProcessor _udp;
    protected LinkedBlockingQueue<AsyncRequest> _toProcess;
    protected String _desc;
    protected RemoteClient _remote;
    protected byte[] _decrypted;
    public AsyncRequestProcessor(String desc, UDPProcessor owner){
        _udp = owner;
        _desc = desc;
        _toProcess = new LinkedBlockingQueue<>();
    }
    public void EnqueueRequest(AsyncRequest request){
        _toProcess.add(request);
    }
    protected void PreProcess(AsyncRequest request){
        _remote = request.GetSender();
        _decrypted = request.GetBytes();
    }
    public void ProcessPacket(AsyncRequest request){
        PreProcess(request);
    }

    @Override
    public void run(){
        Register(_desc);
        while(!_terminated){
            try{
                ProcessPacket(_toProcess.take());
            }
            catch(InterruptedException ex){
                OnInterrupt();
            }
            catch(Exception ex){
                Main.LogError(ex.getMessage());
                Main.LogStackTrace(ex);
            }
        }
        Deregister();
    }
}
