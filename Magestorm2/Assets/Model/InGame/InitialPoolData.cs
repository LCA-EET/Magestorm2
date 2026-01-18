public class InitialPoolData
{
    private Team _team;
    private byte _biasAmount;
    public InitialPoolData(byte team, byte biasAmount)
    {
        _team = (Team)team;
        _biasAmount = biasAmount;
    }
    public Team BiasedToward
    {
        get { return _team; }
    }

    public byte BiasAmount
    {
        get { return _biasAmount; }
    }

}
