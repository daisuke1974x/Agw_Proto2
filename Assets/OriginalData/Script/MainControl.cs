using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;




public class MainControl : MonoBehaviour
{


    //ヒエラルキーのオブジェクト参照（他スクリプトからもアクセスできるよう、共通にする）
    public GameObject PlayerObject;
    public GameObject PlayerAppearanceObject;
    public GameObject FieldCursorObject;
    public GameObject SelectedFramePrefab;
    public GameObject EnemyControl;
    public GameObject MainCameraObject;
    public GameObject TargetCursorObject;
    public GameObject CurrentField;
    public GameObject FragmentsListUI;
    public GameObject FragmentStock;
    //public GameObject objWindow_FieldPartsSelect;    //メニュー画面群
    public GameObject HpBar;

    //インスペクタでは設定しないが、他スクリプトから参照したり更新したりするもの
    public float BlockIntervalX = 10f;//大地のかけらのスナップサイズ
    public float BlockIntervalY = 2.5f;//大地のかけらのスナップサイズ
    public float BlockIntervalZ = 10f;//大地のかけらのスナップサイズ
    public bool isControllEnabled = true; //操作禁止フラグ
    public bool isIdle = true;//アイドリング中かどうか
    public Vector3 WaitingPlacePos = new Vector3(0, 0, 90000f);    //大地のかけらの待機場所
    public string CurrentMapName = "";    //現在のマップ名
    public CharStatus PlayerStatus;
    public bool isFieldCursorSet;    //フィールドカーソルが表示されているかどうか


    private Animator AnimatorObject;
    private CharacterController CharControllerObject;

    private float walkSpeed = 5.0f;  //歩行速度
    private float jumpSpeed = 8f;  //ジャンプ力
    private float gravity = 20.0f;  //重力の大きさ
    private bool jumpFlag = false; //連続ジャンプ禁止フラグ
    private bool buttonFlag_Skill1 = false; //連続操作禁止フラグ
    private bool autoRunFlag = false; //オートラン
    static bool isSliding = false;

    private float DirectionFollowSpeed = 1200.0f;   // 移動方向に対してキャラクターの向きが追随するときの速度
    private Vector3 moveDirection = Vector3.zero;   //  移動する方向とベクトルの変数（最初は初期化しておく）
    private Vector3 charDirection = Vector3.zero;   //  キャラクターの向き（最初は初期化しておく）
    private Vector3 moveDirection_Past = Vector3.zero;
    private Vector3 moveDirection_autoRun = Vector3.zero;

    //大地のかけらのprefabをfrefabからロードしておくもの
    private GameObject[] FragmentPrefabs;


    //モード  これはいずれ廃止する◆◆◆◆◆◆◆◆◆◆◆◆◆◆◆◆◆◆◆◆◆◆◆◆
    public string Mode = "Main";



    //セーブデータ関連
    csSaveData SaveData;
    public int SaveFileNumber = 0;
    private string SaveFilePath;
    
    // Start is called before the first frame update
    void Start()
    {

        SaveFilePath = Application.persistentDataPath + "/" + ".SaveData.json";


        //objControllerManager = gameObject.GetComponent<s_ControllerManager>();

        CharControllerObject = PlayerObject.GetComponent<CharacterController>();
        AnimatorObject = PlayerAppearanceObject.GetComponent<Animator>();
        PlayerStatus = PlayerObject.GetComponent<CharStatus>();


        //着地させる
        Landing(PlayerObject.gameObject);

        ////カメラ初期位置
        //initCamera();

        //大地のかけら　Prefab一覧のの読み込み
        FragmentPrefabs = Resources.LoadAll<GameObject>("FragmentPrefabs");


        //セーブデータの読み込み関連
        SaveData = new csSaveData();

        csSaveData.StSaveFieldData SaveFieldData = new csSaveData.StSaveFieldData();


        if (File.Exists(SaveFilePath))
        {
            StreamReader streamReader;
            streamReader = new StreamReader(SaveFilePath);
            string data = streamReader.ReadToEnd();
            streamReader.Close();

            SaveData = JsonUtility.FromJson<csSaveData>(data);
        }


        //CurrentWorld以外を非アクティブにする
        GameObject WorldMap = GameObject.Find("WorldMap");

        for (int Index = 0; Index < WorldMap.transform.GetChildCount(); Index++)
        {
            if (WorldMap.transform.GetChild(Index).name != "CurrentMap")
            {
                WorldMap.transform.GetChild(Index).gameObject.SetActive(false);
            }
        }


        //テストデータ
        LoadMap("FirstVillage",null);
        //LoadMap("SecondTown",null);



        //ステータス設定
        PlayerStatus.IsPlayer = true;
        PlayerStatus.HP_Max_Base =256;
        PlayerStatus.Offence_Base  = 10;
        PlayerStatus.Defence_Base = 5;
        RecalcStatus(ref PlayerStatus);
        PlayerStatus.HP = PlayerStatus.HP_Max_Calced;
        HpBar.GetComponent<HpBar>().ResetBar(PlayerStatus.HP, PlayerStatus.HP_Max_Calced);

        //FieldPartsSelect画面はいったん非表示
        //objWindow_FieldPartsSelect.SetActive(false);

        //StockListの更新（あとでサブルーチン化する）
        //List<stStockList> FieldPartsStockList = new List<stStockList>(); //ここでnew すると、他のスクリプトで参照したとき、0件になるので、newしないこと
        //foreach (FragmentProperty item in FieldPartsPlacedList)
        //{
        //    if (item.CollectionName.Substring(0, 5) == "Stock")
        //    {
        //        bool flg = false;
        //        foreach(stStockList listCheck in FieldPartsStockList)
        //        {
        //            if (listCheck.CollectionName == item.CollectionName)
        //            {
        //                flg = true;
        //            }
        //        }
        //        if (flg == false)
        //        {
        //            stStockList tmpItem;
        //            tmpItem.CollectionName = item.CollectionName;
        //            tmpItem.Hierarchy = item.Hierarchy;
        //            FieldPartsStockList.Add(tmpItem);
        //        }
        //    }
        //}

        ////ストックリストの配置（あとで、適切な場所に移動する）
        //int cnt = 0;
        //foreach (var Stock in FieldPartsStockList)
        //{
        //    Stock.Hierarchy.transform.position = new Vector3((float)cnt * 40f, 0, 99999f);
        //    cnt++;


        //}






    }




    // Update is called once per frame
    void Update()
    {

        //---------------------------------------------------------------------------------------------------------------------------------------
        // 移動入力(WASD,左スティック）に応じて、移動方向＋量(moveDirection）を設定
        //---------------------------------------------------------------------------------------------------------------------------------------
        moveDirection_Past = moveDirection;
        var cameraForward = Vector3.Scale(MainCameraObject.transform.forward, new Vector3(1, 0, 1)).normalized;  //  カメラに向かって水平の単位ベクトルを求める。
        if (isSliding == false && CharControllerObject.isGrounded && isControllEnabled == true)
        {
            if (Mode == "Main")
            {

                moveDirection = walkSpeed * cameraForward * -Input.GetAxis("LstickUD"); //カメラの前後に対して上下キーの入力成分を加算
                moveDirection += walkSpeed * MainCameraObject.transform.right * Input.GetAxis("LstickLR");//カメラから直角の方向に対して左右の入力成分を加算（多分、カメラが傾いていないことが前提）
            }
            else
            {
                moveDirection = new Vector3(0, 0, 0);
                //moveDirection = walkSpeed * cameraForward * 0; //カメラの前後に対して上下キーの入力成分を加算
                //moveDirection += walkSpeed * MainCameraObject.transform.right *0;//カメラから直角の方向に対して左右の入力成分を加算（多分、カメラが傾いていないことが前提）

            }

        }
        moveDirection.y = moveDirection_Past.y;

        //---------------------------------------------------------------------------------------------------------------------------------------
        //移動入力(WASD,左スティック）があるときは、オブジェクトの方向を変える
        //---------------------------------------------------------------------------------------------------------------------------------------
        if ((Input.GetAxis("LstickLR") != 0 || Input.GetAxis("LstickUD") != 0) && Mode == "Main" && isControllEnabled == true)
        {
            PlayerObject.transform.rotation = Quaternion.LookRotation(moveDirection);
            //垂直に向ける処理をここに入れて悩んだが、LookRotationにしたら、不要になった。

            if (CharControllerObject.isGrounded)
            {
                //攻撃中はアニメーションを遷移させない
                if (PlayerStatus.isAttack == false)
                {
                    AnimatorObject.SetBool("idle", false);

                }

            }

        }
        else
        {
            if (CharControllerObject.isGrounded)
            {
                AnimatorObject.SetBool("idle", true);
                //AnimatorObject.SetBool("run", false);
            }
        }

        //攻撃中は減速
        if (PlayerStatus.isAttack)
        {
            moveDirection.x *= 0.5f;
            moveDirection.z *= 0.5f;
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        // ジャンプ処理
        //---------------------------------------------------------------------------------------------------------------------------------------
        //着地したとき
        if (CharControllerObject.isGrounded)
        {
            moveDirection.y = 0f;
        }

        if (Input.GetButton("Triangle") == true && Mode== "Main" && isControllEnabled == true)
        {
            if (CharControllerObject.isGrounded && (isSliding == false))
            {
                if (jumpFlag == false)
                {

                    //前回攻撃操作をしてから一定時間はジャンプ禁止
                    if (PlayerStatus.isAttack == false)
                    {
                        //ジャンプ実行
                        moveDirection.y = jumpSpeed;

                        //アニメーション切り替え
                        AnimatorObject.SetTrigger("jump");

                        //連続ジャンプ禁止フラグをON
                        jumpFlag = true;

                    }

                }
            }
        }
        else
        {
            //連続ジャンプ禁止フラグをOFF
            jumpFlag = false;
        }







        //---------------------------------------------------------------------------------------------------------------------------------------
        // 物理攻撃処理
        //---------------------------------------------------------------------------------------------------------------------------------------
        if (Input.GetButton("Square") == true && Mode == "Main" && isControllEnabled == true)
        {
            if (buttonFlag_Skill1 == false)
            {
                //前回攻撃操作から一定時間を経過していること
                if (PlayerStatus.isAttack == false)
                {
                    //着地中であること
                    if (CharControllerObject.isGrounded)
                    {
                        //Debug.Log("攻撃");
                        AnimatorObject.SetBool("idle", true);
                        AnimatorObject.SetTrigger("attack");
                        buttonFlag_Skill1 = true;
                        PlayerStatus.startAttack(1f);

                    }

                }


            }
        }
        else
        {
            buttonFlag_Skill1 = false;


        }




        //---------------------------------------------------------------------------------------------------------------------------------------
        //坂道のとき滑らせる処理
        //---------------------------------------------------------------------------------------------------------------------------------------
        //http://hideapp.cocolog-nifty.com/blog/2015/03/unity-tips-char.html
        Vector3 startVec = PlayerAppearanceObject.transform.position + PlayerObject.transform.forward * 0f;
        Vector3 endVec = PlayerAppearanceObject.transform.position + PlayerObject.transform.forward * 0f;
        startVec.y = 9999;
        endVec.y = -9999;
        RaycastHit slideHit;
        if (Physics.Linecast(startVec, endVec, out slideHit))
        {
            //衝突した際の面の角度が滑らせたい角度以上かどうか
            if (Vector3.Angle(slideHit.normal, Vector3.up) > CharControllerObject.slopeLimit)
            {
                isSliding = true;
            }
            else
            {
                isSliding = false;
            }
        }
        //Text txtDebug = objDebugText.GetComponent<Text>();
        //txtDebug.text = "Angle:" + Vector3.Angle(slideHit.normal, Vector3.up) + ", isSliding:" + isSliding;

        //滑るフラグが立っていて、かつ着地しているとき、滑らせる
        if (isSliding && CharControllerObject.isGrounded)
        {
            float slideSpeed = 10f;
            Vector3 hitNormal = slideHit.normal;
            moveDirection.x += hitNormal.x * slideSpeed * Time.deltaTime;
            moveDirection.y -= gravity * Time.deltaTime;//重力落下
            moveDirection.z += hitNormal.z * slideSpeed * Time.deltaTime;
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        //重力に応じた落下処理
        //---------------------------------------------------------------------------------------------------------------------------------------
        moveDirection.y -= gravity * Time.deltaTime;

        //---------------------------------------------------------------------------------------------------------------------------------------
        //算出した moveDirection に基づき、オブジェクトを移動させる
        //---------------------------------------------------------------------------------------------------------------------------------------
        if (PlayerStatus.isKnockBack == false)
        {
            if (autoRunFlag == false)
            {
                CharControllerObject.Move(moveDirection * Time.deltaTime);
            }
            else
            {
                CharControllerObject.Move(moveDirection_autoRun * Time.deltaTime);
            }
        }
        else
        {
            Vector3 direction = PlayerStatus.KnockBackDirection;
            direction.y -= gravity * Time.deltaTime;
            CharControllerObject.Move(direction * Time.deltaTime * 1f);
            PlayerStatus.KnockBackDirection *= 0.98f;//だんだんゆっくり



        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        //フィールドパーツの外には落ちないようにするチェック
        //---------------------------------------------------------------------------------------------------------------------------------------
        if (CheckAbyss(PlayerObject))
        {
            Vector3 AbyssPos = PlayerObject.transform.position;
            AbyssPos.x = Mathf.Ceil((AbyssPos.x - 5f) / 10f) * 10f;
            AbyssPos.z = Mathf.Ceil((AbyssPos.z - 5f) / 10f) * 10f;
            AbyssPos.y = 0;
            FieldCursorObject.transform.position = AbyssPos;
            isFieldCursorSet = true;
            FieldCursorObject.GetComponent<FieldCursor>().Appear();

            if (PlayerStatus.isKnockBack == false)
            {
                moveDirection.y = 0;
                CharControllerObject.Move(moveDirection * -Time.deltaTime * 2);
                moveDirection.x = 0;
                moveDirection.z = 0;
            }
            else
            {
                Vector3 direction = PlayerStatus.KnockBackDirection;
                direction.y = 0;
                CharControllerObject.Move(direction * -Time.deltaTime * 2);
            }



        }
        else
        {
            //プレイヤーがFieldCursorObject一定の距離離れたら、FieldCursorObjectを消す（遠くの場所に移動）
            //2021.5.28 FieldPatsをSetするときに急に変なところに移動するバグの対応のため、距離を 7f -> 9f に変更。
            if (Vector3.Distance(PlayerObject.transform.position, FieldCursorObject.transform.position) > 9f)
            {
                Vector3 AbyssPos = PlayerObject.transform.position;
                AbyssPos.x = 0;
                AbyssPos.z = 99999f;
                AbyssPos.y = 0;
                //FieldCursorObject.transform.position = AbyssPos;
                isFieldCursorSet = false;
                FieldCursorObject.GetComponent<FieldCursor>().Disappear();
            }

        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        //フィールドパーツセット
        //---------------------------------------------------------------------------------------------------------------------------------------
        //if (Input.GetButtonDown("Circle") == true && Mode == "Main" && moveDirection.x == 0 && moveDirection.z == 0)
        //{
        //    if (TargetCursorObject.GetComponent<s_TargetCursor>().objTarget is null)
        //    {
        //        if (isFieldCursorSet == true)
        //        {
        //            Mode = "SelectFieldParts";
        //            objWindow_FieldPartsSelect.SetActive(true);
        //        }
        //    }
        //}

        bool isStarted = false;
        //if (Mode == "Main" && moveDirection.x == 0 && moveDirection.z == 0)
        //{
        //}
        if (TargetCursorObject.GetComponent<TargetCursor>().objTarget is null)
        {
            if (Mode == "Main" && isFieldCursorSet == true)
            {
                FragmentsListUI.GetComponent<FragmentList>().StartStockListSelect();
                isStarted = true;


                //Mode = "SelectFieldParts";
                //objWindow_FieldPartsSelect.SetActive(true);


            }

        }
        if (isStarted == false)
        {
            FragmentsListUI.GetComponent<FragmentList>().EndStockListSelect();

        }


        //Debug.Log("Playerpos x:" + this.transform.position.x);


        //---------------------------------------------------------------------------------------------------------------------------------------
        //PlayerObjectに合わせて、PlayerAppearanceObjectを追随させる
        //---------------------------------------------------------------------------------------------------------------------------------------
        Vector3 tmpVector3 = PlayerAppearanceObject.transform.position;
        tmpVector3.x = PlayerObject.transform.position.x;
        tmpVector3.y = PlayerObject.transform.position.y - 0.5f;
        tmpVector3.z = PlayerObject.transform.position.z;
        PlayerAppearanceObject.transform.position = tmpVector3;


        //  移動キーの入力があるときに、向きも追随させる
        if (Input.GetAxis("LstickUD") != 0 || Input.GetAxis("LstickLR") != 0)
        {
            PlayerAppearanceObject.transform.rotation = Quaternion.RotateTowards(PlayerAppearanceObject.transform.rotation, PlayerObject.transform.rotation, DirectionFollowSpeed * Time.deltaTime);   // 向きを q に向けてじわ～っと変化させる.
        }

        //垂直にする　PlayerAppearanceObjectだけだと変な動き。PlayerObjectのColliderが傾くと変な動きになるので、PlayerObjectも垂直にするよう追加 20210511
        Quaternion tmpQuaternion = PlayerAppearanceObject.transform.rotation;
        tmpQuaternion.x = 0;
        tmpQuaternion.z = 0;
        PlayerAppearanceObject.transform.rotation = tmpQuaternion;

        tmpQuaternion = PlayerObject.transform.rotation;
        tmpQuaternion.x = 0;
        tmpQuaternion.z = 0;
        PlayerObject.transform.rotation = tmpQuaternion;


        //---------------------------------------------------------------------------------------------------------------------------------------
        // 
        //---------------------------------------------------------------------------------------------------------------------------------------
        int motionIdol = Animator.StringToHash("Base Layer.idle");
        if (AnimatorObject.GetCurrentAnimatorStateInfo(0).nameHash == motionIdol)
        {
            isIdle = true;
        }
        else
        {
            isIdle = false;
        }



        //---------------------------------------------------------------------------------------------------------------------------------------
        // 
        //---------------------------------------------------------------------------------------------------------------------------------------
        if (Input.GetKeyUp(KeyCode.Y))
        {
            Debug.Log("セーブ開始");
            DataSave();
            Debug.Log("セーブ完了");
        }


        //---------------------------------------------------------------------------------------------------------------------------------------
        //UI表示
        //---------------------------------------------------------------------------------------------------------------------------------------

        // HP
        HpBar.GetComponent<HpBar>().SetValue(PlayerStatus.HP);



    }





    //*******************************************************************************************************************************************
    // オブジェクトを着地させる
    //*******************************************************************************************************************************************
    public void Landing(GameObject obj)
    {
        Vector3 startVec = obj.transform.position;
        Vector3 endVec = obj.transform.position;
        startVec.y = 9999;
        endVec.y = -9999;
        RaycastHit hit;
        if (Physics.Linecast(startVec, endVec, out hit))
        {
            obj.transform.position = hit.point;
        }
    }

    //*******************************************************************************************************************************************
    // オブジェクトの足元が奈落の底かどうかチェック
    //*******************************************************************************************************************************************
    public bool CheckAbyss(GameObject obj)
    {
        RaycastHit hit;
        if (Physics.Raycast(obj.transform.position,Vector3.down, out hit))
        {
            return false;
        }
        return true;
    }

    //*******************************************************************************************************************************************
    //フィールドパーツの配置
    //*******************************************************************************************************************************************
    public int LoadFragment(string pCollectionName,string pFieldPatrsName, int pBlockX, int pBlockY, int pBlockZ, int pRotate)
    {
        GameObject objCollection;
        
        if (pCollectionName.Substring(0, 5) == "Stock")
        {
            objCollection = GameObject.Find(pCollectionName);
            //配下のヒエラルキーを探して、ない場合は新規作成
            if (objCollection is null)
            {
                objCollection = new GameObject(pCollectionName);
                objCollection.transform.position = new Vector3(0, 0, 0);
                objCollection.transform.parent = FragmentStock.transform;
            }
        }
        else
        {
            objCollection = GameObject.Find("CurrentField");
        }

        //構造体にセットして、LISTに追加
        //FragmentProperty FieldPartsPlaced = new FragmentProperty();
        //FieldPartsPlaced.CollectionName = pCollectionName;
        //FieldPartsPlaced.Hierarchy = objCollection;
        //FieldPartsPlaced.FieldPatrsName = pFieldPatrsName;
        //FieldPartsPlaced.BlockX = pBlockX;
        //FieldPartsPlaced.BlockY = pBlockY;
        //FieldPartsPlaced.BlockZ = pBlockZ;
        //FieldPartsPlaced.Rotate = pRotate;
        //FieldPartsPlaced.isSystemSet = true;

        //GameObject CurrentField = GameObject.Find("CurrentField");

        //prefabの呼び出し
        foreach (var objI in FragmentPrefabs)
        {
            if (objI.name == pFieldPatrsName)
            {
                GameObject objInstance = Instantiate(objI, objCollection.transform, false);
                //CurrentField.transform.parent = objCollection.transform;
                objInstance.transform.parent = objCollection.transform;
                objInstance.name = objI.name;

                //if (pCollectionName.Substring(0, 5) == "Stock")
                //{
                //    objInstance.transform.parent = objCollection.transform;

                //}
                //else
                //{
                //    objInstance.transform.parent = CurrentField.transform;

                //}
                Vector3 pos = objInstance.transform.position;
                pos.x = CurrentField.transform.position.x + (float)(pBlockX) * 10f;// BlockIntervalX;
                pos.y = CurrentField.transform.position.y + (float)(pBlockY) * 2.5f;// BlockIntervalY;
                pos.z = CurrentField.transform.position.z + (float)(pBlockZ) * 10f;// BlockIntervalZ;
                objInstance.transform.localPosition = pos;
                Vector3 rot = objInstance.transform.eulerAngles;
                rot.y = 90f * (float)pRotate;
                objInstance.transform.eulerAngles = rot;

                //ストックのときは、赤枠を付与して、待機場所に移動させる
                if (pCollectionName.Substring(0, 5) == "Stock")
                {
                    GameObject objInstanceF = Instantiate(SelectedFramePrefab, objCollection.transform);
                    objInstanceF.transform.localPosition = pos;
                    //objCollection.transform.position += WaitingPlacePos;
                }

            }
        }

        ////prefabの呼び出し
        //foreach (var objI in FragmentPrefabs)
        //{
        //    if (objI.name == pFieldPatrsName)
        //    {
        //        GameObject objInstance = Instantiate(objI, objCollection.transform, false);
        //        objCollection.transform.parent = objCollection.transform;
        //        Vector3 pos = objInstance.transform.position;
        //        pos.x = objCollection.transform.position.x + (float)(pBlockX) * 10f;// BlockIntervalX;
        //        pos.y = objCollection.transform.position.y + (float)(pBlockY) * 2.5f;// BlockIntervalY;
        //        pos.z = objCollection.transform.position.z + (float)(pBlockZ) * 10f;// BlockIntervalZ;
        //        objInstance.transform.position = pos;
        //        Vector3 rot = objInstance.transform.eulerAngles;
        //        rot.y = 90f * (float)pRotate;
        //        objInstance.transform.eulerAngles = rot;

        //        //ストックのときは、赤枠を付与して、待機場所に移動させる
        //        if (pCollectionName.Substring(0, 5) == "Stock")
        //        {
        //            GameObject objInstanceF = Instantiate(SelectedFramePrefab, objCollection.transform);
        //            objInstanceF.transform.position = pos;
        //            objCollection.transform.position = WaitingPlacePos;
        //        }

        //    }
        //}

        return 0;
    }




    //*******************************************************************************************************************************************
    //HPの増減処理
    //*******************************************************************************************************************************************
    public void RecalcStatus(ref CharStatus pCharStatus)
    {
        pCharStatus.HP_Max_Calced = pCharStatus.HP_Max_Base + 0;
        pCharStatus.Offence_Calced = pCharStatus.Offence_Base + 0;
        pCharStatus.Defence_Calced = pCharStatus.Defence_Base + 0;
    }

    public void HP_Control(ref CharStatus pCharStatus, int pHP)
    {
        pCharStatus.HP += pHP;
        if (pCharStatus.HP < 0)
        {
            pCharStatus.HP = 0;
        }

        if (pCharStatus.HP > pCharStatus.HP_Max_Calced)
        {
            pCharStatus.HP = pCharStatus.HP_Max_Calced;
        }
    }


    //*******************************************************************************************************************************************
    //攻撃処理
    //*******************************************************************************************************************************************
    public void PhyzicalAttack(ref CharStatus pCharStatusFrom, ref CharStatus pCharStatusTo, int DiceMin, int DiceMax)
    {
        //前回ダメージを受けてから一定時間は無敵（ハメ殺し防止）
        if (pCharStatusTo.IsPlayer)
        {
            if (pCharStatusTo.LastDamagedTime + 0.75f > Time.time) return;
        }
        else
        {
            if (pCharStatusTo.isKnockBack == true) return;
        }



        //ダメージ計算
        int DamageValue = pCharStatusFrom.Offence_Calced-pCharStatusTo.Defence_Calced + UnityEngine.Random.Range(DiceMin, DiceMax);
        if (DamageValue < 0) DamageValue = 0;
        HP_Control(ref pCharStatusTo, -DamageValue);

        pCharStatusTo.LastDamagedTime = Time.time;

        //ノックバックさせる
        if (DamageValue > 0)
        {
            Vector3 KnockBackDirection = pCharStatusTo.gameObject.transform.position - pCharStatusFrom.gameObject.transform.position;
            KnockBackDirection = KnockBackDirection.normalized * 10;

            float rot = UnityEngine.Random.Range(-10f,10f);
            KnockBackDirection = Quaternion.Euler(0, rot,0) * KnockBackDirection  ;

            float KnockBackTime;
            if (pCharStatusTo.IsPlayer)
            {
                KnockBackTime = 0.2f;
            }
            else
            {
                KnockBackTime = 1.2f;
            }

            pCharStatusTo.startKnockBack(KnockBackTime, KnockBackDirection);
            if(pCharStatusTo.IsPlayer == false)
            {
                pCharStatusTo.gameObject.GetComponent<Animator>().SetTrigger("KnockBack");
            }

        }


        string LogString = pCharStatusFrom.Name + "の攻撃 > " + pCharStatusTo.Name + "は " + DamageValue.ToString() + "のダメージ。";
        Debug.Log(LogString);
    
    




    }

    //*******************************************************************************************************************************************
    //マップの読み込み
    //*******************************************************************************************************************************************
    public void LoadMap(string pMapName,GameObject DontDestroy)
    {
        GameObject WorldMap = GameObject.Find("WorldMap");

        GameObject CurrentMap = WorldMap.transform.Find("CurrentMap").gameObject;
        GameObject CurrentField = CurrentMap.transform.Find("CurrentField").gameObject;
        GameObject CurrentNpc = CurrentMap.transform.Find("CurrentNpc").gameObject;
        GameObject CurrentSpawn = CurrentMap.transform.Find("CurrentSpawn").gameObject;

        GameObject SourceMap = WorldMap.transform.Find(pMapName).gameObject;
        GameObject SourceField = SourceMap.transform.Find("Field").gameObject;
        GameObject SourceNpc = SourceMap.transform.Find("Npc").gameObject;
        GameObject SourceSpawn = SourceMap.transform.Find("Spawn").gameObject;


        //CurrentFieldにあるオブジェクトを破棄
        for (int Index = 0; Index < CurrentField.transform.GetChildCount(); Index++)
        {
            GameObject DistObject = CurrentField.transform.GetChild(Index).gameObject;
            Destroy(DistObject);
        }

        //SourceFieldからCurrentFieldにコピー
        for (int Index = 0; Index < SourceField.transform.GetChildCount(); Index++)
        {
            GameObject SourceObject = SourceField.transform.GetChild(Index).gameObject;
            GameObject Instance = Instantiate(SourceObject, CurrentField.transform);
            Instance.name = SourceObject.name;
            Instance.GetComponent<FragmentParameter>().isDefaultSet = true;
        }

        //CurrentNpcにあるオブジェクトを破棄
        for (int Index = 0; Index < CurrentNpc.transform.GetChildCount(); Index++)
        {
            //2021.6.20 第二パラメータで指定したオブジェクトはDestroyしない。マップ間を移動する際、コルーチンの処理がうまくいかなくなるため。
            if (!(DontDestroy is null)&&(DontDestroy != CurrentNpc.transform.GetChild(Index).gameObject)){
                GameObject DistObject = CurrentNpc.transform.GetChild(Index).gameObject;
                Destroy(DistObject);
            }
        }

        //SourceNpcからCurrentNpcにコピー
        for (int Index = 0; Index < SourceNpc.transform.GetChildCount(); Index++)
        {
            GameObject SourceObject = SourceNpc.transform.GetChild(Index).gameObject;
            GameObject Instance = Instantiate(SourceObject, CurrentNpc.transform);
            Instance.name = SourceObject.name;
        }


        //CurrentSpawnにあるオブジェクトを破棄
        for (int Index = 0; Index < CurrentSpawn.transform.GetChildCount(); Index++)
        {
            GameObject DistObject = CurrentSpawn.transform.GetChild(Index).gameObject;
            Destroy(DistObject);
        }

        //SourceSpawnからCurrentFieldにコピー
        for (int Index = 0; Index < SourceSpawn.transform.GetChildCount(); Index++)
        {
            GameObject SourceObject = SourceSpawn.transform.GetChild(Index).gameObject;
            GameObject Instance = Instantiate(SourceObject, CurrentSpawn.transform);
            Instance.name = SourceObject.name;
        }


        //Enemiesにあるオブジェクトを破棄
        GameObject Enemies = GameObject.Find("Enemies").gameObject;
        for (int Index = 0; Index < Enemies.transform.GetChildCount(); Index++)
        {
            GameObject DistObject = Enemies.transform.GetChild(Index).gameObject;
            Destroy(DistObject);
        }

        //HpBarEnemyUIにあるオブジェクトを破棄
        GameObject HpBarEnemyUI = GameObject.Find("HpBarEnemyUI").gameObject;
        for (int Index = 0; Index < HpBarEnemyUI.transform.GetChildCount(); Index++)
        {
            GameObject DistObject = HpBarEnemyUI.transform.GetChild(Index).gameObject;
            Destroy(DistObject);
        }

        

        //for (int Index = 0; Index < SourceField.transform.GetChildCount(); Index++) {
        //    GameObject Object = SourceField.transform.GetChild(Index).gameObject;
        //    GameObject Instance = Instantiate(Object, CurrentField.transform);
        //    Instance.GetComponent<FragmentParameter>().isDefaultSet = true;
        //}

        CurrentMapName = pMapName;



        if (!(SaveData.MapName is  null)){
            for (int Index = 0; Index < SaveData.MapName.Count; Index++)
            {
                if (SaveData.MapName[Index] == CurrentMapName)
                {
                    LoadFragment(SaveData.MapName[Index], SaveData.FragmentName[Index], SaveData.BlockX[Index], SaveData.BlockY[Index], SaveData.BlockZ[Index], SaveData.Rotate[Index]);
                }

            }

        }



    }


    void DataSave()
    {

        GameObject WorldMap = GameObject.Find("WorldMap");

        GameObject CurrentMap = WorldMap.transform.Find("CurrentMap").gameObject;
        GameObject CurrentField = CurrentMap.transform.Find("CurrentField").gameObject;
        GameObject CurrentNpc = CurrentMap.transform.Find("CurrentNpc").gameObject;
        GameObject CurrentSpawn = CurrentMap.transform.Find("CurrentSpawn").gameObject;

        for (int Index = 0; Index < CurrentField.transform.GetChildCount(); Index++)
        {
            GameObject DistObject = CurrentField.transform.GetChild(Index).gameObject;
            if (DistObject.GetComponent<FragmentParameter>().isDefaultSet == false)
            {
                csSaveData.StSaveFieldData SaveFieldData = new csSaveData.StSaveFieldData();
                SaveFieldData.MapName = CurrentMapName;

                string FragmentName = DistObject.name;
                //FragmentName = FragmentName.Substring(0, FragmentName.Length - "(Clone)".Length);
                SaveFieldData.FragmentName = FragmentName;
                SaveFieldData.BlockX = Mathf.RoundToInt(DistObject.transform.position.x / 10f);
                SaveFieldData.BlockY = Mathf.RoundToInt(DistObject.transform.position.y / 10f);
                SaveFieldData.BlockZ = Mathf.RoundToInt(DistObject.transform.position.z / 10f);
                SaveFieldData.Rotate = Mathf.RoundToInt(DistObject.transform.rotation.eulerAngles.y / 90f);
                SaveData.AddSaveFieldData(SaveFieldData);
            }

        }




        string json = JsonUtility.ToJson(SaveData);

        StreamWriter streamWriter = new StreamWriter(SaveFilePath);
        streamWriter.Write(json);
        streamWriter.Flush();
        streamWriter.Close();



    }

}

[System.Serializable]
public class csSaveData
{


    public string DayDate;
    public List<string> MapName;
    public List<string> FragmentName;
    public List<int> BlockX;
    public List<int> BlockY;
    public List<int> BlockZ;
    public List<int> Rotate;
    public string Test = "aaa";

    public struct StSaveFieldData
    {
        public string MapName;
        public string FragmentName;
        public int BlockX;
        public int BlockY;
        public int BlockZ;
        public int Rotate;
    }

    public void AddSaveFieldData(StSaveFieldData pSaveFieldData)
    {
        if (MapName is null)
        {
            MapName = new List<string>();
            FragmentName = new List<string>();
            BlockX = new List<int>();
            BlockY = new List<int>();
            BlockZ = new List<int>();
            Rotate = new List<int>();

        }
        MapName.Add(pSaveFieldData.MapName);
        FragmentName.Add(pSaveFieldData.FragmentName);
        BlockX.Add(pSaveFieldData.BlockX);
        BlockY.Add(pSaveFieldData.BlockY);
        BlockZ.Add(pSaveFieldData.BlockZ);
        Rotate.Add(pSaveFieldData.Rotate);

        //Array.Resize(ref SaveFieldData, SaveFieldData.GetLength(0) + 1);
        //SaveFieldData[SaveFieldData.GetLength(0) - 1] = pSaveFieldData;
    }


    public void ClearAllSaveFieldData()
    {
        MapName = new List<string>();
        FragmentName = new List<string>();
        BlockX = new List<int>();
        BlockY = new List<int>();
        BlockZ = new List<int>();
        Rotate = new List<int>();
    }

    public void ClearSaveFieldData(string pMapName)
    {
        //List<StSaveFieldData> NewSaveFieldData = new List<StSaveFieldData>();
        //for (int Index = 0;Index < SaveFieldData.Count; Index++)
        //{
        //    if (SaveFieldData[Index].MapName != pMapName)
        //    {
        //        NewSaveFieldData.Add(SaveFieldData[Index]);
        //    }
        //}
        //SaveFieldData = NewSaveFieldData;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}