using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinectUtil : MonoBehaviour
{
    /// <summary>
    /// System‚ÌVector3‚ðUnity‚ÌVector3‚É•ÏŠ·‚·‚é
    /// </summary>
    /// <param name="sysVec"></param>
    /// <returns></returns>
    public static Vector3 ConvertToUnityVector3(System.Numerics.Vector3 sysVec)
    {
        // Kinect‚ÌYŽ²‚ÆUnity‚ÌYŽ²‚ª”½‘Î‚Ì‚½‚ßYŽ²‚Ì‚Ý”½“]‚³‚¹‚é
        return Vector3.right * sysVec.X + Vector3.up * -sysVec.Y + Vector3.forward * sysVec.Z;
    }

    /// <summary>
    /// System‚ÌQuaternion‚ðUnity‚ÌQuaternion‚É•ÏŠ·‚·‚é
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
