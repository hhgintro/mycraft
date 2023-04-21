using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MyCraft
{
    public class ProgressBar : MonoBehaviour
    {
        public float barDisplay; //current progress
        public Vector2 pos = new Vector2(20, 40);
        public Vector2 size = new Vector2(60, 20);
        public Texture2D emptyTex;
        public Texture2D fullTex;

        void OnGUI()
        {
            //draw the background:
            GUI.BeginGroup(new Rect(pos.x, pos.y, size.x, size.y));
            GUI.Box(new Rect(0, 0, size.x, size.y), emptyTex);

            //draw the filled-in part:
            GUI.BeginGroup(new Rect(0, 0, size.x * barDisplay, size.y));
            GUI.Box(new Rect(0, 0, size.x, size.y), fullTex);
            GUI.EndGroup();
            GUI.EndGroup();
        }

        void Update()
        {
            //for this example, the bar display is linked to the current time,
            //however you would set this value based on your desired display
            //eg, the loading progress, the player's health, or whatever.
            barDisplay = Time.time * 0.05f;
            //        barDisplay = MyControlScript.staticHealth;
        }


        //public float progress = 0.75f;
        //public Vector2 pos = new Vector2(0.67f, 0.02f);
        //public Vector2 size = new Vector2(256, 256);
        //public Texture2D emptyTex;
        //public Texture2D fullTex;

        //void OnGUI()
        //{

        //    float posX = Screen.width * pos.x;
        //    float posY = Screen.height * pos.y;

        //    //draw the background:
        //    GUI.BeginGroup(new Rect(posX, posY, size.x, size.y));
        //    GUI.DrawTexture(new Rect(0, 0, size.x, size.y), emptyTex);

        //    //draw the filled-in part:
        //    //GUI.BeginGroup(new Rect(0, 0, size.x * progress, size.y));
        //    //GUI.DrawTexture(new Rect(0,0, size.x, size.y), fullTex);
        //    //draw the filled-in part FLIPPED:
        //    int xProg = (int)(size.x * progress);
        //    GUI.BeginGroup(new Rect(size.x - xProg, 0, xProg, size.y));
        //    GUI.DrawTexture(new Rect(-size.x + xProg, 0, size.x, size.y), fullTex);

        //    GUI.EndGroup();
        //    GUI.EndGroup();
        //}

    }
}
