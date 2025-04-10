using UnityEngine;

public class SaveFileBook_Click : MonoBehaviour, IClickable
{
    public void OnClick()
    {
        SceneLoader.Load(SceneLoader.Scene.MenuScene);
    }
}
