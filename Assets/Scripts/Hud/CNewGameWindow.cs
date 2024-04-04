using System;
using UnityEngine;
using UnityEngine.UI;

public class CNewGameWindow : CComponent
{
    [SerializeField] private CSceneButton[] sceneButtons;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;

    private CSceneButton[][] sceneButtonsGroup;

    //page start = 0
    private int currentPage = 0;
    private int previousPage = 0;
    private int maxPage = 0;
    private CSceneButton selectedScene;

    public override void Awake()
    {
        maxPage = (sceneButtons.Length - 1) / 3;

        sceneButtonsGroup = new CSceneButton[maxPage + 1][];

        for(int i = 0; i < sceneButtonsGroup.Length; ++i) 
        {
            sceneButtonsGroup[i] = new CSceneButton[3];

            for(int j = 0; j < sceneButtonsGroup[i].Length; ++j)
            {
                sceneButtonsGroup[i][j] = sceneButtons[(i * 3) + j];
            }
        }

        ButtonVisible();

    }

    public void OnTabSelected(CSceneButton button)
    {
        int index = Array.IndexOf(sceneButtons, button);
        int page = index / 3;
        ResetButton(page);
        button.ButtonSelected();
        selectedScene = button;
    }

    public void PreviousScenePage()
    {
        previousPage = currentPage;
        --currentPage;

        if (currentPage < 0)
        {
            currentPage = 0;
            return;
        }

        foreach (CSceneButton button in sceneButtonsGroup[previousPage])
        {
            button.gameObject.SetActive(false);
        }
        foreach (CSceneButton button in sceneButtonsGroup[currentPage])
        {
            button.gameObject.SetActive(true);
        }

        ButtonVisible();
    }

    public void NextScenePage()
    {
        previousPage = currentPage;
        ++currentPage;

        if(currentPage > maxPage)
        {
            currentPage = maxPage;
            return;
        }

        foreach (CSceneButton button in sceneButtonsGroup[previousPage])
        {
            button.gameObject.SetActive(false);
        }
        foreach (CSceneButton button in sceneButtonsGroup[currentPage])
        {
            button.gameObject.SetActive(true);
        }

        ButtonVisible();
    }

    public void StartScene()
    {
        if(selectedScene != null)
        { 
            if(CGameManager.Instance.GetIsPaused())
            {
                CGameManager.Instance.Resume();
            }

            CSceneLoader.Instance.LoadScene(selectedScene.GetSceneNumber());
        }
    }

    public void OpenWindow()
    {
        this.gameObject.SetActive(true);
    }

    public void CloseWindow()
    {
        this.gameObject.SetActive(false);
    }

    public void ResetButton(int page)
    {
        foreach (CSceneButton button in sceneButtonsGroup[page])
        {
            button.ResetColor();
        }
    }
    
    private void ButtonVisible()
    {
        if (currentPage == 0)
            previousButton.gameObject.SetActive(false);
        else
            previousButton.gameObject.SetActive(true);

        if (currentPage == maxPage)
            nextButton.gameObject.SetActive(false);
        else
            nextButton.gameObject.SetActive(true);
    }
}
