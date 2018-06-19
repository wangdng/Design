using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexAnim : MonoBehaviour
{

    private Mesh msh;

    Vector3[] vertexs;

    Vector3[] Normals;
    void Start ()
    {
        msh = GetComponent<MeshFilter>().mesh;
        vertexs = msh.vertices;

        Normals = msh.normals;
    }

    void ChangeMesh()
    {
        float hand = Input.GetAxis("Horizontal") * 50 * Time.deltaTime;

        msh = GetComponent<MeshFilter>().mesh;

        vertexs = msh.vertices;

        Normals = msh.normals;

        Vector3[] newVertexs = new Vector3[vertexs.Length];

        Vector3[] newNormals = new Vector3[Normals.Length];

        for (int i = 0; i < vertexs.Length; i++)
        {
            newVertexs[i] = ChangeVertex(vertexs[i], vertexs[i].y * hand);

            newNormals[i] = ChangeVertex(Normals[i], Normals[i].y * hand);
        }

        msh.vertices = newVertexs;

        msh.normals = newNormals;

        msh.RecalculateBounds();
        msh.RecalculateNormals();

    }

    Vector3 ChangeVertex(Vector3 pos, float t)
    {
        float st = Mathf.Sin(t);

        float ct = Mathf.Cos(t);

        Vector3 reault = Vector3.zero;

        reault.x = pos.x * ct - pos.z * st;

        reault.y = pos.y;

        reault.z = st * pos.x + ct * pos.z;

        return reault;
    }
	
	// Update is called once per frame
	void Update ()
    {
        ChangeMesh();
    }
}
