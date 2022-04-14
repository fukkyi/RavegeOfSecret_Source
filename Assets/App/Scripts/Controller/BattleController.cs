using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

enum GameState
{
    Operation,
    GameClear,
    Wait
}

public class BattleController : MonoBehaviour
{
    [SerializeField] private GameObject droneSpawnPoint = null;
    [SerializeField] private GameObject bulletSpwner = null;
    [SerializeField] private DroneSpawner droneSpawner;
    [SerializeField] private AislesScroller aislesScroller;
    [SerializeField] private int clearDroneDefeatCount = 20;
    private GameState gameState;
    void Start()
    {
        gameState = GameState.Operation;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameState == GameState.Operation)
        {
            TimeLimit();
        }
        ConstantCrushing();
    }

    private void BattleEnd()
    {
        
        gameState = GameState.GameClear;
        aislesScroller.PlayFinishAnim();
        Debug.Log("GameOver");
        bulletSpwner.SetActive(false);
        droneSpawnPoint.SetActive(false);

        ScoreManager.Instance.StopMeasurePlayTime();
    }
    private void ConstantCrushing()
    {
        if (droneSpawner.DefeatCount >= clearDroneDefeatCount)
            BattleEnd();
    }
    [SerializeField] private float limitTime = 90f;
    private void TimeLimit()
    {
        if (limitTime < 0) { BattleEnd(); }
        else { limitTime -= Time.deltaTime; }
    }
}
