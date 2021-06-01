using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class s_FragmentsList : MonoBehaviour
{
    public GameObject Field;
    public GameObject MainCamera;
    public GameObject Player;
    public GameObject MainControl;
    public GameObject FragmentsListCameraPrefab;
    private s_Main MainScript;
    private Vector3 FragmentsListPos = new Vector3(0, 0, 20000);

    public RenderTexture FragmentRenderTexture;
    public RawImage FragmentRawImage;

    public struct StStock
    {
        public string StockName;
        public GameObject StockRawImage;
        public GameObject StockCameraPrefab;
        public GameObject Stock;
    }
    public List<StStock> StockList;


    // Start is called before the first frame update
    void Start()
    {
        MainScript = MainControl.GetComponent<s_Main>();
    }

    public void AddList(string pStockName)
    {
        GameObject Stock = GameObject.Find(pStockName);
        RenderTexture StockRenderTexture = Instantiate(FragmentRenderTexture, this.transform, true);

        GameObject StockCameraPrefab = Instantiate(FragmentsListCameraPrefab.gameObject, Field.transform, true);
        Camera StockCamera = StockCameraPrefab.transform.GetChild(0).GetComponent<Camera>();
        StockCamera.targetTexture = StockRenderTexture;
        StockCameraPrefab.name = "Camera_" + pStockName;
        StockCameraPrefab.transform.position = Stock.transform.position;

        GameObject StockRawImage = Instantiate(FragmentRawImage, this.transform, true).gameObject;
        StockRawImage.GetComponent<RawImage>().texture = StockRenderTexture;
        StockRawImage.name = "Image_" + pStockName;
        StockRawImage.transform.position = new Vector3(100, 100, 0);

        RefreshList();
    }

    void RefreshList()
    {
        float Cnt = 0;
        string StockNum = "";
        List<StStock> StockList= new List<StStock>();

        for (int Index = 0; Index < Field.transform.GetChildCount(); Index++)
        {
            GameObject FindObject = Field.transform.GetChild(Index).gameObject;
            if (FindObject.name.Substring(0, 5) == "Stock")
            {
                GameObject Stock = FindObject;
                StockNum = FindObject.name.Substring("Stock_".Length, FindObject.name.Length - "Stock_".Length);
                GameObject StockCameraPrefab = GameObject.Find("Camera_" + Stock.name);
                GameObject StockRawImage = GameObject.Find("Image_" + Stock.name).gameObject;

                Stock.transform.position = new Vector3(Cnt * 100, 0, 0) + FragmentsListPos;
                StockCameraPrefab.transform.position = Stock.transform.position;
                StockRawImage.transform.position = new Vector3(Cnt * 100, 30, 9);

                StStock AddItem = new StStock();
                AddItem.StockName = Stock.name;
                AddItem.StockRawImage = StockRawImage;
                AddItem.StockCameraPrefab = StockCameraPrefab;
                AddItem.Stock = Stock;

                StockList.Add(AddItem);

                Cnt++;

            }




        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
