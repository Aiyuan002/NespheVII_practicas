using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("Player")]
    public int lifes = 3;
    public int maxHealth = 100;
    public int maxEnergy = 100;
    public int currentHealth = 100;
    public int currentEnergy = 100;
    public int healthCosumed = 25;
    public int energyCosumed = 25;
    private float energyTime;
    public bool haGuardado = false;

    public float energyTimer;
    public int score;
    public int gold;
    public int gems;
    public int ammunition1;
    public int ammunition2;
    public int ammunition3;
    public Slider healthSlider;
    public Slider energySlider;
    public TextMeshProUGUI healthText;
    private TextMeshProUGUI energyText;
    public TextMeshProUGUI lifesText;
    private TextMeshProUGUI ammunition1Text;
    private TextMeshProUGUI ammunition2Text;
    private TextMeshProUGUI ammunition3Text;
    private Image playerImage;
    public Sprite mainFace;
    public Sprite heatlhRecoverFace;
    public Sprite energyRecoverFace;
    public Sprite hurtFace;
    public Sprite tiredFace;

    [Header("Player_Gameobject")]
    public GameObject player_;

    /*********************************************************************************************/
    private bool recoveringHealth = false;

    /*********************************************************************************************/

    [Header("Enemy")]
    private GameObject enemy;
    private Slider enemyHealthSlider;
    private TextMeshProUGUI enemyHealthText;
    private TextMeshProUGUI enemyNameText;
    private Image enemyImage;

    [Header("NPC")]
    private GameObject npc;
    private TextMeshProUGUI npcNameText;
    private Image npcImage;

    [Header("Icons")]
    public Sprite[] meleeSprites;
    private Image meleeImage;
    private int meleeIndex = 0;
    public Sprite[] munitionSprites;
    public GameObject energyIcon;
    public GameObject gameobjectAscensor;
    private RecargarAscensor scriptAscensor;
    private int tubos;
    public GameObject translateIcon;
    public GameObject[] Plus;

    [Header("Map")]
    public GameObject map;
    bool isMapOpen = false;
    bool canToggleMap = true;

    private Animator animator;

    [Header("Dead_Player")]
    [SerializeField]
    private GameObject deadPlayer,
        slicedPlayer;

    void Start()
    {
        animator = GetComponent<Animator>();
        scriptAscensor = gameobjectAscensor.GetComponent<RecargarAscensor>();

        healthSlider = GameObject.Find("HealthBar").GetComponent<Slider>();
        energySlider = GameObject.Find("EnergyBar").GetComponent<Slider>();

        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
        energySlider.maxValue = maxEnergy;
        energySlider.value = currentEnergy;

        healthText = healthSlider.GetComponentInChildren<TextMeshProUGUI>();
        energyText = energySlider.GetComponentInChildren<TextMeshProUGUI>();

        lifesText = GameObject.Find("LifeText").GetComponent<TextMeshProUGUI>();
        playerImage = GameObject.Find("PlayerImage").GetComponent<Image>();
        if (!haGuardado)
        {
            currentHealth = maxHealth;
            currentEnergy = maxEnergy;
        }
        healthText.text = currentHealth + "/" + maxHealth;
        energyText.text = currentEnergy + "/" + maxEnergy;
        lifesText.text = lifes.ToString();

        enemy = GameObject.Find("Enemy");
        enemyHealthSlider = enemy.GetComponentInChildren<Slider>();
        enemyHealthText = enemyHealthSlider.GetComponentInChildren<TextMeshProUGUI>();
        enemyNameText = GameObject.Find("EnemyText")?.GetComponent<TextMeshProUGUI>();
        enemyImage = GameObject.Find("EnemyImage")?.GetComponent<Image>();
        enemy.SetActive(false);

        npc = GameObject.Find("NPC");
        npcNameText = GameObject.Find("dialogueName")?.GetComponent<TextMeshProUGUI>();
        npcImage = GameObject.Find("NPCImage")?.GetComponent<Image>();
        npc?.SetActive(false);

        GameObject meleeObj = GameObject.Find("MeleeAttackImage");
        if (meleeObj != null)
        {
            meleeImage = meleeObj.GetComponent<Image>();
            if (meleeImage != null)
            {
                meleeImage.sprite = meleeSprites[meleeIndex];
            }
            else
            {
                Debug.LogWarning("El objeto 'MeleeAttackImage' no tiene componente Image.");
            }
        }
        else
        {
            Debug.LogWarning("No se encontró el objeto 'MeleeAttackImage'.");
        }

        GameObject ammo1Obj = GameObject.Find("Ammunition1Text");
        if (ammo1Obj != null)
        {
            ammunition1Text = ammo1Obj.GetComponent<TextMeshProUGUI>();
            if (ammunition1Text == null)
            {
                Debug.LogWarning(
                    "El objeto 'Ammunition1Text' no tiene componente TextMeshProUGUI."
                );
            }
        }
        else
        {
            Debug.LogWarning("No se encontró el objeto 'Ammunition1Text'.");
        }

        GameObject ammo2Obj = GameObject.Find("Ammunition2Text");
        if (ammo2Obj != null)
        {
            ammunition2Text = ammo2Obj.GetComponent<TextMeshProUGUI>();
            if (ammunition2Text == null)
            {
                Debug.LogWarning(
                    "El objeto 'Ammunition2Text' no tiene componente TextMeshProUGUI."
                );
            }
        }
        else
        {
            Debug.LogWarning("No se encontró el objeto 'Ammunition2Text'.");
        }

        GameObject ammo3Obj = GameObject.Find("Ammunition3Text");
        if (ammo3Obj != null)
        {
            ammunition3Text = ammo3Obj.GetComponent<TextMeshProUGUI>();
            if (ammunition3Text == null)
            {
                Debug.LogWarning(
                    "El objeto 'Ammunition3Text' no tiene componente TextMeshProUGUI."
                );
            }
        }
        else
        {
            Debug.LogWarning("No se encontró el objeto 'Ammunition3Text'.");
        }
    }

    private void Update()
    {
        healthSlider.value = currentHealth;
        energySlider.value = currentEnergy;
        /*if (currentEnergy < maxEnergy)
        {
            ResetEnergy();
        }*/
        if (lifes <= 0)
        {
            lifes = 0;
            lifesText.text = lifes.ToString();

            //aqui poner una courutine y que se reinice automatico o poner lo que sea


            //
            //
        }

        /*********************************************************************************************/
        if (recoveringHealth)
        {
            // recoveryTime += Time.deltaTime;

            // if (recoveryTime >= recoveryTimer)

            energySlider.value = maxEnergy;
            currentEnergy = maxEnergy;
            energyText.text = currentEnergy + "/" + maxEnergy;
            energyTime = 0;
        }
        /*********************************************************************************************/

        if (canToggleMap && Input.GetKeyDown(KeyCode.H))
        {
            StartCoroutine(ToggleMap());
        }
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator ToggleMap()
    {
        canToggleMap = false;

        isMapOpen = !isMapOpen;
        if (isMapOpen)
            animator.Play("MapComplete");

        map.SetActive(isMapOpen);

        yield return new WaitForSeconds(1);
        canToggleMap = true;
    }

    public void ConsumeHealth(float damagePercentage)
    {
        if (healthSlider.value > healthSlider.minValue)
        {
            ChangePlayerFace();
            healthSlider.value = healthSlider.value - healthCosumed;
            currentHealth = currentHealth - healthCosumed;
        }
        else
        {
            lifes--;
            lifesText.text = lifes.ToString();
            healthSlider.value = healthSlider.maxValue;
            currentHealth = maxHealth;
            if (lifes == 1)
            {
                playerImage.sprite = tiredFace;
            }
        }
        healthText.text = currentHealth + "/" + maxHealth;
    }

    public void ConsumeEnergy()
    {
        if (energySlider.value > energySlider.minValue)
        {
            energySlider.value = energySlider.value - energyCosumed;
            currentEnergy = currentEnergy - energyCosumed;
            energyText.text = currentEnergy + "/" + maxEnergy;
        }
    }

    void ResetEnergy()
    {
        energyTime += Time.deltaTime;

        if (energyTime >= energyTimer)
        {
            energySlider.value = maxEnergy;
            currentEnergy = maxEnergy;
            energyText.text = currentEnergy + "/" + maxEnergy;
            energyTime = 0;
        }
    }

    /*********************************************************************************************/
    public void RecoverHealth(int amount)
    {
        Debug.Log("recuperasion");

        if (healthSlider.value < healthSlider.maxValue)
        {
            switch (amount)
            {
                case 25:
                    Plus[0].SetActive(true);
                    Plus[0].GetComponent<Animator>().Play("PlusAnimation");
                    StartCoroutine(DesactivarPlus());
                    break;
                case 50:
                    Plus[0].SetActive(true);
                    Plus[1].SetActive(true);
                    Plus[0].GetComponent<Animator>().Play("PlusAnimation");
                    StartCoroutine(DesactivarPlus());

                    break;
                case 75:
                    Plus[0].SetActive(true);
                    Plus[1].SetActive(true);
                    Plus[2].SetActive(true);
                    Plus[0].GetComponent<Animator>().Play("PlusAnimation");
                    StartCoroutine(DesactivarPlus());
                    break;
            }

            healthSlider.value =
                (healthSlider.value >= healthSlider.maxValue)
                    ? healthSlider.maxValue
                    : healthSlider.value + amount;
            currentHealth =
                (healthSlider.value >= healthSlider.maxValue)
                    ? currentHealth = 100
                    : currentHealth + amount;
        }

        healthText.text = currentHealth + "/" + maxHealth;
        recoveringHealth = true;
        //currentHealth += amount;
    }

    IEnumerator DesactivarPlus()
    {
        yield return new WaitForSeconds(2);
        Plus[0].SetActive(false);
        Plus[1].SetActive(false);

        Plus[2].SetActive(false);
    }

    public void RecoverEnergy(int amount)
    {
        Debug.Log("recuperasion");

        if (energySlider.value < energySlider.maxValue)
        {
            energySlider.value =
                (energySlider.value >= energySlider.maxValue)
                    ? energySlider.maxValue
                    : energySlider.value + amount;
            currentEnergy =
                (energySlider.value >= energySlider.maxValue)
                    ? currentEnergy = 100
                    : currentEnergy + amount;
        }

        energyText.text = currentEnergy + "/" + maxEnergy;

        //currentHealth += amount;
    }

    public void RecoverEnergy() { }

    /*********************************************************************************************/
    public void ChangePlayerFace()
    {
        if (playerImage.sprite == mainFace)
        {
            playerImage.sprite = hurtFace;
            StartCoroutine(ResetFace(mainFace));
        }
        else if (playerImage.sprite == tiredFace)
        {
            playerImage.sprite = hurtFace;
            StartCoroutine(ResetFace(tiredFace));
        }
    }

    IEnumerator ResetFace(Sprite face)
    {
        yield return new WaitForSeconds(0.7f);
        playerImage.sprite = face;
    }

    public void EnabledEnemyCanvas(int he, int dmg, int maxH, string name, Sprite face)
    {
        enemyImage.sprite = face;
        enemyNameText.text = name;
        enemyHealthSlider.maxValue = maxH;
        enemy.SetActive(true);
        enemyHealthSlider.value = he - dmg;
        he = he - dmg;
        enemyHealthText.text = he + "/" + maxH;
    }

    public void DisabledEnemyCanvas()
    {
        enemy.SetActive(false);
    }

    public void EnabledNPCCanvas(string name, Sprite face)
    {
        npcImage.sprite = face;
        npcNameText.text = name;
        npc.SetActive(true);
    }

    public void DisabledNPCCanvas()
    {
        npc.SetActive(false);
    }

    public void NextMeleeAttack()
    {
        if (meleeIndex == meleeSprites.Length - 1)
        {
            meleeIndex = -1;
        }
        meleeIndex++;
        meleeImage.sprite = meleeSprites[meleeIndex];
    }

    public void GetAmmunition(string ammunitionType)
    {
        switch (ammunitionType)
        {
            case "Ammunition1":
                ammunition1++;
                ammunition1Text.text = ammunition1.ToString();
                break;
            case "Ammunition2":
                ammunition2++;
                ammunition2Text.text = ammunition2.ToString();
                break;
            case "Ammunition3":
                ammunition3++;
                ammunition3Text.text = ammunition3.ToString();
                break;
        }
    }

    public void ConsumeHealth()
    {
        currentHealth -= healthCosumed;

        if (currentHealth > 0)
        {
            ChangePlayerFace();
            healthSlider.value = currentHealth;
        }
        else
        {
            lifes--;
            lifesText.text = lifes.ToString();

            if (lifes <= 0)
            {
                currentHealth = 0;
                currentEnergy = 0;
                healthSlider.value = 0;
                energySlider.value = 0;
                lifes = 0;

                //  Destroy(player_);

                // Destruye al jugador cuando no quedan vidas
                // También puedes agregar lógica de Game Over aquí
            }
            else
            {
                currentHealth = maxHealth;
                healthSlider.value = maxHealth;
                if (lifes == 1)
                {
                    playerImage.sprite = tiredFace;
                }
            }
        }

        healthText.text = currentHealth + "/" + maxHealth;
    }

    public void ActiveEnergy()
    {
        Debug.Log("entrara por lo menos");
        energyIcon.SetActive(true);
        TextMeshProUGUI textCollected = energyIcon.GetComponentInChildren<TextMeshProUGUI>();
        textCollected.text = $"{scriptAscensor.colleted + 1} / 5";
    }

    public void ActiveIconTranslate()
    {
        translateIcon.SetActive(true);
    }

    public void NewGame()
    {
        SceneManager.LoadScene(3);
    }

    public void LoadGame()
    {
        // Carga la escena del juego
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
