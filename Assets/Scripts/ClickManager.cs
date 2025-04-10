using UnityEngine;

public class ClickManager : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
            if (hit.collider != null)
            {
                IClickable[] clickables = hit.collider.GetComponentsInParent<IClickable>();
                foreach (IClickable clickable in clickables)
                {
                    clickable.OnClick();
                }
            }
        }
    }
}