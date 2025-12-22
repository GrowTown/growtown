using System.Collections;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// Plays the attached VideoPlayer as soon as the scene loads and exposes a couple of
/// convenience toggles so designers can hook it up without additional scripting.
/// Attach this to the GameObject that already owns the VideoPlayer (screen, RawImage, etc).
/// </summary>
[RequireComponent(typeof(VideoPlayer))]
public class StartupVideoPlayer : MonoBehaviour
{
    [Header("Playback")]
    [SerializeField] private bool playOnAwake = true;
    [SerializeField] private bool waitForPreparation = true;
    [SerializeField] private bool loopVideo;

    [Header("Post Video Actions")]
    [SerializeField] private GameObject enableAfterVideo;   // e.g. main menu root
    [SerializeField] private GameObject disableDuringVideo; // e.g. HUD canvas

    private VideoPlayer videoPlayer;
    private bool hasStartedPlayback;

    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.isLooping = loopVideo;
        videoPlayer.loopPointReached += HandleVideoFinished;

        if (disableDuringVideo != null)
            disableDuringVideo.SetActive(false);

        if (enableAfterVideo != null)
            enableAfterVideo.SetActive(false);

        if (playOnAwake)
            TryPlayVideo();
    }

    private void Start()
    {
        // Covers the case where the component gets enabled after Awake.
        if (!hasStartedPlayback && !playOnAwake)
            TryPlayVideo();
    }

    private void OnEnable()
    {
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        if (!hasStartedPlayback && playOnAwake)
            TryPlayVideo();
    }

    private void TryPlayVideo()
    {
        if (videoPlayer.clip == null && string.IsNullOrEmpty(videoPlayer.url))
        {
            Debug.LogWarning($"{nameof(StartupVideoPlayer)}: No video clip or URL is assigned.", this);
            return;
        }

        if (waitForPreparation)
        {
            StartCoroutine(PlayWhenPrepared());
            return;
        }

        videoPlayer.Play();
        hasStartedPlayback = true;
    }

    private IEnumerator PlayWhenPrepared()
    {
        if (!videoPlayer.isPrepared)
        {
            videoPlayer.Prepare();
            while (!videoPlayer.isPrepared)
                yield return null;
        }

        videoPlayer.Play();
        hasStartedPlayback = true;
    }

    private void HandleVideoFinished(VideoPlayer source)
    {
        if (disableDuringVideo != null)
            disableDuringVideo.SetActive(true);

        if (enableAfterVideo != null)
            enableAfterVideo.SetActive(true);
    }
}
