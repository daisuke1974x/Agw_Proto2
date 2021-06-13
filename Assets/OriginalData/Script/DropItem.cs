using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    public int MoneyValue;
    public GameObject objPlayer;
    CharStatus PlayerStatus;

    private bool isGot = false;
    private float GotAnimationStartTime;
    private float StartTime;


    // Start is called before the first frame update
    void Start()
    {
        objPlayer = GameObject.Find("Player");
        PlayerStatus = objPlayer.GetComponent<CharStatus>();
        StartTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (isGot==true )
        {
            if (GotAnimationStartTime + 0.3f < Time.time)
            {
                Destroy(this.gameObject);

            }
            else
            {
                Vector3 pos = this.transform.position;
                pos.y += 2f * Time.deltaTime;
                this.transform.position = pos;

            }

        }



    }



    void OnTriggerEnter(Collider other)
    {
        if (isGot == false && StartTime + 0.5f < Time.time && other.tag =="Player")
        {
            isGot = true;
            PlayerStatus.Money += MoneyValue;
            Debug.Log("You Got Money:" + MoneyValue.ToString()); // ƒƒO‚ð•\Ž¦‚·‚é

            GotAnimationStartTime = Time.time;

        }
    }

}
