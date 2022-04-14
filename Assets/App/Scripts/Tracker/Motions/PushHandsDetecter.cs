using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushHandsDetecter : MotionDetecter
{
    protected override bool CheckMotion()
    {
        return false;
    }
}
