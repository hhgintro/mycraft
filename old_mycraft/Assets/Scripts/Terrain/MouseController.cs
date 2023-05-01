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

        
        private Vector3 offset; //이거 뭐하는거야?


        void Start()
        {
            offset = new Vector3(16f, -16f, 0);
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



        bool CheckMouse_Refresh(int posx, int posy, int posz)
        {
            if (posx == mouse_pos_x
                && posy == mouse_pos_y
                && posz == mouse_pos_z
                && false == mouse_refresh)
                return false;

            mouse_pos_x = posx;
            mouse_pos_y = posy;
            mouse_pos_z = posz;
            mouse_refresh = false;
            //Debug.Log($"RaycastHit:({posx},{posy},{posz})");
            return true;
        }

        void CheckMouse_Func()
        {
            OnMouseMove();

            ////인벤이 활성상태이면 L/R button을 막는다.
            //if (true == GameManager.GetInventory().GetActive())
            //    return;

            if (Input.GetMouseButtonDown(0))//mouse left
                OnMouseLButtonDown();
            if (Input.GetMouseButtonUp(0))//mouse left
                OnMouseLButtonUp();

            if (Input.GetMouseButtonUp(1))//mouse right
                OnMouseRButtonUp();
            //if (Input.GetMouseButtonDown(2))//mouse middle
            //    OnMouseMButtonDown();
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


                    //HG_TODO: 이곳때문에 Belt가 2개씩 생성되는 문제발생(우선 주석으로 막아둔다.)
                    //          LButton이 눌린상태이면...belt를 계속생성하도록 합니다.
                    //BlockScript block = GameManager.GetTerrainManager().GetChoicePrefab();
                    ////belt는 마우스를 누른상태에서 이동하면 연속적으로 자동생성합니다.
                    //if (null != block && null != block._itembase
                    //    && BLOCKTYPE.BELT == block._itembase.type)
                    //{
                    //    if (Input.GetMouseButton(0))
                    //        this.CreateBlock(posx, posy, posz);
                    //}

                }
            }

            if (null != InvenBase.choiced_item)
            {
                InvenBase.choiced_item.transform.position = Input.mousePosition + offset;
            }

        }

        void OnMouseLButtonDown()
        {
            BlockScript prefab = GameManager.GetTerrainManager().GetChoicePrefab();
            if (null == prefab) return;

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
                    OnAutomaticConnectBelt(hit.point);
                    return;
                }
#if UNITY_EDITOR
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

                                this.CreateBlock(posx, posy, posz, GameManager.GetTerrainManager().GetChoicePrefab());
                            }
                            break;
                    }
                }

            }//..if(GetRayCast())
            else
            if (GetRayCast(Input.mousePosition, out hit, 1 << (int)LAYER_TYPE.TERRAIN))//picking된 object 정보
            {
                //밸트 자동연결
                if (true == this.belt_hold)
                {
                    OnAutomaticConnectBelt(hit.point);
                    return;
                }

#if UNITY_EDITOR
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
                this.CreateBlock(posx, posy, posz, GameManager.GetTerrainManager().GetChoicePrefab());

            }//..if(GetRayCast())

        }

        void OnMouseRButtonUp()
        {
            RaycastHit hit;
            if (GetRayCast(Input.mousePosition, out hit, 1 << (int)LAYER_TYPE.BLOCK))//picking된 object 정보
            {
#if UNITY_EDITOR
                if (true == EventSystem.current.IsPointerOverGameObject())
                    return;
                //if (EventSystem.current.IsPointerOverGameObject(-1) == false)
#elif UNITY_ANDROID // or iOS 
                if (EventSystem.current.IsPointerOverGameObject(0) == false)
                    return;
#endif

                //Debug.Log("layer: " + hit.collider.gameObject.layer);
                //Debug.Log("hit tag : " + hit.collider.tag.ToString());
                //if (0 == hit.collider.tag.CompareTo("Block"))
                {
                    //Debug.Log("hit tag : " + hit.collider.tag.ToString());
                    GameManager.GetTerrainManager().DeleteBlock(hit.collider.gameObject, true);
                }
            }
        }

        bool GetRayCast(Vector3 pos, out RaycastHit hit, int layer)
        {
            Ray ray = Camera.main.ScreenPointToRay(pos);//현재 마우스의 클릭 위치
            return Physics.Raycast(ray, out hit, Mathf.Infinity, layer);//picking된 object 정보
        }

        //밸트 자동연결
        void OnAutomaticConnectBelt(Vector3 point)
        {
            if (false == this.belt_hold) return;
            this.belt_hold = false;//계속 연속설치 못하도록 초기화.

            BlockScript prefab = GameManager.GetTerrainManager().GetChoicePrefab();
            if (null == prefab) return;

            Vector3 pos = belt_hold_start;  //현재위치(시작위치 부터 시작한다.)
            Vector3 belt_hold_end = point;  //종료위치
            Vector3 forward = prefab.transform.forward;
            if (Common.OnBound(forward.x, 0)) belt_hold_end.x = belt_hold_start.x;
            if (Common.OnBound(forward.z, 0)) belt_hold_end.z = belt_hold_start.z;
            //Debug.Log($"자동연결: {belt_hold_start} ==> {belt_hold_end}");

            //end position
            int endx = Common.PosRound(belt_hold_end.x);
            int endy = Common.PosFloor(belt_hold_end.y);
            int endz = Common.PosRound(belt_hold_end.z);

            bool isSameHeight = false;  //처음시작할때는 목표점 높이를 먼저맞춘다.
            bool isUp = false;          //(앞이 막혀서)위로 올라가야하나?
            bool isDown = false;        //(앞이 막혀서)아래로 내려가야하나?

            int count = -1;
            while(++count < 20)   //최대 n개(무한 루프방지)
            {
                // ********************************** //
                //1. 목표점 높이까지 먼저 올라간다.(내려간다.
                //2. 올라가다(내려가다) 막히면 앞으로 전진
                //3. 장벽이 있으면 넘는다.

                //current position
                int posx = Common.PosRound(pos.x);
                int posy = Common.PosFloor(pos.y);
                int posz = Common.PosRound(pos.z);
                //현위치 점유중이면...종료
                if (null != GameManager.GetTerrainManager().GetBlockLayer().GetBlock(posx, posy, posz))
                    return;


                //목표점 위치와 다르면...높이부터 조정한다.
                //처음에는 무조건 높이를 먼저 맞추기위해 isSameHeight를 도입했다.)
                //if (false == isSameHeight && endy != posy)
                if (endy != posy)
                {
                    //목표점이 높다면...
                    if (posy < endy)
                    {
                        // *********************** //
                        //우선순위 : 위 -> 앞 -> 아래

                        //설치할 곳.바로위
                        prefab = Automatic_Up(pos, ref isUp);
                        //설치할 곳.바로앞
                        if(null == prefab)  prefab = Automatic_Front(pos, forward, ref isUp, ref isDown);
                        //설치할 곳.바로아래
                        if(null == prefab) prefab = Automatic_Down(pos, ref isDown);
                    }
                    //목표점이 낮다면...
                    else
                    {
                        // *********************** //
                        //우선순위 : 아래 -> 앞 -> 위

                        //설치할 곳.바로아래
                        prefab = Automatic_Down(pos, ref isDown);
                        //설치할 곳.바로앞
                        if (null == prefab) prefab = Automatic_Front(pos, forward, ref isUp, ref isDown);
                        //설치할 곳.바로위
                        if (null == prefab) prefab = Automatic_Up(pos, ref isUp);
                    }
                }
                //목표점과 같은 높이이다.
                else
                {
                    isSameHeight = true;

                    // *********************** //
                    //우선순위 : 앞 -> 위 -> 아래

                    //설치할 곳.바로앞
                    prefab = Automatic_Front(pos, forward, ref isUp, ref isDown);
                    //설치할 곳.바로위
                    if (null == prefab) prefab = Automatic_Up(pos, ref isUp);
                    //설치할 곳.바로아래
                    if (null == prefab) prefab = Automatic_Down(pos, ref isDown);
                }

                //terrain에 block을 생성합니다.
                prefab.transform.forward = forward; //방향.
                this.CreateBlock(posx, posy, posz, prefab);
                //arrive
                if (posx == endx && posy == endy && posz == endz)
                    break;

                //다음 위치 설정
                if(true == isUp)            pos = pos + Vector3.up;
                else if(true == isDown)     pos = pos + Vector3.down;
                else                        pos = pos + forward;
            }
        }

        //pos: 현재위치
        private BlockScript Automatic_Front(Vector3 pos, Vector3 forward, ref bool isUp, ref bool isDown)
        {
            BlockScript prefab = null;

            //next: 설치할 곳보다 앞쪽.
            Vector3 next = pos + forward;
            int nextx = Common.PosRound(next.x);
            int nexty = Common.PosFloor(next.y);
            int nextz = Common.PosRound(next.z);

            // *********************** //
            //우선순위 : 앞 -> 위 -> 아래

            //설치할 곳보다 앞쪽이 비어있다면.
            BlockScript block = GameManager.GetTerrainManager().GetBlockLayer().GetBlock(nextx, nexty, nextz);
            if (null == block)
            {
                //올라가는 중이면 up-end
                if (true == isUp)           prefab = GameManager.GetBeltVerticalUpEndManager().GetChoicePrefab();
                //내려가는 중이면 down-end
                else if (true == isDown)    prefab = GameManager.GetBeltVerticalDownEndManager().GetChoicePrefab();
                //전진중
                else                        prefab = GameManager.GetBeltManager().GetChoicePrefab();

                isUp    = false;   //올라가는거 끝
                isDown  = false;   //내려가는거 끝
            }
            else
            {
                //임시테스트
                //밸트이면 연결해 준다.
                if(block.IsBelt())
                {
                    //같은방향이면...
                    if (true == Common.IsSameForward(block.transform.forward, forward))
                    {
                        //올라가는 중이면 up-end
                        if (true == isUp)           prefab = GameManager.GetBeltVerticalUpEndManager().GetChoicePrefab();
                        //내려가는 중이면 down-end
                        else if (true == isDown)    prefab = GameManager.GetBeltVerticalDownEndManager().GetChoicePrefab();
                        //전진중
                        else                        prefab = GameManager.GetBeltManager().GetChoicePrefab();

                        isUp    = false;   //올라가는거 끝
                        isDown  = false;   //내려가는거 끝
                    }
                }

            }
            return prefab;
        }

        private BlockScript Automatic_Up(Vector3 pos, ref bool isUp)
        {
            BlockScript prefab = null;

            //설치할 곳.바로위
            Vector3 upper = pos + Vector3.up;
            int upperx = Common.PosRound(upper.x);
            int uppery = Common.PosFloor(upper.y);
            int upperz = Common.PosRound(upper.z);

            //설치할 곳.바로위 비어있다...올라간다.
            if (null == GameManager.GetTerrainManager().GetBlockLayer().GetBlock(upperx, uppery, upperz))
            {
                //올라가는 중이면 up-middle
                if (true == isUp)   prefab = GameManager.GetBeltVerticalUpMiddleManager().GetChoicePrefab();
                //막 올라가는 중이면 up-begin
                else                prefab = GameManager.GetBeltVerticalUpBeginManager().GetChoicePrefab();

                isUp = true;    //올라가는중.
            }
            return prefab;
        }

        private BlockScript Automatic_Down(Vector3 pos, ref bool isDown)
        {
            BlockScript prefab = null;

            //설치할 곳.바로아래
            Vector3 under = pos + Vector3.down;
            int underx = Common.PosRound(under.x);
            int undery = Common.PosFloor(under.y);
            int underz = Common.PosRound(under.z);

            //설치할 곳.바로아래 비어있다.
            if (null == GameManager.GetTerrainManager().GetBlockLayer().GetBlock(underx, undery, underz))
            {
                //내려가는 중이면 down-middle
                if (true == isDown) prefab = GameManager.GetBeltVerticalDownMiddleManager().GetChoicePrefab();
                //막 내려가는 중이면 down-begin
                else                prefab = GameManager.GetBeltVerticalDownBeginManager().GetChoicePrefab();

                isDown = true;    //낼려가는중.
            }
            return prefab;
        }

        void CreateBlock(int posx, int posy, int posz, BlockScript prefab)
        {
            if (null == prefab)
                return;

            BlockScript script = GameManager.GetTerrainManager().CreateBlock(GameManager.GetTerrainManager().GetBlockLayer(), posx, posy, posz, prefab);
            if (null == script)
                return;

            //destroy
            //한개 남았을때에는 prefab와 icon모드를 지워줍니다.
            if (null == InvenBase.choiced_item)
                return;

            InvenBase.choiced_item.AddStackCount(-1, false);
            if (InvenBase.choiced_item.amount <= 0)
            {
                GameManager.GetTerrainManager().SetChoicePrefab((BlockScript)null);
                Destroy(InvenBase.choiced_item.gameObject);
                InvenBase.choiced_item = null;
            }
        }//..CreateBlock()

        private void ShowChoicePrefab(int posx, int posy, int posz)
        {
            if (null == GameManager.GetTerrainManager().GetChoicePrefab())
                return;

                if (false == CheckMouse_Refresh(posx, posy, posz))
                return;

            if(GameManager.GetTerrainManager().GetChoicePrefab())
                GameManager.GetTerrainManager().GetChoicePrefab().SetPos(posx, posy, posz);

            //if (null != InvenBase.choiced_item)
            //    GameManager.GetTerrainManager().SetChoicePrefab((ItemBase)InvenBase.choiced_item.database);
            //GameManager.GetTerrainManager().ChainBlock(posx, posy, posz, GameManager.GetTerrainManager().GetChoicePrefab());

            //HG[2023.04.20]여기서 Belt를 생성하면. 밸트위에 뺄트가 연달아.장관을 이룬다.
            //  (그래서 terrain일때만 생성하도록 변경)
            ////LButton이 눌린상태이면...belt를 계속생성하도록 합니다.
            //BlockScript block = GameManager.GetTerrainManager().GetChoicePrefab();

            ////인벤이 활성상태이면 block생성을 막는다.
            ////if (false == GameManager.GetInventory().GetActive())
            //{
            //    //belt는 마우스를 누른상태에서 이동하면 연속적으로 자동생성합니다.
            //    if (null != block && null != block._itembase
            //        && BLOCKTYPE.BELT == block._itembase.type)
            //    {
            //        if (Input.GetMouseButton(0))
            //            this.CreateBlock(posx, posy, posz);
            //    }
            //}
        }

    }//..class MouseController
}//..namespace MyCraft