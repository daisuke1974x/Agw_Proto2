using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_FieldCursor : MonoBehaviour
{
    Color DefaultColorBottom;
    Color DefaultColorWall;

    public GameObject Wall01;
    public GameObject Wall02;
    public GameObject Wall03;
    public GameObject Wall04;
    public GameObject Bottom;

    enum EnumMode
    {
        Idle,
        Apper,
        Disapper
    }
    EnumMode Mode = EnumMode.Idle;

    private float TimeCounter=0;

    // Start is called before the first frame update
    void Start()
    {
        DefaultColorBottom = Bottom.GetComponent<Renderer>().material.color;
        DefaultColorWall = Wall01.GetComponent<Renderer>().material.color;


        //Material mat = wall01.GetComponent<Renderer>().material;
        //Color col = wall01.GetComponent<Renderer>().material.color;
        //col.a = 0;
        //wall01.GetComponent<Renderer>().material.color = col;
        
    }

    //private float TimeStep1 = 0.75f;
    private float TimeStep1 = 1f;

    // Update is called once per frame
    void Update()
    {
        TimeCounter += Time.deltaTime;

        Color col;

        switch (Mode){
             case EnumMode.Idle:
                //Bottom.GetComponent<Renderer>().material.color = DefaultColorBottom;
                //Wall01.GetComponent<Renderer>().material.color = DefaultColorWall;
                //Wall02.GetComponent<Renderer>().material.color = DefaultColorWall;
                //Wall03.GetComponent<Renderer>().material.color = DefaultColorWall;
                //Wall04.GetComponent<Renderer>().material.color = DefaultColorWall;
                break;

            case EnumMode.Apper:
                if (TimeStep1 < TimeCounter)
                {
                    TimeCounter = TimeStep1;
                    Mode = EnumMode.Idle;
                }

                col = DefaultColorBottom;
                col.a *= (TimeCounter / TimeStep1);
                Bottom.GetComponent<Renderer>().material.color = col;

                col = DefaultColorWall;
                col.a *= (TimeCounter / TimeStep1);
                Wall01.GetComponent<Renderer>().material.color = col;
                Wall01.GetComponent<Renderer>().material.color = col;
                Wall01.GetComponent<Renderer>().material.color = col;
                Wall01.GetComponent<Renderer>().material.color = col;
                break;

            case EnumMode.Disapper:

                if (TimeStep1 < TimeCounter)
                {
                    TimeCounter = TimeStep1;
                    Mode = EnumMode.Idle;
                    this.transform.position = new Vector3(0, 0, 100000);
                }


                col = DefaultColorBottom;
                col.a *= (TimeStep1 - TimeCounter) / TimeStep1;
                Bottom.GetComponent<Renderer>().material.color = col;

                col = DefaultColorWall;
                col.a *= (TimeStep1 - TimeCounter) / TimeStep1;
                Wall01.GetComponent<Renderer>().material.color = col;
                Wall01.GetComponent<Renderer>().material.color = col;
                Wall01.GetComponent<Renderer>().material.color = col;
                Wall01.GetComponent<Renderer>().material.color = col;
                break;
        }

    }

    public void Appear()
    {
        if (Mode == EnumMode.Idle || Mode == EnumMode.Disapper)
        {
            TimeCounter = 0;
            Mode = EnumMode.Apper;

        }


    }

    public void Disappear()
    {
        if (Mode == EnumMode.Idle)
        {
            TimeCounter = 0;
            Mode = EnumMode.Disapper;

        }



    }

}
