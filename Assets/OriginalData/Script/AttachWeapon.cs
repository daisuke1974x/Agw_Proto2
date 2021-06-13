using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachWeapon : MonoBehaviour
{

    public GameObject objPlayer;
    public GameObject MainControl;
    private Animator objAnimator;
    private CharacterController objCharController;
    //private MainControl MainControl;

    private CharStatus EnemyStatus;
    private CharStatus PlayerStatus;



    // Start is called before the first frame update
    void Start()
    {
        objPlayer = GameObject.Find("Player");
        MainControl = GameObject.Find("MainControl");
        //MainControl = objMain.GetComponent<MainControl>();
        PlayerStatus = objPlayer.GetComponent<CharStatus>();
        //EnemyStatus = this.GetComponent<s_CharStatus>();
        Debug.Log("AtouchWeapon!");
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
                CharStatus EnemyStatus = other.gameObject.GetComponent<CharStatus>();
                MainControl.GetComponent<MainControl>().PhyzicalAttack(ref PlayerStatus, ref EnemyStatus, 1, 10);

                Debug.Log("Hit!");
            }

        }
    }
}
