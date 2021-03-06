using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinectUtil : MonoBehaviour
{
    /// <summary>
    /// SystemのVector3をUnityのVector3に変換する
    /// </summary>
    /// <param name="sysVec"></param>
    /// <returns></returns>
    public static Vector3 ConvertToUnityVector3(System.Numerics.Vector3 sysVec)
    {
        // KinectのY軸とUnityのY軸が反対のためY軸のみ反転させる
        return Vector3.right * sysVec.X + Vector3.up * -sysVec.Y + Vector3.forward * sysVec.Z;
    }

    /// <summary>
    /// SystemのQuaternionをUnityのQuaternionに変換する
    /// </summary>
    /// <param name="sysQtn"></param>
    /// <returns></returns>
    public static Quaternion ConvertToUnityQuaternion(System.Numerics.Quaternion sysQtn)
    {
        Quaternion convertedQuaternion = Quaternion.identity;
        convertedQuaternion.x = sysQtn.X;
        convertedQuaternion.y = sysQtn.Y;
        convertedQuaternion.z = sysQtn.Z;
        convertedQuaternion.w = sysQtn.W;

        return convertedQuaternion;
    }
}
