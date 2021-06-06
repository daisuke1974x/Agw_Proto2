using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class s_FragmentsList : MonoBehaviour
{
    public GameObject Field;
    public GameObject MainCamera;
    public GameObject Player;
    public GameObject MainControl;
    public GameObject FragmentsListCameraPrefab;
    public GameObject Chest;
    private s_Main MainScript;
    private Vector3 FragmentsListPos = new Vector3(0, 0, 20000);

    public RenderTexture FragmentRenderTexture;
    public RawImage FragmentRawImage;
    public GameObject ParticleStar;
    public GameObject Particles;
    

    public struct StStock
    {
        public string StockName;
        public GameObject StockRawImage;
        public GameObject StockCameraPrefab;
        public GameObject Stock;
    }
    public List<StStock> StockList = new List<StStock>();

    public enum EnumMode
    {
        Idle,
        StockList,
        LargeStockHandle,

        TransitionIdleToStockList,
        TransitionStockListToIdle,
        TransitionStockListToLargeStockSet,
        TransitionLargeStockSetToStockList,
        TransitionDeployToIdle,

        AddStock,
        StockListSelect,
        StockListRotateStock,
        StockDelete,
        StockDeploy,
        LargeStockHandleSlide,
        LargeStockHandleRotate,
        LargeStockHandleRotateWorld,
        LargeStockDeploy,
        LargeStockDeployAfter
    }
    public EnumMode Mode = EnumMode.Idle;

    private int StockListCursorIndex = 0;//StockListにおける選択位置

    private int StockIndex = 0;//AddされたときのIndex番号

    private float TimerCounter =0;//各処理で使用するカウンター
    private int OperationDirection;//各処理で使用する方向

    private Vector3 StartPos;
    private Vector3 EndPos;
    private Vector3 PorchPos;
    private Vector3 RotationBackUp;
    private Vector3 PositionBackUp;
    private Vector3 CameraPositionBackUp;
    private Vector3 CameraRotationBackUp;
    private Vector3 CameraPositionHomePosition;
    private Vector3 CameraRotationHomePosition;
    private Vector3 CameraPosition;
    private Vector3 CameraRotation;
    private Vector3 FragmentPositionBackUp;

    public struct CheckItem
    {
        public float SelfBlockX;
        public float SelfBlockY;
        public float SelfBlockZ;
        public float NodeBlockX;
        public float NodeBlockZ;
        public int Direction;
        public string ConnectionCode;

    }

    private float BlockIntervalX = 10f;
    private float BlockIntervalY = 2.5f;
    private float BlockIntervalZ = 10f;
    private float FloatHeight = 2f;

    //private float StockListDistance = 100f;

    private int[] DirectionIndexX = { 0, 1, 0, -1 };
    private int[] DirectionIndexZ = { 1, 0, -1, 0 };

    private int SlideDirection = 0;
    private int SlideIndexX = 0;
    private int SlideIndexZ = 0;

    private int RotateIndex = 0;
    private int RotateIndexWorld = 0;

    private float SetCounter = 0;

    public GameObject objCaption;
    public GameObject objSelectedFrame;

    public GameObject DebugText;

    public void StartStockListSelect()
    {
        if (Mode == EnumMode.Idle)
        {
            TimerCounter = 0;
            Mode = EnumMode.TransitionIdleToStockList;
        }

        
    }
    public void EndStockListSelect()
    {
        if (Mode == EnumMode.StockList)
        {
            TimerCounter = 0;
            Mode = EnumMode.TransitionStockListToIdle;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        MainScript = MainControl.GetComponent<s_Main>();
        PorchPos = GameObject.Find("PorchImage").transform.position ;

    }

    //============================================================================================================
    //
    //============================================================================================================
    public void AddList(string pStockName)
    {
        GameObject Stock = GameObject.Find(pStockName);
        RenderTexture StockRenderTexture = Instantiate(FragmentRenderTexture, this.transform, true);

        GameObject StockCameraPrefab = Instantiate(FragmentsListCameraPrefab.gameObject, Field.transform, true);
        Camera StockCamera = StockCameraPrefab.transform.GetChild(0).GetComponent<Camera>();
        StockCamera.targetTexture = StockRenderTexture;
        StockCameraPrefab.name = "Camera_" + pStockName;
        StockCameraPrefab.transform.position = Stock.transform.position;

        GameObject StockRawImage = Instantiate(FragmentRawImage, this.transform, true).gameObject;
        StockRawImage.GetComponent<RawImage>().texture = StockRenderTexture;
        StockRawImage.name = "Image_" + pStockName;
        StockRawImage.transform.position = new Vector3(100, 100, 0);

        RefreshList();

        Mode = EnumMode.AddStock;
        TimerCounter = 0;
        StockIndex = 0;
        for (int Index = 0; Index < StockList.Count; Index++)
        {
            if (StockList[Index].StockName == pStockName)
            {
                StockIndex = Index;
            }
        }

        StartPos = RectTransformUtility.WorldToScreenPoint(MainCamera.GetComponent<Camera>(), Chest.transform.position);
        EndPos = PorchPos + new Vector3(-100, 0, 0);



    }

    //============================================================================================================
    //ストックリストの更新
    //============================================================================================================
    void RefreshList()
    {
        float Cnt = 0;
        string StockNum = "";
        StockList.Clear();

        for (int Index = 0; Index < Field.transform.GetChildCount(); Index++)
        {
            GameObject FindObject = Field.transform.GetChild(Index).gameObject;
            if (FindObject.name.Substring(0, 5) == "Stock")
            {
                GameObject Stock = FindObject;
                StockNum = FindObject.name.Substring("Stock_".Length, FindObject.name.Length - "Stock_".Length);
                GameObject StockCameraPrefab = GameObject.Find("Camera_" + Stock.name);
                GameObject StockRawImage = GameObject.Find("Image_" + Stock.name).gameObject;

                Stock.transform.position = new Vector3(Cnt * 100, 0, 0) + FragmentsListPos;
                StockCameraPrefab.transform.position = Stock.transform.position;
                StockRawImage.transform.position = new Vector3(Cnt * 100, 30, 9);

                StStock AddItem = new StStock();
                AddItem.StockName = Stock.name;
                AddItem.StockRawImage = StockRawImage;
                AddItem.StockCameraPrefab = StockCameraPrefab;
                AddItem.Stock = Stock;

                StockList.Add(AddItem);

                Cnt++;

            }




        }
        if (StockList.Count-2 < StockListCursorIndex)
        {
            StockListCursorIndex--;
            if (StockListCursorIndex < 0)
            {
                StockListCursorIndex = 0;
            }
        }
        var json = JsonUtility.ToJson(StockList);
        Debug.Log(json);
    } 

    // Update is called once per frame
    //============================================================================================================
    //
    //============================================================================================================
    void Update()
    {
        switch (Mode)
        {
            case EnumMode.Idle:
                Update_Idle();
                break;
            case EnumMode.StockList:
                Update_StockList();
                break;
            case EnumMode.LargeStockHandle:
                Update_LargeStockHandle();
                break;
            case EnumMode.TransitionIdleToStockList:
                Update_TransitionIdleToStockList();
                break;
            case EnumMode.TransitionStockListToIdle:
                Update_TransitionStockListToIdle();
                break;
            case EnumMode.TransitionStockListToLargeStockSet:
                Update_TransitionStockListToLargeStockSet();
                break;
            case EnumMode.TransitionLargeStockSetToStockList:
                Update_TransitionLargeStockSetToStockList();
                break;
            case EnumMode.TransitionDeployToIdle:
                Update_TransitionDeployToIdle();
                break;
            case EnumMode.AddStock:
                Update_AddStock();
                break;
            case EnumMode.StockListSelect:
                Update_StockListSelect();
                break;
            case EnumMode.StockListRotateStock:
                Update_StockListRotateStock();
                break;
            case EnumMode.StockDelete:
                Update_StockDelete();
                break;
            case EnumMode.StockDeploy:
                Update_StockDeploy();
                break;
            case EnumMode.LargeStockHandleSlide:
                Update_LargeStockHandleSlide();
                break;
            case EnumMode.LargeStockHandleRotate:
                Update_LargeStockHandleRotate();
                break;
            case EnumMode.LargeStockHandleRotateWorld:
                Update_LargeStockHandleRotateWorld();
                break;
            case EnumMode.LargeStockDeploy:
                Update_LargeStockDeploy();
                break;
            case EnumMode.LargeStockDeployAfter:
                Update_LargeStockDeployAfter();
                break;


        }
    }


    //============================================================================================================
    //
    //============================================================================================================
    void Update_Idle() {

        //ここではStockListは表示しない。前フレームでの設置後のDetroyが反映されないため、ぬるぽになる。
        //ViewStockList(0, 0);


    }


    //============================================================================================================
    //
    //============================================================================================================
    void Update_StockList() {

        ViewStockList(1, 0);
        float HatLR = Input.GetAxis("HatLR");
        if(HatLR != 0)
        {
            if (0 < HatLR)
            {
                if (StockListCursorIndex < StockList.Count-1)
                {
                    OperationDirection = 1;
                    TimerCounter = 0;
                    Mode = EnumMode.StockListSelect;
                }
            }
            else
            {
                if (0 < StockListCursorIndex)
                {
                    OperationDirection = -1;
                    TimerCounter = 0;
                    Mode = EnumMode.StockListSelect;
                }
            }
            return;
        }

        bool InputR1 = Input.GetButton("R1");
        bool InputL1 = Input.GetButton("L1");
        if (InputR1 || InputL1)
        {
            Mode = EnumMode.StockListRotateStock;
            TimerCounter = 0;
            RotationBackUp = StockList[StockListCursorIndex].Stock.transform.rotation.eulerAngles;
            if (InputL1)
            {
                OperationDirection = -1;
            }
            else
            {
                OperationDirection = 1;
            }
            return;
        }
        if (Input.GetButtonDown("Circle"))
        {
            if (0<StockList.Count)
            {
                if (MainScript.isIdle == true)
                {
                    CameraRotationBackUp = MainCamera.transform.rotation.eulerAngles;
                    CameraPositionBackUp = MainCamera.transform.position;

                    GameObject tempGameObject = new GameObject();
                    tempGameObject.transform.position = MainScript.objFieldCursor.transform.position + new Vector3(0, 50, 0);
                    tempGameObject.transform.LookAt(MainScript.objFieldCursor.transform.position);
                    tempGameObject.transform.RotateAround(MainScript.objFieldCursor.transform.position, Vector3.right, -20f);
                    RotateIndexWorld = 0;

                    CameraRotation = tempGameObject.transform.rotation.eulerAngles;
                    CameraPosition = tempGameObject.transform.position;

                    CameraPositionHomePosition = CameraPosition;
                    CameraRotationHomePosition = CameraRotation;

                    FragmentPositionBackUp = StockList[StockListCursorIndex].Stock.transform.position;
                    StockList[StockListCursorIndex].Stock.transform.position = MainScript.objFieldCursor.transform.position + new Vector3(0, FloatHeight, 0);
                    StockList[StockListCursorIndex].Stock.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

                    MainScript.isControllEnabled = false;
                    MainCamera.GetComponent<s_MainCamera>().isControllEnabled = false;

                    Mode = EnumMode.TransitionStockListToLargeStockSet;
                    TimerCounter = 0;

                }

            }

        }


    }



    //============================================================================================================
    //
    //============================================================================================================
    void Update_LargeStockHandle() {


        SelectedFrameBlink();//赤枠を点滅させる

        if (Input.GetButtonDown("Cross"))
        {
            CameraRotation = MainCamera.transform.rotation.eulerAngles;
            CameraPosition = MainCamera.transform.position;

            //MainCamera.GetComponent<s_MainCamera>().isControllEnabled = false;
            StockList[StockListCursorIndex].Stock.transform.position = FragmentPositionBackUp;

            Mode = EnumMode.TransitionLargeStockSetToStockList;
            TimerCounter = 0;
            GameObject.Find("Guide2").GetComponent<Text>().text = "";
            return;

        }

        bool InputR1 = Input.GetButton("R1");
        bool InputL1 = Input.GetButton("L1");
        if (InputR1 || InputL1)
        {
            Mode = EnumMode.LargeStockHandleRotate;
            TimerCounter = 0;
            PositionBackUp = StockList[StockListCursorIndex].Stock.transform.position;
            RotationBackUp = StockList[StockListCursorIndex].Stock.transform.rotation.eulerAngles;
            if (InputL1)
            {
                OperationDirection = -1;
            }
            else
            {
                OperationDirection = 1;
            }
            GameObject.Find("Debug2").GetComponent<Text>().text = "";
            GameObject.Find("Guide2").GetComponent<Text>().text = "";
            return;
        }

        bool InputR2 = Input.GetButton("R2");
        bool InputL2 = Input.GetButton("L2");
        if (InputR2 || InputL2)
        {
            Mode = EnumMode.LargeStockHandleRotateWorld;
            TimerCounter = 0;
            if (InputL2)
            {
                OperationDirection = -1;
            }
            else
            {
                OperationDirection = 1;
            }
            GameObject.Find("Debug2").GetComponent<Text>().text = "";
            GameObject.Find("Guide2").GetComponent<Text>().text = "";
        }

        if (Input.GetAxis("HatLR") != 0 || Input.GetAxis("HatUD") != 0)
        {
            if (Input.GetAxis("HatUD") == 1) SlideDirection = 0;
            if (Input.GetAxis("HatLR") == 1) SlideDirection = 1;
            if (Input.GetAxis("HatUD") == -1) SlideDirection = 2;
            if (Input.GetAxis("HatLR") == -1) SlideDirection = 3;


            //動かす（ずらす）ができるかどうかチェック
            bool isMoveOk = false;
            int Dir = (4 + SlideDirection - RotateIndexWorld) % 4;
            Vector3 Compare1 = new Vector3();
            Compare1.x = -DirectionIndexX[Dir] + Mathf.RoundToInt(MainScript.objFieldCursor.transform.position.x / 10);
            Compare1.z = -DirectionIndexZ[Dir] + Mathf.RoundToInt(MainScript.objFieldCursor.transform.position.z / 10);
            for (int i = 0; i < StockList[StockListCursorIndex].Stock.transform.GetChildCount(); i++)
            {
                var obj = StockList[StockListCursorIndex].Stock.transform.GetChild(i);
                if (obj.name != "SelectedFrame(Clone)")
                {
                    var ChidPos = obj.transform.position;
                    Vector3 Compare2 = new Vector3();
                    Compare2.x = Mathf.RoundToInt(ChidPos.x / 10);
                    Compare2.z = Mathf.RoundToInt(ChidPos.z / 10);
                    if (Compare1 == Compare2) isMoveOk = true;
                }

            }



            if (isMoveOk == true)
            {
                RotateIndex = Mathf.RoundToInt(StockList[StockListCursorIndex].Stock.transform.rotation.eulerAngles.y / 90);
                Mode = EnumMode.LargeStockHandleSlide;
                TimerCounter = 0;
                GameObject.Find("Debug2").GetComponent<Text>().text = "";
                GameObject.Find("Guide2").GetComponent<Text>().text = "";
            }
            return;

        }

        if (Input.GetButtonDown("Circle"))
        {
            int rc = SetCheck();
            if (rc == 0)
            {
                Mode = EnumMode.LargeStockDeploy;
                TimerCounter = 0;
                return;
            }

        }

        //Text DebugText = GameObject.Find("Debug3").GetComponent<Text>();
        //DebugText.text = "";
        //DebugText.text += "SlideDirection:" + SlideDirection.ToString() + "\n";
        //DebugText.text += "RotateIndex:" + RotateIndex.ToString() + "\n";
        //DebugText.text += "objFieldCursor:" + MainScript.objFieldCursor.transform.position.ToString() + "\n";
        //DebugText.text += "Stock.position:" + StockList[StockListCursorIndex].Stock.transform.position.ToString() + "\n";
        //DebugText.text += "Stock.rotation:" + StockList[StockListCursorIndex].Stock.transform.rotation.eulerAngles.ToString() + "\n";
        //DebugText.text += "SlideIndexX:" + SlideIndexX.ToString() + "\n";
        //DebugText.text += "SlideIndexZ:" + SlideIndexZ.ToString() + "\n";



    }



    //============================================================================================================
    //
    //============================================================================================================
    void Update_TransitionIdleToStockList() 
    {
        RefreshList();

        float TransitionTime = 0.2f;
        TimerCounter += Time.deltaTime;

        ViewStockList(TimerCounter / TransitionTime,0);

        if (TransitionTime < TimerCounter)
        {
            Mode = EnumMode.StockList;
        }
    
    
    }
    //============================================================================================================
    //
    //============================================================================================================
    void Update_TransitionStockListToIdle()
    {
        float TransitionTime = 0.2f;
        TimerCounter += Time.deltaTime;

        ViewStockList((TransitionTime-TimerCounter) / TransitionTime,0);

        if (TransitionTime < TimerCounter)
        {
            Mode = EnumMode.Idle;
        }


    }

    //============================================================================================================
    //
    //============================================================================================================
    void Update_TransitionStockListToLargeStockSet() 
    {
        float TimeStep1 = 0.3f;
        TimerCounter += Time.deltaTime;
        MainCamera.transform.position = CameraPositionBackUp + (CameraPosition - CameraPositionBackUp) * (TimerCounter / TimeStep1);
        MainCamera.transform.rotation = Quaternion.Euler(CameraRotationBackUp + (CameraRotation - CameraRotationBackUp) * (TimerCounter / TimeStep1));
        ViewStockList((TimeStep1-TimerCounter) / TimeStep1, 0);

        if (TimeStep1 < TimerCounter)
        {
            MainCamera.transform.position = CameraPosition;
            MainCamera.transform.rotation = Quaternion.Euler(CameraRotation);
            SlideIndexX = 0;
            SlideIndexZ = 0;
            Mode = EnumMode.LargeStockHandle;
            SetCheckGuide();
        }

    }
    //============================================================================================================
    //
    //============================================================================================================
    void Update_TransitionLargeStockSetToStockList()
    {
        float TimeStep1 = 0.3f;
        TimerCounter += Time.deltaTime;
        MainCamera.transform.position = CameraPosition + (CameraPositionBackUp - CameraPosition) * (TimerCounter / TimeStep1);
        MainCamera.transform.rotation = Quaternion.Euler(CameraRotation + (CameraRotationBackUp - CameraRotation) * (TimerCounter / TimeStep1));
        ViewStockList(TimerCounter / TimeStep1, 0);

        if (TimeStep1 < TimerCounter)
        {
            MainCamera.transform.position = CameraPositionBackUp;
            MainCamera.transform.rotation = Quaternion.Euler(CameraRotationBackUp);
            Mode = EnumMode.StockList;
            MainScript.isControllEnabled = true;
            MainCamera.GetComponent<s_MainCamera>().isControllEnabled = true;
        }

    }

    //============================================================================================================
    //
    //============================================================================================================

    void Update_TransitionDeployToIdle() {

        float TimeStep1 = 0.3f;
        TimerCounter += Time.deltaTime;
        MainCamera.transform.position = CameraPosition + (CameraPositionBackUp - CameraPosition) * (TimerCounter / TimeStep1);
        MainCamera.transform.rotation = Quaternion.Euler(CameraRotation + (CameraRotationBackUp - CameraRotation) * (TimerCounter / TimeStep1));

        if (TimeStep1 < TimerCounter)
        {
            MainCamera.transform.position = CameraPositionBackUp;
            MainCamera.transform.rotation = Quaternion.Euler(CameraRotationBackUp);
            Mode = EnumMode.Idle;
            MainScript.isControllEnabled = true;
            MainCamera.GetComponent<s_MainCamera>().isControllEnabled = true;
        }



    }



    //============================================================================================================
    //
    //============================================================================================================
    void Update_AddStock() {
        ViewStockList(0,0);
        for (int Index = 0; Index < StockList.Count; Index++)
        {
            Vector3 pos = StockList[Index].StockRawImage.transform.position;
            pos.x = -300;
            pos.y = -300;
            StockList[Index].StockRawImage.transform.position = pos;

        }


        float TimeStep1 = 0.75f;
        float TimeStep2 = 2.25f;
        float TimeStep3 = 2.75f;
        float ParticleCounter = 0;
        float TimeStepParticle = 1f / 60f/8f;
        float ParabolicConstant = 2000;//放物線の山なりを決める定数。TimeStep1を変更した場合、調整が必要。

        TimerCounter += Time.deltaTime;
        ParticleCounter += Time.deltaTime;

        if (TimerCounter <= TimeStep1)
        {
            Vector3 pos = StockList[StockIndex].StockRawImage.transform.position;
            pos = StartPos + (EndPos - StartPos) * TimerCounter / TimeStep1;
            pos.y -= Mathf.Pow(TimerCounter - (TimeStep1 / 2), 2) * ParabolicConstant - Mathf.Pow((TimeStep1 / 2), 2) * ParabolicConstant;
            StockList[StockIndex].StockRawImage.transform.position = pos;
            StockList[StockIndex].StockRawImage.transform.localScale = new Vector3(1, 1, 1) * (TimerCounter / TimeStep1);
            StockList[StockIndex].StockRawImage.transform.rotation = Quaternion.Euler(0, 0, -TimerCounter * 720);

            
            
            //パーティクルを発生させる
            if (TimeStepParticle < ParticleCounter)
            {
                GameObject Particle = Instantiate(ParticleStar, Particles.transform, false);
                Vector3 pos2 = pos;
                pos2.x += Random.Range(-20f, 20f);
                pos2.y += Random.Range(-20f, 20f);
                Particle.transform.position = pos2;


                //ParticleCounter = 0;
            }

        }
        if (TimeStep1< TimerCounter  && TimerCounter <TimeStep2)
        {
            Vector3 pos = StockList[StockIndex].StockRawImage.transform.position;
            pos = EndPos;
            StockList[StockIndex].StockRawImage.transform.position = pos;
            StockList[StockIndex].StockRawImage.transform.localScale = new Vector3(1, 1, 1);
            StockList[StockIndex].StockRawImage.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        if (TimeStep2 < TimerCounter && TimerCounter < TimeStep3)
        {
            Vector3 pos = StockList[StockIndex].StockRawImage.transform.position;
            pos = EndPos + (PorchPos-EndPos ) * (TimerCounter-TimeStep2 ) / (TimeStep3 - TimeStep2);
            StockList[StockIndex].StockRawImage.transform.position = pos;
            StockList[StockIndex].StockRawImage.transform.localScale = new Vector3(1, 1, 1) * ((TimeStep3 - TimerCounter) / (TimeStep3 - TimeStep2));
            StockList[StockIndex].StockRawImage.transform.rotation = Quaternion.Euler(0, 0,- TimerCounter * 720);
        }

        if (TimeStep3 < TimerCounter)
        {
            Mode = EnumMode.Idle;
        }

    }


    //============================================================================================================
    //
    //============================================================================================================
    void Update_StockListSelect() {
        float TimeStep1 = 0.2f;
        TimerCounter += Time.deltaTime;

        float Progress = OperationDirection * TimerCounter / TimeStep1;
        ViewStockList(1, Progress);


        

        if (TimeStep1 < TimerCounter)
        {

            StockListCursorIndex += OperationDirection;
            if (StockListCursorIndex < 0)
            {
                StockListCursorIndex = 0;
            }
            if (StockList.Count-1< StockListCursorIndex )
            {
                StockListCursorIndex = StockList.Count - 1;
            }

            Mode = EnumMode.StockList;

        }



    }


    //============================================================================================================
    //
    //============================================================================================================
    void Update_StockListRotateStock() {
        float TimeStep1 = 0.3f;
        TimerCounter += Time.deltaTime;
        float Angle = OperationDirection * 90 * TimerCounter / TimeStep1;

        if (TimeStep1< TimerCounter)
        {
            Angle = OperationDirection * 90;
            Mode = EnumMode.StockList;
        }
        StockList[StockListCursorIndex].Stock.transform.rotation = Quaternion.Euler(RotationBackUp + new Vector3(0, Angle, 0));
        ViewStockList(1, 0);


    }
    //============================================================================================================
    //
    //============================================================================================================
    void Update_StockDelete() { }
    //============================================================================================================
    //
    //============================================================================================================
    void Update_StockDeploy() { }
    //============================================================================================================
    //
    //============================================================================================================
    void Update_LargeStockHandleSlide() 
    {
        float TimeStep1 = 0.2f;
        TimerCounter += Time.deltaTime;

        //int dir = (8 + SlideDirection - RotateIndex - RotateIndexWorld) % 4;
        int dir = (8 + SlideDirection  - RotateIndex - RotateIndexWorld) % 4;
        int dir2 = (8 + SlideDirection - RotateIndexWorld) % 4;
        Vector3 pos = StockList[StockListCursorIndex].Stock.transform.position;
        Vector3 cpos = MainScript.objFieldCursor.transform.position;
        pos.x = cpos.x + SlideIndexX * BlockIntervalX + (BlockIntervalX * DirectionIndexX[dir2]) * (TimerCounter / TimeStep1);
        pos.z = cpos.z + SlideIndexZ * BlockIntervalZ + (BlockIntervalZ * DirectionIndexZ[dir2]) * (TimerCounter / TimeStep1);


        if (TimeStep1< TimerCounter)
        {
            TimerCounter = TimeStep1;
            SlideIndexX += DirectionIndexX[dir2];
            SlideIndexZ += DirectionIndexZ[dir2];
            pos.x = cpos.x + SlideIndexX * BlockIntervalX ;
            pos.z = cpos.z + SlideIndexZ * BlockIntervalZ ;
            Mode = EnumMode.LargeStockHandle;
        }
        StockList[StockListCursorIndex].Stock.transform.position = pos;


        if(Mode == EnumMode.LargeStockHandle)
        {
            SetCheckGuide();

        }


    }

    //============================================================================================================
    //
    //============================================================================================================
    void Update_LargeStockHandleRotate()
    {
        float TimeStep1 = 0.3f;
        TimerCounter += Time.deltaTime;
        float Angle = OperationDirection * 90 * TimerCounter / TimeStep1;

        if (TimeStep1 < TimerCounter)
        {
            //RotateIndex = (4 + RotateIndex + OperationDirection) % 4;
            Angle = OperationDirection * 90;
            Mode = EnumMode.LargeStockHandle;
        }

        Vector3 RotateCenter = MainScript.objFieldCursor.transform.position;//回転の中心の指定
        StockList[StockListCursorIndex].Stock.transform.rotation = Quaternion.Euler(RotationBackUp);//いったん戻す
        StockList[StockListCursorIndex].Stock.transform.position = PositionBackUp;//いったん戻す

        StockList[StockListCursorIndex].Stock.transform.RotateAround(RotateCenter, Vector3.up,  Angle);//回転

        if (Mode == EnumMode.LargeStockHandle)
        {
            SlideIndexX = Mathf.RoundToInt((StockList[StockListCursorIndex].Stock.transform.position.x - RotateCenter.x) / BlockIntervalX);
            SlideIndexZ = Mathf.RoundToInt((StockList[StockListCursorIndex].Stock.transform.position.z - RotateCenter.z) / BlockIntervalZ);
        }

        if (Mode == EnumMode.LargeStockHandle)
        {
            SetCheckGuide();

        }

    }
    //============================================================================================================
    //
    //============================================================================================================
    void Update_LargeStockHandleRotateWorld()
    {
        float TimeStep1 = 0.3f;
        TimerCounter += Time.deltaTime;
        float Angle = OperationDirection * 90 * TimerCounter / TimeStep1;

        if (TimeStep1 < TimerCounter)
        {
            Angle = 0;
            Mode = EnumMode.LargeStockHandle;
            RotateIndexWorld = (4 + RotateIndexWorld + OperationDirection) % 4;
            TimerCounter = TimeStep1;
        }

        MainCamera.transform.position = CameraPositionHomePosition;
        MainCamera.transform.rotation = Quaternion.Euler(CameraRotationHomePosition);
        Vector3 StockSelectCameraPos = MainScript.objFieldCursor.transform.position;
        StockSelectCameraPos.y = MainCamera.transform.position.y;
        MainCamera.transform.RotateAround(StockSelectCameraPos, Vector3.up, - RotateIndexWorld * 90 - Angle);


    }


    //============================================================================================================
    //
    //============================================================================================================
    void Update_LargeStockDeploy() {
        float TimeStep1 = 0.75f;
        TimerCounter += Time.deltaTime;

        SelectedFrameTransparency((TimeStep1 - TimerCounter) / TimeStep1);
        Vector3 pos = StockList[StockListCursorIndex].Stock.transform.position;
        pos.y = FloatHeight * (TimeStep1 - TimerCounter) / TimeStep1;
        StockList[StockListCursorIndex].Stock.transform.position = pos;

        if (TimeStep1 < TimerCounter)
        {
            MainScript.objFieldCursor.GetComponent<s_FieldCursor>().Disappear();

            //MainScript.objFieldCursor.transform.position = FragmentsListPos;

            //赤枠の除去とParentの付け替え
            for (int Index = 0; Index < StockList[StockListCursorIndex].Stock.transform.GetChildCount(); Index++)
            {
                GameObject obj = StockList[StockListCursorIndex].Stock.transform.GetChild(Index).gameObject;
                if (obj.name == "SelectedFrame(Clone)")
                {
                    //赤枠の場合は除去
                    Destroy(obj, 0.1f);
                }
                else
                {
                    //赤枠ではない場合は、Worldに移す
                    obj.transform.parent = MainScript.CurrentWorld.transform;
                    //端数を整理
                    Vector3 pos2 = obj.transform.position;
                    pos2.x = Mathf.RoundToInt(pos2.x / BlockIntervalX) * BlockIntervalX;
                    pos2.y = Mathf.RoundToInt(pos2.y / BlockIntervalY) * BlockIntervalY;
                    pos2.z = Mathf.RoundToInt(pos2.z / BlockIntervalZ) * BlockIntervalZ;
                    obj.transform.position = pos2;
                }

            }

            Destroy(StockList[StockListCursorIndex].StockCameraPrefab);
            Destroy(StockList[StockListCursorIndex].StockRawImage);
            Destroy(StockList[StockListCursorIndex].Stock);
            //RefreshList();
            //if (StockList.Count - 2 < StockListCursorIndex) StockListCursorIndex = StockList.Count - 1;
            Mode = EnumMode.LargeStockDeployAfter;
        }




    }
    //================================================================================================================
    //　設置完了後の待機画面
    //================================================================================================================
    void Update_LargeStockDeployAfter()
    {
        GameObject.Find("Guide2").GetComponent<Text>().text = "設置が完了しました。";

        if (Input.GetButtonDown("Circle") || Input.GetButtonDown("Cross"))
        {
            Mode = EnumMode.TransitionDeployToIdle;
            TimerCounter = 0;

        }
    }
    //================================================================================================================
    //　赤枠の透明度
    //================================================================================================================
    void SelectedFrameTransparency(float pCola)
    {
        if (StockList.Count == 0) return;

        for (int i = 0; i < StockList[StockListCursorIndex].Stock.transform.childCount; i++)
        {
            GameObject obj = StockList[StockListCursorIndex].Stock.transform.GetChild(i).gameObject;
            if (obj.name == "SelectedFrame(Clone)")
            {
                for (int j = 0; j < obj.transform.childCount; j++)
                {
                    GameObject obj2 = obj.transform.GetChild(j).gameObject;
                    Color col2 = obj2.GetComponent<Renderer>().material.color;
                    col2.a = pCola;
                    obj2.GetComponent<Renderer>().material.color = col2;

                    //Setのときの演出。スーッと消す
                    //col2.a = (FloatHeight - SetCounter) / FloatHeight;

                }
            }

        }
    }

    //================================================================================================================
    //　赤枠の点滅
    //================================================================================================================
    void SelectedFrameBlink()
    {
        SelectedFrameTransparency(Mathf.Sin(Time.time * 8) / 2 + 0.5f);
    }

    //============================================================================================================
    //
    //============================================================================================================
    void ViewStockList(float pPorchProgress,float pSelectProgress)
    {
        //PorchPos = GameObject.Find("PorchImage").transform.position;
        float StockListInterval = 100;
        float StockListPosY = 80;

        for (int Index = 0; Index < StockList.Count; Index++)
        {
            Vector3 pos = StockList[Index].StockRawImage.transform.position;
            pos.x = Screen.width / 2 + (Index - StockListCursorIndex) * StockListInterval - pSelectProgress * StockListInterval;
            pos.y = StockListPosY;
            pos -= (pos - PorchPos ) * (1- pPorchProgress);
            StockList[Index].StockRawImage.transform.position = pos;
            StockList[Index].StockRawImage.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));


            StockList[Index].StockRawImage.transform.localScale = new Vector3(1, 1, 1) * pPorchProgress;
            if (pPorchProgress == 1)
            {
                if (Index == StockListCursorIndex - 1 && Mathf.Sign(pSelectProgress)==-1)
                {
                    StockList[Index].StockRawImage.transform.localScale *= (1f + Mathf.Abs(pSelectProgress) / 2);
                }
                if (Index == StockListCursorIndex)
                {
                    StockList[Index].StockRawImage.transform.localScale *= (1.5f - Mathf.Abs(pSelectProgress) / 2);
                }
                if (Index == StockListCursorIndex + 1 && Mathf.Sign(pSelectProgress) == 1)
                {
                    StockList[Index].StockRawImage.transform.localScale *= (1f + Mathf.Abs(pSelectProgress) / 2);
                }
            }

        }
    }

    //================================================================================================================
    //　設置可能かどうかのチェック
    //================================================================================================================
    int SetCheck()
    {
        if (!(Mode == EnumMode.StockList || Mode == EnumMode.LargeStockHandle)) return -9999;//正しく呼び出されていない。通常は起きないエラー。

        List<CheckItem> CheckItemWorld = new List<CheckItem>();
        List<CheckItem> CheckItemStock = new List<CheckItem>();

        //World側　被仕向けノードの抽出
        for (int Index = 0; Index < MainScript.CurrentWorld.transform.GetChildCount(); Index++)
        {
            GameObject FragmentPrefab = MainScript.CurrentWorld.transform.GetChild(Index).gameObject;
            s_FieldPartsParameter FieldPartsParameter = FragmentPrefab.GetComponent<s_FieldPartsParameter>();
            Vector3 SelfBlockPos = new Vector3();
            SelfBlockPos.x = Mathf.RoundToInt(FragmentPrefab.transform.position.x / BlockIntervalX);
            SelfBlockPos.z = Mathf.RoundToInt(FragmentPrefab.transform.position.z / BlockIntervalZ);
            float Angle = FragmentPrefab.transform.rotation.eulerAngles.y;
            int SelfRotateIndex = Mathf.RoundToInt(Angle / 90);

            for (int dir = 0; dir < 4; dir++)
            {
                //隣接チェック
                Vector3 CheckBlockPos1 = new Vector3();
                CheckBlockPos1.x = SelfBlockPos.x + DirectionIndexX[dir];
                CheckBlockPos1.z = SelfBlockPos.z + DirectionIndexZ[dir];
                bool isNeighbor = false;
                for (int Index2 = 0; Index2 < MainScript.CurrentWorld.transform.GetChildCount(); Index2++)
                {
                    GameObject FragmentPrefab2 = MainScript.CurrentWorld.transform.GetChild(Index2).gameObject;
                    Vector3 CheckBlockPos2 = new Vector3();
                    CheckBlockPos2.x = Mathf.RoundToInt(FragmentPrefab2.transform.position.x / BlockIntervalX);
                    CheckBlockPos2.z = Mathf.RoundToInt(FragmentPrefab2.transform.position.z / BlockIntervalZ);
                    if (CheckBlockPos1 == CheckBlockPos2) isNeighbor = true;
                }

                //隣接がない場合は、ノードを作成する
                if (isNeighbor == false)
                {
                    //被仕向けのときのNodeは、Selfと同じ
                    CheckItem Item = new CheckItem();
                    Item.SelfBlockX = SelfBlockPos.x;
                    Item.SelfBlockY = Mathf.RoundToInt(FragmentPrefab.transform.position.y / BlockIntervalY);
                    Item.SelfBlockZ = SelfBlockPos.z;
                    Item.NodeBlockX = SelfBlockPos.x;
                    Item.NodeBlockZ = SelfBlockPos.z;
                    Item.Direction = (dir + 2) % 4;
                    Item.ConnectionCode = FieldPartsParameter.ConnectionCodeOpponent[(4 + dir + 0 - SelfRotateIndex) % 4];
                    CheckItemWorld.Add(Item);

                    string LogString = MainScript.CurrentWorld.name + ";" + FragmentPrefab.name + ";";
                    LogString += "(";
                    LogString += Item.SelfBlockX.ToString() + ",";
                    LogString += Item.SelfBlockY.ToString() + ",";
                    LogString += Item.SelfBlockZ.ToString() + ",";
                    LogString += "),(";
                    LogString += Item.NodeBlockX.ToString() + ",";
                    LogString += Item.NodeBlockZ.ToString() + ",";
                    LogString += "),";
                    LogString += Item.Direction.ToString() + ",";
                    LogString += Item.ConnectionCode;
                    Debug.Log(LogString);
                }
            }
        }

        //Stock側　仕向けノードの抽出
        for (int Index = 0; Index < StockList[StockListCursorIndex].Stock.transform.GetChildCount(); Index++)
        {
            GameObject FragmentPrefab = StockList[StockListCursorIndex].Stock.transform.GetChild(Index).gameObject;
            if (FragmentPrefab.name != "SelectedFrame(Clone)")
            {
                s_FieldPartsParameter FieldPartsParameter = FragmentPrefab.GetComponent<s_FieldPartsParameter>();
                Vector3 SelfBlockPos = new Vector3();
                SelfBlockPos.x = Mathf.RoundToInt(FragmentPrefab.transform.position.x / BlockIntervalX);
                SelfBlockPos.z = Mathf.RoundToInt(FragmentPrefab.transform.position.z / BlockIntervalZ);
                float Angle = FragmentPrefab.transform.rotation.eulerAngles.y;
                int SelfRotateIndex = Mathf.RoundToInt(Angle / 90);

                for (int dir = 0; dir < 4; dir++)
                {
                    //隣接チェック
                    Vector3 CheckBlockPos1 = new Vector3();
                    CheckBlockPos1.x = SelfBlockPos.x + DirectionIndexX[(4 + dir + RotateIndex) % 4];
                    CheckBlockPos1.z = SelfBlockPos.z + DirectionIndexZ[(4 + dir + RotateIndex) % 4];
                    bool isNeighbor = false;

                    for (int Index2 = 0; Index2 < StockList[StockListCursorIndex].Stock.transform.GetChildCount(); Index2++)
                    {
                        GameObject FragmentPrefab2 = StockList[StockListCursorIndex].Stock.transform.GetChild(Index2).gameObject;
                        if (FragmentPrefab2.name != "SelectedFrame(Clone)")
                        {
                            Vector3 CheckBlockPos2 = new Vector3();
                            CheckBlockPos2.x = Mathf.RoundToInt(FragmentPrefab2.transform.position.x / BlockIntervalX);
                            CheckBlockPos2.z = Mathf.RoundToInt(FragmentPrefab2.transform.position.z / BlockIntervalZ);
                            if (CheckBlockPos1 == CheckBlockPos2) isNeighbor = true;
                        }

                    }

                    //隣接がない場合は、ノードを作成する
                    if (isNeighbor == false)
                    {
                        CheckItem Item = new CheckItem();
                        //仕向けのときの、X,Z座標は、Nodeは増分補正後
                        //Item.NodeBlockX = Mathf.RoundToInt(FragmentPrefab.transform.position.x / BlockIntervalX) + DirectionIndexX[(dir + RotateIndex + rotdir) % 4];
                        Item.SelfBlockX = SelfBlockPos.x;
                        Item.SelfBlockY = Mathf.RoundToInt(FragmentPrefab.transform.position.y / BlockIntervalY);
                        Item.SelfBlockZ = SelfBlockPos.z;
                        Item.NodeBlockX = SelfBlockPos.x + DirectionIndexX[(4 + dir + RotateIndex) % 4];
                        Item.NodeBlockZ = SelfBlockPos.z + DirectionIndexZ[(4 + dir + RotateIndex) % 4];
                        Item.Direction = (4 + dir + RotateIndex) % 4;
                        Item.ConnectionCode = FieldPartsParameter.ConnectionCode[(4 + dir + RotateIndex - SelfRotateIndex) % 4];
                        CheckItemStock.Add(Item);

                        string LogString = StockList[StockListCursorIndex].Stock.name + ";" + FragmentPrefab.name + ";";
                        LogString += "(";
                        LogString += Item.SelfBlockX.ToString() + ",";
                        LogString += Item.SelfBlockY.ToString() + ",";
                        LogString += Item.SelfBlockZ.ToString() + ",";
                        LogString += "),(";
                        LogString += Item.NodeBlockX.ToString() + ",";
                        LogString += Item.NodeBlockZ.ToString() + ",";
                        LogString += "),";
                        LogString += Item.Direction.ToString() + ",";
                        LogString += Item.ConnectionCode;
                        Debug.Log(LogString);
                    }
                }

            }
        }

        //マッチング判定
        int CountOK = 0;
        int CountNG = 0;
        int Overlap = 0;
        for (int Index = 0; Index < CheckItemWorld.Count; Index++)
        {
            CheckItem ItemWorld = CheckItemWorld[Index];
            for (int Index2 = 0; Index2 < CheckItemStock.Count; Index2++)
            {
                CheckItem ItemStock = CheckItemStock[Index2];

                if (ItemWorld.SelfBlockX == ItemStock.SelfBlockX && ItemWorld.SelfBlockZ == ItemStock.SelfBlockZ)
                {
                    Overlap++;
                }
                else
                {
                    if (ItemWorld.NodeBlockX == ItemStock.NodeBlockX && ItemWorld.NodeBlockZ == ItemStock.NodeBlockZ && ItemWorld.Direction == ItemStock.Direction)
                    {
                        if (ItemWorld.ConnectionCode == ItemStock.ConnectionCode)
                        {
                            CountOK++;
                        }
                        else
                        {
                            CountNG++;
                        }
                    }

                }

            }

        }

        Debug.Log("CountOK:" + CountOK.ToString() + ", CountNG:" + CountNG.ToString() + ", Overlap:" + Overlap.ToString());

        if (CountOK == 0) return -1;//接している部分の絵柄で、一致した箇所がない
        if (CountNG > 0) return -2;//接合面の絵柄が合っていない箇所がある
        if (Overlap > 0) return -3;//一部が重なっている。Fragmentが２枚以上のときありうる。

        return 0;//正常


    }

    int SetCheckGuide()
    {
        int Rc = SetCheck();
        Text GuideText = GameObject.Find("Guide2").GetComponent<Text>();
        if (Rc == 0) GuideText.text = "設置できます。";
        if (Rc == -1 || Rc == -2) GuideText.text = "接合面の絵柄が合っていない箇所があります。";
        if (Rc == -3)GuideText.text = "一部がかさなっています。";
        if (Rc == -9999) GuideText.text = "致命的なエラー。";
        return Rc;
    }


    void test()
    {
        var json = JsonUtility.ToJson(StockList);
    }

}
