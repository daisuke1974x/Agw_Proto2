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
        int Cnt = 1;
        int Numbering = 0;

        for (int Index = 0; Index < objField.transform.GetChildCount(); Index++)
        {
            string NameSearch = objField.transform.GetChild(Index).name;
            if (NameSearch.Substring(0, 5) == "Stock")
            {
                int Num = int.Parse(NameSearch.Substring(6, NameSearch.Length - "Stock_".Length));
                if ((Cnt != Num) && (Numbering == 0))
                {
                    Numbering = Cnt;
                }
                Cnt++;
            }
        }
        if (Numbering == 0) Numbering = Cnt;
        string StockName = "Stock_" + Numbering.ToString();

        //ストックの作成
        s_Main ScriptMain = objMainControl.GetComponent<s_Main>();
        int Rnd = Random.Range(0, 11);
        switch (Rnd)
        {
            case 0:
                ScriptMain.LoadFieldParts(StockName, "Green", 0, 0, 0, 0);
                break;
            case 1:
                ScriptMain.LoadFieldParts(StockName, "Road0", 0, 0, 0, 0);
                break;
            case 2:
                ScriptMain.LoadFieldParts(StockName, "Road1", 0, 0, 0, 0);
                break;
            case 3:
                ScriptMain.LoadFieldParts(StockName, "Road2", 0, 0, 0, 0);
                break;
            case 4:
                ScriptMain.LoadFieldParts(StockName, "Road3", 0, 0, 0, 0);
                break;
            case 5:
                ScriptMain.LoadFieldParts(StockName, "Road4", 0, 0, 0, 0);
                break;
            case 6:
                ScriptMain.LoadFieldParts(StockName, "River1", 0, 0, 0, 0);
                break;
            case 7:
                ScriptMain.LoadFieldParts(StockName, "River2", 0, 0, 0, 0);
                break;
            case 8:
                ScriptMain.LoadFieldParts(StockName, "River3", 0, 0, 0, 0);
                break;
            case 9:
                ScriptMain.LoadFieldParts(StockName, "RiverBridge", 0, 0, 0, 0);
                break;
            case 10:
                ScriptMain.LoadFieldParts(StockName, "Green", 0, 0, 0, 0);
                ScriptMain.LoadFieldParts(StockName, "Green", 1, 0, 0, 0);
                ScriptMain.LoadFieldParts(StockName, "Green", 0, 0, 1, 0);
                ScriptMain.LoadFieldParts(StockName, "Green", 1, 0, 1, 0);
                break;
        }



    }
}
