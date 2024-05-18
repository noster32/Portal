using UnityEngine;

public class CTurretLaser : CComponent
{
    private LineRenderer lineRenderer;
    [SerializeField] private LineRenderer lineRendererThroughPortal;

    private Vector3 lineEndPosition;
    private Vector3 portalLaserStartPosition;
    private Vector3 portalLaserEndPosition;

    public override void Awake()
    {
        base.Awake();

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRendererThroughPortal.positionCount = 2;
        lineRenderer.SetPosition(0, this.transform.position);
        lineRenderer.SetPosition(1, this.transform.position);
        lineEndPosition = this.transform.position;
    }
     
    public override void Update()
    {
        base.Update();

        lineRenderer.SetPosition(0, this.transform.position);
        lineRenderer.SetPosition(1, lineEndPosition);

        if(lineRendererThroughPortal.enabled)
        {
            lineRendererThroughPortal.SetPosition(0, portalLaserStartPosition);
            lineRendererThroughPortal.SetPosition(1, portalLaserEndPosition);
        }
    }

    public void SetEndPoint(Vector3 position) => lineEndPosition = position;

    public void DisableLaser() => lineRenderer.enabled = false;

    public void EnableLaserThroughPortal()
    {
        if (!lineRendererThroughPortal.enabled) 
            lineRendererThroughPortal.enabled = true;
    }

    public void DisableLaserThroughPortal() => lineRendererThroughPortal.enabled = false;

    public void SetPointPortalLaser(Vector3 start,  Vector3 end)
    {
        portalLaserStartPosition = start;
        portalLaserEndPosition = end;
    }

}