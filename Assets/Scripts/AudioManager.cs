using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [SerializeField]
    private AudioSource _audioSource;

    [SerializeField]
    private AudioClip _audioClipBG;

    // Start is called before the first frame update
    void Start()
    {
        _audioSource.clip = _audioClipBG;
       _audioSource.Play();
    }

  
}
