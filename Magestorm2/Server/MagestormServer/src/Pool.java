public class Pool {
    private byte _bias;
    private byte _team;
    private byte _id;
    private byte _poolPower;
    private final Match _owningMatch;

    public Pool(Match owningMatch, byte id, byte poolPower){
        _team = MatchTeam.Neutral;
        _id = id;
        _poolPower = poolPower;
        _owningMatch = owningMatch;
    }

    public void Bias(MatchCharacter biaser){
        byte amount = (byte)GameUtils.DiceRoll(30,1);
        amount *= (byte)(biaser.GetClass().BiasMultiplier() * (byte)(_team==biaser.GetTeamID()?1:-1));
        _bias += amount;
        if(_bias < 0){
            byte difference = (byte)Math.abs(_bias);
            if(biaser.GetClass().IsMagician()){
                _bias = difference;
                _team = biaser.GetTeamID();
            }
            else{
                _bias = 0;
                _team = MatchTeam.Neutral;
            }
        }
        if(_bias > 100){
            _bias = 100;
        }
        _owningMatch.SendToAll(Packets.PoolBiasPacket(_id, _bias, _team, biaser.GetIDinMatch()));
    }

    public byte GetPoolID(){
        return _id;
    }

    public byte GetPoolTeam(){
        return _team;
    }

    public byte GetPoolBiasAmount(){
        return _bias;
    }

    public byte GetPoolPower(){
        return _poolPower;
    }

    public static byte BiasChance(byte classCode) {
        return switch (classCode) {
            case CharacterClass.Arcanist -> 0;
            case CharacterClass.Magician -> 50;
            default -> 30;
        };
    }



}
