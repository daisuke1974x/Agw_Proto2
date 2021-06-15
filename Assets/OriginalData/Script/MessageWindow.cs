using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageWindow : MonoBehaviour
{
    public bool isActive = false;
    public string MessageText="�e�X�g�I";
    public GameObject TextMeshPro;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        //�\���E��\��
        if (isActive)
        {
            //this.gameObject.SetActive(true);
            //�\���ʒu�i�^�񒆉��j
            this.transform.position = new Vector3(Screen.width / 2, 0, 0);
        }
        else
        {
            //this.gameObject.SetActive(false);
            //�\���ʒu�i�^�񒆉��j
            this.transform.position = new Vector3(Screen.width / 2, -1000, 0);

        }

        //��ʉ𑜓x�ɉ����đ傫����ς���
        if (100 < Screen.height && Screen.height <= 600)
        {
            this.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
        if (600 < Screen.height && Screen.height <= 1400)
        {
            this.transform.localScale = new Vector3(1, 1, 1);
        }

        if (1400 < Screen.height && Screen.height <= 3000)
        {
            this.transform.localScale = new Vector3(2, 2, 2);
        }


        //�����ݒ�
        TextMeshPro.GetComponent<TextMeshProUGUI>().text  = MessageText;

    }
}
