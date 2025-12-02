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

    public const byte IndexModelSex = 0;
    public const byte IndexModelSkin = 1;
    public const byte IndexModelHair = 2;
    public const byte IndexModelFace = 3;
    public const byte IndexModelHead = 4;

    private GameObject[] _maleLightBodies;
    private GameObject[] _femaleLightBodies;

    private GameObject[] _maleDarkBodies;
    private GameObject[] _femaleDarkBodies;

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

    private Dictionary<string, Material> _opaqueToTransparent;
    private Dictionary<string, Material> _transparentToOpaque;

    private int[] _componentIndices;
    private void Awake()
    {
        if(ComponentRegister.ModelBuilder == null)
        {
            ComponentRegister.ModelBuilder = this;
            DontDestroyOnLoad(this);
            BuildMaterialsTable();
            _componentIndices = new int[4];
            _maleLightBodies = new GameObject[12];
            _femaleLightBodies = new GameObject[12];

            _maleDarkBodies = new GameObject[12];
            _femaleDarkBodies = new GameObject[12];

            FillBodyArray(ref _maleLightBodies, new GameObject[][] { MaleLightNeutralBody, MaleLightChaosBody, MaleLightBalanceBody, MaleLightOrderBody });
            FillBodyArray(ref _maleDarkBodies, new GameObject[][] { MaleDarkNeutralBody, MaleDarkChaosBody, MaleDarkBalanceBody, MaleDarkOrderBody });
            FillBodyArray(ref _femaleLightBodies, new GameObject[][] { FemaleLightNeutralBody, FemaleLightChaosBody, FemaleLightBalanceBody, FemaleLightOrderBody });
            FillBodyArray(ref _femaleDarkBodies, new GameObject[][] { FemaleDarkNeutralBody, FemaleDarkChaosBody, FemaleDarkBalanceBody, FemaleDarkOrderBody });
            FillDictionary(ref _maleDarkParts, _maleDarkBodies, MaleHair, MaleDarkHeads, MaleFaces);
            FillDictionary(ref _maleLightParts, _maleLightBodies, MaleHair, MaleLightHeads, MaleFaces);
            FillDictionary(ref _femaleDarkParts, _femaleDarkBodies, FemaleHair, FemaleDarkHeads, FemaleFaces);
            FillDictionary(ref _femaleLightParts, _femaleLightBodies, FemaleHair, FemaleLightHeads, FemaleFaces);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }
    private void BuildMaterialsTable()
    {
        _opaqueToTransparent = new Dictionary<string, Material>();
        _transparentToOpaque = new Dictionary<string, Material>();
        Material[] loaded = Resources.LoadAll<Material>("Materials");
        //Debug.Log("ModelBuilder: Loading " + loaded.Length + " materials.");
        Dictionary<string, Material> temp = new Dictionary<string, Material>();
        foreach (Material mat in loaded)
        {
            temp.Add(mat.name.ToLower(), mat);
            //Debug.Log("ModelBuilder: Loaded material " + mat.name.ToLower() + " to the temp table.");
        }
        foreach (string matName in temp.Keys)
        {
            string transparentMaterialName = "transparent " + matName;
            if(temp.ContainsKey(transparentMaterialName))
            {
                //Debug.Log("ModelBuilder: Added transparent material " + transparentMaterialName);
                _opaqueToTransparent.Add(matName, temp[transparentMaterialName]);
                _transparentToOpaque.Add(transparentMaterialName, temp[matName]);
            }
        }
    }
    public bool GetMaterial(string materialName, bool opaque, ref Material material)
    {
        string key = materialName.ToLower();
        Dictionary<string, Material> toUse = opaque ? _transparentToOpaque : _opaqueToTransparent;
        if (toUse.ContainsKey(key))
        {
            material = toUse[key];
            return true;
        }
        return false;
    }
    private void FillBodyArray(ref GameObject[] array, GameObject[][] bodies)
    {
        int index = 0;
        for(int i = 0; i < bodies.Length; i++)
        {
            for(int j = 0; j < bodies[i].Length; j++)
            {
                array[index] = bodies[i][j];
                index++;
            }
        }
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
    public GameObject ConstructModel(byte[] appearance, byte team, byte level, GameObject parent)
    {
        byte sex = appearance[IndexModelSex];
        byte skin = appearance[IndexModelSkin];
        Dictionary<byte, GameObject[]> components = GetOptions(sex, skin);
        
        GameObject head = components[IndexHead][appearance[IndexModelHead]];
        GameObject face = components[IndexFace][appearance[IndexModelFace]];
        GameObject hair = components[IndexHair][appearance[IndexModelHair]];
        int bodyIndex = (team * 3) + (int)Mathf.Floor(level / 8);
        //Debug.Log("Team: " + team + ", BI: " + bodyIndex);
        GameObject body = components[IndexBody][bodyIndex];
        return InstantiateModel(body, head, face, hair, parent);
    }
    public static GameObject InstantiateModel(GameObject bodyPrefab, GameObject headPrefab, GameObject facePrefab, GameObject hairPrefab, GameObject parent)
    {
        GameObject head = Instantiate(headPrefab);
        GameObject hair = Instantiate(hairPrefab);
        GameObject face = Instantiate(facePrefab);
        GameObject body = Instantiate(bodyPrefab);
        head.transform.parent = body.transform;
        face.transform.parent = head.transform;
        hair.transform.parent = head.transform;
        body.transform.parent = parent.transform;
        body.transform.localRotation = Quaternion.identity;
        body.transform.localPosition = Vector3.zero;
        return body;
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
