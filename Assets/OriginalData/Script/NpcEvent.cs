using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcEvent : MonoBehaviour
{

    private string EventCode;
    private GameObject MessageWindow;
    private GameObject Player;
    private GameObject FadeInOutPanel;
    

    // Start is called before the first frame update
    void Start()
    {
        EventCode = this.GetComponent<Text>().text;
        MessageWindow = GameObject.Find("MessageWindow");
        Player = GameObject.Find("Player");
        FadeInOutPanel = GameObject.Find("FadeInOutPanel");

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Circle"))
        {
            //Debug.Log(EventCode);
            MessageWindow.GetComponent<MessageWindow>().isActive = true;
            MessageWindow.GetComponent<MessageWindow>().MessageText = this.gameObject.GetComponent<Text>().text;

            StartCoroutine("Test");
            //MessageWindow.GetComponent<MessageWindow>().isActive = false;

        }

        float dist = Vector3.Distance(Player.transform.position, this.transform.position);
        if (dist > 2f)
        {

            MessageWindow.GetComponent<MessageWindow>().isActive = false;

        }


    }


    private IEnumerator Test()
    {
        Debug.Log("Test1");

        float TimeRemain = 1;

        for (; ; )
        {
            TimeRemain -= Time.deltaTime;
            FadeInOutPanel.GetComponent<Image>().color = new Color(0, 0, 0,1- TimeRemain);
            Debug.Log(TimeRemain);
            yield return null;
            if (TimeRemain < 0) break;

        }

        yield return new WaitForSeconds(1.0f);
        Debug.Log("Test2");

        TimeRemain = 1;
        for (; ; )
        {
            TimeRemain -= Time.deltaTime;
            FadeInOutPanel.GetComponent<Image>().color = new Color(0, 0, 0, TimeRemain);
            Debug.Log(TimeRemain);
            yield return null;
            if (TimeRemain < 0) break;

        }

        //yield break;

    }

}
