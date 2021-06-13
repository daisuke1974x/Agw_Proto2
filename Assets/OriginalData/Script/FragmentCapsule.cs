using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentCapsule : MonoBehaviour
{
    
    private float Counter = 0;

    // Start is called before the first frame update
    void Start()
    {
        Counter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Counter += Time.deltaTime;

        if (Counter < 0.75f)
        {
            Vector3 pos = this.transform.position;
            pos.y += Time.deltaTime;
            this.transform.position = pos;
        }
        else
        {
            Destroy(this.gameObject);

        }

    }
}
