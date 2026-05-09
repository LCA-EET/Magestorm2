using UnityEngine;
using System.Collections.Generic;
using System;
public class UIPrefabManager : MonoBehaviour
{
    private Stack<GameObject> _uiStack;
    public GameObject PrefabMessageBox;
    public GameObject PrefabCreateAccount;
    public GameObject PrefabPregamePacketProcessor;
    public GameObject PrefabCharacterSelector;
    public GameObject PrefabCharacterCreator;
    public GameObject PrefabCharacterEditor;
    public GameObject PrefabMatchList;
    public GameObject PrefabYesNoBox;
    public GameObject PrefabMatchCreator;
    public GameObject PrefabCharacterModelBuilder;
    public GameObject PrefabJoinMatch;
    public GameObject PrefabLoginScreen;
    public GameObject PrefabAppearanceChooser;
    public GameObject PrefabUIIngameMenu;
    public GameObject PrefabUIKeyMapper;
    public GameObject PrefabUIMatchScores;
    public GameObject PrefabSpellInfo;
    public GameObject PrefabAvailableSpells;
    public GameObject PrefabUISpellSlots;

    private Queue<GameObject> _poppedObjects;
    private void Awake()
    {
        if(ComponentRegister.UIPrefabManager != null)
        {
            Destroy((ComponentRegister.UIPrefabManager.gameObject));
        }
        ComponentRegister.UIPrefabManager = this;
        _uiStack = new Stack<GameObject>();
        _poppedObjects = new Queue<GameObject>();
        DontDestroyOnLoad(gameObject);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        while(_poppedObjects.Count > 0)
        {
            Destroy(_poppedObjects.Dequeue());
        }
    }
    public void ClearStack()
    {
        _uiStack.Clear();
    }
    public void InstantiateCharacterEditor(PlayerCharacter character)
    {
        GameObject instantiated = Instantiate(PrefabCharacterEditor);
        instantiated.GetComponent<UIPCEditor>().InitForm(character);
        AddToStack(instantiated);
    }
    public void InstantiateMatchScores(byte[] data, bool resetMatchID)
    {
        GameObject go = Instantiate(PrefabUIMatchScores);
        go.GetComponent<UIMatchScore>().PopulateForm(data, resetMatchID);
        AddToStack(go);
    }
    public void InstantiateInGameMenu()
    {
        
        AddToStack(Instantiate(PrefabUIIngameMenu));
    }
    public void InstantiateSpellInfo()
    {
        AddToStack(Instantiate(PrefabSpellInfo));
    }
    public void InstantiateAppearanceChooser()
    {
        AddToStack(Instantiate(PrefabAppearanceChooser));
    }
    public void InstantiateLoginForm()
    {
        AddToStack(Instantiate(PrefabLoginScreen));
    }
    public void InstantiateJoinMatch()
    {
        AddToStack(Instantiate(PrefabJoinMatch));
    }
    public void InstantiateCharacterModelBuilder()
    {
        AddToStack(Instantiate(PrefabCharacterModelBuilder));
    }
    public void InstantiateCharacterSelector()
    {
        AddToStack(Instantiate(PrefabCharacterSelector));
    }
    public void InstantiateYesNoBox(string message, ValidatableForm instantiator) 
    {
        YesNo instantiated = Instantiate(PrefabYesNoBox).GetComponent<YesNo>();
        instantiated.SetParams(new object[] { message, instantiator });
        AddToStack(instantiated.gameObject);
    }
    public void InstantiateMessageBox(string message)
    {
        MessageBox instantiated = Instantiate(PrefabMessageBox).GetComponent<MessageBox>();
        instantiated.SetParams(new object[] { message });
        AddToStack(instantiated.gameObject);
    }
    public void InstantiateMessageBox_Function(string message, Action toPerform)
    {
        MessageBox instantiated = Instantiate(PrefabMessageBox).GetComponent<MessageBox>();
        instantiated.SetParams(new object[] { message, toPerform });
        AddToStack(instantiated.gameObject);
    }
    public void InstantiateCreateAccountForm(GameObject instantiator)
    {
        AddToStack(Instantiate(PrefabCreateAccount));
    }
    public void InstantiatePregamePacketProcessor()
    {
        PregamePacketProcessor packetProcessor = Instantiate(PrefabPregamePacketProcessor).GetComponent<PregamePacketProcessor>();
    }
    public void InstantiateCharacterCreator()
    {
        AddToStack(Instantiate(PrefabCharacterCreator));
    }
    public void InstantiateMatchList()
    {
        AddToStack(Instantiate(PrefabMatchList));
    }
    public void InstantiateMatchCreator()
    {
        AddToStack(Instantiate(PrefabMatchCreator));
    }
    public void InstantiateSpellSlotter()
    {
        AddToStack(Instantiate(PrefabUISpellSlots));
    }
    public void InstantiateKeyMapper()
    {
        AddToStack(Instantiate(PrefabUIKeyMapper));
    }
    public void InstantiateAvailableSpellList(byte characterLevel, byte slotID, ISpellProcessor owner, Dictionary<byte, byte> disciplines)
    {
        GameObject newForm = Instantiate(PrefabAvailableSpells);
        newForm.GetComponentInChildren<UIAvailableSpells>().InitializeForm(characterLevel, slotID, owner, disciplines);
        AddToStack(newForm);
    }
    public GameObject AddToStack(GameObject go)
    {
        go.transform.localPosition = Vector3.zero;
        go.SetActive(true);
        if (_uiStack.Count > 0)
        {
            _uiStack.Peek().SetActive(false);
        }
        _uiStack.Push(go);
        //Debug.Log("Stack size: " + _uiStack.Count);
        return go;
    }

    public void PopFromStack()
    {
        GameObject popped = _uiStack.Pop();
        //Debug.Log("Stack size: " + _uiStack.Count);
        if (_uiStack.Count > 0)
        {
            _uiStack.Peek().SetActive(true);
        }
        _poppedObjects.Enqueue(popped);
    }
}
