using UnityEngine;
using static SceneLoader;

public class Loader : MonoBehaviour
{
    public void IntroScene()
    {
        Load(Scene.ChooseSaveFile);
    }
    
    public void MenuScene()
    {
        Load(Scene.MenuScene);
    }
    
    public void GameScene()
    {
        Load(Scene.GameScene_Main);
    }
    
    public void SettingsScene()
    {
        Load(Scene.SettingsScene);
    }
    
    public void VictoryScene()
    {
        Load(Scene.VictoryScene);
    }
    
    public void GameScene_2()
    {
        Load(Scene.GameScene_Main_2);
    }
    
    public void GameScene_3()
    {
        Load(Scene.GameScene_Main_2);
    }
}
