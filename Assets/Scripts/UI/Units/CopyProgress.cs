using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CopyProgress : MonoBehaviour
{
    public Image progressToCopy;
    private Image thisImage;

    private void Awake()
    {
        thisImage = GetComponent<Image>();
    }

    private void Update()
    {
        if (!Mathf.Approximately(thisImage.fillAmount, progressToCopy.fillAmount))
        {
            thisImage.fillAmount = progressToCopy.fillAmount;
        }
    }
}
