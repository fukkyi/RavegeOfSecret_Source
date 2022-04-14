using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleHandler : MonoBehaviour
{
    [SerializeField]
    private HackingController hackingController = null;
    [SerializeField]
    private RightPuzzlePreant rightPuzzle = null;
    [SerializeField]
    private RightPuzzlePreant leftPuzzle = null;
    [SerializeField]
    private float puzzleLimitTime = 180.0f;

    private bool isPuzzleComplate = false;
    private float puzzleTime = 0;

    public bool isReadyPuzzle = false;
    public bool isTimeUp = false;

    private void Update()
    {
        if (rightPuzzle.finishGame && leftPuzzle.finishGame && !isPuzzleComplate)
        {
            OnPuzzleComplated();
            isPuzzleComplate = true;
        }

        UpdatePuzzleTime();
    }

    private void OnPuzzleComplated()
    {
        hackingController.OnClearPuzzle();
    }

    private void UpdatePuzzleTime()
    {
        if (!isReadyPuzzle) return;
        if (isPuzzleComplate) return;
        if (isTimeUp) return;
        
        puzzleTime += Time.deltaTime;

        if (puzzleTime >= puzzleLimitTime)
        {
            hackingController.OnPuzzleTimeUp();
            isTimeUp = true;
        }
    }

    public void EnableMotion()
    {
        rightPuzzle.AddMotionAction();
        leftPuzzle.AddMotionAction();
    }

    public void DisableMotion()
    {
        rightPuzzle.RemoveMotionAction();
        leftPuzzle.RemoveMotionAction();
    }
}
