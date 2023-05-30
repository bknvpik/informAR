using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Google.XR.ARCoreExtensions;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

public class ARManager : MonoBehaviour
{
  public AREarthManager EarthManager;
  public VpsInitializer Initializer;
  public ARAnchorManager AnchorManager;
  public PopUpController popUpController;
  private List<GameObject> _anchorObjects = new List<GameObject>();
  private List<Place> _data = new List<Place>();
  private Dictionary<GameObject, Place> _objectToPlace = new Dictionary<GameObject, Place>();
  private Place minDistance;
  public RectTransform arrowRectTransform;
  public Text OutputText;
  public TextMeshProUGUI closestDistance;
  public TextMeshProUGUI closestName;
  public double HeadingThreshold = 25;
  public double HorizontalThreshold = 20;
  private bool _isLocalizing = false;
  private int _terrainCount = 0;
  public string SnackBarText = "Result: ";
  public string SnackBarTextAnchor = "Anchors: ";
  public string ClickText;
  private const string _resolvingTimeoutMessage =
      "Still resolving the terrain anchor.\n" +
      "Please make sure you're in an area that has VPS coverage.";

  void Start()
  {
    this._data = Database.instance.GetAllPlaces();
    StartCoroutine(GetMinDistance());
  }
  void Update()
  {
    string status = "";
    if (!Initializer.IsReady || EarthManager.EarthTrackingState != TrackingState.Tracking)
    {
      return;
    }
    GeospatialPose pose = EarthManager.CameraGeospatialPose;
    StartCoroutine(AppHelper.RecalculateDistance(pose));
    if (pose.OrientationYawAccuracy > HeadingThreshold ||
          pose.HorizontalAccuracy > HorizontalThreshold)
    {
      status = "Low accuracy: Look Around";
    }
    else
    {
      status = "High accuracy: High Tracking Precision";

      for (int i = 0; i < this._data.Count; i++)
      {
        if (i >= _anchorObjects.Count || !_anchorObjects[i])
        {
          if (this._data[i].Category == "Signposts")
          {
            PlaceGeospatialAnchor(_data[i].ContentPrefab, _data[i].Latitude, _data[i].Longitude, pose.Altitude - 1.5f, _data[i], _data[i].Heading, false);
          }
          else
          {
            PlaceGeospatialAnchor(_data[i].ContentPrefab, _data[i].Latitude, _data[i].Longitude, 0, _data[i], 0, true);
          }
        }
      }
    }

    ShowTrackingInfo(status, pose);

    double bearing = AppHelper.CalculateBearing(
        pose.Latitude,
        pose.Longitude,
        this.minDistance.Latitude,
        this.minDistance.Longitude
    );

    float heading = CompassManager.heading;
    float direction = this.UpdateArrow(heading, bearing);
    arrowRectTransform.eulerAngles = new Vector3(0, 0, -direction + 90);

    if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
    {
      Ray ray;
      // If it's a touch screen (mobile)
      if (Input.touchCount > 0)
      {
        ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
      }
      // Else it's a mouse click (PC)
      else
      {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      }

      RaycastHit hit;
      if (Physics.Raycast(ray, out hit))
      {
        GameObject clickedObject = hit.transform.gameObject;

        // Look up the associated Place in the dictionary.
        if (_objectToPlace.TryGetValue(clickedObject, out Place associatedPlace))
        {
          ClickText = $"Object {associatedPlace.Name}";
          // Call DisplayPopup with the associated Place.
          popUpController.DisplayPopup(associatedPlace);
        }
      }
    }
  }

  private float UpdateArrow(float heading, double bearing)
  {
    float direction = (float)bearing - heading;

    return direction;
  }

  void ShowTrackingInfo(string status, GeospatialPose pose)
  {
    OutputText.text = string.Format(
      "Latitude/Longitude: {0}°, {1}°\n" +
      "Horizontal Accuracy: {2}m\n" +
      "Altitude: {3}m\n" +
      "Vertical Accuracy: {4}m\n" +
      "Heading: {5}°\n" +
      "Heading Accuracy: {6}°\n" +
      "{7} \n" +
      "{8} \n" +
      "{9} \n" +
      "{10} \n"
      ,
      pose.Latitude.ToString("F6"),  //{0}
      pose.Longitude.ToString("F6"), //{1}
      pose.HorizontalAccuracy.ToString("F6"), //{2}
      pose.Altitude.ToString("F2"),  //{3}
      pose.VerticalAccuracy.ToString("F2"),  //{4}
      pose.EunRotation.ToString("F1"),   //{5}
      pose.OrientationYawAccuracy.ToString("F1"),   //{6}
      status, // {7}
      SnackBarTextAnchor, // {8}
      SnackBarText, // {9}
      ClickText //{10}
    );
  }

  private ARGeospatialAnchor PlaceGeospatialAnchor(
    GameObject contentPrefab,
    double latitude,
    double longtitude,
    double altitude,
    Place place,
    double heading = 0.0,
    bool terrain = false
  )
  {
    Quaternion eunRotation = Quaternion.AngleAxis(180f - (float)heading, Vector3.up);

    var anchor = terrain ?
        AnchorManager.ResolveAnchorOnTerrain(latitude, longtitude, 0, eunRotation) :
            AnchorManager.AddAnchor(latitude, longtitude, altitude, eunRotation);

    if (anchor != null)
    {
      GameObject anchorGO = Instantiate(contentPrefab, anchor.transform);
      this._objectToPlace[anchorGO] = place;
      TextMeshProUGUI[] textComponents = anchorGO.GetComponentsInChildren<TextMeshProUGUI>();

      if (textComponents.Length > 0)
      {
        foreach (TextMeshProUGUI txt in textComponents)
        {
          txt.text = place.Name;
        }
      }
      anchor.gameObject.SetActive(!terrain);
      _anchorObjects.Add(anchor.gameObject);

      if (terrain)
      {
        StartCoroutine(CheckTerrainAnchorState(anchor));
      }
      else
      {
        SnackBarTextAnchor = $"{_anchorObjects.Count} Anchor(s) Set!";
      }
    }
    else
    {
      SnackBarText = string.Format(
          "Failed to set {0}!", terrain ? "a terrain anchor" : "an anchor");
    }

    return anchor;
  }

  private IEnumerator CheckTerrainAnchorState(ARGeospatialAnchor anchor)
  {
    if (anchor == null || _anchorObjects == null)
    {
      yield break;
    }

    int retry = 0;
    while (anchor.terrainAnchorState == TerrainAnchorState.TaskInProgress)
    {
      if (_anchorObjects.Count == 0 || !_anchorObjects.Contains(anchor.gameObject))
      {
        Debug.LogFormat(
            "{0} has been removed, exist terrain anchor state check.",
            anchor.trackableId);
        yield break;
      }

      if (retry == 100 && _anchorObjects.Last().Equals(anchor.gameObject))
      {
        SnackBarText = _resolvingTimeoutMessage;
      }

      yield return new WaitForSeconds(0.1f);
      retry = Math.Min(retry + 1, 100);
    }

    anchor.gameObject.SetActive(
        !_isLocalizing && anchor.terrainAnchorState == TerrainAnchorState.Success);
    if (_anchorObjects.Last().Equals(anchor.gameObject))
    {
      this._terrainCount++;
      SnackBarText = $"Terrain anchor state: {anchor.terrainAnchorState}, count: {this._terrainCount}";
    }

    yield break;
  }

  private IEnumerator GetMinDistance()
  {
    while (true)
    {
      List<Place> data = Database.instance.GetAllPlaces();
      data = data.Where(place => place.Category != "Signposts")
                 .OrderBy(place => place.distance)
                 .ToList();
      Place result = data.First();

      closestDistance.text = $"{result.distance} m";
      closestName.text = $"{result.Name}";
      this.minDistance = result;
      yield return new WaitForSeconds(5);
    }
  }
}