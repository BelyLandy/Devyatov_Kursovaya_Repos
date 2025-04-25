using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Surface : MonoBehaviour
{
    public string footstepSFX = "";

    private void OnValidate()
    {
        Collider2D col2D = GetComponent<Collider2D>();
        if (col2D && !col2D.isTrigger)
        {
            col2D.isTrigger = true;
            Debug.Log("Set collider 2D of '" + gameObject.name + "' to trigger");
        }

        int surfaceLayer = LayerMask.NameToLayer("Surface");
        if (gameObject.layer != surfaceLayer && surfaceLayer != -1)
        {
            gameObject.layer = surfaceLayer;
            Debug.Log(gameObject.name + " was set to Surface layer");
        }
    }
}