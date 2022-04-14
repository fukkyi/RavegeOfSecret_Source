using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenStateChange : MonoBehaviour
{
    [SerializeField] private GameObject[] GlassCrack = new GameObject[3];
    [SerializeField] private float durationTime = 0f;
    [SerializeField] private Drone a;
    static public ScreenStateChange instance;

    void Start()
    {
        if (instance == null)
            instance = this;

        for (int i = 0; i < 3; i++)
        {
            GlassCrack[i].SetActive(false);
        }

    }
    private void Update()
    {
        DeleteTimer();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="duration">Œp‘±ŽžŠÔ</param>
    public IEnumerator DamegeScreen()
    {
        ActiveTrue();
        yield return new WaitForSeconds(durationTime);
        ActiveFalse();
    }

    private void ActiveFalse()
    {
        if (GlassCrack[arrayNumber] != null)
        {
            GlassCrack[arrayNumber].SetActive(false);
        }
    }

    private int arrayNumber;

    private void ActiveTrue()
    {
        if (GlassCrack[arrayNumber].gameObject.activeSelf) { return; }
        else { GlassCrack[arrayNumber].SetActive(true); }
    }
    private bool deleteDefeatTimer = false;
    [SerializeField] private float timer = 2f;
    private void DeleteTimer()
    {
        if (deleteDefeatTimer == true) timer -= Time.deltaTime;
        if (timer < 0)
        {
            timer = 2f;
            deleteDefeatTimer = false;
            GlassCrack[arrayNumber].SetActive(false);
        }
    }
}
