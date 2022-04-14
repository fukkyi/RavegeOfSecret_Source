using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : PoolableObject, IDamage
{
    [SerializeField] private float bulletSpeed = 1f;

    private Vector3 leftBulletMoveVec = Vector3.zero;
    private Vector3 rightBulletMoveVec = Vector3.zero;
    Vector3 rightRayHitPos;
    Vector3 leftRayHitPos;
    Rigidbody rb2d;
    void Start()
    {
        rb2d = GetComponent<Rigidbody>();
        //lifeTime = 1.0f;
    }
    void Update()
    {
        //  UpdateLifeTime();
    }
    #region 左右の照準が当たっている箇所に弾(prefab)を飛ばす処理
    public void RightBulletMove(Transform rightTransform)
    {
        transform.position = rightTransform.position;
        rightRayHitPos = TrackingManager.Instance.MainTrackingState.RightPointingState.pointRay.GetPoint(4f);
        rightBulletMoveVec = rightRayHitPos - rightTransform.position;
        Vector3 rightMoveVec = Vector3.forward;
        if (rightRayHitPos != null)
        {
            rightMoveVec = rightBulletMoveVec.normalized;
        }

        rb2d.velocity = rightMoveVec * bulletSpeed;
        rb2d.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.LookRotation(rightMoveVec, Vector3.forward);
    }
    public void LeftBulletMove(Transform leftTransform)
    {
        transform.position = leftTransform.position;
        leftRayHitPos = TrackingManager.Instance.MainTrackingState.LeftPointingState.pointRay.GetPoint(4f);
        leftBulletMoveVec = leftRayHitPos - leftTransform.position;
        Vector3 leftMoveVec = Vector3.forward;
        if (leftRayHitPos != null)
        {
            leftMoveVec = leftBulletMoveVec.normalized;
        }

        rb2d.velocity = leftMoveVec * bulletSpeed;
        rb2d.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.LookRotation(leftMoveVec, Vector3.forward);
    }
    #endregion
    #region リスポーン地点設定
    public void RightSetSpawn(Transform rightTrans)
    {
        this.transform.position = rightTrans.position;
    }
    public void LeftSetSpawn(Transform leftTrans)
    {
        this.transform.position = leftTrans.position;
    }
    #endregion

    public void ApplyDamage(ref float hitPoint)
    {
        hitPoint -= 1; 
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.contactCount > 0)
        {
            ContactPoint[] contactPoints = new ContactPoint[collision.contactCount];
            collision.GetContacts(contactPoints);
            foreach (ContactPoint contactPoint in contactPoints)
            {
                EffectManager.Instance.PlayParticle(ParticleType.BulletCollision, contactPoint.point, Quaternion.LookRotation(contactPoint.normal));
            }
        }

        DisableObject();
    }
}
