// Script taken directly from Rene Schulte's repo: https://github.com/reneschulte/WinMLExperiments/blob/master/HoloVision20/Assets/Scripts/MediaCapturer.cs

using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

#if ENABLE_WINMD_SUPPORT
using Windows.Media;
using Windows.Media.Capture;
using Windows.Storage.Streams;
using Windows.Media.Capture.Frames;
using Windows.Media.MediaProperties;
using Windows.Media.Devices;
using Windows.Graphics.Imaging;
using Windows.Devices.Enumeration;
#endif

public class MediaCapturer : MonoBehaviour
{
    // https://docs.microsoft.com/en-us/windows/mixed-reality/develop/platform-capabilities-and-apis/locatable-camera
    public enum MediaCaptureProfiles
    {
        HL2_2272x1278,
        HL2_896x504,
        HL1_1280x720
    }
    public enum MediaCaptureFPS
    {
        FPS_15,
        FPS_30
    }

    [SerializeField] MediaCaptureProfiles mediaCaptureProfiles;
    [SerializeField] MediaCaptureFPS fpsProfile;
#if ENABLE_WINMD_SUPPORT
    
    public event Action<MediaFrameReference> onFrameArrived;
    private MediaCapture _mediaCapture;
    private MediaFrameReader _mediaFrameReader;
#endif
    
    public bool IsCapturing { get; set; }
    bool hasStarted = false;
    private async void Start()
    {
        await StartCapture(mediaCaptureProfiles);
        hasStarted = true;
    }
    private async void OnDestroy()
    {
        await StopCapture();
    }
    private async void OnEnable()
    {
        if (!hasStarted)
            return;
        await ResumeCapture();
    }
    private async void OnDisable()
    {
        await PauseCapture();
    }

    //Start
    public async Task StartCapture(MediaCaptureProfiles mediaCaptureProfiles)
    {
        this.mediaCaptureProfiles = mediaCaptureProfiles;
#if ENABLE_WINMD_SUPPORT
        await StopCapture();

        // Get the media capture description and request media capture profile
        int width = 0;
        int height = 0;
        int fps = 30;
        bool isHL1 = false;
        switch (mediaCaptureProfiles)
        {
            case MediaCaptureProfiles.HL2_2272x1278:
                width = 2272;
                height = 1278;
                break;
            case MediaCaptureProfiles.HL2_896x504:
                width = 896;
                height = 504;
                break;
            case MediaCaptureProfiles.HL1_1280x720:
                width = 1280;
                height = 720;
                isHL1 = true;
                Debug.Log("InitializeMediaFrameReaderAsync: Using the HoloLens 1 settings for initialization.");

                break;
            default:
                width = 0;
                height = 0;
                break;
        }

        // Convert the pixel formats to bgra8
        var subtype = MediaEncodingSubtypes.Bgra8;

        // Create the media capture and media capture frame source from description
        // as a colour media frame source with 30 FPS
        //Debug.Log(fps);
        var mediaCaptureAndFrameSource = await GetMediaCaptureForDescriptionAsync(
            MediaFrameSourceKind.Color, width, height, fps, isHL1);

        // Create the media frame reader with specified description and subtype
        _mediaCapture = mediaCaptureAndFrameSource.capture;
        _mediaFrameReader = await mediaCaptureAndFrameSource.capture.CreateFrameReaderAsync(
            mediaCaptureAndFrameSource.source,
            subtype);
        _mediaFrameReader.AcquisitionMode = MediaFrameReaderAcquisitionMode.Realtime;

        _mediaFrameReader.FrameArrived += ProcessFrame;
        await _mediaFrameReader.StartAsync();
            
        IsCapturing = true;
#endif
    }

    public async Task ResumeCapture()
    {
#if ENABLE_WINMD_STUPPORT
        if (_mediaFrameReader == null)
            return;
        _mediaFrameReader.StartAsync();
        IsCapturing = true;
#endif
    }
    public async Task PauseCapture()
    {
#if ENABLE_WINMD_SUPPORT
        if (_mediaFrameReader == null)
            return;
        await _mediaFrameReader.StopAsync();
        IsCapturing = false;
#endif
    }
    /// <summary>
    /// Asynchronously stop media capture and dispose of resources
    /// </summary>
    /// <returns></returns>
    public async Task StopCapture()
    {
#if ENABLE_WINMD_SUPPORT
        if (_mediaCapture != null && _mediaCapture.CameraStreamState != CameraStreamState.Shutdown)
        {
            _mediaFrameReader.FrameArrived -= ProcessFrame;
            await _mediaFrameReader.StopAsync();
            _mediaFrameReader.Dispose();
            _mediaCapture.Dispose();
            _mediaCapture = null;
            _mediaFrameReader = null;
        }
        IsCapturing = false;
#endif
    }


#if ENABLE_WINMD_SUPPORT
    private void ProcessFrame(MediaFrameReader frameReader, MediaFrameArrivedEventArgs args)
    {
        var frameRef = frameReader.TryAcquireLatestFrame();
        onFrameArrived?.Invoke(frameRef);
        frameRef?.Dispose();
    }


    /// <summary>
    /// https://mtaulty.com/page/5/
    /// Provide an input width, height and framerate to request for the 
    /// media capture initialization. Return a media capture and media
    /// frame source object.
    /// </summary>
    /// <param name="sourceKind"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="frameRate"></param>
    /// <returns></returns>
    async Task<(MediaCapture capture, MediaFrameSource source)> GetMediaCaptureForDescriptionAsync(
            MediaFrameSourceKind sourceKind,
            int width,
            int height,
            int frameRate,
            bool isHL1)
    {
        MediaCapture mediaCapture = null;
        MediaFrameSource frameSource = null;

        var sourceGroups = await MediaFrameSourceGroup.FindAllAsync();

        // Ignore frame rate here on the description as both depth streams seem to tell me they are
        // 30fps whereas I don't think they are (from the docs) so I leave that to query later on.
        var sourceInfo =
            sourceGroups.SelectMany(group => group.SourceInfos)
            .FirstOrDefault(
                si =>
                    // Testing with Video Preview - 
                    // https://holodevelopers.slack.com/archives/C1CQKRQM6/p1605046698173100?thread_ts=1580916605.219700&cid=C1CQKRQM6
                    (si.MediaStreamType == MediaStreamType.VideoPreview) &&
                    (si.SourceKind == sourceKind) && 
                    (si.VideoProfileMediaDescription.Any(
                        desc =>
                            desc.Width == width &&
                            desc.Height == height &&
                            desc.FrameRate == frameRate)));

        // For some reason, I can't select the resolution the same way...
        // Just choose the default params.
        if (isHL1)
        {
            sourceInfo =
                sourceGroups.SelectMany(group => group.SourceInfos)
                .LastOrDefault(
                    si =>
                        (si.MediaStreamType == MediaStreamType.VideoPreview) &&
                        (si.SourceKind == sourceKind) &&
                        (si.VideoProfileMediaDescription.Any(
                            desc =>
                                desc.Width == width &&
                                desc.Height == height &&
                                desc.FrameRate == frameRate)));
        }


        if (sourceInfo != null)
        {
            var sourceGroup = sourceInfo.SourceGroup;

            mediaCapture = new MediaCapture();

            await mediaCapture.InitializeAsync(
               new MediaCaptureInitializationSettings()
               {
                   // Want software bitmaps
                   MemoryPreference = MediaCaptureMemoryPreference.Cpu,
                   SourceGroup = sourceGroup,
                   StreamingCaptureMode = StreamingCaptureMode.Video,
               }
            );
            frameSource = mediaCapture.FrameSources[sourceInfo.Id];

            var selectedFormat = frameSource.SupportedFormats.First(
                format => format.VideoFormat.Width == width && format.VideoFormat.Height == height &&
                format.FrameRate.Numerator / format.FrameRate.Denominator == frameRate);

            await frameSource.SetFormatAsync(selectedFormat);
        }
        return (mediaCapture, frameSource);
    }
#endif
}