using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_Chest : MonoBehaviour
{
    //入力関連
    private float inputHorizontal;
    private float inputVertical;
    private float inputHorizontalCamera;
    private float inputVerticalCamera;

    public GameObject objChestOpen;
    public GameObject objChestClose;
    public bool isOpen = false;
    public GameObject objTargetCursor;
    private GameObject Hierarchy_UnTargetObject;
    private bool isDestroying = false;
    private float DestroyCounter = 0;
    private float DestroyStartTime = 5;
    private float DestroyingTime = 1;

    public GameObject objPlayer;
    public GameObject objMainControl;
    public GameObject objField;
    public GameObject FragmentCapsule;


    private s_FragmentsList FragmentsListScript;

    // Start is called before the first frame update
    void Start()
    {
        objChestClose = this.transform.GetChild(0).gameObject;
        objChestOpen = this.transform.GetChild(1).gameObject;
        objTargetCursor = GameObject.Find("TargetCursor");
        Hierarchy_UnTargetObject = GameObject.Find("UnTargetObject");
        objPlayer = GameObject.Find("Player");
        objMainControl = GameObject.Find("MainControl");
        objField = GameObject.Find("Field");
        //FragmentCapsule = GameObject.Find("FragmentCapsule");

        objChestOpen.SetActive(false);
        FragmentsListScript = GameObject.Find("FragmentsList").GetComponent<s_FragmentsList>();

    }

    // Update is called once per frame
    void Update()
    {
        if (isDestroying == false)
        {
            if (Input.GetButtonDown("Circle") == true)
            {
                GameObject objTarget = objTargetCursor.GetComponent<s_TargetCursor>().objTarget;
                if (objTarget == this.gameObject)
                {
                    objChestClose.SetActive(false);
                    objChestOpen.SetActive(true);
                    isOpen = true;
                    this.transform.parent = Hierarchy_UnTargetObject.transform;
                    isDestroying = true;
                    DestroyCounter = 0;
                    DropControl();
                    GameObject objInstance = Instantiate(FragmentCapsule );
                    objInstance.transform.position = this.transform.position + new Vector3(0, 1f, 0);
                    //Destroy(this.gameObject, 5f);

                    //+ new Vector3(0, 0.5f, 0)
                }
            }

        }
        else
        {
            DestroyCounter += 1 * Time.deltaTime;
            if (DestroyCounter > DestroyStartTime)
            {
                Vector3 Scale = this.transform.localScale;
                Scale.x = (DestroyingTime - (DestroyCounter - DestroyStartTime)) / DestroyingTime;
                Scale.y = (DestroyingTime - (DestroyCounter - DestroyStartTime)) / DestroyingTime;
                Scale.z = (DestroyingTime - (DestroyCounter - DestroyStartTime)) / DestroyingTime;
                this.transform.localScale = Scale;
                if (DestroyCounter > DestroyingTime + DestroyStartTime)
                {
                    Destroy(this.gameObject);
                }

            }


        }

    }

    private void DropControl()
    {
        //ストック番号の採番
        string StockName = "";
        bool isFound = false;
        int Index = 1;
        while (isFound == false)
        {
            StockName = "Stock_" + Index.ToString();
            GameObject SearchObject = GameObject.Find(StockName);
            if (SearchObject is null) isFound = true;
            Index++;
        }


        //ストックの作成
        s_Main ScriptMain = objMainControl.GetComponent<s_Main>();
        int Rnd = 0;
        Rnd= Random.Range(0, 100);

        //緑地系
        if (0<= Rnd && Rnd < 60)
        {
            Rnd = Random.Range(0, 100);
            if (0<=Rnd&&Rnd < 50)
            {
                ScriptMain.LoadFieldParts(StockName, "Green", 0, 0, 0, 0);
                FragmentsListScript.AddList(StockName);
                return;
            }
            if (50 <= Rnd && Rnd < 70)
            {
                ScriptMain.LoadFieldParts(StockName, "Green", 0, 0, 0, 0);
                ScriptMain.LoadFieldParts(StockName, "Green", 1, 0, 0, 0);
                FragmentsListScript.AddList(StockName);
                return;
            }
            if (70 <= Rnd && Rnd < 80)
            {
                ScriptMain.LoadFieldParts(StockName, "Green", 0, 0, 0, 0);
                ScriptMain.LoadFieldParts(StockName, "Green", 1, 0, 0, 0);
                ScriptMain.LoadFieldParts(StockName, "Green", -1, 0, 0, 0);
                FragmentsListScript.AddList(StockName);
                return;
            }
            if (80 <= Rnd && Rnd < 85)
            {
                ScriptMain.LoadFieldParts(StockName, "Green", 0, 0, 0, 0);
                ScriptMain.LoadFieldParts(StockName, "Green", 1, 0, 0, 0);
                ScriptMain.LoadFieldParts(StockName, "Green", 0, 0, 1, 0);
                FragmentsListScript.AddList(StockName);
                return;
            }
            if (85 <= Rnd && Rnd < 88)
            {
                ScriptMain.LoadFieldParts(StockName, "Green", 0, 0, 0, 0);
                ScriptMain.LoadFieldParts(StockName, "Green", 1, 0, 0, 0);
                ScriptMain.LoadFieldParts(StockName, "Green", 0, 0, 1, 0);
                ScriptMain.LoadFieldParts(StockName, "Green", 0, 0, 2, 0);
                FragmentsListScript.AddList(StockName);
                return;
            }
            if (88 <= Rnd && Rnd < 91)
            {
                ScriptMain.LoadFieldParts(StockName, "Green", 0, 0, 0, 0);
                ScriptMain.LoadFieldParts(StockName, "Green", -1, 0, 0, 0);
                ScriptMain.LoadFieldParts(StockName, "Green", 0, 0, 1, 0);
                ScriptMain.LoadFieldParts(StockName, "Green", 0, 0, 2, 0);
                FragmentsListScript.AddList(StockName);
                return;
            }
            if (91 <= Rnd && Rnd < 100)
            {
                ScriptMain.LoadFieldParts(StockName, "Green", 0, 0, 0, 0);
                ScriptMain.LoadFieldParts(StockName, "Green", 1, 0, 1, 0);
                ScriptMain.LoadFieldParts(StockName, "Green", 0, 0, 1, 0);
                ScriptMain.LoadFieldParts(StockName, "Green", 1, 0, 0, 0);
                FragmentsListScript.AddList(StockName);
                return;
            }


        }

        //道路系
        if (60 <= Rnd && Rnd < 90)
        {
            Rnd = Random.Range(0, 100);
            if (0 <= Rnd && Rnd < 70)
            {
                Rnd = Random.Range(0, 100);
                if (0 <= Rnd && Rnd < 50)
                {
                    ScriptMain.LoadFieldParts(StockName, "Road1", 0, 0, 0, 0);
                    FragmentsListScript.AddList(StockName);
                    return;
                }
                if (50 <= Rnd && Rnd < 60)
                {
                    ScriptMain.LoadFieldParts(StockName, "Road0", 0, 0, 0, 0);
                    FragmentsListScript.AddList(StockName);
                    return;
                }
                if (60 <= Rnd && Rnd < 80)
                {
                    ScriptMain.LoadFieldParts(StockName, "Road2", 0, 0, 0, 0);
                    FragmentsListScript.AddList(StockName);
                    return;
                }
                if (80 <= Rnd && Rnd < 95)
                {
                    ScriptMain.LoadFieldParts(StockName, "Road3", 0, 0, 0, 0);
                    FragmentsListScript.AddList(StockName);
                    return;
                }
                if (95 <= Rnd && Rnd < 100)
                {
                    ScriptMain.LoadFieldParts(StockName, "Road4", 0, 0, 0, 0);
                    FragmentsListScript.AddList(StockName);
                    return;
                }

            }
            if (70 <= Rnd && Rnd < 100)
            {
                if (0 <= Rnd && Rnd < 50)
                {
                    ScriptMain.LoadFieldParts(StockName, "Road1", 0, 0, 0, 0);
                    ScriptMain.LoadFieldParts(StockName, "Road1", 1, 0, 0, 0);
                    FragmentsListScript.AddList(StockName);
                    return;
                }
                if (50 <= Rnd && Rnd < 70)
                {
                    ScriptMain.LoadFieldParts(StockName, "Road2", 0, 0, 0, 0);
                    ScriptMain.LoadFieldParts(StockName, "Road1", 1, 0, 0, 1);
                    FragmentsListScript.AddList(StockName);
                    return;
                }
                if (70 <= Rnd && Rnd < 90)
                {
                    ScriptMain.LoadFieldParts(StockName, "Road2", 0, 0, 0, 2);
                    ScriptMain.LoadFieldParts(StockName, "Road1", -1, 0, 0,1);
                    FragmentsListScript.AddList(StockName);
                    return;
                }
                if (90 <= Rnd && Rnd < 94)
                {
                    ScriptMain.LoadFieldParts(StockName, "Road3", 0, 0, 0, 0);
                    ScriptMain.LoadFieldParts(StockName, "Road1", -1, 0, 0, 1);
                    FragmentsListScript.AddList(StockName);
                    return;
                }
                if (94 <= Rnd && Rnd < 98)
                {
                    ScriptMain.LoadFieldParts(StockName, "Road3", 0, 0, 0, 0);
                    ScriptMain.LoadFieldParts(StockName, "Road1", 1, 0, 0, 1);
                    FragmentsListScript.AddList(StockName);
                    return;
                }
                if (98 <= Rnd && Rnd < 100)
                {
                    ScriptMain.LoadFieldParts(StockName, "Road3", 0, 0, 0, 0);
                    ScriptMain.LoadFieldParts(StockName, "Road1", 0, 0, 1, 0);
                    FragmentsListScript.AddList(StockName);
                    return;
                }
            }


        }

        //河川系
        if (90 <= Rnd && Rnd < 100)
        {
            Rnd = Random.Range(0, 100);
            if (0 <= Rnd && Rnd < 60)
            {
                Rnd = Random.Range(0, 100);
                if (0 <= Rnd && Rnd < 60)
                {
                    ScriptMain.LoadFieldParts(StockName, "River1", 0, 0, 0, 0);
                    FragmentsListScript.AddList(StockName);
                    return;

                }
                if (60 <= Rnd && Rnd < 80)
                {
                    ScriptMain.LoadFieldParts(StockName, "River2", 0, 0, 0, 0);
                    FragmentsListScript.AddList(StockName);
                    return;

                }
                if (80 <= Rnd && Rnd < 90)
                {
                    ScriptMain.LoadFieldParts(StockName, "RiverBridge", 0, 0, 0, 0);
                    FragmentsListScript.AddList(StockName);
                    return;

                }
                if (90 <= Rnd && Rnd < 100)
                {
                    ScriptMain.LoadFieldParts(StockName, "River3", 0, 0, 0, 0);
                    FragmentsListScript.AddList(StockName);
                    return;

                }

            }
            if (60 <= Rnd && Rnd < 100)
            {
                Rnd = Random.Range(0, 100);
                if (0 <= Rnd && Rnd < 60)
                {
                    ScriptMain.LoadFieldParts(StockName, "River1", 0, 0, 0, 0);
                    ScriptMain.LoadFieldParts(StockName, "River1", 0, 0, 1, 0);
                    FragmentsListScript.AddList(StockName);
                    return;
                }
                if (60 <= Rnd && Rnd < 80)
                {
                    ScriptMain.LoadFieldParts(StockName, "River1", 0, 0, 1, 0);
                    ScriptMain.LoadFieldParts(StockName, "River2", 0, 0, 0, 3);
                    FragmentsListScript.AddList(StockName);
                    return;

                }
                if (80 <= Rnd && Rnd < 90)
                {
                    ScriptMain.LoadFieldParts(StockName, "River1", 0, 0, 1, 0);
                    ScriptMain.LoadFieldParts(StockName, "River2", 0, 0, 0, 2);
                    FragmentsListScript.AddList(StockName);
                    return;
                }
                if (90 <= Rnd && Rnd < 94)
                {
                    ScriptMain.LoadFieldParts(StockName, "River3", 0, 0, 0, 0);
                    ScriptMain.LoadFieldParts(StockName, "River1", -1, 0, 0, 0);
                    FragmentsListScript.AddList(StockName);
                    return;
                }
                if (94 <= Rnd && Rnd < 98)
                {
                    ScriptMain.LoadFieldParts(StockName, "River3", 0, 0, 0, 0);
                    ScriptMain.LoadFieldParts(StockName, "River1", 1, 0, 0, 0);
                    FragmentsListScript.AddList(StockName);
                    return;
                }
                if (98 <= Rnd && Rnd < 100)
                {
                    ScriptMain.LoadFieldParts(StockName, "River3", 0, 0, 0, 0);
                    ScriptMain.LoadFieldParts(StockName, "River1", 0, 0, 1, 0);
                    FragmentsListScript.AddList(StockName);
                    return;
                }

            }

        }






    }
}
