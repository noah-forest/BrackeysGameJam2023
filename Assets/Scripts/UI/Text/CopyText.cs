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
	void Update()
    {
		thisText.text = textToCopy.text;
    }
}
