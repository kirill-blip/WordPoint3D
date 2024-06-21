using UnityEngine;

public static class ButtonAudioClick
{
    public static void PlaySound(AudioClip clip)
    {
        AudioSource.PlayClipAtPoint(clip, Camera.main.transform.position, 1f);
    }
}
