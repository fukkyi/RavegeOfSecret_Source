using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletSpawn : MonoBehaviour
{
    [SerializeField] private Transform EnemyPos = null;
    [SerializeField] private GameObject Enemy = null;
    [SerializeField] private GameObject EnemyBullet = null;


    private void Start()
    {
       //OonStay();
    }
    private void OonStay()
    {
        var parent = GameObject.Find("AirDrone");
        var enemyMovePos = EnemyPos.position;
        Instantiate(EnemyBullet,new Vector3(enemyMovePos.x,enemyMovePos.y,enemyMovePos.z), Quaternion.identity, parent.transform);
    }
}
