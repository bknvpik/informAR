using UnityEngine;
using UnityEngine.UI;
using System;

[CreateAssetMenu(fileName = "New Place", menuName = "Data/Place", order = 0)]
public class Place : ScriptableObject
{
  [SerializeField]
  private string placeId;
  public string Category;
  [TextArea]
  public string Description;
  public double Heading;
  public double Latitude;
  public double Longitude;
  public string Name;
  public string Year;
  public Sprite Image;
  public Sprite PlaceImage;
  public GameObject ContentPrefab;
  public double distance;

  public string PlaceId
  {
    get { return placeId; }
  }

  private void OnEnable()
  {
    placeId = Guid.NewGuid().ToString();
  }
}