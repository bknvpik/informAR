using UnityEngine;

public class CompassManager : MonoBehaviour
{
  public static float heading;
  private void Start()
  {
    // Check if the device has a compass sensor
    if (!SystemInfo.supportsGyroscope)
    {
      Debug.LogError("Device does not support gyroscope/compass");
      return;
    }

    // Enable the compass
    Input.compass.enabled = true;
  }

  private void Update()
  {
    // Read the compass heading
    heading = Input.compass.trueHeading;
  }
}
