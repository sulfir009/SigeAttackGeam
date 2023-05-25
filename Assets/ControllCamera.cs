

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ControllCamera : MonoBehaviour
{
    public Transform player;
    public float rotationSpeed = 1f;
    public Vector3 offset;
    public float smoothSpeed = 0.125f;
    public float cameraHeight = 5f;
    public RectTransform joystickArea;
    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }
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
                transform.RotateAround(player.position, Vector3.up, rotationAmount);
            }
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {

        Vector3 desiredPosition = player.position + new Vector3(0, cameraHeight, -10f);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
