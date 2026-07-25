using System.Collections;
using UnityEngine;
public class PMDByte
{
    private static byte idxPostureLSB = 0;
    private static byte idxMoving = 2;
    private static byte idxDirection = 3;
    private static byte idxRunning = 4;
    private byte _postureByte = Postures.Standing;
    private BitArray _bitArray;
    public PMDByte()
    {
        _bitArray = new BitArray(8, false);
    }
    public void SetPMD(byte pmd)
    {
        
        _bitArray = new BitArray(new byte[] { pmd });
        if (IsStanding)
        {
            _postureByte = Postures.Standing;
        }
        else if(IsCrouched)
        {
            _postureByte = Postures.Crouched;
        }
        else if (IsAirborne)
        {
            _postureByte = Postures.Airborne;
        }
        else
        {
            _postureByte = Postures.Jump;
        }
        Debug.Log("SetPMD " + IsMoving + " " + IsMovingForward + " " + IsMovingBackward);
    }
    public void SetMoving(bool moving)
    {
        _bitArray[idxMoving] = moving;
    }
    public void SetDirection(bool forward)
    {
        _bitArray[idxDirection] = forward;
    }
    public void SetRunning(bool running)
    {
        _bitArray[idxRunning] = running;
    }

    public void SetMovingAndDirection(bool moving, bool forward)
    {
        SetMoving(moving);
        SetDirection(forward);
    }
    public byte Posture
    {
        get { 
            return _postureByte;
        }
    }

    public void SetLocalPosture(byte posture)
    {
        _postureByte = posture;
        bool msb = false; // 0, 00 (Standing)
        bool lsb = false;
        switch (posture)
        {
            case Postures.Crouched: // 1, 01
                msb = false;
                lsb = true;
                break;
            case Postures.Airborne: // 2, 10
                msb = true;
                lsb = false;
                break;
            case Postures.Jump:     //3, 11
                msb = true;
                lsb = true;
                break;
        }
        _bitArray[idxPostureLSB + 1] = msb;
        _bitArray[idxPostureLSB] = lsb;
        Debug.Log("SLP " + lsb + " " + msb);
    }
    public bool IsStanding
    {
        get
        {
            return !_bitArray[idxPostureLSB + 1] && !_bitArray[idxPostureLSB];
        }
    }

    public bool IsCrouched
    {
        get
        {
            return !_bitArray[idxPostureLSB + 1] && _bitArray[idxPostureLSB];
        }
    }

    public bool IsAirborne
    {
        get
        {
            return _bitArray[idxPostureLSB + 1] && !_bitArray[idxPostureLSB];
        }
    }

    public bool IsJumping
    {
        get
        {
            return _bitArray[idxPostureLSB + 1] && _bitArray[idxPostureLSB];
        }
    }

    public bool IsMovingForward
    {
        get
        {
            return _bitArray[idxMoving] && _bitArray[idxDirection];
        }
    }

    public bool IsMovingBackward
    {
        get
        {
            return _bitArray[idxMoving] && !_bitArray[idxDirection];
        }
    }

    public bool IsMoving
    {
        get
        {
            return _bitArray[idxMoving];
        }
    }
    public bool IsRunning
    {
        get
        {
            return _bitArray[idxRunning];
        }
    }
    public byte ToByte()
    {
        byte[] byteArray = new byte[1]; // Ensure enough space for all bits
        _bitArray.CopyTo(byteArray, 0);
        return byteArray[0];
    }
}
