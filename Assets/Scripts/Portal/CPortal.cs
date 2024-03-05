using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPortal : CComponent
{
    [Header("Setting")]
    public CPortal otherPortal;
    [SerializeField] private Renderer outlineRenderer;
    [SerializeField] private Color portalColor;

    [HideInInspector] public List<CTeleportObject> teleportObjects = new List<CTeleportObject>();
    [HideInInspector] public Collider wallCollider;

    [Header("Sound")]
    [SerializeField] private CPortalSound portalSoundPrefab;
    [SerializeField] private Transform portalSoundParent;
    [SerializeField] private AudioClip portalOpenClip;
    [SerializeField] private AudioClip portalCloseClip;

    private Material material;
    private Renderer portalRenderer;

    private Coroutine lerpCoroutine;

    private bool isPlaced = false;
    private bool isLevelPlaced = false;


    public override void Awake()
    {
        base.Awake();

        portalRenderer = GetComponent<Renderer>();
        material = portalRenderer.material;
    }

    public override void Start()
    {
        base.Start();
        SetColor(portalColor);

    }

    public override void Update()
    {
        base.Update();

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
                teleportObjects[i].Teleport();
                teleportObjects[i].ExitPortal(wallCollider);
                teleportObjects.RemoveAt(i);
                i--;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isPlaced || !otherPortal.isPlaced)
            return;

        CTeleportObject tpObject;
        if(other.tag == "Turret")
            tpObject = other.GetComponentInParent<CTeleportObject>();
        else
            tpObject = other.GetComponent<CTeleportObject> ();

        if (tpObject != null)
        {
            teleportObjects.Add(tpObject);
            tpObject.EnterPortal(this, otherPortal, wallCollider);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isPlaced || !otherPortal.isPlaced)
            return;
        CTeleportObject tpObject;
        
        if (other.tag == "Turret")
            tpObject = other.GetComponentInParent<CTeleportObject>();
        else
            tpObject = other.GetComponent<CTeleportObject> ();

        if(teleportObjects.Contains(tpObject))
        {
            teleportObjects.Remove(tpObject);
            tpObject.ExitPortal(wallCollider);
        }
    }

    public void PlacePortal(Collider collide, Vector3 pos, Quaternion rot)
    {
        if(isPlaced)
        {
            var portalCloseSoundInstance = Instantiate(portalSoundPrefab, transform.position, Quaternion.identity, portalSoundParent);
            portalCloseSoundInstance.PlayPortalSound(portalCloseClip, 1f);

            isPlaced = false;
        }

        if (lerpCoroutine != null)
        {
            StopCoroutine(lerpCoroutine);
            lerpCoroutine = null;
        }
        this.wallCollider = collide;
        transform.position = pos;
        transform.rotation = rot;
        transform.position += transform.forward * 0.001f;
        transform.localScale = Vector3.zero;

        gameObject.SetActive(true);
        lerpCoroutine = StartCoroutine(LerpPortal(0.8f, Vector3.zero, Vector3.one, true));

        var portalOpenSoundInstance = Instantiate(portalSoundPrefab, pos, Quaternion.identity, portalSoundParent);
        portalOpenSoundInstance.PlayPortalSound(portalOpenClip, 1f);
    }

    public void CleanPortal()
    {
        if (lerpCoroutine != null)
        {
            StopCoroutine(lerpCoroutine);
            lerpCoroutine = null;
        }

        var portalCloseSoundInstance = Instantiate(portalSoundPrefab, transform.position, Quaternion.identity, portalSoundParent);
        portalCloseSoundInstance.PlayPortalSound(portalCloseClip, 1f);

        isPlaced = false;
        lerpCoroutine = StartCoroutine(LerpPortal(0.1f, transform.localScale, Vector3.zero, false));
    }

    
    private IEnumerator LerpPortal(float duration, Vector3 start, Vector3 end, bool place)
    {
        //placed : 포탈을 설치하려는 경우 true 그렇지 않은 경우 false
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            transform.localScale = Vector3.Lerp(start, end, t);
            timeElapsed += Time.fixedDeltaTime;

            yield return null;
        }

        transform.localScale = end;

        if(place)
            isPlaced = true;
        else
            gameObject.SetActive(false);

        lerpCoroutine = null;
    }
    public bool IsRendererVisible()
    {
        return portalRenderer.isVisible;
    }

    public void SetTexture(Texture texture)
    {
        material.mainTexture = texture;
    }

    public bool IsPlaced()
    {
        return isPlaced;
    }

    public bool IsLevelPlaced()
    {
        return isLevelPlaced;
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
