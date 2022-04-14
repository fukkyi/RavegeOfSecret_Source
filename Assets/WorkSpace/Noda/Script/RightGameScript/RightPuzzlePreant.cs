using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum PzType
{
    BESIDE,
    VERTICAL,
    UPCURVE,
    DOWNCURVE,
    NONE,
}
public enum PuzzlePosition
{
RIGHT,
LEFT,
}

public class RightPuzzlePreant : MonoBehaviour
{
   
    [SerializeField] private RightGamePuzzle[] pzls;
    [SerializeField] private Image image;
    [SerializeField] private float timer = 0.0f;
    [SerializeField] private PuzzlePosition puzzlePosition;
    [SerializeField] private float coolTime=0;
    private int[] correctedPuzzle1 = new int[6];
    private int x, y;
    private bool pzlMovedCoolTimerFinish = false;
    private bool[] finishPart1 = new bool[4];
    public bool finishGame { get; private set; }
    private bool initialize = false;

    //public int addx { get { return x; } set { x = value; } }
    //public int addy { get { return y; } set { y = value; } }
    public RightGamePuzzle[,] pzlList = new RightGamePuzzle[3, 4];

    void Start()
    {
        var n = 0;
        pzls = new RightGamePuzzle[12];

        for (int i = 0; i < transform.childCount; i++)
        {
            pzls[i] = transform.GetChild(i).GetComponent<RightGamePuzzle>();
        }

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                pzlList[i, j] = pzls[n];
                n++;
            }
        }
        CorePuzzleAlphaChange();
    }

    void Update()
    {
        PuzzleCoolDown();
        PuzzleCoolDown();
        UpdateCheckingTheArray();
    }

    public void AddMotionAction()
    {
        if (initialize) return;

        if (puzzlePosition == PuzzlePosition.LEFT)
        {
            TrackingManager.Instance.MainMotionUpdater.AddMotionDetectedAction(typeof(PuzzleLeftSideUpDetecter), UpActionArrayVariation);
            TrackingManager.Instance.MainMotionUpdater.AddMotionDetectedAction(typeof(PuzzleLeftSideDownDetecter), DownActionArrayVariation);
            TrackingManager.Instance.MainMotionUpdater.AddMotionDetectedAction(typeof(PuzzleLeftSideRightDetecter), RightActionArrayVariation);
            TrackingManager.Instance.MainMotionUpdater.AddMotionDetectedAction(typeof(PuzzleLeftSideLeftDetecter), LeftActionArrayVariation);
        }
        else if (puzzlePosition == PuzzlePosition.RIGHT)
        {
            TrackingManager.Instance.MainMotionUpdater.AddMotionDetectedAction(typeof(PuzzleRightSideUpDetecter), UpActionArrayVariation);
            TrackingManager.Instance.MainMotionUpdater.AddMotionDetectedAction(typeof(PuzzleRightSideDownDetecter), DownActionArrayVariation);
            TrackingManager.Instance.MainMotionUpdater.AddMotionDetectedAction(typeof(PuzzleRightSideRightDetecter), RightActionArrayVariation);
            TrackingManager.Instance.MainMotionUpdater.AddMotionDetectedAction(typeof(PuzzleRightSideLeftDetecter), LeftActionArrayVariation);
        }

        initialize = true;
    }

    public void RemoveMotionAction()
    {
        if (puzzlePosition == PuzzlePosition.LEFT)
        {
            TrackingManager.Instance.MainMotionUpdater.RemoveMotionDetectedAction(typeof(PuzzleLeftSideUpDetecter), UpActionArrayVariation);
            TrackingManager.Instance.MainMotionUpdater.RemoveMotionDetectedAction(typeof(PuzzleLeftSideDownDetecter), DownActionArrayVariation);
            TrackingManager.Instance.MainMotionUpdater.RemoveMotionDetectedAction(typeof(PuzzleLeftSideRightDetecter), RightActionArrayVariation);
            TrackingManager.Instance.MainMotionUpdater.RemoveMotionDetectedAction(typeof(PuzzleLeftSideLeftDetecter), LeftActionArrayVariation);
        }
        else if (puzzlePosition == PuzzlePosition.RIGHT)
        {
            TrackingManager.Instance.MainMotionUpdater.RemoveMotionDetectedAction(typeof(PuzzleRightSideUpDetecter), UpActionArrayVariation);
            TrackingManager.Instance.MainMotionUpdater.RemoveMotionDetectedAction(typeof(PuzzleRightSideDownDetecter), DownActionArrayVariation);
            TrackingManager.Instance.MainMotionUpdater.RemoveMotionDetectedAction(typeof(PuzzleRightSideRightDetecter), RightActionArrayVariation);
            TrackingManager.Instance.MainMotionUpdater.RemoveMotionDetectedAction(typeof(PuzzleRightSideLeftDetecter), LeftActionArrayVariation);
        }
    }

    [ContextMenu("Down")]
    //下方向に移動
    private void DownActionArrayVariation()
    {
        if (IsCoolDown()) { return; }
        if (finishGame) { return; }

        var temp = pzlList[2, x];
        pzlList[2, x] = pzlList[1, x];
        pzlList[1, x] = pzlList[0, x];
        pzlList[0, x] = temp;

        y = y < 2 ? y + 1 : 0;

        UIPuzzleMoveX(0);
        SetPuzzleCoolDown();

        AudioManager.Instance.PlaySE("Bullet Flyby 6");
    }
    private void SetPuzzleCoolDown()
    {
        coolTime = 0.5f;
    }
    private bool IsCoolDown()
    {
        return coolTime>0;
    }

    [ContextMenu("Up")]
    //上方向に移動
    private void UpActionArrayVariation()
    {
        if (IsCoolDown()) { return; }
        if (finishGame) { return; }

        var temp = pzlList[0, x];
        pzlList[0, x] = pzlList[1, x];
        pzlList[1, x] = pzlList[2, x];
        pzlList[2, x] = temp;

        y = y > 0 ? y - 1 : 2;
        UIPuzzleMoveX(1);
        SetPuzzleCoolDown();

        AudioManager.Instance.PlaySE("Bullet Flyby 6");
    }

    [ContextMenu("Right")]
    //右方向へpuzzle移動
    private void RightActionArrayVariation()
    {
        if (IsCoolDown()) { return; }
        if (finishGame) { return; }

        var temp = pzlList[y, 3];
        pzlList[y, 3] = pzlList[y, 2];
        pzlList[y, 2] = pzlList[y, 1];
        pzlList[y, 1] = pzlList[y, 0];
        pzlList[y, 0] = temp;

        x = x < 3 ? x + 1 : 0;
        UIPuzzleMoveY(1);
        SetPuzzleCoolDown();

        AudioManager.Instance.PlaySE("Bullet Flyby 6");
    }

    [ContextMenu("Left")]
    //左方向へpuzzle移動
    private void LeftActionArrayVariation()
    {
        if (IsCoolDown()) { return; }
        if (finishGame) { return; }

        var temp = pzlList[y, 0];
        pzlList[y, 0] = pzlList[y, 1];
        pzlList[y, 1] = pzlList[y, 2];
        pzlList[y, 2] = pzlList[y, 3];
        pzlList[y, 3] = temp;

        x = x > 0 ? x - 1 : 3;
        UIPuzzleMoveY(0);
        SetPuzzleCoolDown();

        AudioManager.Instance.PlaySE("Bullet Flyby 6");
    }
    //alpha値変更
    private void CorePuzzleAlphaChange()
    {
        Color c = image.color;
        c.a = 0.3f;
        image.color = c;
    }
    //上下の移動
    private void UIPuzzleMoveX(int n)
    {

        var pos = pzlList[0, x].transform.position;
        var pos1 = pzlList[1, x].transform.position;
        var pos2 = pzlList[2, x].transform.position;
        if (n == 0)
        {
            pzlList[2, x].transform.position = pos;
            pzlList[1, x].transform.position = pos2;
            pzlList[0, x].transform.position = pos1;
        }
        else if (n == 1)
        {
            pzlList[0, x].transform.position = pos2;
            pzlList[1, x].transform.position = pos;
            pzlList[2, x].transform.position = pos1;
        }
    }
    //左右移動
    private void UIPuzzleMoveY(int n)
    {
        var pos = pzlList[y, 0].transform.position;
        var pos1 = pzlList[y, 1].transform.position;
        var pos2 = pzlList[y, 2].transform.position;
        var pos3 = pzlList[y, 3].transform.position;

        if (n == 0)
        {
            pzlList[y, 0].transform.position = pos3;
            pzlList[y, 1].transform.position = pos;
            pzlList[y, 2].transform.position = pos1;
            pzlList[y, 3].transform.position = pos2;
        }
        else if (n == 1)
        {
            pzlList[y, 0].transform.position = pos1;
            pzlList[y, 1].transform.position = pos2;
            pzlList[y, 2].transform.position = pos3;
            pzlList[y, 3].transform.position = pos;
        }
    }
    //クリア条件  条件が重なる部分には同じ数字を代入
    private void UpdateCheckingTheArray()
    {
        if (puzzlePosition == PuzzlePosition.LEFT) { 
        /*
        ● ● ● ●
              ●
              ●
        */
        _ = pzlList[0, 0].GetPzl == PzType.BESIDE ? correctedPuzzle1[0] = 1 : correctedPuzzle1[0] = 0;
        _ = pzlList[0, 1].GetPzl == PzType.BESIDE && correctedPuzzle1[0] == 1 ? correctedPuzzle1[1] = 1 : correctedPuzzle1[1] = 0;
        _ = pzlList[0, 2].GetPzl == PzType.BESIDE && correctedPuzzle1[1] == 1 ? correctedPuzzle1[2] = 1 : correctedPuzzle1[2] = 0;
        _ = pzlList[0, 3].GetPzl == PzType.DOWNCURVE && correctedPuzzle1[2] == 1 ? correctedPuzzle1[3] = 1 : correctedPuzzle1[3] = 0;
        _ = pzlList[1, 3].GetPzl == PzType.VERTICAL && correctedPuzzle1[3] == 1 ? correctedPuzzle1[4] = 1 : correctedPuzzle1[4] = 0;
        _ = pzlList[2, 3].GetPzl == PzType.UPCURVE && correctedPuzzle1[4] == 1 ? finishPart1[0] = true : finishPart1[0] = false;

        /*
        ● ●      
          ●
          ● ● ●
        */
        _ = pzlList[0, 0].GetPzl == PzType.BESIDE ? correctedPuzzle1[0] = 1 : correctedPuzzle1[0] = 0;
        _ = pzlList[0, 1].GetPzl == PzType.DOWNCURVE && correctedPuzzle1[0] == 1 ? correctedPuzzle1[1] = 1 : correctedPuzzle1[1] = 0;
        _ = pzlList[1, 1].GetPzl == PzType.VERTICAL && correctedPuzzle1[1] == 1 ? correctedPuzzle1[2] = 2 : correctedPuzzle1[2] = 0;
        _ = pzlList[2, 1].GetPzl == PzType.UPCURVE && correctedPuzzle1[2] == 2 ? correctedPuzzle1[3] = 2 : correctedPuzzle1[3] = 0;
        _ = pzlList[2, 2].GetPzl == PzType.BESIDE && correctedPuzzle1[3] == 2 ? correctedPuzzle1[4] = 2 : correctedPuzzle1[4] = 0;
        _ = pzlList[2, 3].GetPzl == PzType.BESIDE && correctedPuzzle1[4] == 2 ? finishPart1[1] = true : finishPart1[1] = false;

        /*
         ● ● ●
             ●
        　   ●　●　
         */
        _ = pzlList[0, 0].GetPzl == PzType.BESIDE ? correctedPuzzle1[0] = 1 : correctedPuzzle1[0] = 0;
        _ = pzlList[0, 1].GetPzl == PzType.BESIDE && correctedPuzzle1[0] == 1 ? correctedPuzzle1[1] = 1 : correctedPuzzle1[1] = 0;
        _ = pzlList[0, 2].GetPzl == PzType.DOWNCURVE && correctedPuzzle1[1] == 1 ? correctedPuzzle1[2] = 1 : correctedPuzzle1[2] = 0;
        _ = pzlList[1, 2].GetPzl == PzType.VERTICAL && correctedPuzzle1[2] == 1 ? correctedPuzzle1[3] = 2 : correctedPuzzle1[3] = 0;
        _ = pzlList[2, 2].GetPzl == PzType.UPCURVE && correctedPuzzle1[3] == 2 ? correctedPuzzle1[4] = 2 : correctedPuzzle1[4] = 0;
        _ = pzlList[2, 3].GetPzl == PzType.BESIDE && correctedPuzzle1[4] == 2 ? finishPart1[2] = true : finishPart1[2] = false;

        /*
         ●
         ●
         ● ● ● ●
         */
        _ = pzlList[0, 0].GetPzl == PzType.DOWNCURVE ? correctedPuzzle1[0] = 1 : correctedPuzzle1[0] = 0;
        _ = pzlList[1, 0].GetPzl == PzType.VERTICAL && correctedPuzzle1[0] == 1 ? correctedPuzzle1[1] = 1 : correctedPuzzle1[0] = 0;
        _ = pzlList[2, 0].GetPzl == PzType.UPCURVE && correctedPuzzle1[1] == 1 ? correctedPuzzle1[2] = 5 : correctedPuzzle1[1] = 0;
        _ = pzlList[2, 1].GetPzl == PzType.BESIDE && correctedPuzzle1[2] == 5 ? correctedPuzzle1[3] = 2 : correctedPuzzle1[2] = 0;
        _ = pzlList[2, 2].GetPzl == PzType.BESIDE && correctedPuzzle1[3] == 2 ? correctedPuzzle1[4] = 2 : correctedPuzzle1[3] = 0;
        _ = pzlList[2, 3].GetPzl == PzType.BESIDE && correctedPuzzle1[4] == 2 ? finishPart1[3] = true : finishPart1[3] = false;
        }
        else if(puzzlePosition == PuzzlePosition.RIGHT)
        {
            _ = pzlList[0, 3].GetPzl == PzType.BESIDE ? correctedPuzzle1[0] = 1 : correctedPuzzle1[0] = 0;
            _ = pzlList[0, 2].GetPzl == PzType.BESIDE && correctedPuzzle1[0] == 1 ? correctedPuzzle1[1] = 1 : correctedPuzzle1[1] = 0;
            _ = pzlList[0, 1].GetPzl == PzType.BESIDE && correctedPuzzle1[1] == 1 ? correctedPuzzle1[2] = 1 : correctedPuzzle1[2] = 0;
            _ = pzlList[0, 0].GetPzl == PzType.DOWNCURVE && correctedPuzzle1[2] == 1 ? correctedPuzzle1[3] = 1 : correctedPuzzle1[3] = 0;
            _ = pzlList[1, 0].GetPzl == PzType.VERTICAL && correctedPuzzle1[3] == 1 ? correctedPuzzle1[4] = 1 : correctedPuzzle1[4] = 0;
            _ = pzlList[2, 0].GetPzl == PzType.UPCURVE && correctedPuzzle1[4] == 1 ? finishPart1[0] = true : finishPart1[0] = false;

            _ = pzlList[0, 3].GetPzl == PzType.BESIDE ? correctedPuzzle1[0] = 1 : correctedPuzzle1[0] = 0;
            _ = pzlList[0, 2].GetPzl == PzType.DOWNCURVE && correctedPuzzle1[0] == 1 ? correctedPuzzle1[1] = 1 : correctedPuzzle1[1] = 0;
            _ = pzlList[1, 2].GetPzl == PzType.VERTICAL && correctedPuzzle1[1] == 1 ? correctedPuzzle1[2] = 2 : correctedPuzzle1[2] = 0;
            _ = pzlList[2, 2].GetPzl == PzType.UPCURVE && correctedPuzzle1[2] == 2 ? correctedPuzzle1[3] = 2 : correctedPuzzle1[3] = 0;
            _ = pzlList[2, 1].GetPzl == PzType.BESIDE && correctedPuzzle1[3] == 2 ? correctedPuzzle1[4] = 2 : correctedPuzzle1[4] = 0;
            _ = pzlList[2, 0].GetPzl == PzType.BESIDE && correctedPuzzle1[4] == 2 ? finishPart1[1] = true : finishPart1[1] = false;

            _ = pzlList[0, 3].GetPzl == PzType.BESIDE ? correctedPuzzle1[0] = 1 : correctedPuzzle1[0] = 0;
            _ = pzlList[0, 2].GetPzl == PzType.BESIDE && correctedPuzzle1[0] == 1 ? correctedPuzzle1[1] = 1 : correctedPuzzle1[1] = 0;
            _ = pzlList[0, 1].GetPzl == PzType.DOWNCURVE && correctedPuzzle1[1] == 1 ? correctedPuzzle1[2] = 1 : correctedPuzzle1[2] = 0;
            _ = pzlList[1, 1].GetPzl == PzType.VERTICAL && correctedPuzzle1[2] == 1 ? correctedPuzzle1[3] = 2 : correctedPuzzle1[3] = 0;
            _ = pzlList[2, 1].GetPzl == PzType.UPCURVE && correctedPuzzle1[3] == 2 ? correctedPuzzle1[4] = 2 : correctedPuzzle1[4] = 0;
            _ = pzlList[2, 0].GetPzl == PzType.BESIDE && correctedPuzzle1[4] == 2 ? finishPart1[2] = true : finishPart1[2] = false;

            _ = pzlList[0, 3].GetPzl == PzType.DOWNCURVE ? correctedPuzzle1[0] = 1 : correctedPuzzle1[0] = 0;
            _ = pzlList[1, 3].GetPzl == PzType.VERTICAL && correctedPuzzle1[0] == 1 ? correctedPuzzle1[1] = 1 : correctedPuzzle1[0] = 0;
            _ = pzlList[2, 3].GetPzl == PzType.UPCURVE && correctedPuzzle1[1] == 1 ? correctedPuzzle1[2] = 5 : correctedPuzzle1[1] = 0;
            _ = pzlList[2, 2].GetPzl == PzType.BESIDE && correctedPuzzle1[2] == 5 ? correctedPuzzle1[3] = 2 : correctedPuzzle1[2] = 0;
            _ = pzlList[2, 1].GetPzl == PzType.BESIDE && correctedPuzzle1[3] == 2 ? correctedPuzzle1[4] = 2 : correctedPuzzle1[3] = 0;
            _ = pzlList[2, 0].GetPzl == PzType.BESIDE && correctedPuzzle1[4] == 2 ? finishPart1[3] = true : finishPart1[3] = false;
        }
        if (!finishGame && (finishPart1[0] == true || finishPart1[1] == true || finishPart1[2] == true || finishPart1[3] == true)) { 
            finishGame = true;
            Debug.Log(finishGame);
            AudioManager.Instance.PlaySE("Locked 5");
        }
    }

    private void PuzzleCoolDown()
    {
        if (coolTime>0)
        {
            coolTime -=Time.deltaTime;
        }
        else 
        {
            coolTime = 0f;
        }
    }

}