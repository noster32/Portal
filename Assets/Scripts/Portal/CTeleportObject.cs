using System.Collections.Generic;
using UnityEngine;

public class CTeleportObject : CComponent
{
    protected CPortal portal1;
    protected CPortal portal2;

    #region portalClone
    [Header("Portal Clone Object")]
    [SerializeField] private GameObject grapicsObject;

    [HideInInspector] public GameObject grapicsClone;
    [HideInInspector] public Material[] originalMaterials;
    [HideInInspector] public Material[] cloneMaterials;
    [HideInInspector] public Animator originalAnimator;
    [HideInInspector] public Animator cloneAnimator;
    [HideInInspector] public Rigidbody objRigidbody;
    #endregion

    #region player
    [HideInInspector] public CPlayerMouseLook mouseLook;
    #endregion

    #region Grab
    [HideInInspector] public Vector3 grabPosition;                    //그랩 위치
    [HideInInspector] public bool isGrabbed;                          //현재 그랩되어 있음
    [HideInInspector] public bool isGrabbedTeleport;                  //그랩된 상태에서 텔레포트 여부
    #endregion

    protected Quaternion reverse = Quaternion.Euler(0f, 180f, 0f);

    protected new Collider collider;

    [HideInInspector] public Transform cameraTransform;
    [HideInInspector] public bool isInPortal;
    [HideInInspector] public bool portalPlacedFloor;
    private bool isPlayer;
    private bool isTurret;
    private bool isBullet;

    private CPlayerData playerData;

    public override void Awake()
    {
        base.Awake();

        isPlayer = transform.tag == "Player";
        isTurret = transform.tag == "Turret";
        isBullet = transform.tag == "Bullet";

        if (!isPlayer && !isTurret && !isBullet)
        {
            grapicsClone = new GameObject();
            grapicsClone.SetActive(false);
            var meshFilter = grapicsClone.AddComponent<MeshFilter>();
            var meshRenderer = grapicsClone.AddComponent<MeshRenderer>();

            meshFilter.mesh = grapicsObject.GetComponent<MeshFilter>().mesh;
            meshRenderer.materials = grapicsObject.GetComponent<MeshRenderer>().materials;
            grapicsClone.transform.parent = transform;
            grapicsClone.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if(isPlayer)
        {
            cameraTransform = Camera.main.transform;
            mouseLook = GetComponent<CPlayerMouseLook>();
        }

        if(isTurret)
            collider = GetComponentInChildren<Collider>();
        else
            collider = GetComponent<Collider>();

        objRigidbody = GetComponent<Rigidbody>();

        if (!objRigidbody && !isBullet)
            objRigidbody = grapicsClone.GetComponent<Rigidbody>();
    }

    public override void Start()
    {
        base.Start();

        if(isPlayer)
            playerData = CPlayerData.GetInstance();
    }

    public override void Update()
    {
        base.Update();

        //rigidbody의 velocity는 오브젝트가 월드 공간을 기준으로 몇유닛을 이동하고 있는지 나타냄
        // ex) 플레이어가 월드의 z방향으로 이동할 때엔 velo의 z값이 높게 나옴
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
        
        if(portal1 == null || portal2 == null)
        {
            return;
        }

        if(grapicsClone != null && grapicsClone.activeSelf)
        {
            Transform enterPortalTransform = portal1.transform;
            Transform exitPortalTransform = portal2.transform;

            Vector3 relativePos = enterPortalTransform.InverseTransformPoint(transform.position);
            relativePos = reverse * relativePos;
            grapicsClone.transform.position = exitPortalTransform.TransformPoint(relativePos);

            Quaternion relativeRot = Quaternion.Inverse(enterPortalTransform.rotation) * transform.rotation;
            relativeRot = reverse * relativeRot;
            grapicsClone.transform.rotation = exitPortalTransform.rotation * relativeRot;
        }
    }

    public virtual void Teleport()
    {
        Debug.Log("Tel");
        Transform enterPortalTransform = portal1.transform;
        Transform exitPortalTransform = portal2.transform;

        Vector3 relativeObjPos = enterPortalTransform.InverseTransformPoint(transform.position);
        relativeObjPos = reverse * relativeObjPos;
        transform.position = exitPortalTransform.TransformPoint(relativeObjPos);

        if(isPlayer && cameraTransform)
        {
            Quaternion relativeCamRot = Quaternion.Inverse(enterPortalTransform.rotation) * cameraTransform.rotation;
            relativeCamRot = reverse * relativeCamRot;
            Quaternion resultRot = exitPortalTransform.rotation * relativeCamRot;
            transform.rotation = Quaternion.Euler(0f, resultRot.eulerAngles.y, 0f);
            mouseLook.TeleportCameraRotation(resultRot);
        }
        else
        {
            Quaternion relativeObjRot = Quaternion.Inverse(enterPortalTransform.rotation) * transform.rotation;
            relativeObjRot = reverse * relativeObjRot;
            transform.rotation = exitPortalTransform.rotation * relativeObjRot;
        }

        if(objRigidbody)
        {
            Vector3 relativeObjVel = enterPortalTransform.InverseTransformDirection(objRigidbody.velocity);
            relativeObjVel = reverse * relativeObjVel;
            objRigidbody.velocity = exitPortalTransform.TransformDirection(relativeObjVel);
        }

        if (isPlayer && playerData.grabObject)
        {
            if (playerData.grabObject.isGrabbedTeleport)
                playerData.grabObject.isGrabbedTeleport = false;
            else if(!playerData.grabObject.isGrabbedTeleport && playerData.grabObject.isGrabbed)
                playerData.grabObject.isGrabbedTeleport = true;
        }

        var tmp = portal1;
        portal1 = portal2;
        portal2 = tmp;
    }

    public virtual void Teleport(Vector3 pos)
    {
        Debug.Log("tel2");
        Transform enterPortalTransform = portal1.transform;
        Transform exitPortalTransform = portal2.transform;

        Vector3 relativeObjPos = enterPortalTransform.InverseTransformPoint(pos);
        relativeObjPos = reverse * relativeObjPos;
        transform.position = exitPortalTransform.TransformPoint(relativeObjPos);

        Quaternion relativeObjRot = Quaternion.Inverse(enterPortalTransform.rotation) * transform.rotation;
        relativeObjRot = reverse * relativeObjRot;
        transform.rotation = exitPortalTransform.rotation * relativeObjRot;

        Vector3 relativeObjVel = enterPortalTransform.InverseTransformDirection(objRigidbody.velocity);
        relativeObjVel = reverse * relativeObjVel;
        objRigidbody.velocity = exitPortalTransform.TransformDirection(relativeObjVel);

        var tmp = portal1;
        portal1 = portal2;
        portal2 = tmp;
    }

    public virtual void GrabTeleport()
    {
        Transform enterPortalTransform = portal1.transform;
        Transform exitPortalTransform = portal2.transform;
        //여기를 수정해야됨
        Vector3 relativeObjPos = enterPortalTransform.InverseTransformPoint(transform.position);
        relativeObjPos = reverse * relativeObjPos;
        transform.position = exitPortalTransform.TransformPoint(relativeObjPos);

        Quaternion relativeObjRot = Quaternion.Inverse(enterPortalTransform.rotation) * transform.rotation;
        relativeObjRot = reverse * relativeObjRot;
        transform.rotation = exitPortalTransform.rotation * relativeObjRot;

        Vector3 relativeObjVel = enterPortalTransform.InverseTransformDirection(objRigidbody.velocity);
        relativeObjVel = reverse * relativeObjVel;
        objRigidbody.velocity = exitPortalTransform.TransformDirection(relativeObjVel);

        if (!isGrabbedTeleport)
            isGrabbedTeleport = true;
        else
            isGrabbedTeleport = false;

        var tmp = portal1;
        portal1 = portal2;
        portal2 = tmp;
    }

    public void EnterPortal(CPortal enterPortal, CPortal exitPortal, Collider wallCollider)
    {
        this.portal1 = enterPortal;
        this.portal2 = exitPortal;
        isInPortal = true;

        if(isPlayer && grapicsClone == null)
        {
            grapicsClone = Instantiate(grapicsObject);
            grapicsClone.transform.parent = grapicsObject.transform;
            grapicsClone.transform.GetChild(1).gameObject.layer = LayerMask.NameToLayer("PlayerClone"); 
            grapicsClone.transform.localScale = new Vector3(1f, 1f, 1f);

            originalMaterials = GetMaterials(grapicsObject);
            cloneMaterials = GetMaterials(grapicsClone);
            if(grapicsObject.tag == "Player")
            {
                GetAnimator(grapicsObject, grapicsClone);
            }
        }
        else if(isTurret && grapicsClone == null)
        {
            Debug.Log("test");
            grapicsClone = Instantiate(grapicsObject);
            Destroy(grapicsClone.transform.GetChild(1).gameObject);
            grapicsClone.transform.parent = grapicsObject.transform;
            grapicsClone.transform.GetChild(2).gameObject.layer = LayerMask.NameToLayer("PlayerClone");
            //grapicsObject.transform.local

            originalMaterials = GetMaterials(grapicsObject);
            cloneMaterials = GetMaterials(grapicsClone);
            if(grapicsObject.tag == "Turret")
            {
                GetAnimator(grapicsObject, grapicsClone);
            }
        }
        else
            grapicsClone.SetActive(true);

        if(wallCollider)
        {
            Debug.Log(collider.gameObject);
            Physics.IgnoreCollision(collider, wallCollider, true);
        }
    }

    public void ExitPortal(Collider wallCollider)
    {
        if(wallCollider)
            Physics.IgnoreCollision(collider, wallCollider, false);
        isInPortal = false;
        if(!isBullet)
            grapicsClone.SetActive(false);
        Debug.Log("exit");
    }

    Material[] GetMaterials(GameObject g)
    {
        SkinnedMeshRenderer smRenderer = null;
        MeshRenderer[] mRenderer = null;
        if (isPlayer)
        {
            smRenderer = g.transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();

            return smRenderer.materials;
        }
        else
        {
            mRenderer = g.GetComponentsInChildren<MeshRenderer>();

            var matList = new List<Material>();

            foreach (var renderer in mRenderer)
            {
                foreach (var mat in renderer.materials)
                {
                    matList.Add(mat);
                }
            }
            return matList.ToArray();
        }
    }

    private void GetAnimator(GameObject og, GameObject cg)
    {
        originalAnimator = og.GetComponent<Animator>();
        cloneAnimator = cg.GetComponent<Animator>();

        cloneAnimator.runtimeAnimatorController = originalAnimator.runtimeAnimatorController;
    }

    private void ChangeLayerChild(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach(Transform child in obj.transform)
        {
            ChangeLayerChild(child.gameObject, layer);
        }
    }

}
