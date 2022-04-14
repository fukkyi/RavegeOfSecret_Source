using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformUtil
{
    /// <summary>
    /// ����̓x��
    /// </summary>
    public const float AroundDegAngle = 360;

    /// <summary>
    /// ����̃G���A���Ń����_���ȍ��W��Ԃ�
    /// </summary>
    /// <param name="center"></param>
    /// <param name="boxSize"></param>
    /// <returns></returns>
    public static Vector3 GetRandPosByBox(Vector3 center, Vector3 boxSize)
    {
        Vector3 randPos = center;
        randPos.x += Random.Range(-boxSize.x / 2, boxSize.x / 2);
        randPos.y += Random.Range(-boxSize.y / 2, boxSize.y / 2);
        randPos.z += Random.Range(-boxSize.z / 2, boxSize.z / 2);

        return randPos;
    }

    /// <summary>
    /// Y���𖳎�����������Ԃ�
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static float Calc2DDistance(Vector3 a, Vector3 b)
    {
        a.y = 0;
        b.y = 0;
        return Vector3.Distance(a, b);
    }

    /// <summary>
    /// Y���𖳎������ڕW�ւ̕�����Ԃ�
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static Vector3 Calc2DDirection(Vector3 origin, Vector3 target)
    {
        origin.y = 0;
        target.y = 0;
        return (target - origin).normalized;
    }

    /// <summary>
    /// Y���𖳎������ڕW�ւ̕�����O������Ƃ���3�����x�N�g���ŕԂ�
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static Vector3 Calc2DDirectionForVector3(Vector3 origin, Vector3 target)
    {
        Vector2 direction2D = Calc2DDirection(origin, target);
        return Vector3.right * direction2D.x + Vector3.forward * direction2D.y;
    }

    /// <summary>
    /// Z���������_���ɂ����N�H�[�^�j�I�����擾����
    /// </summary>
    /// <param name="minAngle"></param>
    /// <param name="maxAngle"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public static Quaternion GetRandomZRotation(float minAngle = 0, float maxAngle = 360, Quaternion? target = null)
    {
        Quaternion targetRotation = Quaternion.identity;
        if (target != null)
        {
            targetRotation = (Quaternion)target;
        }

        Vector3 targetEulerAngle = targetRotation.eulerAngles;
        float zAngle = Random.Range(minAngle, maxAngle);

        return Quaternion.Euler(targetEulerAngle.x, targetEulerAngle.y, zAngle);
    }

    /// <summary>
    /// �l�p��̃����_���ȓ_���擾����
    /// </summary>
    /// <param name="targetRect"></param>
    /// <returns></returns>
    public static Vector2 GetRandomPotisionOnRect(Rect targetRect)
    {
        float xPos = Random.Range(targetRect.xMin, targetRect.xMax);
        float yPos = Random.Range(targetRect.yMin, targetRect.yMax);

        return Vector2.right * xPos + Vector2.up * yPos;
    }
}