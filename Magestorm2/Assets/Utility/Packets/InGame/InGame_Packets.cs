using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public static class InGame_Packets
{
    public static byte[] InactivityResponsePacket() { return OpCodePlusID(InGame_Send.InactivityCheckResponse); }

    public static byte[] PlayerMovedPacket(byte controlCode, byte[] data)
    {
        byte[] unencrypted = new byte[3 + data.Length];
        unencrypted[0] = InGame_Send.PlayerMoved;
        unencrypted[1] = MatchParams.IDinMatch;
        unencrypted[2] = controlCode;
        data.CopyTo(unencrypted, 3);
        return unencrypted;
    }
    
    public static byte[] FlagCapturedPacket(byte flagCaptured)
    {
        byte[] unencrypted = new byte[3];
        unencrypted[0] = InGame_Send.FlagCaptured;
        unencrypted[1] = MatchParams.IDinMatch;
        unencrypted[2] = flagCaptured;
        return unencrypted;
    }
    public static byte[] FetchPlayerPacket(byte playerID)
    {
        byte[] unencrypted = new byte[3];
        unencrypted[0] = InGame_Send.FetchPlayer;
        unencrypted[1] = MatchParams.IDinMatch;
        unencrypted[2] = playerID;
        return unencrypted;
    }
    public static byte[] FlagTakenPacket(byte flagID)
    {
        byte[] unencrypted = new byte[3];
        unencrypted[0] = InGame_Send.FlagTaken;
        unencrypted[1] = MatchParams.IDinMatch;
        unencrypted[2] = flagID;
        return unencrypted;
    }
    public static byte[] FlagReturnedPacket(byte flagID)
    {
        byte[] unencrypted = new byte[3];
        unencrypted[0] = InGame_Send.FlagReturned;
        unencrypted[1] = MatchParams.IDinMatch;
        unencrypted[2] = flagID;
        return unencrypted;
    }
    public static byte[] QuitGamePacket()
    {
        byte[] unencrypted = new byte[7];
        unencrypted[0] = InGame_Send.QuitGame;
        unencrypted[1] = MatchParams.IDinMatch;
        unencrypted[2] = MatchParams.MatchTeamID;
        PlayerAccount.AccountIDBytes.CopyTo(unencrypted, 3);
        return unencrypted;
    }
    public static byte[] LeaveMatchPacket()
    {
        byte[] unencrypted = new byte[3];
        unencrypted[0] = InGame_Send.LeaveMatch;
        unencrypted[1] = MatchParams.IDinMatch;
        unencrypted[2] = MatchParams.MatchTeamID;
        return unencrypted;
    }
    public static byte[] BroadcastMessagePacket(string messageText)
    {
        byte[] utf8 = Encoding.UTF8.GetBytes(messageText);
        byte[] unencrypted = new byte[1 + 1 + 4 + utf8.Length];
        unencrypted[0] = InGame_Send.BroadcastMessage;
        unencrypted[1] = MatchParams.IDinMatch;
        BitConverter.GetBytes(utf8.Length).CopyTo(unencrypted, 2);
        utf8.CopyTo(unencrypted, 6);
        return unencrypted;
    }
    public static byte[] AdjustShrinePacket (byte shrineID)
    {
        byte[] unencrypted = new byte[3];
        unencrypted[0] = InGame_Send.AdjustShrine;
        unencrypted[1] = MatchParams.IDinMatch;
        unencrypted[2] = shrineID;
        return unencrypted;
    }
    public static byte[] BiasPoolPacket(byte poolID)
    {
        byte[] unencrypted = new byte[3];
        unencrypted[0] = InGame_Send.BiasPool;
        unencrypted[1] = MatchParams.IDinMatch;
        unencrypted[2] = poolID;
        return unencrypted;
    }
    public static byte[] FetchShrineHealthPacket()
    {
        byte[] unencrypted = new byte[2];
        unencrypted[0] = InGame_Send.FetchShrineHealth;
        unencrypted[1] = MatchParams.IDinMatch;
        return unencrypted;
    }
    public static byte[] ChangedObjectStatePacket(byte key, byte state, byte selfReset)
    {
        byte[] unencrypted = new byte[5];
        unencrypted[0] = InGame_Send.ChangedObjectState;
        unencrypted[1] = MatchParams.IDinMatch;
        unencrypted[2] = key;
        unencrypted[3] = state;
        unencrypted[4] = selfReset;
        return unencrypted;
    }
    public static byte[] MatchJoinedPacket()
    {
        byte[] unencrypted = new byte[11];
        unencrypted[0] = InGame_Send.JoinedMatch;
        PlayerAccount.AccountIDBytes.CopyTo(unencrypted, 1);
        PlayerAccount.SelectedCharacter.IDBytes.CopyTo(unencrypted, 5);
        unencrypted[9] = MatchParams.IDinMatch;
        unencrypted[10] = MatchParams.MatchTeamID;
        return unencrypted;

    }
    
    public static byte[] OpCodePlusID(byte opCode)
    {
        byte[] unencrypted = new byte[2];
        unencrypted[0] = opCode;
        unencrypted[1]= MatchParams.IDinMatch;
        return unencrypted;
    }
}
