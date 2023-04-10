using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


namespace MyCraft
{
    public class SaveLoad : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        ////Update is called once per frame
        //void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.P))
        //    {
        //        this.Save();
        //        this.Load();
        //    }
        //}

        void Save()
        {
            //BinaryFormatter binary = new BinaryFormatter();
            //FileStream fs = File.Create(Application.persistentDataPath + "/savefile.sav");

            //SaveManager saver = new SaveManager();
            //saver.coins = 99;
            ////all other...

            //binary.Serialize(fs, saver);
            //fs.Close();

            using (FileStream fs = File.Create(Application.persistentDataPath + "/savefile.sav"))
            {
                BinaryWriter writer = new BinaryWriter(fs);

                GameManager.GetInventory().Save(writer);
                GameManager.GetQuickInven().Save(writer);

                //byte[] tmp = { 0x1, 0x2, 0x3, 0x4, 0x5 };

                //writer.Write(11);
                //writer.Write(22);
                //writer.Write(33);
                //writer.Write(tmp, 1, 4);
                //Debug.Log("seek:" + (int)writer.Seek(0, SeekOrigin.Current));

                fs.Close();
            }
        }

        void Load()
        {
            //if(File.Exists(Application.persistentDataPath + "/savefile.sav"))
            //{
            //    BinaryFormatter binary = new BinaryFormatter();
            //    FileStream fs = File.Open(Application.persistentDataPath + "/savefile.sav", FileMode.Open);
            //    SaveManager saver = (SaveManager)binary.Deserialize(fs);
            //    fs.Close();

            //    int coins = saver.coins;
            //    Debug.Log("coins : " + coins.ToString());
            //    //all stuff...

            //}

            if(File.Exists(Application.persistentDataPath + "/savefile.sav"))
            {
                using (FileStream fs = File.Open(Application.persistentDataPath + "/savefile.sav", FileMode.Open))
                {
                    BinaryReader reader = new BinaryReader(fs);

                    GameManager.GetInventory().Load(reader);
                    GameManager.GetQuickInven().Load(reader);
                    //for (int i = 0; i < 4; ++i)
                    //    Debug.Log(i + " reader:" + reader.ReadInt32());

                    fs.Close();

                }
            }

        }


    }

    //[Serializable]
    //class SaveManager
    //{
    //    public int coins;
    //    //add stuff...

    //}

}