using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class HingedDoor : RotatingAO
{
    protected override void ApplyStateChange(bool force)
    {
        if (!force)
        {
            
        }
        base.ApplyStateChange(force);
    }
}
