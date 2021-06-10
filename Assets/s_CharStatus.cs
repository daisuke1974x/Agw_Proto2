using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_CharStatus : MonoBehaviour
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
    private GameObject[] objDropItemPrefab;
    public GameObject objDropItem;

    private GameObject objChestPrefab;
    private GameObject Hierarchy_TargetObject;

    //Avoidance回避


    // Start is called before the first frame update
    void Start()
    {


        objEnemies = GameObject.Find("Enemies");
        objDropItemPrefab = Resources.LoadAll<GameObject>("DropItems");
        objChestPrefab = objDropItemPrefab[1];
        Hierarchy_TargetObject= GameObject.Find("TargetObject");
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
                objInstance.transform.position = this.transform.position;
                Destroy(objInstance, 2f);

                DropControl();

                Destroy(this.GetComponent<s_Enemy>().HpBarEnemy.gameObject);

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


    private void DropControl()
    {
        //ドロップアイテム
        int Rnd = Random.Range(0, 100);
        if (Rnd < 1)
        {
            //ドロップなし

        }
        else
        {
            Rnd = Random.Range(0, 100);
            if (Rnd < 1)
            {
                Rnd = Random.Range(0, 100);
                if (Rnd < 20)
                {
                    DropMoney(0, objEnemies.transform.position);
                }
                else
                {
                    DropMoney(1, objEnemies.transform.position);

                }

            }
            else
            {
                DropChest(objEnemies.transform.position);

            }
        }


        //int rnd = Random.Range(3, 7);
        //DropMoney(rnd, objEnemies.transform.position);



    }


    //ドロップアイテム
    private void DropMoney(int pType,Vector3 pPos)
    {
        GameObject objInstanceItem = Instantiate(objDropItemPrefab[0], objEnemies.transform, false);
        Vector3 pos = this.transform.position;
        pos.y += 0.3f;
        objInstanceItem.transform.position = pos;

        Color col = objInstanceItem.GetComponent<Renderer>().material.color;
        int MoneyValue = 0;
        switch (pType)
        {
            case 0:
                MoneyValue = 1;
                col = new Color(0f, 1f, 0f, 1f);
                break;
            case 1:
                MoneyValue = 5;
                col = new Color(0f, 1f, 1f, 1f);
                break;
            case 2:
                MoneyValue = 10;
                col = new Color(1f, 1f, 0f, 1f);
                break;
            case 3:
                MoneyValue = 20;
                col = new Color(1f, 0f, 0f, 1f);
                break;
            case 4:
                MoneyValue = 50;
                col = new Color(1f, 0f, 1f, 1f);
                break;
            case 5:
                MoneyValue = 100;
                col = new Color(1f, 0.5f, 1f, 1f);
                break;
            case 6:
                MoneyValue = 200;
                col = new Color(1f, 1f, 1f, 1f);
                break;
        }
        objInstanceItem.GetComponent<s_DropItem>().MoneyValue = MoneyValue;
        objInstanceItem.GetComponent<Renderer>().material.color = col;

        Destroy(objInstanceItem, 15f);





    }


    private void DropChest(Vector3 pPos)
    {

        GameObject objChest = Instantiate( objChestPrefab, Hierarchy_TargetObject.transform, false);
        Vector3 pos = this.transform.position;
        objChest.transform.position = pos;

        GameObject objPlayer = GameObject.Find("Player");
        objChest.transform.LookAt(objPlayer.transform);

        Quaternion q  = objChest.transform.rotation;
        q.x = 0;
        q.z = 0;
        objChest.transform.rotation = q;

    }

}
