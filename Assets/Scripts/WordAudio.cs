using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WordAudio : MonoBehaviour
{
    private const string WORD_AUDIO_PATH = "Audio/Words";

    private static AudioSource _audioSource;
    private static List<AudioClip> _audioClips;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioClips = Resources.LoadAll<AudioClip>(WORD_AUDIO_PATH).ToList();
    }

    public static void PlayWordAudio(string word)
    {
        word = word.ToLower();

        AudioClip audioClip = _audioClips.FirstOrDefault(clip => clip.name.ToLower() == word);

        _audioSource.clip = audioClip;
        _audioSource.Play();
    }
}
