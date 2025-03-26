using NUnit.Framework;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class EffectsList : MonoBehaviour
{
    public EffectDisplay[] EffectDisplays;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ComponentRegister.EffectsList = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void RefreshEffects(byte[] activeEffectsBytes)
    {
        BitArray bitArray = new BitArray(activeEffectsBytes);
        List<byte> activeEffects = new List<byte>();
        byte index = 0;
        for(byte b = 0; b < bitArray.Count; b++)
        {
            if (bitArray[b])
            {
                activeEffects.Add(b);
            }
        }
        while(index < activeEffects.Count && index < 8)
        {
            EffectDisplays[index].Show(true, activeEffects[index]);            
            index++;
        }
        while (index < 8)
        {
            EffectDisplays[index].Show(false, 0);
            index++;
        }
    }
}
