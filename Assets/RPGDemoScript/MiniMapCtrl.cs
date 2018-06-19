using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapCtrl : MonoBehaviour {

    private Terrain myTerrain;

    private Transform Player;

    private GameObject row;

    Vector2 parentRectSize;

    RectTransform rowRecttrans;

    void Start ()
    {
        myTerrain = GameObject.FindGameObjectWithTag("Terrain").GetComponent<Terrain>();

        Player = GameObject.FindGameObjectWithTag("Player").transform;

        row = GameObject.FindGameObjectWithTag("row");

        parentRectSize = row.transform.parent.GetComponent<RectTransform>().rect.size;

        rowRecttrans = row.transform.GetComponent<RectTransform>();

    }
	

	void Update ()
    {
        row.transform.rotation = Quaternion.Euler(0,0,-Player.rotation.eulerAngles.y-90);

        float tmpXX = Player.position.x / myTerrain.terrainData.size.x;

        float tmpYY = Player.position.z / myTerrain.terrainData.size.z;

        tmpXX = parentRectSize.x * tmpXX;

        tmpYY = parentRectSize.y * tmpYY;

        rowRecttrans.anchoredPosition3D = new Vector3(tmpXX, tmpYY, 0);

    }
}
