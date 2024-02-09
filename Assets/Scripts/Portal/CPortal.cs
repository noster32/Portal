using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class CPortal : CComponent
{
    #region private
    [SerializeField] public CPortal otherPortal;
    [SerializeField] private Renderer outlineRenderer;
    [SerializeField] private Color portalColor;
    [SerializeField] private LayerMask placementMask;
    [SerializeField] private GameObject TestWall;

    private bool isPlaced = false;

    public List<CTeleportObject> teleportObjects = new List<CTeleportObject>();

    private Material material;
    private Renderer portalRenderer;

    #endregion
    #region public
    public Collider wallCollider;
    #endregion

    private AudioSource audioSource;
    [SerializeField] private AudioClip portalOpenClip;
    [SerializeField] private AudioClip[] portalTeleportClip;
    [SerializeField] private AudioClip portalSoundClip;

    public override void Awake()
    {
        base.Awake();

        portalRenderer = GetComponent<Renderer>();
        audioSource = GetComponent<AudioSource>();
        material = portalRenderer.material;
    }

    public override void Start()
    {
        base.Start();
        SetColor(portalColor);
        CSoundLoader.Instance.AudioInit(audioSource, portalSoundClip, 0.05f, true, 1f, 10f);
        CSoundLoader.Instance.SetListener();
    }

    public override void Update()
    {
        base.Update();

        CSoundLoader.Instance.PlaySound3D(transform.position, 0.25f);

        if (!isPlaced || !otherPortal.isPlaced)
            return;

        for (int i = 0; i < teleportObjects.Count; ++i)
        {
            Vector3 offsetFromPortal;

            if (teleportObjects[i].tag == "Player")
            {
                offsetFromPortal = (teleportObjects[i].transform.position + new Vector3(0f, 0.8f, 0f)) - transform.position;
            }
            else
            {
                offsetFromPortal = teleportObjects[i].transform.position - transform.position;
            }

            int dotValue = System.Math.Sign(Vector3.Dot(offsetFromPortal, transform.forward));

            if (dotValue <= 0f)
            {
                
                if (teleportObjects[i].isGrabbed)
                {
                    teleportObjects[i].GrabTeleport();
                }
                else
                {
                    teleportObjects[i].Teleport();
                    
                    //플레이어한테 사운드 나게하기
                    audioSource.PlayOneShot(portalTeleportClip[0], 0.2f);
                }

                teleportObjects[i].isInPortal = false;
                teleportObjects[i].ExitPortal(wallCollider);
                teleportObjects.RemoveAt(i);
                i--;
            }
        }
    }

    public void TeleportObjectEnterPortal(CTeleportObject teleportObject)
    {
        if(!teleportObjects.Contains(teleportObject))
        {
            teleportObjects.Add(teleportObject);
            teleportObject.EnterPortal(this, otherPortal, wallCollider);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isPlaced || !otherPortal.isPlaced)
            return;

        var tpObject = other.GetComponent<CTeleportObject> ();
        if(tpObject != null)
        {
            teleportObjects.Add(tpObject);
            tpObject.EnterPortal(this, otherPortal, wallCollider);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isPlaced || !otherPortal.isPlaced)
            return;

        var tpObject = other.GetComponent<CTeleportObject> ();
        if(teleportObjects.Contains(tpObject))
        {
            teleportObjects.Remove(tpObject);
            tpObject.ExitPortal(wallCollider);
        }
    }

    public void Warp(Transform warpObj)
    {
        warpObj.transform.position = otherPortal.transform.position;
    }

    public bool IsRendererVisible()
    {
        return portalRenderer.isVisible;
    }
    
    public void SetTexture(Texture texture)
    {
        material.mainTexture = texture;
    }

    public void PlacePortal(Collider collide, Vector3 pos, Quaternion rot)
    {
        this.wallCollider = collide;
        transform.position = pos;
        transform.rotation = rot;
        transform.position += transform.forward * 0.001f;
        transform.localScale = Vector3.zero;

        gameObject.SetActive(true);
        StartCoroutine(LerpPortal(0.8f));

        audioSource.PlayOneShot(portalOpenClip, 0.1f);
    }

    public void RemovePortal()
    {
        isPlaced = false;
        gameObject.SetActive(false);
    }
    private IEnumerator LerpPortal(float duration)
    {
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            timeElapsed += Time.fixedDeltaTime;

            yield return null;
        }

        transform.localScale = Vector3.one;
        isPlaced = true;
    }

    public bool IsPlaced()
    {
        return isPlaced;
    }

    public void SetColor(Color color)
    {
        outlineRenderer.material.SetColor("_OutlineColor", color);
    }

    public bool isVisibleFromMainCamera(Camera camera)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);

        return GeometryUtility.TestPlanesAABB(planes, portalRenderer.bounds);
    }
}
