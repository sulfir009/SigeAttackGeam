using UnityEngine;

public class ControllCamera : MonoBehaviour
{
    public Transform playerTransform;
    public float rotationSpeed = 1f;
    public Vector3 offset;
    public float smoothSpeed = 0.125f;
    public float cameraHeight = 5f;
    public RectTransform joystickArea;

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Проверьте, началось ли касание внутри области джойстика
            if (RectTransformUtility.RectangleContainsScreenPoint(joystickArea, touch.position))
            {
                // Если это так, то не вращайте камеру
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
        Vector3 desiredPosition = playerTransform.position + offset + new Vector3(0, cameraHeight, -10f);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}