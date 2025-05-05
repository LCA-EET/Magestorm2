using System.Collections.Generic;
using UnityEngine;

public class ModelBuilder : MonoBehaviour
{
    public const byte FairSkin = 0;
    public const byte TanSkin = 1;
    
    public const byte MaleSex = 0;
    public const byte FemaleSex = 1;

    public const byte IndexBody = 0;
    public const byte IndexHair = 1;
    public const byte IndexHead = 2;
    public const byte IndexFace = 3;

    public GameObject[] MaleLightNeutralBody;
    public GameObject[] MaleDarkNeutralBody;

    public GameObject[] MaleLightOrderBody;
    public GameObject[] MaleDarkOrderBody;

    public GameObject[] MaleLightBalanceBody;
    public GameObject[] MaleDarkBalanceBody;

    public GameObject[] MaleLightChaosBody;
    public GameObject[] MaleDarkChaosBody;

    public GameObject[] MaleHair;
    public GameObject[] MaleFaces;

    public GameObject[] MaleLightHeads;
    public GameObject[] MaleDarkHeads;

    public GameObject[] FemaleLightNeutralBody;
    public GameObject[] FemaleDarkNeutralBody;

    public GameObject[] FemaleLightOrderBody;
    public GameObject[] FemaleDarkOrderBody;

    public GameObject[] FemaleLightBalanceBody;
    public GameObject[] FemaleDarkBalanceBody;

    public GameObject[] FemaleLightChaosBody;
    public GameObject[] FemaleDarkChaosBody;

    public GameObject[] FemaleHair;
    public GameObject[] FemaleFaces;

    public GameObject[] FemaleLightHeads;
    public GameObject[] FemaleDarkHeads;

    private Dictionary<byte, GameObject[]> _maleLightParts;
    private Dictionary<byte, GameObject[]> _maleDarkParts;

    private Dictionary<byte, GameObject[]> _femaleLightParts;
    private Dictionary<byte, GameObject[]> _femaleDarkParts;

    private int[] _componentIndices;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        _componentIndices = new int[4];
        FillDictionary(ref _maleDarkParts, MaleDarkNeutralBody, MaleHair, MaleDarkHeads, MaleFaces);
        FillDictionary(ref _maleLightParts, MaleLightNeutralBody, MaleHair, MaleLightHeads, MaleFaces);
        FillDictionary(ref _femaleDarkParts, FemaleDarkNeutralBody, FemaleHair, FemaleDarkHeads, FemaleFaces);
        FillDictionary(ref _femaleLightParts, FemaleLightNeutralBody, FemaleHair, FemaleLightHeads, FemaleFaces);
        ComponentRegister.ModelBuilder = this;
    }
    private void FillDictionary(ref Dictionary<byte, GameObject[]> toFill, GameObject[] body, GameObject[] hair, GameObject[] head, GameObject[] face)
    {
        toFill = new Dictionary<byte, GameObject[]>();
        toFill.Add(IndexBody, body);
        toFill.Add(IndexHair, hair);
        toFill.Add(IndexHead, head);
        toFill.Add(IndexFace, face);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public GameObject ConstructModel(byte[] appearance, byte team, byte level)
    {

    }
    public Dictionary<byte, GameObject[]> GetOptions(byte sex, byte skin)
    {
        if(sex == MaleSex)
        {
            if(skin == FairSkin)
            {
                return _maleLightParts;
            }
            else
            {
                return _maleDarkParts;
            }
        }
        else
        {
            if (skin == FairSkin)
            {
                return _femaleLightParts;
            }
            else
            {
                return _femaleDarkParts;
            }
        }
    }
}
