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
    public static UIController Instance { get; private set; }

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
    public GameObject livesIndicator;
    public Transform pos3Vidas;
    public Transform pos2Vidas;
    public Transform pos1Vida;
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
    public GameObject bootsIcon;
    public GameObject pioletIcon;
    //public GameObject[] Plus;

    [Header("Map")]
    public GameObject map;
    bool isMapOpen = false;
    bool canToggleMap = true;

    private Animator animator;

    [Header("Dead_Player")]
    [SerializeField]
    private GameObject deadPlayer,
        slicedPlayer;

    [Header("Flash")]
    public GameObject flash;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }
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

        //lifesText = GameObject.Find("LifeText").GetComponent<TextMeshProUGUI>();
        UpdateLivesIndicator();
        playerImage = GameObject.Find("PlayerImage").GetComponent<Image>();
        if (!haGuardado)
        {
            currentHealth = maxHealth;
            currentEnergy = maxEnergy;
        }
        healthText.text = currentHealth + "/" + maxHealth;
        energyText.text = currentEnergy + "/" + maxEnergy;
        UpdateLivesIndicator();
        //lifesText.text = lifes.ToString();

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

    public void Flash()
    {
        StartCoroutine(EnableFlash());
    }

    private IEnumerator EnableFlash()
    {
        flash.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        flash.SetActive(false);
    }

    private void Update()
    {
        healthSlider.value = currentHealth;
        energySlider.value = currentEnergy;

        if (lifes <= 0)
        {
            lifes = 0;
            UpdateLivesIndicator();
            //lifesText.text = lifes.ToString();
        }

        if (recoveringHealth)
        {
            energySlider.value = maxEnergy;
            currentEnergy = maxEnergy;
            energyText.text = currentEnergy + "/" + maxEnergy;
            energyTime = 0;
        }

        if (canToggleMap && Input.GetKeyDown(KeyCode.M))
        {
            if (PauseMenu.IsPaused)
                return;
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

        if (PauseGameManager.Instance != null)
            PauseGameManager.Instance.SetPausedByMap(isMapOpen);

        yield return new WaitForSecondsRealtime(1f);
        canToggleMap = true;
    }

    // --- CORRECCIÓN APLICADA AQUÍ ---
    public void ConsumeHealth(float damagePercentage)
    {
        // En lugar de tener dos lógicas separadas, llamamos a la función principal
        ConsumeHealth();
    }

    public bool ConsumeEnergy()
    {
        if (currentEnergy < energyCosumed)
        {
            Inventory inventory = FindFirstObjectByType<Inventory>();

            if (inventory != null)
                inventory.RequestFeedbackNoMana();

            return false;
        }

        currentEnergy -= energyCosumed;

        if (currentEnergy < 0)
            currentEnergy = 0;

        energySlider.value = currentEnergy;
        energyText.text = currentEnergy + "/" + maxEnergy;

        return true;
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

    public void RecoverHealth(int amount)
    {
        Debug.Log("recuperasión");

        if (currentHealth >= maxHealth)
            return;

        currentHealth += amount;

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        healthSlider.value = currentHealth;
        healthText.text = currentHealth + "/" + maxHealth;
    }

    public void RecoverEnergy(int amount)
    {
        Debug.Log("recuperasion");

        if (energySlider.value < energySlider.maxValue)
        {
            currentEnergy += amount;
            if (currentEnergy > maxEnergy) currentEnergy = maxEnergy;

            energySlider.value = currentEnergy;
        }

        energyText.text = currentEnergy + "/" + maxEnergy;
    }

    public void UpdateLivesIndicator()
    {
        Transform targetPos = lifes switch
        {
            3 => pos3Vidas,
            2 => pos2Vidas,
            1 => pos1Vida,
            _ => pos1Vida
        };

        livesIndicator.GetComponent<RectTransform>().position = targetPos.position;
    }


    public void RecoverEnergy() { }

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

    // --- LÓGICA DE MUERTE CORREGIDA ---
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
            lifes--; // Pierde una vida
            UpdateLivesIndicator();
            //lifesText.text = lifes.ToString();

            if (lifes > 0)
            {
                // Todavía tiene vidas: reset de salud al 100%
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

    public void ActiveIconBoots()
    {
        bootsIcon.SetActive(true);
    }

    public void ActiveIconPiolet()
    {
        pioletIcon.SetActive(true);
    }

    public void NewGame()
    {
        SceneManager.LoadScene(3);
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
