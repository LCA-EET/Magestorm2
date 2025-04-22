using UnityEngine;
using System.Collections.Generic;
public class UIPrefabManager : MonoBehaviour
{
    private Stack<GameObject> _uiStack;
    public GameObject PrefabMessageBox;
    public GameObject PrefabCreateAccount;
    public GameObject PrefabPregamePacketProcessor;
    public GameObject PrefabCharacterSelector;
    public GameObject PrefabCharacterCreator;
    public GameObject PrefabMatchList;
    public GameObject PrefabYesNoBox;

    private Queue<GameObject> _poppedObjects;
    private void Awake()
    {
        _uiStack = new Stack<GameObject>();
        _poppedObjects = new Queue<GameObject>();
        ComponentRegister.UIPrefabManager = this;
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
    public void InstantiateCharacterSelector()
    {
        UICharacterSelectForm form = Instantiate(PrefabCharacterSelector).GetComponent<UICharacterSelectForm>();
        AddToStack(form.gameObject);
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
    public void InstantiateCreateAccountForm(GameObject instantiator, int port)
    {
        UICreateAccountForm instantiated = Instantiate(PrefabCreateAccount).GetComponent<UICreateAccountForm>();
        AddToStack(instantiated.gameObject);
    }
    public void InstantiatePregamePacketProcessor(int port)
    {
        PregamePacketProcessor packetProcessor = Instantiate(PrefabPregamePacketProcessor).GetComponent<PregamePacketProcessor>();
        packetProcessor.Init(port);
    }
    public void InstantiateCharacterCreator()
    {
        UICharacterCreationForm form = Instantiate(PrefabCharacterCreator).GetComponent<UICharacterCreationForm>();
        AddToStack(form.gameObject);
    }
    public void InstantiateMatchList()
    {
        UIMatchList form = Instantiate(PrefabMatchList).GetComponent<UIMatchList>();
        AddToStack(form.gameObject);
    }

    public void AddToStack(GameObject go)
    {
        Debug.Log("Adding " + go.ToString() + " to the UI stack");
        go.transform.SetParent(ComponentRegister.UIParent);
        go.transform.localPosition = Vector3.zero;
        //go.transform.localScale = ComponentRegister.UIParent.localScale;
        go.SetActive(true);
        if (_uiStack.Count > 0)
        {
            _uiStack.Peek().SetActive(false);
        }
        _uiStack.Push(go);
    }

    public void PopFromStack()
    {
        GameObject popped = _uiStack.Pop();
        _uiStack.Peek().SetActive(true);
        _poppedObjects.Enqueue(popped);
    }
}
