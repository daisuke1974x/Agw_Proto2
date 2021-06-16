using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcEvent : MonoBehaviour
{

    private string EventCode;
    private bool isEventInProgress = false;
    private GameObject MessageWindow;
    private GameObject Player;
    private GameObject FadeInOutPanel;
    private GameObject WaitCursorObject;
    private GameObject MainControl;


    // Start is called before the first frame update
    void Start()
    {
        EventCode = this.GetComponent<Text>().text;
        MessageWindow = GameObject.Find("MessageWindow");
        Player = GameObject.Find("Player");
        FadeInOutPanel = GameObject.Find("FadeInOutPanel");
        WaitCursorObject = GameObject.Find("WaitCursor");
        //WaitCursorObject.SetActive(false);
        MainControl = GameObject.Find("MainControl");

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Circle"))
        {
            if (MainControl.GetComponent<MainControl>().isIdle ==true)
            {
                if (isEventInProgress == false)
                {
                    isEventInProgress = true;
                    StartCoroutine(EventControl());
                }
            }
        }

        //float dist = Vector3.Distance(Player.transform.position, this.transform.position);
        //if (dist > 2f)
        //{
        //    MessageWindow.GetComponent<MessageWindow>().isActive = false;
        //}


    }

     private IEnumerator EventControl()
    {

        MainControl.GetComponent<MainControl>().isControllEnabled = false;
//        yield return null;
        //Debug.Log(EventCode);
        MessageWindow.GetComponent<MessageWindow>().isActive = true;
        MessageWindow.GetComponent<MessageWindow>().MessageText = this.gameObject.GetComponent<Text>().text;
        yield return StartCoroutine(WaitCursor());
//        yield return null;
        MessageWindow.GetComponent<MessageWindow>().MessageText = "テキスト２";
        yield return StartCoroutine(WaitCursor());
        MessageWindow.GetComponent<MessageWindow>().isActive = false;
        yield return null;
        //        yield return null;


        //StartCoroutine(Test());
        //MessageWindow.GetComponent<MessageWindow>().isActive = false;
        isEventInProgress = false;
        MainControl.GetComponent<MainControl>().isControllEnabled = true;

    }

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
            if (TimeRemain < 0) break;
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
            if (TimeRemain < 0) break;
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

}
