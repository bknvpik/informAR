using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Database : MonoBehaviour
{
  public PlacesDatabase places;
  public static Database instance;
  private List<Place> _data;
  private void Awake()
  {
    if (instance == null)
    {
      instance = this;
      DontDestroyOnLoad(gameObject);
      this.FetchData();
    }
    else
    {
      Destroy(gameObject);
    }
  }

  public static Place GetPlaceById(string id)
  {
    return instance.places.allPlaces.FirstOrDefault(i => i.PlaceId == id);
  }

  public void FetchData()
  {
    _data = instance.places.allPlaces.ToList();
  }

  public List<Place> GetAllPlaces()
  {
    return this._data;
  }
}
