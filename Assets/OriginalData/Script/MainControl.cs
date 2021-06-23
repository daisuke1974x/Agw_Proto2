using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;




public class MainControl : MonoBehaviour
{


    //�q�G�����L�[�̃I�u�W�F�N�g�Q�Ɓi���X�N���v�g������A�N�Z�X�ł���悤�A���ʂɂ���j
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
    //public GameObject objWindow_FieldPartsSelect;    //���j���[��ʌQ
    public GameObject HpBar;

    //�C���X�y�N�^�ł͐ݒ肵�Ȃ����A���X�N���v�g����Q�Ƃ�����X�V�����肷�����
    public float BlockIntervalX = 10f;//��n�̂�����̃X�i�b�v�T�C�Y
    public float BlockIntervalY = 2.5f;//��n�̂�����̃X�i�b�v�T�C�Y
    public float BlockIntervalZ = 10f;//��n�̂�����̃X�i�b�v�T�C�Y
    public bool isControllEnabled = true; //����֎~�t���O
    public bool isIdle = true;//�A�C�h�����O�����ǂ���
    public Vector3 WaitingPlacePos = new Vector3(0, 0, 90000f);    //��n�̂�����̑ҋ@�ꏊ
    public string CurrentMapName = "";    //���݂̃}�b�v��
    public CharStatus PlayerStatus;
    public bool isFieldCursorSet;    //�t�B�[���h�J�[�\�����\������Ă��邩�ǂ���


    private Animator AnimatorObject;
    private CharacterController CharControllerObject;

    private float walkSpeed = 5.0f;  //���s���x
    private float jumpSpeed = 8f;  //�W�����v��
    private float gravity = 20.0f;  //�d�͂̑傫��
    private bool jumpFlag = false; //�A���W�����v�֎~�t���O
    private bool buttonFlag_Skill1 = false; //�A������֎~�t���O
    private bool autoRunFlag = false; //�I�[�g����
    static bool isSliding = false;

    private float DirectionFollowSpeed = 1200.0f;   // �ړ������ɑ΂��ăL�����N�^�[�̌������ǐ�����Ƃ��̑��x
    private Vector3 moveDirection = Vector3.zero;   //  �ړ���������ƃx�N�g���̕ϐ��i�ŏ��͏��������Ă����j
    private Vector3 charDirection = Vector3.zero;   //  �L�����N�^�[�̌����i�ŏ��͏��������Ă����j
    private Vector3 moveDirection_Past = Vector3.zero;
    private Vector3 moveDirection_autoRun = Vector3.zero;

    //��n�̂������prefab��frefab���烍�[�h���Ă�������
    private GameObject[] FragmentPrefabs;


    //���[�h  ����͂�����p�~���遟����������������������������������������������
    public string Mode = "Main";



    //�Z�[�u�f�[�^�֘A
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


        //���n������
        Landing(PlayerObject.gameObject);

        ////�J���������ʒu
        //initCamera();

        //��n�̂�����@Prefab�ꗗ�̂̓ǂݍ���
        FragmentPrefabs = Resources.LoadAll<GameObject>("FragmentPrefabs");


        //�Z�[�u�f�[�^�̓ǂݍ��݊֘A
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


        //CurrentWorld�ȊO���A�N�e�B�u�ɂ���
        GameObject WorldMap = GameObject.Find("WorldMap");

        for (int Index = 0; Index < WorldMap.transform.GetChildCount(); Index++)
        {
            if (WorldMap.transform.GetChild(Index).name != "CurrentMap")
            {
                WorldMap.transform.GetChild(Index).gameObject.SetActive(false);
            }
        }


        //�e�X�g�f�[�^
        LoadMap("FirstVillage",null);
        //LoadMap("SecondTown",null);



        //�X�e�[�^�X�ݒ�
        PlayerStatus.IsPlayer = true;
        PlayerStatus.HP_Max_Base =256;
        PlayerStatus.Offence_Base  = 10;
        PlayerStatus.Defence_Base = 5;
        RecalcStatus(ref PlayerStatus);
        PlayerStatus.HP = PlayerStatus.HP_Max_Calced;
        HpBar.GetComponent<HpBar>().ResetBar(PlayerStatus.HP, PlayerStatus.HP_Max_Calced);

        //FieldPartsSelect��ʂ͂��������\��
        //objWindow_FieldPartsSelect.SetActive(false);

        //StockList�̍X�V�i���ƂŃT�u���[�`��������j
        //List<stStockList> FieldPartsStockList = new List<stStockList>(); //������new ����ƁA���̃X�N���v�g�ŎQ�Ƃ����Ƃ��A0���ɂȂ�̂ŁAnew���Ȃ�����
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

        ////�X�g�b�N���X�g�̔z�u�i���ƂŁA�K�؂ȏꏊ�Ɉړ�����j
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
        // �ړ�����(WASD,���X�e�B�b�N�j�ɉ����āA�ړ������{��(moveDirection�j��ݒ�
        //---------------------------------------------------------------------------------------------------------------------------------------
        moveDirection_Past = moveDirection;
        var cameraForward = Vector3.Scale(MainCameraObject.transform.forward, new Vector3(1, 0, 1)).normalized;  //  �J�����Ɍ������Đ����̒P�ʃx�N�g�������߂�B
        if (isSliding == false && CharControllerObject.isGrounded && isControllEnabled == true)
        {
            if (Mode == "Main")
            {

                moveDirection = walkSpeed * cameraForward * -Input.GetAxis("LstickUD"); //�J�����̑O��ɑ΂��ď㉺�L�[�̓��͐��������Z
                moveDirection += walkSpeed * MainCameraObject.transform.right * Input.GetAxis("LstickLR");//�J�������璼�p�̕����ɑ΂��č��E�̓��͐��������Z�i�����A�J�������X���Ă��Ȃ����Ƃ��O��j
            }
            else
            {
                moveDirection = new Vector3(0, 0, 0);
                //moveDirection = walkSpeed * cameraForward * 0; //�J�����̑O��ɑ΂��ď㉺�L�[�̓��͐��������Z
                //moveDirection += walkSpeed * MainCameraObject.transform.right *0;//�J�������璼�p�̕����ɑ΂��č��E�̓��͐��������Z�i�����A�J�������X���Ă��Ȃ����Ƃ��O��j

            }

        }
        moveDirection.y = moveDirection_Past.y;

        //---------------------------------------------------------------------------------------------------------------------------------------
        //�ړ�����(WASD,���X�e�B�b�N�j������Ƃ��́A�I�u�W�F�N�g�̕�����ς���
        //---------------------------------------------------------------------------------------------------------------------------------------
        if ((Input.GetAxis("LstickLR") != 0 || Input.GetAxis("LstickUD") != 0) && Mode == "Main" && isControllEnabled == true)
        {
            PlayerObject.transform.rotation = Quaternion.LookRotation(moveDirection);
            //�����Ɍ����鏈���������ɓ���ĔY�񂾂��ALookRotation�ɂ�����A�s�v�ɂȂ����B

            if (CharControllerObject.isGrounded)
            {
                //�U�����̓A�j���[�V������J�ڂ����Ȃ�
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

        //�U�����͌���
        if (PlayerStatus.isAttack)
        {
            moveDirection.x *= 0.5f;
            moveDirection.z *= 0.5f;
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        // �W�����v����
        //---------------------------------------------------------------------------------------------------------------------------------------
        //���n�����Ƃ�
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

                    //�O��U����������Ă����莞�Ԃ̓W�����v�֎~
                    if (PlayerStatus.isAttack == false)
                    {
                        //�W�����v���s
                        moveDirection.y = jumpSpeed;

                        //�A�j���[�V�����؂�ւ�
                        AnimatorObject.SetTrigger("jump");

                        //�A���W�����v�֎~�t���O��ON
                        jumpFlag = true;

                    }

                }
            }
        }
        else
        {
            //�A���W�����v�֎~�t���O��OFF
            jumpFlag = false;
        }







        //---------------------------------------------------------------------------------------------------------------------------------------
        // �����U������
        //---------------------------------------------------------------------------------------------------------------------------------------
        if (Input.GetButton("Square") == true && Mode == "Main" && isControllEnabled == true)
        {
            if (buttonFlag_Skill1 == false)
            {
                //�O��U�����삩���莞�Ԃ��o�߂��Ă��邱��
                if (PlayerStatus.isAttack == false)
                {
                    //���n���ł��邱��
                    if (CharControllerObject.isGrounded)
                    {
                        //Debug.Log("�U��");
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
        //�⓹�̂Ƃ����点�鏈��
        //---------------------------------------------------------------------------------------------------------------------------------------
        //http://hideapp.cocolog-nifty.com/blog/2015/03/unity-tips-char.html
        Vector3 startVec = PlayerAppearanceObject.transform.position + PlayerObject.transform.forward * 0f;
        Vector3 endVec = PlayerAppearanceObject.transform.position + PlayerObject.transform.forward * 0f;
        startVec.y = 9999;
        endVec.y = -9999;
        RaycastHit slideHit;
        if (Physics.Linecast(startVec, endVec, out slideHit))
        {
            //�Փ˂����ۂ̖ʂ̊p�x�����点�����p�x�ȏォ�ǂ���
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

        //����t���O�������Ă��āA�����n���Ă���Ƃ��A���点��
        if (isSliding && CharControllerObject.isGrounded)
        {
            float slideSpeed = 10f;
            Vector3 hitNormal = slideHit.normal;
            moveDirection.x += hitNormal.x * slideSpeed * Time.deltaTime;
            moveDirection.y -= gravity * Time.deltaTime;//�d�͗���
            moveDirection.z += hitNormal.z * slideSpeed * Time.deltaTime;
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        //�d�͂ɉ�������������
        //---------------------------------------------------------------------------------------------------------------------------------------
        moveDirection.y -= gravity * Time.deltaTime;

        //---------------------------------------------------------------------------------------------------------------------------------------
        //�Z�o���� moveDirection �Ɋ�Â��A�I�u�W�F�N�g���ړ�������
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
            PlayerStatus.KnockBackDirection *= 0.98f;//���񂾂�������



        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        //�t�B�[���h�p�[�c�̊O�ɂ͗����Ȃ��悤�ɂ���`�F�b�N
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
            //�v���C���[��FieldCursorObject���̋������ꂽ��AFieldCursorObject�������i�����̏ꏊ�Ɉړ��j
            //2021.5.28 FieldPats��Set����Ƃ��ɋ}�ɕςȂƂ���Ɉړ�����o�O�̑Ή��̂��߁A������ 7f -> 9f �ɕύX�B
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
        //�t�B�[���h�p�[�c�Z�b�g
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
        //PlayerObject�ɍ��킹�āAPlayerAppearanceObject��ǐ�������
        //---------------------------------------------------------------------------------------------------------------------------------------
        Vector3 tmpVector3 = PlayerAppearanceObject.transform.position;
        tmpVector3.x = PlayerObject.transform.position.x;
        tmpVector3.y = PlayerObject.transform.position.y - 0.5f;
        tmpVector3.z = PlayerObject.transform.position.z;
        PlayerAppearanceObject.transform.position = tmpVector3;


        //  �ړ��L�[�̓��͂�����Ƃ��ɁA�������ǐ�������
        if (Input.GetAxis("LstickUD") != 0 || Input.GetAxis("LstickLR") != 0)
        {
            PlayerAppearanceObject.transform.rotation = Quaternion.RotateTowards(PlayerAppearanceObject.transform.rotation, PlayerObject.transform.rotation, DirectionFollowSpeed * Time.deltaTime);   // ������ q �Ɍ����Ă���`���ƕω�������.
        }

        //�����ɂ���@PlayerAppearanceObject�������ƕςȓ����BPlayerObject��Collider���X���ƕςȓ����ɂȂ�̂ŁAPlayerObject�������ɂ���悤�ǉ� 20210511
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
            Debug.Log("�Z�[�u�J�n");
            DataSave();
            Debug.Log("�Z�[�u����");
        }


        //---------------------------------------------------------------------------------------------------------------------------------------
        //UI�\��
        //---------------------------------------------------------------------------------------------------------------------------------------

        // HP
        HpBar.GetComponent<HpBar>().SetValue(PlayerStatus.HP);



    }





    //*******************************************************************************************************************************************
    // �I�u�W�F�N�g�𒅒n������
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
    // �I�u�W�F�N�g�̑������ޗ��̒ꂩ�ǂ����`�F�b�N
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
    //�t�B�[���h�p�[�c�̔z�u
    //*******************************************************************************************************************************************
    public int LoadFragment(string pCollectionName,string pFieldPatrsName, int pBlockX, int pBlockY, int pBlockZ, int pRotate)
    {
        GameObject objCollection;
        
        if (pCollectionName.Substring(0, 5) == "Stock")
        {
            objCollection = GameObject.Find(pCollectionName);
            //�z���̃q�G�����L�[��T���āA�Ȃ��ꍇ�͐V�K�쐬
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

        //�\���̂ɃZ�b�g���āALIST�ɒǉ�
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

        //prefab�̌Ăяo��
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

                //�X�g�b�N�̂Ƃ��́A�Ԙg��t�^���āA�ҋ@�ꏊ�Ɉړ�������
                if (pCollectionName.Substring(0, 5) == "Stock")
                {
                    GameObject objInstanceF = Instantiate(SelectedFramePrefab, objCollection.transform);
                    objInstanceF.transform.localPosition = pos;
                    //objCollection.transform.position += WaitingPlacePos;
                }

            }
        }

        ////prefab�̌Ăяo��
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

        //        //�X�g�b�N�̂Ƃ��́A�Ԙg��t�^���āA�ҋ@�ꏊ�Ɉړ�������
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
    //HP�̑�������
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
    //�U������
    //*******************************************************************************************************************************************
    public void PhyzicalAttack(ref CharStatus pCharStatusFrom, ref CharStatus pCharStatusTo, int DiceMin, int DiceMax)
    {
        //�O��_���[�W���󂯂Ă����莞�Ԃ͖��G�i�n���E���h�~�j
        if (pCharStatusTo.IsPlayer)
        {
            if (pCharStatusTo.LastDamagedTime + 0.75f > Time.time) return;
        }
        else
        {
            if (pCharStatusTo.isKnockBack == true) return;
        }



        //�_���[�W�v�Z
        int DamageValue = pCharStatusFrom.Offence_Calced-pCharStatusTo.Defence_Calced + UnityEngine.Random.Range(DiceMin, DiceMax);
        if (DamageValue < 0) DamageValue = 0;
        HP_Control(ref pCharStatusTo, -DamageValue);

        pCharStatusTo.LastDamagedTime = Time.time;

        //�m�b�N�o�b�N������
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


        string LogString = pCharStatusFrom.Name + "�̍U�� > " + pCharStatusTo.Name + "�� " + DamageValue.ToString() + "�̃_���[�W�B";
        Debug.Log(LogString);
    
    




    }

    //*******************************************************************************************************************************************
    //�}�b�v�̓ǂݍ���
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


        //CurrentField�ɂ���I�u�W�F�N�g��j��
        for (int Index = 0; Index < CurrentField.transform.GetChildCount(); Index++)
        {
            GameObject DistObject = CurrentField.transform.GetChild(Index).gameObject;
            Destroy(DistObject);
        }

        //SourceField����CurrentField�ɃR�s�[
        for (int Index = 0; Index < SourceField.transform.GetChildCount(); Index++)
        {
            GameObject SourceObject = SourceField.transform.GetChild(Index).gameObject;
            GameObject Instance = Instantiate(SourceObject, CurrentField.transform);
            Instance.name = SourceObject.name;
            Instance.GetComponent<FragmentParameter>().isDefaultSet = true;
        }

        //CurrentNpc�ɂ���I�u�W�F�N�g��j��
        for (int Index = 0; Index < CurrentNpc.transform.GetChildCount(); Index++)
        {
            //2021.6.20 ���p�����[�^�Ŏw�肵���I�u�W�F�N�g��Destroy���Ȃ��B�}�b�v�Ԃ��ړ�����ہA�R���[�`���̏��������܂������Ȃ��Ȃ邽�߁B
            if (!(DontDestroy is null)&&(DontDestroy != CurrentNpc.transform.GetChild(Index).gameObject)){
                GameObject DistObject = CurrentNpc.transform.GetChild(Index).gameObject;
                Destroy(DistObject);
            }
        }

        //SourceNpc����CurrentNpc�ɃR�s�[
        for (int Index = 0; Index < SourceNpc.transform.GetChildCount(); Index++)
        {
            GameObject SourceObject = SourceNpc.transform.GetChild(Index).gameObject;
            GameObject Instance = Instantiate(SourceObject, CurrentNpc.transform);
            Instance.name = SourceObject.name;
        }


        //CurrentSpawn�ɂ���I�u�W�F�N�g��j��
        for (int Index = 0; Index < CurrentSpawn.transform.GetChildCount(); Index++)
        {
            GameObject DistObject = CurrentSpawn.transform.GetChild(Index).gameObject;
            Destroy(DistObject);
        }

        //SourceSpawn����CurrentField�ɃR�s�[
        for (int Index = 0; Index < SourceSpawn.transform.GetChildCount(); Index++)
        {
            GameObject SourceObject = SourceSpawn.transform.GetChild(Index).gameObject;
            GameObject Instance = Instantiate(SourceObject, CurrentSpawn.transform);
            Instance.name = SourceObject.name;
        }


        //Enemies�ɂ���I�u�W�F�N�g��j��
        GameObject Enemies = GameObject.Find("Enemies").gameObject;
        for (int Index = 0; Index < Enemies.transform.GetChildCount(); Index++)
        {
            GameObject DistObject = Enemies.transform.GetChild(Index).gameObject;
            Destroy(DistObject);
        }

        //HpBarEnemyUI�ɂ���I�u�W�F�N�g��j��
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