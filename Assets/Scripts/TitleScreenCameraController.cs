using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenCameraController : MonoBehaviour
{
    public float rotationSpeed;
    public AudioClip[] introAudioClips;
    public GameManager gameManagerScript;
    private Transform cameraCenterTransform;
    private AudioSource introAudioSrcs;
    public int audioClip = 0;

    // Start is called before the first frame update
    void Start()
    {
        cameraCenterTransform = GetComponent<Transform>();
        introAudioSrcs = GetComponentInChildren<AudioSource>();
        introAudioSrcs.clip = introAudioClips[audioClip];
        introAudioSrcs.Play();
    }

    // Update is called once per frame
    void Update()
    {
        cameraCenterTransform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed, Space.Self);

        if (!introAudioSrcs.isPlaying)
        {
            audioClip++;
            audioClip = audioClip % introAudioClips.Length;
            //Debug.Log(audioClip);
            introAudioSrcs.clip = introAudioClips[audioClip];
            introAudioSrcs.Play();
        }


    }
}
