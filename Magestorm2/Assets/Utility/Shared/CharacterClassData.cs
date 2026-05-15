public class CharacterClassData
{
    private byte _id, _hpMultiplier, _manaStatCode, _deadSight;
    private short _nameRef;
    private string _abbrev;
    public CharacterClassData(string[] fields, string contents)
    {
        string[] split = contents.Split("<br>");
        string fieldID, fieldValue;
        for (int i = 0; i < fields.Length; i++)
        {
            fieldID = fields[i];
            fieldValue = split[i + 1];
            switch (fieldID)
            {
                case CharacterClassAttributes.ID:
                    _id = byte.Parse(fieldValue);
                    break;
                case CharacterClassAttributes.NAMEREFERENCE:
                    _nameRef = short.Parse(fieldValue);
                    break;
                case CharacterClassAttributes.HPMULTIPLIER:
                    _hpMultiplier = byte.Parse(fieldValue);
                    break;
                case CharacterClassAttributes.ABBREVIATION:
                    _abbrev = fieldValue;
                    break;
                case CharacterClassAttributes.MANASTATCODE:
                    _manaStatCode = byte.Parse(fieldValue);
                    break;
                case CharacterClassAttributes.DEADSIGHT:
                    _deadSight = byte.Parse(fieldValue);
                    break;
            }
        }
    }
    public bool CanSeeDeadPlayers
    {
        get {
            return _deadSight == 1;
        }
    }
    public string Abbreviation
    {
        get { return _abbrev; }
    }
    public byte HPMultiplier
    {
        get { return _hpMultiplier; }
    }
    public byte CharacterClassID
    {
        get { return _id; }
    }

    public string CharacterClassName
    {
        get { return Language.GetBaseString(_nameRef); }
    }
    public byte ManaStatCode
    {
        get { return _manaStatCode; }
    }
    public byte DeadSight
    {
        get { return _deadSight; }
    }
}

public static class CharacterClassAttributes
{
    public const string ID = "id";
    public const string NAMEREFERENCE = "nameref";
    public const string HPMULTIPLIER = "hpmultiplier";
    public const string ABBREVIATION = "abbrev";
    public const string MANASTATCODE = "manastat";
    public const string DEADSIGHT = "deadsight";
}