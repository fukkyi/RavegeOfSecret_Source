using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleLeftSideRightDetecter : PuzzleMoveDetecter
{
    protected override DetectSide GetDetectSide()
    {
        return DetectSide.Left;
    }

    protected override bool IsValidDirection(Vector2 direction)
    {
        return direction.x > 0.8f;
    }
}
