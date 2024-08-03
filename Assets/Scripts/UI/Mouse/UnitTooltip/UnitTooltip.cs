using TMPro;
using UnityEngine;

public class UnitTooltip : MonoBehaviour
{
	public TextMeshProUGUI header;
	public RectTransform _rectTransform;

	public void SetText(string h = "")
	{
		if (string.IsNullOrEmpty(h))
		{
			header.gameObject.SetActive(false);
		}
		else
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
		var pivotLeft = new Vector2(-0.1f, 0.75f);
		var pivotTopRight = new Vector2(1.1f, 0.75f);
		var pivotBottomRight = new Vector2(1.1f, 0.5f);

		if (normalizedPosition.x < 0.5f && normalizedPosition.y >= 0.5f || normalizedPosition.x <= 0.5f && normalizedPosition.y < 0.5f)
		{
			return pivotLeft;
		}
		else if (normalizedPosition.x > 0.5f && normalizedPosition.y >= 0.425f)
		{
			return pivotTopRight;
		}
		else
		{
			return pivotBottomRight;
		}
	}
}

