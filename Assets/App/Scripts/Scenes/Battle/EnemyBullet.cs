using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private GameObject TemporaryPlayerObj = null;
    [SerializeField] private float bulletSpeed = 1f;

    private void Start()
    {
        TemporaryPlayerObj = GameObject.Find("TemporaryPlayerObj");
    }
    public void ShotBullet()
    {
        var currentPos = TemporaryPlayerObj.transform.position;
        this.transform.position = Vector3.MoveTowards(transform.position, currentPos, bulletSpeed);
    }
}
