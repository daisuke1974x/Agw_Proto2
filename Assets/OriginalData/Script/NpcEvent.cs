using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcEvent : MonoBehaviour
{

    private string EventCode;
    private GameObject MessageWindow;
    private GameObject Player;

    // Start is called before the first frame update
    void Start()
    {
        EventCode = this.GetComponent<Text>().text;
        MessageWindow = GameObject.Find("MessageWindow");
        Player = GameObject.Find("Player");

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Circle"))
        {
            Debug.Log(EventCode);
            MessageWindow.GetComponent<MessageWindow>().isActive = true;
            MessageWindow.GetComponent<MessageWindow>().MessageText = this.gameObject.GetComponent<Text>().text;
        }

        float dist = Vector3.Distance(Player.transform.position, this.transform.position);
        if (dist > 2f)
        {

            MessageWindow.GetComponent<MessageWindow>().isActive = false;

        }

    }
}
