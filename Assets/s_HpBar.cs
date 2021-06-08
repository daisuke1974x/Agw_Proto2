using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_HpBar : MonoBehaviour
{
    private GameObject BlackBg;
    private GameObject ValueBar;
    private GameObject WhiteFrame;

    private static int UnitValue = 64;

    private float UnitWidth ;
    private float UnitX ;
    private float UnitY ;

    public int MaxHp = UnitValue * 3;
    public int Hp = UnitValue * 3;

    public int InnerMaxHp;
    public int InnerHp;

    private string InstanceName = "WhiteFrameInstance";

    private int Units;

    // Start is called before the first frame update
    void Start()
    {
        BlackBg = GameObject.Find("BlackBg");
        ValueBar = GameObject.Find("ValueBar");
        WhiteFrame = GameObject.Find("WhiteFrame");
        UnitWidth = WhiteFrame.GetComponent<RectTransform>().sizeDelta.x;
        UnitX = WhiteFrame.transform.position.x;
        UnitY = WhiteFrame.transform.position.y;

        ResetBar(Hp, MaxHp);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ResetBar(int pHp, int pMaxHp)
    {
        Units = (int)Mathf.Floor(pMaxHp / UnitValue);
        InnerMaxHp = UnitValue * Units;
        if (InnerHp > InnerMaxHp) InnerHp = InnerMaxHp;

        for (int Index = 0; Index < this.transform.GetChildCount(); Index++)
        {
            if (this.transform.GetChild(Index).name == InstanceName)
            {
                Destroy(this.transform.GetChild(Index));
            }
        }

        for (int Index = 1; Index < Units; Index++)
        {
            GameObject Instance = Instantiate(WhiteFrame.gameObject, this.transform, false);
            Instance.name = InstanceName;
            Vector3 pos = Instance.transform.position;
            pos.x = UnitX + Index * UnitWidth;
            pos.y = UnitY;
            Instance.transform.position = pos;
        }

        Vector2 size = BlackBg.GetComponent<RectTransform>().sizeDelta;
        size.x = Units * UnitWidth;
        BlackBg.GetComponent<RectTransform>().sizeDelta = size;


        SetValue(96);

    }

    void SetValue(int pHp)
    {
        Hp = pHp;
        if (MaxHp<Hp) Hp = MaxHp;
        if (Hp<0 ) Hp = 0;

        Vector2 size =  ValueBar.GetComponent<RectTransform>().sizeDelta;
        size.x = (int)Mathf.Floor(((float)Hp / (float)MaxHp) * (float)Units * (float)UnitWidth); 
        ValueBar.GetComponent<RectTransform>().sizeDelta = size;
    }
    


    

}
