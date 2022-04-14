using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DroneFlashing : MonoBehaviour
{
    [SerializeField] private MeshRenderer mesh;
    [SerializeField] private MeshRenderer leftWingColor;
    [SerializeField] private MeshRenderer rightWingColor;
    [SerializeField] private MeshRenderer leftLegColor;
    [SerializeField] private MeshRenderer rightLegColor;
    [SerializeField] private MeshRenderer centerLegColor;
    [SerializeField] private MeshRenderer[] m = new MeshRenderer[5]; 
    private Color droneNormalColor;
    [SerializeField] private float dronecolorRGB = 0f;
    [SerializeField] private float PostponementFlashing = 0.2f;
    void Start()
    {
        mesh.materials[1].EnableKeyword("_EMISSION");
        leftWingColor.materials[0].EnableKeyword("_EMISSION");
        rightWingColor.materials[0].EnableKeyword("_EMISSION");
        leftLegColor.materials[0].EnableKeyword("_EMISSION");
        rightLegColor.materials[0].EnableKeyword("_EMISSION");
        centerLegColor.materials[0].EnableKeyword("_EMISSION");
        droneNormalColor = new Color(dronecolorRGB, dronecolorRGB, dronecolorRGB, dronecolorRGB);
        mesh.materials[1].SetColor("_EmissionColor", droneNormalColor);
        leftWingColor.materials[0].SetColor("_EmissionColor", droneNormalColor);
        rightWingColor.materials[0].SetColor("_EmissionColor", droneNormalColor);
        leftLegColor.materials[0].SetColor("_EmissionColor", droneNormalColor);
        rightLegColor.materials[0].SetColor("_EmissionColor", droneNormalColor);
        centerLegColor.materials[0].SetColor("_EmissionColor", droneNormalColor);
    }

    public IEnumerator FlashingDrone()
    {
        mesh.materials[1].SetColor("_EmissionColor", Color.white);
        leftWingColor.materials[0].SetColor("_EmissionColor", Color.white);
        rightWingColor.materials[0].SetColor("_EmissionColor", Color.white);
        rightLegColor.materials[0].SetColor("EmissionColor", Color.white);
        leftLegColor.materials[0].SetColor("EmissionColor", Color.white);
        centerLegColor.materials[0].SetColor("_EmissionColor", Color.white);
        yield return new WaitForSeconds(PostponementFlashing);
        mesh.materials[1].SetColor("_EmissionColor", droneNormalColor);
        leftWingColor.materials[0].SetColor("_EmissionColor", droneNormalColor);
        rightWingColor.materials[0].SetColor("_EmissionColor", droneNormalColor);
        rightLegColor.materials[0].SetColor("EmissionColor", droneNormalColor);
        leftLegColor.materials[0].SetColor("EmissionColor", droneNormalColor);
        centerLegColor.materials[0].SetColor("_EmissionColor", droneNormalColor);
    }

}
