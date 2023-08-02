using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class smoothHPBarChange : MonoBehaviour
{
    private float nextValue;

    private Slider slider;
    private float smDampVelocity;

    private void Start()
    {
        slider = GetComponent<Slider>();
        nextValue = 1;
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = Mathf.SmoothDamp(slider.value, nextValue,ref smDampVelocity, 1f);
    }

    public void SmoothChangeHPBar(float nextValue)
    {
        this.nextValue = nextValue;
    }
}
