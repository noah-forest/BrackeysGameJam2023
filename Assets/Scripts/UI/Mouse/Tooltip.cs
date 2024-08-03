using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
	public RectTransform rectTransform;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
	}

	// Update is called once per frame
	void Update()
    {
		Vector2 position = Input.mousePosition;

		float pivotX = position.x / Screen.width;
		float pivotY = position.y / Screen.height;

		rectTransform.pivot = new Vector2(pivotX, pivotY);
		transform.position = position;
    }
}
