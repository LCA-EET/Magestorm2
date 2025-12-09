using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class EffectsList : MonoBehaviour
{
    public EffectDisplay[] EffectDisplays;
    void Start()
    {
        ComponentRegister.EffectsList = this;
    }

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
            EffectDisplays[index].Show(true, GetSpriteSet(activeEffects[index]));            
            index++;
        }
        while (index < 8)
        {
            EffectDisplays[index].Hide();
            index++;
        }
    }
    private SpriteSet GetSpriteSet(byte effectIndex)
    {
        EffectCode code = (EffectCode)effectIndex;
        switch (code)
        {
            case EffectCode.Bleeding:
                return IconLibrary.GetSpriteSet("bleed");
            case EffectCode.Burning:
                return IconLibrary.GetSpriteSet("burn");
            case EffectCode.EarthShield:
                return IconLibrary.GetSpriteSet("earthshield");
            case EffectCode.ElectricShield:
                return IconLibrary.GetSpriteSet("elecshield");
            case EffectCode.Entangle:
                return IconLibrary.GetSpriteSet("entangle");
            case EffectCode.FireShield:
                return IconLibrary.GetSpriteSet("fireshield");
            case EffectCode.Freezing:
                return IconLibrary.GetSpriteSet("freeze");
            case EffectCode.Haste:
                return IconLibrary.GetSpriteSet("haste");
            case EffectCode.IceShield:
                return IconLibrary.GetSpriteSet("iceshield");
            case EffectCode.Prayer:
                return IconLibrary.GetSpriteSet("prayer");
            case EffectCode.Shocked:
                return IconLibrary.GetSpriteSet("shock");
            case EffectCode.Slow:
                return IconLibrary.GetSpriteSet("slow");
            
        }
        return null;
    }
}
