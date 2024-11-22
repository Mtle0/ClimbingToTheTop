using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : MonoBehaviour
{
    private StarterAssetsInputs _input;

    [Header("Cine machine Settings")]
    public GameObject cineMachineCameraTarget;
    public float topClamp = 70.0f;
    public float bottomClamp = -30.0f;
    public float cameraAngleOverride = 0.0f;
    public bool lockCameraPosition = false;

    private float _cineMachineTargetYaw, _cineMachineTargetPitch;
    private const float Threshold = 0.01f;
    public GameObject MainCamera { get; private set; }

    public float TargetRotation { get; set; }


    private PlayerInput _playerInput;
    private bool IsCurrentDeviceMouse =>
#if ENABLE_INPUT_SYSTEM
            _playerInput.currentControlScheme == "KeyboardMouse";
#else
            false;
#endif

    private void Awake()
    {
        MainCamera = MainCamera ?? GameObject.FindGameObjectWithTag("MainCamera");
    }

    private void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();
        _playerInput = GetComponent<PlayerInput>();
    }

    public void LateUpdate() => RotateCamera();

    private void RotateCamera()
    {
        if (_input.look.sqrMagnitude >= Threshold && !lockCameraPosition)
        {
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
            _cineMachineTargetYaw += _input.look.x * deltaTimeMultiplier;
            _cineMachineTargetPitch += _input.look.y * deltaTimeMultiplier;
        }

        _cineMachineTargetYaw = ClampAngle(_cineMachineTargetYaw, float.MinValue, float.MaxValue);
        _cineMachineTargetPitch = ClampAngle(_cineMachineTargetPitch, bottomClamp, topClamp);

        cineMachineCameraTarget.transform.rotation = Quaternion.Euler(_cineMachineTargetPitch + cameraAngleOverride, _cineMachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f) angle += 360f;
        if (angle > 360f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }
}
