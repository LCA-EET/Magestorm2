using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class UIAudio
{

    public static void PlayButtonPress()
    {
        ComponentRegister.AudioPlayer.PlayButtonPress();
    }

    public static void PlayBiasSFX()
    {
        ComponentRegister.AudioPlayer.PlayBiasSound();
    }
}
