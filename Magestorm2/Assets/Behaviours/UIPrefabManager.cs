using UnityEngine;

public class UIPrefabManager : MonoBehaviour
{
    public GameObject PrefabMessageBox;
    private void Awake()
    {
        ComponentRegister.UIPrefabManager = this;
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
        instantiated.transform.SetParent(parentTransform);
        instantiated.transform.localPosition = instantiator.transform.localPosition;
        instantiator.SetActive(false);
    }
}
