import java.util.ArrayList;
import java.util.HashSet;

public class Effect {
    private final String _effectName;
    private final byte _effectCode, _effectTarget, _notificationCode, _effectType, _duration, _element, _vfxCode;
    private final float _percentOverTime;

    private final HashSet<Byte> _effectsPrevented;
    private final ArrayList<Byte> _effectsCancelled;
    public Effect(byte effectID, String effectName, long effectsPrevented, long effectsCancelled, byte[] attrib){
        _effectCode = effectID;
        _effectName = effectName;
        _effectsPrevented = new HashSet<>();
        _effectsCancelled = new ArrayList<>();
        SharedFunctions.FillEffects(effectsPrevented, _effectsPrevented);
        SharedFunctions.FillEffects(effectsCancelled, _effectsCancelled);
        _effectTarget = attrib[0];
        _notificationCode = attrib[1];
        _effectType = attrib[2];
        _duration = attrib[3];
        _percentOverTime = attrib[4] / 100.0f;
        _element = attrib[5];
        _vfxCode = attrib[6];
    }
    public ArrayList<Byte> GetEffectsCancelled(){
        return _effectsCancelled;
    }
    public byte GetEffectNotificationCode(){
        return _notificationCode;
    }
    public byte GetEffectCode(){
        return _effectCode;
    }
    public boolean IsEffectPrevented(byte effectCode){
        return _effectsPrevented.contains(effectCode);
    }
    public byte GetEffectType(){
        return _effectType;
    }
    public byte GetDuration(){
        return _duration;
    }
    public byte GetEffectTarget(){
        return _effectTarget;
    }
    public float PercentOverTime(){
        return _percentOverTime;
    }
    public byte GetVFXCode(){
        return _vfxCode;
    }
}
