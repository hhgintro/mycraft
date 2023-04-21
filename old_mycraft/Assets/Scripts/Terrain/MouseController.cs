using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyCraft
{
    public class MouseController : MonoBehaviour
    {
        //private TerrainManager terrain_manager;


        int mouse_pos_x = 0;
        int mouse_pos_y = 0;
        int mouse_pos_z = 0;
        public bool mouse_refresh = false;

        private Vector3 offset;
        // Use this for initialization
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
            if (Input.GetMouseButtonDown(1))//mouse right
                OnMouseRButtonDown();
            //if (Input.GetMouseButtonDown(2))//mouse middle
            //    OnMouseMButtonDown();
        }

        void OnMouseMove()
        {
            //this.choice_prefab = 1;
            if (null != GameManager.GetTerrainManager().GetChoicePrefab())
            {
                RaycastHit hit;
                if (GetRayCast(Input.mousePosition, out hit, 1 << (int)LAYER_TYPE.BLOCK))//picking된 object 정보
                {
                    //0.6:맞닫는면의 수선의 길이가 block의 1칸을 넘지 않도록
                    Vector3 hitPoint = hit.point + hit.normal*0.6f;

                    int posx = Common.PosRound(hitPoint.x);
                    int posy = Common.PosFloor(hitPoint.y);
                    int posz = Common.PosRound(hitPoint.z);
                    //Debug.Log($"RaycastHit BLOCK:   ray({hit.point})");
                    //Debug.Log($"RaycastHit BLOCK:   hitPoint({hitPoint})");
                    //Debug.Log($"RaycastHit BLOCK:   pos({posx},{posy},{posz})");
                    ShowChoicePrefab(posx, posy, posz);

                }
                else
                if (GetRayCast(Input.mousePosition, out hit, 1 << (int)LAYER_TYPE.TERRAIN))//picking된 object 정보
                {
                    int posx = Common.PosRound(hit.point.x);
                    int posy = Common.PosFloor(hit.point.y);
                    int posz = Common.PosRound(hit.point.z);
                    //Debug.Log($"RaycastHit TERRAIN:   ray({hit.point})");
                    //Debug.Log($"RaycastHit TERRAIN:   pos({posx},{posy},{posz})");
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
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(Camera.main.transform.position, ray.direction * 100.0f, Color.red, 1.0f);

            RaycastHit hit;

            //HG[2017.05.16] object생성되면서 바로 처리되기 때문에. 생성되는 로직 앞쪽에서 처리하도록 합니다.
            if (GetRayCast(Input.mousePosition, out hit, 1 << (int)LAYER_TYPE.BLOCK))//picking된 object 정보
            {
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

                                this.CreateBlock(posx, posy, posz);
                            }
                            break;
                    }
                }

            }//..if(GetRayCast())
            else
            if (GetRayCast(Input.mousePosition, out hit, 1 << (int)LAYER_TYPE.TERRAIN))//picking된 object 정보
            {
                //HG_TODO : LButton을 눌렸을때 block 뒤에 새로운 block을 놓이기도 하고,
                //          앞의 block이 "chest"인 경우에는 chest의 인벤정보를 보여줘야 합니다.
                //      해결책 : block을 손에 들고 있는 경우에는 새로운 block을 놓도록 처리하며
                //              빈 손인 경우에는 chest의 정보를 보여주도록 합니다.
                //..

                int posx = Common.PosRound(hit.point.x);
                int posy = Common.PosFloor(hit.point.y);
                int posz = Common.PosRound(hit.point.z);

                //terrain에 block을 생성합니다.
                this.CreateBlock(posx, posy, posz);

            }//..if(GetRayCast())

        }

        void OnMouseRButtonDown()
        {
            RaycastHit hit;
            if (GetRayCast(Input.mousePosition, out hit, 1 << (int)LAYER_TYPE.BLOCK))//picking된 object 정보
            {
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

        void CreateBlock(int posx, int posy, int posz)
        {
            if (null == GameManager.GetTerrainManager().GetChoicePrefab())
                return;

            BlockScript script = GameManager.GetTerrainManager().CreateBlock(GameManager.GetTerrainManager().GetBlockLayer()
                , posx, posy, posz, GameManager.GetTerrainManager().GetChoicePrefab());
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