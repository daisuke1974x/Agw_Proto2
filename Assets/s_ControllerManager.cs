using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_ControllerManager : MonoBehaviour
{
    public string[] nameAxis;
    //public string[] nameButton;
    public string[] nameDS4;
    public string[] configController;

    public bool flgAction = false;



    // Start is called before the first frame update
    void Start()
    {


        nameAxis = new string[29];
        //nameButton = new string[20];

        for (int i = 1; i < 29; i++)
        {
            nameAxis[i] = "Axis " + i.ToString();
        }
        for (int i = 1; i < 29; i++)
        {
            nameAxis[i] = "Axis " + i.ToString();
        }
        //enum a = KeyCode.JoystickButton0;

        nameDS4 = new string[20];
        nameDS4[0] = "〇";
        nameDS4[1] = "×";
        nameDS4[2] = "□";
        nameDS4[3] = "△";
        nameDS4[4] = "R1";
        nameDS4[5] = "R2";
        nameDS4[6] = "R3";
        nameDS4[7] = "L1";
        nameDS4[8] = "L2";
        nameDS4[9] = "L3";
        nameDS4[10] = "OPTIONS";
        nameDS4[11] = "SHARE";
        nameDS4[12] = "HOME";
        nameDS4[13] = "TOUCH";
        nameDS4[14] = "STICK_X_V";
        nameDS4[15] = "STICK_X_H";
        nameDS4[16] = "STICK_R_V";
        nameDS4[17] = "STICK_R_H";
        nameDS4[18] = "STICK_L_V";
        nameDS4[19] = "STICK_L_H";

        configController = new string[20];
        loadDefaultSetting(1);

        //セーブデータから取ってくる処理を入れる
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void loadDefaultSetting(int Num)
    {
        switch (Num)
        {
            case 1:
                //初期値 (1)Win
                configController[0] = "btn2";
                configController[1] = "btn1";
                configController[2] = "btn0";
                configController[3] = "btn3";
                configController[4] = "btn5";
                configController[5] = "btn7";
                configController[6] = "btn11";
                configController[7] = "btn4";
                configController[8] = "btn6";
                configController[9] = "btn10";
                configController[10] = "btn9";
                configController[11] = "btn8";
                configController[12] = "btn12";
                configController[13] = "btn13";
                configController[14] = "axis9";
                configController[15] = "axis8";
                configController[16] = "axis7r";
                configController[17] = "axis4r";
                configController[18] = "axis3";
                configController[19] = "axis1";
                break;
            case 2:
                //初期値 (2)Mac
                configController[0] = "btn2";
                configController[1] = "btn1";
                configController[2] = "btn0";
                configController[3] = "btn3";
                configController[4] = "btn5";
                configController[5] = "btn7";
                configController[6] = "btn11";
                configController[7] = "btn4";
                configController[8] = "btn6";
                configController[9] = "btn10";
                configController[10] = "btn9";
                configController[11] = "btn8";
                configController[12] = "btn12";
                configController[13] = "btn13";
                configController[14] = "axis12";
                configController[15] = "axis11";
                configController[16] = "axis4r";
                configController[17] = "axis3r";
                configController[18] = "axis2";
                configController[19] = "axis1";
                break;
        }
    }


    public float getAxisState(string pnameDS4)
    {

        if (nameDS4.Length < 2) { return 0; }
        int i;
        for (i = 14; i < 20; i++)
        {
            //Debug.Log("getAxisStateDS4:"+i+ ", nameDS4:"+ nameDS4+ ", nameDS4.Length:"+ nameDS4.Length);
            if (nameDS4[i].Equals(pnameDS4))
            {
                float value = getAxisStateFromController(configController[i]);
                if (value != 0f) { return value; }
            }
        }

        //仮のコード。キーコードは変数で設定できるようにする予定
        switch (pnameDS4)
        {
            //日本語キーボードについては、こちらを参照した。
            // http://fantom1x.blog130.fc2.com/blog-entry-326.html?sp

            case "STICK_X_V":
                if (Input.GetKey(KeyCode.UpArrow)) { return -1; }
                if (Input.GetKey(KeyCode.DownArrow)) { return 1; }
                break;
            case "STICK_X_H":
                if (Input.GetKey(KeyCode.LeftArrow)) { return -1; }
                if (Input.GetKey(KeyCode.RightArrow)) { return 1; }
                break;
            case "STICK_R_V":
                if (Input.GetKey(KeyCode.Semicolon)) { return -1; }//:
                if (Input.GetKey(KeyCode.BackQuote)) { return 1; }//@
                break;
            case "STICK_R_H":
                if (Input.GetKey(KeyCode.Equals)) { return -1; }//;
                if (Input.GetKey(KeyCode.RightBracket)) { return 1; }
                break;
            case "STICK_L_V":
                if (Input.GetKey(KeyCode.W)) { return -1; }
                if (Input.GetKey(KeyCode.S)) { return 1; }
                break;
            case "STICK_L_H":
                if (Input.GetKey(KeyCode.A)) { return -1; }
                if (Input.GetKey(KeyCode.D)) { return 1; }
                break;


        }




        return 0f;
    }

    float getAxisStateFromController(string nameAxis)
    {
        float R = 1f;
        if (nameAxis.Substring(nameAxis.Length - 1, 1) == "r")
        {
            R = -1f;
            nameAxis = nameAxis.Substring(0, nameAxis.Length - 1);
        }

        if (nameAxis == "axis1") { return Input.GetAxis("Axis 1") * R; }
        if (nameAxis == "axis2") { return Input.GetAxis("Axis 2") * R; }
        if (nameAxis == "axis3") { return Input.GetAxis("Axis 3") * R; }
        if (nameAxis == "axis4") { return Input.GetAxis("Axis 4") * R; }
        if (nameAxis == "axis5") { return Input.GetAxis("Axis 5") * R; }
        if (nameAxis == "axis6") { return Input.GetAxis("Axis 6") * R; }
        if (nameAxis == "axis7") { return Input.GetAxis("Axis 7") * R; }
        if (nameAxis == "axis8") { return Input.GetAxis("Axis 8") * R; }
        if (nameAxis == "axis9") { return Input.GetAxis("Axis 9") * R; }
        if (nameAxis == "axis10") { return Input.GetAxis("Axis 10") * R; }
        if (nameAxis == "axis11") { return Input.GetAxis("Axis 11") * R; }
        if (nameAxis == "axis12") { return Input.GetAxis("Axis 12") * R; }
        return 0f;
    }


    public bool getFuncState(string funcName)
    {
        //仮のコード。キーコードは変数で設定できるようにする予定
        switch (funcName)
        {
            case "Action":
                if (getButtonState("〇") == true || Input.GetKey(KeyCode.Return)) {
                    if (flgAction == true)
                    {
                        return false;
                    }
                    else {
                        flgAction = true;
                        return true;
                    }
                }
                else
                {
                    flgAction = false;
                    return false;
                }
                break;
            case "Menu":
                if (getButtonState("OPTIONS")) { return true; }
                if (Input.GetKey(KeyCode.Slash)) { return true; }
                break;
            case "Dash":
                if (getButtonState("×") == true) { return true; }
                if (Input.GetKey(KeyCode.Escape)) { return true; }
                break;
            case "Jump":
                if ( getButtonState("△")) { return true; }
                if (Input.GetKey(KeyCode.Space)) { return true; }
                break;
            case "Attack":
                if ( getButtonState("□")) { return true; }
                if (Input.GetKey(KeyCode.M)) { return true; }
                break;
            case "Inventry":
                if (Input.GetKey(KeyCode.I)) { return true; }
                break;

            case "Zoom":
                if (getButtonState("L1")) { return true; }
                if (Input.GetKey(KeyCode.Q)) { return true; }
                break;
            case "Prepare":
                if (getButtonState("R2")) { return true; }
                if (Input.GetKey(KeyCode.Backslash)) { return true; }
                break;




        }

        return false;
    }



    public bool getButtonState(string pnameDS4)
    {
        int i;
        for (i = 0; i < 20; i++)
        {
            if (nameDS4[i].Equals(pnameDS4))
            {
                bool value = getButtonStateFromController(configController[i]);
                if (value == true) { return true; }
            }
        }

        /*
        //仮のコード。キーコードは変数で設定できるようにする予定
        switch (pnameDS4)
        {
            case "〇":
                return Input.GetKey(KeyCode.Return);
            case "×":
                return Input.GetKey(KeyCode.Escape);
            case "□":
                return Input.GetKey(KeyCode.V);
            case "△":
                return Input.GetKey(KeyCode.Space);
            case "R1":
                return Input.GetKey(KeyCode.Alpha8);
            case "R2":
                return Input.GetKey(KeyCode.Alpha9);
            case "R3":
                return Input.GetKey(KeyCode.Alpha0);
            case "L1":
                return Input.GetKey(KeyCode.Alpha1);
            case "L2":
                return Input.GetKey(KeyCode.Alpha2);
            case "L3":
                return Input.GetKey(KeyCode.Alpha3);
            case "OPTIONS":
                return Input.GetKey(KeyCode.Underscore);
            case "SHARE":
                return Input.GetKey(KeyCode.Alpha4);
            case "HOME":
                return Input.GetKey(KeyCode.Alpha5);
            case "TOUCH":
                return Input.GetKey(KeyCode.Alpha6);


                nameDS4[0] = "〇";
        nameDS4[1] = "×";
        nameDS4[2] = "□";
        nameDS4[3] = "△";
        nameDS4[4] = "R1";
        nameDS4[5] = "R2";
        nameDS4[6] = "R3";
        nameDS4[7] = "L1";
        nameDS4[8] = "L2";
        nameDS4[9] = "L3";
        nameDS4[10] = "OPTIONS";
        nameDS4[11] = "SHARE";
        nameDS4[12] = "HOME";
        nameDS4[13] = "TOUCH";


    }
    */
        return false;
    }

    public bool getButtonStateFromController(string namebutton)
    {
        if (namebutton == "btn0" && Input.GetKey(KeyCode.JoystickButton0) == true) { return true; }
        if (namebutton == "btn1" && Input.GetKey(KeyCode.JoystickButton1) == true) { return true; }
        if (namebutton == "btn2" && Input.GetKey(KeyCode.JoystickButton2) == true) { return true; }
        if (namebutton == "btn3" && Input.GetKey(KeyCode.JoystickButton3) == true) { return true; }
        if (namebutton == "btn4" && Input.GetKey(KeyCode.JoystickButton4) == true) { return true; }
        if (namebutton == "btn5" && Input.GetKey(KeyCode.JoystickButton5) == true) { return true; }
        if (namebutton == "btn6" && Input.GetKey(KeyCode.JoystickButton6) == true) { return true; }
        if (namebutton == "btn7" && Input.GetKey(KeyCode.JoystickButton7) == true) { return true; }
        if (namebutton == "btn8" && Input.GetKey(KeyCode.JoystickButton8) == true) { return true; }
        if (namebutton == "btn9" && Input.GetKey(KeyCode.JoystickButton9) == true) { return true; }
        if (namebutton == "btn10" && Input.GetKey(KeyCode.JoystickButton10) == true) { return true; }
        if (namebutton == "btn11" && Input.GetKey(KeyCode.JoystickButton11) == true) { return true; }
        if (namebutton == "btn12" && Input.GetKey(KeyCode.JoystickButton12) == true) { return true; }
        if (namebutton == "btn13" && Input.GetKey(KeyCode.JoystickButton13) == true) { return true; }
        if (namebutton == "btn14" && Input.GetKey(KeyCode.JoystickButton14) == true) { return true; }
        if (namebutton == "btn15" && Input.GetKey(KeyCode.JoystickButton15) == true) { return true; }
        if (namebutton == "btn16" && Input.GetKey(KeyCode.JoystickButton16) == true) { return true; }
        if (namebutton == "btn17" && Input.GetKey(KeyCode.JoystickButton17) == true) { return true; }
        if (namebutton == "btn18" && Input.GetKey(KeyCode.JoystickButton18) == true) { return true; }
        if (namebutton == "btn19" && Input.GetKey(KeyCode.JoystickButton19) == true) { return true; }
        return false;
    }
    public string anyButtonFromController()
    {
        if (getButtonStateFromController("btn0") == true) { return "btn0"; }
        if (getButtonStateFromController("btn1") == true) { return "btn1"; }
        if (getButtonStateFromController("btn2") == true) { return "btn2"; }
        if (getButtonStateFromController("btn3") == true) { return "btn3"; }
        if (getButtonStateFromController("btn4") == true) { return "btn4"; }
        if (getButtonStateFromController("btn5") == true) { return "btn5"; }
        if (getButtonStateFromController("btn6") == true) { return "btn6"; }
        if (getButtonStateFromController("btn7") == true) { return "btn7"; }
        if (getButtonStateFromController("btn8") == true) { return "btn8"; }
        if (getButtonStateFromController("btn9") == true) { return "btn9"; }
        if (getButtonStateFromController("btn10") == true) { return "btn10"; }
        if (getButtonStateFromController("btn11") == true) { return "btn11"; }
        if (getButtonStateFromController("btn12") == true) { return "btn12"; }
        if (getButtonStateFromController("btn13") == true) { return "btn13"; }
        if (getButtonStateFromController("btn14") == true) { return "btn14"; }
        if (getButtonStateFromController("btn15") == true) { return "btn15"; }
        if (getButtonStateFromController("btn16") == true) { return "btn16"; }
        if (getButtonStateFromController("btn17") == true) { return "btn17"; }
        if (getButtonStateFromController("btn18") == true) { return "btn18"; }
        if (getButtonStateFromController("btn19") == true) { return "btn19"; }
        return "";
    }
    public string anyAxisStateFromController()
    {
        if (Mathf.Abs(getAxisStateFromController("axis1")) > 0.1f) { return "axsis1"; }
        if (Mathf.Abs(getAxisStateFromController("axis2")) > 0.1f) { return "axsis2"; }
        if (Mathf.Abs(getAxisStateFromController("axis3")) > 0.1f) { return "axsis3"; }
        if (Mathf.Abs(getAxisStateFromController("axis4")) > 0.1f) { return "axsis4"; }
        if (Mathf.Abs(getAxisStateFromController("axis5")) > 0.1f) { return "axsis5"; }
        if (Mathf.Abs(getAxisStateFromController("axis6")) > 0.1f) { return "axsis6"; }
        if (Mathf.Abs(getAxisStateFromController("axis7")) > 0.1f) { return "axsis7"; }
        if (Mathf.Abs(getAxisStateFromController("axis8")) > 0.1f) { return "axsis8"; }
        if (Mathf.Abs(getAxisStateFromController("axis9")) > 0.1f) { return "axsis9"; }
        if (Mathf.Abs(getAxisStateFromController("axis10")) > 0.1f) { return "axsis10"; }
        if (Mathf.Abs(getAxisStateFromController("axis11")) > 0.1f) { return "axsis11"; }
        if (Mathf.Abs(getAxisStateFromController("axis12")) > 0.1f) { return "axsis12"; }
        return "";
    }



}
