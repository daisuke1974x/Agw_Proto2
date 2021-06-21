using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharStatus : MonoBehaviour
{
    public string Name;
    public bool IsPlayer;
    public int HP;
    public int HP_Max_Base;
    public int HP_Max_Calced;
    public int Offence_Base;
    public int Offence_Calced;
    public int Defence_Base;
    public int Defence_Calced;
    public float LastDamagedTime;
    public int Money;

    public bool isAttack = false;
    public float AttackEndTime = 0;
    public bool isKnockBack = false;
    public float KnockBackEndTime = 0;
    public Vector3 KnockBackDirection;
    public bool isDie = false;
    public float DeadEndTime = 0;

    public GameObject objEffect_Disappearance;
    public GameObject objEnemies;


    //敵キャラ関連
    //private GameObject[] objDropItemPrefab;
    //public GameObject objDropItem;

    //private GameObject objChestPrefab;
    //private GameObject Hierarchy_TargetObject;

    //Avoidance回避


    // Start is called before the first frame update
    void Start()
    {


        objEnemies = GameObject.Find("Enemies");
        //objDropItemPrefab = Resources.LoadAll<GameObject>("DropItems");
        //objChestPrefab = objDropItemPrefab[1];
        //Hierarchy_TargetObject= GameObject.Find("CurrentNpc");
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttack)
        {
            if (AttackEndTime < Time.time)
            {
                AttackEndTime = 0;
                isAttack = false;
            }
        }

        if (isKnockBack)
        {
            if (KnockBackEndTime < Time.time)
            {
                KnockBackEndTime = 0;
                isKnockBack = false;
            }
        }

        if (isDie)
        {
            if ((DeadEndTime - 0.75f < Time.time) && (Time.time < DeadEndTime))
            {
                //Color col = this.gameObject.GetComponent<Renderer>().material.color;
                //col.a = (DeadEndTime - Time.time);
                //col.a = 0.5f;
                //this.gameObject.GetComponent<Renderer>().material.color = col;
                //Debug.Log(col);

            }


            if (DeadEndTime < Time.time)
            {
                //エフェクト
                GameObject objInstance = Instantiate(objEffect_Disappearance, objEnemies.transform, false);
                objInstance.name = objEffect_Disappearance.name;
                objInstance.transform.position = this.transform.position;
                Destroy(objInstance, 2f);

                this.transform.parent.GetComponent<SpawnScript>().DropControl();

                Destroy(this.GetComponent<Enemy>().HpBarEnemy.gameObject);

                Destroy(this.gameObject);

            }
        }
    }

    public int startAttack(float pTimeValue)
    {
        if (isAttack) return -1;

        AttackEndTime = Time.time + pTimeValue;
        isAttack = true;
        return 0;

    }

    public int startKnockBack(float pTimeValue,Vector3 pKnockBackDirection)
    {
        if (isKnockBack) return -1;

        KnockBackEndTime = Time.time + pTimeValue;
        isKnockBack = true;
        KnockBackDirection = pKnockBackDirection;
        return 0;
    }

    public int startDie(float pTimeValue)
    {
        if (isDie) return -1;

        DeadEndTime = Time.time + pTimeValue;
        isDie = true;
        return 0;
    }



}
