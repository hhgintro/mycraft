using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SerializeTest : MonoBehaviour {
    [Serializable]
    public class Person
    {
        public int age;
        public string name;

        public override string ToString()
        {
            return "age:" + age + " name:" + name;
        }

        public void Start()
        {
            Debug.Log("age:" + age + " name:" + name);
        }
    }

    string filepath = string.Empty;
    Person p1, p2;

    // Use this for initialization
    void Start() {
        filepath = Application.dataPath + "/test.bin";

        p1 = new Person { age = 9, name = "Rain" };
        p2 = new Person { age = 11, name = "Tom" };
	}

    void OnGUI()
    {
        if (GUILayout.Button("Save"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fs = new FileStream(filepath, FileMode.Create);

            bf.Serialize(fs, p1);
            bf.Serialize(fs, p2);

            fs.Close();
        }

        if (GUILayout.Button("Load"))
        {
            if(File.Exists(filepath))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream fs = new FileStream(filepath, FileMode.Open);

                p1 = (Person)bf.Deserialize(fs);
                Debug.Log("load p1");
                p1.Start();
                p2 = (Person)bf.Deserialize(fs);
                Debug.Log("load p2");
                p2.Start();
                fs.Close();
            }
        }
    }
}
