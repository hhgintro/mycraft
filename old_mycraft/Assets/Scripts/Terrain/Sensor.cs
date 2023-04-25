using MyCraft;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour
{
    public BlockScript _owner;    //������
    public SENSOR _sensor;        //Front, Left, Right, Back(��������Ǻ���)

    private void Awake()
    {
        this._owner = this.transform.parent.parent.GetComponent<BlockScript>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != (int)LAYER_TYPE.SENSOR) return;

        Sensor sensor = other.gameObject.GetComponent<Sensor>();

        //terrain�� ��ġ��[other] ��츸 üũ�մϴ�.
        if (false == sensor._owner._bOnTerrain)
            return;

        //[�ڽ�]�� prefab�����϶� ������ �ٲ��� �ʽ��ϴ�(�ּ�����)
        //[�ڽ�]�� prefab�����϶� ������ �ٲ�� �ϹǷ� �Ʒ��� �ּ�ó����
        if (false == this._owner._bOnTerrain)
            return;

        if (false == this.WeightTurn(sensor))
            return;

        //������ ��ġ�� ��쿡 ���� ó��
        if (this._sensor == sensor._sensor)
            return;

        //��������
        this._owner.LinkedSensor(this._sensor, sensor);
        //block�� ������ ����Ǵ��� üũ�Ѵ�.
        ++this._owner._sensors;

    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != (int)LAYER_TYPE.SENSOR) return;

        Sensor sensor = other.gameObject.GetComponent<Sensor>();

        //terrain�� ��ġ��[other] ��츸 üũ�մϴ�.
        if (false == sensor._owner._bOnTerrain)
            return;

        //[�ڽ�]�� prefab�����϶� ������ �ٲ��� �ʽ��ϴ�(�ּ�����)
        //[�ڽ�]�� prefab�����϶� ������ �ٲ�� �ϹǷ� �Ʒ��� �ּ�ó����
        if (false == this._owner._bOnTerrain)
            return;

        if (false == this.WeightTurn(sensor))
            return;

        //������ ��ġ�� ��쿡 ���� ó��
        if (this._sensor == sensor._sensor)
            return;

        //��������
        this._owner.LinkedSensor(this._sensor, null);        //block�� ������ ����Ǵ��� üũ�Ѵ�.
        //block�� ������ ����Ǵ��� üũ�Ѵ�.
        ++this._owner._sensors;


        //BlockScript block = GameManager.GetTerrainManager().GetChoicePrefab();
        //if (block) GameManager.GetTerrainManager().ChainBlock((int)block.transform.position.x, (int)block.transform.position.y, (int)block.transform.position.z, block);

        ////terrain�� ��ġ�� ��츸 üũ�մϴ�.
        //if (true == this._owner._bOnTerrain)
        //    ++this._owner._sensors;



        //Deactive(sensor);       //OnTriggerExit()���Ŀ� SetActive(false)�ϱ� ����.
        DestroyObject(sensor);  //OnTriggerExit()���Ŀ� �����ϱ� ����.
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    //Debug.Log($"Collision Enter({this.gameObject.name}): ���({collision.gameObject.name}");
    //}

    //private void OnCollisionExit(Collision collision)
    //{
    //    //Debug.Log($"Collision Exit({this.gameObject.name}): ���({collision.gameObject.name}");
    //}



    //[�ڽ�]�� forward�� ��ġ���� true�� �����մϴ�.
    protected bool WeightTurn(Sensor other)
    {
        if (null == other) return false;

        //forward(�����߻�...�������� üũ)
        float err_bound = 0.01f;
        float angle = Vector3.Angle(other.transform.forward, this.transform.forward);
        if (angle < -err_bound || err_bound < angle)
            return false;

        return true;//����ġ ����
    }

    //protected void Deactive(Sensor sensor)
    //{
    //    //������ ���� �ڵ尪
    //    if (false == sensor._owner._deactive)
    //        return;
    //    sensor._owner.SetActive(false);
    //}
    protected void DestroyObject(Sensor sensor)
    {
        //������ ���� �ڵ尪
        if (false == sensor._owner._destory)
            return;

        sensor._owner._destory = false;
        //HG_TODO : �������� �ʰ� pool���� �����ϵ��� �ٲ�� �մϴ�.(crash�߻�)
        GameObject.Destroy(sensor._owner.gameObject);
        Debug.Log($"({sensor._owner._index}){sensor._owner.name}({sensor._sensor})�� �ı��Ǿ����ϴ�.");
    }

}
