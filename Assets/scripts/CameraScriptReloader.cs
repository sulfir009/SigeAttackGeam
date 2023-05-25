using UnityEditor;
using UnityEngine;

public class CameraScriptReloader : MonoBehaviour
{
    public Transform newPlayerTransform;
    public Vector3 newOffset;

    private ControllCamera controllCamera;

    private void Start()
    {
        controllCamera = GetComponent<ControllCamera>();
        ReloadCameraScript();
    }

    private void ReloadCameraScript()
    {
        controllCamera.player = newPlayerTransform;
        controllCamera.offset = newOffset;
    }
}
