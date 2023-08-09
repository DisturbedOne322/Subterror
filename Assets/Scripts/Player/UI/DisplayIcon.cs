using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayIcon : MonoBehaviour
{
    [SerializeField]
    private Sprite icon;

    private Image iconRenderer;

    private IsPlayerInRange playerInRange;

    private float maxAlpha = 1;
    private float minAlpha = 0;

    float smDampVelocity;
    private float smDampSpeed = 0.05f;

    private bool gameRecentrlySaved;

    private bool canDisplayIcon = true;

    private SaveGame saveGame;

    private void Start()
    {
        iconRenderer = GetComponent<Image>();
        iconRenderer.sprite = icon;
        playerInRange = GetComponentInParent<IsPlayerInRange>();
        playerInRange.OnPlayerInRange += PlayerInRange_OnPlayerInRange;

        saveGame = GetComponentInParent<SaveGame>();

        if (saveGame != null)
        {
            GetComponentInParent<SaveGame>().OnDisplayIcon += DisplayIcon_OnDisplayIcon;
            GetComponentInParent<CheckCanSaveGame>().CanSaveGame += DisplayIcon_CanSaveGame;
        }
    }

    private void OnDestroy()
    {
        playerInRange.OnPlayerInRange -= PlayerInRange_OnPlayerInRange;
        if (saveGame != null)
        {
            GetComponentInParent<SaveGame>().OnDisplayIcon += DisplayIcon_OnDisplayIcon;
            GetComponentInParent<CheckCanSaveGame>().CanSaveGame += DisplayIcon_CanSaveGame;
        }
    }

    private void DisplayIcon_CanSaveGame(bool canSave)
    {
        canDisplayIcon = canSave;
    }

    private void DisplayIcon_OnDisplayIcon()
    {
        gameRecentrlySaved = true;
    }

    private void PlayerInRange_OnPlayerInRange(bool inRange)
    {
        if (!inRange)
            gameRecentrlySaved = false;

        float targetAlpha = inRange ? maxAlpha : minAlpha;

        if (gameRecentrlySaved)
            targetAlpha = minAlpha;

        if (!canDisplayIcon)
            targetAlpha = minAlpha;

        Color tempColor = iconRenderer.color;
        tempColor.a = Mathf.SmoothDamp(tempColor.a, targetAlpha, ref smDampVelocity, smDampSpeed);

        iconRenderer.color = tempColor;
    }
}
