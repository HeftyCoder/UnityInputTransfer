using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArucoBoardLayoutProvider : MonoBehaviour
{
	// size of the markers in the aruco board OR individual markers
    public float markerSizeForSingle = 0.08f;
    public float markerSizeForBoard = 0.04f;

	public ArucoBoardLayout arucoLayout; 
	// custom points representing the corner locations of 
	public List<Vector3> customObjectPointsUnity;
	
	/// <summary>
	/// Convert from unity vector 3 to windows vector 3
	/// </summary>
	/// <returns></returns>
	public List<System.Numerics.Vector4> GetLayout()
    {
		List<System.Numerics.Vector4> customObjectPoints = new List<System.Numerics.Vector4>();
		foreach(var data in arucoLayout.items)
        {
			var pos = data.topLeftCorner;
			customObjectPoints.Add(new System.Numerics.Vector4(pos.x, pos.y, pos.z, data.size));
        }
		return customObjectPoints;
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
