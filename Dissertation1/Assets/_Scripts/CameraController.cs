using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraState
{
    AtPosition,
    MovingToPosition
}

public class CameraController : MonoBehaviour
{

    public static CameraController instance;

    [Header("Controls")]
    [SerializeField] float movementSpeed;

    [Header("Room Positions")]
    [SerializeField] Transform room1Pos;
    [SerializeField] Transform room2Pos;
    [SerializeField] Transform room3Pos;

    [SerializeField] Vector3 targetPosition;
    [SerializeField] CameraState currentState;

    private void Awake()
    {
        instance = this;
    }

    public void MoveLeft()
    {
        if (currentState == CameraState.MovingToPosition) return;
        if (GameManager.instance.currentRoom == 3) return;
        GameManager.instance.currentRoom++;
        MoveToRoom();
    }

    public void MoveRight()
    {
        if (currentState == CameraState.MovingToPosition) return;
        if (GameManager.instance.currentRoom == 1) return;
        GameManager.instance.currentRoom--;
        MoveToRoom();
    }

    void MoveToRoom()
    {
        if (GameManager.instance.currentRoom == 1)
        {
            targetPosition = room1Pos.position;
        }
        else if (GameManager.instance.currentRoom == 2)
        {
            targetPosition = room2Pos.position;
        }
        else
        {
            targetPosition = room3Pos.position;
        }
        currentState = CameraState.MovingToPosition;
    }

    private void Update()
    {
        if (currentState == CameraState.MovingToPosition)
        {
            transform.localPosition = transform.localPosition + ((targetPosition - transform.localPosition).normalized * (movementSpeed * Time.deltaTime));
            if (Vector3.Distance(transform.localPosition, targetPosition) < 0.01f)
            {
                currentState = CameraState.AtPosition;
                GameManager.instance.SetRoomObjs();
            }
        }
    }
}