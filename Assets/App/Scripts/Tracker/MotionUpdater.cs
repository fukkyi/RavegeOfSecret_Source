using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;


public class MotionUpdater : MonoBehaviour
{
    [SerializeField]
    private Transform motionParent = null;

    private List<MotionDetecter> motionDetecterList = new List<MotionDetecter>();

    // Start is called before the first frame update
    void Awake()
    {
        if (motionParent == null)
        {
            motionParent = transform;
        }

        foreach (MotionDetecter motionDetecter in motionParent.GetComponentsInChildren<MotionDetecter>())
        {
            motionDetecterList.Add(motionDetecter);
        }
    }

    /// <summary>
    /// トラッカーのモーション検知を更新する
    /// </summary>
    public void UpdateMotionDetectersByTracker(TrackingState state)
    {
        motionDetecterList.ForEach((detecter) => {
            detecter.UpdateDetecter(state);
        });
    }

    /// <summary>
    /// 複数トラッカーのモーション検知を更新する
    /// </summary>
    public void UpdateMotionDetectersByTrackers(TrackingState[] states)
    {
        foreach(TrackingState state in states)
        {
            UpdateMotionDetectersByTracker(state);
        }
    }

    /// <summary>
    /// 型からモーションディテクターを取得する
    /// </summary>
    /// <param name="motionDetecter"></param>
    /// <returns></returns>
    private MotionDetecter FindDetecterByType(Type detecterType)
    {
        return motionDetecterList.Find(detecter => detecter.GetType() == detecterType);
    }

    /// <summary>
    /// モーションを検知した際のイベントを追加する
    /// </summary>
    /// <param name="addDetecter"></param>
    /// <param name="action"></param>
    public void AddMotionDetectedAction(Type detecterType, UnityAction action)
    {
        MotionDetecter targetDetecter = FindDetecterByType(detecterType);
        if (targetDetecter == null) return;

        targetDetecter.AddDetectedAction(action);
    }

    /// <summary>
    /// モーションを検知した際の一度きりのイベントを追加する
    /// </summary>
    /// <param name="addDetecter"></param>
    /// <param name="action"></param>
    public void AddMotionDetectedOnceAction(Type detecterType, UnityAction action)
    {
        MotionDetecter targetDetecter = FindDetecterByType(detecterType);
        if (targetDetecter == null) return;

        targetDetecter.AddDetectedOnceAction(action);
    }

    /// <summary>
    /// モーションを検知した際のイベントを全て消す
    /// </summary>
    /// <param name="addDetecter"></param>
    public void ClearMotionDetectedAction(Type detecterType)
    {
        MotionDetecter targetDetecter = FindDetecterByType(detecterType);
        if (targetDetecter == null) return;

        targetDetecter.ClearDetectedAction();
    }

    /// <summary>
    /// 全てのモーション検知のイベントを全て消す
    /// </summary>
    public void ClearAllMotionDetectedAction()
    {
        motionDetecterList.ForEach((detecter) => {
            ClearMotionDetectedAction(detecter.GetType());
        });
    }

    /// <summary>
    /// モーションを検知した際のイベントを取り除く
    /// </summary>
    /// <param name="addDetecter"></param>
    /// <param name="action"></param>
    public void RemoveMotionDetectedAction(Type detecterType, UnityAction action)
    {
        MotionDetecter targetDetecter = FindDetecterByType(detecterType);
        if (targetDetecter == null) return;

        targetDetecter.RemoveDetectedAction(action);
    }
}
