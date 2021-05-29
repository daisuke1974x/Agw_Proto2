using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_Weapon : MonoBehaviour
{

    public GameObject objPlayer;
    public GameObject objMain;
    private Animator objAnimator;
    private CharacterController objCharController;
    private s_Main objScriptMain;

    private s_CharStatus EnemyStatus;
    private s_CharStatus PlayerStatus;



    // Start is called before the first frame update
    void Start()
    {
        objPlayer = GameObject.Find("Player");
        objMain = GameObject.Find("MainControl");
        objScriptMain = objMain.GetComponent<s_Main>();
        PlayerStatus = objPlayer.GetComponent<s_CharStatus>();
        //EnemyStatus = this.GetComponent<s_CharStatus>();
        Debug.Log("s_Weapon!");
    }

    // Update is called once per frame
    void Update()
    {

    }

    //void OnCollisionEnter(Collision collision)
    //{
    //    if (PlayerStatus.isAttack == true)
    //    {
    //        if (collision.gameObject.tag == "Enemy")
    //        {
    //            s_CharStatus EnemyStatus = collision.gameObject.GetComponent<s_CharStatus>();
    //            objScriptMain.PhyzicalAttack(ref PlayerStatus, ref EnemyStatus, 1, 10);

    //            Debug.Log("Hit!");
    //        }

    //    }
    //}

    void OnTriggerEnter(Collider other)
    {
        if (PlayerStatus.isAttack == true)
        {
            if (other.gameObject.tag == "Enemy")
            {
                s_CharStatus EnemyStatus = other.gameObject.GetComponent<s_CharStatus>();
                objScriptMain.PhyzicalAttack(ref PlayerStatus, ref EnemyStatus, 1, 10);

                Debug.Log("Hit!");
            }

        }
    }
}
