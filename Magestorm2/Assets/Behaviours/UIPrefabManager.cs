using UnityEngine;
using System.Collections.Generic;
public class UIPrefabManager : MonoBehaviour
{
    public GameObject PrefabMessageBox;
    public GameObject PrefabCreateAccount;
    public GameObject PrefabUIPacketProcessor;
    private Queue<GameObject> _toActivate;
    private Queue<GameObject> _toDestroy;
    private void Awake()
    {
        _toActivate = new Queue<GameObject>();
        _toDestroy = new Queue<GameObject>();
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
        while (_toActivate.Count > 0)
        {
            GameObject toActivate = _toActivate.Dequeue();
            toActivate.SetActive(true);
        }
        while(_toDestroy.Count > 0)
        {
            GameObject toDestroy = _toDestroy.Dequeue();
            toDestroy.SetActive(false);
            Destroy(toDestroy);
        }
    }
    public void InstantiateMessageBox(string message, GameObject instantiator, Transform parentTransform)
    {
        MessageBox instantiated = Instantiate(PrefabMessageBox).GetComponent<MessageBox>();
        instantiated.SetInstantiator(instantiator, new object[] { message });
        SpawnPrefab(instantiated.gameObject, instantiator, parentTransform, false);
    }
    public void InstantiateCreateAccountForm(GameObject instantiator, Transform parentTransform, int port)
    {
        UICreateAccountForm instantiated = Instantiate(PrefabCreateAccount).GetComponent<UICreateAccountForm>();
        instantiated.SetInstantiator(instantiator, new object[] {port});
        SpawnPrefab(instantiated.gameObject, instantiator, parentTransform, false);
    }
    public void InstantiateUIPacketProcessor(int port)
    {
        UIPacketProcessor packetProcessor = Instantiate(PrefabUIPacketProcessor).GetComponent<UIPacketProcessor>();
        packetProcessor.Init(port);
        SpawnPrefab(packetProcessor.gameObject, gameObject, gameObject.transform.parent, true);
    }
    private void SpawnPrefab(GameObject instantiated, GameObject instantiator, Transform parentTransform, bool parentActive)
    {
        instantiated.transform.SetParent(parentTransform);
        instantiated.transform.position = instantiator.transform.position;
        instantiated.transform.localPosition = Vector3.zero;
        instantiator.SetActive(parentActive);
    }
    public void ActivateUIPrefab(GameObject go)
    {
        _toActivate.Enqueue(go);
    }
    public void DestroyUIPrefab(GameObject go)
    {
        _toDestroy.Enqueue(go);
    }
}
