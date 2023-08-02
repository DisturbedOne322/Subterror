using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class Checkbox : MonoBehaviour
{
    private Image image;

    private Color visibleColor;
    private Color invisibleColor;

    [SerializeField]
    private OptionsSO options;

    private void Start()
    {                          
        image = GetComponent<Image>();
        visibleColor = image.color;
        visibleColor.a = 1;
        invisibleColor = image.color;
        invisibleColor.a = 0;

        image.color = options.breatingEnabled ? visibleColor : invisibleColor;
    }

    public void SwitchState()
    {
        image.color = options.breatingEnabled ? invisibleColor : visibleColor;
        options.breatingEnabled = !options.breatingEnabled;

    }
}
