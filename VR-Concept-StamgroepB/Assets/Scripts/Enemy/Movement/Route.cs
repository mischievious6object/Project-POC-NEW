using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Route : MonoBehaviour
{
    public Vector3 StartPosition => transform.position;
    public List<Vector3> Waypoints => _waypoints;

    private List<Vector3> _waypoints = new();

    private void Awake()
    {
        _waypoints = GetComponentsInChildren<Transform>().Select(t => t.position).ToList();
        DisableChildrenMeshes();
    }

    private void DisableChildrenMeshes()
    {
        MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer mesh in meshes)
        {
            mesh.enabled = false;
        }
    }

}
