using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Sound Library")]
public class SoundLibrary : ScriptableObject
{
    [Header("Footsteps")]
    public AudioClip walkGrass;
    public AudioClip runGrass;

    public AudioClip walkMud;
    public AudioClip runMud;

    public AudioClip walkWood;
    public AudioClip runWood;

    public AudioClip walkRock;
    public AudioClip runRock;

    public AudioClip walkRockySand;
    public AudioClip runRockySand;

    [Header("Pickups")]
    public AudioClip pickupItem;
    public AudioClip pickupItem_02;

    [Header("Combat")]
    public AudioClip punch;
    public AudioClip playerShot;
    public AudioClip enemyShot;
    public AudioClip explosion;

    [Header("Enemies")]
    public AudioClip miniRobotExplosion;
    public AudioClip SpittingPlantShot;

    [Header("Ambiental")]
    public AudioClip fireCamp;

    [Header("Dialogue")]
    public AudioClip dialogueKey1;
    public AudioClip dialogueKey2;
    public AudioClip dialogueKey3;

}
