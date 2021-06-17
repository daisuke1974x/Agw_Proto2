using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
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
        Money_View = objPlayer.GetComponent<CharStatus>().Money;
        TextMoneyUpper = UiMoneyUpper.GetComponent<Text>();
        TextMoneyLower = UiMoneyLower.GetComponent<Text>();

    }

    // Update is called once per frame
    void Update()
    {

        //Š‹à‚Ì‘Œ¸ˆ—
        if (MoneyCountTimer + 0.02f < Time.time)
        {
            MoneyCountTimer = Time.time;
            int Money = objPlayer.GetComponent<CharStatus>().Money;
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
        TextMoneyUpper.text = Money_View.ToString();
        TextMoneyLower.text = Money_View.ToString();
    }

    public float GetScreenMagnificationRate()
    {
        //‰æ–Ê‰ğ‘œ“x‚É‰‚¶‚Ä‘å‚«‚³‚ğ•Ï‚¦‚é
        if (Screen.height <= 100)
        {
            return 0.1f;
        }
        if (100 < Screen.height && Screen.height <= 600)
        {
            return 0.5f;
        }
        if (600 < Screen.height && Screen.height <= 1400)
        {
            return 1;
        }
    
        if (1400 < Screen.height && Screen.height <= 3000)
        {
            return 2;
        }
        return 3;
    }

}
