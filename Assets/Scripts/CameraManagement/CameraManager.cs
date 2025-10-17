using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

/*  Owner: Ryan
 *  
 *  Manager that controls how the camera reacts in the scene. Quite simple as that, it can set up the split-screen view
 *  and will do fancy camera tricks when we need
 *  
 */
public class CameraManager : SingletonMonobehavior<CameraManager>
{
    [Header("Unity Cameras")]
    [SerializeField] private Camera combinedCamera;
    [SerializeField] private Camera grimoireCamera;
    [SerializeField] private Camera staffCamera;
    private List<Camera> cameras;

    [Header("Virtual Cameras")]
    [SerializeField] private CinemachineCamera combinedCameraCinemachine;
    [SerializeField] private CinemachineCamera grimoireCameraCinemachine;
    [SerializeField] private CinemachineCamera staffCameraCinemachine;
    [SerializeField] private CinemachineCamera targetGroupCameraCinemachine;

    [Header("Cinemachine Input Axis Controllers")]
    [SerializeField] private CinemachineInputAxisController grimoireInputAxisController;
    [SerializeField] private CinemachineInputAxisController staffInputAxisController;
    [SerializeField] private CinemachineInputAxisController targetGroupInputAxisController;


    [Header("Targeting")]
    [SerializeField] Transform grimoireTransform;
    [SerializeField] Transform staffTransform;

    [Header("Split / Combine Settings")]
    [Tooltip("Distance under which the separated players get a single camera.")]
    [SerializeField] float combineDistanceThreshold = 8f;
    [Tooltip("Hysteresis margin to prevent flip-flopping when players hover near threshold.")]
    [SerializeField] private float thresholdHysteresis = 1.5f;
    [Tooltip("Blend duration for CinemachineBrain moving to correct transform")]
    [SerializeField] float transitionBlendSeconds = .5f;

    private Coroutine _transitionCoroutine;
    LazyDependency<GoblinStateManager> _GoblinStateManager;
    LazyDependency<InputManager> _InputManager;
    private bool isUsingSplit = false;

    const int LOW_PRIORITY = 10;
    const int HIGH_PRIORITY = 20;

    #region Unity Functions

    protected override void Awake()
    {
        base.Awake();

        cameras = new List<Camera>()
        {
            combinedCamera,
            grimoireCamera,
            staffCamera,
        };

        List<CinemachineBrain> cameraBrains = cameras
            .Select(cam => cam.GetComponent<CinemachineBrain>())
            .ToList();

        // Setting each brain to have a transition duration of blend seconds
        foreach (var brain in cameraBrains)
        {
            brain.DefaultBlend.Time = transitionBlendSeconds;
        }
    }

    private void Start()
    {
        // Apply the current camera state
        ApplyCameraState(_GoblinStateManager.Value.CurrentGoblinState.Value);

        // Set split cameras to correct start settings
        SetObliqueness(grimoireCamera, -1, 0);
        SetObliqueness(staffCamera, 1, 0);

        // Subscribe so we can change whenever goblin state updates
        _GoblinStateManager.Value.CurrentGoblinState.Subscribe(ApplyCameraState);
    }

    private void OnEnable()
    {
        if (_GoblinStateManager.IsResolvable())
            _GoblinStateManager.Value.CurrentGoblinState.Subscribe(ApplyCameraState);
    }

    private void OnDisable()
    {
        _GoblinStateManager.Value.CurrentGoblinState.Unsubscribe(ApplyCameraState);
    }

    private void Update()
    {
        //ApplyCameraState(_GoblinStateManager.Value.CurrentGoblinState.Value);

        // Grabbing distance between goblins and doing some transition checks
        if (_GoblinStateManager.Value.CurrentGoblinState.Value == GoblinState.Separated)
        {
            float distance = Vector3.Distance(grimoireTransform.position, staffTransform.position);
            bool shouldBeSplit = ShouldUseSplit(distance);

            // If we need to split/unsplit and currently not using it
            if (shouldBeSplit != isUsingSplit)
            {
                isUsingSplit = shouldBeSplit;
                if (_transitionCoroutine != null) StopCoroutine(_transitionCoroutine);
                _transitionCoroutine = StartCoroutine(SwitchSplitCombine(!shouldBeSplit));
            }
        }
    }

    #endregion

    #region Private Helpers

    private bool ShouldUseSplit(float distance)
    {
        if (!isUsingSplit && distance > combineDistanceThreshold + thresholdHysteresis)
            return true;

        if (isUsingSplit && distance < combineDistanceThreshold - thresholdHysteresis)
            return false;

        return isUsingSplit;
    }

    private void ApplyCameraState(GoblinState state)
    {
        // Gets the correct camera state whenever the goblin state observable is changed

        switch (state)
        {
            case GoblinState.Separated:
                HandleSeparatedState();
                break;

            case GoblinState.StaffTop:
            case GoblinState.BookTop:
                combinedCameraCinemachine.Priority = HIGH_PRIORITY;
                grimoireCameraCinemachine.Priority = LOW_PRIORITY;
                staffCameraCinemachine.Priority = LOW_PRIORITY;
                targetGroupCameraCinemachine.Priority = LOW_PRIORITY;
                break;
        }
    }

    private void HandleSeparatedState()
    {
        DisplayCameraInFront(grimoireCamera, staffCamera);
        isUsingSplit = false;
    }

    private IEnumerator SwitchSplitCombine(bool combineCameras)
    {
        // Handles how the camera reacts when the camera either needs to split or combine

        float elapsed = 0f;

        // Get current obliqueness values of camera transforms
        float grimoireStart = GetCurrentObliqueness(grimoireCamera);
        float staffStart = GetCurrentObliqueness(staffCamera);

        // Determine what values obliqueness needs to be tweened to
        // 0 is completely center of view, -1/1 is sliced in half view
        float grimoireTarget = combineCameras ? -1f : 0f;
        float staffTarget = combineCameras ? 1f : 0f;

        // Setting camera priority for camera lerp towards target
        grimoireCameraCinemachine.Priority = combineCameras ? LOW_PRIORITY : HIGH_PRIORITY;
        staffCameraCinemachine.Priority = combineCameras ? LOW_PRIORITY : HIGH_PRIORITY;
        targetGroupCameraCinemachine.Priority = combineCameras ? HIGH_PRIORITY : LOW_PRIORITY;

        // Display split cameras in the front
        DisplayCameraInFront(grimoireCamera, staffCamera);

        // Change the obliqueness matrix of the cameras here
        // Also changes the positions of the cameras
        while (elapsed < transitionBlendSeconds)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / transitionBlendSeconds);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            SetObliqueness(grimoireCamera, Mathf.Lerp(grimoireStart, grimoireTarget, t), 0f);
            SetObliqueness(staffCamera, Mathf.Lerp(staffStart, staffTarget, t), 0f);

            yield return null;
        }

        // Making sure values are finalized
        SetObliqueness(grimoireCamera, grimoireTarget, 0f);
        SetObliqueness(staffCamera, staffTarget, 0f);
    }

    private void DisplayCameraInFront(params Camera[] frontCams)
    {
        foreach (Camera camera in cameras)
        {
            camera.depth = 0;
        }

        foreach (Camera camera in frontCams)
        {
            camera.depth = 1;
        }
    }

    void SetObliqueness(Camera camera, float horizontalOblique, float verticalOblique)
    {
        Matrix4x4 mat = camera.projectionMatrix;

        mat[0, 2] = horizontalOblique;
        mat[1, 2] = verticalOblique;

        camera.projectionMatrix = mat;
    }

    float GetCurrentObliqueness(Camera cam)
    {
        return cam.projectionMatrix[0, 2];
    }

    #endregion
}
