using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmulatedGamePieceLabel : MonoBehaviour
{
    private Text[] _labels;
    private string _id;
    private string _side;
    
    void Awake()
    {
        _labels = transform.GetChild(0).GetComponentsInChildren<Text>();
    }

    public void SetId(int id)
    {
        _id = "Id: " + id;
        _labels[0].text = _id + _side;
    }

    public void SetSide(int side)
    {
        _side = " Side: " + side;
        _labels[0].text = _id + _side;
    }

    public void SetType(GamePiece.Type type)
    {
        _labels[1].text = type.ToString();
    }

}
