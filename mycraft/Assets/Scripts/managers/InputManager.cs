//using MyCraft;
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
		if (KeyAction != null)
        {
            ////only keyboard, not mouse
            //if(Input.anyKeyDown
            //    && !Input.GetMouseButton(0) && !Input.GetMouseButton(1) && !Input.GetMouseButton(2))
            //    KeyAction.Invoke();
            if (Input.anyKeyDown)   //keyboard, mouse
                KeyAction.Invoke();
        }

		if (MouseAction != null)
        {
            if (Input.GetMouseButton(0))        //left
            {
                MouseAction.Invoke(Define.MouseEvent.L_Press);
                _lpressed = true;
                //Debug.Log("L down");
            }
			else if (Input.GetMouseButton(1))   //right
			{
				MouseAction.Invoke(Define.MouseEvent.R_Press);
				_rpressed = true;
				//Debug.Log("R down");
			}
			else
			{
                if (_lpressed) {
                    MouseAction.Invoke(Define.MouseEvent.L_Click);
                    _lpressed = false;
					//Debug.Log("L up");
				}
				if (_rpressed) {
                    MouseAction.Invoke(Define.MouseEvent.R_Click);
                    _rpressed = false;
                    //Debug.Log("R up");
				}
			}

            MouseAction.Invoke(Define.MouseEvent.Move);
        }
    }

    public void Clear()
    {
        KeyAction = null;
        MouseAction = null;
    }

	public void KeyActionList()
    {
		Delegate[] delegates = this.KeyAction.GetInvocationList();
        ActionList(this.KeyAction.ToString(), delegates);
	}
	public void MouseActionList<T>(Action<T> action)
    {
		Delegate[] delegates = this.MouseAction.GetInvocationList();
        ActionList(this.MouseAction.ToString(), delegates);
	}
    private void ActionList(string name, Delegate[] delegates)
    {
		Debug.Log($"Registered {name} Delegates({delegates.Length}):");
		for (int i = 0; i < delegates.Length; i++)
			Debug.Log($"  {i,2}:{delegates[i].Method.Name}");   //{i,n}:n전체필드의폭(오른쪽정렬)
	}
}
