using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class s_UI : MonoBehaviour
{

    private float MoneyCountTimer;
    private int Money_View;
    public GameObject objPlayer;

    public GameObject objUI_Money;
    private Text objText_Money;


    // Start is called before the first frame update
    void Start()
    {
        MoneyCountTimer = 0;
        Money_View = objPlayer.GetComponent<s_CharStatus>().Money;
        objText_Money = objUI_Money.GetComponent<Text>();

    }

    // Update is called once per frame
    void Update()
    {

        //Š‹à‚Ì‘Œ¸ˆ—
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

        //Š‹à‚Ì•\¦
        objText_Money.text = "Money:" + Money_View.ToString();
    }
}
