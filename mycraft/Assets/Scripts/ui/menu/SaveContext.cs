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
        public Transform _save_file_content;    //저장파일을 정렬할 위치

        private Text _save_file;    //선택한 저장파일의 파일명
        private Image _save_image;  //선택된 저장파일의 이미지
        private List<GameObject> _contexts = new List<GameObject>();    //_save_file_content에 등록된 개체정보

        protected void Init()
        {
            this._save_file = this.transform.Find("Load File/Load File Info/Title/Text").GetComponent<Text>();
            this._save_image = this.transform.Find("Load File/Load File Info/ScreenCapture").GetComponent<Image>();
            this._save_file.text = "";
		}

        protected virtual void OnSelectSaveFile(string filename)
        {
            //Debug.Log($"select:{filename}");
            this._save_file.text = filename;
            LoadScreenShot(filename);
		}

        protected void RefreshContext()
        {
            //기존꺼 삭제
            foreach (var obj in this._contexts) Managers.Resource.Destroy(obj);
            this._contexts.Clear();
            //재 등록
            string[] savefiles = Common.GetFilesInFolderWithExtensionOrderByCreationTime(Managers.Game._save_dir, "sav");
            foreach (string file in savefiles)
            {
                //Debug.Log(file);
                GameObject clone = Managers.Resource.Instantiate("Prefabs/ui/Save-Filename", this.transform);
                clone.name = file;
                clone.transform.GetChild(0).GetComponent<Text>().text = file;
                clone.transform.parent = this._save_file_content;
                clone.GetComponent<Button>().onClick.AddListener(() => OnSelectSaveFile(file));
                this._contexts.Add(clone);
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
			int a = 0;
			a = 0;
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