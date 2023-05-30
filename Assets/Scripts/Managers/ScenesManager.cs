using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
  public static ScenesManager Instance;

  private void Awake()
  {
    Instance = this;
  }

  public enum Scene
  {
    MainScene,
    PlacesScene,
    SettingsScene,
    Sample
  }

  public void LoadScene(Scene scene)
  {
    SceneManager.LoadScene(scene.ToString());
  }

  public void LoadMainScene()
  {
    SceneManager.LoadScene(Scene.MainScene.ToString());
  }
}
