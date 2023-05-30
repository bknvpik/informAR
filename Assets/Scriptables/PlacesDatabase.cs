using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

[CreateAssetMenu(fileName = "New Places Database", menuName = "Data/Databases/Places Database")]
public class PlacesDatabase : ScriptableObject
{
  public List<Place> allPlaces;
}