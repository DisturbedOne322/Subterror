using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIBossSubtitles : MonoBehaviour
{
    private TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void SetText(string subs)
    {
        text.text = subs;
        StartCoroutine(IncreaseAlpha());
    }

    private IEnumerator IncreaseAlpha()
    {
        while(text.color.a < 1)
        {
            Color temp = text.color;
            temp.a += Time.deltaTime;
            text.color = temp;
            yield return null;  
        }
        StartCoroutine(DecreaseAlpha());
    }

    private IEnumerator DecreaseAlpha()
    {
        while (text.color.a > 0)
        {
            Color temp = text.color;
            temp.a -= Time.deltaTime;
            text.color = temp;
            yield return null;
        }
    }
}
