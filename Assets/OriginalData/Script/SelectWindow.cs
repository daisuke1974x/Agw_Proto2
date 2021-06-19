using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SelectWindow : MonoBehaviour
{

    public bool isActive = false;
    public string SelectText = "�e�X�g�I";
    private GameObject UiObject;
    private GameObject TextMeshPro;
    private GameObject SelectCursor;
    public Vector3 WindowPosition;// = new Vector3(700, 0135, 0);

    public List<string> SelectItem= new List<string>();

    public int CursorPosX;
    public int CursorPosY;
    private int Columns;
    private int Rows;
    public int ReturnIndex;
    private int MaxLength;
    private float FontSize;

    private float BlinkTimer;
    private float BlinkTimerSetting = 0.8f;
    private int CaptionRow=0;

    // Start is called before the first frame update
    void Start()
    {
        UiObject = this.transform.parent.gameObject;
        TextMeshPro = this.transform.Find("Text (TMP)").gameObject;
        SelectCursor = this.transform.Find("SelectCursor").gameObject;

    }

    public IEnumerator Test()
    {
        SelectItem.Clear();
        SelectItem.Add("����������");
        SelectItem.Add("����");
        SelectItem.Add("����������������");
        SelectItem.Add("�����Ă�");
        SelectItem.Add("����������");
        SelectItem.Add("����������");
        SelectItem.Add("������");
        SelectItem.Add("������");
        SelectItem.Add("������");
        SelectItem.Add("������");
        SelectItem.Add("������");
        SelectItem.Add("������");
        WindowPosition = new Vector3(100, Screen.height - 100);
        yield return StartCoroutine(OpenSelectWindow("�I��ł�������", WindowPosition, SelectItem, 3, 4, 0));
        //yield return null;
    }

    public IEnumerator YesNoWindow()
    {
        SelectItem.Clear();
        SelectItem.Add("�́@��");
        SelectItem.Add("������");
        Vector3 WindowPosition = new Vector3(100, -200);
        yield return StartCoroutine(OpenSelectWindow("",WindowPosition, SelectItem, 1, 2, 0));

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


    public IEnumerator OpenSelectWindow(string pCaption, Vector3 pWindowPosition,List<string> pSelectItem,int pColumns,int pRows,int pDefaultIndex)
    {

        //�Ăяo���G���[�`�F�b�N
        if (pColumns < 1)
        {
            Debug.Log("OpenSelectWindow Error -> pColumns : " + pColumns.ToString());
            yield return -2;
        }
        if (pRows < 1)
        {
            Debug.Log("OpenSelectWindow Error -> pRows : " + pRows.ToString());
            yield return -2;
        }

        //Caption�̎w�肪����ꍇ�́A�E�B���h�E�̏c�T�C�Y���{�P�s�ǉ����đ傫������
        if (pCaption == "")
        {
            CaptionRow = 0;
            this.transform.Find("Caption").gameObject.GetComponent<TextMeshProUGUI>().text = "";
        }
        else
        {
            CaptionRow = 1;
            this.transform.Find("Caption").gameObject.GetComponent<TextMeshProUGUI>().text = pCaption;

        }

        //�P�����̕�
        float ScreenMagnificationRate = this.transform.parent.GetComponent<UI>().GetScreenMagnificationRate();
        FontSize = 64 * ScreenMagnificationRate;

        //pSelectItem�̍ő啶�������擾����
        MaxLength = 0;
        for (int Index = 0; Index < pSelectItem.Count; Index++)
        {
            if (pSelectItem[Index].Length > MaxLength) MaxLength = pSelectItem[Index].Length;
        }

        //�E�B���h�E�̃T�C�Y�����肷��
        float WindowWidth = (MaxLength + 5) * pColumns * FontSize;
        float WindowHeight = 2f *( pRows + 1+ CaptionRow) * FontSize;
        Vector2 SizeDelta = this.GetComponent<RectTransform>().sizeDelta;
        SizeDelta.x = WindowWidth;
        SizeDelta.y = WindowHeight;
        this.GetComponent<RectTransform>().sizeDelta = SizeDelta;

        //�A�C�e����\������
        GameObject Items = this.transform.Find("Items").gameObject;
        TextMeshPro = this.transform.Find("Text (TMP)").gameObject;
        for (int Index = 0; Index < pSelectItem.Count; Index++)
        {
            GameObject InstanceTextMeshPro = Instantiate(TextMeshPro, Items.transform, true);
            Vector3 Pos = InstanceTextMeshPro.transform.localPosition;
            Pos.x = (Index % pColumns) * FontSize * (MaxLength + 4) + 1 * FontSize;
            Pos.y = -FontSize * (Mathf.Floor(Index / pColumns) - 1 + CaptionRow) * 2f;
            InstanceTextMeshPro.transform.localPosition = Pos;
            InstanceTextMeshPro.GetComponent<TextMeshProUGUI>().text = pSelectItem[Index];
        }

        Rows = pRows;
        Columns = pColumns;
        CursorPosX = pDefaultIndex % Columns;
        CursorPosY = Mathf.RoundToInt(pDefaultIndex / Columns);
        BlinkTimer = BlinkTimerSetting;
        isActive = true;

        yield return null;//�����ł�������Ă����Ȃ��ƁAvoid start��ւ��GameObject�擾���ł����Anull�ɂȂ�
        yield return StartCoroutine(SelectWindowCursor());
        isActive = false;
        //for (; ; )
        //{


        //}

//        yield return -1;
    }

    IEnumerator SelectWindowCursor()
    {
        bool InputHatFlg = false;
        for(; ; )
        {
            float HatLR = Input.GetAxis("HatLR");
            float HatUD = Input.GetAxis("HatUD");

            if (HatLR != 0)
            {
                if (InputHatFlg == false)
                {
                    InputHatFlg = true;
                    if (HatLR == 1)
                    {
                        CursorPosX = (CursorPosX + Columns + 1) % Columns;
                        BlinkTimer = BlinkTimerSetting;
                        //Debug.Log("SelectWindow : CursorPosX = " + CursorPosX.ToString() + ", CursorPosY = " + CursorPosY.ToString());
                    }
                    if (HatLR == -1)
                    {
                        CursorPosX = (CursorPosX + Columns - 1) % Columns;
                        //Debug.Log("SelectWindow : CursorPosX = " + CursorPosX.ToString() + ", CursorPosY = " + CursorPosY.ToString());
                        BlinkTimer = BlinkTimerSetting;
                    }

                }
            }
            else
            {
                if (HatUD != 0)
                {
                    if (InputHatFlg == false)
                    {
                        InputHatFlg = true;
                        if (HatUD == -1)
                        {
                            CursorPosY = (CursorPosY + Rows + 1) % Rows;
                            //Debug.Log("SelectWindow : CursorPosX = " + CursorPosX.ToString() + ", CursorPosY = " + CursorPosY.ToString());
                            BlinkTimer = BlinkTimerSetting;
                        }
                        if (HatUD == 1)
                        {
                            CursorPosY = (CursorPosY + Rows - 1) % Rows;
                            //Debug.Log("SelectWindow : CursorPosX = " + CursorPosX.ToString() + ", CursorPosY = " + CursorPosY.ToString());
                            BlinkTimer = BlinkTimerSetting;
                        }

                    }

                }
                else
                {
                    InputHatFlg = false;

                }
            }
            BlinkTimer -= Time.deltaTime;
            if (BlinkTimer<0) BlinkTimer = BlinkTimerSetting;
            if (BlinkTimer > BlinkTimerSetting / 2)
            {
                SelectCursor.SetActive(true);
                SelectCursor.transform.localPosition = new Vector3((MaxLength + 4) * CursorPosX * FontSize, -2f * (CursorPosY + 1 + CaptionRow) * FontSize, 0);
            }
            else
            {
                SelectCursor.SetActive(false);

            }

            yield return null;

            if (Input.GetButtonDown("Circle"))
            {
                BlinkTimer = BlinkTimerSetting;
                ReturnIndex = CursorPosX + CursorPosY * Columns;
                break;
            }

            if (Input.GetButtonDown("Cross"))
            {
                BlinkTimer = BlinkTimerSetting;
                ReturnIndex = -1;
                break;
            }

        }


        //yield return null;

    }

}
