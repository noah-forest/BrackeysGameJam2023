using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CopyText : MonoBehaviour
{
	public TextMeshProUGUI textToCopy;
	private TextMeshProUGUI thisText;

	private void Awake()
	{
		thisText = GetComponent<TextMeshProUGUI>();
	}

	// Update is called once per frame
	private void Update()
    {
		if(thisText.text != textToCopy.text) 
            thisText.text = textToCopy.text;
    }
}
