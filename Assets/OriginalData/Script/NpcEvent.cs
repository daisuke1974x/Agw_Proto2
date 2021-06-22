using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcEvent : MonoBehaviour
{
    public enumNpcType NpcType; 
    public string NameJp;
    public string NameEn;

    private string EventCode;
    private bool isEventInProgress = false;
    private GameObject MessageWindow;
    private GameObject FadeInOutPanel;
    private GameObject WaitCursorObject;
    private GameObject MainControl;
    private GameObject TargetCursor;
    private GameObject SelectWindow;
    private GameObject UiObject;
    private GameObject WorldMapObject;
    private GameObject MainCameraObject;

    //public bool isPorter = false;
    public GameObject PortDistination;

    public enum enumNpcType
    {
        General,
        Porter,
        Terminal,
        DedicatedScript


    }

    // Start is called before the first frame update
    void Start()
    {
        UiObject = GameObject.Find("UI");
        MessageWindow = UiObject.transform.Find("MessageWindow").gameObject;
        WaitCursorObject = MessageWindow.transform.Find("WaitCursor").gameObject;
        SelectWindow = UiObject.transform.Find("SelectWindow").gameObject;
        FadeInOutPanel = UiObject.transform.Find("FadeInOutPanel").gameObject;

        MainControl = GameObject.Find("MainControl");
        TargetCursor = GameObject.Find("TargetCursor");
        MainCameraObject = GameObject.Find("MainCamera");

        WorldMapObject = GameObject.Find("WorldMap");

        if (!(this.GetComponent<Text>() is null))
        {
            EventCode = this.GetComponent<Text>().text;

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Circle"))
        {
            if (MainControl.GetComponent<MainControl>().isIdle ==true)
            {
                if (TargetCursor.GetComponent<TargetCursor>().objTarget == this.gameObject)
                {
                    if (isEventInProgress == false)
                    {
                        isEventInProgress = true;

                        switch (NpcType)
                        {
                            case enumNpcType.Porter:
                                StartCoroutine(GeneralPorting());
                                break;

                            case enumNpcType.Terminal:
                                StartCoroutine(EventControl());
                                break;

                            case enumNpcType.DedicatedScript:
                                string Name = this.name;
                                switch (Name)
                                {
                                    case "Tomb_FirstVillage":
                                        StartCoroutine(EventControl());
                                        break;


                                }
                                break;
                        }

                    }

                }
            }
        }

        //float dist = Vector3.Distance(Player.transform.position, this.transform.position);
        //if (dist > 2f)
        //{
        //    MessageWindow.GetComponent<MessageWindow>().isActive = false;
        //}


    }

    //---------------------------------------------------------------------------------------------------
    // イベント
    //---------------------------------------------------------------------------------------------------
    private IEnumerator EventControl()
    {

        //MessageWindow.GetComponent<MessageWindow>().MessageText = this.gameObject.GetComponent<Text>().text;

        yield return null;
        MainControl.GetComponent<MainControl>().isControllEnabled = false;
        MessageWindow.GetComponent<MessageWindow>().isActive = true;

        MessageWindow.GetComponent<MessageWindow>().MessageText = "大地の端末がある。";
        yield return StartCoroutine(WaitCursor());
        MessageWindow.GetComponent<MessageWindow>().MessageText = "端末を起動しますか？";

        GameObject SelectWindowObject = Instantiate(SelectWindow, UiObject.transform, false);
        yield return StartCoroutine(SelectWindowObject.GetComponent<SelectWindow>().YesNoWindow());
        int ReturnIndex = SelectWindowObject.GetComponent<SelectWindow>().ReturnIndex;
        Destroy(SelectWindowObject);

        if (ReturnIndex == 0){
            MessageWindow.GetComponent<MessageWindow>().MessageText = "端末を起動した。";
        }
        else
        {
            MessageWindow.GetComponent<MessageWindow>().MessageText = "端末を起動しなかった。";
        }
        yield return StartCoroutine(WaitCursor());

        MessageWindow.GetComponent<MessageWindow>().isActive = false;
        yield return null;


        //StartCoroutine(Test());
        //MessageWindow.GetComponent<MessageWindow>().isActive = false;
        isEventInProgress = false;
        MainControl.GetComponent<MainControl>().isControllEnabled = true;

    }

    //---------------------------------------------------------------------------------------------------
    // 汎用移動処理
    //---------------------------------------------------------------------------------------------------
    private IEnumerator GeneralPorting()
    {
        string DistName = PortDistination.name;
        string DistNameJp = PortDistination.GetComponent<NpcEvent>().NameJp;
        Vector3 DistPos = PortDistination.transform.position;
        yield return null;
        MainControl.GetComponent<MainControl>().isControllEnabled = false;
        MessageWindow.GetComponent<MessageWindow>().isActive = true;

        MessageWindow.GetComponent<MessageWindow>().MessageText = DistNameJp +" へ移動します。\nよろしいですか？";

        GameObject SelectWindowObject = Instantiate(SelectWindow, UiObject.transform, false);
        yield return StartCoroutine(SelectWindowObject.GetComponent<SelectWindow>().YesNoWindow());
        int ReturnIndex = SelectWindowObject.GetComponent<SelectWindow>().ReturnIndex;
        Destroy(SelectWindowObject);

        if (ReturnIndex == 0)
        {
            GameObject Player = GameObject.Find("Player");
            yield return StartCoroutine(FadeOut(1));//フェードアウト
            MainControl.GetComponent<MainControl>().LoadMap(PortDistination.transform.parent.transform.parent.name, this.gameObject);//2021．6.20第二パラメータを追加。自分自身のオブジェクトは後でDestroyするよう修正。これをしないと、この後のコルーチンが正常に動作しなくなるため。

            // プレイヤーの位置の移動（ワープ）
            //  2021.6.23 うまく移動できるときとそうでないときがあって悩んだが、ググったら原因が判明。enabled = false
            //  https://gametukurikata.com/basic/pitfallsofcharactercontroller
            Player.GetComponent<CharacterController>().enabled = false;
            Player.transform.position = DistPos + new Vector3(0, 0, -2);
            MainControl.GetComponent<MainControl>().Landing(Player);
            Player.GetComponent<CharacterController>().enabled = true;

            MainCameraObject.GetComponent<MainCamera>().SetHomePosition();

            MessageWindow.GetComponent<MessageWindow>().isActive = false;//ウィンドウを消す
            this.gameObject.transform.position = new Vector3(10000f, 10000f, 10000f);//このイベントオブジェクトを見えない位置に移動する
            yield return StartCoroutine(FadeIn(1));//フェードイン
            isEventInProgress = false;
            MainControl.GetComponent<MainControl>().isControllEnabled = true;
            Destroy(this.gameObject);
            yield break;
        }


        MessageWindow.GetComponent<MessageWindow>().isActive = false;
        isEventInProgress = false;
        MainControl.GetComponent<MainControl>().isControllEnabled = true;

    }
    //private IEnumerator LoadMap(string pMapName )
    //{
    //    Debug.Log(pMapName);
    //    MainControl.GetComponent<MainControl>().LoadMap(pMapName);
    //    yield return null;
    //}




    




    private IEnumerator Test()
    {
        yield return StartCoroutine(FadeOut(1));
        yield return new WaitForSeconds(1.0f);
        yield return StartCoroutine(FadeIn(1));
    }


    //-----------------------------------------------------------------------------------------------------
    // FadeOut
    //-----------------------------------------------------------------------------------------------------
    private IEnumerator FadeOut(float TimeRequired)
    {
        float TimeRemain = TimeRequired;
        for (; ; )
        {
            TimeRemain -= Time.deltaTime;
            FadeInOutPanel.GetComponent<Image>().color = new Color(0, 0, 0, 1 - (TimeRemain/ TimeRequired));
            yield return null;
            if (TimeRemain < 0) yield break;
        }

    }
    //-----------------------------------------------------------------------------------------------------
    // FadeIn
    //-----------------------------------------------------------------------------------------------------
    private IEnumerator FadeIn(float TimeRequired)
    {
        float TimeRemain = TimeRequired;
        for (; ; )
        {
            TimeRemain -= Time.deltaTime;
            FadeInOutPanel.GetComponent<Image>().color = new Color(0, 0, 0,  (TimeRemain / TimeRequired));
            yield return null;
            if (TimeRemain < 0) yield break;
        }
    }

    //-----------------------------------------------------------------------------------------------------
    // WaitCursor
    //-----------------------------------------------------------------------------------------------------
    private IEnumerator WaitCursor()
    {
        float WaitTime= 0.5f;
        float TimeRemain = 0.5f;
        int ViewSwitch = 1;
        for (; ; )
        {
            TimeRemain -= Time.deltaTime;
            if (TimeRemain < 0)
            {
                ViewSwitch = 1 - ViewSwitch;
                TimeRemain = WaitTime;
            }
            if (ViewSwitch == 1)
            {
                WaitCursorObject.SetActive(true);
            }
            else
            {
                WaitCursorObject.SetActive(false);
            }
            yield return null;
            if (Input.GetButtonDown("Circle")) break;
        }
        WaitCursorObject.SetActive(false);
    }

    private GameObject SearchNpc(string pName)
    {

        for (int Index = 0; Index < WorldMapObject.transform.GetChildCount(); Index++)
        {
            GameObject SearchMap = WorldMapObject.transform.GetChild(Index).gameObject;
            if (!(SearchMap.transform.FindChild("Npc/" + pName) is null))
            {
                return SearchMap.transform.FindChild("Npc/" + pName).gameObject;
            }
        }
        return null;
    }

}
