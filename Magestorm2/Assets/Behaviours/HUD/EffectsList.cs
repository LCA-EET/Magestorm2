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
        switch (effectIndex)
        {
            case ControlCodes.EffectCode_Bleeding:
                return IconLibrary.GetSpriteSet("bleed");
            case ControlCodes.EffectCode_Burning:
                return IconLibrary.GetSpriteSet("burn");
            case ControlCodes.EffectCode_EarthShield:
                return IconLibrary.GetSpriteSet("earthshield");
            case ControlCodes.EffectCode_ElectricShield:
                return IconLibrary.GetSpriteSet("elecshield");
            case ControlCodes.EffectCode_Entangle:
                return IconLibrary.GetSpriteSet("entangle");
            case ControlCodes.EffectCode_FireShield:
                return IconLibrary.GetSpriteSet("fireshield");
            case ControlCodes.EffectCode_Freezing:
                return IconLibrary.GetSpriteSet("freeze");
            case ControlCodes.EffectCode_Haste:
                return IconLibrary.GetSpriteSet("haste");
            case ControlCodes.EffectCode_IceShield:
                return IconLibrary.GetSpriteSet("iceshield");
            case ControlCodes.EffectCode_Prayer:
                return IconLibrary.GetSpriteSet("prayer");
            case ControlCodes.EffectCode_Shocked:
                return IconLibrary.GetSpriteSet("shock");
            case ControlCodes.EffectCode_Slow:
                return IconLibrary.GetSpriteSet("slow");
            
        }
        return null;
    }
}
