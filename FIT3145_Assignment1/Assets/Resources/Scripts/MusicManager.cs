using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EMusicTrack
{
    TITLE_SCREEN,
    INGAME,

    MAX
}


public class MusicManager : Singleton<MusicManager>
{
    [SerializeField] AudioClip[] m_musicTracks = new AudioClip[(int)EMusicTrack.MAX];
    [SerializeField] AudioSource m_audioSource;
    EMusicTrack m_currentMusicTrack = EMusicTrack.MAX;

    public void PlayMusicTrack(EMusicTrack trackToPlay, float volume = 1.0f)
    {
        if(m_currentMusicTrack != trackToPlay)
        {
            m_audioSource.Stop();
            m_audioSource.clip = m_musicTracks[(int)trackToPlay];
            m_audioSource.Play();
            m_audioSource.volume = volume;

            m_currentMusicTrack = trackToPlay;
        }
    }

    public void StopMusic(bool withFade = false)
    {

        if(withFade)
        {
            StartCoroutine(IE_AudioFadeOut(m_audioSource, 2.0f));
        }
        else
        {
            m_audioSource.Stop();
            OnMusicStopped();
        }


        m_currentMusicTrack = EMusicTrack.MAX;
    }
    
    private void OnMusicStopped()
    {
        m_audioSource.clip = null;
    }

    public IEnumerator IE_AudioFadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;
            audioSource.volume = Mathf.Clamp(audioSource.volume, 0, 1);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
        OnMusicStopped();
    }

}
