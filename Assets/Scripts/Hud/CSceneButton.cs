using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CSceneButton : CComponent, IPointerClickHandler
{
    private CNewGameWindow window;
    [SerializeField] private Color selectedColor;
    [SerializeField] private Image outline;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] private int sceneNumber = 0;

    public override void Awake()
    {
        base.Awake();

        if(window == null)
            window = transform.parent.GetComponent<CNewGameWindow>();
        if(outline == null) 
            outline = transform.GetChild(1).GetComponent<Image>();
        if( text == null )
            text = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    public void ButtonSelected()
    {
        outline.color = selectedColor;
        text.color = selectedColor;
    }

    public void ResetColor()
    {
        outline.color = Color.black;
        text.color = Color.white;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        window.OnTabSelected(this);
    }

    public int GetSceneNumber() => sceneNumber;
}
