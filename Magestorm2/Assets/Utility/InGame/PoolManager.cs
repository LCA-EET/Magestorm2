using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class PoolManager
{
    private static Dictionary<byte, ManaPool> _pools;
    private static Dictionary<byte, InitialPoolData> _initialPoolData;
    private static Level _level;
    public static void Init()
    {
        _pools = new Dictionary<byte, ManaPool>();
        _level = LevelData.GetLevel(MatchParams.SceneID);
        byte[] poolData = MatchParams.GetPoolData();
        _initialPoolData = new Dictionary<byte, InitialPoolData>();
        for (int i = 0; i < poolData.Length; i += 3)
        {
            _initialPoolData.Add(poolData[i], new InitialPoolData(poolData[i + 1], poolData[i + 2]));
        }
    }

    public static byte RegisterPool(ManaPool toRegister)
    {
        _pools.Add(toRegister.PoolID, toRegister);
        InitialPoolData poolData = _initialPoolData[toRegister.PoolID];
        toRegister.SetBiasAmount(poolData.BiasAmount, poolData.BiasedToward);
        return _level.GetPoolPower(toRegister.PoolID);
    }

    public static void PoolBiased(byte biaserID, byte poolID, byte teamID, byte biasAmount)
    {
        if (_pools.ContainsKey(poolID))
        {
            _pools[poolID].BiasPool(biasAmount, (Team)teamID, biaserID);
        }
    }
}

