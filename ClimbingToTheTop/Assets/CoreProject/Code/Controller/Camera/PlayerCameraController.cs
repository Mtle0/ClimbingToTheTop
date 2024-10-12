using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : MonoBehaviour
{
    private StarterAssetsInputs input;

    [Header("Cinemachine Settings")]
    public GameObject CinemachineCameraTarget;
    public float TopClamp = 70.0f;
    public float BottomClamp = -30.0f;
    public float CameraAngleOverride = 0.0f;
    public bool LockCameraPosition = false;

    private float cinemachineTargetYaw, cinemachineTargetPitch;
    private const float threshold = 0.01f;
    private GameObject mainCamera;
    public GameObject MainCamera { get { return mainCamera; } }
    private float targetRotation;
    public float TargetRotation { get { return targetRotation; } set { targetRotation = value; } }
   


    private PlayerInput playerInput;
    private bool IsCurrentDeviceMouse =>
#if ENABLE_INPUT_SYSTEM
            playerInput.currentControlScheme == "KeyboardMouse";
#else
            false;
#endif

    private void Awake()
    {
        mainCamera = mainCamera ?? GameObject.FindGameObjectWithTag("MainCamera");
    }

    void Start()
    {
        input = GetComponent<StarterAssetsInputs>();
        playerInput = GetComponent<PlayerInput>();
    }

    public void LateUpdate() => RotateCamera();

    public void RotateCamera()
    {
        if (input.look.sqrMagnitude >= threshold && !LockCameraPosition)
        {
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
            cinemachineTargetYaw += input.look.x * deltaTimeMultiplier;
            cinemachineTargetPitch += input.look.y * deltaTimeMultiplier;
        }

        cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
        cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, BottomClamp, TopClamp);

        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(cinemachineTargetPitch + CameraAngleOverride, cinemachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f) angle += 360f;
        if (angle > 360f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }
}
