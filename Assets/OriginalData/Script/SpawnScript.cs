using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnScript : MonoBehaviour
{
    public string EnemyName;
    public float SpawnInterval;
    public float SpawnDistance;

    private GameObject SpawnedEnemy;
    private float SpawnIntervalCounter;
    private GameObject PlayerObject;

    private GameObject[] EnemyPrefab;
    public bool SpawnEnebled = true;//一度範囲外に出ないと再ポップしないようにするためのフラグ

    // Start is called before the first frame update
    void Start()
    {
        SpawnIntervalCounter = 0;
        PlayerObject = GameObject.Find("Player");

        //敵のプレファブの読み込み
        EnemyPrefab = Resources.LoadAll<GameObject>("Enemies");

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

}
