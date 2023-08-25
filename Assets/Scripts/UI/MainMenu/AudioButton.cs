using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioButton : MonoBehaviour
{
    [SerializeField] private GameObject slider;
    
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void OnClick()
    {
        if(anim) anim.speed = 0; //pause the anim
        slider.SetActive(true);
    }

    public void SliderDeactive()
    {
        anim.speed = 1;
    }
}
