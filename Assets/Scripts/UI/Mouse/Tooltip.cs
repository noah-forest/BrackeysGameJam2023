using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
	public TextMeshProUGUI header;
	public RectTransform _rectTransform;

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
		var pivot = CalculatePivot(normalizedPosition) * 100;

		transform.position = position + (Vector3) pivot;
	}

	private Vector2 CalculatePivot(Vector2 normalizedPosition)
	{
		float y = 1.05f;
		float right = -4.2f;
		float left = -0.05f;	

		var pivotTopLeft = new Vector2(left, y);
		var pivotTopRight = new Vector2(right, y);
		var pivotBottomLeft = new Vector2(left, y);
		var pivotBottomRight = new Vector2(right, y);

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

