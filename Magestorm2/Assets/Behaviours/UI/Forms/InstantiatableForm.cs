using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InstantiatableForm : MonoBehaviour, IComparer<Selectable>
{
    private List<Selectable> _selectables;
    private int _selectedIndex;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Awake()
    {
        _selectables = new List<Selectable>(GetComponentsInChildren<Selectable>());
        _selectables.Sort(this);
        _selectables.Reverse();
        _selectedIndex = 0;
    }
    void Start()
    {
        ComponentRegister.UIPrefabManager.AddToStack(gameObject);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            if (_selectedIndex == _selectables.Count - 1)
            {
                _selectedIndex = 0;
            }
            else
            {
                _selectedIndex++;
            }
            _selectables[_selectedIndex].Select();
        }
    }

    public virtual void SetParams(object[] args)
    {

    }
    public virtual void CloseForm()
    {
        ComponentRegister.UIPrefabManager.PopFromStack();
    }

    public int Compare(Selectable x, Selectable y)
    {
        return SharedFunctions.CompareVectorsY(x.transform.position, y.transform.position);
    }
}
