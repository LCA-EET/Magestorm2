import java.util.ArrayList;
import java.util.Collection;
import java.util.concurrent.ConcurrentHashMap;

public class TimedObjectCollection {
    private final ConcurrentHashMap<Short, ITimedObject> _collection;
    private final long _interval;
    private long _elapsed;
    private final ArrayList<Short> _expiredObjects;
    public TimedObjectCollection(long interval){
        _collection = new ConcurrentHashMap<>();
        _interval = interval;
        _elapsed = 0;
        _expiredObjects = new ArrayList<>();
    }

    public void AddTimedObject(short id, ITimedObject timedObject)
    {
        _collection.put(id, timedObject);
    }

    public ITimedObject GetTimedObject(short id){
        return _collection.get(id);
    }

    public Collection<ITimedObject> GetObjects(){
        return _collection.values();
    }
    public boolean IsEmpty(){
        return _collection.isEmpty();
    }
    public void ClearExpirations(){
        _expiredObjects.clear();
    }
    public ArrayList<Short> GetExpiredObjects(){
        return _expiredObjects;
    }
    public boolean CountdownObjects(long msElapsed){
        _elapsed += msElapsed;
        if(_elapsed >= _interval){
            _elapsed -= _interval;
            if(!_collection.isEmpty()){
                for(ITimedObject to : _collection.values()){
                    if(to.ReduceDuration(msElapsed)){
                        _expiredObjects.add(to.TimedObjectID());
                    }
                }
                for(Short toID : _expiredObjects){
                    _collection.remove(toID);
                }
            }
        }
        return !_expiredObjects.isEmpty();
    }
}
