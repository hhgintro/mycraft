using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace MyCraft
{
    public class MouseController : MonoBehaviour
    {
        //private TerrainManager terrain_manager;
        private bool belt_hold; //belt를 잡고 드래그하면 연속설치가 가능하다.
        Vector3 belt_hold_start;

        int mouse_pos_x = 0;
        int mouse_pos_y = 0;
        int mouse_pos_z = 0;
        public bool mouse_refresh = false;

        
        private Vector3 offset; //인벤아이템을 잡았을때 마우스와 이격거리
        private GameObject _delete_obj; //삭제하려는 obj. Rbutton을 일정시간 누르면 삭제할 수 있다.

        void Start()
        {
            offset = new Vector3(50f, -16f, 0);
            //terrain_manager = GameObject.Find("Terrain").GetComponent<TerrainManager>();

            StartCoroutine(CheckMouse());
        }

        IEnumerator CheckMouse()
        {
            while (true)
            {
                CheckMouse_Func();
                yield return 0;
            }
        }

        void CheckMouse_Func()
        {
            OnMouseMove();

            //mouse left
            if (Input.GetMouseButtonDown(0))    OnMouseLButtonDown();
            if (Input.GetMouseButtonUp(0))      OnMouseLButtonUp();
            //mouse right
            if (Input.GetMouseButtonDown(1))    OnMouseRButtonDown();
            if (Input.GetMouseButton(1))        OnMouseRButton();
            if (Input.GetMouseButtonUp(1))      OnMouseRButtonUp();
            //mouse middle
            //if (Input.GetMouseButtonDown(2))  OnMouseMButtonDown();
        }

        bool GetRayCast(Vector3 pos, out RaycastHit hit, int layer)
        {
            Ray ray = Camera.main.ScreenPointToRay(pos);//현재 마우스의 클릭 위치
            return Physics.Raycast(ray, out hit, Mathf.Infinity, layer);//picking된 object 정보
        }

        private void ShowChoicePrefab(int posx, int posy, int posz)
        {
            BlockScript prefab = GameManager.GetTerrainManager().GetChoicePrefab();
            if (null == prefab) return;

            if (false == CheckMouse_Refresh(posx, posy, posz))
                return;

            prefab.SetPos(posx, posy, posz);
        }

        bool CheckMouse_Refresh(int posx, int posy, int posz)
        {
            if (false == mouse_refresh
                && posx == mouse_pos_x && posy == mouse_pos_y && posz == mouse_pos_z)
                return false;

            mouse_pos_x     = posx;
            mouse_pos_y     = posy;
            mouse_pos_z     = posz;
            mouse_refresh   = false;
            //Debug.Log($"RaycastHit:({posx},{posy},{posz})");
            return true;
        }

        void OnMouseMove()
        {
            //this.choice_prefab = 1;
            //if (null != GameManager.GetTerrainManager().GetChoicePrefab())
            {
                RaycastHit hit;
                if (GetRayCast(Input.mousePosition, out hit, 1 << (int)LAYER_TYPE.BLOCK))//picking된 object 정보
                {
                    //0.6:맞닫는면의 수선의 길이가 block의 1칸을 넘지 않도록
                    Vector3 hitPoint = hit.point + hit.normal*0.6f;

                    int posx = Common.PosRound(hitPoint.x);
                    int posy = Common.PosFloor(hitPoint.y);
                    int posz = Common.PosRound(hitPoint.z);

                    //GameManager.GetCoordinates().DrawCoordinate(posx, posy, posz);    //좌표 표기
                    ShowChoicePrefab(posx, posy, posz);
                }
                else
                if (GetRayCast(Input.mousePosition, out hit, 1 << (int)LAYER_TYPE.TERRAIN))//picking된 object 정보
                {
                    int posx = Common.PosRound(hit.point.x);
                    int posy = Common.PosFloor(hit.point.y);
                    int posz = Common.PosRound(hit.point.z);

                    GameManager.GetCoordinates().DrawCoordinate(posx, posy, posz);  //좌표 표기
                    ShowChoicePrefab(posx, posy, posz);
                }
            }

            //인벤아이템을 잡았을때 마우스와 이격거리
            if (null != InvenBase.choiced_item)
                InvenBase.choiced_item.transform.position = Input.mousePosition + offset;

        }

        void OnMouseLButtonDown()
        {
#if UNITY_EDITOR
            //UI위를 클릭했을때...무시
            if (true == EventSystem.current.IsPointerOverGameObject())
                return;
            //if (EventSystem.current.IsPointerOverGameObject(-1) == false)
#elif UNITY_ANDROID // or iOS 
                if (EventSystem.current.IsPointerOverGameObject(0) == false)
                    return;
#endif

            BlockScript prefab = GameManager.GetTerrainManager().GetChoicePrefab();
            if (null == prefab) return;

            //밸트id
            if (1100 == prefab._itembase.id)
            {
                RaycastHit hit;
                int layer = 1 << (int)LAYER_TYPE.BLOCK | 1 << (int)LAYER_TYPE.TERRAIN;
                if (GetRayCast(Input.mousePosition, out hit, layer))
                {
                    this.belt_hold_start = hit.point;
                    this.belt_hold = true;
                }
            }
        }
        void OnMouseLButtonUp()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);

            RaycastHit hit;
            //HG[2017.05.16] object생성되면서 바로 처리되기 때문에. 생성되는 로직 앞쪽에서 처리하도록 합니다.
            if (GetRayCast(Input.mousePosition, out hit, 1 << (int)LAYER_TYPE.BLOCK))//picking된 object 정보
            {
                //밸트 자동연결
                if(true == this.belt_hold)
                {
                    //OnAutomaticConnectBelt(hit.point);
                    BlockScript prefab = GameManager.GetTerrainManager().GetChoicePrefab();
                    if (null != prefab) prefab.manager.OnAutomaticConnectBelt(this.belt_hold_start, hit.point, prefab);
                    this.belt_hold = false;
                    return;
                }
#if UNITY_EDITOR
                //UI위를 클릭했을때...무시
                if (true == EventSystem.current.IsPointerOverGameObject())
                    return;
                //if (EventSystem.current.IsPointerOverGameObject(-1) == false)
#elif UNITY_ANDROID // or iOS 
                if (EventSystem.current.IsPointerOverGameObject(0) == false)
                    return;
#endif

                //Debug.Log($"RaycastHit BLOCK:({hit.normal})");
                BlockScript script = hit.collider.GetComponent<BlockScript>();
                if (null != script)
                {
                    switch (script._itembase.type)
                    {
                        case BLOCKTYPE.CHEST:
                        case BLOCKTYPE.STONE_FURNACE:
                        case BLOCKTYPE.MACHINE:
                            script.OnClicked();
                            break;
                        default:
                            {
                                //0.6:맞닫는면의 수선의 길이가 block의 1칸을 넘지 않도록
                                Vector3 hisPoint = hit.point + hit.normal*0.6f;

                                int posx = Common.PosRound(hisPoint.x);
                                int posy = Common.PosFloor(hisPoint.y);
                                int posz = Common.PosRound(hisPoint.z);

                                GameManager.GetTerrainManager().CreateBlock(GameManager.GetTerrainManager().GetBlockLayer(), posx, posy, posz, GameManager.GetTerrainManager().GetChoicePrefab());
                            }
                            break;
                    }
                }
            }//..if(GetRayCast(LAYER_TYPE.BLOCK))
            else
            if (GetRayCast(Input.mousePosition, out hit, 1 << (int)LAYER_TYPE.TERRAIN))//picking된 object 정보
            {
                //밸트 자동연결
                if (true == this.belt_hold)
                {
                    //OnAutomaticConnectBelt(hit.point);
                    BlockScript prefab = GameManager.GetTerrainManager().GetChoicePrefab();
                    if (null != prefab) prefab.manager.OnAutomaticConnectBelt(this.belt_hold_start, hit.point, prefab);
                    this.belt_hold = false;
                    return;
                }

#if UNITY_EDITOR
                //UI위를 클릭했을때...무시
                if (true == EventSystem.current.IsPointerOverGameObject())
                    return;
                //if (EventSystem.current.IsPointerOverGameObject(-1) == false)
#elif UNITY_ANDROID // or iOS 
                if (EventSystem.current.IsPointerOverGameObject(0) == false)
                    return;
#endif

                //HG_TODO : LButton을 눌렸을때 block 뒤에 새로운 block을 놓이기도 하고,
                //          앞의 block이 "chest"인 경우에는 chest의 인벤정보를 보여줘야 합니다.
                //      해결책 : block을 손에 들고 있는 경우에는 새로운 block을 놓도록 처리하며
                //              빈 손인 경우에는 chest의 정보를 보여주도록 합니다.
                //..

                int posx = Common.PosRound(hit.point.x);
                int posy = Common.PosFloor(hit.point.y);
                int posz = Common.PosRound(hit.point.z);

                //terrain에 block을 생성합니다.
                GameManager.GetTerrainManager().CreateBlock(GameManager.GetTerrainManager().GetBlockLayer(), posx, posy, posz, GameManager.GetTerrainManager().GetChoicePrefab());

            }//..if(GetRayCast(LAYER_TYPE.TERRAIN))

        }

        void OnMouseRButtonDown()
        {
#if UNITY_EDITOR
            //UI위를 클릭했을때...무시
            if (true == EventSystem.current.IsPointerOverGameObject())
                return;
            //if (EventSystem.current.IsPointerOverGameObject(-1) == false)
#elif UNITY_ANDROID // or iOS 
            if (EventSystem.current.IsPointerOverGameObject(0) == false)
                return;
#endif

            //RaycastHit hit;
            //if (GetRayCast(Input.mousePosition, out hit, 1 << (int)LAYER_TYPE.BLOCK))//picking된 object 정보
            //{
            //    //Debug.Log("hit tag : " + hit.collider.tag.ToString());
            //    //GameManager.GetTerrainManager().DeleteBlock(hit.collider.gameObject, true);
            //}

            GameManager.GetDeleteProgress().SetProgress(null);
            GameManager.GetDeleteProgress().gameObject.SetActive(true);
        }

        void OnMouseRButton()
        {
#if UNITY_EDITOR
            //UI위를 클릭했을때...무시
            if (true == EventSystem.current.IsPointerOverGameObject())
                return;
            //if (EventSystem.current.IsPointerOverGameObject(-1) == false)
#elif UNITY_ANDROID // or iOS 
            if (EventSystem.current.IsPointerOverGameObject(0) == false)
                return;
#endif

            RaycastHit hit;
            if (GetRayCast(Input.mousePosition, out hit, 1 << (int)LAYER_TYPE.BLOCK))//picking된 object 정보
            {
                //Debug.Log("hit tag : " + hit.collider.tag.ToString());
                //GameManager.GetTerrainManager().DeleteBlock(hit.collider.gameObject, true);
                GameManager.GetDeleteProgress().SetProgress(hit.collider.gameObject);
                GameManager.GetDeleteProgress().gameObject.SetActive(true);
            }
            //지정한 block을 벗어나면...초기화.
            else
            {
                GameManager.GetDeleteProgress().SetProgress(null);
                GameManager.GetDeleteProgress().gameObject.SetActive(false);
            }
        }

        void OnMouseRButtonUp()
        {
#if UNITY_EDITOR
            //UI위를 클릭했을때...무시
            if (true == EventSystem.current.IsPointerOverGameObject())
                return;
            //if (EventSystem.current.IsPointerOverGameObject(-1) == false)
#elif UNITY_ANDROID // or iOS 
            if (EventSystem.current.IsPointerOverGameObject(0) == false)
                return;
#endif

            //RaycastHit hit;
            //if (GetRayCast(Input.mousePosition, out hit, 1 << (int)LAYER_TYPE.BLOCK))//picking된 object 정보
            //{
            //    //Debug.Log("hit tag : " + hit.collider.tag.ToString());
            //    GameManager.GetTerrainManager().DeleteBlock(hit.collider.gameObject, true);
            //}
            GameManager.GetDeleteProgress().gameObject.SetActive(false);
        }

    }//..class MouseController
}//..namespace MyCraft