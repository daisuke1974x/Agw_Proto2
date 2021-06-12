using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_EnemyControl : MonoBehaviour
{

    //敵キャラ関連
    private GameObject[] EnemyPrefab;

    private float SpawnCounter = 0;
    private float NextSpawn = 0;
    public int SpawnCount = 0;

    public GameObject objPlayer;

    // Start is called before the first frame update
    void Start()
    {
        //敵のプレファブの読み込み
        EnemyPrefab = Resources.LoadAll<GameObject>("Enemies");

        SpawnCounter = 9999;

    }

    // Update is called once per frame
    void Update()
    {
        

        SpawnCounter += Time.deltaTime;
        if (SpawnCounter > NextSpawn)
        {
            if (SpawnCount < 5)
            {
                GameObject obj = new GameObject();
                Vector3 pos = objPlayer.transform.position;
                pos.x += Random.Range(5f, 10f);
                float rot = Random.Range(0f, 360f);
                obj.transform.position = pos;
                obj.transform.RotateAround(objPlayer.transform.position, Vector3.up, rot);
                pos = obj.transform.position;
                Destroy(obj);
                GameObject obj2 = Spawn("TurtleShellPBR", pos);
                if (!(obj2 is null))
                {
                    SpawnCount++;
                }


            }

            SpawnCounter = 0;
            NextSpawn = Random.Range(5f, 10f);
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
                GameObject objInstance = Instantiate(objI,pPos,new Quaternion(), this.transform);
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
