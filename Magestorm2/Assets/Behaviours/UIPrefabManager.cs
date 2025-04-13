using UnityEngine;
using System.Collections.Generic;
public class UIPrefabManager : MonoBehaviour
{
    private Stack<GameObject> _uiStack;
    private GameObject _topOfStack;
    public GameObject PrefabMessageBox;
    public GameObject PrefabCreateAccount;
    public GameObject PrefabPregamePacketProcessor;
    public GameObject PrefabCharacterSelector;
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

    }
    public void InstantiateMessageBox(string message)
    {
        MessageBox instantiated = Instantiate(PrefabMessageBox).GetComponent<MessageBox>();
        instantiated.SetParams(new object[] { message });
        SpawnPrefab(instantiated.gameObject);
    }
    public void InstantiateCreateAccountForm(GameObject instantiator, int port)
    {
        UICreateAccountForm instantiated = Instantiate(PrefabCreateAccount).GetComponent<UICreateAccountForm>();
        instantiated.SetParams(new object[] {port});
        SpawnPrefab(instantiated.gameObject);
    }
    public void InstantiateUIPacketProcessor(int port)
    {
        PregamePacketProcessor packetProcessor = Instantiate(PrefabPregamePacketProcessor).GetComponent<PregamePacketProcessor>();
        packetProcessor.Init(port);
    }
    private void SpawnPrefab(GameObject instantiated)
    {
        AddToStack(instantiated);
        //instantiated.transform.position = transform.parent.position;
        instantiated.transform.localPosition = Vector3.zero;
    }

    public void AddToStack(GameObject go)
    {
        go.transform.SetParent(ComponentRegister.UIParent);
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
