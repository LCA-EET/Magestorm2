using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
public class AvatarList : MonoBehaviour
{
    private float _elapsed;
    public AvatarStatus[] StatusList;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _elapsed = 0.0f;
        ComponentRegister.AvatarList = this;
    }

    // Update is called once per frame
    void Update()
    {
        _elapsed += Time.deltaTime;
        if(_elapsed > 3.0f)
        {
            _elapsed = 0.0f;
            int index;
            List<Avatar> toDisplay = Game.GetSortedPlayers();
            for (index = 0; index < toDisplay.Count; index++)
            {
                if(index < 20)
                {
                    StatusList[index].UpdateStatus(toDisplay[index]);
                }
                else
                {
                    break;
                }
            }
            while(index < 20)
            {
                StatusList[index].Show(false);
                index++;
            }
        }
    }
}
