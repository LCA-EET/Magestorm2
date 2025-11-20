using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
public class AvatarList : MonoBehaviour
{
    private List<PeriodicAction> _actionList;
    public AvatarStatus[] StatusList;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _actionList = new List<PeriodicAction>();
        new PeriodicAction(1.0f, UpdateList, _actionList);
        ComponentRegister.AvatarList = this;
    }

    // Update is called once per frame
    void Update()
    {
        PeriodicAction.PerformActions(Time.deltaTime, _actionList);
    }
    private void UpdateList()
    {
        int index;
        List<Avatar> toDisplay = Match.GetSortedPlayers();
        for (index = 0; index < toDisplay.Count; index++)
        {
            if (index < 20)
            {
                StatusList[index].UpdateStatus(toDisplay[index]);
                //Debug.Log("Updating PL for " + toDisplay[index].Name);
            }
            else
            {
                break;
            }
        }
        while (index < 20)
        {
            StatusList[index].Deactivate();
            index++;
        }
    }
}
