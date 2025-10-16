using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

/*  Owner: Ryan
 *  
 *  Manager that controls how the camera reacts in the scene. Quite simple as that, it can set up the split-screen view
 *  and will do fancy camera tricks when we need
 *  
 *  I need to comment later :')
 *  Im tired
 */
public class CameraManager : SingletonMonobehavior<CameraManager>
{
    [Header("Unity Cameras")]
    [SerializeField] private GameObject combinedCameraGameobject;
    [SerializeField] private GameObject grimoireCameraGameobject;
    [SerializeField] private GameObject staffCameraGameobject;
    [SerializeField] private GameObject targetGroupCameraGameobject;

    [Header("Virtual Cameras")]
    [SerializeField] private CinemachineCamera combinedCameraCinemachine;
    [SerializeField] private CinemachineCamera grimoireCameraCinemachine;
    [SerializeField] private CinemachineCamera staffCameraCinemachine;
    [SerializeField] private CinemachineCamera targetGroupCameraCinemachine;

    [Header("Targeting")]
    [SerializeField] Transform grimoireTransform;
    [SerializeField] Transform staffTransform;

    [Header("Split / Combine Settings")]
    [Tooltip("Distance under which the separated players get a single camera.")]
    [SerializeField] float combineDistanceThreshold = 8f;
    [Tooltip("Hysteresis margin to prevent flip-flopping when players hover near threshold.")]
    [SerializeField] private float thresholdHysteresis = 1.5f;
    [Tooltip("Blend duration for CinemachineBrain when optionally overriding.")]
    [SerializeField] float transitionBlendSeconds = 0.35f;

    [Header("Composer Offsets")]
    [SerializeField] Vector3 combinedOffsetDefault = Vector3.zero;
    [SerializeField] Vector3 combinedOffsetWhenGrimoireUI = new Vector3(2f, 0f, 0f);

    private Coroutine _transitionCoroutine;
    LazyDependency<GoblinStateManager> _GoblinStateManager;
    private bool isUsingSplit = false;

    private void Start()
    {
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
        ApplyCameraState(_GoblinStateManager.Value.CurrentGoblinState.Value);

        if (_GoblinStateManager.Value.CurrentGoblinState.Value == GoblinState.Separated)
        {
            float distance = Vector3.Distance(grimoireTransform.position, staffTransform.position);
            bool shouldBeSplit = ShouldUseSplit(distance);

            if (shouldBeSplit != isUsingSplit)
            {
                if (_transitionCoroutine != null) StopCoroutine(_transitionCoroutine);
                _transitionCoroutine = StartCoroutine(SwitchSplitCombine(!shouldBeSplit));
            }
        }
    }

    // Pretty sure this is broken :/
    private bool ShouldUseSplit(float distance)
    {
        Debug.Log(isUsingSplit);
        Debug.Log(distance);

        if (!isUsingSplit && distance > combineDistanceThreshold + thresholdHysteresis)
            return true;

        if (isUsingSplit && distance < combineDistanceThreshold - thresholdHysteresis)
            return false;

        return isUsingSplit;
    }

    private void ApplyCameraState(GoblinState state)
    {
        combinedCameraGameobject.SetActive(false);
        targetGroupCameraGameobject.SetActive(false);
        grimoireCameraGameobject.SetActive(false);
        staffCameraGameobject.SetActive(false);

        switch (state)
        {
            case GoblinState.Separated:
                HandleSeparatedState();
                break;

            case GoblinState.StaffTop:
            case GoblinState.BookTop:
                combinedCameraGameobject.SetActive(true);
                break;
        }
    }

    private void HandleSeparatedState()
    {
        float distance = Vector3.Distance(grimoireTransform.position, staffTransform.position);

        if (distance <= combineDistanceThreshold)
        {
            targetGroupCameraGameobject.SetActive(true);
            isUsingSplit = false;
        }
        else
        {
            grimoireCameraGameobject.SetActive(true);
            staffCameraGameobject.SetActive(true);
            isUsingSplit = true;
        }
    }

    private IEnumerator SwitchSplitCombine(bool toCombined)
    {
        if (toCombined)
        {
            grimoireCameraGameobject.SetActive(false);
            staffCameraGameobject.SetActive(false);
            targetGroupCameraGameobject.SetActive(true);
            isUsingSplit = false;
        }
        else
        {
            targetGroupCameraGameobject.SetActive(false);
            grimoireCameraGameobject.SetActive(true);
            staffCameraGameobject.SetActive(true);
            isUsingSplit = true;
        }

        yield return new WaitForSeconds(transitionBlendSeconds);
    }
}
