using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-20)]
public class CSceneManager : CSingleton<CSceneManager>
{
    [Header("Manager")]
    [SerializeField] private CAudioManager audioManager;
    [SerializeField] private CFMODEvents fMODEvents;
    [SerializeField] private CSceneLoader sceneLoader;
    [SerializeField] private CScriptManager scriptManager;

    [Header("Player")]
    public CPlayerMovement player;
    public CPortalPair portalPair;

    [Header("UI")]
    public CSubtitle subtitle;
    public GameObject pausePanel;

    [Header("Start Point")]
    public GameObject startElevatorGameObject;

    public override void Awake()
    {
        base.Awake();

        if (m_oInstance != null)
        {
            Debug.Log("There can only be one SceneManager");
        }
        else
            m_oInstance = this;
        if (audioManager == null)
            audioManager = FindObjectOfType<CAudioManager>();
        if (fMODEvents == null)
            fMODEvents = FindObjectOfType<CFMODEvents>();
        if (sceneLoader == null)
            sceneLoader = FindObjectOfType<CSceneLoader>();
        if (scriptManager == null)
            scriptManager = FindObjectOfType<CScriptManager>();
        if (player == null)
            player = FindObjectOfType<CPlayerMovement>();
        if (portalPair == null)
            portalPair = FindObjectOfType<CPortalPair>();
        if (subtitle == null)
            subtitle = FindObjectOfType<CSubtitle>();
        if (pausePanel == null)
            pausePanel = GameObject.FindWithTag("PausePanel");
    }

    public override void Start()
    {
        base.Start();

        if (CAudioManager.Instance == null)
            audioManager.gameObject.SetActive(true);
        else
            Debug.Log("audioManager already have");
        if (CFMODEvents.Instance == null)
            fMODEvents.gameObject.SetActive(true);
        else
            Debug.Log("fmodEvents already have");

        if (CSceneLoader.Instance == null)
            sceneLoader.gameObject.SetActive(true);
        else
            Debug.Log("sceneLoader already have");

        if (CScriptManager.Instance == null)
            scriptManager.gameObject.SetActive(true);
        else
            Debug.Log("ScriptManager already have");
    }
}
