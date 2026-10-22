using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class UIButtonHover : UIButton, IPointerEnterHandler, IPointerExitHandler
{
    private void Start()
    {
        UnselectButton();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SelectButton();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UnselectButton();
    }

    private void OnEnable()
    {
        UnselectButton();
    }
}
