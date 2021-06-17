using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SelectWindow : MonoBehaviour
{

    public bool isActive = true;
    public string SelectText = "テスト！";
    private GameObject TextMeshPro;
    public Vector3 WindowPosition = new Vector3(700, 0135, 0);

    public List<string> SelectItem= new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        TextMeshPro = this.transform.Find("Text (TMP)").gameObject;

        //SelectItem.Clear();
        //SelectItem.Add("は　い");
        //SelectItem.Add("いいえ");
        //Vector3 WindowPosition = new Vector3(100, -200);
        //OpenSelectWindow(WindowPosition, SelectItem, 1, 2, 0);


        SelectItem.Clear();
        SelectItem.Add("あいうえお");
        SelectItem.Add("かき");
        SelectItem.Add("くけこさしすせそ");
        SelectItem.Add("たちつてと");
        SelectItem.Add("あいうえお");
        SelectItem.Add("あいうえお");
        SelectItem.Add("いいえ");
        Vector3 WindowPosition = new Vector3(100, -300);
        OpenSelectWindow(WindowPosition, SelectItem, 2, 2, 0);

    }

    // Update is called once per frame
    void Update()
    {
        //表示・非表示
        if (isActive)
        {
            //所定の場所に表示
            this.transform.position = WindowPosition;
        }
        else
        {
            //画面外に退避
            this.transform.position = new Vector3(Screen.width / 2, 1000, 0);
        }

        //画面解像度に応じて大きさを変える
        float ScreenMagnificationRate = this.transform.parent.GetComponent<UI>().GetScreenMagnificationRate();
        this.transform.localScale = Vector3.one * ScreenMagnificationRate;

        //文字設定
        TextMeshPro.GetComponent<TextMeshProUGUI>().text = "";
    }


    public int OpenSelectWindow(Vector3 pWindowPosition,List<string> pSelectItem,int pColumns,int pRows,int pDefaultIndex)
    {
        //呼び出しエラーチェック
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

        //１文字の幅
        float ScreenMagnificationRate = this.transform.parent.GetComponent<UI>().GetScreenMagnificationRate();
        float FontSize = 64 * ScreenMagnificationRate;

        //pSelectItemの最大文字数を取得する
        int MaxLength = 0;
        for (int Index = 0; Index < pSelectItem.Count; Index++)
        {
            if (pSelectItem[Index].Length > MaxLength) MaxLength = pSelectItem[Index].Length;
        }

        //ウィンドウのサイズを決定する
        float WindowWidth = (MaxLength + 5) * pColumns * FontSize;
        float WindowHeight = 2f *( pRows+1) * FontSize;
        Vector2 SizeDelta = this.GetComponent<RectTransform>().sizeDelta;
        SizeDelta.x = WindowWidth;
        SizeDelta.y = WindowHeight;
        this.GetComponent<RectTransform>().sizeDelta = SizeDelta;

        //アイテムを表示する
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
