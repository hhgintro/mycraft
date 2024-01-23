using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MyCraft
{
    public class SaveContext : MonoBehaviour
    {
        public RectTransform _save_file_content;    //저장파일을 정렬할 위치

        private Text _save_file;    //선택한 저장파일의 파일명
        private Image _save_image;  //선택된 저장파일의 이미지
        private List<GameObject> _contexts = new List<GameObject>();    //_save_file_content에 등록된 개체정보

		void Awake()
		{
            this._save_file = this.transform.Find("Load File/Load File Info/Title/Text").GetComponent<Text>();
            this._save_image = this.transform.Find("Load File/Load File Info/ScreenCapture").GetComponent<Image>();
            this._save_file.text = "";
		}
        private void OnEnable()
        {
			//refresh save files
			this.RefreshContext();
		}

		void Start()
		{
            fnStart();
		}

        protected virtual void fnStart() { }
		protected virtual void OnSelectSaveFile(GameObject go)
        {
            //Debug.Log($"select:{filename}");
            this._save_file.text = go.name;
            LoadScreenShot(go.name);
		}

		public void OnBack()
		{
			//prev
			this.gameObject.SetActive(false);

			//HG_TODO:[통합방법모색] lobby에 호출될 때와 world에서 호출될 때. 각각 다른값을 호출하고 있다,
			//next(lobby)
			this.transform.parent.GetComponent<Menu>()?._playmenu.SetActive(true);
			//next(world)
			this.transform.parent.parent.GetComponent<SystemMenuManager>()?.gameObject.SetActive(true);
		}

		protected void RefreshContext()
        {
            //기존꺼 삭제
            foreach (var obj in this._contexts) Managers.Resource.Destroy(obj);
            this._contexts.Clear();
            //재 등록(파일목록을 수정일자순으로 가져오는 코드)
            string[] savefiles = Common.GetFilesInFolderWithExtensionOrderByCreationTime(Managers.Game._save_dir, "sav");
            foreach (string file in savefiles)
            {
                //Debug.Log(file);
                GameObject clone = Managers.Resource.Instantiate("Prefabs/ui/Save-Filename", this.transform);
                clone.name = file;
                clone.transform.GetChild(0).GetComponent<Text>().text = file;
                //clone.transform.parent = this._save_file_content;
                clone.transform.SetParent(this._save_file_content);
                clone.GetComponent<Button>().onClick.AddListener(() => OnSelectSaveFile(clone));
                this._contexts.Add(clone);
            }

            //마지막 저장된 파일을 선택한다.
            if (0 < this._contexts.Count)
            {
                GameObject go = this._contexts[this._contexts.Count - 1];
                go.GetComponent<Button>().Select();//선택된 상태
                OnSelectSaveFile(go);
            }
        }
		public void OnDeleteFile()
		{
			if (string.IsNullOrEmpty(this._save_file.text)) return;
			string filepath = Path.Combine(Managers.Game._save_dir, this._save_file.text);
			if (false == filepath.Contains(".sav")) filepath += ".sav"; //확장자 추가
			if (false == File.Exists(filepath))
			{
				//Debug.LogError($"load failed({filepath})");
				return;
			}
			File.Delete(filepath);
			this.RefreshContext();
		}

		private void LoadScreenShot(string filename)
        {
			string filepath = Path.Combine(Managers.Game._save_dir, filename);
			if(false == filepath.Contains(".sav")) filepath += ".sav";	//확장자 추가
            using (FileStream fs = File.Open(filepath, FileMode.Open))
			{
				BinaryReader br = new BinaryReader(fs);

				// byte 배열을 Texture2D로 변환합니다.
				Texture2D tex = new Texture2D(1, 1);
                int length = br.ReadInt32();
                tex.LoadImage(br.ReadBytes(length));

                // SpriteRenderer를 사용하여 Texture2D를 Sprite로 변환하여 화면에 나타냅니다.
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
                _save_image.sprite = sprite;
            }
        }
    }
}