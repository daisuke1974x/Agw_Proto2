using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBarEnemy : MonoBehaviour
{
    public GameObject EnemyObject;
    public GameObject MainCamera;
    public GameObject ValueBar;

    public int MaxHp;
    public int Hp;


    // Start is called before the first frame update
    void Start()
    {
        MainCamera = GameObject.Find("MainCamera");
    }

    // Update is called once per frame
    void Update()
    {

        if (EnemyObject == null)
        {
            Destroy(this.gameObject);
        }
        else
        {

            Hp = EnemyObject.GetComponent<CharStatus>().HP;
            MaxHp = EnemyObject.GetComponent<CharStatus>().HP_Max_Calced;
            if (Hp == MaxHp)
            {
                Vector3 Pos = new Vector3(0,-1000,0);
                this.transform.position = Pos;
            }
            else
            {
                Vector3 Pos = RectTransformUtility.WorldToScreenPoint(MainCamera.GetComponent<Camera>(), EnemyObject.transform.position);
                this.transform.position = Pos;

                Vector2 size = ValueBar.GetComponent<RectTransform>().sizeDelta;
                size.x = (int)(128f * (float)Hp / (float)MaxHp);
                ValueBar.GetComponent<RectTransform>().sizeDelta = size;

            }

        }
    }
}
