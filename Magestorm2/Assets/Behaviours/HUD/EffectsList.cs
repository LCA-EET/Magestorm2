using System.Collections.Generic;
using System.Collections;
using UnityEngine;
namespace Magus
{

    public class EffectsList : MonoBehaviour
    {
        public EffectDisplay[] EffectDisplays;
        void Start()
        {
            ComponentRegister.EffectsList = this;
        }
        public void RefreshEffects(byte[] activeEffectsBytes)
        {
            BitArray bitArray = new BitArray(activeEffectsBytes);
            List<byte> activeEffects = new List<byte>();
            byte index = 0;
            for (byte b = 0; b < bitArray.Count; b++)
            {
                if (bitArray[b])
                {
                    activeEffects.Add(b);
                }
            }
            while (index < activeEffects.Count && index < 8)
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
            return IconLibrary.GetSpriteSet(effectIndex);
        }
    }

}