using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class MainControl : MonoBehaviour
{
    //�v���C���[�L�����N�^�[�֘A
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

    //Unity�̌����X�N���v�g���t�@���X�ɂ�����̂Ɠ���
    //�\���L�[�݂̂ő���(���L�[���O�㍶�E�ړ�)
    //CharacterController���K�v

    //�iPublic���C���X�y�N�^�Œ����\�j
    private float walkSpeed = 5.0f;  //���s���x
    private float jumpSpeed = 8f;  //�W�����v��
    private float gravity = 20.0f;  //�d�͂̑傫��


    private bool jumpFlag = false; //�A���W�����v�֎~�t���O
    //private float LastAttackTime = -0.5f;//�O��U���������������

    //private float AttackFreezeTime = 0.5f;//�U������ňړ���W�����v���֎~���鎞��

    //private bool runFlag = false; //�A�j���[�V�����g���K�[�����x�������Ȃ��悤�ɂ��邽�߂̃t���O
    //private bool idleFlag = true; //�A�j���[�V�����g���K�[�����x�������Ȃ��悤�ɂ��邽�߂̃t���O
    private bool buttonFlag_Skill1 = false; //�A������֎~�t���O
    private bool autoRunFlag = false; //�I�[�g����

    static bool isSliding = false;

    public float DirectionFollowSpeed = 1200.0f;   // �ړ������ɑ΂��ăL�����N�^�[�̌������ǐ�����Ƃ��̑��x

    private Vector3 moveDirection = Vector3.zero;   //  �ړ���������ƃx�N�g���̕ϐ��i�ŏ��͏��������Ă����j
    private Vector3 charDirection = Vector3.zero;   //  �L�����N�^�[�̌����i�ŏ��͏��������Ă����j
    private Vector3 moveDirection_Past = Vector3.zero;
    private Vector3 moveDirection_autoRun = Vector3.zero;


    //�J�����֘A
    public GameObject objCamera;

    //�^�[�Q�b�g�J�[�\��
    public GameObject objTargetCursor;

    //UI�A�f�o�b�O�֘A
    public GameObject objUI_Debug;
    private Text objText_Debug;
    public GameObject objUI_HP;
    private Text objText_HP;

    public float BlockIntervalX = 10f;
    public float BlockIntervalY = 2.5f;
    public float BlockIntervalZ = 10f;

    //����֎~�t���O�i�ʃX�N���v�g���瑀�삷��j
    public bool isControllEnabled = true;
    public bool isIdle = true;

    //�t�B�[���h�֘A
    public GameObject CurrentField;
    private GameObject[] FragmentPrefabs;

    private Vector3 WaitingPlacePos = new Vector3(0, 0, 1000f);

    //public GameObject CurrentWorld;
    public GameObject FragmentsListUI;
    public string CurrentMapName = "";

    //���[�h
    public string Mode = "Main";

    //���j���[��ʌQ
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

        //���n������
        Landing(objPlayer.gameObject);

        ////�J���������ʒu
        //initCamera();

        //��n�̂�����@Prefab�ꗗ�̂̓ǂݍ���
        FragmentPrefabs = Resources.LoadAll<GameObject>("FragmentPrefabs");







        //�e�X�g�f�[�^
        LoadMap("FirstVillage");



        //LoadFieldParts("World_001", "Road2", 0, 0, 0, 1);
        //LoadFieldParts("World_001", "Road2", 1, 0, 0, 3);
        //LoadFieldParts("World_001", "Green", 0, 0, 1, 0);
        //LoadFieldParts("World_001", "Road2", 1, 0, 1, 1);
        //LoadFieldParts("World_001", "RiverBridge", 0, 0, -1, 1);


        //CurrentWorld = GameObject.Find("World_001");

        //�X�e�[�^�X�ݒ�
        PlayerStatus.IsPlayer = true;
        PlayerStatus.HP_Max_Base =256;
        PlayerStatus.Offence_Base  = 10;
        PlayerStatus.Defence_Base = 5;
        RecalcStatus(ref PlayerStatus);
        PlayerStatus.HP = PlayerStatus.HP_Max_Calced;
        HpBar.GetComponent<HpBar>().ResetBar(PlayerStatus.HP, PlayerStatus.HP_Max_Calced);

        //FieldPartsSelect��ʂ͂��������\��
        objWindow_FieldPartsSelect.SetActive(false);

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
        objText_Debug.text = "";




        //---------------------------------------------------------------------------------------------------------------------------------------
        // �ړ�����(WASD,���X�e�B�b�N�j�ɉ����āA�ړ������{��(moveDirection�j��ݒ�
        //---------------------------------------------------------------------------------------------------------------------------------------
        moveDirection_Past = moveDirection;
        var cameraForward = Vector3.Scale(objCamera.transform.forward, new Vector3(1, 0, 1)).normalized;  //  �J�����Ɍ������Đ����̒P�ʃx�N�g�������߂�B
        if (isSliding == false && objCharController.isGrounded && isControllEnabled == true)
        {
            if (Mode == "Main")
            {

                moveDirection = walkSpeed * cameraForward * -Input.GetAxis("LstickUD"); //�J�����̑O��ɑ΂��ď㉺�L�[�̓��͐��������Z
                moveDirection += walkSpeed * objCamera.transform.right * Input.GetAxis("LstickLR");//�J�������璼�p�̕����ɑ΂��č��E�̓��͐��������Z�i�����A�J�������X���Ă��Ȃ����Ƃ��O��j
            }
            else
            {
                moveDirection = new Vector3(0, 0, 0);
                //moveDirection = walkSpeed * cameraForward * 0; //�J�����̑O��ɑ΂��ď㉺�L�[�̓��͐��������Z
                //moveDirection += walkSpeed * objCamera.transform.right *0;//�J�������璼�p�̕����ɑ΂��č��E�̓��͐��������Z�i�����A�J�������X���Ă��Ȃ����Ƃ��O��j

            }

        }
        moveDirection.y = moveDirection_Past.y;

        //---------------------------------------------------------------------------------------------------------------------------------------
        //�ړ�����(WASD,���X�e�B�b�N�j������Ƃ��́A�I�u�W�F�N�g�̕�����ς���
        //---------------------------------------------------------------------------------------------------------------------------------------
        if ((Input.GetAxis("LstickLR") != 0 || Input.GetAxis("LstickUD") != 0) && Mode == "Main" && isControllEnabled == true)
        {
            objPlayer.transform.rotation = Quaternion.LookRotation(moveDirection);
            //�����Ɍ����鏈���������ɓ���ĔY�񂾂��ALookRotation�ɂ�����A�s�v�ɂȂ����B

            if (objCharController.isGrounded)
            {
                //�U�����̓A�j���[�V������J�ڂ����Ȃ�
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

                    //�O��U����������Ă����莞�Ԃ̓W�����v�֎~
                    if (PlayerStatus.isAttack == false)
                    {
                        //�W�����v���s
                        moveDirection.y = jumpSpeed;

                        //�A�j���[�V�����؂�ւ�
                        objAnimator.SetTrigger("jump");

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
                    if (objCharController.isGrounded)
                    {
                        //Debug.Log("�U��");
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
        //�⓹�̂Ƃ����点�鏈��
        //---------------------------------------------------------------------------------------------------------------------------------------
        //http://hideapp.cocolog-nifty.com/blog/2015/03/unity-tips-char.html
        Vector3 startVec = objPlayerAppearance.transform.position + objPlayer.transform.forward * 0f;
        Vector3 endVec = objPlayerAppearance.transform.position + objPlayer.transform.forward * 0f;
        startVec.y = 9999;
        endVec.y = -9999;
        RaycastHit slideHit;
        if (Physics.Linecast(startVec, endVec, out slideHit))
        {
            //�Փ˂����ۂ̖ʂ̊p�x�����点�����p�x�ȏォ�ǂ���
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

        //����t���O�������Ă��āA�����n���Ă���Ƃ��A���点��
        if (isSliding && objCharController.isGrounded)
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
            PlayerStatus.KnockBackDirection *= 0.98f;//���񂾂�������



        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        //�t�B�[���h�p�[�c�̊O�ɂ͗����Ȃ��悤�ɂ���`�F�b�N
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
            //�v���C���[��objFieldCursor���̋������ꂽ��AobjFieldCursor�������i�����̏ꏊ�Ɉړ��j
            //2021.5.28 FieldPats��Set����Ƃ��ɋ}�ɕςȂƂ���Ɉړ�����o�O�̑Ή��̂��߁A������ 7f -> 9f �ɕύX�B
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
        //�t�B�[���h�p�[�c�Z�b�g
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
        //objPlayer�ɍ��킹�āAobjPlayerAppearance��ǐ�������
        //---------------------------------------------------------------------------------------------------------------------------------------
        Vector3 tmpVector3 = objPlayerAppearance.transform.position;
        tmpVector3.x = objPlayer.transform.position.x;
        tmpVector3.y = objPlayer.transform.position.y - 0.5f;
        tmpVector3.z = objPlayer.transform.position.z;
        objPlayerAppearance.transform.position = tmpVector3;


        //  �ړ��L�[�̓��͂�����Ƃ��ɁA�������ǐ�������
        if (Input.GetAxis("LstickUD") != 0 || Input.GetAxis("LstickLR") != 0)
        {
            objPlayerAppearance.transform.rotation = Quaternion.RotateTowards(objPlayerAppearance.transform.rotation, objPlayer.transform.rotation, DirectionFollowSpeed * Time.deltaTime);   // ������ q �Ɍ����Ă���`���ƕω�������.
        }

        //�����ɂ���@objPlayerAppearance�������ƕςȓ����BobjPlayer��Collider���X���ƕςȓ����ɂȂ�̂ŁAobjPlayer�������ɂ���悤�ǉ� 20210511
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
        //UI�\��
        //---------------------------------------------------------------------------------------------------------------------------------------

        // HP
        objText_HP.text = "";
        objText_HP.text = "HP:" + PlayerStatus.HP.ToString() + "/" + PlayerStatus.HP_Max_Calced.ToString();
        HpBar.GetComponent<HpBar>().SetValue(PlayerStatus.HP);

        //�f�o�b�O
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
    // �I�u�W�F�N�g�𒅒n������
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
    public int LoadFieldParts(string pCollectionName,string pFieldPatrsName, int pBlockX, int pBlockY, int pBlockZ, int pRotate)
    {
        //�z���̃q�G�����L�[��T���āA�Ȃ��ꍇ�͐V�K�쐬
        GameObject objCollection = GameObject.Find(pCollectionName);
        if (objCollection is null)
        {
            objCollection = new GameObject(pCollectionName);
            objCollection.transform.position = new Vector3(0, 0, 0);
            objCollection.transform.parent = CurrentField.transform;

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

        //prefab�̌Ăяo��
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

                //�X�g�b�N�̂Ƃ��́A�Ԙg��t�^���āA�ҋ@�ꏊ�Ɉړ�������
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

