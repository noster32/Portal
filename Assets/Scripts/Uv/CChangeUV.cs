using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CChangeUV : CComponent
{
    private MeshFilter meshFilter;
    private Mesh mesh;

    public override void Start()
    {
        base.Start();

        meshFilter = GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;

        Vector2[] uvMap = mesh.uv;

        //front
        uvMap[0] = new Vector2(0f, 0f);
        uvMap[1] = new Vector2(1f, 0f);
        uvMap[2] = new Vector2(0f, 1f);
        uvMap[3] = new Vector2(1f, 1f);

        //back
        uvMap[10] = new Vector2(1f, 1f);
        uvMap[11] = new Vector2(0f, 1f);
        uvMap[6]  = new Vector2(1f, 0f);
        uvMap[7]  = new Vector2(0f, 0f);

        mesh.uv = uvMap;
    }
}

