using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Voicelines")]
public class Voicelines : ScriptableObject
{
    public List<AudioClip> voicelines;
    public List<string> subtitles;
    public float voicelineLength;
    public int voicelineToPlay;

    public void Play(AudioSource audioSource, Subtitles givenSubtitles)
    {
        voicelineToPlay = Random.Range(0, voicelines.Count - 1);

        voicelineLength = voicelines[voicelineToPlay].length;

        audioSource.PlayOneShot(voicelines[voicelineToPlay]);
        givenSubtitles.Play(subtitles[voicelineToPlay], voicelines[voicelineToPlay].length);

        voicelineToPlay++;
    }
}
