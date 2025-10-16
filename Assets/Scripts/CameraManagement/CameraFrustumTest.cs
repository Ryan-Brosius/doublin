using UnityEngine;

public class CameraFrustumTest : MonoBehaviour
{
    public float horizontalOblique = 0.5f;
    public float verticalOblique = 0.5f;

    Camera camera;

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    private void Start()
    {
        SetObliqueness();
    }

    void SetObliqueness()
    {
        Matrix4x4 mat = camera.projectionMatrix;

        mat[0, 2] = horizontalOblique;
        mat[1, 2] = verticalOblique;

        camera.projectionMatrix = mat;
    }
}
