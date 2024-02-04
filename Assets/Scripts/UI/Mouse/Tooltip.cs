using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
	public TextMeshProUGUI header;
	public TextMeshProUGUI content;

	public LayoutElement layoutElement;

	public int characterWrapLimit;

	public RectTransform rectTransform;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
	}

	public void SetText(string c, string h = "")
	{
		if (string.IsNullOrEmpty(h))
		{
			header.gameObject.SetActive(false);
		} else
		{
			header.gameObject.SetActive(true);
			header.text = h;
		}

		content.text = c;
	}

	private void Update()
	{
		if (Application.isEditor)
		{
			int headerLength = header.text.Length;
			int contentLength = content.text.Length;

			layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit);
		}

		Vector2 position = Input.mousePosition;

		float pivotX = position.x / Screen.width;
		float pivotY = position.y / Screen.height;

		rectTransform.pivot = new Vector2(-pivotX, pivotY);
		transform.position = position;
	}
}
