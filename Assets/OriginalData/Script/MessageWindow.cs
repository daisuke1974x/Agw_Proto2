using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageWindow : MonoBehaviour
{
    public bool isActive = false;
    public string MessageText="�e�X�g�I";
    private GameObject TextMeshPro;

    // Start is called before the first frame update
    void Start()
    {
        TextMeshPro = this.transform.Find("Text (TMP)").gameObject;
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
        float ScreenMagnificationRate = this.transform.parent.GetComponent<UI>().GetScreenMagnificationRate();
        this.transform.localScale = Vector3.one * ScreenMagnificationRate;


        //�����ݒ�
        TextMeshPro.GetComponent<TextMeshProUGUI>().text  = MessageText;

    }
}
