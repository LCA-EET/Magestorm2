using System.Collections.Generic;
using UnityEngine;

public class ModelContainer : MonoBehaviour
{
    private const byte _body = 0;
    private const byte _hair = 1;
    private const byte _head = 2;
    private const byte _face = 3;

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

    private Dictionary<byte, GameObject[]> _maleLightParts;
    private Dictionary<byte, GameObject[]> _maleDarkParts;

    private int[] _componentIndices;
    private void Awake()
    {
        _componentIndices = new int[4];
        FillDictionary(_maleDarkParts, MaleDarkNeutralBody, MaleHair, MaleDarkHeads, MaleFaces);
        FillDictionary(_maleLightParts, MaleLightNeutralBody, MaleHair, MaleLightHeads, MaleFaces);
    }
    private void FillDictionary(Dictionary<byte, GameObject[]> toFill, GameObject[] body, GameObject[] hair, GameObject[] head, GameObject[] face)
    {
        toFill = new Dictionary<byte, GameObject[]>();
        toFill.Add(_body, body);
        toFill.Add(_hair, hair);
        toFill.Add(_head, head);
        toFill.Add(_face, face);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
