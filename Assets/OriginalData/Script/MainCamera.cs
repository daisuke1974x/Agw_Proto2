using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{

    public GameObject objPlayer;
    private GameObject objCameraLink;

    //�J�����֘A
    float CameraDistance = 5;  //�I�u�W�F�N�g����J�����̋����̏����l
    float cameraRotateSpeed = 150;  //�J�������񂷑���
                                    //private Quaternion tmpQuaternion;

    //����֎~�t���O�i�ʃX�N���v�g���瑀�삷��j
    public bool isControllEnabled = true;



    // Start is called before the first frame update
    void Start()
    {
        //�J���������ʒu
        this.transform.position = objPlayer.transform.position;
        this.transform.rotation = objPlayer.transform.rotation;
        this.transform.position += this.transform.forward * -CameraDistance;
        this.transform.RotateAround(objPlayer.transform.position, objPlayer.transform.TransformDirection(Vector3.left), -30);

        //�J���������N�I�u�W�F�N�g�̐���
        objCameraLink = new GameObject("CameraLink");  //GameObject objCameraLink = new GameObject("CameraLink");  �Ƃ���ƁAUpdate��Null�ɂȂ��B2021/5/23
        objCameraLink.transform.position = objPlayer.transform.position;
        objCameraLink.transform.rotation = objPlayer.transform.rotation;

    }

    // Update is called once per frame
    void Update()
    {

        if (isControllEnabled == false) return;

        //---------------------------------------------------------------------------------------------------------------------------------------
        //�v���C���[�I�u�W�F�N�g�ɍ��킹�āAobjCameraLink��ǐ�������
        //---------------------------------------------------------------------------------------------------------------------------------------
        Vector3 CameraLinkVec = objCameraLink.transform.position;
        CameraLinkVec.x = objPlayer.transform.position.x;
        CameraLinkVec.y = objPlayer.transform.position.y + 1f;//�`���g�l
        CameraLinkVec.z = objPlayer.transform.position.z;
        objCameraLink.transform.position = CameraLinkVec;

        //---------------------------------------------------------------------------------------------------------------------------------------
        //�J���������N�I�u�W�F�N�g���A�J��������������Č������Ɍ�����@�@�i��������Ȃ��ƁA�J�������㉺�ɓ������Ƃ��̉�]��������邽�߁j
        //---------------------------------------------------------------------------------------------------------------------------------------
        Quaternion tmpQuaternion = Quaternion.LookRotation(objCameraLink.transform.position - this.transform.position, Vector3.up);
        tmpQuaternion.z = 0;
        tmpQuaternion.x = 0;
        objCameraLink.transform.rotation = tmpQuaternion;


        //---------------------------------------------------------------------------------------------------------------------------------------
        //�J�����ړ����� ���E
        //---------------------------------------------------------------------------------------------------------------------------------------
        if (Input.GetAxis("RstickLR") != 0)
        {
            //�J���������N�I�u�W�F�N�g�𒆐S�ɃJ��������
            this.transform.RotateAround(objCameraLink.transform.position, Vector3.up, -cameraRotateSpeed * Time.deltaTime * -Input.GetAxis("RstickLR"));
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        //�J�����ړ����� �㉺�@
        //---------------------------------------------------------------------------------------------------------------------------------------
        if (Input.GetButton("L1") == false)//L1��������Ă��Ȃ�
        {
            if (Input.GetAxis("RstickUD") != 0)
            {
                //�J���������N�I�u�W�F�N�g�𒆐S�ɃJ��������
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
        //�J�����񂹁E���� (L1�������Ȃ���j
        //---------------------------------------------------------------------------------------------------------------------------------------
        if (Input.GetButton("L1") == true)//L1��������Ă���Ƃ�
        {
            if (Input.GetAxis("RstickUD") != 0)
            {
                CameraDistance += Input.GetAxis("RstickUD") * cameraRotateSpeed * Time.deltaTime / 10;
                if (CameraDistance < 2) { CameraDistance = 2; }
                if (CameraDistance > 15) { CameraDistance = 15; }
            }
        }

        //---------------------------------------------------------------------------------------------------------------------------------------
        //�J�����̒ǐ�
        //---------------------------------------------------------------------------------------------------------------------------------------
        Quaternion tmpQuaternion2 = this.transform.rotation;                             //�J�����̌��݂̌�����tmpQuaternion�Ƀo�b�N�A�b�v
        this.transform.position = objCameraLink.transform.position;          //�I�u�W�F�N�g�̈ʒu�Ɉړ������i�������J���������N�I�u�W�F�N�g�Ɠ����ɂȂ�j
        this.transform.rotation = tmpQuaternion2;                             //������߂���
        this.transform.position -= this.transform.forward * CameraDistance; //�J���������܂ň���


        //---------------------------------------------------------------------------------------------------------------------------------------
        //�@�L�����N�^�[�ƃJ�����̊Ԃɏ�Q�������������Q���̈ʒu�ɃJ�������ړ�������
        //  �@���v���C���[�̃I�u�W�F�N�g�̃��C���[�� Ignore Ray ��ݒ肷�邱�ƁB
        //---------------------------------------------------------------------------------------------------------------------------------------
        RaycastHit hit;
        if (Physics.Linecast(objCameraLink.transform.position, this.transform.position, out hit))
        {
            this.transform.position = hit.point;
            //            objCamera.transform.position = Vector3.MoveTowards( objCamera.transform.position, hit.point, 10*Time.deltaTime);    
        }


        //---------------------------------------------------------------------------------------------------------------------------------------
        //���C���J�����𐂒��ɂ��� �i�����Ⴎ����ɑ��삵�Ă�ƁA�Ȃ����΂߂ɂȂ邽�߁j
        //---------------------------------------------------------------------------------------------------------------------------------------
        //2020.6.14 ����Ƃł���
        Vector3 tmpRotation = this.transform.rotation.eulerAngles;
        tmpRotation.z = 0;
        this.transform.rotation = Quaternion.Euler(tmpRotation);

    }
}
