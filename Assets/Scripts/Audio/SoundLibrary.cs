using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Sound Library")]
public class SoundLibrary : MonoBehaviour
{
    [Header("Footsteps")]
    public AudioClip walkGrass;
    public AudioClip runGrass;
    public AudioClip walkMud;
    public AudioClip runMud;

    [Header("Jump & Land")]
    public AudioClip jump;
    public AudioClip landSoft;
    public AudioClip landHard;

    [Header("Pickups")]
    public AudioClip pickupItem;
    public AudioClip pickupHealth;
    public AudioClip pickupAbility;

    [Header("Combat")]
    public AudioClip punch;
    public AudioClip playerShot;
    public AudioClip enemyShot;
    public AudioClip explosion;
}
