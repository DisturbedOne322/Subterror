using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HintDisplay : MonoBehaviour
{
    [SerializeField]
    private GameObject hintHolder;

    [SerializeField] 
    private Material hintMaterial;
    private const string PROGRESS = "_Progress";

    [SerializeField]
    private TextMeshProUGUI hintText;

    private AudioSource audioSource;

    [SerializeField]
    private AudioClip hintOpenAudioClip;
    [SerializeField]
    private AudioClip hintCloseAudioClip;

    private float showTimeInSeconds = 0.33f;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        hintHolder.SetActive(false);
        Hint.OnDisplayHint += Hint_OnDisplayHint;
        Hint.OnPlayerNotInRange += Hint_OnPlayerNotInRange;
        InputManager.Instance.OnPauseAction += Instance_OnPauseAction;
    }

    private void OnDestroy()
    {
        Hint.OnDisplayHint -= Hint_OnDisplayHint;
        Hint.OnPlayerNotInRange -= Hint_OnPlayerNotInRange;
        InputManager.Instance.OnPauseAction -= Instance_OnPauseAction;
    }

    private void Instance_OnPauseAction()
    {
        CloseHint();
    }

    private void Hint_OnPlayerNotInRange()
    {
        CloseHint();
        hintHolder.SetActive(false);
    }

    private void CloseHint()
    {
        if (hintHolder.activeSelf)
        {
            audioSource.PlayOneShot(hintCloseAudioClip);
        }
    }

    private void Hint_OnDisplayHint(HintSO hint)
    {
        if(!hintHolder.activeSelf)
        {
            hintHolder.SetActive(true);

            audioSource.PlayOneShot(hintOpenAudioClip);
            hintText.text = hint.Text.Replace("\\n", "\n");
            hintText.color = hint.FontColor;
            hintText.fontSize = hint.FontSize;

            StartCoroutine(ShowHint());
        }
        else
        {
            CloseHint();
            hintHolder.SetActive(false);
        }
    }

    private IEnumerator ShowHint()
    {
        Color color = hintText.color;

        for (float i = 0; i < 1; i+= Time.deltaTime / showTimeInSeconds)
        {
            hintMaterial.SetFloat(PROGRESS, i);
            color.a = i;
            hintText.color = color;
            yield return null;
        }
        hintMaterial.SetFloat(PROGRESS, 1);
        color.a = 1;
        hintText.color = color;
    }
}
