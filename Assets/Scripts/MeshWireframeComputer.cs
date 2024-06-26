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
        // (called whenever the object is updated)
        UpdateMesh();
    }
#endif

    [ContextMenu("Update Mesh")]
    public void UpdateMesh()
    {
        if (!gameObject.activeSelf || !GetComponent<MeshRenderer>().enabled || !GetComponent<SkinnedMeshRenderer>().enabled)
            return;

        Mesh m = GetComponent<MeshFilter>().sharedMesh;
        if (m == null) return;

        // compute and store vertex colors for the
        // wireframe shader
        Color[] colors = _SortedColoring(m);

        if (colors != null)
            m.SetColors(colors);

        // Remove the longest side of each triangle
        _RemoveLongestSides(m);
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

    private void _RemoveLongestSides(Mesh mesh)
    {
        List<int> newTriangles = new List<int>();
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            int v0 = triangles[i];
            int v1 = triangles[i + 1];
            int v2 = triangles[i + 2];

            float d0 = Vector3.Distance(vertices[v0], vertices[v1]);
            float d1 = Vector3.Distance(vertices[v1], vertices[v2]);
            float d2 = Vector3.Distance(vertices[v2], vertices[v0]);

            if (d0 >= d1 && d0 >= d2)
            {
                // Remove side v0-v1
                newTriangles.Add(v1);
                newTriangles.Add(v2);
            }
            else if (d1 >= d0 && d1 >= d2)
            {
                // Remove side v1-v2
                newTriangles.Add(v2);
                newTriangles.Add(v0);
            }
            else
            {
                // Remove side v2-v0
                newTriangles.Add(v0);
                newTriangles.Add(v1);
            }
        }

        mesh.triangles = newTriangles.ToArray();
        mesh.RecalculateNormals();
    }
}
