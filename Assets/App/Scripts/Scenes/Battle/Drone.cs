using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Drone : MonoBehaviour, IPointable
{
    [SerializeField]
    private Transform centerTrans = null;
    [SerializeField]
    private int attackingCount = 5;
    [SerializeField]
    private float attackCountTime = 1.0f;
    //ドローンHP
    [SerializeField]
    private float droneHitPoint = 3;
    private UnityAction onDestroyedAction = null;
    private int currentAttackingCount = 0;
    private float attackPreparationTime = 0;
    private float currentAttackPreparationTime = 0;
    private float currentAttackingCountTime = 0;
    [SerializeField] private DroneFlashing flashing;
    [SerializeField] private DroneColorChange change;
    // Update is called once per frame
    void Update()
    {
        UpdateAttackTime();
    }

    public void Init(UnityAction onDestroyed, float preparationTime)
    {
        onDestroyedAction = onDestroyed;
        attackPreparationTime = preparationTime;
    }

    public void OnGraped(PointingState state)
    {

    }

    public void OnPointed(PointingState state)
    {

    }

    public void OnUnGraped(PointingState state)
    {

    }

    public void OnUnPointed(PointingState state)
    {

    }

    public void UpdateGraping(PointingState state)
    {

    }
    /// <summary>
    /// 照準が当たっていると欣男処理
    /// </summary>
    /// <param name="state"></param>
    public void UpdatePointing(PointingState state)
    {

    }

    /// <summary>
    /// 攻撃インターバルの更新をする
    /// </summary>
    private void UpdateAttackTime()
    {
        if (currentAttackPreparationTime <= attackPreparationTime)
        {
            currentAttackPreparationTime += Time.deltaTime;
            if (currentAttackPreparationTime >= attackPreparationTime)
            {
                currentAttackingCount = attackingCount;
            }
        }
        else
        {
            UpdateAttackingCount();
        }
    }

    /// <summary>
    /// 攻撃までのカウントを更新する
    /// </summary>
    int i = 0;
    private void UpdateAttackingCount()
    {
        if (currentAttackingCountTime < attackCountTime)
        {
            currentAttackingCountTime += Time.deltaTime;
            return;
        }

        currentAttackingCount--;
        currentAttackingCountTime = 0;
        if (currentAttackingCount < 4 && i < 3)
        {
            //StartCoroutine(flashing.FlashingDrone());
            StartCoroutine(change.ColorFlashingChangechange());
            i++;
        }
        if (currentAttackingCount <= 0)
        {
            currentAttackingCount = attackingCount;
            currentAttackPreparationTime = 0;
            StartCoroutine(AttackForPlayer());
        }
    }

    /// <summary>
    /// 撃破時の処理
    /// </summary>
    [ContextMenu("Defeat")]
    private void Defeat()
    {
        onDestroyedAction?.Invoke();
        EffectManager.Instance.PlayParticle(ParticleType.DroneExplosion, centerTrans.position, Quaternion.identity);
        Destroy(gameObject);
    }

    /// <summary>
    /// ドローン攻撃時処理
    /// </summary>
    [SerializeField] private float destoryTime = 1.5f;
    private IEnumerator AttackForPlayer()
    {
        EffectManager.Instance.PlayDamageEffect();
        ScoreManager.Instance.AddDamageCount();
        //ScreenStateChange.instance.StartCoroutine("DamegeScreen");
        //AudioManager.Instance.PlaySE("GlassCrack");
        yield return new WaitForSeconds(destoryTime);

        Destroy(gameObject);
    }
    /// <summary>
    /// 当たったobjに以下のインターフェースがある場合処理を実行
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        var temp = collision.gameObject.GetComponent<IDamage>();
        if (temp != null)
        {
            temp.ApplyDamage(ref droneHitPoint);
        }
        DroneDestruction();
    }
    //hp0処理
    private void DroneDestruction()
    {
        if (droneHitPoint < 0) Defeat();
    }

}
