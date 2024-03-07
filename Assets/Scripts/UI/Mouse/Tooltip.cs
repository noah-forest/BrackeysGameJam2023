using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
	public TextMeshProUGUI header;

	public int characterWrapLimit;

	public RectTransform rectTransform;

	public bool switchSide;

	public void SetText(string h = "")
	{
		if (string.IsNullOrEmpty(h))
		{
			header.gameObject.SetActive(false);
		} else
		{
			header.gameObject.SetActive(true);
			header.text = h;
		}
	}

	private void Update()
	{
		Vector2 position = Input.mousePosition;
		float pivotX;
		float pivotY;

		pivotX = position.x / Screen.width / 2;
		pivotY = position.y / Screen.height / 2;

		rectTransform.pivot = new Vector2(-pivotX, pivotY);

		if (switchSide)
		{
			rectTransform.pivot = new Vector2(pivotX, pivotY);
			transform.position = position;
		}

		transform.position = position;
	}
}

