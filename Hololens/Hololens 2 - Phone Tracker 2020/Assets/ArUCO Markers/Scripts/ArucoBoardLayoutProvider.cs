using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArucoBoardLayoutProvider : MonoBehaviour
{
	// size of the markers in the aruco board OR individual markers
    public float markerSizeForSingle = 0.08f;
    public float markerSizeForBoard = 0.04f;
    public int numMarkers;

	// custom points representing the corner locations of 
	public List<Vector3> customObjectPointsUnity;
	
	/// <summary>
	/// Convert from unity vector 3 to windows vector 3
	/// </summary>
	/// <returns></returns>
	public List<System.Numerics.Vector3> GetLayout()
    {
		List<System.Numerics.Vector3> customObjectPoints = new List<System.Numerics.Vector3>();
		foreach(var objectPoint in customObjectPointsUnity)
        {
			customObjectPoints.Add(new System.Numerics.Vector3(objectPoint.x, objectPoint.y, objectPoint.z));
        }
		return customObjectPoints;
    }		

	public float GetMarkerSize(ArUcoUtils.ArUcoTrackingType arUcoTrackingType)
    {
		switch (arUcoTrackingType)
		{
			case ArUcoUtils.ArUcoTrackingType.Markers:
				return markerSizeForSingle;

			case ArUcoUtils.ArUcoTrackingType.CustomBoard:
				return markerSizeForBoard;
			case ArUcoUtils.ArUcoTrackingType.None:
				Debug.Log("Not tracking...");
				return 0;
		}
		return 0;
	}

	/*
	Centered Markup from Slicer
	9.56385,8.93296
	-5.74237,8.93345
	-5.68413,-6.27982
	9.52103,-6.31107
	*/

	// y
	// ^
	// |
	// |
	//  _______> x

	// Scale points for meters in Unity
	// Centered in Slicer
	// Multiply components by 10
	//markerLocations.push_back(cv::Point3f(-95.6385f, 89.3296f, 0) / 1000.0); // convert from mm to m for unity
	//markerLocations.push_back(cv::Point3f(57.4237f, 89.3345f, 0) / 1000.0);
	//markerLocations.push_back(cv::Point3f(56.8413f, -62.7982f, 0) / 1000.0);
	//markerLocations.push_back(cv::Point3f(-95.2103f, -63.1107f, 0) / 1000.0);

}
