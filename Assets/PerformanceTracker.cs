using Assets.Scripts.Units;
using UnityEngine;


/// <summary>
/// Coordinates the tracking of unit performance between the individual units and the UI
/// </summary>
public class PerformanceTracker : MonoBehaviour, ISlotPayloadChangeHandler, ISlotEndDragHandler
{

    public UnitPerformanceDisplay display;
    GameObject lastPayload;
    UnitPerformance storedPerformanceAllTime;
    UnitPerformance storedPerformanceLastBattle;

    public void SlotDragEnded(GameObject payload)
    {
        if (!display) return;
        lastPayload = payload;
    }

    public void SlotPayloadChanged(GameObject payload)
    {
        if (!display) return;
        //Debug.Log($"PT Change: {gameObject.name} Last: {(lastPayload ? lastPayload.name : "none")} Cur: {(payload ? payload.name : "none")}");
        if (lastPayload && lastPayload != payload)
        {
            lastPayload.GetComponent<UnitController>()?.performanceUpdatedEvent.RemoveAllListeners();
            //Debug.Log("removing binding from lastpayload");
        }
        if (payload)
        {
            UnitController unit = payload.GetComponent<UnitController>();
            unit?.performanceUpdatedEvent.RemoveAllListeners();
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
            if (lastPayload) lastPayload.GetComponent<UnitController>()?.performanceUpdatedEvent.RemoveAllListeners();
            //Debug.Log("Payload is null removing binding from lastpayload");

            lastPayload = null;
        }
    }

    public void UpdateDisplay(UnitPerformance unitPerformance)
    {
        storedPerformanceAllTime = unitPerformance;

        if (display.damagePreviewText) display.damagePreviewText.text = storedPerformanceAllTime.damageDealt.ToString();
    }


}
