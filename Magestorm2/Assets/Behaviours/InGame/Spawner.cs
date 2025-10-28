using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject AvatarPrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        ComponentRegister.Spawner = this;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Avatar SpawnAvatar()
    {
        return Instantiate(AvatarPrefab).GetComponent<Avatar>();
    }
}
