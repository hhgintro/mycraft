//using SonicBloom.Koreo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(ExcelLoader))]
public class ExcelLoaderEditor : Editor
{

    ExcelLoader myTarget;

    private void OnEnable()
    {
        myTarget = (ExcelLoader)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if(GUILayout.Button("clear"))
        {
            Clear();
            Debug.Log("clear note");
        }
        if (GUILayout.Button("load"))
        {
            Load(myTarget.ExcelFile);
            Debug.Log(myTarget.ExcelFile.ToString() + " : load clicked");
        }
        if (GUILayout.Button("save"))
        {
            Save(myTarget.ExcelFile);
            Debug.Log(myTarget.ExcelFile.ToString() + " : save clicked");
        }
    }
    private void Clear()
    {
        //GameObject obj = GameObject.Find("BezierCurve");
        ////bezier 정보를 삭제하고.
        //BezierCurve bezier = obj.GetComponent<BezierCurve>();
        //bezier.RemoveAllPoint();
        ////unity에 obj를 지운다.
        //Debug.Log(obj.transform.childCount);
        //for (int i = obj.transform.childCount - 1; i >= 0; --i)
        //{
        //    Transform child = obj.transform.GetChild(i);
        //    DestroyImmediate(child.gameObject);
        //}
    }

    private void Load(string filename)
    {
        //string strFile = "testcsv.csv";

        using (FileStream fs = new FileStream(filename, FileMode.Open))
        {
            using (StreamReader sr = new StreamReader(fs, Encoding.UTF8, false))
            {
                string strLineValue = null;
                string[] keys = null;
                string[] values = null;

                while ((strLineValue = sr.ReadLine()) != null)
                {
                    // Must not be empty.
                    if (string.IsNullOrEmpty(strLineValue)) return;

                    if (strLineValue.Substring(0, 1).Equals("#"))
                    {
                        keys = strLineValue.Split(',');

                        keys[0] = keys[0].Replace("#", "");

                        //Debug.Log("Key : ");
                        //Console.Write("Key : ");
                        // Output
                        for (int nIndex = 0; nIndex < keys.Length; nIndex++)
                        {
                            //Debug.Log(keys[nIndex]);
                            //Console.Write(keys[nIndex]);
                            if (nIndex != keys.Length - 1)
                            {
                                //Debug.Log(", ");
                                //Console.Write(", ");
                            }
                        }

                        //Debug.Log("");
                        //Console.WriteLine();

                        continue;
                    }

                    values = strLineValue.Split(',');


                    //GameObject obj = GameObject.Find("BezierCurve");
                    //BezierCurve bezier = obj.GetComponent<BezierCurve>();

                    ////Debug.Log("Value : ");
                    ////Console.Write("Value : ");
                    //// Output
                    //for (int i = 0; i < values.Length; ++i)
                    //{
                    //    //Debug.Log(values[i]);
                    //    //Console.Write(values[i]);
                    //    if (i != values.Length - 1)
                    //    {
                    //        //Debug.Log(", ");
                    //        //Console.Write(", ");
                    //    }
                    //}

                    ////pos(x/y/z),style,handle1(x/y/z),handle2(x/y/z)
                    //GameObject pointObject = new GameObject("Point " + obj.transform.childCount);
                    //pointObject.transform.parent = obj.transform;
                    ////pointObject.transform.localPosition = Vector3.zero;
                    //pointObject.transform.localPosition = new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));

                    //BezierPoint newPoint = pointObject.AddComponent<BezierPoint>();
                    //newPoint.curve = bezier;
                    //newPoint.handleStyle = (BezierPoint.HandleStyle)int.Parse(values[3]);
                    //newPoint.handle1 = new Vector3(float.Parse(values[4]), float.Parse(values[5]), float.Parse(values[6]));
                    //newPoint.handle2 = new Vector3(float.Parse(values[7]), float.Parse(values[8]), float.Parse(values[9]));

                    ////Debug.Log("");
                    ////Console.WriteLine();
                }
            }
            Debug.Log(filename + " load completed");
        }
    }

    private void Save(string filename)
    {
        //GameObject obj = GameObject.Find("BezierCurve");
        //BezierCurve bezier = obj.GetComponent<BezierCurve>();

        ////string strFile = "testcsv.csv";

        ////using (FileStream fs = new FileStream(strFile, FileMode.Open))
        ////{
        //using (StreamWriter wr = new StreamWriter(filename, false, Encoding.UTF8))
        //{
        //    ////Unit m_Unit = gameObject.AddComponent("Marine") as Unit;
        //    ////Koreographer grapher = new Koreographer();
        //    ////int cnt = grapher.GetNumLoadedKoreography();
        //    ////Debug.Log("cnt:" + cnt.ToString());

        //    ////DataTable table1 = new DataTable("NOTE");
        //    ////table1.Columns.Add("CODE_NAME");
        //    ////table1.Columns.Add("CODE_VALUE");
        //    ////table1.Rows.Add("배달", 1);
        //    ////table1.Rows.Add("심부름", 2);
        //    ////ds.Tables.Add(table1);


        //    ////track개수(piano / pathtrack)
        //    //for (int t = 0; t < myTarget.playingKoreo.Tracks.Count; ++t)
        //    //{
        //    //    KoreographyTrackBase rhythmTrack = myTarget.playingKoreo.Tracks[t];
        //    //    List<KoreographyEvent> rawEvents = rhythmTrack.GetAllEvents();

        //    //    //track이 시작될 때
        //    //    //sheet명/컬럼명을 정해줍니다.
        //    //    //DataTable table1 = null;
        //    //    //switch (t)
        //    //    //{
        //    //    //    case 0: //Note
        //    //    //    case 1: //path
        //    //    //        table1 = new DataTable(rhythmTrack.EventID);//sheet명
        //    //    //        table1.Columns.Add("START_SAMPLE");     //컬럼명
        //    //    //        table1.Columns.Add("END_SAMPLE");       //컬럼명
        //    //    //        table1.Columns.Add("PAYLOAD");          //컬럼명
        //    //    //        break;
        //    //    //}

        //    //    //key
        //    //    wr.WriteLine("#startsample,endsampe,textpayload");

        //    //    for (int i = 0; i < rawEvents.Count; ++i)
        //    //    {
        //    //        KoreographyEvent evt = rawEvents[i];
        //    //        string payload = evt.GetTextValue();

        //    //        int ss = evt.StartSample;
        //    //        int es = evt.EndSample;
        //    //        string txt = (null == evt.Payload) ? "" : ((TextPayload)evt.Payload).TextVal;
        //    //        //Debug.Log(txt);
        //    //        //string[] sArray = LoadEventText(txt);
        //    //        //Debug.Log(evt.StartSample + "/" + evt.EndSample + "/" + txt);
        //    //        //table1.Rows.Add(evt.StartSample, evt.EndSample, txt);

        //    //        wr.WriteLine(evt.StartSample
        //    //            + "," + evt.EndSample
        //    //            + "," + txt);

        //    //        ////HG_TEST
        //    //        ////GameObject noteobj = GameObject.Instantiate<NoteData>(prefNote);
        //    //        //float current = (float)evt.StartSample / audioCom.clip.samples;
        //    //        //NoteData noteobj = new NoteData(GameObject.Instantiate<GameObject>(prefNote), current);
        //    //        ////noteobj.transform.position = bezier.GetPointAt(current);
        //    //        //noteobj._obj.transform.position = bezier.GetPointAt(current);
        //    //        //if (i == rawEvents.Count - 1)
        //    //        //    Debug.Log("last event : " + i.ToString() + " time : " + current.ToString() + " pos: " + noteobj._obj.transform.position.ToString());
        //    //        //noteList.Add(noteobj);
        //    //    }
        //    //}
        //    ////KoreographyTrackBase rhythmTrack = myTarget.playingKoreo.GetTrackByID("Piano");
        //    ////Debug.Log("cnt:" + rhythmTrack.GetAllEvents().Count.ToString());
        //    ////rhythmTrack = myTarget.playingKoreo.GetTrackByID("hg_PathTrack");
        //    ////Debug.Log("cnt:" + rhythmTrack.GetAllEvents().Count.ToString());

        //    ////int cnt = myTarget.playingKoreo.GetNumLoadedKoreography();
        //    ////Debug.Log("cnt:" + cnt.ToString());

        //    ////if (null == Koreographer.Instance)
        //    ////    Koreographer.Instance = new Koreographer();
        //    ////int cnt = Koreographer.Instance.GetNumLoadedKoreography();
        //    ////Debug.Log("cnt:" + cnt.ToString());
        //    ////for(int i=0; i<cnt; ++i)
        //    ////{
        //    ////    Koreography koreo = Koreographer.Instance.GetKoreographyAtIndex(i);
        //    ////    Debug.Log(koreo.SourceClipName.ToString());
        //    ////    Debug.Log(koreo.SourceClipPath.ToString());

        //    ////}

        //    //key
        //    wr.WriteLine("#position(x/y/z),style,handle1(x/y/z),handle2(x/y/z)");

        //    for (int i = 0; i < bezier.pointCount; ++i)
        //    {
        //        //Debug.Log("pos:" + bezier[i].position.ToString());
        //        //Debug.Log("stype:" + bezier[i].handleStyle.ToString()
        //        //    + " hand1:" + bezier[i].handle1.ToString()
        //        //    + " hand2:" + bezier[i].handle2.ToString());
        //        wr.WriteLine(bezier[i].position.x + "," + bezier[i].position.y + "," + bezier[i].position.z
        //            + "," + (int)bezier[i].handleStyle
        //            + "," + bezier[i].handle1.x + "," + bezier[i].handle1.y + "," + bezier[i].handle1.z
        //            + "," + bezier[i].handle2.x + "," + bezier[i].handle2.y + "," + bezier[i].handle2.z);
        //    }


        //    wr.Flush();
        //    wr.Close();
        //}

    }


    public string[] LoadEventText(string text)
    {
        // split the items
        string[] sArray = text.Split('|');

        //// store as a Vector3
        //Vector3 forward = StringToVector3(sArray[0]).normalized;
        //int style = int.Parse(sArray[1]);
        //Vector3 h1 = StringToVector3(sArray[2]);
        //Vector3 h2 = StringToVector3(sArray[3]);

        return sArray;
    }

}