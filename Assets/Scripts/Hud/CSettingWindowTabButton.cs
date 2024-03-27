using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CSettingWindowTabButton : CComponent, IPointerClickHandler
{
    [SerializeField] private Transform tabPannel;
    [SerializeField] private Color defalutColor;
    private Image buttonImage;
    private CSettingWindow window;
    private CCameraFade fade;

    public override void Awake()
    {
        base.Awake();

        buttonImage = GetComponent<Image>();
        window = transform.parent.GetComponent<CSettingWindow>();
        fade = tabPannel.GetComponent<CCameraFade>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        window.OnTabSelected(this);
    }

    public void ButtonSelected()
    {
        buttonImage.color = Color.white;
        tabPannel.gameObject.SetActive(true);
        fade.FadeOutUnscaledTime(0.3f);

    }
    
    public void ButtonReset()
    {
        buttonImage.color = defalutColor;
        fade.SetAlpha(0f);
        tabPannel.gameObject.SetActive(false);
    }
}
