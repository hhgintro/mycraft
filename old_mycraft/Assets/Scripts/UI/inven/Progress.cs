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
        BlockScript _owner;
        ItemInvenBase _inven;

        int _id;      //progress 식별자
        public float _fillAmount { get; set; }    //Image.fillAmount
        bool _bReverse = false;   //false이면 차고, true이면 줄어듭니다.
        float _time = 1f;//(단위:s)

        //true이면 Progress가 활성화(자원/연료를 소모한) 상태
        //자원 또는 연료를 소모하면 progress가 끝까지 진행할때 까지 활성으로 판단합니다.
        //public bool _bBunning { get; set; }
        ////true이면 진행중, false이면 잠시멈출
        //public bool _bRunning { get; set; }

        public Progress(BlockScript owner, int id, float time, bool bReverse)
        {
            this._owner = owner;
            this._inven = null;
            this._id = id;
            this._bReverse = bReverse;
            this._time = time;
            //this._bBunning = false;
            //this._bRunning = false;
            this.InitProgress();
        }

        //progress를 초기상태로 설정한다.
        public void InitProgress()
        {
            if (true == this._bReverse) this._fillAmount = 1f;
            else                        this._fillAmount = 0f;
        }
        public void Update()
        {
            ////float deltatime = Time.smoothDeltaTime;
            //if (false == this._bRunning)
            //    return;

            if(true == _bReverse)
            {
                this._fillAmount -= Time.smoothDeltaTime / _time;
                if (this._fillAmount <= 0f)
                {
                    this._fillAmount = 1f;
                    //HG_TODO : callback을 통해 progress 완료를 통보합니다.
                    //..
                    this._owner.OnProgressCompleted(this._id);
                }
            }
            else
            {
                this._fillAmount += Time.smoothDeltaTime / _time;
                if (1f <= this._fillAmount)
                {
                    this._fillAmount = 0f;
                    //HG_TODO : callback을 통해 progress 완료를 통보합니다.
                    //..
                    this._owner.OnProgressCompleted(this._id);
                }
            }

            if (null != this._inven)
                this._inven.SetProgress(this._id, this._fillAmount);
        }

        public void SetTime(float time) { this._time = time; }

        public void SetInven(ItemInvenBase inven)
        {
            this._inven = inven;
        }

        public bool GetIsBunning()
        {
            //초기값이면 bunning 상태가 아니다.
            if (true == this._bReverse)
                return (this._fillAmount != 1f);
            return (this._fillAmount != 0f);
        }
    }
}
