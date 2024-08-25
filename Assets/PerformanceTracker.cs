using Assets.Scripts.Units;
using UnityEngine;


/// <summary>
/// Coordinates the tracking of unit performance between the individual units and the UI
/// </summary>
public class PerformanceTracker : MonoBehaviour, ISlotPayloadChangeHandler, ISlotEndDragHandler
{

    public UnitPerformanceDisplay display;
    GameObject lastPayload;
    UnitPerformance storedPerformance;

    public void SlotDragEnded(GameObject payload)
    {
        lastPayload = payload;
    }

    public void SlotPayloadChanged(GameObject payload)
    {
        if (lastPayload && lastPayload != payload)
        {
            lastPayload.GetComponent<UnitController>()?.performanceUpdatedEvent.RemoveListener(UpdateDisplay);
        }
        if (payload)
        {
            UnitController unit = payload.GetComponent<UnitController>();
            unit?.performanceUpdatedEvent.AddListener(UpdateDisplay);

            ISlotItem renderer = payload.GetComponent<ISlotItem>();
            if (display)
            {
                display.InitPreview(
                    renderer?.GetSlotSprite(),
                    payload.name.Replace("(Clone)", "").Trim(),
                    unit.unitPerformanceAllTime.damageDealt
                    );
            }
        }
        else
        {
            if(display) display.ClearDisplay();
        }
    }

    public void UpdateDisplay(UnitPerformance unitPerformance)
    {
        storedPerformance = unitPerformance;

        if (display.damagePreviewText) display.damagePreviewText.text = storedPerformance.damageDealt.ToString();
    }


}
