using MyCraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Coordinates : MonoBehaviour
{
    Text _text;

    private void Start()
    {
        _text = this.transform.GetChild(0).GetComponent<Text>();
    }

    public void DrawCoordinate(int posx, int posy, int posz)
    {
        //int posx = Common.PosRound(pos.x);
        //int posy = Common.PosFloor(pos.y);
        //int posz = Common.PosRound(pos.z);

        //_text.text = string.Format($"ÁÂÇ¥(x,y,z): {pos}");
        _text.text = string.Format($"ÁÂÇ¥(x,y,z): ({posx},{ posy},{posz})");
    }
}
