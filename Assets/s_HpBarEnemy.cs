using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_HpBarEnemy : MonoBehaviour
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

        if (EnemyObject is null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Vector3 Pos = RectTransformUtility.WorldToScreenPoint(MainCamera.GetComponent<Camera>(), EnemyObject.transform.position);
            this.transform.position = Pos;

            Hp = EnemyObject.GetComponent<s_CharStatus>().HP;
            MaxHp = EnemyObject.GetComponent<s_CharStatus>().HP_Max_Calced;
            Vector2 size = ValueBar.GetComponent<RectTransform>().sizeDelta;
            size.x = (int)(128f * (float)Hp / (float)MaxHp);
            ValueBar.GetComponent<RectTransform>().sizeDelta = size;
     
        }
    }
}
