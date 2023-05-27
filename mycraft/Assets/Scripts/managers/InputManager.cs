using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
    public Action KeyAction = null;
    public Action<Define.MouseEvent> MouseAction = null;

    bool _lpressed = false;
    bool _rpressed = false;

    public void OnUpdate()
    {
//#if UNITY_EDITOR
//        //UI위를 클릭했을때...무시
//        if (true == EventSystem.current.IsPointerOverGameObject())
//            return;
//        //if (EventSystem.current.IsPointerOverGameObject(-1) == false)
//#elif UNITY_ANDROID // or iOS 
//                if (EventSystem.current.IsPointerOverGameObject(0) == false)
//                    return;
//#endif

        if (Input.anyKeyDown && KeyAction != null)
				KeyAction.Invoke();

        if (MouseAction != null)
        {
            if (Input.GetMouseButton(0))
            {
                MouseAction.Invoke(Define.MouseEvent.L_Press);
                _lpressed = true;
            }
            else
            {
                if (_lpressed)
                    MouseAction.Invoke(Define.MouseEvent.L_Click);
                _lpressed = false;
            }

            MouseAction.Invoke(Define.MouseEvent.Move);
        }
    }

    public void Clear()
    {
        KeyAction = null;
        MouseAction = null;
    }
}
