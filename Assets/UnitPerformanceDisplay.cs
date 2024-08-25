using Assets.Scripts.Units;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Handles the display of unit performance to UI
/// </summary>
public class UnitPerformanceDisplay : MonoBehaviour
{
    public TextMeshProUGUI damagePreviewText;
    public TextMeshProUGUI namePreviewText;
    [SerializeField]
    public UnityEngine.UI.Image previewIcon;

    private void Start()
    {
        ClearDisplay();
    }

    public void InitPreview(Sprite icon , string name, float damage)
    {
        if (previewIcon != null) previewIcon.sprite = icon;
        if (namePreviewText) namePreviewText.text = name;
        if (damagePreviewText) damagePreviewText.text = damage.ToString();
    }

    public void ClearDisplay()
    {
        previewIcon.sprite = null;
        namePreviewText.text = "No Unit";
        damagePreviewText.text = 0.ToString();
    }
}
