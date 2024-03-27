using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CMenuButton : CComponent, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Color pointerEnterColor; 
    [SerializeField] private TextMeshProUGUI textMeshPro;

    public override void Awake()
    {
        base.Awake();

        if(textMeshPro == null ) 
            textMeshPro = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.buttonClickRelease);
        textMeshPro.color = Color.white;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CAudioManager.Instance.PlayOneShot(CFMODEvents.Instance.buttonRollOver);
        textMeshPro.color = pointerEnterColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        textMeshPro.color = Color.white;
    }

    public void GameQuit()
    {
        //IDE는 실제로 컴파일되는 블럭을 보여주므로 에디터에서 작성하는 경우 else에 입력되는 코드는 회색으로 처리됨.
        #if UNITY_EDITOR
            Time.timeScale = 1f;
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Time.timeScale = 1f;
            Application.Quit();
        #endif

    }
}
