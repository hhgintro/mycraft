using FactoryFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace MyCraft
{
	public class Progress
	{
		Building _owner;
		InvenBase _inven;

		PROGRESSID _id;				//progress 식별자
		public float _fillAmount;   //Image.fillAmount
		bool _bReverse	= false;	//false이면 차고, true이면 줄어듭니다.
		float _time		= 1f;		//(단위:s)

		//true이면 Progress가 활성화(자원/연료를 소모한) 상태
		//자원 또는 연료를 소모하면 progress가 끝까지 진행할때 까지 활성으로 판단합니다.
		//public bool _bBunning { get; set; }
		////true이면 진행중, false이면 잠시멈출
		//public bool _bRunning { get; set; }

		public Progress(Building owner, PROGRESSID id, float time, bool bReverse)
		{
			this._owner     = owner;
			this._inven     = null;
			this._id        = id;
			this._bReverse  = bReverse;
			this._time      = time;
			//this._bBunning = false;
			//this._bRunning = false;

			this.InitProgress();
		}

		public void SetTime(float time) { this._time = time; }
		public void SetInven(InvenBase inven) { this._inven = inven; }

		//progress를 초기상태로 설정한다.
		public void InitProgress()
		{
			if (true == this._bReverse) this._fillAmount = 1f;
			else                        this._fillAmount = 0f;
		}

		public void Update()
		{
			if(true == _bReverse)
			{
				this._fillAmount -= Time.deltaTime / _time;
				if (this._fillAmount <= 0f)
				{
					this._fillAmount = 1f;
					this._owner.OnProgressCompleted(this._id);  //progress 완료를 통보합니다.
				}
			}
			else
			{
				this._fillAmount += Time.deltaTime / _time;
				if (1f <= this._fillAmount)
				{
					this._fillAmount = 0f;
					this._owner.OnProgressCompleted(this._id);  //progress 완료를 통보합니다.
				}
			}

			if (null != this._inven) this._inven.SetProgress((int)this._id, this._fillAmount);
		}
	}
}
