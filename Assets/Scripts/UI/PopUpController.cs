using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopUpController : MonoBehaviour
{
  [SerializeField] Button _homeButton;
  public GameObject popupPanel;
  public Image image;
  public Image categoryImg;

  public TextMeshProUGUI nameText;
  public TextMeshProUGUI yearText;
  public TextMeshProUGUI descriptionText;

  void Start()
  {
    popupPanel.SetActive(false);
    _homeButton.onClick.AddListener(ClosePopup);
  }

  public void DisplayPopup(Place place)
  {
    image.sprite = place.PlaceImage;
    categoryImg.sprite = place.Image;
    nameText.text = place.Name;
    yearText.text = $"Year: {place.Year}";
    descriptionText.text = place.Description;

    popupPanel.SetActive(true);
  }

  public void ClosePopup()
  {
    popupPanel.SetActive(false);
  }
}