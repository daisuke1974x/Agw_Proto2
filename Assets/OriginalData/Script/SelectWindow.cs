using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SelectWindow : MonoBehaviour
{

    public bool isActive = true;
    public string SelectText = "�e�X�g�I";
    private GameObject TextMeshPro;
    public Vector3 WindowPosition = new Vector3(700, 0135, 0);

    public List<string> SelectItem= new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        TextMeshPro = this.transform.Find("Text (TMP)").gameObject;

        //SelectItem.Clear();
        //SelectItem.Add("�́@��");
        //SelectItem.Add("������");
        //Vector3 WindowPosition = new Vector3(100, -200);
        //OpenSelectWindow(WindowPosition, SelectItem, 1, 2, 0);


        SelectItem.Clear();
        SelectItem.Add("����������");
        SelectItem.Add("����");
        SelectItem.Add("����������������");
        SelectItem.Add("�����Ă�");
        SelectItem.Add("����������");
        SelectItem.Add("����������");
        SelectItem.Add("������");
        Vector3 WindowPosition = new Vector3(100, -300);
        OpenSelectWindow(WindowPosition, SelectItem, 2, 2, 0);

    }

    // Update is called once per frame
    void Update()
    {
        //�\���E��\��
        if (isActive)
        {
            //����̏ꏊ�ɕ\��
            this.transform.position = WindowPosition;
        }
        else
        {
            //��ʊO�ɑޔ�
            this.transform.position = new Vector3(Screen.width / 2, 1000, 0);
        }

        //��ʉ𑜓x�ɉ����đ傫����ς���
        float ScreenMagnificationRate = this.transform.parent.GetComponent<UI>().GetScreenMagnificationRate();
        this.transform.localScale = Vector3.one * ScreenMagnificationRate;

        //�����ݒ�
        TextMeshPro.GetComponent<TextMeshProUGUI>().text = "";
    }


    public int OpenSelectWindow(Vector3 pWindowPosition,List<string> pSelectItem,int pColumns,int pRows,int pDefaultIndex)
    {
        //�Ăяo���G���[�`�F�b�N
        if (pColumns < 1)
        {
            Debug.Log("OpenSelectWindow Error -> pColumns : " + pColumns.ToString());
            return -2;
        }
        if (pRows < 1)
        {
            Debug.Log("OpenSelectWindow Error -> pRows : " + pRows.ToString());
            return -2;
        }

        //�P�����̕�
        float ScreenMagnificationRate = this.transform.parent.GetComponent<UI>().GetScreenMagnificationRate();
        float FontSize = 64 * ScreenMagnificationRate;

        //pSelectItem�̍ő啶�������擾����
        int MaxLength = 0;
        for (int Index = 0; Index < pSelectItem.Count; Index++)
        {
            if (pSelectItem[Index].Length > MaxLength) MaxLength = pSelectItem[Index].Length;
        }

        //�E�B���h�E�̃T�C�Y�����肷��
        float WindowWidth = (MaxLength + 5) * pColumns * FontSize;
        float WindowHeight = 2f *( pRows+1) * FontSize;
        Vector2 SizeDelta = this.GetComponent<RectTransform>().sizeDelta;
        SizeDelta.x = WindowWidth;
        SizeDelta.y = WindowHeight;
        this.GetComponent<RectTransform>().sizeDelta = SizeDelta;

        //�A�C�e����\������
        GameObject Items = this.transform.Find("Items").gameObject;
        for (int Index = 0; Index < pSelectItem.Count; Index++)
        {
            GameObject InstanceTextMeshPro = Instantiate(TextMeshPro, Items.transform, true);
            Vector3 Pos = InstanceTextMeshPro.transform.localPosition;
            Pos.x = (Index % pColumns) * FontSize * (MaxLength + 4) + 1 * FontSize;
            Pos.y = -FontSize * Mathf.Floor(Index / pColumns) * 2f;
            InstanceTextMeshPro.transform.localPosition = Pos;
            InstanceTextMeshPro.GetComponent<TextMeshProUGUI>().text = pSelectItem[Index];
        }


        return -1;
    }

}
