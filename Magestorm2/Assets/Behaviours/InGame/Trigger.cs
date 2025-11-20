using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    protected List<PeriodicAction> _actionList;

    protected virtual void InitTrigger()
    {
        _actionList = new List<PeriodicAction>();
    }
    public virtual void EnterAction(){
       
    }
    public virtual void ExitAction() { }
}

