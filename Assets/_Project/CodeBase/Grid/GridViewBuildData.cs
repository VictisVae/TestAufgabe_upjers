using System.Collections.Generic;
using UnityEngine;
using static CodeBase.Utilities.Constants.GridGraphics;

namespace CodeBase.Grid {
  public class GridViewBuildData {
    private readonly List<Vector3> _vertices;
    private readonly List<int> _triangles;
    private readonly List<Vector2> _uvs;

    public GridViewBuildData(int gridLengthX, int gridLengthZ) {
      int xzSum = gridLengthX * gridLengthZ;
      _vertices = new List<Vector3>(xzSum * VerticesPerQuad);
      _triangles = new List<int>(xzSum * TrianglesPerQuad);
      _uvs = new List<Vector2>(xzSum * UVsPerQuad);
    }

    public void SetVertices(Vector3 vertic) => _vertices.Add(vertic);
    public void SetTriangle(int triangle) => _triangles.Add(triangle);
    public void SetUVs(Vector2 uv) => _uvs.Add(uv);
    public void SetUVsAtIndex(Vector2 uv, int index) => _uvs[index] = uv;
    public Vector3[] GetVertices() => _vertices.ToArray();
    public int[] GetTriangles() => _triangles.ToArray();
    public Vector2[] GetUVs() => _uvs.ToArray();
    public int VerticesCount => _vertices.Count;
    public int UVsCount => _uvs.Count;
  }
}