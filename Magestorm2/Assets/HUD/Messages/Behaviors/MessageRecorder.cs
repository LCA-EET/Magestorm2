using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class MessageRecorder : MonoBehaviour
{
    private float _timeElapsed = 0.0f;
    private float _timeToShow = 30.0f;
    private float _fadeStart = 25.0f;
    private float _opDenominator;
    private List<MessageData> _messagesReceived;
    private bool _hidden = false;
    public MessageDisplay[] Messages;
    private void Awake()
    {
        _opDenominator = _timeToShow - _fadeStart;
        _messagesReceived = new List<MessageData>();
        ComponentRegister.MessageRecorder = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _timeElapsed += Time.deltaTime;
        if (!_hidden)
        {
            if (_timeElapsed > _timeToShow)
            {
                HideMessages();
            }
            else if(_timeElapsed > _fadeStart)
            {
                ChangeOpacity();
            }
        }
        
    }
    private void HideMessages()
    {
        foreach(MessageDisplay md in Messages)
        {
            md.gameObject.SetActive(false);
        }
        _hidden = true;
    }
    public void MessageReceived(MessageData message)
    {
        Debug.Log("Message Received.");
        _messagesReceived.Add(message);
        int index = (_messagesReceived.Count - 1);
        for (int i = 0; i < Messages.Length; i++)
        {
            if(_messagesReceived.Count > i)
            {
                Messages[i].gameObject.SetActive(true);
                Messages[i].SetMessage(_messagesReceived[index - i]);
            }
        }
        _timeElapsed = 0.0f;
        _hidden = false;
    }
    private void ChangeOpacity()
    {
        float percentOpacity =  ((_timeToShow - _timeElapsed) / _opDenominator);
        Debug.Log(percentOpacity);
        foreach (MessageDisplay md in Messages)
        {
            md.ChangeOpacity(percentOpacity);
        }
    }
}
