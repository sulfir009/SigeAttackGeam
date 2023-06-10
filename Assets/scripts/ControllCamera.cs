using System.Collections;
using UnityEngine;

public class ControllCamera : MonoBehaviour
{
    public Transform playerTransform;
    public Transform arenaCenter; // ����� �����, ������ �������� ������ ���������
    public float rotationSpeed = 1f;
    public Vector3 offset;
    public float smoothSpeed = 0.125f;
    public float cameraHeight = 5f;
    public RectTransform joystickArea;
    public float rotationTime = 3f; // ����� �������� ������ ������ �����
    private bool isRotating = true; // ����, ������� ������������ �������� ������
    public float rotationCameraHeight = 10f; // ������ ������ �� ����� ��������
public Vector3 rotationCameraOffset; // �������� ������ �� ����� ��������



    private void Start()
    {
        StartCoroutine(RotateAroundArena());
    }

    private IEnumerator RotateAroundArena()
    {
        float startTime = Time.time;
     

        while (Time.time - startTime < rotationTime)
        {
            // �������� ������ ������ �����
            float rotationAmount = rotationSpeed * Time.deltaTime;
            transform.RotateAround(arenaCenter.position, Vector3.up, rotationAmount);

            // ���������� ��������� ������
            Vector3 desiredPosition = arenaCenter.position + rotationCameraOffset + new Vector3(0, rotationCameraHeight, 0);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;

            yield return null;
        }
        
        isRotating = false;
    }

    private void Update()
    {
        if (isRotating)
        {
            return;
        }

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // ���������, �������� �� ������� ������ ������� ���������
            if (RectTransformUtility.RectangleContainsScreenPoint(joystickArea, touch.position))
            {
                // ���� ��� ���, �� �� �������� ������
                return;
            }

            if (touch.phase == TouchPhase.Moved)
            {
                float rotationAmount = touch.deltaPosition.x * rotationSpeed * Time.deltaTime;
                transform.RotateAround(playerTransform.position, Vector3.up, rotationAmount);
            }
        }
    }

    private void FixedUpdate()
    {
        if (isRotating)
        {
            return;
        }

        Vector3 desiredPosition = playerTransform.position + offset + new Vector3(0, cameraHeight, -10f);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
