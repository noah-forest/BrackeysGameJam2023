using UnityEngine;
using TMPro;

public class FontRotator : MonoBehaviour
{
    public TMP_FontAsset[] fonts;   // List of fonts to rotate between
    public float framesPerSecond = 10f;   // Time interval between font changes (in seconds)

    private TMP_Text textMeshPro;
    private int currentFontIndex = 0;

    private void Start()
    {
        textMeshPro = GetComponent<TMP_Text>();
        if (fonts.Length > 0)
        {
            textMeshPro.font = fonts[currentFontIndex];
            // Start the animation loop
            InvokeRepeating("RotateFont", 0f, 1f / framesPerSecond);
        }
    }
    private void RotateFont()
    {
        currentFontIndex = (currentFontIndex + 1) % fonts.Length;
        textMeshPro.font = fonts[currentFontIndex];
    }
}