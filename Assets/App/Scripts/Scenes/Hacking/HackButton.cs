using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HackButton : MonoBehaviour, IPointable
{
    [SerializeField]
    private HackingController hackingController = null;

    public void OnGraped(PointingState state)
    {
        
    }

    public void OnPointed(PointingState state)
    {
        hackingController.OnPushHackButton();
    }

    public void OnUnGraped(PointingState state)
    {
        
    }

    public void OnUnPointed(PointingState state)
    {
        
    }

    public void UpdateGraping(PointingState state)
    {
        
    }

    public void UpdatePointing(PointingState state)
    {
        
    }
}
