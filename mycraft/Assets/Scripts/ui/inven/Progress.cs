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
		float _time		= 1f;       //(단위:s)
		//int _maxMultiple	= 1;		//최대 반복회수
		//int _curMultiple	= 0;		//현재 반복회수

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

			this.Init();
		}

		//progress를 초기상태로 설정한다.(시작상태아니다. 완료된 상태이다)
		//	시작상태로 설정은 SetFillUp() 함수를 사용한다.
		void Init()
		{
			//float rate = this._curMultiple / this._maxMultiple;
			//if (true == this._bReverse) this._fillAmount = 1f - rate;
			//else                        this._fillAmount = rate;
			if (true == this._bReverse)	this._fillAmount = 0f;
			else						this._fillAmount = 1f;
		}

		//재료/연료를 모두 소진(끝까지 갔으면)하면 true를 리턴한다.
		public bool IsEmpty()
		{
			if (true == this._bReverse)
			{
				if (0f < this._fillAmount)	return false;
				return true;
			}
			if (this._fillAmount < 1f)	return false;
			return true;
		}

		public void SetInven(InvenBase inven) { this._inven = inven; }
		public void SetFillUp(float time)	//가득채움
		{
			//재료/연료를 조금이라도 남아있다면...재설정하지 않는다.
			if(false == IsEmpty()) return;

			if (0.5f < time)	this._time = time;

			//가득채움
			if (true == this._bReverse)	this._fillAmount = 1f;
			else						this._fillAmount = 0f;
		}
		//public void SetMultiple(int cur, int multiple)
		//{
		//	this._maxMultiple = multiple;
		//	this._curMultiple = cur;
		//	this.InitProgress();
		//}

		public void Update(float PowerEfficiency)
		{
			if(true == _bReverse)	ReverseUpdate(PowerEfficiency);
			else					ForwardUpdate(PowerEfficiency);
			if (null != this._inven) this._inven.SetProgress((int)this._id, this._fillAmount);
		}

		void ReverseUpdate(float PowerEfficiency)
		{
			this._fillAmount -= Time.deltaTime * PowerEfficiency / _time;
			if (this._fillAmount <= 0f)
			{
				//this._fillAmount = 1f;	//(여기서 채우지 않고)아이템 소모할때 다시 채워준다.
				this._owner?.OnProgressCompleted(this._id);  //progress 완료를 통보합니다.
				return;
			}

			//if (this._maxMultiple <= 1) return;
			//float rateNext = (this._curMultiple + 1) / this._maxMultiple;
			//if (this._fillAmount <= rateNext)
			//{
			//	this._curMultiple += 1;
			//	this._fillAmount = rateNext;
			//	this._owner?.OnProgressReaching(this._id);  //중간정산 통보합니다.(_maxMultiple 회수만큼 통보한다.)
			//}
		}
		void ForwardUpdate(float PowerEfficiency)
		{
			this._fillAmount += Time.deltaTime * PowerEfficiency / _time;
			if (1f <= this._fillAmount)
			{
				//this._fillAmount = 0f;	//(여기서 채우지 않고)아이템 소모할때 다시 채워준다.
				this._owner?.OnProgressCompleted(this._id);  //progress 완료를 통보합니다.
				return;
			}

			//if (this._maxMultiple <= 1) return;
			//float rateNext = (this._curMultiple + 1) / this._maxMultiple;
			//if (rateNext <= this._fillAmount)
			//{
			//	this._curMultiple += 1;
			//	this._fillAmount = rateNext;
			//	this._owner?.OnProgressReaching(this._id);  //중간정산 통보합니다.(_maxMultiple 회수만큼 통보한다.)
			//}
		}
	}
}
