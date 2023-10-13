using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
//using Microsoft.MixedReality.OpenXR;
using TMPro;
#if ENABLE_WINMD_SUPPORT
using Windows.Graphics.Imaging;
using Windows.Perception.Spatial;
using OpenCVRuntimeComponent;
using OpenCVRuntimeComponent.Aruco;
using Windows.Media.Devices.Core;
#endif

public class ArucoTracker : MonoBehaviour
{
    [SerializeField] MediaCapturer mediaCapturer;
    
    public TMP_Text status;
    public int minimumMarkersForDetection = 5;
    public int processAfterNumFrames = 1;
    public ArUcoUtils.ArUcoDictionaryName ArUcoDictionaryName = ArUcoUtils.ArUcoDictionaryName.DICT_6X6_50;
    public ArUcoUtils.ArUcoTrackingType ArUcoTrackingType = ArUcoUtils.ArUcoTrackingType.Markers;
    public ArucoBoardLayoutProvider boardPositions;
    

    public Action<IReadOnlyList<Marker>> onDetectionFinished;

    private int count = 0;
    private List<Marker> markersInUnity = new List<Marker>();

#if ENABLE_WINMD_SUPPORT
    /// <summary>
    /// OpenCV windows runtime dll component
    /// </summary>
    
    ArucoDetector detector;
    OpenCVRuntimeComponent.Aruco.CameraIntrinsics calibParams;

    Windows.Media.Devices.Core.CameraIntrinsics camIntrinsics;
    /// <summary>
    /// Coordinate system reference for Unity to WinRt transform construction.
    /// </summary>
    private SpatialCoordinateSystem _unityCoordinateSystem = null;
    private SpatialCoordinateSystem _frameCoordinateSystem = null;

#endif

    public void Toggle() => enabled = !enabled;
    private void OnEnable()
    {
        StartTracking();
    }

    private void OnDisable()
    {
        StopTracking();
    }

    private void StartTracking()
    {
#if ENABLE_WINMD_SUPPORT
        // Get the unity spatial coordinate system
        try
        {
            _unityCoordinateSystem = PerceptionInterop.GetSceneCoordinateSystem(Pose.identity) as SpatialCoordinateSystem;
            Debug.Log("Successfully cached pointer to Unity spatial coordinate system.");
        }
        catch (Exception ex)
        {
            status.text = $"Failed to get Unity spatial coordinate system: {ex.Message}.";
        }

        // Configure the dll with input parameters
        var layout = boardPositions.GetLayout();
        
        status.text = "Setting dll";
        try
        {
            //This will never be null, I think
            detector = new ArucoDetector((int)ArUcoDictionaryName, layout);
            camIntrinsics = null;
        }
        catch(Exception e){
            status.text = $"{e.Message}";
        }
        mediaCapturer.onFrameArrived += HandleArUcoTracking;
#endif
    }

    private void StopTracking()
    {
#if ENABLE_WINMD_SUPPORT
        mediaCapturer.onFrameArrived -= HandleArUcoTracking;
#endif
    }

#if ENABLE_WINMD_SUPPORT
    /// <summary>
    /// Method to extract important paramters and software bitmap from
    /// media frame reference and send to opencv dll for marker-based or
    /// board-based tracking.
    /// </summary>
    private void HandleArUcoTracking(Windows.Media.Capture.Frames.MediaFrameReference mediaFrameReference)
    {
        if (count != 0)
        {   
            count--;
            return;
        }
        count = processAfterNumFrames;
        // Request software bitmap from media frame reference
        var videoMediaFrame = mediaFrameReference?.VideoMediaFrame;
        var softwareBitmap = videoMediaFrame?.SoftwareBitmap;
        
        if (softwareBitmap == null)
            return;
        // Cache the current camera projection transform (not using currently) ??
        //var cameraProjectionTransform = camIntrinsics.UndistortedProjectionTransform;
            
        // Cache the current camera frame coordinate system
        _frameCoordinateSystem = mediaFrameReference.CoordinateSystem;
            
        if (camIntrinsics == null)
        {
            camIntrinsics = videoMediaFrame.CameraIntrinsics;
            calibParams = new OpenCVRuntimeComponent.Aruco.CameraIntrinsics(
                    camIntrinsics.FocalLength, // Focal length
                    camIntrinsics.PrincipalPoint, // Principal point
                    camIntrinsics.RadialDistortion, // Radial distortion
                    camIntrinsics.TangentialDistortion, // Tangential distortion
                    (int)camIntrinsics.ImageWidth, // Image width
                    (int)camIntrinsics.ImageHeight); // Image height

            detector.SetCameraIntrinsics(calibParams);
        }

        IReadOnlyList<Marker> markers = null;
        switch (ArUcoTrackingType)
        {
            case ArUcoUtils.ArUcoTrackingType.Markers:
                markers = DetectMarkers(softwareBitmap);
                break;

            case ArUcoUtils.ArUcoTrackingType.CustomBoard:
                markers = DetectBoard(softwareBitmap);
                break;

            case ArUcoUtils.ArUcoTrackingType.None:
                status.text = $"Not running tracking...";
                break;

            default:
                status.text = $"No option selected for tracking...";
                break;
        }

        if (markers != null)
        {
                UnityEngine.WSA.Application.InvokeOnAppThread(() =>
                {
                    status.text = $"markers.Count";
                    onDetectionFinished?.Invoke(markers);
                }, false);
        }
        softwareBitmap.Dispose();
    }

    private IReadOnlyList<Marker> DetectMarkers(SoftwareBitmap softwareBitmap)
    {

        // Get marker detections from opencv component
        var markers = detector.DetectMarkers(softwareBitmap);
        markersInUnity.Clear();

        // Iterate across detections
        foreach (var marker in markers)
        {
            var transformUnityCamera = ArUcoUtils.GetTransformInUnityCamera(
                        ArUcoUtils.Vec3FromFloat3(marker.Position),
                        ArUcoUtils.RotationQuatFromRodrigues(ArUcoUtils.Vec3FromFloat3(marker.Rotation)));

            // Camera view transform used for transform chain
            var cameraToWorld = GetViewToUnityTransform(_frameCoordinateSystem);
            
            var transformUnityWorld = cameraToWorld * transformUnityCamera;

            var posInUnity = ArUcoUtils.GetVectorFromMatrix(transformUnityWorld);
            var rotInUnity = ArUcoUtils.GetQuatFromMatrix(transformUnityWorld);
            var markerInUnity = new Marker(posInUnity, rotInUnity);
            markersInUnity.Add(markerInUnity);
        }

        //Avoid returning a pose estimation with too little markers as it is inconsistent
        if (markersInUnity.Count >= minimumMarkersForDetection)
            return null;

        return markersInUnity;
    }

    private IReadOnlyList<Marker> DetectBoard(SoftwareBitmap softwareBitmap)
    {
        // Get marker detections from opencv component
        var board = detector.DetectBoard(softwareBitmap);
        markersInUnity.Clear();

        Vector3 unityPosition = Vector3.zero, markerPosition = Vector3.zero;
        Quaternion unityRotation = Quaternion.identity, markerRotation = Quaternion.identity;
        var isDetected = board.IsDetected;
        
        //Ensure that the markers found are adequate for board detection (it's inconsistent to find 1-2 markers if board has 24)
        if (board.MarkersCount < minimumMarkersForDetection)
            return null;
        
        var yRot = Quaternion.Euler(new Vector3(0,180,0));
        if (isDetected)
        {
            // Get the transform from C++ component and format for Unity coordinate system
            var transformUnityCamera = ArUcoUtils.GetMatrixFromOpenCVToUnity(
                board.Position,
                board.Rotation);

            // Camera view transform used for transform chain
            var cameraToWorld = GetViewToUnityTransform(_frameCoordinateSystem);
            
            var predefinedMatrix = Matrix4x4.identity;
            var transformUnityWorld = cameraToWorld * transformUnityCamera;
            unityPosition = ArUcoUtils.GetVectorFromMatrix(transformUnityWorld);
            unityRotation = ArUcoUtils.GetQuatFromMatrix(transformUnityWorld);
            
            var markerInUnity = new Marker(unityPosition, unityRotation);
            
            markersInUnity.Add(markerInUnity);
        }

        return markersInUnity;
    }

    //https://github.com/microsoft/MixedReality-SpectatorView/blob/7796da6acb0ae41bed1b9e0e9d1c5c683b4b8374/src/SpectatorView.Unity/Assets/PhotoCapture/Scripts/HoloLensCamera.cs#L1256
    /// <summary>
    /// Create the camera extrinsics from unity coordinate system
    /// and the current frame coordinate system.
    /// </summary>
    /// <param name="frameCoordinateSystem"></param>
    /// <returns></returns>
    private Matrix4x4 GetViewToUnityTransform(
        SpatialCoordinateSystem frameCoordinateSystem)
    {
        if (frameCoordinateSystem == null || _unityCoordinateSystem == null)
        {
            return Matrix4x4.identity;
        }

        // Get the reference transform from camera frame to unity space
        System.Numerics.Matrix4x4? cameraToUnityRef = frameCoordinateSystem.TryGetTransformTo(_unityCoordinateSystem);

        // Return identity if value does not exist
        if (!cameraToUnityRef.HasValue)
            return Matrix4x4.identity;

        // No cameraViewTransform availabnle currently, using identity for HL2
        // Inverse of identity is identity
        //var viewToCamera = Matrix4x4.identity;
        
        var cameraToUnity = ArUcoUtils.Mat4x4FromFloat4x4(cameraToUnityRef.Value);

        // Compute transform to relate winrt coordinate system with unity coordinate frame (viewToUnity)
        // WinRT transfrom -> Unity transform by transpose and flip row 3
        //var viewToUnityWinRT = viewToCamera * cameraToUnity;
        
        var viewToUnityWinRT = cameraToUnity;
        var viewToUnity = Matrix4x4.Transpose(viewToUnityWinRT);
        
        viewToUnity.m20 *= -1.0f;
        viewToUnity.m21 *= -1.0f;
        viewToUnity.m22 *= -1.0f;
        viewToUnity.m23 *= -1.0f;

        return viewToUnity;
    }
#endif
}
