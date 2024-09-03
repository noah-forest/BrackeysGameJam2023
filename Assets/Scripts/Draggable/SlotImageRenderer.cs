using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class SlotImageRenderer : MonoBehaviour, ISlotPayloadChangeHandler, ISlotEndDragHandler, ISlotStartDragHandler
{
    private Image _image;
    private Image image
    {
        get
        {
            if (_image == null)
            {
                _image = GetComponent<Image>();
            }

            return _image;
        }
    }

    public void SlotPayloadChanged(GameObject payload)
    {
        if (payload != null)
        {
            ISlotItem slotItem = payload.GetComponent<ISlotItem>();

            if (slotItem != null)
            {
                image.sprite = slotItem.GetSlotSprite();
                image.transform.localScale = new Vector3(-1, 1, 1);
                image.color = new Color(1,1,1,1);
            }
        }
        else
        {
            image.sprite = null;
            image.color = new Color(0, 0, 0, 0);
        }
    }

    public void SlotDragStarted(GameObject payload)
    {
        if (payload)
        {
            image.color = new Color(0, 0, 0, 0.5f);
            image.transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    public void SlotDragEnded(GameObject payload)
    {
        if (payload)
        {
            image.color = new Color(0, 0, 0, 1);
            image.transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}
