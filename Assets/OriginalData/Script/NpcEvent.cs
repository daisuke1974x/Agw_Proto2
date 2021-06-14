using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcEvent : MonoBehaviour
{

    private string EventCode;

    // Start is called before the first frame update
    void Start()
    {
        EventCode = this.GetComponent<Text>().text;


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Circle"))
        {
            Debug.Log(EventCode);



        }
    }
}
