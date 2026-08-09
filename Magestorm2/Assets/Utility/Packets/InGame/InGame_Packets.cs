using System;
using System.Text;
using UnityEngine;
public static class InGame_Packets
{
    
    public static byte[] InactivityResponsePacket() 
    {
        return new byte[] { InGame_Send.InactivityCheckResponse, MatchParams.IDinMatch };
    }
    public static byte[] PostureChangePacket(byte pmd)
    {
        return new byte[] { InGame_Send.PostureChange, MatchParams.IDinMatch, pmd};
    }
    public static byte[] TapPacket()
    {
        return new byte[] { InGame_Send.Tap, MatchParams.IDinMatch };
    }
    public static byte[] AllPlayerData()
    {
        return new byte[] {InGame_Send.AllPlayerData, MatchParams.IDinMatch};
    }
    public static byte[] UpdateLeyPacket(float newLey)
    {
        byte[] unencrypted = new byte[6];
        unencrypted[0] = InGame_Send.UpdateLey;
        unencrypted[1] = MatchParams.IDinMatch;
        BitConverter.GetBytes(newLey).CopyTo(unencrypted, 2);
        return unencrypted;
    }
    public static byte[] PlayerMovedPacket(byte controlCode, byte[] data, byte pmd, ref int packetID)
    {
        packetID++;
        byte[] unencrypted = new byte[8 + data.Length];
        unencrypted[0] = InGame_Send.PlayerMoved;
        unencrypted[1] = MatchParams.IDinMatch;
        BitConverter.GetBytes(packetID).CopyTo(unencrypted, 2);
        unencrypted[6] = pmd;
        unencrypted[7] = controlCode;
        data.CopyTo(unencrypted, 8);
        return unencrypted;
    }
    public static byte[] GenericCastPacket(byte payloadLength, byte spellID, byte spellType)
    {
        byte[] unencrypted = new byte[5 + payloadLength];
        unencrypted[0] = InGame_Send.Cast;
        unencrypted[1] = MatchParams.IDinMatch;
        unencrypted[2] = spellType;
        unencrypted[3] = spellID;
        unencrypted[4] = payloadLength;
        return unencrypted;
    }
    public static byte[] ProjectileCastPacket(byte spellID)
    {
        byte[] unencrypted = GenericCastPacket(12, spellID, ControlCodes.SpellTypes_Projectile);
        SharedFunctions.FillCameraDirectionBytes(ref unencrypted, ControlCodes.CastPayloadStartIndex);
        return unencrypted;
    }
    public static byte[] CastBoltPacket(byte spellID, byte target)
    {
        byte[] unencrypted = GenericCastPacket(13, spellID, ControlCodes.SpellTypes_Bolt);
        unencrypted[ControlCodes.CastPayloadStartIndex] = target;
        SharedFunctions.FillCameraDirectionBytes(ref unencrypted, ControlCodes.CastPayloadStartIndex + 1);
        return unencrypted;
    }
    public static byte[] DevourPacket(short castID, byte playerDevoured)
    {
        byte[] unencrypted = new byte[5];
        unencrypted[0] = InGame_Send.Devour;
        unencrypted[1] = MatchParams.IDinMatch;
        unencrypted[2] = playerDevoured;
        BitConverter.GetBytes(castID).CopyTo(unencrypted, 3);
        return unencrypted;
    }
    public static byte[] SummonPacket(byte spellID, byte playerToSummon)
    {
        byte[] unencrypted = GenericCastPacket(1, spellID, ControlCodes.SpellTypes_Summon);
        unencrypted[ControlCodes.CastPayloadStartIndex] = playerToSummon;
        return unencrypted;
    }
    public static byte[] FlagCapturedPacket(byte flagCaptured)
    {
        return new byte[] { InGame_Send.FlagCaptured, MatchParams.IDinMatch, flagCaptured };
    }
    public static byte[] FetchPlayerPacket(byte playerID)
    {
        return new byte[] { InGame_Send.FetchPlayer, MatchParams.IDinMatch, playerID };
    }
    public static byte[] FlagTakenPacket(byte flagID)
    {
        return new byte[] { InGame_Send.FlagTaken, MatchParams.IDinMatch, flagID };
    }
    public static byte[] FlagReturnedPacket(byte flagID)
    {
        return new byte[] { InGame_Send.FlagReturned, MatchParams.IDinMatch, flagID};
    }

    public static byte[] WallCastPacket(byte spellID, byte spellType)
    {
        byte[] generic = GenericCastPacket(24, spellID, spellType);
        byte[] camera = SharedFunctions.GetCameraPositionBytes(2.0f);
        camera.CopyTo(generic, ControlCodes.CastPayloadStartIndex);
        byte[] rotation = ByteUtils.Vector3ToBytes(Camera.main.transform.rotation.eulerAngles);
        rotation.CopyTo(generic, ControlCodes.CastPayloadStartIndex + 12);
        return generic;
    }
    public static byte[] LeaderboardPacket()
    {
        return new byte[] { InGame_Send.LeaderboardRequest, MatchParams.IDinMatch };
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
        return new byte[] { InGame_Send.LeaveMatch, MatchParams.IDinMatch, MatchParams.MatchTeamID};
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
        return new byte[] { InGame_Send.AdjustShrine, MatchParams.IDinMatch, shrineID};
    }
    public static byte[] BiasPoolPacket(byte poolID)
    {
        return new byte[] {InGame_Send.BiasPool, MatchParams.IDinMatch, poolID};
    }
    public static byte[] FetchShrineHealthPacket()
    {
        return new byte[] {InGame_Send.FetchShrineHealth, MatchParams.IDinMatch};
    }
    public static byte[] ChangedObjectStatePacket(byte key, byte state, byte selfReset)
    {
        return new byte[] { InGame_Send.ChangedObjectState, MatchParams.IDinMatch, key, state, selfReset };
    }
    public static byte[] MatchJoinedPacket(byte packetID)
    {
        byte[] unencrypted = new byte[11];
        unencrypted[0] = packetID;
        PlayerAccount.AccountIDBytes.CopyTo(unencrypted, 1);
        PlayerAccount.SelectedCharacter.IDBytes.CopyTo(unencrypted, 5);
        unencrypted[9] = MatchParams.IDinMatch;
        unencrypted[10] = MatchParams.MatchTeamID;
        return unencrypted;

    }
    public static byte[] ReportResistableHit(short castID)
    {
        byte[] unencrypted = new byte[4];
        unencrypted[0] = InGame_Send.ReportResistableHit;
        unencrypted[1] = MatchParams.IDinMatch;
        BitConverter.GetBytes(castID).CopyTo(unencrypted, 2);
        return unencrypted;
    }
    public static byte[] ReportHitPacket(short castID)
    {
        byte[] unencrypted = new byte[4];
        unencrypted[0] = InGame_Send.ReportHit;
        unencrypted[1] = MatchParams.IDinMatch;
        BitConverter.GetBytes(castID).CopyTo(unencrypted, 2);
        return unencrypted;
    }
    public static byte[] ReportHitByWallPacket(short castID)
    {
        byte[] unencrypted = new byte[4];
        unencrypted[0] = InGame_Send.ReportHitByWall;
        unencrypted[1] = MatchParams.IDinMatch;
        BitConverter.GetBytes(castID).CopyTo(unencrypted, 2);
        return unencrypted;
    }

    public static byte[] ReportSplashHit(short castID)
    {
        byte[] unencrypted = new byte[4];
        unencrypted[0] = InGame_Send.ReportSplash;
        unencrypted[1] = MatchParams.IDinMatch;
        BitConverter.GetBytes(castID).CopyTo(unencrypted, 2);
        return unencrypted;
    }

    public static byte[] WallHitPacket(short castID, short wallID)
    {
        byte[] unencrypted = new byte[6];
        unencrypted[0] = InGame_Send.WallHit;
        unencrypted[1] = MatchParams.IDinMatch;
        BitConverter.GetBytes(castID).CopyTo (unencrypted, 2);
        BitConverter.GetBytes(wallID).CopyTo(unencrypted, 4);
        return unencrypted;
    }

    public static byte[] WallAndSigilRequest()
    {
        return new byte[] { InGame_Send.RequestWallsAndSigils, MatchParams.IDinMatch};
    }
    public static byte[] UpdateSlots(byte[] slots)
    {
        byte[] unencrypted = new byte[slots.Length + 2];
        unencrypted[0] = InGame_Send.UpdateSlots;
        unencrypted[1] = MatchParams.IDinMatch;
        Array.Copy(slots, 0, unencrypted, 2, slots.Length);
        return unencrypted;
    }
    public static byte[] HitScanPacket(byte spellID, Vector3 pointHit)
    {
        byte[] unencrypted = GenericCastPacket(12, spellID, ControlCodes.SpellTypes_HitScan);
        ByteUtils.InsertVector3AtIndex(unencrypted, pointHit, ControlCodes.CastPayloadStartIndex);
        return unencrypted;
    }
    public static byte[] PlacedSigilPacket(byte spellID, byte spellType)
    {
        byte[] unencrypted = GenericCastPacket(13, spellID, spellType);
        unencrypted[ControlCodes.CastPayloadStartIndex] = MatchParams.MatchTeamID;
        Array.Copy(SharedFunctions.GetCameraPositionBytes(1.25f), 0, unencrypted, ControlCodes.CastPayloadStartIndex + 1, 12);
        return unencrypted;
    }

    public static byte[] TriggeredSigilPacket(short castID)
    {
        byte[] unencrypted = new byte[4];
        unencrypted[0] = InGame_Send.TriggeredSigil;
        unencrypted[1] = MatchParams.IDinMatch;
        BitConverter.GetBytes(castID).CopyTo(unencrypted, 2);
        return unencrypted;
    }
}
