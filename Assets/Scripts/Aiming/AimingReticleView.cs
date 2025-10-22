using UnityEngine;

public class AimingReticleView : MonoBehaviour
{
    [SerializeField] private RectTransform reticleTransform;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private AimingSettings settings;

    private LazyDependency<AimingManager> _AimingManager;

    private void Update()
    {
        var target = _AimingManager.Value.CurrentTarget;

        if (target == null)
        {
            reticleTransform.gameObject.SetActive(true);
            reticleTransform.position = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        }
        else
        {
            Vector3 screenPos = playerCamera.WorldToScreenPoint(target.position);
            reticleTransform.position = screenPos;
            reticleTransform.Rotate(Vector3.forward, settings.reticleSpinSpeed * Time.deltaTime);
        }
    }
}
