using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotUnitRenderer : MonoBehaviour, ISlotRenderer
{
    private GameObject spawnedUnit = null;
    public void RenderSlot(GameObject payload)
    {
        if (spawnedUnit)
        {
            Destroy(spawnedUnit);
        }

        if (payload != null)
        {
            ISlotItem slotItem = payload.GetComponent<ISlotItem>();
            spawnedUnit = Instantiate(payload);
            spawnedUnit.transform.parent = transform;
            spawnedUnit.transform.position = transform.position;
        }
    }

    public void SlotDragStarted(GameObject payload)
    {
        if (payload != null && spawnedUnit != null)
        {
            spawnedUnit.transform.Find("Sprite").GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0.7f);
        }
    }

    public void SlotDragEnded(GameObject payload)
    {
        if (payload != null && spawnedUnit != null)
        {
            spawnedUnit.transform.Find("Sprite").GetComponent<SpriteRenderer>().color = new Color(1,1,1,1);
        }
    }
}
