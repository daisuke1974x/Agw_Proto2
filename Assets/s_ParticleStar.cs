using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_ParticleStar : MonoBehaviour
{
    float TimeCounter = 0;
    float DestroyTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        DestroyTime = Random.Range(0.3f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        TimeCounter += Time.deltaTime;

        Vector3 pos = this.transform.position;
        pos.y -= 100 * Time.deltaTime;
        pos.z = 100;
        this.transform.position = pos;
        this.transform.localScale = new Vector3(1, 1, 1) * ((DestroyTime - TimeCounter) / DestroyTime)*0.5f;


        if (DestroyTime<TimeCounter )
        {
            Destroy(this.gameObject);
        }



    }
}
