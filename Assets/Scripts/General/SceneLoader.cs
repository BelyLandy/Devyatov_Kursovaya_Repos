using UnityEngine.SceneManagement;

public static class SceneLoader
{
    public enum Scene
    {
        ChooseSaveFile,
        MenuScene,
        GameScene_Main,
        SettingsScene,
        VictoryScene,
        GameScene_Main_2,
        GameScene_Main_3
    }

    public static void Load(Scene scene)
    {
        SceneManager.LoadScene(scene.ToString());
    }
}

