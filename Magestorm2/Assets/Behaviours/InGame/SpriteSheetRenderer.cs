using UnityEngine;
using System.Collections.Generic;
public class SpriteSheetRenderer : MonoBehaviour
{
    public SpriteRenderer Renderer;
    public Sprite[] Frames;
    public int StartingFrame = 0;
    public float CycleDuration = 1.0f;
    private int _currentFrame, _numFrames;
    private bool _forward = true;
    public bool ResetAfterLastFrame = true;
    private List<PeriodicAction> _actions;
    private void Start()
    {
        _actions = new List<PeriodicAction>();
        _numFrames = Frames.Length;
        new PeriodicAction(0.1f, RotateToCamera, _actions);
        new PeriodicAction(CycleDuration / _numFrames, AdvanceFrame, _actions);
        _currentFrame = StartingFrame;
        Renderer.sprite = Frames[_currentFrame];
    }
    private void Update()
    {
        PeriodicAction.PerformActions(Time.deltaTime, _actions);
    }
    private void AdvanceFrame()
    {
        if (_forward)
        {
            _currentFrame++;
            if(_currentFrame == _numFrames)
            {
                if (ResetAfterLastFrame)
                {
                    _currentFrame = 0;
                }
                else
                {
                    _currentFrame -= 2;
                    _forward = false;
                }
            }
        }
        else
        {
            _currentFrame--;
            if(_currentFrame == -1)
            {
                if (ResetAfterLastFrame)
                {
                    _currentFrame = Frames.Length-1;
                }
                else
                {
                    _currentFrame += 2;
                    _forward = true;
                }
            }
        }
        Renderer.sprite = Frames[_currentFrame];
    }
    private void RotateToCamera()
    {
        transform.LookAt(Camera.main.transform.position);
        transform.Rotate(0, 180, 0);
    }
}
