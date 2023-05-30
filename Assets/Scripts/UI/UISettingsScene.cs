using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISettingsScene : MonoBehaviour
{
  [SerializeField] Button _homeButton;
  
  void Start()
  {
    _homeButton.onClick.AddListener(LoadMainScene);
  }

  private void LoadMainScene()
  {
    ScenesManager.Instance.LoadMainScene();
  }
}
