using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPointable
{
    public void OnPointed(PointingState state);

    public void OnUnPointed(PointingState state);

    public void UpdatePointing(PointingState state);

    public void OnGraped(PointingState state);

    public void OnUnGraped(PointingState state);

    public void UpdateGraping(PointingState state);
}
