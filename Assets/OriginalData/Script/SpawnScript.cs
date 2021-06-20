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
    public bool SpawnEnebled = true;//��x�͈͊O�ɏo�Ȃ��ƍă|�b�v���Ȃ��悤�ɂ��邽�߂̃t���O

    // Start is called before the first frame update
    void Start()
    {
        SpawnIntervalCounter = 0;
        PlayerObject = GameObject.Find("Player");

        //�G�̃v���t�@�u�̓ǂݍ���
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
    // �G�L�������o��������
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
                    //���n�\�ȏꏊ�̂Ƃ��́A���n������
                    objInstance.transform.position = hit.point;
                    return objInstance;
                }
                else
                {
                    //���n�s�\�ȏꏊ�̂Ƃ��́A�폜
                    Destroy(objInstance);
                    return null;
                }
                return objInstance;

            }
        }
        return null;
    }

}
