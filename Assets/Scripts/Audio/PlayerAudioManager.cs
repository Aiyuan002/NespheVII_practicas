using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    private SoundLibrary Sounds => AudioManager.Instance.sounds;

    public void PlayPlayerShot()
    {
        Debug.Log($"[Audio] PlayPlayerShot | audioSource: {audioSource != null} | clip: {Sounds.playerShot != null}");
        PlayOneShot(Sounds.playerShot);
    }

    // public void PlayJump()
    // {
    //     // PlayOneShot(Sounds.jump);
    // }

    // public void PlayDeath()
    // {
    //     // PlayOneShot(Sounds.death);
    // }

    // public void PlayCrouch()
    // {
    //     // PlayOneShot(Sounds.crouch);
    // }

    private void PlayOneShot(AudioClip clip)
    {
        if (audioSource == null || clip == null)
        {
            Debug.Log("SOY EL OBJETO: " + gameObject.name + " Y ALGO ESTA NULL: " + audioSource == null ? "AudioSource" : "Clip");
            return;
        }
        Debug.Log($"[PlayerAudio] vol={audioSource.volume} mute={audioSource.mute} enabled={audioSource.enabled} go_active={audioSource.gameObject.activeInHierarchy} outputGroup={audioSource.outputAudioMixerGroup}");
        audioSource.PlayOneShot(clip);
    }
}
