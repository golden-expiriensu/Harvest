using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DJ_Eblan : MonoBehaviour
{
    public List<AudioClip> musicAudioClips = new List<AudioClip>();

    private AudioSource musicAudioSource = null;

    private void Awake()
    {
        musicAudioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        //StartCoroutine(PlayBackgroudMusic());
    }

    IEnumerator PlayBackgroudMusic()
    {
        int musicIndex = 0;
        while (musicAudioClips.Count > 0)
        {
            float waitTime = musicAudioClips[musicIndex].length;

            musicAudioSource.PlayOneShot(musicAudioClips[musicIndex]);

            musicIndex++;
            if (musicIndex >= musicAudioClips.Count)
            {
                musicIndex = 0;
            }

            yield return new WaitForSeconds(waitTime);
        }
    }
}
