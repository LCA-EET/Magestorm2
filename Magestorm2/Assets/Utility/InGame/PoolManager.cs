using System;
using System.Collections.Generic;

public static class PoolManager
{
    private static Dictionary<byte, ManaPool> _pools;
    private static Dictionary<byte, InitialPoolData> _initialPoolData;
    private static Level _level;
    private static byte[] _poolData;
    public static void Init(byte[] decrypted, int index)
    {
        int numPools = decrypted[index];
        index++;
        _poolData = new byte[numPools * 3];
        Array.Copy(decrypted, index, _poolData, 0, _poolData.Length);
        index += _poolData.Length;
        _level = LevelData.GetLevel(MatchParams.SceneID);
        _initialPoolData = new Dictionary<byte, InitialPoolData>();
        for (int i = 0; i < _poolData.Length; i += 3)
        {
            _initialPoolData.Add(_poolData[i], new InitialPoolData(_poolData[i + 1], _poolData[i + 2]));
        }
    }
    public static void InitializePools()
    {
        _pools = new Dictionary<byte, ManaPool>();
    }
    public static void RegisterPool(ManaPool toRegister)
    {
        _pools.Add(toRegister.PoolID, toRegister);
        if(_initialPoolData != null)
        {
            if (_initialPoolData.ContainsKey(toRegister.PoolID))
            {
                InitialPoolData poolData = _initialPoolData[toRegister.PoolID];
                toRegister.SetBiasAmount(poolData.BiasAmount, poolData.BiasedToward);
            }
        }
        else
        {
            toRegister.SetBiasAmount(0, Team.Neutral);
        }
    }

    public static void PoolBiased(byte biaserID, byte poolID, byte teamID, byte biasAmount)
    {
        if (_pools.ContainsKey(poolID))
        {
            _pools[poolID].BiasPool(biasAmount, (Team)teamID, biaserID);
        }
    }
}

