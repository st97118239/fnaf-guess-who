using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource soundEffects;
    public AudioSource voicelines;
    public AudioSource music;

    public List<AudioClip> bgm;

    public AudioClip noteSFX;
    public AudioClip markerSFX;
    public AudioClip folderOpenSFX;
    public AudioClip folderCloseSFX;
    public AudioClip clipboardSFX;

    private void Start()
    {
        music.loop = true;

        music.clip = bgm.OrderBy(i => Random.value).First();

        music.Play();
    }

    public void PlayNoteSound()
    {
        soundEffects.PlayOneShot(noteSFX);
    }

    public IEnumerator PlayDelayedClip(AudioSource givenAudioSource, AudioClip givenClip, float givenDelay)
    {
        yield return new WaitForSeconds(givenDelay);
        givenAudioSource.PlayOneShot(givenClip);
    }
}
