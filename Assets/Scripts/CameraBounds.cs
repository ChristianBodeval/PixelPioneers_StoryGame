using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBounds : MonoBehaviour
{
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Vector3 pos1 = Camera.main.ViewportToWorldPoint(new Vector2(1f, 1f));
        Vector3 pos2 = Camera.main.ViewportToWorldPoint(new Vector2(0f, 1f));
        Vector3 pos3 = Camera.main.ViewportToWorldPoint(new Vector2(1f, 0f));
        Vector3 pos4 = Camera.main.ViewportToWorldPoint(new Vector2(0f, 0f));
        Gizmos.color = Color.red;

        Vector3 pos5 = GetPointAtHeight(pos1);
        Vector3 pos6 = GetPointAtHeight(pos2);
        Vector3 pos7 = GetPointAtHeight(pos3);
        Vector3 pos8 = GetPointAtHeight(pos4);

        Gizmos.DrawSphere(pos1, 1);
        Gizmos.DrawSphere(pos2, 1);
        Gizmos.DrawSphere(pos3, 1);
        Gizmos.DrawSphere(pos4, 1);
        Gizmos.DrawSphere(pos5, 1);
        Gizmos.DrawSphere(pos6, 1);
        Gizmos.DrawSphere(pos7, 1);
        Gizmos.DrawSphere(pos8, 1);
        Gizmos.DrawCube(Camera.main.transform.position + (Vector3)Vector2.up * 10f, new Vector3(1f,1f,1f));
    }

    public static Vector3 GetPointAtHeight(Vector3 pos)
    {
        return new Vector3(pos.x, pos.y - pos.z, 0f);
    }
}
