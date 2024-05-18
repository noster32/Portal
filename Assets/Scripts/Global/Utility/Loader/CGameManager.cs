using UnityEngine;
using UnityEngine.SceneManagement;

public class CGameManager : CSingleton<CGameManager>
{
    private bool isPause = false;
    private CPlayerState playerState;


    public override void Awake()
    {
        base.Awake();

        if (m_oInstance == null)
        {
            m_oInstance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (CSceneManager.Instance.player != null)
            playerState = CSceneManager.Instance.player.transform.GetComponent<CPlayerState>();
    }

   public override void Update()
   {
        base.Update();

        if(playerState)
            if (playerState.GetIsPlayerDie())
                if (Input.anyKeyDown)
                    CSceneLoader.Instance.RestartScene();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(isPause)
                Resume();
            else
                Pause();
        }
   }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void Resume()
    {
        if (CSceneManager.Instance.pausePanel != null)
            CSceneManager.Instance.pausePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;

        isPause = false;
    }

    //aperture_vo 같은 경우에는 playoneshot으로 되어있기 떄문에 pause가 되더라도 계속해서 들릴수도
    //AudioManager에 따로 EventInstance를 만들어서 거기에 하나씩 할당하는 방법도?
    private void Pause()
    {
        if(CSceneManager.Instance.pausePanel != null)
            CSceneManager.Instance.pausePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;

        isPause = true;
    }

    public bool GetIsPaused() => isPause;

}
