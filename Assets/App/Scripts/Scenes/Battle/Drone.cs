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
    //�h���[��HP
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
    /// �Ə����������Ă���ƋӒj����
    /// </summary>
    /// <param name="state"></param>
    public void UpdatePointing(PointingState state)
    {

    }

    /// <summary>
    /// �U���C���^�[�o���̍X�V������
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
    /// �U���܂ł̃J�E���g���X�V����
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
    /// ���j���̏���
    /// </summary>
    [ContextMenu("Defeat")]
    private void Defeat()
    {
        onDestroyedAction?.Invoke();
        EffectManager.Instance.PlayParticle(ParticleType.DroneExplosion, centerTrans.position, Quaternion.identity);
        Destroy(gameObject);
    }

    /// <summary>
    /// �h���[���U��������
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
    /// ��������obj�Ɉȉ��̃C���^�[�t�F�[�X������ꍇ���������s
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
    //hp0����
    private void DroneDestruction()
    {
        if (droneHitPoint < 0) Defeat();
    }

}
