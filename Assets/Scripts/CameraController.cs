using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerTransform;
    public PlayerController playerControllerScript;
    public GameManager gameManagerScript;

    public float speed = 1f;
    public Vector3 velocity = Vector3.zero;
    public bool moveCamera = false;
    public float stallCameraTime;

    public AudioClip[] gameAudioClips;
    private AudioSource gameAudioSrc;
    private int audioClipPlaying = 0;

    // Start is called before the first frame update
    void Start()
    {
        playerControllerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        gameAudioSrc = GetComponent<AudioSource>();
        gameAudioSrc.clip = gameAudioClips[audioClipPlaying];
        gameAudioSrc.Play();

    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x != playerTransform.transform.position.x)
        {
            StartCoroutine(StallCameraMovement());
        }

        if (transform.position.x != playerTransform.transform.position.x && moveCamera)
        {

            transform.position = Vector3.SmoothDamp(transform.position, CameraMovePosition(), ref velocity, speed);
        }

        if (!gameAudioSrc.isPlaying)
        {
            gameAudioSrc.clip = gameAudioClips[1];
            gameAudioSrc.Play();
        }


    }

    Vector3 CameraMovePosition()
    {
        Vector3 newCameraPosYZplayerPosX;
        if (playerTransform.position.x <= -3)
        {
            newCameraPosYZplayerPosX = new Vector3(-3, transform.position.y, transform.position.z);
        }

        else if (playerTransform.position.x >= 3)
        {
            newCameraPosYZplayerPosX = new Vector3(3, transform.position.y, transform.position.z);
        }
        else
        {
            newCameraPosYZplayerPosX = new Vector3(playerTransform.position.x, transform.position.y, transform.position.z);
        }

        return newCameraPosYZplayerPosX;
    }

    IEnumerator StallCameraMovement()
    {
        yield return new WaitForSeconds(stallCameraTime);
        moveCamera = true;
    }
}
