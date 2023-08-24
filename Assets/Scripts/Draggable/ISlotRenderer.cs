using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface ISlotRenderer
{
    public void RenderSlot(GameObject payload);
    public void SlotDragStarted(GameObject payload);

    public void SlotDragEnded(GameObject payload);
}
