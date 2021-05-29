using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharaMove : MonoBehaviour
{
    //���[���̈ړ��̐��l�����ꂼ��̕ϐ��Ő錾���܂��B
    const int MinLane = -2;
    const int MaxLane = 2;
    const float LaneWidth = 1.0f;

    //CharacterController�^��ϐ�controller�Ő錾���܂��B
    CharacterController controller;
    //Animator�^��ϐ�animator�Ő錾���܂��B
    Animator animator;

    //���ꂼ��̍��W���O�Ő錾���܂��B
    Vector3 moveDirection = Vector3.zero;
    //int�^��ϐ�targetLane�Ő錾���܂��B
    int targetLane;

    //���ꂼ��̃p�����[�^�[�̐ݒ��Inspector�ŕς���l�ɂ��܂��B
    public float gravity;
    public float speedZ;
    public float speedX;
    public float speedJump;
    public float accelerationZ;

    void Start()
    {
        //GetComponent��CharacterControllerwp�擾���ĕϐ�controllse�ŎQ�Ƃ��܂��B
        controller = GetComponent<CharacterController>();
        //GetComponent��Animator���擾���ĕϐ�animator�ŎQ�Ƃ��܂��B
        animator = GetComponent<Animator>();


    }


    void Update()
    {

        Debug.Log("controller.isGrounded:" + controller.isGrounded.ToString()); ;
        //���ꂼ��̖�󂪉����ꂽ�炻�ꂼ��̊֐������s���܂��B
        if (Input.GetKeyDown(KeyCode.LeftArrow)) MoveToLeft();
        if (Input.GetKeyDown(KeyCode.RightArrow)) MoveToRight();
        if (Input.GetKeyDown(KeyCode.Space)) Jump();


        float acceleratedZ = moveDirection.z + (accelerationZ * Time.deltaTime);
        moveDirection.z = Mathf.Clamp(acceleratedZ, 0, speedZ);

        float ratioX = (targetLane * LaneWidth - transform.position.x) / LaneWidth;
        moveDirection.x = ratioX * speedX;

        moveDirection.y -= gravity * Time.deltaTime;

        Vector3 globalDirection = transform.TransformDirection(moveDirection);
        controller.Move(globalDirection * Time.deltaTime);

        if (controller.isGrounded) moveDirection.y = 0;

        animator.SetBool("run", moveDirection.z > 0.0f);

    }

    //�V����������֐��̂��ꂼ��̏����B
    public void MoveToLeft()
    {
        Debug.Log("MoveToLeft");
        if (controller.isGrounded && targetLane > MinLane) targetLane--;
    }

    public void MoveToRight()
    {
        Debug.Log("MoveToRight");
        if (controller.isGrounded && targetLane < MaxLane) targetLane++;
    }

    public void Jump()
    {
        Debug.Log("Jump");
        if (controller.isGrounded)
        {
            moveDirection.y = speedJump;

            animator.SetTrigger("jump");
        }
    }

}