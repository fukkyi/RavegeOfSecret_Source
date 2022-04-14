using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AislesScroller : MonoBehaviour
{
    [SerializeField]
    private Transform endRoomTrans = null;
    [SerializeField]
    private List<GameObject> scrollableAislesList = new List<GameObject>();
    [SerializeField]
    private Animator cameraAnim = null;
    [SerializeField]
    private Animator leftDoorAnim = null;
    [SerializeField]
    private Animator rightDoorAnim = null;
    [SerializeField]
    private float scrollSpeed = 10.0f;
    [SerializeField]
    private float aislesRange = 3.0f;
    /// <summary>
    /// ドア付きの廊下のドアを開かせるタイミング
    /// </summary>
    [SerializeField]
    private int doorOpenAislesCount = 4;

    private Queue<GameObject> aislesQueue = new Queue<GameObject>();
    private bool isReadyFinish = false;
    private bool isFinishAnimStarted = false;
    private float scrollingPos = 0f;

    // Start is called before the first frame update
    void Start()
    {
        // スクロールさせる分の通路をキューに追加する
        foreach(GameObject scrollableAisles in scrollableAislesList)
        {
            aislesQueue.Enqueue(scrollableAisles);
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScrollPos();
    }

    private void UpdateScrollPos()
    {
        scrollingPos += scrollSpeed * Time.deltaTime;
        if (scrollingPos >= aislesRange)
        {
            ResetAisles();
        }

        UpdateAislesScroll();
    }

    private void UpdateAislesScroll()
    {
        int aislesCount = 0;
        foreach(GameObject aisles in aislesQueue)
        {
            Vector3 aislesPos = aisles.transform.position;
            aislesPos.z = -scrollingPos + aislesRange * aislesCount;
            aisles.transform.position = aislesPos;

            aislesCount++;
        }
    }

    private void ResetAisles()
    {
        if (aislesQueue.Count <= 0) return;

        scrollingPos -= aislesRange;
        GameObject resetAisles = aislesQueue.Dequeue();

        if (isFinishAnimStarted)
        {
            // 最後のドアが特定の場所まで迫ってきたらドアを開かせる
            if (aislesQueue.Count == doorOpenAislesCount)
            {
                PlayOpenDoorAnim();
            }
            return;
        }

        aislesQueue.Enqueue(resetAisles);

        if (!isReadyFinish) return;

        endRoomTrans.SetParent(resetAisles.transform);
        endRoomTrans.position = Vector3.forward * -aislesRange;

        isFinishAnimStarted = true;
    }

    private void PlayOpenDoorAnim()
    {
        leftDoorAnim.SetBool("Open", true);
        rightDoorAnim.SetBool("Open", true);

        StartCoroutine(PlayFinishFogAnim());
    }

    private IEnumerator PlayFinishFogAnim()
    {
        cameraAnim.SetTrigger("Escape");

        yield return new WaitForSeconds(2.0f);

        SceneTransitionManager.Instance.TransitionByName("Result", TransitionType.FadeInOut);
    }

    [ContextMenu("Finish")]
    public void PlayFinishAnim()
    {
        isReadyFinish = true;
    }
}
