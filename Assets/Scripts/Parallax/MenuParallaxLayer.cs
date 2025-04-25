using UnityEngine;

public class EndlessHorizontalScroller : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] segments = new SpriteRenderer[3];
    [Tooltip("Скорость движения (+ вправо, – влево), юн/сек")]
    [SerializeField] private float speed = 3f;

    private float segWidth;
    private Camera cam;

    void Awake()
    {
        if (segments.Length != 3)
        {
            Debug.LogError("Нужно ровно 3 спрайта!"); enabled = false; return;
        }
        segWidth = segments[0].bounds.size.x;
        cam = Camera.main;
    }

    void Update()
    {
        float dx = speed * Time.deltaTime;

        foreach (var s in segments)
            s.transform.Translate(Vector3.right * dx, Space.World);

        float leftmostX = segments[0].transform.position.x;
        foreach (var s in segments)
            if (s.transform.position.x < leftmostX) leftmostX = s.transform.position.x;

        float camRight = cam.transform.position.x + cam.orthographicSize * cam.aspect;

        foreach (var s in segments)
        {
            float spriteLeftEdge = s.transform.position.x - segWidth * 0.5f;
            if (spriteLeftEdge > camRight)
            {
                s.transform.position = new Vector3(
                    leftmostX - segWidth,
                    s.transform.position.y,
                    s.transform.position.z);

                leftmostX = s.transform.position.x;    // обновляем, чтобы второй «проскок» в тот же кадр тоже сработал
            }
        }
    }
}