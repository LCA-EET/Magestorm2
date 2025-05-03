using System.Collections.Generic;
using UnityEngine;

public class UIModelPreview : MonoBehaviour
{
    public SequentialSelector SexSelector;
    public SequentialSelector SkinSelector;
    public SequentialSelector HeadSelector;
    public SequentialSelector HairSelector;
    public SequentialSelector FaceSelector;

    public GameObject ModelContainer;
    public GameObject Displayed;

    private Dictionary<byte, SequentialSelector> _selectors;
    private void Awake()
    {
        _selectors = new Dictionary<byte, SequentialSelector>();
        _selectors.Add(0, SexSelector);
        _selectors.Add(1, SkinSelector);
        _selectors.Add(2, HairSelector);
        _selectors.Add(3, FaceSelector);
        _selectors.Add(4, HeadSelector);
        SexSelector.SetOptionCount(2);
        SkinSelector.SetOptionCount(2);

        foreach(byte idx in _selectors.Keys)
        {
            _selectors[idx].AssignOwner(this);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RebuildModel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void RebuildModel()
    {
        Dictionary<byte, GameObject[]> _bodyParts = ComponentRegister.ModelBuilder.GetOptions(SexSelector.SelectedIndex, SkinSelector.SelectedIndex);
        Debug.Log(ModelBuilder.IndexHead + ", " + HeadSelector.SelectedIndex);
        GameObject[] headParts = _bodyParts[ModelBuilder.IndexHead];
        GameObject[] hairParts = _bodyParts[ModelBuilder.IndexHair];
        GameObject[] faceParts = _bodyParts[ModelBuilder.IndexFace];
        GameObject[] bodyParts = _bodyParts[ModelBuilder.IndexBody];
        HeadSelector.SetOptionCount((byte)headParts.Length);
        HairSelector.SetOptionCount((byte)hairParts.Length);
        FaceSelector.SetOptionCount((byte)faceParts.Length);
        GameObject head = Instantiate(headParts[HeadSelector.SelectedIndex]);
        GameObject hair = Instantiate(hairParts[HairSelector.SelectedIndex]);
        GameObject face = Instantiate(faceParts[FaceSelector.SelectedIndex]);
        GameObject body = Instantiate(bodyParts[0]);
        head.transform.parent = body.transform;
        face.transform.parent = head.transform;
        hair.transform.parent = head.transform;
        
        Destroy(Displayed);
        Displayed = body;
        Displayed.transform.parent = ModelContainer.transform;
        Displayed.transform.localRotation = Quaternion.identity;
        Displayed.transform.localPosition = Vector3.zero;
    }
    public void SelectionChanged()
    {
        RebuildModel();
    }
    public void ButtonPressed(byte index, bool increase)
    {
        
    }
}
