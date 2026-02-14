public class Score {
    private short _kills;
    private short _deaths;
    private short _raises;
    private short _capturesFor;
    private short _capturesAgainst;
    public Score(){
        _kills = 0;
        _deaths = 0;
        _raises = 0;
        _capturesFor = 0;
        _capturesAgainst = 0;
    }

    public void IncrementKills(){
        _kills++;
    }
    public void IncrementDeaths(){
        _deaths++;
    }
    public void IncrementRaises(){
        _raises++;
    }
    public void IncrementCapturesFor(){
        _capturesFor++;
    }
    public void IncrementCapturesAgainst(){
        _capturesAgainst++;
    }
    public short GetScore(){
        return (short)((_capturesFor - _capturesAgainst) + (_kills - _deaths) + (_raises / 2));
    }
    public byte GetCTFScore(){
        return (byte)(_capturesFor - _capturesAgainst);
    }
}
