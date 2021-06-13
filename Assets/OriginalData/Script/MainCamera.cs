using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{

    public GameObject objPlayer;
    private GameObject objCameraLink;

    //カメラ関連
    float CameraDistance = 5;  //オブジェクトからカメラの距離の初期値
    float cameraRotateSpeed = 150;  //カメラを回す速さ
                                    //private Quaternion tmpQuaternion;

    //操作禁止フラグ（別スクリプトから操作する）
    public bool isControllEnabled = true;



    // Start is called before the first frame update
    void Start()
    {
        //カメラ初期位置
        this.transform.position = objPlayer.transform.position;
        this.transform.rotation = objPlayer.transform.rotation;
        this.transform.position += this.transform.forward * -CameraDistance;
        this.transform.RotateAround(objPlayer.transform.position, objPlayer.transform.TransformDirection(Vector3.left), -30);

        //カメラリンクオブジェクトの生成
        objCameraLink = new GameObject("CameraLink");  //GameObject objCameraLink = new GameObject("CameraLink");  とすると、UpdateでNullになる謎。2021/5/23
        objCameraLink.transform.position = objPlayer.transform.position;
        objCameraLink.transform.rotation = objPlayer.transform.rotation;

    }

    // Update is called once per frame
    void Update()
    {

        if (isControllEnabled == false) return;

        //---------------------------------------------------------------------------------------------------------------------------------------
        //プレイヤーオブジェクトに合わせて、objCameraLinkを追随させる
        //---------------------------------------------------------------------------------------------------------------------------------------
        Vector3 CameraLinkVec = objCameraLink.transform.position;
        CameraLinkVec.x = objPlayer.transform.position.x;
        CameraLinkVec.y = objPlayer.transform.position.y + 1f;//チルト値
        CameraLinkVec.z = objPlayer.transform.position.z;
        objCameraLink.transform.position = CameraLinkVec;

        //---------------------------------------------------------------------------------------------------------------------------------------
        //カメラリンクオブジェクトを、カメラから向かって向こうに向ける　　（これをやらないと、カメラを上下に動かすときの回転軸がずれるため）
        //---------------------------------------------------------------------------------------------------------------------------------------
        Quaternion tmpQuaternion = Quaternion.LookRotation(objCameraLink.transform.position - this.transform.position, Vector3.up);
        tmpQuaternion.z = 0;
        tmpQuaternion.x = 0;
        objCameraLink.transform.rotation = tmpQuaternion;


        //---------------------------------------------------------------------------------------------------------------------------------------
        //カメラ移動処理 左右
        //---------------------------------------------------------------------------------------------------------------------------------------
        if (Input.GetAxis("RstickLR") != 0)
        {
            //カメラリンクオブジェクトを中心にカメラを回す
            this.transform.RotateAround(objCameraLink.transform.position, Vector3.up, -cameraRotateSpeed * Time.deltaTime * -Input.GetAxis("RstickLR"));
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        //カメラ移動処理 上下　
        //---------------------------------------------------------------------------------------------------------------------------------------
        if (Input.GetButton("L1") == false)//L1が押されていない
        {
            if (Input.GetAxis("RstickUD") != 0)
            {
                //カメラリンクオブジェクトを中心にカメラを回す
                this.transform.RotateAround(objCameraLink.transform.position, objCameraLink.transform.TransformDirection(Vector3.left), cameraRotateSpeed * Time.deltaTime * -Input.GetAxis("RstickUD"));
                if (this.transform.rotation.eulerAngles.x > 85)
                {
                    this.transform.RotateAround(objCameraLink.transform.position, objCameraLink.transform.TransformDirection(Vector3.left), -cameraRotateSpeed * Time.deltaTime * -Input.GetAxis("RstickUD"));
                }
                if (this.transform.rotation.eulerAngles.x < -45)
                {
                    this.transform.RotateAround(objCameraLink.transform.position, objCameraLink.transform.TransformDirection(Vector3.left), -cameraRotateSpeed * Time.deltaTime * -Input.GetAxis("RstickUD"));
                }
            }
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        //カメラ寄せ・引き (L1を押しながら）
        //---------------------------------------------------------------------------------------------------------------------------------------
        if (Input.GetButton("L1") == true)//L1が押されているとき
        {
            if (Input.GetAxis("RstickUD") != 0)
            {
                CameraDistance += Input.GetAxis("RstickUD") * cameraRotateSpeed * Time.deltaTime / 10;
                if (CameraDistance < 2) { CameraDistance = 2; }
                if (CameraDistance > 15) { CameraDistance = 15; }
            }
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        //カメラの追随
        //---------------------------------------------------------------------------------------------------------------------------------------
        Quaternion tmpQuaternion2 = this.transform.rotation;                             //カメラの現在の向きをtmpQuaternionにバックアップ
        this.transform.position = objCameraLink.transform.position;          //オブジェクトの位置に移動させ（向きがカメラリンクオブジェクトと同じになる）
        this.transform.rotation = tmpQuaternion2;                             //向きを戻して
        this.transform.position -= this.transform.forward * CameraDistance; //カメラ距離まで引く


        //---------------------------------------------------------------------------------------------------------------------------------------
        //　キャラクターとカメラの間に障害物があったら障害物の位置にカメラを移動させる
        //  　※プレイヤーのオブジェクトのレイヤーに Ignore Ray を設定すること。
        //---------------------------------------------------------------------------------------------------------------------------------------
        RaycastHit hit;
        if (Physics.Linecast(objCameraLink.transform.position, this.transform.position, out hit))
        {
            this.transform.position = hit.point;
            //            objCamera.transform.position = Vector3.MoveTowards( objCamera.transform.position, hit.point, 10*Time.deltaTime);    
        }


        //---------------------------------------------------------------------------------------------------------------------------------------
        //メインカメラを垂直にする （ぐちゃぐちゃに操作してると、なぜか斜めになるため）
        //---------------------------------------------------------------------------------------------------------------------------------------
        //2020.6.14 やっとできた
        Vector3 tmpRotation = this.transform.rotation.eulerAngles;
        tmpRotation.z = 0;
        this.transform.rotation = Quaternion.Euler(tmpRotation);

    }
}
