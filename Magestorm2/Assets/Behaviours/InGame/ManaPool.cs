using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ManaPool : MonoBehaviour
{
    public byte PoolID;
    private byte _poolPower;

    public void Awake()
    {
        
    }
    public void Start()
    {
        _poolPower = Match.RegisterPool(this);
        Debug.Log("Pool ID: " + PoolID + ", Power: " + _poolPower);
    }
    public void Update()
    {
        
    }
}
