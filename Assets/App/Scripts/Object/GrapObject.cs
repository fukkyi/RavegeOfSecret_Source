using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapObject : MonoBehaviour, IPointable
{
    private bool isGraping = false;

    public void OnGraped(PointingState state)
    {
        isGraping = true;
    }

    public void OnPointed(PointingState state)
    {

    }

    public void OnUnGraped(PointingState state)
    {
        isGraping = false;
    }

    public void OnUnPointed(PointingState state)
    {

    }

    public void UpdateGraping(PointingState state)
    {
        if (isGraping)
        {
            transform.position = state.pointRay.GetPoint(state.pointDistance);
        }
    }

    public void UpdatePointing(PointingState state)
    {

    }
}
