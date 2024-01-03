using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum Scene
    {
        Unknown,
        Logo,
        Lobby,
        World,
    }

    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount,
    }

    public enum UIEvent
    {
        Click,
        Drag,
    }

    public enum MouseEvent
    {
        Move,

        L_Press,
        L_Click,

        //미구현
        L_DblClick,

        R_Press,
        R_Click,
        R_DblClick,

    }

    public enum CameraMode
    {
        QuarterView,
    }
}
