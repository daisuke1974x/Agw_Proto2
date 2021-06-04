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
        LargeStockDeploy
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
    private Vector3 CameraPositionBackUp;
    private Vector3 CameraRotationBackUp;
    private Vector3 CameraPosition;
    private Vector3 CameraRotation;

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
        EndPos = PorchPos + new Vector3(-50, 0, 0);

    }

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


    }

    // Update is called once per frame
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


        }
    }


    void Update_Idle() {

        ViewStockList(0, 0);


    }


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
            if (MainScript.isIdle == true)
            {
                CameraRotationBackUp = MainCamera.transform.rotation.eulerAngles;
                CameraPositionBackUp = MainCamera.transform.position;

                GameObject tempGameObject = new GameObject();
                tempGameObject.transform.position = MainScript.objFieldCursor.transform.position + new Vector3(0, 50, 0);
                tempGameObject.transform.LookAt(MainScript.objFieldCursor.transform.position);
                //tempGameObject.transform.rotation = Quaternion.Euler(new Vector3(0, Mathf.Round(CameraRotationBackUp.y / 90) * 90, 0));
                tempGameObject.transform.RotateAround(MainScript.objFieldCursor.transform.position, Vector3.right, -10f);


                CameraRotation = tempGameObject.transform.rotation.eulerAngles;
                CameraPosition = tempGameObject.transform.position;




                MainScript.isControllEnabled = false;
                MainCamera.GetComponent<s_MainCamera>().isControllEnabled = false;

                Mode = EnumMode.TransitionStockListToLargeStockSet;
                TimerCounter = 0;

            }

        }


    }



    void Update_LargeStockHandle() {
        if (Input.GetButtonDown("Cross"))
        {
            CameraRotation = MainCamera.transform.rotation.eulerAngles;
            CameraPosition = MainCamera.transform.position;

            //MainCamera.GetComponent<s_MainCamera>().isControllEnabled = false;

            Mode = EnumMode.TransitionLargeStockSetToStockList;
            TimerCounter = 0;



        }



    }
    void Update_TransitionIdleToStockList() 
    {
        float TransitionTime = 0.2f;
        TimerCounter += Time.deltaTime;

        ViewStockList(TimerCounter / TransitionTime,0);

        if (TransitionTime < TimerCounter)
        {
            Mode = EnumMode.StockList;
        }
    
    
    }
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

    void Update_TransitionStockListToLargeStockSet() 
    {
        float TimeStep1 = 0.3f;
        TimerCounter += Time.deltaTime;
        MainCamera.transform.position = CameraPositionBackUp + (CameraPosition - CameraPositionBackUp) * (TimerCounter / TimeStep1);
        MainCamera.transform.rotation = Quaternion.Euler(CameraRotationBackUp + (CameraRotation - CameraRotationBackUp) * (TimerCounter / TimeStep1));

        if (TimeStep1 < TimerCounter)
        {
            MainCamera.transform.position = CameraPosition;
            MainCamera.transform.rotation = Quaternion.Euler(CameraRotation);
            Mode = EnumMode.LargeStockHandle;
        }

    }
    void Update_TransitionLargeStockSetToStockList()
    {
        float TimeStep1 = 0.3f;
        TimerCounter += Time.deltaTime;
        MainCamera.transform.position = CameraPosition + (CameraPositionBackUp - CameraPosition) * (TimerCounter / TimeStep1);
        MainCamera.transform.rotation = Quaternion.Euler(CameraRotation + (CameraRotationBackUp - CameraRotation) * (TimerCounter / TimeStep1));

        if (TimeStep1 < TimerCounter)
        {
            MainCamera.transform.position = CameraPositionBackUp;
            MainCamera.transform.rotation = Quaternion.Euler(CameraRotationBackUp);
            Mode = EnumMode.StockList;
            MainScript.isControllEnabled = true;
            MainCamera.GetComponent<s_MainCamera>().isControllEnabled = true;
        }

    }


    void Update_TransitionDeployToIdle() { }
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
    void Update_StockDelete() { }
    void Update_StockDeploy() { }
    void Update_LargeStockHandleSlide() { }
    void Update_LargeStockHandleRotate() { }
    void Update_LargeStockHandleRotateWorld() { }
    void Update_LargeStockDeploy() { }

    void ViewStockList(float pPorchProgress,float pSelectProgress)
    {
        //PorchPos = GameObject.Find("PorchImage").transform.position;

        for (int Index = 0; Index < StockList.Count; Index++)
        {
            Vector3 pos = StockList[Index].StockRawImage.transform.position;
            pos.x = Screen.width / 2 + (Index - StockListCursorIndex) * 100 - pSelectProgress * 100;
            pos.y = 80;
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

}
