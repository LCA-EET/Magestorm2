using TMPro;
using UnityEngine;

public class PastMessages : MonoBehaviour
{
    private float _timeElapsed = 0.0f;
    private float _timeToShow = 30.0f;
    public Message[] Messages;
    private void Awake()
    {

        ComponentRegister.PastMessages = this;
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
