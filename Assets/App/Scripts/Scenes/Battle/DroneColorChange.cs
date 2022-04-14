using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneColorChange : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] mesh = new MeshRenderer[5];

    [SerializeField] private float BeforeChangeColor = 1.0f;
    [SerializeField] private float AfterChangeColor = 1.0f;
    [SerializeField] private float PostponementFlashing = 0.4f;
    private Color addedColor;
    private Color normalColor;

    /// <summary>
    /// コルーチンで使用する色の初期設定&mesh[]のobjのemission設定変更
    /// </summary>
    void Start()
    {
        addedColor = new Color(AfterChangeColor, AfterChangeColor, AfterChangeColor, AfterChangeColor);
        normalColor = new Color(BeforeChangeColor, BeforeChangeColor, BeforeChangeColor, BeforeChangeColor);
        mesh[0].materials[1].EnableKeyword("_EMISSION");
        mesh[0].materials[1].SetColor("_EmissionColor", normalColor);
        for (int i = 0; i < mesh.Length; i++){
            mesh[i].materials[0].EnableKeyword("_EMISSION");
            mesh[i].materials[0].SetColor("_EmissionColor", normalColor);
        }
    }

    /// <summary>
    /// ドローンの色をPostponementFlashing秒毎変更
    /// </summary>
    /// <returns></returns>
    public IEnumerator ColorFlashingChangechange()
    {
        mesh[0].materials[1].SetColor("_EmissionColor", addedColor);
        for (int i = 0; i < mesh.Length; i++){
            mesh[i].materials[0].SetColor("_EmissionColor", addedColor);
        }
        yield return new WaitForSeconds(PostponementFlashing);
        mesh[0].materials[1].SetColor("_EmissionColor", normalColor);
        for (int i = 0; i < mesh.Length; i++){
            mesh[i].materials[0].SetColor("_EmissionColor", normalColor);
        }
    }

}
