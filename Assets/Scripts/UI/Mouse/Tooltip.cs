using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
	public TextMeshProUGUI header;

	public int characterWrapLimit;

	public RectTransform _rectTransform;

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
		var position = Input.mousePosition;
		var normalizedPosition = new Vector2(position.x / Screen.width, position.y / Screen.height);
		var pivot = CalculatePivot(normalizedPosition);
		_rectTransform.pivot = pivot;
		transform.position = position;
	}

	private Vector2 CalculatePivot(Vector2 normalizedPosition)
	{
		var pivotTopLeft = new Vector2(-0.05f, 1.05f);
		var pivotTopRight = new Vector2(1.05f, 1.05f);
		var pivotBottomLeft = new Vector2(-0.05f, -0.05f);
		var pivotBottomRight = new Vector2(1.05f, -0.05f);

		if (normalizedPosition.x < 0.5f && normalizedPosition.y >= 0.5f)
		{
			return pivotTopLeft;
		}
		else if (normalizedPosition.x > 0.5f && normalizedPosition.y >= 0.5f)
		{
			return pivotTopRight;
		}
		else if (normalizedPosition.x <= 0.5f && normalizedPosition.y < 0.5f)
		{
			return pivotBottomLeft;
		}
		else
		{
			return pivotBottomRight;
		}
	}
}

