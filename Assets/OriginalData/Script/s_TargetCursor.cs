using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_TargetCursor : MonoBehaviour
{
    public bool isFoundTarget;
    public GameObject objTarget;
    public GameObject objPlayer;
    public GameObject Hierarchy_TargetObject;
    public GameObject[] objTargetList;

    // Start is called before the first frame update
    void Start()
    {
        


    }

    // Update is called once per frame
    void Update()
    {
        //��]������
        Quaternion Q = this.transform.rotation;
        Vector3 rot = Q.eulerAngles;
        rot.y += 240f * Time.deltaTime;
        if (rot.y > 360) {
            rot.y = 0;
        }
        Q = Quaternion.Euler(rot);
        this.transform.rotation = Q;



        //�^�[�Q�b�g�Ώۂ̃I�u�W�F�N�g���擾
        objTarget = null;
        objTargetList = new GameObject[Hierarchy_TargetObject.transform.childCount];
        float dist1;
        float dist2;
        float angle=0;
        
        for (int i = 0; i < Hierarchy_TargetObject.transform.childCount; i++)
        {
            //�����ΏۃI�u�W�F�N�g
            GameObject tmpTarget = Hierarchy_TargetObject.transform.GetChild(i).gameObject;


            //�㉺�̍����`�F�b�N
            if (Mathf.Abs(tmpTarget.transform.position.y - objPlayer.transform.position.y) < 2f)
            {
                //�O����`�͈͓̔��`�F�b�N
                var forward = objPlayer.transform.TransformDirection(Vector3.forward);
                forward.y = 0;
                var targetDirection = tmpTarget.transform.position - objPlayer.transform.position;
                targetDirection.y = 0f;
                angle = Vector3.Angle(targetDirection, forward);

                if (angle < 90f)
                {
                    //�����`�F�b�N
                    dist1 = Vector3.Distance(objPlayer.transform.position, tmpTarget.transform.position);
                    if (objTarget is null)
                    {
                        dist2 = 9999f;
                    }
                    else
                    {
                        dist2 = Vector3.Distance(objPlayer.transform.position, objTarget.transform.position);
                    }

                    //�Q���ȓ�
                    if (dist1 < 2f)
                    {
                        if (dist1 < dist2)
                        {
                            objTarget = tmpTarget;
                        }

                    }
                }

            }



        }

        if (objTarget is null)
        {
            Vector3 pos = this.transform.position;
            pos.y = -9999;
            this.transform.position = pos;

        }
        else
        {
            //Debug.Log(angle);

            this.transform.position = objTarget.transform.position;
            Vector3 pos = this.transform.position;
            pos.y += 2;
            this.transform.position = pos;
        }


    }
}
