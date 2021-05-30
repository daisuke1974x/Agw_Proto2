using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class s_Window_FieldPartsSelect : MonoBehaviour
{
    //private bool isInput = true;
    private s_Main scriptMain = default;
    public GameObject objMainControl;
    public GameObject GuideText;
    public List<GameObject> StockList = new List<GameObject>();
    private GameObject StockSelected;

    private Vector3 StockViewPos = new Vector3(0, 0, 1000f);
    public GameObject objCamera_FieldPartsSelect;
    private bool isSetCheck = false;

    private enum EnumMoveMode
    {
        Idle,
        Select,
        SetSlide,
        SetRotate,
        SetRotateWorld,
        Set
    }
    private EnumMoveMode MoveMode;

    private enum EnumScreenMode
    {
        Select,
        Set,
        AfterSet
    }

    public struct CheckItem
    {
        public float SelfBlockX;
        public float SelfBlockY;
        public float SelfBlockZ;
        public float NodeBlockX;
        public float NodeBlockZ;
        public int Direction;
        public string ConnectionCode;

    }

    public float BlockIntervalX = 10f;
    public float BlockIntervalY = 2.5f;
    public float BlockIntervalZ = 10f;
    private float FloatHeight = 2f;



    private EnumScreenMode ScreenMode;

    private float StockListDistance = 40f;

    private int[] DirectionIndexX = { 0, 1, 0, -1 };
    private int[] DirectionIndexZ = { 1, 0, -1, 0 };

    private int SelectIndex=0;
    private int SelectDirection = 0;
    private float SelectCounter = 0;

    private int SlideDirection = 0;
    private int SlideIndexX=0;
    private int SlideIndexZ=0;
    private float SlideCounter = 0;

    private int RotateDirection = 0;
    private int RotateIndex = 0;
    private float RotateCounter = 0;

    private int RotateDirectionWorld = 0;
    private int RotateIndexWorld = 0;
    private float RotateCounterWorld = 0;

    private float SetCounter = 0;


    public GameObject objCaption;
    public GameObject objSelectedFrame;

    public GameObject DebugText;




    // Start is called before the first frame update
    void Start()
    {
        scriptMain = objMainControl.GetComponent<s_Main>();
        SelectIndex = 0;
    }

    //================================================================================================================
    //�@�E�B���h�E���\�����ꂽ�Ƃ��ɌĂяo���B
    //================================================================================================================
    void OnEnable()
    {

        ScreenMode = EnumScreenMode.Select;
        MoveMode = EnumMoveMode.Idle;
        //SelectIndex = 0;
        SetCounter = 0;

        //�X�g�b�N���X�g�̔z�u
        //List<GameObject> StockList = new List<GameObject>();
        StockList.Clear();
        for (int Index = 0; Index < scriptMain.objFieldPath.transform.GetChildCount(); Index++)
        {
            GameObject obj = scriptMain.objFieldPath.transform.GetChild(Index).gameObject;
            if (obj.name.Substring(0, 5) == "Stock")
            {
                StockList.Add(obj);
            }
        }
        for (int Index = 0; Index < StockList.Count; Index++)
        {
            StockList[Index].transform.position = new Vector3(Index * 40f, 0, 0) + StockViewPos;
            StockList[Index].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));

            //�Ԙg�͔�\���ɂ���
            for (int Index2 = 0; Index2 < StockList[Index].transform.childCount; Index2++)
            {
                GameObject obj = StockList[Index].transform.GetChild(Index2).gameObject;
                if (obj.name == "SelectedFrame(Clone)")
                {
                    for (int Index3 = 0; Index3 < obj.transform.childCount; Index3++)
                    {
                        GameObject obj2 = obj.transform.GetChild(Index3).gameObject;
                        Color col2 = obj2.GetComponent<Renderer>().material.color;
                        col2.a = 0;
                        obj2.GetComponent<Renderer>().material.color = col2;
                    }
                }
            }
        }

        if (SelectIndex > StockList.Count - 1) SelectIndex = StockList.Count - 1;
        if (StockList.Count == 0) SelectIndex = 0;

    }

    //================================================================================================================
    //�@��ԃt���O�ɂ�镪��
    //================================================================================================================
    // Update is called once per frame
    void Update()
    {
        switch (ScreenMode)
        {
            case EnumScreenMode.Select:
                Update_Select();
                break;

            case EnumScreenMode.Set:
                Update_Set();
                break;

            case EnumScreenMode.AfterSet:
                Update_AfterSet();
                break;
        }
    }

    //================================================================================================================
    //�@�I���ς̑�n�̂�������A�������āA�Z�b�g����
    //================================================================================================================
    void Update_Set()
    {
        Vector3 FieldCursorPos = scriptMain.objFieldCursor.transform.position;
        //var StockList = scriptMain.FieldPartsStockList;
        //var StockSelected = StockList[SelectIndex];


        objCaption.GetComponent<Text>().text = "PartsSet : ";

        if (MoveMode == EnumMoveMode.Idle)
        {
            if (Input.GetButtonDown("Cross"))
            {
                OnEnable();
                return;
            }

            if (Input.GetButtonDown("Circle"))
            {

                //Mode = "PartsSet";
                int Rc = SetCheck();
                Debug.Log("SetCheck:" + Rc.ToString());
                if (Rc == 0)
                {
                    MoveMode = EnumMoveMode.Set;
                    SetCounter = 0;
                }
            }

            if (Input.GetAxis("HatLR") != 0 || Input.GetAxis("HatUD") != 0)
            {
                if (Input.GetAxis("HatUD") == 1) SlideDirection = 0;
                if (Input.GetAxis("HatLR") == 1) SlideDirection = 1;
                if (Input.GetAxis("HatUD") == -1) SlideDirection = 2;
                if (Input.GetAxis("HatLR") == -1) SlideDirection = 3;


                //�������i���炷�j���ł��邩�ǂ����`�F�b�N
                bool isMoveOk = false;
                int Dir = (4 + SlideDirection - RotateIndexWorld) % 4;
                Vector3 Compare1 = new Vector3();
                Compare1.x = -DirectionIndexX[Dir] + Mathf.RoundToInt(FieldCursorPos.x / 10);
                Compare1.z = -DirectionIndexZ[Dir] + Mathf.RoundToInt(FieldCursorPos.z / 10);
                for (int i = 0; i < StockSelected.transform.GetChildCount(); i++)
                {
                    var obj = StockSelected.transform.GetChild(i);
                    if (obj.name != "SelectedFrame(Clone)")
                    {
                        var ChidPos = obj.transform.position;
                        Vector3 Compare2 = new Vector3();
                        Compare2.x = Mathf.RoundToInt(ChidPos.x / 10);
                        Compare2.z = Mathf.RoundToInt(ChidPos.z / 10);
                        if (Compare1 == Compare2) isMoveOk = true;
                    }

                }
                if (isMoveOk == true)
                {
                    MoveMode = EnumMoveMode.SetSlide;
                    SlideCounter = 0;
                    GuideText.GetComponent<Text>().text = "";
                }

            }

            bool InputR1 = Input.GetButton("R1");
            bool InputL1 = Input.GetButton("L1");
            if (InputR1 || InputL1)
            {
                MoveMode = EnumMoveMode.SetRotate;
                RotateCounter = 0;
                if (InputL1)
                {
                    RotateDirection = -1;
                }
                else
                {
                    RotateDirection = 1;
                }
                GuideText.GetComponent<Text>().text = "";
            }

            bool InputR2 = Input.GetButton("R2");
            bool InputL2 = Input.GetButton("L2");
            if( InputR2 || InputL2)
            {
                MoveMode = EnumMoveMode.SetRotateWorld;
                RotateCounterWorld = 0;
                if (InputL2)
                {
                    RotateDirectionWorld = -1;
                }
                else
                {
                    RotateDirectionWorld = 1;
                }
                GuideText.GetComponent<Text>().text = "";
            }



        }
        else
        {
            if (MoveMode == EnumMoveMode.SetSlide)
            {
                SlideCounter += 50 * Time.deltaTime;
                if (SlideCounter > 10f)
                {
                    MoveMode = EnumMoveMode.Idle;
                    int Dir2 = (8 + SlideDirection - RotateIndex - RotateIndexWorld) % 4;
                    SlideCounter = 0;
                    SlideIndexX += DirectionIndexX[Dir2];
                    SlideIndexZ += DirectionIndexZ[Dir2];
                    isSetCheck = false;
                }

            }
            if (MoveMode == EnumMoveMode.SetRotate)
            {
                RotateCounter += 200 * Time.deltaTime * RotateDirection;
                if (Mathf.Abs(RotateCounter) > 90)
                {
                    MoveMode = EnumMoveMode.Idle;
                    RotateCounter = 0;
                    RotateIndex = (4 + RotateIndex + RotateDirection) % 4;
                    RotateDirection = 0;
                    isSetCheck = false;
                }

            }

            if (MoveMode == EnumMoveMode.SetRotateWorld)
            {
                RotateCounterWorld += -200 * Time.deltaTime * RotateDirectionWorld;
                if (Mathf.Abs(RotateCounterWorld) > 90)
                {
                    MoveMode = EnumMoveMode.Idle;
                    RotateCounterWorld = 0;
                    RotateIndexWorld = (4 + RotateIndexWorld + RotateDirectionWorld) % 4;
                    RotateDirectionWorld = 0;
                    isSetCheck = false;
                }
            }

            if (MoveMode == EnumMoveMode.Set)
            {
                SetCounter += 2 * Time.deltaTime;
                if (SetCounter > FloatHeight)
                {
                    for (int Index = 0;Index < StockSelected.transform.GetChildCount(); Index++)
                    {
                        GameObject obj = StockSelected.transform.GetChild(Index).gameObject;
                        if (obj.name == "SelectedFrame(Clone)")
                        {
                            //�Ԙg�̏ꍇ�͏���
                            Destroy(obj,0.1f);
                        }
                        else
                        {
                            //�Ԙg�ł͂Ȃ��ꍇ�́AWorld�Ɉڂ�
                            obj.transform.parent = scriptMain.CurrentWorld.transform;
                            //�[���𐮗�
                            Vector3 pos2 = obj.transform.position;
                            pos2.x = Mathf.RoundToInt(pos2.x / BlockIntervalX) * BlockIntervalX;
                            pos2.y = Mathf.RoundToInt(pos2.y / BlockIntervalY) * BlockIntervalY;
                            pos2.z = Mathf.RoundToInt(pos2.z / BlockIntervalZ) * BlockIntervalZ;
                            obj.transform.position = pos2;
                        }

                    }
                    Destroy(StockSelected);
                    scriptMain.objFieldCursor.transform.position = StockViewPos;

                    SetCounter = FloatHeight;
                    ScreenMode = EnumScreenMode.AfterSet;
                    MoveMode = EnumMoveMode.Idle;
                }


            }
        }
        
        //�J�����̈ʒu
        Vector3 CameraRotatepos = FieldCursorPos + new Vector3(0, 50, 0);
        objCamera_FieldPartsSelect.transform.position = CameraRotatepos;
        Vector3 angle = new Vector3(80f, 0, 0);
        objCamera_FieldPartsSelect.transform.rotation = Quaternion.Euler(angle);
       

        //���[���h�̕\������
        Vector3 StockSelectCameraPos = objCamera_FieldPartsSelect.transform.position;
        StockSelectCameraPos.y = FieldCursorPos.y;
        objCamera_FieldPartsSelect.transform.RotateAround(FieldCursorPos, Vector3.up, RotateCounterWorld - RotateIndexWorld * 90f);
        //objCamera_FieldPartsSelect.transform.RotateAround(pos, Vector3.up, -RotatePos_World * 90f);

        //�p�[�c�̕\���A�ʒu
        Vector3 pos = FieldCursorPos;
        pos.x += SlideIndexX * 10;
        pos.y += (FloatHeight - SetCounter);
        pos.z += SlideIndexZ * 10;
        if (MoveMode == EnumMoveMode.SetSlide)
        {
            int dir = (8 + SlideDirection - RotateIndex - RotateIndexWorld) % 4 ;
            pos.x +=  DirectionIndexX[dir] * SlideCounter;
            pos.z +=  DirectionIndexZ[dir] * SlideCounter;
        }
        StockSelected.transform.position = pos;



        //�p�[�c�̌���
        StockSelected.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));//���ʂɂ�������߂�
        Vector3 AroundPos = FieldCursorPos;//��]�̒��S�̎w��
        StockSelected.transform.RotateAround(AroundPos, Vector3.up, RotateCounter + RotateIndex * 90f);//��]

        //�Ԙg��_�ł�����
        for (int i = 0; i < StockSelected.transform.childCount; i++)
        {
            GameObject obj = StockSelected.transform.GetChild(i).gameObject;
            if (obj.name == "SelectedFrame(Clone)")
            {
                for (int j = 0; j < obj.transform.childCount; j++)
                {
                    GameObject obj2 = obj.transform.GetChild(j).gameObject;
                    Color col2 = obj2.GetComponent<Renderer>().material.color;

                    if (!(MoveMode != EnumMoveMode.Set) && !(ScreenMode != EnumScreenMode.AfterSet))
                    {
                        //�_��
                        col2.a = Mathf.Sin(Time.time * 8) / 2 + 0.5f;
                    }
                    else
                    {
                        //Set�̂Ƃ��̉��o�B�X�[�b�Ə���
                        col2.a = (FloatHeight - SetCounter) / FloatHeight;
                    }
                
                    obj2.GetComponent<Renderer>().material.color = col2;
                }
            }

        }

        if(MoveMode == EnumMoveMode.Idle)
        {
            if (isSetCheck == false)
            {
                int Rc = SetCheck();
                if (Rc == 0) GuideText.GetComponent<Text>().text = "�ݒu�ł��܂��B";
                if (Rc == -1 || Rc == -2) GuideText.GetComponent<Text>().text = "�ڍ��ʂ̊G���������Ă��Ȃ��ӏ�������܂��B";
                if (Rc == -3) GuideText.GetComponent<Text>().text = "�ꕔ�������Ȃ��Ă��܂��B";
                isSetCheck = true;
            }
        }



        Text DebugText2 = DebugText.GetComponent<Text>();
        DebugText2.text = "";
        DebugText2.text += "FieldCursorPos:" + FieldCursorPos.ToString() + "\n";
        DebugText2.text += "StockSelected.Position:" + StockSelected.transform.position.ToString() + "\n";
        DebugText2.text += "StockSelected.LocalPosition:" + StockSelected.transform.localPosition.ToString() + "\n";
        DebugText2.text += "StockSelected.rotation:" + StockSelected.transform.rotation.eulerAngles.ToString() + "\n";
        DebugText2.text += "StockSelected.localRotation:" + StockSelected.transform.localRotation.eulerAngles.ToString() + "\n";
        //DebugText2.text += "GetChild(2).Position:" + StockSelected.Hierarchy.transform.GetChild(2).transform.position.ToString() + "\n";
        //DebugText2.text += "GetChild(2).LocalPosition:" + StockSelected.Hierarchy.transform.GetChild(2).transform.localPosition.ToString() + "\n";

        DebugText2.text += "RotateIndex:" + RotateIndex.ToString() + "\n";
        DebugText2.text += "SlideIndexX:" + SlideIndexX.ToString() + "\n";
        DebugText2.text += "SlideIndexZ:" + SlideIndexZ.ToString() + "\n";
        DebugText2.text += "RotateIndexWorld:" + RotateIndexWorld.ToString() + "\n";
        DebugText2.text += "SlideDirection:" + SlideDirection.ToString() + "\n";
        DebugText2.text += "HatUD:" + Input.GetAxis("HatUD").ToString() + "\n";
        DebugText2.text += "HatLR:" + Input.GetAxis("HatLR").ToString() + "\n";


    }
    

    //================================================================================================================
    //�@�ݒu������̑ҋ@��ʁi�s�v�̉\������j
    //================================================================================================================
    void Update_AfterSet()
    {
        GuideText.GetComponent<Text>().text = "�ݒu���������܂����B";

        if (Input.GetButtonDown("Circle") || Input.GetButtonDown("Cross"))
        {
            scriptMain.Mode = "Main";
            ScreenMode = EnumScreenMode.Select;
            MoveMode = EnumMoveMode.Idle;
            this.gameObject.SetActive(false);
        }
    }

    //================================================================================================================
    //�@��n�̂������I������
    //================================================================================================================
    void Update_Select()
    {



        objCaption.GetComponent<Text>().text = "PartsSelect : " + SelectIndex.ToString();

        if (StockList.Count == 0)
        {
            Vector3 pos2 = StockViewPos;
            pos2.y += 40f;
            pos2.z -= 10f;
            pos2.x = SelectIndex * StockListDistance + SelectCounter;
            objCamera_FieldPartsSelect.transform.position = pos2;

            Vector3 angle2 = new Vector3(80f, 0, 0);
            objCamera_FieldPartsSelect.transform.rotation = Quaternion.Euler(angle2);

            GuideText.GetComponent<Text>().text = "��n�̂�����������Ă��܂���B";
            if (Input.GetButtonDown("Circle") || Input.GetButtonDown("Cross"))
            {
                scriptMain.Mode = "Main";
                ScreenMode = EnumScreenMode.Select;
                MoveMode = EnumMoveMode.Idle;
                this.gameObject.SetActive(false);
                return;
            }
            return;


        }

        GuideText.GetComponent<Text>().text = "��n�̂������I�����Ă��������B";


        if (MoveMode == EnumMoveMode.Select)
        {
            SelectCounter += 100f * SelectDirection * Time.deltaTime;
            if (Mathf.Abs(SelectCounter) > StockListDistance)
            {
                SelectIndex += SelectDirection;
                SelectDirection = 0;
                SelectCounter = 0;
                MoveMode = EnumMoveMode.Idle;
            }
        }
        else
        {
            if (Input.GetButtonDown("Cross"))
            {
                scriptMain.Mode = "Main";
                ScreenMode = EnumScreenMode.Select;
                MoveMode = EnumMoveMode.Idle;
                this.gameObject.SetActive(false);
            }

            if (Input.GetButtonDown("Circle"))
            {
                ScreenMode = EnumScreenMode.Set;
                MoveMode = EnumMoveMode.Idle;
                RotateIndexWorld = 0;
                RotateDirectionWorld = 0;
                RotateCounterWorld = 0;
                RotateIndex = 0;
                RotateDirection = 0;
                RotateCounter = 0;
                SlideIndexX = 0;
                SlideIndexZ = 0;
                SlideCounter = 0;
                SlideDirection = 0;
                StockSelected = StockList[SelectIndex];
                isSetCheck = false;
            }

            if (Input.GetAxis("HatLR") == -1)
            {
                if (SelectIndex > 0)
                {
                    SelectDirection = -1;
                    MoveMode = EnumMoveMode.Select;
                    SelectCounter = 0;

                }
            }
            if (Input.GetAxis("HatLR") == 1)
            {
                if (SelectIndex < StockList.Count - 1)
                {
                    SelectDirection = 1;
                    MoveMode = EnumMoveMode.Select;
                    SelectCounter = 0f;
                }
            }


        }

        Vector3 pos = StockViewPos;
        pos.y += 40f;
        pos.z -= 10f;
        pos.x = SelectIndex * StockListDistance + SelectCounter;
        objCamera_FieldPartsSelect.transform.position = pos;

        Vector3 angle = new Vector3(80f, 0, 0);
        objCamera_FieldPartsSelect.transform.rotation = Quaternion.Euler(angle);



    }

    //================================================================================================================
    //�@�ݒu�\���ǂ����̃`�F�b�N
    //================================================================================================================
    int SetCheck()
    {
        if (ScreenMode != EnumScreenMode.Set) return -9999;//�������Ăяo����Ă��Ȃ��B�ʏ�͋N���Ȃ��G���[�B

        List<CheckItem> CheckItemWorld = new List<CheckItem>();
        List<CheckItem> CheckItemStock = new List<CheckItem>();

        //World���@��d�����m�[�h�̒��o
        for (int Index = 0; Index < scriptMain.CurrentWorld.transform.GetChildCount(); Index++)
        {
            GameObject FragmentPrefab = scriptMain.CurrentWorld.transform.GetChild(Index).gameObject;
            s_FieldPartsParameter FieldPartsParameter = FragmentPrefab.GetComponent<s_FieldPartsParameter>();
            Vector3 SelfBlockPos = new Vector3();
            SelfBlockPos.x = Mathf.RoundToInt(FragmentPrefab.transform.position.x / BlockIntervalX) ;
            SelfBlockPos.z = Mathf.RoundToInt(FragmentPrefab.transform.position.z / BlockIntervalZ) ;
            float Angle = FragmentPrefab.transform.rotation.eulerAngles.y;
            int SelfRotateIndex = Mathf.RoundToInt(Angle / 90);

            for (int dir = 0; dir < 4; dir++)
            {
                //�אڃ`�F�b�N
                Vector3 CheckBlockPos1 = new Vector3();
                CheckBlockPos1.x = SelfBlockPos.x + DirectionIndexX[dir];
                CheckBlockPos1.z = SelfBlockPos.z + DirectionIndexZ[dir];
                bool isNeighbor = false;
                for (int Index2 = 0; Index2 < scriptMain.CurrentWorld.transform.GetChildCount(); Index2++)
                {
                    GameObject FragmentPrefab2 = scriptMain.CurrentWorld.transform.GetChild(Index2).gameObject;
                    Vector3 CheckBlockPos2 = new Vector3();
                    CheckBlockPos2.x = Mathf.RoundToInt(FragmentPrefab2.transform.position.x / BlockIntervalX);
                    CheckBlockPos2.z = Mathf.RoundToInt(FragmentPrefab2.transform.position.z / BlockIntervalZ);
                    if (CheckBlockPos1 == CheckBlockPos2) isNeighbor = true;
                }

                //�אڂ��Ȃ��ꍇ�́A�m�[�h���쐬����
                if (isNeighbor == false)
                {
                    //��d�����̂Ƃ���Node�́ASelf�Ɠ���
                    CheckItem Item = new CheckItem();
                    Item.SelfBlockX = SelfBlockPos.x;
                    Item.SelfBlockY = Mathf.RoundToInt(FragmentPrefab.transform.position.y / BlockIntervalY);
                    Item.SelfBlockZ = SelfBlockPos.z;
                    Item.NodeBlockX = SelfBlockPos.x;
                    Item.NodeBlockZ = SelfBlockPos.z;
                    Item.Direction = (dir + 2) % 4;
                    Item.ConnectionCode = FieldPartsParameter.ConnectionCodeOpponent[(4+dir + 0 - SelfRotateIndex) % 4];
                    CheckItemWorld.Add(Item);

                    string LogString = scriptMain.CurrentWorld.name + ";" + FragmentPrefab.name + ";";
                    LogString += "(";
                    LogString += Item.SelfBlockX.ToString() + ",";
                    LogString += Item.SelfBlockY.ToString() + ",";
                    LogString += Item.SelfBlockZ.ToString() + ",";
                    LogString += "),(";
                    LogString += Item.NodeBlockX.ToString() + ",";
                    LogString += Item.NodeBlockZ.ToString() + ",";
                    LogString += "),";
                    LogString += Item.Direction.ToString() + ",";
                    LogString += Item.ConnectionCode;
                    Debug.Log(LogString);
                }
            }
        }

        //Stock���@�d�����m�[�h�̒��o
        for (int Index = 0; Index < StockSelected.transform.GetChildCount(); Index++)
        {
            GameObject FragmentPrefab = StockSelected.transform.GetChild(Index).gameObject;
            if (FragmentPrefab.name != "SelectedFrame(Clone)")
            {
                s_FieldPartsParameter FieldPartsParameter = FragmentPrefab.GetComponent<s_FieldPartsParameter>();
                Vector3 SelfBlockPos = new Vector3();
                SelfBlockPos.x = Mathf.RoundToInt(FragmentPrefab.transform.position.x / BlockIntervalX);
                SelfBlockPos.z = Mathf.RoundToInt(FragmentPrefab.transform.position.z / BlockIntervalZ);
                float Angle = FragmentPrefab.transform.rotation.eulerAngles.y;
                int SelfRotateIndex = Mathf.RoundToInt(Angle / 90);

                for (int dir = 0; dir < 4; dir++)
                {
                    //�אڃ`�F�b�N
                    Vector3 CheckBlockPos1 = new Vector3();
                    CheckBlockPos1.x = SelfBlockPos.x + DirectionIndexX[(4 + dir + RotateIndex) % 4];
                    CheckBlockPos1.z = SelfBlockPos.z + DirectionIndexZ[(4 + dir + RotateIndex) % 4];
                    bool isNeighbor = false;

                    for (int Index2 = 0; Index2 < StockSelected.transform.GetChildCount(); Index2++)
                    {
                        GameObject FragmentPrefab2 = StockSelected.transform.GetChild(Index2).gameObject;
                        if (FragmentPrefab2.name != "SelectedFrame(Clone)")
                        {
                            Vector3 CheckBlockPos2 = new Vector3();
                            CheckBlockPos2.x = Mathf.RoundToInt(FragmentPrefab2.transform.position.x / BlockIntervalX);
                            CheckBlockPos2.z = Mathf.RoundToInt(FragmentPrefab2.transform.position.z / BlockIntervalZ);
                            if (CheckBlockPos1 == CheckBlockPos2) isNeighbor = true;
                        }

                    }

                    //�אڂ��Ȃ��ꍇ�́A�m�[�h���쐬����
                    if (isNeighbor == false)
                    {
                        CheckItem Item = new CheckItem();
                        //�d�����̂Ƃ��́AX,Z���W�́ANode�͑����␳��
                        //Item.NodeBlockX = Mathf.RoundToInt(FragmentPrefab.transform.position.x / BlockIntervalX) + DirectionIndexX[(dir + RotateIndex + rotdir) % 4];
                        Item.SelfBlockX = SelfBlockPos.x;
                        Item.SelfBlockY = Mathf.RoundToInt(FragmentPrefab.transform.position.y / BlockIntervalY);
                        Item.SelfBlockZ = SelfBlockPos.z;
                        Item.NodeBlockX = SelfBlockPos.x + DirectionIndexX[(4 + dir + RotateIndex) % 4];
                        Item.NodeBlockZ = SelfBlockPos.z + DirectionIndexZ[(4 + dir + RotateIndex) % 4];
                        Item.Direction = (4 + dir + RotateIndex) % 4;
                        Item.ConnectionCode = FieldPartsParameter.ConnectionCode[(4 + dir + RotateIndex - SelfRotateIndex) % 4];
                        CheckItemStock.Add(Item);

                        string LogString = StockSelected.name + ";" + FragmentPrefab.name + ";";
                        LogString += "(";
                        LogString += Item.SelfBlockX.ToString() + ",";
                        LogString += Item.SelfBlockY.ToString() + ",";
                        LogString += Item.SelfBlockZ.ToString() + ",";
                        LogString += "),(";
                        LogString += Item.NodeBlockX.ToString() + ",";
                        LogString += Item.NodeBlockZ.ToString() + ",";
                        LogString += "),";
                        LogString += Item.Direction.ToString() + ",";
                        LogString += Item.ConnectionCode;
                        Debug.Log(LogString);
                    }
                }

            }
        }

        //�}�b�`���O����
        int CountOK = 0;
        int CountNG = 0;
        int Overlap = 0;
        for (int Index = 0; Index < CheckItemWorld.Count; Index++)
        {
            CheckItem ItemWorld = CheckItemWorld[Index];
            for (int Index2 = 0; Index2 < CheckItemStock.Count; Index2++)
            {
                CheckItem ItemStock = CheckItemStock[Index2];

                if (ItemWorld.SelfBlockX == ItemStock.SelfBlockX && ItemWorld.SelfBlockZ == ItemStock.SelfBlockZ)
                {
                    Overlap++;
                }
                else
                {
                    if (ItemWorld.NodeBlockX == ItemStock.NodeBlockX && ItemWorld.NodeBlockZ == ItemStock.NodeBlockZ && ItemWorld.Direction == ItemStock.Direction)
                    {
                        if (ItemWorld.ConnectionCode == ItemStock.ConnectionCode)
                        {
                            CountOK++;
                        }
                        else
                        {
                            CountNG++;
                        }
                    }

                }

            }

        }

        Debug.Log("CountOK:" + CountOK.ToString() + ", CountNG:" + CountNG.ToString() + ", Overlap:" + Overlap.ToString());

        if (CountOK == 0) return -1;//�ڂ��Ă��镔���̊G���ŁA��v�����ӏ����Ȃ�
        if (CountNG > 0) return -2;//�ڍ��ʂ̊G���������Ă��Ȃ��ӏ�������
        if (Overlap > 0) return -3;//�ꕔ���d�Ȃ��Ă���BFragment���Q���ȏ�̂Ƃ����肤��B

        return 0;//����


    }


}
