using MyCraft;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatManager : MonoBehaviour
{
    public List<string> chatList = new List<string>();

    public GameObject content;
    public TMP_InputField input;
    public bool ready;  //("")문자를 입력하면 창을 닫는다.

    GameObject contentText; //content의 child

    private void Start()
    {
        contentText = content.transform.GetChild(0).gameObject;
    }

    //void Update()
    //{
    //    //if (Input.GetKeyDown(KeyCode.Return) && input.isFocused == false)
    //    //{
    //    //    input.ActivateInputField();
    //    //}
    //}

    private void OnEnable()
    {
        ready = true;
        input.ActivateInputField();
    }

    private void ChatMessage(string message)
    {
        GameObject goText = Instantiate(contentText, content.transform);
        goText.GetComponent<TextMeshProUGUI>().text = message;
        content.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
    }
    public void AddChatMessage(string message)
    {
        if (message.Equals(""))
        {
            ready = false;  //창을 닫기위해
            return;
        }

        ChatMessage(message);   //TextMeshProUGUI를 그려준다.
        input.ActivateInputField();
        input.text = "";

        if(IsCheat(message))
        {

            return;
        }
    }

    private bool IsCheat(string message)
    {
        if (false == message.Substring(0, 1).Equals("/"))
            return false;

        string[] words = message.Substring(1).Split(' ');
        if (words.Length <= 0) return false;

        if (true == Cheat_Help(words))          return true;
        if (true == Cheat_Mineral(words))       return true;


        return true;
    }

    bool Cheat_Help(string[] words)
    {
        if (words.Length <= 0) return false;
        if (false == words[0].Equals("?"))
            return false;

        string message = string.Format(" - mineral [itemid] [x] [y] [z]");
        ChatMessage(message);
        return true;
    }

    bool Cheat_Mineral(string[] words)
    {
        if (false == words[0].Equals("mineral"))
            return false;

        short itemid = short.Parse(words[1]);
        int x = int.Parse(words[2]);
        int y = int.Parse(words[3]);
        int z = int.Parse(words[4]);

        GameManager.GetTerrainManager().PutdownBlock(itemid, 1, x, y, z);  //iron-ore
        return true;
    }
}
