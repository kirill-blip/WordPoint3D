using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LetterAudio : MonoBehaviour
{
    private const string LETTERS_AUDIO_PATH = "Audio/Letters";

    private AudioSource _audioSource;
    private List<AudioClip> _lettersClip;

    private bool _isPlaying = false;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _lettersClip = Resources.LoadAll<AudioClip>(LETTERS_AUDIO_PATH).ToList();
    }

    private IEnumerator Play(char letter)
    {
        _isPlaying = true;

        AudioClip clip = GetAudioClip(letter);

        _audioSource.clip = clip;
        _audioSource.Play();

        yield return new WaitWhile(() => _audioSource.isPlaying);
        yield return new WaitForSeconds(.1f);

        _isPlaying = false;
    }

    private AudioClip GetAudioClip(char letter)
    {
        return _lettersClip.FirstOrDefault(clip => clip.name.ToLower() == letter.ToString().ToLower());
    }

    public void PlayAudioLetter(char letter)
    {
        if (!_isPlaying)
        {
            StartCoroutine(Play(letter));
        }
    }

    public bool IsPlaying()
    {
        return _isPlaying;
    }
}
