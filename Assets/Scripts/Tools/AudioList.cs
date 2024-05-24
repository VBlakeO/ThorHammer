using UnityEngine;

public class AudioList : MonoBehaviour
{
    public AudioSource audioSource = null;
    [Space]

    [SerializeField] private AudioClip[] audioClips = new AudioClip[0];
    [SerializeField] private AudioClip[] randonAudioClips = new AudioClip[0];

    public void PlayRandonAudioClip()
    {
        int r = Random.Range(0, randonAudioClips.Length);
        audioSource.PlayOneShot(randonAudioClips[r]);
    }

    public void PlayOnceAudioClip(int i)
    {
        audioSource.PlayOneShot(audioClips[i]);
    }

    public void PlayAudioClip(int i)
    {
        audioSource.clip = audioClips[i];
        audioSource.Play();
    }


    public void StopAudio()
    {
        audioSource.Stop();
    }
}
