using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//FIX PLAYER HP UI

public class UI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI weaponJammedNotification;
    [SerializeField]
    private Transform weaponJamNotificationStartPos;
    [SerializeField]
    private Transform weaponJamNotificationEndPos;

    [SerializeField]
    private Slider staminaSlider;

    [SerializeField]
    private Slider headlightCapacitySlider;

    private PlayerMovement player;
    private PlayerHealth playerHealth;

    [SerializeField]
    private FocusedHeadlight focusedHeadlight;

    [SerializeField]
    private Image saveGameIcon;
    [SerializeField]
    private Image saveGameBrush;
    [SerializeField]
    private Animator brushAnimator;



    //health UI
    [SerializeField]
    private Sprite[] sanitySprites;
    [SerializeField] 
    private Image sanityImage;

    [SerializeField]
    private TextMeshProUGUI healthText;

    [SerializeField] MageBoss mageBoss;


    //UI Materials
    #region StaminaBar
    [SerializeField]
    private Material staminaBarMaterial;
    private const float staminaBarMaterialDepleteSpeed = 1f;
    private const float staminaBarMaterialRegenSpeed = -3f;
    #endregion
    #region HealthBar
    [SerializeField]
    private Material healthBarMaterial;
    private const string HEALTH_BAR_OUTLINE_THICKNESS = "_OutlineThickness";
    private float healthBarRestoreTime = 0;
    private const float damageHealthBarOutlineThickness = 7;
    private const float defaultHealthBarOutlineThickness = 2;
    #endregion

    #region Boss UI
    [SerializeField]
    private Slider bossHPBar;
    private const string BOSS_FIGHT_STARTED_TRIGGER = "OnBossFightStarted";
    private const string BOSS_FIGHT_ENDED_TRIGGER = "OnBossFightEnded";

    private float nextHPBarValue = 1;
    private float smDampVelocity;

    private float smDampTime = 0.3f;
    #endregion


    // Start is called before the first frame update
    void Start()
    {
        Shoot.OnWeaponJammed += Shoot_OnWeaponJammed;

        player = GameManager.Instance.GetPlayerReference();
        playerHealth = player.GetComponent<PlayerHealth>();

        playerHealth.OnHealthChanged += Player_OnHealthChanged;
        player = GameManager.Instance.GetPlayerReference();
        player.OnStaminaStateChange += Player_OnStaminaStateChange;
        mageBoss.OnHPChanged += MageBoss_OnHPChanged;
        mageBoss.OnStageChanged += MageBoss_OnStageChanged;
        GameManager.Instance.OnBossFightStarted += Instance_OnBossFightStarted;

        SaveGame.OnGameSaved += SaveGame_OnGameSaved;

        saveGameIcon.gameObject.SetActive(false);
        saveGameBrush.gameObject.SetActive(false);
    }


    private void OnDestroy()
    {
        Shoot.OnWeaponJammed -= Shoot_OnWeaponJammed;
        playerHealth.OnHealthChanged -= Player_OnHealthChanged;
        player.OnStaminaStateChange -= Player_OnStaminaStateChange;
        mageBoss.OnHPChanged -= MageBoss_OnHPChanged;
        mageBoss.OnStageChanged -= MageBoss_OnStageChanged;
        GameManager.Instance.OnBossFightStarted -= Instance_OnBossFightStarted;

        SaveGame.OnGameSaved -= SaveGame_OnGameSaved;
    }

    private void SaveGame_OnGameSaved()
    {
        SoundManager.Instance.PlaySaveSound();
        StartCoroutine(DisableImageAfterDelay(saveGameIcon, 2));
        StartCoroutine(DisableImageAfterDelay(saveGameBrush, 2));
        brushAnimator.SetTrigger("OnSaved");
    }

    private IEnumerator DisableImageAfterDelay(Image img, float delay)
    {
        img.gameObject.SetActive(true);
        yield return new WaitForSeconds(delay);
        img.gameObject.SetActive(false);
    }

    private void MageBoss_OnStageChanged()
    {
        nextHPBarValue = 1;
        bossHPBar.GetComponent<Animator>().SetTrigger(BOSS_FIGHT_STARTED_TRIGGER);
    }

    private void Instance_OnBossFightStarted()
    {
        
        bossHPBar.GetComponent<Animator>().SetTrigger(BOSS_FIGHT_STARTED_TRIGGER);
    }

    private void MageBoss_OnHPChanged(int arg1, int arg2)
    {
        nextHPBarValue = arg1 * 1f / arg2;
        if (nextHPBarValue <= 0f)
        {
            bossHPBar.GetComponent<Animator>().SetTrigger(BOSS_FIGHT_ENDED_TRIGGER);
        }
    }

    private void Player_OnStaminaStateChange(PlayerMovement.StaminaState obj)
    {
        float speed = -1;
        switch (obj)
        {
            case PlayerMovement.StaminaState.Regen:
                speed = staminaBarMaterialRegenSpeed;
                break;
            case PlayerMovement.StaminaState.Deplete:
                speed = staminaBarMaterialDepleteSpeed;
                break;

        }
        staminaBarMaterial.SetFloat("_ScrollingSpeed", speed);
    }

    private void Player_OnHealthChanged(GameManager.PlayerHealthStatus hpStatus)
    {
        healthBarMaterial.SetFloat(HEALTH_BAR_OUTLINE_THICKNESS, damageHealthBarOutlineThickness);

        healthBarRestoreTime = 0f;
        switch (hpStatus)
        {
            case GameManager.PlayerHealthStatus.HighHP:
                sanityImage.sprite = sanitySprites[0];
                break;
            case GameManager.PlayerHealthStatus.MidHP:
                sanityImage.sprite = sanitySprites[1];
                break;
            case GameManager.PlayerHealthStatus.LowHP:
                sanityImage.sprite = sanitySprites[2];
                break;
        }

        healthText.text = (playerHealth.HealthPoint * 1.0f / playerHealth.MaxHealthPoint * 100) + "%";
    }

    #region Weapon Jam Notification

    private bool notificationShown = false;

    private readonly float jamNotificationStayDurationTotal = 1f;
    private float jamNotificationStayDuration;

    private Vector3 smDampJamNotificationVelocity;

    private readonly float smDampJamNotificationAppearTime = 0.5f;
    private readonly float smDampJamNotificationDisappearTime = 1;


    private void Shoot_OnWeaponJammed()
    {
        jamNotificationStayDuration = jamNotificationStayDurationTotal;
        
        if(!notificationShown)
            StartCoroutine(ShowJamNotification(smDampJamNotificationAppearTime));
    }
    
    private IEnumerator ShowJamNotification(float duration)
    {
        notificationShown = true;

        while(duration > 0)
        {
            weaponJammedNotification.transform.position =
                Vector3.SmoothDamp(weaponJammedNotification.transform.position,
                weaponJamNotificationEndPos.transform.position, ref smDampJamNotificationVelocity, duration);
            duration -= Time.deltaTime;
            yield return null;
        }

        StartCoroutine(StayJamNotification());
    }

    private IEnumerator StayJamNotification()
    {
        while(jamNotificationStayDuration > 0)
        {
            jamNotificationStayDuration -= Time.deltaTime;
            yield return null;
        }

        StartCoroutine(HideJamNotification(smDampJamNotificationDisappearTime));
    }

    private IEnumerator HideJamNotification(float duration)
    {
        while(duration > 0)
        {
            weaponJammedNotification.transform.position =
                Vector3.SmoothDamp(weaponJammedNotification.transform.position,
                weaponJamNotificationStartPos.transform.position, ref smDampJamNotificationVelocity, duration);
            duration -= Time.deltaTime;
            yield return null;
        }
        notificationShown = false;
    }

    #endregion    

    // Update is called once per frame
    void Update()
    {
        bossHPBar.value = Mathf.SmoothDamp(bossHPBar.value, nextHPBarValue, ref smDampVelocity, smDampTime);

        healthBarRestoreTime += Time.deltaTime;
        healthBarMaterial.SetFloat(HEALTH_BAR_OUTLINE_THICKNESS,
            Mathf.Lerp(healthBarMaterial.GetFloat(HEALTH_BAR_OUTLINE_THICKNESS),
            defaultHealthBarOutlineThickness, healthBarRestoreTime));

        staminaSlider.value = player.Stamina;

        headlightCapacitySlider.value = focusedHeadlight.CurrentFocusedLightCapacity;
    }
}
