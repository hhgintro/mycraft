using MyCraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    public BlockScript _owner;    //소유자
    public SENSOR _sensor;        //Front, Left, Right, Back(어느방향판별용)

    private void Awake()
    {
        this._owner = this.transform.parent.parent.GetComponent<BlockScript>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != (int)LAYER_TYPE.SENSOR) return;

        Sensor sensor = other.gameObject.GetComponent<Sensor>();

        //terrain에 설치된[other] 경우만 체크합니다.
        if (false == sensor._owner._bOnTerrain)
            return;

        //[자신]이 prefab상태일때 외형을 바꾸지 않습니다(주석해제)
        //[자신]이 prefab상태일때 외형이 바꿔야 하므로 아래는 주석처리함
        if (false == this._owner._bOnTerrain)
            return;

        if (false == this.WeightTurn(sensor))
            return;

        //생성시 겹치는 경우에 대한 처리
        if (this._sensor == sensor._sensor)
            return;

        //센서연결
        this._owner.LinkedSensor(this._sensor, sensor);
        //block의 외형이 변경되는지 체크한다.
        ++this._owner._sensors;

    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != (int)LAYER_TYPE.SENSOR) return;

        Sensor sensor = other.gameObject.GetComponent<Sensor>();

        //terrain에 설치된[other] 경우만 체크합니다.
        if (false == sensor._owner._bOnTerrain)
            return;

        //[자신]이 prefab상태일때 외형을 바꾸지 않습니다(주석해제)
        //[자신]이 prefab상태일때 외형이 바꿔야 하므로 아래는 주석처리함
        if (false == this._owner._bOnTerrain)
            return;

        if (false == this.WeightTurn(sensor))
            return;

        //생성시 겹치는 경우에 대한 처리
        if (this._sensor == sensor._sensor)
            return;

        //센서해제
        this._owner.LinkedSensor(this._sensor, null);        //block의 외형이 변경되는지 체크한다.
        //block의 외형이 변경되는지 체크한다.
        ++this._owner._sensors;


        //BlockScript block = GameManager.GetTerrainManager().GetChoicePrefab();
        //if (block) GameManager.GetTerrainManager().ChainBlock((int)block.transform.position.x, (int)block.transform.position.y, (int)block.transform.position.z, block);

        ////terrain에 설치된 경우만 체크합니다.
        //if (true == this._owner._bOnTerrain)
        //    ++this._owner._sensors;



        //Deactive(sensor);       //OnTriggerExit()이후에 SetActive(false)하기 위해.
        DestroyObject(sensor);  //OnTriggerExit()이후에 삭제하기 위해.
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    //Debug.Log($"Collision Enter({this.gameObject.name}): 대상({collision.gameObject.name}");
    //}

    //private void OnCollisionExit(Collision collision)
    //{
    //    //Debug.Log($"Collision Exit({this.gameObject.name}): 대상({collision.gameObject.name}");
    //}



    //[자신]의 forward와 일치히면 true를 리턴합니다.
    protected bool WeightTurn(Sensor other)
    {
        if (null == other) return false;

        //forward(오차발생...허용범위로 체크)
        float err_bound = 0.01f;
        float angle = Vector3.Angle(other.transform.forward, this.transform.forward);
        if (angle < -err_bound || err_bound < angle)
            return false;

        return true;//가중치 적용
    }

    //protected void Deactive(Sensor sensor)
    //{
    //    //삭제를 위한 코드값
    //    if (false == sensor._owner._deactive)
    //        return;
    //    sensor._owner.SetActive(false);
    //}
    protected void DestroyObject(Sensor sensor)
    {
        //삭제를 위한 코드값
        if (false == sensor._owner._destory)
            return;

        sensor._owner._destory = false;
        //HG_TODO : 삭제하지 않고 pool에서 관리하도록 바꿔야 합니다.(crash발생)
        GameObject.Destroy(sensor._owner.gameObject);
        Debug.Log($"({sensor._owner._index}){sensor._owner.name}({sensor._sensor})가 파괴되었습니다.");
    }

}
