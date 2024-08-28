using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PR_Tab : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public bool firstSelected;
    public PR_TabGroup tabGroup;
    public Image background;

    public Image icon;

    private MouseUtils mouseUtils;
    
    // Start is called before the first frame update
    private void Start()
    {
        mouseUtils = MouseUtils.singleton;
        
        tabGroup.Subscribe(this);
        if(firstSelected) tabGroup.OnTabSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseUtils.SetHoverCursor();
        tabGroup.OnTabEnter(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        tabGroup.OnTabSelected(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseUtils.SetToDefaultCursor();
        tabGroup.OnTabExit(this);
    }
}
