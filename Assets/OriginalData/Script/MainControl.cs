using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class MainControl : MonoBehaviour
{
    //プレイヤーキャラクター関連
    public GameObject objPlayer;
    public GameObject objPlayerAppearance;
    private Animator objAnimator;
    private CharacterController objCharController;
    //private bool MoveFlag = false;

    public GameObject objFieldCursor;
    public bool isFieldCursorSet;
    public GameObject objSelectedFrame;

    public GameObject EnemyControl;

    public CharStatus PlayerStatus;

    //Unityの公式スクリプトレファンスにあるものと同じ
    //十字キーのみで操作(矢印キー＝前後左右移動)
    //CharacterControllerが必要

    //（Public＝インスペクタで調整可能）
    private float walkSpeed = 5.0f;  //歩行速度
    private float jumpSpeed = 8f;  //ジャンプ力
    private float gravity = 20.0f;  //重力の大きさ


    private bool jumpFlag = false; //連続ジャンプ禁止フラグ
    //private float LastAttackTime = -0.5f;//前回攻撃操作をした時刻

    //private float AttackFreezeTime = 0.5f;//攻撃操作で移動やジャンプを禁止する時間

    //private bool runFlag = false; //アニメーショントリガーを何度も引かないようにするためのフラグ
    //private bool idleFlag = true; //アニメーショントリガーを何度も引かないようにするためのフラグ
    private bool buttonFlag_Skill1 = false; //連続操作禁止フラグ
    private bool autoRunFlag = false; //オートラン

    static bool isSliding = false;

    public float DirectionFollowSpeed = 1200.0f;   // 移動方向に対してキャラクターの向きが追随するときの速度

    private Vector3 moveDirection = Vector3.zero;   //  移動する方向とベクトルの変数（最初は初期化しておく）
    private Vector3 charDirection = Vector3.zero;   //  キャラクターの向き（最初は初期化しておく）
    private Vector3 moveDirection_Past = Vector3.zero;
    private Vector3 moveDirection_autoRun = Vector3.zero;


    //カメラ関連
    public GameObject objCamera;

    //ターゲットカーソル
    public GameObject objTargetCursor;

    //UI、デバッグ関連
    public GameObject objUI_Debug;
    private Text objText_Debug;
    public GameObject objUI_HP;
    private Text objText_HP;

    public float BlockIntervalX = 10f;
    public float BlockIntervalY = 2.5f;
    public float BlockIntervalZ = 10f;

    //操作禁止フラグ（別スクリプトから操作する）
    public bool isControllEnabled = true;
    public bool isIdle = true;

    //フィールド関連
    public GameObject CurrentField;
    private GameObject[] FragmentPrefabs;

    private Vector3 WaitingPlacePos = new Vector3(0, 0, 1000f);

    //public GameObject CurrentWorld;
    public GameObject FragmentsListUI;
    public string CurrentMapName = "";

    //モード
    public string Mode = "Main";

    //メニュー画面群
    public GameObject objWindow_FieldPartsSelect;

    public GameObject HpBar;

    public int SaveFileNumber = 0;

    // Start is called before the first frame update
    void Start()
    {



        //objControllerManager = gameObject.GetComponent<s_ControllerManager>();

        objCharController = objPlayer.GetComponent<CharacterController>();
        objAnimator = objPlayerAppearance.GetComponent<Animator>();
        PlayerStatus = objPlayer.GetComponent<CharStatus>();

        objText_Debug = objUI_Debug.GetComponent<Text>();
        objText_HP = objUI_HP.GetComponent<Text>();

        //着地させる
        Landing(objPlayer.gameObject);

        ////カメラ初期位置
        //initCamera();

        //大地のかけら　Prefab一覧のの読み込み
        FragmentPrefabs = Resources.LoadAll<GameObject>("FragmentPrefabs");







        //テストデータ
        LoadMap("FirstVillage");



        //LoadFieldParts("World_001", "Road2", 0, 0, 0, 1);
        //LoadFieldParts("World_001", "Road2", 1, 0, 0, 3);
        //LoadFieldParts("World_001", "Green", 0, 0, 1, 0);
        //LoadFieldParts("World_001", "Road2", 1, 0, 1, 1);
        //LoadFieldParts("World_001", "RiverBridge", 0, 0, -1, 1);


        //CurrentWorld = GameObject.Find("World_001");

        //ステータス設定
        PlayerStatus.IsPlayer = true;
        PlayerStatus.HP_Max_Base =256;
        PlayerStatus.Offence_Base  = 10;
        PlayerStatus.Defence_Base = 5;
        RecalcStatus(ref PlayerStatus);
        PlayerStatus.HP = PlayerStatus.HP_Max_Calced;
        HpBar.GetComponent<HpBar>().ResetBar(PlayerStatus.HP, PlayerStatus.HP_Max_Calced);

        //FieldPartsSelect画面はいったん非表示
        objWindow_FieldPartsSelect.SetActive(false);

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
        objText_Debug.text = "";




        //---------------------------------------------------------------------------------------------------------------------------------------
        // 移動入力(WASD,左スティック）に応じて、移動方向＋量(moveDirection）を設定
        //---------------------------------------------------------------------------------------------------------------------------------------
        moveDirection_Past = moveDirection;
        var cameraForward = Vector3.Scale(objCamera.transform.forward, new Vector3(1, 0, 1)).normalized;  //  カメラに向かって水平の単位ベクトルを求める。
        if (isSliding == false && objCharController.isGrounded && isControllEnabled == true)
        {
            if (Mode == "Main")
            {

                moveDirection = walkSpeed * cameraForward * -Input.GetAxis("LstickUD"); //カメラの前後に対して上下キーの入力成分を加算
                moveDirection += walkSpeed * objCamera.transform.right * Input.GetAxis("LstickLR");//カメラから直角の方向に対して左右の入力成分を加算（多分、カメラが傾いていないことが前提）
            }
            else
            {
                moveDirection = new Vector3(0, 0, 0);
                //moveDirection = walkSpeed * cameraForward * 0; //カメラの前後に対して上下キーの入力成分を加算
                //moveDirection += walkSpeed * objCamera.transform.right *0;//カメラから直角の方向に対して左右の入力成分を加算（多分、カメラが傾いていないことが前提）

            }

        }
        moveDirection.y = moveDirection_Past.y;

        //---------------------------------------------------------------------------------------------------------------------------------------
        //移動入力(WASD,左スティック）があるときは、オブジェクトの方向を変える
        //---------------------------------------------------------------------------------------------------------------------------------------
        if ((Input.GetAxis("LstickLR") != 0 || Input.GetAxis("LstickUD") != 0) && Mode == "Main" && isControllEnabled == true)
        {
            objPlayer.transform.rotation = Quaternion.LookRotation(moveDirection);
            //垂直に向ける処理をここに入れて悩んだが、LookRotationにしたら、不要になった。

            if (objCharController.isGrounded)
            {
                //攻撃中はアニメーションを遷移させない
                if (PlayerStatus.isAttack == false)
                {
                    objAnimator.SetBool("idle", false);

                }

            }

        }
        else
        {
            if (objCharController.isGrounded)
            {
                objAnimator.SetBool("idle", true);
                //objAnimator.SetBool("run", false);
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
        if (objCharController.isGrounded)
        {
            moveDirection.y = 0f;
        }

        if (Input.GetButton("Triangle") == true && Mode== "Main" && isControllEnabled == true)
        {
            if (objCharController.isGrounded && (isSliding == false))
            {
                if (jumpFlag == false)
                {

                    //前回攻撃操作をしてから一定時間はジャンプ禁止
                    if (PlayerStatus.isAttack == false)
                    {
                        //ジャンプ実行
                        moveDirection.y = jumpSpeed;

                        //アニメーション切り替え
                        objAnimator.SetTrigger("jump");

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
                    if (objCharController.isGrounded)
                    {
                        //Debug.Log("攻撃");
                        objAnimator.SetBool("idle", true);
                        objAnimator.SetTrigger("attack");
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
        Vector3 startVec = objPlayerAppearance.transform.position + objPlayer.transform.forward * 0f;
        Vector3 endVec = objPlayerAppearance.transform.position + objPlayer.transform.forward * 0f;
        startVec.y = 9999;
        endVec.y = -9999;
        RaycastHit slideHit;
        if (Physics.Linecast(startVec, endVec, out slideHit))
        {
            //衝突した際の面の角度が滑らせたい角度以上かどうか
            if (Vector3.Angle(slideHit.normal, Vector3.up) > objCharController.slopeLimit)
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
        if (isSliding && objCharController.isGrounded)
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
                objCharController.Move(moveDirection * Time.deltaTime);
            }
            else
            {
                objCharController.Move(moveDirection_autoRun * Time.deltaTime);
            }
        }
        else
        {
            Vector3 direction = PlayerStatus.KnockBackDirection;
            direction.y -= gravity * Time.deltaTime;
            objCharController.Move(direction * Time.deltaTime * 1f);
            PlayerStatus.KnockBackDirection *= 0.98f;//だんだんゆっくり



        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        //フィールドパーツの外には落ちないようにするチェック
        //---------------------------------------------------------------------------------------------------------------------------------------
        if (CheckAbyss(objPlayer))
        {
            Vector3 AbyssPos = objPlayer.transform.position;
            AbyssPos.x = Mathf.Ceil((AbyssPos.x - 5f) / 10f) * 10f;
            AbyssPos.z = Mathf.Ceil((AbyssPos.z - 5f) / 10f) * 10f;
            AbyssPos.y = 0;
            objFieldCursor.transform.position = AbyssPos;
            isFieldCursorSet = true;
            objFieldCursor.GetComponent<FieldCursor>().Appear();

            if (PlayerStatus.isKnockBack == false)
            {
                moveDirection.y = 0;
                objCharController.Move(moveDirection * -Time.deltaTime * 2);
                moveDirection.x = 0;
                moveDirection.z = 0;
            }
            else
            {
                Vector3 direction = PlayerStatus.KnockBackDirection;
                direction.y = 0;
                objCharController.Move(direction * -Time.deltaTime * 2);
            }



        }
        else
        {
            //プレイヤーがobjFieldCursor一定の距離離れたら、objFieldCursorを消す（遠くの場所に移動）
            //2021.5.28 FieldPatsをSetするときに急に変なところに移動するバグの対応のため、距離を 7f -> 9f に変更。
            if (Vector3.Distance(objPlayer.transform.position, objFieldCursor.transform.position) > 9f)
            {
                Vector3 AbyssPos = objPlayer.transform.position;
                AbyssPos.x = 0;
                AbyssPos.z = 99999f;
                AbyssPos.y = 0;
                //objFieldCursor.transform.position = AbyssPos;
                isFieldCursorSet = false;
                objFieldCursor.GetComponent<FieldCursor>().Disappear();
            }

        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        //フィールドパーツセット
        //---------------------------------------------------------------------------------------------------------------------------------------
        //if (Input.GetButtonDown("Circle") == true && Mode == "Main" && moveDirection.x == 0 && moveDirection.z == 0)
        //{
        //    if (objTargetCursor.GetComponent<s_TargetCursor>().objTarget is null)
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
        if (objTargetCursor.GetComponent<TargetCursor>().objTarget is null)
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
        //objPlayerに合わせて、objPlayerAppearanceを追随させる
        //---------------------------------------------------------------------------------------------------------------------------------------
        Vector3 tmpVector3 = objPlayerAppearance.transform.position;
        tmpVector3.x = objPlayer.transform.position.x;
        tmpVector3.y = objPlayer.transform.position.y - 0.5f;
        tmpVector3.z = objPlayer.transform.position.z;
        objPlayerAppearance.transform.position = tmpVector3;


        //  移動キーの入力があるときに、向きも追随させる
        if (Input.GetAxis("LstickUD") != 0 || Input.GetAxis("LstickLR") != 0)
        {
            objPlayerAppearance.transform.rotation = Quaternion.RotateTowards(objPlayerAppearance.transform.rotation, objPlayer.transform.rotation, DirectionFollowSpeed * Time.deltaTime);   // 向きを q に向けてじわ～っと変化させる.
        }

        //垂直にする　objPlayerAppearanceだけだと変な動き。objPlayerのColliderが傾くと変な動きになるので、objPlayerも垂直にするよう追加 20210511
        Quaternion tmpQuaternion = objPlayerAppearance.transform.rotation;
        tmpQuaternion.x = 0;
        tmpQuaternion.z = 0;
        objPlayerAppearance.transform.rotation = tmpQuaternion;

        tmpQuaternion = objPlayer.transform.rotation;
        tmpQuaternion.x = 0;
        tmpQuaternion.z = 0;
        objPlayer.transform.rotation = tmpQuaternion;


        //---------------------------------------------------------------------------------------------------------------------------------------
        // 
        //---------------------------------------------------------------------------------------------------------------------------------------
        int motionIdol = Animator.StringToHash("Base Layer.idle");
        if (objAnimator.GetCurrentAnimatorStateInfo(0).nameHash == motionIdol)
        {
            isIdle = true;
        }
        else
        {
            isIdle = false;
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        //UI表示
        //---------------------------------------------------------------------------------------------------------------------------------------

        // HP
        objText_HP.text = "";
        objText_HP.text = "HP:" + PlayerStatus.HP.ToString() + "/" + PlayerStatus.HP_Max_Calced.ToString();
        HpBar.GetComponent<HpBar>().SetValue(PlayerStatus.HP);

        //デバッグ
        //objText_Debug.text += "isGrounded:" + objCharController.isGrounded.ToString() + "\n";
        //objText_Debug.text += "moveDirection.z:" + moveDirection.ToString() + "\n";
        //objText_Debug.text += "CheckAbyss:" + CheckAbyss(objPlayer).ToString() + "\n";
        //objText_Debug.text += "isKnockBack:" + PlayerStatus.isKnockBack.ToString() + "\n";

        objText_Debug.text += "EnemyCount:" + EnemyControl.GetComponent<EnemyControl>().SpawnCount + "\n";

        //objText_Debug.text += "LstickUD:" + Input.GetAxis("LstickUD").ToString() + "\n";
        //objText_Debug.text += "LstickLR:" + Input.GetAxis("LstickLR").ToString() + "\n";
        //objText_Debug.text += "RstickUD:" + Input.GetAxis("RstickUD").ToString() + "\n";
        //objText_Debug.text += "RstickLR:" + Input.GetAxis("RstickLR").ToString() + "\n";
        //objText_Debug.text += "HatUD:" + Input.GetAxis("HatUD").ToString() + "\n";
        //objText_Debug.text += "HatLR:" + Input.GetAxis("HatLR").ToString() + "\n";
        //objText_Debug.text += "Circle:" + Input.GetButton("Circle").ToString() + "\n";
        //objText_Debug.text += "Cross:" + Input.GetButton("Cross").ToString() + "\n";
        //objText_Debug.text += "Square:" + Input.GetButton("Square").ToString() + "\n";
        //objText_Debug.text += "Triangle:" + Input.GetButton("Triangle").ToString() + "\n";
        //objText_Debug.text += "moveDirection:" + moveDirection.ToString() + "\n";

    }





    //*******************************************************************************************************************************************
    // オブジェクトを着地させる
    //*******************************************************************************************************************************************
    void Landing(GameObject obj)
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
    public int LoadFieldParts(string pCollectionName,string pFieldPatrsName, int pBlockX, int pBlockY, int pBlockZ, int pRotate)
    {
        //配下のヒエラルキーを探して、ない場合は新規作成
        GameObject objCollection = GameObject.Find(pCollectionName);
        if (objCollection is null)
        {
            objCollection = new GameObject(pCollectionName);
            objCollection.transform.position = new Vector3(0, 0, 0);
            objCollection.transform.parent = CurrentField.transform;

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

        //prefabの呼び出し
        foreach (var objI in FragmentPrefabs)
        {
            if (objI.name == pFieldPatrsName)
            {
                GameObject objInstance = Instantiate(objI, objCollection.transform, false);
                objCollection.transform.parent = objCollection.transform;
                Vector3 pos = objInstance.transform.position;
                pos.x = objCollection.transform.position.x + (float)(pBlockX) * 10f;// BlockIntervalX;
                pos.y = objCollection.transform.position.y + (float)(pBlockY) * 2.5f;// BlockIntervalY;
                pos.z = objCollection.transform.position.z + (float)(pBlockZ) * 10f;// BlockIntervalZ;
                objInstance.transform.position = pos;
                Vector3 rot = objInstance.transform.eulerAngles;
                rot.y = 90f * (float)pRotate;
                objInstance.transform.eulerAngles = rot;

                //ストックのときは、赤枠を付与して、待機場所に移動させる
                if (pCollectionName.Substring(0, 5) == "Stock")
                {
                    GameObject objInstanceF = Instantiate(objSelectedFrame, objCollection.transform);
                    objInstanceF.transform.position = pos;
                    objCollection.transform.position = WaitingPlacePos;
                }

            }
        }

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
    private void LoadMap(string pMapName)
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

        for (int Index = 0; Index < CurrentField.transform.GetChildCount(); Index++)
        {
            GameObject DistObject = CurrentField.transform.GetChild(Index).gameObject;
            Destroy(DistObject);
        }

        for (int Index = 0; Index < SourceField.transform.GetChildCount(); Index++)
        {
            GameObject SourceObject = SourceField.transform.GetChild(Index).gameObject;
            GameObject Instance = Instantiate(SourceObject, CurrentField.transform);
            Instance.GetComponent<FragmentParameter>().isDefaultSet = true;

        }



        for (int Index = 0; Index < SourceField.transform.GetChildCount(); Index++) {
            GameObject Object = SourceField.transform.GetChild(Index).gameObject;
            GameObject Instance = Instantiate(Object, CurrentField.transform);
            Instance.GetComponent<FragmentParameter>().isDefaultSet = true;

        }
        CurrentMapName = pMapName;
    }

}

