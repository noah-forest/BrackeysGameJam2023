using UnityEngine;
using TMPro;
using System.Collections;

public class FontRotator : MonoBehaviour
{
    public TMP_FontAsset[] fonts;   // List of fonts to rotate between
    public float framesPerSecond = 10f;   // Time interval between font changes (in seconds)

	public bool stopOnPause;

    private TMP_Text textMeshPro;
    private int currentFontIndex = 0;

	private void Awake()
	{
		textMeshPro = GetComponent<TMP_Text>();

		if (fonts.Length > 0)
		{
			textMeshPro.font = fonts[currentFontIndex];
		}
	}

	private void OnEnable()
	{
		StartCoroutine(nameof(RotateFont));
	}


	private IEnumerator RotateFont()
    {
		while (true)
		{
			currentFontIndex = (currentFontIndex + 1) % fonts.Length;
			textMeshPro.font = fonts[currentFontIndex];

			if (stopOnPause)
			{
				yield return new WaitForSeconds(1f / framesPerSecond);
			}
            else
            {
                 yield return new WaitForSecondsRealtime(1f / framesPerSecond);
            }
        }
    }
}