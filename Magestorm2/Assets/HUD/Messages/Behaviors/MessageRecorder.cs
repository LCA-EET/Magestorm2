using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System;

public class MessageRecorder : MonoBehaviour
{
    private float _timeElapsed = 0.0f;
    private float _timeToShow = 30.0f;
    private float _fadeStart = 25.0f;
    private float _opDenominator;
    private List<MessageData> _messagesReceived;
    private bool _hidden = false;
    private int _messageIndex = 0;
    private int _priorIndex = -1;
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
        MessageData md = new MessageData("Welcome to Magus.", "Server");
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
        if (InputControls.ChatScrollTop)
        {
            _messageIndex = 0;
        }
        if (InputControls.ChatScrollBottom)
        {
            _messageIndex = _messagesReceived.Count - 1;
        }
        if (InputControls.ChatScrollUp)
        {
            if(_messageIndex > 0)
            {
                _messageIndex--;
            }
        }
        if (InputControls.ChatScrollDown)
        {
            if( _messageIndex < (_messagesReceived.Count - 1))
            {
                _messageIndex++;
            }
        }
        if(_priorIndex != _messageIndex)
        {
            _priorIndex = _messageIndex;
            DisplayMessages();
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
        _messageIndex = _messagesReceived.Count-1;
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
    private void DisplayMessages()
    {
       
        int index = _messageIndex;
        int upperBound = _messagesReceived.Count;
        //Debug.Log("Displaying messages. Index = " + index +", UB: " + upperBound);
        for (int i = 0; i < Messages.Length; i++)
        {
            if (index >= 0 && (index < upperBound))
            {
                Messages[i].gameObject.SetActive(true);
                Messages[i].SetMessage(_messagesReceived[index]);
                index--;
            }
            else
            {
                Messages[i].gameObject.SetActive(false);
            }
        }
        _timeElapsed = 0.0f;
        _hidden = false;
    }
}
