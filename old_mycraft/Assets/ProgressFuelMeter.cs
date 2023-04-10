using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace MyCraft
{
    public class ProgressFuelMeter : MonoBehaviour
    {
        public Texture2D progressBarEmpty;
        public Texture2D progressBarFull;
        public Vector2 pos = new Vector2(20, 40);
        public Vector2 size = new Vector2(20, 60);
        public float speed = 0.5f;
        float barDisplay = 0;
        void OnGUI()
        {
            // draw the background:
            GUI.BeginGroup(new Rect(pos.x, pos.y, size.x, size.y));
            GUI.Box(new Rect(0, 0, size.x, size.y), progressBarEmpty);

            // draw the filled-in part:
            GUI.BeginGroup(new Rect(0, (size.y - (size.y * barDisplay)), size.x, size.y * barDisplay));
            GUI.Box(new Rect(0, -size.y + (size.y * barDisplay), size.x, size.y), progressBarFull);
            GUI.EndGroup();
            GUI.EndGroup();
        }
        void Update()
        {
            barDisplay += speed * Time.smoothDeltaTime;

            if (barDisplay >= 1.0f)
            {
                barDisplay = 1.0f;
                speed *= -1;
            }
            else if (barDisplay <= 0)
            {
                barDisplay = 0.0f;
                speed *= -1;
            }
        }
    }
}
