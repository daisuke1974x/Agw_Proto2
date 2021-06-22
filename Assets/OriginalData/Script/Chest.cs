using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
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
    public GameObject MainControl;
    public GameObject objField;
    public GameObject FragmentCapsule;

    public string FragmentStockName;

    private FragmentList FragmentListScript;

    // Start is called before the first frame update
    void Start()
    {
        objChestClose = this.transform.GetChild(0).gameObject;
        objChestOpen = this.transform.GetChild(1).gameObject;
        objTargetCursor = GameObject.Find("TargetCursor");
        Hierarchy_UnTargetObject = GameObject.Find("UnTargetObject");
        objPlayer = GameObject.Find("Player");
        MainControl = GameObject.Find("MainControl");
        objField = GameObject.Find("Field");
        //FragmentCapsule = GameObject.Find("FragmentCapsule");

        objChestOpen.SetActive(false);
        FragmentListScript = GameObject.Find("FragmentsList").GetComponent<FragmentList>();

    }

    // Update is called once per frame
    void Update()
    {
        if (isDestroying == false)
        {
            if (Input.GetButtonDown("Circle") == true)
            {
                GameObject objTarget = objTargetCursor.GetComponent<TargetCursor>().objTarget;
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
        //s_Main MainControl.GetComponent<MainControl>() = objMainControl.GetComponent<s_Main>();
        int Rnd = 0;
        Rnd= Random.Range(0, 100);
        FragmentListScript.Chest = this.gameObject;

        MakeFragmentStock(FragmentStockName, StockName);
        return;


    }

    private void MakeFragmentStock(string pFragmentStockName,string pStockName)
    {
        switch (pFragmentStockName)
        {
            case "1x1_Green":
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Green", 0, 0, 0, 0);
                break;

            case "1x2_Green":
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Green", 0, 0, 0, 0);
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Green", 1, 0, 0, 0);
                break;

            case "1x3_Green":
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Green", 0, 0, 0, 0);
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Green", 1, 0, 0, 0);
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Green", -1, 0, 0, 0);
                break;

            case "1x3_Green_L":
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Green", 0, 0, 0, 0);
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Green", 1, 0, 0, 0);
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Green", 0, 0, 1, 0);
                break;

            case "1x4_Green":
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Green", 0, 0, 0, 0);
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Green", 1, 0, 0, 0);
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Green", 0, 0, 1, 0);
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Green", 0, 0, 2, 0);
                break;

            case "2x2_Green":
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Green", 0, 0, 0, 0);
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Green", -1, 0, 0, 0);
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Green", 0, 0, 1, 0);
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Green", 0, 0, 2, 0);
                break;

            case "2x2_Green_L":
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Green", 0, 0, 0, 0);
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Green", 1, 0, 1, 0);
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Green", 0, 0, 1, 0);
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Green", 1, 0, 0, 0);
                break;

            case "1x1_Road1":
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Road1", 0, 0, 0, 0);
                break;

            case "1x1_Road0":
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Road0", 0, 0, 0, 0);
                break;

            case "1x1_Road2":
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Road2", 0, 0, 0, 0);
                break;

            case "1x1_Road3":
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Road3", 0, 0, 0, 0);
                break;

            case "1x1_Road4":
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Road4", 0, 0, 0, 0);
                break;

            case "1x2_Road11":
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Road1", 0, 0, 0, 0);
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Road1", 1, 0, 0, 0);
                break;

            case "1x2_Road12_1":
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Road2", 0, 0, 0, 0);
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Road1", 1, 0, 0, 1);
                break;

            case "1x2_Road12_2":
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Road2", 0, 0, 0, 2);
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Road1", -1, 0, 0, 1);
                break;

            case "1x2_Road13_1":
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Road3", 0, 0, 0, 0);
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Road1", -1, 0, 0, 1);
                break;

            case "1x2_Road13_2":
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Road3", 0, 0, 0, 0);
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Road1", 1, 0, 0, 1);
                break;

            case "1x2_Road13_3":
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Road3", 0, 0, 0, 0);
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "Road1", 0, 0, 1, 0);
                break;

            case "1x1_River1":
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "River1", 0, 0, 0, 0);
                break;

            case "1x1_River2":
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "River2", 0, 0, 0, 0);
                break;

            case "1x1_RiverBridge":
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "RiverBridge", 0, 0, 0, 0);
                break;

            case "1x1_River3":
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "River3", 0, 0, 0, 0);
                break;

            case "1x2_River11":
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "River1", 0, 0, 0, 0);
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "River1", 0, 0, 1, 0);
                break;

            case "1x2_River12_1":
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "River1", 0, 0, 1, 0);
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "River2", 0, 0, 0, 3);
                break;

            case "1x2_River12_2":
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "River1", 0, 0, 1, 0);
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "River2", 0, 0, 0, 2);
                break;

            case "1x2_River13_1":
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "River3", 0, 0, 0, 0);
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "River1", -1, 0, 0, 0);
                break;

            case "1x2_River13_2":
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "River3", 0, 0, 0, 0);
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "River1", 1, 0, 0, 0);
                break;

            case "1x2_River13_3":
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "River3", 0, 0, 0, 0);
                MainControl.GetComponent<MainControl>().LoadFragment(pStockName, "River1", 0, 0, 1, 0);
                break;

        }

        FragmentListScript.AddList(pStockName);
        return;

    }

}
