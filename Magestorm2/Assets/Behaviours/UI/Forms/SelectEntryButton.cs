using UnityEngine;
using UnityEngine.UI;
public class SelectEntryButton : MonoBehaviour
{
    public bool Selected;
    private Image _backgroundImage;
    private Button _button;
    private bool _priorState;
    private SelectionGroup _owningGroup;
    private int _index;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _priorState = false;
        _backgroundImage = GetComponent<Image>();
        _backgroundImage.color = Colors.EntryUnselected;
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnPress);
    }

    private void OnPress()
    {
        if (Selected)
        {
            _owningGroup.DeselectAll();
        }
        else if (!Selected)
        {
            _owningGroup.DeselectOthers(this);
        }
        UIAudio.PlayButtonPress();
    }
    // Update is called once per frame
    void Update()
    {
        if (_priorState != Selected)
        {
            _backgroundImage.color = Colors.ApplySelectionHighlightColor(Selected);
        }
        _priorState = Selected;
    }
    public void SetOwner(SelectionGroup owningGroup, int index)
    {
        _owningGroup = owningGroup;
        _index = index;
    }
    public int SelectionIndex
    {
        get { return _index; }
    }
}
