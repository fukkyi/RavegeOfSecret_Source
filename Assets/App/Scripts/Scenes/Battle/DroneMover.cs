using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMover : MonoBehaviour
{
    //移動速度
    [SerializeField]
    private float droneSpeedMove = 3f;
    //一時停止時間
    [SerializeField]
    private float stopTime = 2f;
    #region 移動選択肢幅　調整用
    [SerializeField]
    private int minSelectValue = 1;
    [SerializeField]
    private int maxSelectValue = 6;
    #endregion
    #region ドローンの移動速度ふり幅 調整用
    [SerializeField]
    private float minDroneSpeed = 0.4f;
    [SerializeField]
    private float maxDroneSpeed = 2f;
    #endregion
    private int randomMoveValue = 0;
    private bool dirtransStopTimeFlag = false;
    private float circle_rad = 0.3f;
    private Vector3 initPos;
    

    // Start is called before the first frame update
    void Start()
    {
        MoveLimit();
        initPos = transform.position;
        SetRandomValue();
    }

    // Update is called once per frame
    void Update()
    {
        DirePos();
        SetMoveType();
    }
    //ランダムで移動手段と移動するスピードを設定
    private void SetRandomValue()
    {
        randomMoveValue = Random.Range(minSelectValue, maxSelectValue);
        droneSpeedMove = Random.Range(minDroneSpeed, maxDroneSpeed);
    }
    /// <summary>
    /// ドローンの位置が一定の位置を超えたらboolを変更する処理
    /// </summary>
    [SerializeField] private float rightDistanceLimit;
    [SerializeField] private float leftDistanceLimit;
    [SerializeField] private float upperDistanceLimit;
    [SerializeField] private float downDistanceLimit;
    private void DirePos()
    {
        if (transform.position.x > rightDistanceLimit)
        {
            dirtransStopTimeFlag = true;
        }
        else if (transform.position.x < leftDistanceLimit)
        {
            dirtransStopTimeFlag = false;
        }
        if (transform.position.y > upperDistanceLimit)
        {
            dirtransStopTimeFlag = true;
        }
        else if (transform.position.y < downDistanceLimit)
        {
            dirtransStopTimeFlag = false;
        }
    }
    /// <summary>
    /// 左右移動コルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator SideDroneMove()
    {
        //yield return new WaitForSeconds(stopTime);
        if (this.transform.position.x <= rightDistanceLimit && !dirtransStopTimeFlag)
        {
            this.transform.position += new Vector3(droneSpeedMove, 0, 0) * Time.deltaTime;
        }
        else if (this.transform.position.x >= leftDistanceLimit && dirtransStopTimeFlag)
        {
            yield return new WaitForSeconds(stopTime);
            this.transform.position -= new Vector3(droneSpeedMove, 0, 0) * Time.deltaTime;
        }
    }
    /// <summary>
    /// 上下移動のコルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator UpandDownDroneMove()
    {
        // yield return new WaitForSeconds(stopTime);
        if (transform.position.y <= upperDistanceLimit && !dirtransStopTimeFlag)
        {
            transform.position += new Vector3(0, droneSpeedMove, 0) * Time.deltaTime;
        }
        else if (transform.position.y >= downDistanceLimit && dirtransStopTimeFlag)
        {
            yield return new WaitForSeconds(stopTime);
            transform.position -= new Vector3(0, droneSpeedMove, 0) * Time.deltaTime;
        }
    }
    private float depth = 2f;
    private Vector3 rightTop = new Vector3(4f, 1f, 0f);
    private Vector3 leftBottom = new Vector3(-4f, -1f, -0f);
    private float distance;
    private float currentPos;
    /// <summary>
    /// 移動制限
    /// </summary>
    private void MoveLimit()
    {
        // rightTop = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, depth));
        //leftBottom = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, depth));
        distance = Vector3.Distance(rightTop, leftBottom);
        currentPos = (Time.deltaTime * droneSpeedMove) / distance;
    }
    private void SlantedDroneMove()
    {
        transform.position = Vector3.Lerp(rightTop, leftBottom, currentPos);
    }

    /// <summary>
    /// 八の字移動
    /// </summary>
    /// <returns></returns>
    #region [ふり幅]　[角周波数]　[初期位相]　[経過時間]
    private float swingWidth = 800f;
    private float corner = 0.4f;
    private float InitialPhase = 0.0f;
    private float time = 0.0f;
    #endregion
    private void FigureOfEight()
    {
        var timer = Mathf.PI / 180.0f;
        var x = swingWidth * Mathf.Sin(corner * time - InitialPhase);
        var y = -swingWidth * Mathf.Sin(corner * time * 2 - InitialPhase);
        this.transform.position = new Vector3(x / 430 * 1f, 2 + y / 1920 * 1.5f, transform.position.z);
        time += timer;
    }
    /// <summary>
    /// 円形移動
    /// </summary>
    private float x, y;
    private void CircleDroneMove()
    {
        x = -circle_rad * Mathf.Sin(Time.time * droneSpeedMove);      //X軸の設定
        y = -circle_rad * Mathf.Cos(Time.time * droneSpeedMove);      //Z軸の設定
        transform.position = new Vector3(x + initPos.x, y + initPos.y, initPos.z);
    }
    /// <summary>
    /// 各ドローンの動きを番号で振り分け
    /// </summary>
    private void SetMoveType()
    {
        switch (randomMoveValue)
        {
            case 1:
                StartCoroutine(SideDroneMove());
                break;
            case 2:
                StartCoroutine(UpandDownDroneMove());
                break;
            case 3:
                CircleDroneMove();
                break;
            case 4:
                FigureOfEight();
                break;
            case 5:
                SlantedDroneMove();
                break;
        }
    }

}
