using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RightGamePuzzle : MonoBehaviour, IPointable
{
    public PzType GetPzl => type;
    public PzType type;
    [SerializeField]private int _num;
    [SerializeField]private int _num2; 
    public int Num { get { return _num; } set { _num = value; } }
    public int Num2 { get { return _num2; } set { _num2 = value; } }
   [SerializeField] private Image image;
    Color c;
    RightPuzzlePreant pareant;
    void Awake()
    {
        pareant = GetComponentInParent<RightPuzzlePreant>();
    }
    private void Start()
    {
        image = GetComponent<Image>();
        Color c = image.color;
        image.color = c;
    }
    public void OnPointed(PointingState state)
    {
       // pareant.ResetNum();
    }
    public void OnUnPointed(PointingState state)
    {
    //    pareant.x = 0;
    //    Color c = image.color;
    //    pareant.y = 0;
    //    c.a = 0.5f;
    //    image.color = c;
    //    Debug.Log(pareant.x);
    //    Debug.Log(pareant.y);

    }
    public void UpdatePointing(PointingState state)
    {
        
    }

    public void OnGraped(PointingState state)
    {
    }

    public void OnUnGraped(PointingState state)
    {
    }

    public void UpdateGraping(PointingState state)
    {
    }
   
}
