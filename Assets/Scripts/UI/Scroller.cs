using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Scroller : MonoBehaviour
{

	[SerializeField] private RawImage image;
	[SerializeField] private float x, y;

    // Update is called once per frame
    void Update()
    {
		image.uvRect = new Rect(image.uvRect.position + new Vector2(x,y) * Time.deltaTime, image.uvRect.size);
    }
}
