using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshWireframeComputer : MonoBehaviour
{
    private static Color[] _COLORS = new Color[]
    {
        Color.red,
        Color.green,
        Color.blue,
    };

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Called whenever the object is updated
        UpdateMesh();
    }
#endif

    [ContextMenu("Update Mesh")]

    private void Awake()
    {
        UpdateMesh();
    }
    public void UpdateMesh()
    {
        if (!gameObject.activeSelf)
            return;

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        SkinnedMeshRenderer skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();

        if (meshRenderer == null && skinnedMeshRenderer == null)
            return;

        if (meshRenderer != null && !meshRenderer.enabled)
            return;

        if (skinnedMeshRenderer != null && !skinnedMeshRenderer.enabled)
            return;

        Mesh mesh = null;
        if (meshRenderer != null)
        {
            mesh = GetComponent<MeshFilter>().sharedMesh;
        }
        else if (skinnedMeshRenderer != null)
        {
            mesh = skinnedMeshRenderer.sharedMesh;
        }

        if (mesh == null) return;

        // Compute and store vertex colors for the wireframe shader
        Color[] colors = _SortedColoring(mesh);

        if (colors != null)
            mesh.SetColors(colors);
    }

    private Color[] _SortedColoring(Mesh mesh)
    {
        int n = mesh.vertexCount;
        int[] labels = new int[n];

        List<int[]> triangles = _GetSortedTriangles(mesh.triangles);
        triangles.Sort((int[] t1, int[] t2) =>
        {
            int i = 0;
            while (i < t1.Length && i < t2.Length)
            {
                if (t1[i] < t2[i]) return -1;
                if (t1[i] > t2[i]) return 1;
                i += 1;
            }
            if (t1.Length < t2.Length) return -1;
            if (t1.Length > t2.Length) return 1;
            return 0;
        });

        foreach (int[] triangle in triangles)
        {
            List<int> availableLabels = new List<int>() { 1, 2, 3 };
            foreach (int vertexIndex in triangle)
            {
                if (availableLabels.Contains(labels[vertexIndex]))
                    availableLabels.Remove(labels[vertexIndex]);
            }
            foreach (int vertexIndex in triangle)
            {
                if (labels[vertexIndex] == 0)
                {
                    if (availableLabels.Count == 0)
                    {
                        Debug.LogError("Could not find color");
                        return null;
                    }
                    labels[vertexIndex] = availableLabels[0];
                    availableLabels.RemoveAt(0);
                }
            }
        }

        Color[] colors = new Color[n];
        for (int i = 0; i < n; i++)
            colors[i] = labels[i] > 0 ? _COLORS[labels[i] - 1] : _COLORS[0];

        return colors;
    }

    private List<int[]> _GetSortedTriangles(int[] triangles)
    {
        List<int[]> result = new List<int[]>();
        for (int i = 0; i < triangles.Length; i += 3)
        {
            List<int> t = new List<int> { triangles[i], triangles[i + 1], triangles[i + 2] };
            t.Sort();
            result.Add(t.ToArray());
        }
        return result;
    }
}
