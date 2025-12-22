using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
    public VideoPlayer video;

    void Start()
    {
        video.loopPointReached += OnVideoFinished;
    }

    public void Skip()
    {
        LoadNext();
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        LoadNext();
    }

    void LoadNext()
    {
        SceneManager.LoadScene("GrowTownVer_1");
    }
}
