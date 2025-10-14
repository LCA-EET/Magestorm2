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
        InitializePools();
    }

    private void InitializePools(){
        _matchPools= new ConcurrentHashMap<>();
        byte[] poolBytes = GameServer.GetPoolData(_owningMatch.GetSceneID());
        for(int i = 0; i < poolBytes.length; i+=2){
            byte poolID = poolBytes[i];
            byte poolPower = poolBytes[i+1];
            Pool toAdd = new Pool(_owningMatch, poolID, poolPower);
            _matchPools.put(poolID, toAdd);
        }
    }

    public void BiasPool(byte biaserID, byte poolID, RemoteClient rc) {
        if(_matchPools.containsKey(poolID)){
            MatchCharacter biaser = _owningMatch.GetMatchCharacter(biaserID);
            if(biaser.IsAlive()){
                short diceRoll = GameUtils.DiceRoll(100, 1);
                if(Pool.BiasChance(biaser.GetClassCode()) >= diceRoll){
                    _matchPools.get(poolID).Bias(biaser);
                    _biasChange = true;
                }
                else{
                    _owningMatch.SendToPlayer(Packets.PoolBiasFailurePacket(), biaser);
                }
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
