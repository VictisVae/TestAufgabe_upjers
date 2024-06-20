using CodeBase.Utilities;
using UnityEngine;

namespace CodeBase.Grid {
  public class GameGridView : MonoBehaviour {
    private const float ArrowNull = 0.5f;
    [SerializeField]
    private Transform _ground;
    [SerializeField]
    private MeshFilter _arrowsMeshFilter;
    [SerializeField]
    private MeshRenderer _arrowsMeshRenderer;
    [SerializeField]
    private Material _gridMaterial;
    private GridViewBuildData _gridViewBuildData;
    private Mesh _mesh;

    public void InitializeGroundView(Vector2Int gridSize) {
      _ground.localScale = new Vector3(gridSize.x, gridSize.y, 1.0f);
      _ground.localPosition = new Vector3(gridSize.x * Constants.Math.Half, 0, gridSize.y * Constants.Math.Half);
    }

    public void InitializeArrowsView(GridTile[,] gridTileArray, float cellSize, float arrowConfigSize) {
      _arrowsMeshRenderer.sharedMaterial = new Material(_gridMaterial);
      _mesh = new Mesh();
      float arrowSize = ArrowNull - ArrowNull * arrowConfigSize;
      arrowSize *= cellSize;
      float offset = cellSize - arrowSize;
      int lengthX = gridTileArray.GetLength(0);
      int lengthZ = gridTileArray.GetLength(1);
      _gridViewBuildData = new GridViewBuildData(lengthX, lengthZ);

      for (int x = 0; x < lengthX; x++) {
        for (int z = 0; z < lengthZ; z++) {
          Vector3 gridPosition = new Vector3(x, 0, z);
          Vector2 uvPosition = new Vector2(x, z);
          GenerateTileQuad(arrowSize, gridPosition, offset);
          AddLastQuadVertices();
          GenerateTileUVs(gridTileArray[x,z], uvPosition);
        }
      }

      _mesh.vertices = _gridViewBuildData.GetVertices();
      _mesh.triangles = _gridViewBuildData.GetTriangles();
      _mesh.uv = _gridViewBuildData.GetUVs();
      _mesh.RecalculateBounds();
      _mesh.RecalculateNormals();
      _mesh.name = "ArrowMesh";
      _arrowsMeshFilter.mesh = _mesh;
    }

    public void UpdateArrowView(int[] uvIndexes, Vector2[] uvsDirection) {
      for (int i = 0; i < uvIndexes.Length; i++) {
        _gridViewBuildData.SetUVsAtIndex(uvsDirection[i], uvIndexes[i]);
      }
      
      _mesh.uv = _gridViewBuildData.GetUVs();
    }

    private void GenerateTileQuad(float arrowSize, Vector3 gridPosition, float offset) {
      _gridViewBuildData.SetVertices(new Vector3(arrowSize, 0, arrowSize) + gridPosition);
      _gridViewBuildData.SetVertices(new Vector3(offset, 0, arrowSize) + gridPosition);
      _gridViewBuildData.SetVertices(new Vector3(arrowSize, 0, offset) + gridPosition);
      _gridViewBuildData.SetVertices(new Vector3(offset, 0, offset) + gridPosition);
    }

    private void AddLastQuadVertices() {
      int verticesCount = _gridViewBuildData.VerticesCount;
      _gridViewBuildData.SetTriangle(verticesCount - 4);
      _gridViewBuildData.SetTriangle(verticesCount - 2);
      _gridViewBuildData.SetTriangle(verticesCount - 3);
      _gridViewBuildData.SetTriangle(verticesCount - 2);
      _gridViewBuildData.SetTriangle(verticesCount - 1);
      _gridViewBuildData.SetTriangle(verticesCount - 3);
    }

    private void GenerateTileUVs(GridTile gridTile, Vector2 uvPosition) {
      Vector2[] uvsUpDirected = GridUVData.GetUVDirectionUp();
      int[] uvIndexes = new int[Constants.GridGraphics.UVsPerQuad];

      for (int i = 0; i < uvsUpDirected.Length; i++) {
        uvIndexes[i] = _gridViewBuildData.UVsCount;
        _gridViewBuildData.SetUVs(uvsUpDirected[i] + uvPosition);
      }

      gridTile.SetUVData(new GridUVData(uvIndexes));
    }
  }
}