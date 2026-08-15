public class Shrine {
    private final byte _teamID;
    private final DeathMatch _owningMatch;
    private byte _shrineHealth;

    public Shrine(byte teamID, DeathMatch owningMatch){
        _teamID = teamID;
        _shrineHealth = 100;
        _owningMatch = owningMatch;
    }
    public byte GetShrineTeam(){
        return _teamID;
    }
    public void AdjustShrineHealth(MatchCharacter adjuster){
        byte amount = (byte)GameUtils.DiceRoll(30,1);
        amount *= (byte)(adjuster.GetClass().GetBiasMultiplier() * (byte)(_teamID==adjuster.GetTeamID()?1:-1));
        short newHealth = _shrineHealth;
        newHealth += amount;
        if(newHealth < 0){
            newHealth = 0;
        }
        else if(newHealth > 100){
            newHealth = 100;
        }
        SetShrineHealth((byte)newHealth, adjuster.GetIDinMatch());
    }

    public void SetShrineHealth(byte newHealth, byte adjusterID){
        _shrineHealth = newHealth;
        if(newHealth == 100 || newHealth == 0){
            _owningMatch.CheckVictoryCondition();
        }
        _owningMatch.SendToAll(Packets.ShrineAdjustmentPacket(_shrineHealth, _teamID, adjusterID));
    }

    public byte ShrineHealth(){
        return _shrineHealth;
    }
    public boolean IsAlive(){
        return _shrineHealth > 0;
    }
    public static byte AdjustmentChance(byte classCode) {
        if(classCode == CharacterClass.Cleric){
            return 50;
        }
        return 25;
    }
}
