public class Shrine {
    private final byte _teamID;
    private final Match _owningMatch;
    private byte _shrineHealth;

    public Shrine(byte teamID, DeathMatch owningMatch){
        _teamID = teamID;
        _shrineHealth = 100;
        _owningMatch = owningMatch;
    }

    public void AdjustShrineHealth(MatchCharacter adjuster){
        byte amount = (byte)GameUtils.DiceRoll(30,1);
        amount *= (byte)(adjuster.GetClass().ShrineMultiplier() * (byte)(_teamID==adjuster.GetTeamID()?1:-1));
        short newHealth = _shrineHealth;
        newHealth += amount;
        if(newHealth < 0){
            newHealth = 0;
        }
        if(newHealth > 100){
            newHealth = 100;
        }
        _shrineHealth = (byte)newHealth;
        _owningMatch.SendToAll(Packets.ShrineAdjustmentPacket(_shrineHealth, _teamID, adjuster.GetIDinMatch()));
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
