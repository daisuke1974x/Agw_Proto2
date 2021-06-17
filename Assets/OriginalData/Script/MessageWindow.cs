using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageWindow : MonoBehaviour
{
    public bool isActive = false;
    public string MessageText="テスト！";
    private GameObject TextMeshPro;

    // Start is called before the first frame update
    void Start()
    {
        TextMeshPro = this.transform.Find("Text (TMP)").gameObject;
    }

    // Update is called once per frame
    void Update()
    {

        //表示・非表示
        if (isActive)
        {
            //this.gameObject.SetActive(true);
            //表示位置（真ん中下）
            this.transform.position = new Vector3(Screen.width / 2, 0, 0);
        }
        else
        {
            //this.gameObject.SetActive(false);
            //表示位置（真ん中下）
            this.transform.position = new Vector3(Screen.width / 2, -1000, 0);

        }

        //画面解像度に応じて大きさを変える
        float ScreenMagnificationRate = this.transform.parent.GetComponent<UI>().GetScreenMagnificationRate();
        this.transform.localScale = Vector3.one * ScreenMagnificationRate;


        //文字設定
        TextMeshPro.GetComponent<TextMeshProUGUI>().text  = MessageText;

    }
}
