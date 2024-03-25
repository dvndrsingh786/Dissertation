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
    [Header("Controls")]
    [SerializeField] float movementSpeed;

    [Header("Room Positions")]
    [SerializeField] Transform room1Pos;
    [SerializeField] Transform room2Pos;
    [SerializeField] Transform room3Pos;

    [SerializeField] Vector3 targetPosition;
    [SerializeField] CameraState currentState;
    [SerializeField] int abc = 1;

    [ContextMenu("Room")]
    void MoveToRoom()
    {
        int roomNumber = abc;
        if (roomNumber == 1)
        {
            targetPosition = room1Pos.position;
        }
        else if (roomNumber == 2)
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
            }
        }
    }
}