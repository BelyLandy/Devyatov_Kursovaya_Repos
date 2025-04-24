using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public enum Scene
    {
        ChooseSaveFile,
        MenuScene,
        GameScene_Main,
        SettingsScene,
        VictoryScene
    }

    public static void Load(Scene scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }
}

