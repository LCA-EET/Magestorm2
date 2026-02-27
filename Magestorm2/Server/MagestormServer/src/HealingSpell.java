public class HealingSpell extends CastSpell{
    protected short _healing;

    public HealingSpell(MatchCharacter caster, short castID, Spell baseReference){
        super(caster, castID, baseReference);
    }

    private void CalculateHeal(){
        byte minRoll = _baseReference.GetMinHealPerRoll();
        byte maxRoll = _baseReference.GetMaxHealPerRoll();
        _healing = GameUtils.DiceRoll(minRoll, maxRoll, _baseReference.GetNumRolls());
    }

    public short GetHealAmount(){
        if(_healing == 0){
            CalculateHeal();
        }
        return _healing;
    }

    @Override
    public void ProcessSpell(MatchCharacter affectedPlayer){
        if(affectedPlayer.GetIDinMatch() == _casterReference.GetIDinMatch()
            || (affectedPlayer.GetTeamID() == _castingTeam && _castingTeam != MatchTeam.Neutral)){
            Main.LogMessage("PriorHP: " + affectedPlayer.GetCurrentHP());
            affectedPlayer.Heal(GetHealAmount(), _casterReference);
            Main.LogMessage("CurrentHP: " + affectedPlayer.GetCurrentHP());
        }
    }
}
