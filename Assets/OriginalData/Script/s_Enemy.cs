using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_Enemy : MonoBehaviour
{

    private GameObject objPlayer;
    private GameObject MainControl;
    private Animator objAnimator;
    private CharacterController objCharController;
    //private MainControl MainControl;

    private s_CharStatus EnemyStatus;
    private s_CharStatus PlayerStatus;

    public GameObject HpBarEnemyFrefab;
    public GameObject HpBarEnemy;
    public GameObject HpBarEnemyUI;

    private float gravity = 20.0f;  //�d�͂̑傫��

    // Start is called before the first frame update
    void Start()
    {
        objPlayer = GameObject.Find("Player");
        MainControl = GameObject.Find("MainControl");
        //MainControl = objMain.GetComponent<MainControl>();
        PlayerStatus = objPlayer.GetComponent<s_CharStatus>();
        EnemyStatus = this.GetComponent<s_CharStatus>();

        objCharController = this.GetComponent<CharacterController>();
        objAnimator = this.GetComponent<Animator>();

        EnemyStatus.IsPlayer = false;
        EnemyStatus.HP_Max_Base = 30;//20�łR��U�����炢
        EnemyStatus.Offence_Base = 10;
        EnemyStatus.Defence_Base = 5;
        MainControl.GetComponent<MainControl>().RecalcStatus(ref EnemyStatus);
        EnemyStatus.HP = EnemyStatus.HP_Max_Calced;
        HpBarEnemy = Instantiate(HpBarEnemyFrefab,GameObject.Find("HpBarEnemyUI").transform);
        HpBarEnemy.GetComponent<s_HpBarEnemy>().EnemyObject = this.gameObject;


    }

    // Update is called once per frame
    void Update()
    {
        
        //if (objScriptMain.Mode != "Main")
        //{
        //    this.GetComponent<Animation>().Stop();
        //    return;
        //}
        //this.GetComponent<Animation>().Play();

        float dist = Vector3.Distance(this.transform.position, objPlayer.transform.position);

        if (EnemyStatus.isDie == false)
        {

            if (EnemyStatus.isKnockBack == false)
            {
                if (dist < 8f)
                {
                    if (dist < 2f)
                    {
                        if (EnemyStatus.isAttack == false)
                        {
                            objAnimator.SetTrigger("Attack");
                            EnemyStatus.startAttack(5f);
                            MainControl.GetComponent<MainControl>().PhyzicalAttack(ref EnemyStatus, ref PlayerStatus, 0, 10);
                        }
                    }
                    else
                    {

                        objAnimator.SetBool("isIdle", false);

                        //Y���Œ�Ńv���C���[�̕���������
                        Vector3 direction = (objPlayer.transform.position - this.transform.position);
                        direction.y = 0;
                        var lookRotation = Quaternion.LookRotation(direction, Vector3.up);
                        this.transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, 0.1f);

                        //�ēx�������Z�o���A�P�ʃx�N�g���ɕϊ��A�ړ�����
                        direction = (objPlayer.transform.position - this.transform.position).normalized;
                        direction.y -= gravity * Time.deltaTime;
                        objCharController.Move(direction * Time.deltaTime * 1f);
                        //Landing(EnemyStatus.gameObject);
                    }

                }
                else
                {
                    if (dist > 30f)
                    {
                        GameObject.Find("Enemies") .GetComponent<s_EnemyControl>().SpawnCount--;
                        Destroy(HpBarEnemy.gameObject);
                        Destroy(this.gameObject);
                    }
                    else
                    {
                        objAnimator.SetBool("isIdle", true);

                    }

                }

            }
            else
            {
                Vector3 direction = EnemyStatus.KnockBackDirection;
                direction.y -= gravity * Time.deltaTime;
                objCharController.Move(direction * Time.deltaTime * 1f);
                EnemyStatus.KnockBackDirection *= 0.98f;//���񂾂�������
            }

        }
        if (EnemyStatus.HP == 0)
        {
            if (EnemyStatus.isDie == false)
            {
                objAnimator.SetTrigger("Die");
                EnemyStatus.startDie(1.0f);
                this.transform.parent.GetComponent<s_EnemyControl>().SpawnCount--;

            }
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        //�t�B�[���h�p�[�c�̊O�ɂ͗����Ȃ��悤�ɂ���`�F�b�N
        //---------------------------------------------------------------------------------------------------------------------------------------
        if (MainControl.GetComponent<MainControl>().CheckAbyss(this.gameObject))
        {
            if (EnemyStatus.isKnockBack) 
            { 
                Vector3 direction = EnemyStatus.KnockBackDirection;
                direction.y = 0;
                objCharController.Move(direction * -Time.deltaTime * 2);
            }
        }

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
}
