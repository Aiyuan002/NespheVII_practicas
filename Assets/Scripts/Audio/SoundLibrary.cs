using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Sound Library")]
public class SoundLibrary : MonoBehaviour
{
    [Header("Footsteps")]
    public AudioClip walkGrass;
    public AudioClip runGrass;
    public AudioClip walkMud;
    public AudioClip runMud;
    public AudioClip walkWood;
    public AudioClip runWood;

    [Header("Pickups")]
    public AudioClip pickupItem;

    [Header("Combat")]
    public AudioClip punch;
    public AudioClip playerShot;
    public AudioClip enemyShot;
    public AudioClip explosion;

    [Header("Enemies")]
    public AudioClip miniRobotExplosion;



}
