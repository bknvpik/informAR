using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ListItemController : MonoBehaviour
{
  public Image image;
  public TextMeshProUGUI nameText;
  public TextMeshProUGUI distanceText;

  private Place localPlace;
  public PopUpController popUpController;
  public GameObject listItemPrefab;

  void Start()
  {
    if (this.localPlace == null)
      listItemPrefab.SetActive(false);
  }

  public void Setup(Place place)
  {
    localPlace = place;
    image.sprite = localPlace.Image;
    nameText.text = localPlace.Name;
    distanceText.text = $"{localPlace.distance} m";
    listItemPrefab.SetActive(true);
  }

  public void OnItemClick()
  {
    popUpController.DisplayPopup(this.localPlace);
  }
}
