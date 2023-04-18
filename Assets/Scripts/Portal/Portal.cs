using Palmmedia.ReportGenerator.Core.CodeAnalysis;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.Timeline;

[RequireComponent(typeof(BoxCollider))]
public class Portal : CComponent
{
    [SerializeField]
    private Portal otherPortal;

    [SerializeField]
    private Renderer outlineRenderer;

    [SerializeField]
    private Color portalColor;

    [SerializeField]
    private LayerMask placementMask;

    [SerializeField]
    private Collider wallCollider;

    private bool isPlaced = true;

    private List<PortalableObject> portalObjects = new List<PortalableObject>();

    private Material material;
    private new Renderer renderer;
    private new BoxCollider collider;

    public override void Awake()
    {
        base.Awake();

        collider = GetComponent<BoxCollider>();
        renderer = GetComponent<Renderer>();
        material = renderer.material;
    }

    public override void Start()
    {
        base.Start();

        PlacePortal(wallCollider, transform.position, transform.rotation);
        SetColor(portalColor);
    }

    public override void Update()
    {
        base.Update();

        for(int i = 0; i < portalObjects.Count; i++)
        {
            Vector3 objPos = transform.InverseTransformPoint(portalObjects[i].transform.position);

            if(objPos.z > 0.0f)
            {
                portalObjects[i].Warp();
            }
        }
    }

    public void SetMaskID(int id)
    {
        material.SetInt("_MaskID", id);
    }

    public bool IsRendererVisible()
    {
        return renderer.isVisible;
    }

    private void OnTriggerEnter(Collider other)
    {
        var obj = other.GetComponent<PortalableObject>();
        if(obj != null)
        {
            portalObjects.Add(obj);
            obj.SetisInPortal(this, otherPortal, wallCollider);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var obj = other.GetComponent<PortalableObject>();

        if(portalObjects.Contains(obj))
        {
            portalObjects.Remove(obj);
            obj.ExitPortal(wallCollider);
        }
    }

    public bool IsPlaced()
    {
        return isPlaced;
    }

    public void PlacePortal(Collider wallCollider, Vector3 pos, Quaternion rot)
    {
        this.wallCollider = wallCollider;
        transform.position = pos;
        transform.rotation = rot;
        transform.position -= transform.forward * 0.001f;

        FixOverhangs();
        FixIntersects();
    }

    private void FixOverhangs()
    {
        var testPoints = new List<Vector3>
        {
            new Vector3(-1.1f, 0.0f, 0.1f),
            new Vector3( 1.1f, 0.0f, 0.1f),
            new Vector3( 0.0f, -2.1f, 0.1f),
            new Vector3( 0.0f, 2.1f, 0.1f)
        };

        var testDirs = new List<Vector3>
        {
            Vector3.right,
            -Vector3.right,
            Vector3.up,
            -Vector3.up
        };

        for(int i = 0; i < 4; ++i)
        {
            RaycastHit hit;
            Vector3 raycastPos = transform.TransformPoint(testPoints[i]);
            Vector3 raycastDir = transform.TransformDirection(testDirs[i]);

            if(Physics.CheckSphere(raycastPos, 0.05f, placementMask))
            {
                break;
            }
            else if(Physics.Raycast(raycastPos, raycastDir, out hit, 2.1f, placementMask))
            {
                var offset = hit.point = raycastPos;
                transform.Translate(offset, Space.World);
            }
        }
    }

    private void FixIntersects()
    {
        var testDirs = new List<Vector3>
        {
            Vector3.right,
            -Vector3.right,
            Vector3.up,
            -Vector3.up
        };

        var testDists = new List<float> { 1.1f, 1.1f, 2.1f, 2.1f };

        for ( int i = 0; i < 4; ++i)
        {
            RaycastHit hit;
            Vector3 raycastPos = transform.TransformPoint(0.0f, 0.0f, -0.1f);
            Vector3 raycastDir = transform.TransformDirection(testDirs[i]);

            if(Physics.Raycast(raycastPos, raycastDir, out hit, testDists[i], placementMask))
            {
                var offset = (hit.point - raycastPos);
                var newOffset = -raycastDir * (testDists[i] - offset.magnitude);
                transform.Translate(newOffset, Space.World);
            }
        }
    }

    public Portal GetOtherPortal()
    {
        return otherPortal;
    }

    public void SetTexture(RenderTexture tex)
    {
        material.mainTexture = tex;
    }

    public void SetColor(Color color)
    {
        material.SetColor("_Color", color);
        outlineRenderer.material.SetColor("_OutlineColor", color);
    }
}
