using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    protected float _elapsed = 0.0f;
    protected float _interval = 5.0f;

    public virtual void EnterAction(){
       
    }
    public virtual void ExitAction() { }
}

