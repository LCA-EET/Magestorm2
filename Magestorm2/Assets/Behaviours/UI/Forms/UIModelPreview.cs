using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class UIModelPreview : ValidatableForm
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
    public override void ButtonPressed(ButtonType buttonType)
    {
        UIAudio.PlayButtonPress();
        switch (buttonType)
        {
            case ButtonType.Submit:
                break;
            case ButtonType.Cancel:
                CloseForm();
                break;
        }
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
        GameObject model = ModelBuilder.InstantiateModel(
            headParts[HeadSelector.SelectedIndex],
            hairParts[HairSelector.SelectedIndex],
            faceParts[FaceSelector.SelectedIndex],
            bodyParts[0]
        );
        
        
        Destroy(Displayed);
        Displayed = model;
        Displayed.transform.parent = ModelContainer.transform;
        Displayed.transform.localRotation = Quaternion.identity;
        Displayed.transform.localPosition = Vector3.zero;
    }
    public void SelectionChanged()
    {
        RebuildModel();
    }
    public byte[] AppearanceBytes()
    {
        byte[] toReturn = new byte[5];
        toReturn[ModelBuilder.IndexModelSex] = SexSelector.SelectedIndex;
        toReturn[ModelBuilder.IndexModelSkin] = SkinSelector.SelectedIndex;
        toReturn[ModelBuilder.IndexModelHair] = HairSelector.SelectedIndex;
        toReturn[ModelBuilder.IndexModelFace] = FaceSelector.SelectedIndex;
        toReturn[ModelBuilder.IndexModelHead] = HeadSelector.SelectedIndex;
        return toReturn;
    }
}
