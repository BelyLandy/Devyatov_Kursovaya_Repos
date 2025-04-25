using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private PixelPerfectCamera pixelPerfectCamera;
    [Header("Player Targets")]
    [SerializeField] private GameObject[] targets;
    [SerializeField] private bool restrictTargetsToCamView = true;
    [SerializeField] private float borderMargin = 0.05f;
    [Header("Follow Settings")]
    [SerializeField] private float yOffset = 0;
    [HideInInspector] public float additionalYOffset = 0;
    [Header("Damp Settings")]
    [SerializeField] private float DampX = 3f;
    [SerializeField] private float DampY = 1f;
    [Header("View Area")]
    [SerializeField] private float Left;
    [SerializeField] private float Right;
    [SerializeField] private float Top;
    [SerializeField] private float Bottom;
    [SerializeField] private bool showViewAreaRect;
    [Header("BackTracking")]
    [SerializeField] private bool allowBacktracking;
    [SerializeField] private float BackTrackMargin;
    [Header("Level Bound")]
    public LevelBound levelBound;

    private Vector3 centerPos;
    private Vector3 prevPos;
    private float camX;
    private float camY;

    void Start()
    {
        GetPlayerTargets();
        prevPos = transform.position;
        centerPos = GetCenterPosOfAllTargets();
        camX = centerPos.x;
        camY = centerPos.y - yOffset;
    }

    void Update()
    {
        if (targets.Length == 0) return;
        centerPos = GetCenterPosOfAllTargets();
        camX = Mathf.Lerp(prevPos.x, centerPos.x, DampX * Time.deltaTime);
        camY = Mathf.Lerp(prevPos.y, centerPos.y - yOffset, DampY * Time.deltaTime);
        if (float.IsNaN(camX) || float.IsNaN(camY)) return;
        transform.position = new Vector3(
            Mathf.Clamp(camX, Left, Right),
            Mathf.Clamp(camY, Bottom, Top),
            transform.position.z
        );
        prevPos = transform.position;
        if (levelBound != null)
        {
            float halfSize = Camera.main.orthographicSize * Camera.main.aspect;
            float cameraRightEdge = Camera.main.transform.position.x + halfSize;
            float levelBoundPos = levelBound.transform.position.x;
            if (cameraRightEdge > levelBoundPos)
                Camera.main.transform.position = new Vector3(
                    levelBoundPos - halfSize,
                    Camera.main.transform.position.y,
                    Camera.main.transform.position.z
                );
        }
        if (!allowBacktracking)
        {
            float cameraLeftEdge = Camera.main.transform.position.x;
            if (cameraLeftEdge - BackTrackMargin > Left)
                Left = cameraLeftEdge - BackTrackMargin;
        }
        ShowCameraView();
        ShowViewArea();
    }

    private void FixedUpdate()
    {
        if (restrictTargetsToCamView) RestrictTargetsToScreen();
    }

    private void LateUpdate()
    {
        transform.position += Vector3.up * additionalYOffset;
    }

    public void GetPlayerTargets()
    {
        targets = GameObject.FindGameObjectsWithTag("Player");
    }

    public Vector3 GetCenterPosOfAllTargets()
    {
        if (targets.Length == 0) return Vector3.zero;
        centerPos = Vector3.zero;
        int count = 0;
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i])
            {
                centerPos += targets[i].transform.position;
                count++;
            }
        }
        return centerPos / count;
    }

    void RestrictTargetsToScreen()
    {
        foreach (GameObject target in targets)
        {
            Vector3 viewportPosition = Camera.main.WorldToViewportPoint(target.transform.position);
            viewportPosition.x = Mathf.Clamp(viewportPosition.x, borderMargin, 1f - borderMargin);
            viewportPosition.y = Mathf.Clamp(viewportPosition.y, borderMargin, 1f - borderMargin);
            Vector3 clampedWorldPosition = Camera.main.ViewportToWorldPoint(viewportPosition);
            target.transform.position = new Vector2(clampedWorldPosition.x, clampedWorldPosition.y);
        }
    }

    void OnValidate()
    {
        if (Right < Left) Right = Left;
        if (Left > Right) Left = Right;
        if (Top < Bottom) Top = Bottom;
        if (Bottom > Top) Top = Bottom;
    }

    void ShowCameraView()
    {
        if (pixelPerfectCamera == null)
        {
            pixelPerfectCamera = Camera.main.GetComponent<PixelPerfectCamera>();
            return;
        }
        float aspectRatio = (float)pixelPerfectCamera.refResolutionX / pixelPerfectCamera.refResolutionY;
        float cameraHalfHeight = pixelPerfectCamera.refResolutionY / (2f * pixelPerfectCamera.assetsPPU);
        float cameraHalfWidth = cameraHalfHeight * aspectRatio;
        Vector2 topLeft = new Vector2(transform.position.x - cameraHalfWidth, transform.position.y + cameraHalfHeight);
        Vector2 bottomLeft = new Vector2(transform.position.x - cameraHalfWidth, transform.position.y - cameraHalfHeight);
        Vector2 topRight = new Vector2(transform.position.x + cameraHalfWidth, transform.position.y + cameraHalfHeight);
        Vector2 bottomRight = new Vector2(transform.position.x + cameraHalfWidth, transform.position.y - cameraHalfHeight);
        Debug.DrawLine(bottomLeft, topLeft);
        Debug.DrawLine(topLeft, topRight);
        Debug.DrawLine(topRight, bottomRight);
        Debug.DrawLine(bottomRight, bottomLeft);
    }

    void ShowViewArea()
    {
        float aspectRatio = (float)pixelPerfectCamera.refResolutionX / pixelPerfectCamera.refResolutionY;
        float cameraHalfHeight = pixelPerfectCamera.refResolutionY / (2f * pixelPerfectCamera.assetsPPU);
        float cameraHalfWidth = cameraHalfHeight * aspectRatio;
        Vector2 topLeft = new Vector2(Left - cameraHalfWidth, Top + cameraHalfHeight);
        Vector2 topRight = new Vector2(Right + cameraHalfWidth, Top + cameraHalfHeight);
        Vector2 bottomLeft = new Vector2(Left - cameraHalfWidth, Bottom - cameraHalfHeight);
        Vector2 bottomRight = new Vector2(Right + cameraHalfWidth, Bottom - cameraHalfHeight);
        Debug.DrawLine(bottomLeft, topLeft, Color.red);
        Debug.DrawLine(topLeft, topRight, Color.red);
        Debug.DrawLine(topRight, bottomRight, Color.red);
        Debug.DrawLine(bottomRight, bottomLeft, Color.red);
    }
}
