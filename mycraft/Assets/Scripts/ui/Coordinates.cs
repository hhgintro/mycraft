using MyCraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Coordinates : MonoBehaviour
{
    Text _text;
    string _coordinate;

    private void Start()
    {
        _text = this.transform.GetChild(0).GetComponent<Text>();

        _coordinate = Managers.Locale.GetLocale("coordinates", "coordinate");
    }

    public void DrawCoordinate(Vector3 point)
    {
        //int posx = Common.PosRound(pos.x);
        //int posy = Common.PosRound(pos.y);
        //int posz = Common.PosRound(pos.z);

        //_text.text = string.Format($"ÁÂÇ¥(x,y,z): {pos}");
        _text.text = string.Format($"{_coordinate}: {point}");

        
        //int layer = 1 << (int)LAYER_TYPE.BLOCK | 1 << (int)LAYER_TYPE.TERRAIN;
        //Debug.Log($"Height:{Common.GetMapHeight(point, layer)}");
    }
}
