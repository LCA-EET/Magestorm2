using UnityEngine;
using UnityEngine.UI;
public class TabIndices : MonoBehaviour 
{
    public Selectable[] SelectableControls;

    private byte _index;

    private void Awake()
    {
        _index = 0;
    }
    private void Start()
    {
        SelectableControls[0].Select();
    }
    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            if (_index == SelectableControls.Length - 1)
            {
                _index = 0;
            }
            else
            {
                _index++;
            }
            SelectableControls[_index].Select();
        }
    }
}
