using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class InGame_Packets
{
    public static byte[] InactivityResponsePacket() { return OpCodePlusID(InGame_Send.InactivityCheckResponse); }
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
    public static byte[] FetchShrineHealthPacket()
    {
        byte[] unencrypted = new byte[2];
        unencrypted[0] = InGame_Send.FetchShrineHealth;
        unencrypted[1] = MatchParams.IDinMatch;
        return unencrypted;
    }
    public static byte[] ChangedObjectStatePacket(byte key, byte state)
    {
        byte[] unencrypted = new byte[4];
        unencrypted[0] = InGame_Send.ChangedObjectState;
        unencrypted[1] = MatchParams.IDinMatch;
        unencrypted[2] = key;
        unencrypted[3] = state;
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
