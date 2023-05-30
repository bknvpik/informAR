using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Google.XR.ARCoreExtensions;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class AppHelper : MonoBehaviour
{
  private static AppHelper instance;

  private void Awake()
  {
    if (instance == null)
    {
      instance = this;
      DontDestroyOnLoad(gameObject);
    }
    else
    {
      Destroy(gameObject);
    }
  }

  public static double CalculateHaversineDistance(double lat1, double lon1, double lat2, double lon2)
  {
    const double EarthRadiusKm = 6371.0;

    double latDiff = ToRadians(lat2 - lat1);
    double lonDiff = ToRadians(lon2 - lon1);
    double lat1Rad = ToRadians(lat1);
    double lat2Rad = ToRadians(lat2);

    double a = Math.Pow(Math.Sin(latDiff / 2), 2) +
              Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
              Math.Pow(Math.Sin(lonDiff / 2), 2);
    double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

    return Math.Round(EarthRadiusKm * c * 1000, 0);
  }

  static double ToRadians(double degrees)
  {
    return degrees * (Math.PI / 180);
  }

  static public IEnumerator RecalculateDistance(GeospatialPose pose)
  {
    while (true)
    {
      foreach (Place place in Database.instance.GetAllPlaces())
      {
        if (place.Category != "Signposts")
          place.distance = CalculateHaversineDistance(place.Latitude, place.Longitude, pose.Latitude, pose.Longitude);
      }

      yield return new WaitForSeconds(30);
    }
  }

  static public double CalculateBearing(double lat1, double lon1, double lat2, double lon2)
  {
    // Convert from degrees to radians
    double lat1Rad = Math.PI * lat1 / 180.0;
    double lon1Rad = Math.PI * lon1 / 180.0;
    double lat2Rad = Math.PI * lat2 / 180.0;
    double lon2Rad = Math.PI * lon2 / 180.0;

    // Calculate the difference in longitudes
    double dLon = lon2Rad - lon1Rad;

    // Calculate the bearing
    double y = Math.Sin(dLon) * Math.Cos(lat2Rad);
    double x = Math.Cos(lat1Rad) * Math.Sin(lat2Rad) -
                Math.Sin(lat1Rad) * Math.Cos(lat2Rad) * Math.Cos(dLon);
    double bearingRad = Math.Atan2(y, x);

    // Convert from radians to degrees and adjust the value to be between 0 and 360
    double bearingDeg = (bearingRad * 180.0 / Math.PI + 360) % 360;

    return bearingDeg;
  }
}
