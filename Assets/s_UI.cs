using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class s_UI : MonoBehaviour
{

    private float MoneyCountTimer;
    private int Money_View;
    public GameObject objPlayer;

    public GameObject UiMoneyUpper;
    public GameObject UiMoneyLower;
    private Text TextMoneyUpper;
    private Text TextMoneyLower;


    // Start is called before the first frame update
    void Start()
    {
        MoneyCountTimer = 0;
        Money_View = objPlayer.GetComponent<s_CharStatus>().Money;
        TextMoneyUpper = UiMoneyUpper.GetComponent<Text>();
        TextMoneyLower = UiMoneyLower.GetComponent<Text>();

    }

    // Update is called once per frame
    void Update()
    {

        //所持金の増減処理
        if (MoneyCountTimer + 0.02f < Time.time)
        {
            MoneyCountTimer = Time.time;
            int Money = objPlayer.GetComponent<s_CharStatus>().Money;
            if (Money < Money_View)
            {
                Money_View--;
            }
            if (Money > Money_View)
            {
                Money_View++;
            }
        }

        //所持金の表示
        TextMoneyUpper.text = Money_View.ToString();
        TextMoneyLower.text = Money_View.ToString();
    }
}
