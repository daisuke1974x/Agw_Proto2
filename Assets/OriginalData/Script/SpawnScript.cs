using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnScript : MonoBehaviour
{
    public string EnemyName;
    public float SpawnInterval;
    public float SpawnDistance;

    public float[] DropRate;
    public enumItemType[] ItemType;
    public string[] ItemName;

    private GameObject SpawnedEnemy;
    private float SpawnIntervalCounter;
    private GameObject PlayerObject;

    private GameObject[] EnemyPrefab;
    public bool SpawnEnebled = true;//一度範囲外に出ないと再ポップしないようにするためのフラグ

    private GameObject[] objDropItemPrefab;
    //public GameObject objDropItem;

    private GameObject objChestPrefab;
    private GameObject Hierarchy_TargetObject;

    //public struct stDropItem
    //{
    //    float DropRate;
    //    enumItemType ItemType;
    //    string ItemName;
    //}

    public enum enumItemType
    {
        Fragment,
        Money,
        Item

    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnIntervalCounter = 0;
        PlayerObject = GameObject.Find("Player");

        //敵のプレファブの読み込み
        EnemyPrefab = Resources.LoadAll<GameObject>("Enemies");

        //ドロップアイテムのプレファブの読み込み
        objDropItemPrefab = Resources.LoadAll<GameObject>("DropItems");
        objChestPrefab = objDropItemPrefab[1];
        Hierarchy_TargetObject = GameObject.Find("CurrentNpc");
    }

    // Update is called once per frame
    void Update()
    {
        if (SpawnedEnemy == null)
        {
            SpawnIntervalCounter -= Time.deltaTime;
            if (SpawnIntervalCounter < 0)
            {
                float dist = Vector3.Distance(PlayerObject.transform.position, this.transform.position);
                if (dist < SpawnDistance)
                {
                    if (SpawnEnebled == true)
                    {
                        SpawnedEnemy = Spawn(EnemyName, this.transform.position);
                        SpawnEnebled = false;
                        SpawnIntervalCounter = SpawnInterval;
                    }
                }
                else
                {
                    SpawnEnebled = true;
                }


            }
        }
        else
        {
            float dist = Vector3.Distance(PlayerObject.transform.position, SpawnedEnemy.transform.position);
            if (dist > 20f)
            {
                Destroy(SpawnedEnemy);
                SpawnEnebled = false;
            }

        }


    }

    //*******************************************************************************************************************************************
    // 敵キャラを出現させる
    //*******************************************************************************************************************************************
    public GameObject Spawn(string pEnemyName, Vector3 pPos)
    {
        foreach (var objI in EnemyPrefab)
        {
            if (objI.name == pEnemyName)
            {
                GameObject objInstance = Instantiate(objI, pPos, new Quaternion(), this.transform);
                //objInstance.transform.position = pPos;

                Vector3 startVec = objInstance.transform.position;
                Vector3 endVec = objInstance.transform.position;
                startVec.y = 9999;
                endVec.y = -9999;
                //if (Physics.Linecast(startVec, endVec, out hit))
                RaycastHit hit;
                if (Physics.Raycast(objInstance.transform.position, Vector3.down, out hit))
                {
                    //着地可能な場所のときは、着地させる
                    objInstance.transform.position = hit.point;
                    return objInstance;
                }
                else
                {
                    //着地不可能な場所のときは、削除
                    Destroy(objInstance);
                    return null;
                }
                return objInstance;

            }
        }
        return null;
    }

    //*******************************************************************************************************************************************
    // ドロップコントロール
    //*******************************************************************************************************************************************
    public void DropControl()
    {

        //GameObject SpawnObject = this.transform.parent.gameObject;
        //SpawnScript SpawnScriptObject = SpawnObject.GetComponent<SpawnScript>();

        if (DropRate.Length == 0) return;

        float TotalRate = 0;
        float Lot = Random.Range(0f, 1f);
        int GetIndex = -1;

        //DropRateに設定された確率を順次チェックして、当選したかどうかチェック
        for (int Index = 0; Index < DropRate.Length; Index++)
        {
            TotalRate += DropRate[Index];
            if ((Lot <= TotalRate) && (GetIndex == -1))
            {
                GetIndex = Index;
            }
        }
        if (GetIndex == -1) return;

        switch (ItemType[GetIndex])
        {
            case enumItemType.Money:
                DropMoney(ItemName[GetIndex], SpawnedEnemy.transform.position);
                break;




        }


        //if (GetIndex == 0) DropMoney(1, SpawnedEnemy.transform.position);
        //if (GetIndex == 1) DropMoney(5, SpawnedEnemy.transform.position);
        return;


        //ドロップアイテム
        int Rnd = Random.Range(0, 100);
        if (0 <= Rnd && Rnd < 0)
        {
            //ドロップなし

        }
        if (0 <= Rnd && Rnd < 100)
        {
            int Rnd2 = Random.Range(0, 100);
            if (0 <= Rnd2 && Rnd2 < 0)
            {
                DropMoney("1", SpawnedEnemy.transform.position);
            }
            if (30 <= Rnd2 && Rnd2 < 0)
            {
                DropMoney("10", SpawnedEnemy.transform.position);
            }
            if (0 <= Rnd2 && Rnd2 < 100)
            {
                DropChest(SpawnedEnemy.transform.position);
            }
            if (00 <= Rnd2 && Rnd2 < 0)
            {
                DropPotion(SpawnedEnemy.transform.position);
            }
        }


        //int rnd = Random.Range(3, 7);
        //DropMoney(rnd, objEnemies.transform.position);



    }


    //*******************************************************************************************************************************************
    // お金をドロップさせる
    //*******************************************************************************************************************************************
    private void DropMoney(string pMoneyValue, Vector3 pPos)
    {
        GameObject objInstanceItem = Instantiate(objDropItemPrefab[0], this.transform.parent, false);
        objInstanceItem.name = objDropItemPrefab[0].name;

        Vector3 pos = this.transform.position;
        pos.y += 0.3f;
        objInstanceItem.transform.position = pos;

        Color col = objInstanceItem.GetComponent<Renderer>().material.color;
        int MoneyValue = 0;
        switch (pMoneyValue)
        {
            case "1":
                MoneyValue = 1;
                col = new Color(0f, 1f, 0f, 1f);
                break;
            case "5":
                MoneyValue = 5;
                col = new Color(0f, 1f, 1f, 1f);
                break;
            case "10":
                MoneyValue = 10;
                col = new Color(1f, 1f, 0f, 1f);
                break;
            case "20":
                MoneyValue = 20;
                col = new Color(1f, 0f, 0f, 1f);
                break;
            case "50":
                MoneyValue = 50;
                col = new Color(1f, 0f, 1f, 1f);
                break;
            case "100":
                MoneyValue = 100;
                col = new Color(1f, 0.5f, 1f, 1f);
                break;
            case "200":
                MoneyValue = 200;
                col = new Color(1f, 1f, 1f, 1f);
                break;
        }

        if (MoneyValue != 0)
        {
            objInstanceItem.GetComponent<DropItem>().MoneyValue = MoneyValue;
            objInstanceItem.GetComponent<Renderer>().material.color = col;

            Destroy(objInstanceItem, 15f);

        }
        else
        {
            Debug.Log("DropMoney : pMonetValueError");
        }





    }
    //*******************************************************************************************************************************************
    // ポーションをドロップさせる
    //*******************************************************************************************************************************************
    private void DropPotion(Vector3 pPos)
    {
        GameObject objInstanceItem = Instantiate(objDropItemPrefab[2], SpawnedEnemy.transform, false);
        Vector3 pos = this.transform.position;
        pos.y += 0.3f;
        objInstanceItem.transform.position = pos;
        Destroy(objInstanceItem, 15f);
    }

    //*******************************************************************************************************************************************
    // 宝箱をドロップさせる
    //*******************************************************************************************************************************************
    private void DropChest(Vector3 pPos)
    {

        GameObject objChest = Instantiate(objChestPrefab, Hierarchy_TargetObject.transform, false);
        Vector3 pos = this.transform.position;
        objChest.transform.position = pos;

        GameObject objPlayer = GameObject.Find("Player");
        objChest.transform.LookAt(objPlayer.transform);

        Quaternion q = objChest.transform.rotation;
        q.x = 0;
        q.z = 0;
        objChest.transform.rotation = q;

    }

}
