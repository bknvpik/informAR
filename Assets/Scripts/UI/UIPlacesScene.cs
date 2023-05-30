using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UIPlacesScene : MonoBehaviour
{
  [SerializeField] Button _homeButton;
  public GameObject listItemPrefab;
  public Transform listItemContainer;
  private List<Place> _data = new List<Place>();

  void Start()
  {
    _homeButton.onClick.AddListener(LoadMainScene);

    this._data = Database.instance.GetAllPlaces()
      .Where(place => place.Category != "Signposts")
      .OrderBy(place => place.distance)
      .ToList();

    RenderTiles();
  }

  private void LoadMainScene()
  {
    ScenesManager.Instance.LoadMainScene();
  }

  void RenderTiles()
  {
    foreach (Place place in _data)
    {
      GameObject newItem = Instantiate(listItemPrefab, listItemContainer);

      ListItemController controller = newItem.GetComponent<ListItemController>();
      if (controller != null)
      {
        controller.Setup(place);

        Button showPopUpButton = newItem.GetComponent<Button>();
        if (showPopUpButton != null)
        {
          showPopUpButton.onClick.AddListener(controller.OnItemClick);
        }
      }
    }
  }
}
