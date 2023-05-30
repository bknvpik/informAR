using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainScene : MonoBehaviour
{
  [SerializeField] Button _placesButton;
  [SerializeField] Button _settingsButton;
  [SerializeField] Button _infoButton;
  [SerializeField] ARManager arManager;

  private Text _text;
  void Start()
  {
    this._text = arManager.OutputText;
    _placesButton.onClick.AddListener(LoadPlacesScene);
    _settingsButton.onClick.AddListener(LoadSettingsScene);
    _infoButton.onClick.AddListener(ToggleInfo);

  }

  private void LoadPlacesScene()
  {
    ScenesManager.Instance.LoadScene(ScenesManager.Scene.PlacesScene);
  }

  private void LoadSettingsScene()
  {
    ScenesManager.Instance.LoadScene(ScenesManager.Scene.SettingsScene);
  }

  private void ToggleInfo()
  {
    this._text.gameObject.SetActive(!this._text.gameObject.activeSelf);
  }
}
