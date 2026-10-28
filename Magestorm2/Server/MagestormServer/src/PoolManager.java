import java.util.concurrent.ConcurrentHashMap;

public class PoolManager {
    private ConcurrentHashMap<Byte, Pool> _matchPools;
    private final Match _owningMatch;
    private byte[] _biasData;
    private boolean _biasChange;

    public PoolManager(Match owningMatch){
        _biasChange = true;
        _matchPools = new ConcurrentHashMap<>();
        _owningMatch = owningMatch;
        _matchPools= new ConcurrentHashMap<>();
    }
    public void BiasPool(MatchCharacter verified, byte poolID, RemoteClient rc) {
        if(!_matchPools.containsKey(poolID)){
            _matchPools.put(poolID, new Pool(_owningMatch, poolID));
        }
        if(verified.IsAlive()){
            short diceRoll = GameUtils.DiceRoll(100, 1);
            byte biasChance = verified.GetClass().GetPoolBiasChance();
            if(biasChance >= diceRoll){
                _matchPools.get(poolID).Bias(verified);
                _biasChange = true;
            }
            else{
                _owningMatch.SendToPlayer(Packets.PoolBiasFailurePacket(), verified);
            }
        }
    }

    public byte[] GetPoolBiasData(){
        if(_biasChange){
            _biasData = new byte[1 + (_matchPools.size() * 3)];
            _biasData[0] = (byte)_matchPools.size();
            int trIndex = 1;
            for(Pool pool : _matchPools.values() ){
                _biasData[trIndex] = pool.GetPoolID();
                _biasData[trIndex + 1] = pool.GetPoolTeam();
                _biasData[trIndex + 2] = pool.GetPoolBiasAmount();
                trIndex += 3;
            }
            _biasChange = false;
        }
        return _biasData;
    }
}
