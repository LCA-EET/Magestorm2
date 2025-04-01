using UnityEngine;

public class UIPrefabManager : MonoBehaviour
{
    public GameObject PrefabMessageBox;
    public GameObject PrefabCreateAccount;
    private void Awake()
    {
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
        
    }
    public void InstantiateMessageBox(string message, GameObject instantiator, Transform parentTransform)
    {
        MessageBox instantiated = Instantiate(PrefabMessageBox).GetComponent<MessageBox>();
        instantiated.SetMessageText(message, instantiator);
        SpawnPrefab(instantiated.gameObject, instantiator, parentTransform);
    }
    public void InstantiateCreateAccountForm(GameObject instantiator, Transform parentTransform)
    {
        SpawnPrefab(Instantiate(PrefabCreateAccount), instantiator, parentTransform);
    }
    private void SpawnPrefab(GameObject instantiated, GameObject instantiator, Transform parentTransform)
    {
        instantiated.transform.SetParent(parentTransform);
        instantiated.transform.position = instantiator.transform.position;
        instantiated.transform.localPosition = Vector3.zero;
        instantiator.SetActive(false);
    }
}
