using UnityEngine;

public static class MeshGenerator {

    public static void updateMesh(int size, float heightMultiplier, float[,] heightMap, Mesh mesh) {
        int nrOfVertices = mesh.vertexCount;
        Vector3[] vertices = new Vector3[nrOfVertices];
        float topLeftX = (size - 1) / -2f;
        float topLeftZ = (size - 1) / 2f;

        int vertexIndex = 0;

        for(int y = 0; y < size; y++) {
            for(int x = 0; x < size; x++) {
                float currentHeight = heightMap [x, y];
                vertices [vertexIndex] = new Vector3 (topLeftX + x, currentHeight * currentHeight * heightMultiplier, topLeftZ - y);
                vertices [vertexIndex + 1] = new Vector3 (topLeftX + x, currentHeight * currentHeight * heightMultiplier, topLeftZ - y);
                vertexIndex ++;
            }
        }

        mesh.vertices = vertices;
        RecalculateNormals(mesh);

    }

    public static void RecalculateNormals(Mesh mesh) {
        int[] triangles = mesh.triangles;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = new Vector3[vertices.Length];

        for(int i = 0; i < triangles.Length; i += 3) {
            Vector3 a = vertices[triangles[i]];
            Vector3 b = vertices[triangles[i + 1]];
            Vector3 c = vertices[triangles[i + 2]];

            Vector3 ab = b - a;
            Vector3 ac = c - a;
            Vector3 normal = Vector3.Cross(ab, ac);

            normals[triangles[i]] = normal;
            normals[triangles[i + 1]] = normal;
            normals[triangles[i + 2]] = normal;

        }

        for(int i = 0; i < normals.Length; i++) {
            normals[i].Normalize();
        }

        mesh.normals =  normals;

    }

    public static MeshData createMesh(bool flatShading, int size, float heightMultiplier, float[,] heightMap) {
        float topLeftX = (size - 1) / -2f;
        float topLeftZ = (size - 1) / 2f;
        MeshData meshData = new MeshData (flatShading, size);

        int vertexIndex = 0;

        for(int y = 0; y < size; y++) {
            for(int x = 0; x < size; x++) {
                // size at current position
                float currentHeight = heightMap [x, y];

                //  Defining vertices and uvs
                meshData.vertices [vertexIndex] = new Vector3 (topLeftX + x, currentHeight *  currentHeight * heightMultiplier, topLeftZ - y);
                meshData.uvs [vertexIndex] = new Vector2 (x / (float)size, y / (float)size);

                // Defining triangles
                if(x < size - 1 && y < size - 1) {   // guard to ignore right bottom edge and far right edge
                    meshData.AddTriangle(vertexIndex, vertexIndex + size + 1, vertexIndex + size);
                    meshData.AddTriangle(vertexIndex + size + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        return meshData;
    }

    public static MeshData createMesh(bool flatShading, int size, float edgeLength) {
        float topLeftX = (size - 1) / -2f;
        float topLeftZ = (size - 1) / 2f;
        MeshData meshData = new MeshData (flatShading, size);

        int vertexIndex = 0;

        for(int y = 0; y < size; y++) {
            for(int x = 0; x < size; x++) {
                //  Defining vertices and uvs
                meshData.vertices [vertexIndex] = new Vector3 (topLeftX + x, topLeftZ - y);
                meshData.uvs [vertexIndex] = new Vector2 (x / (float)size, y / (float)size);

                // Defining triangles
                if(x < size - 1 && y < size - 1) {   // guard to ignore right bottom edge and far right edge
                    meshData.AddTriangle(vertexIndex, vertexIndex + size + 1, vertexIndex + size);
                    meshData.AddTriangle(vertexIndex + size + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        return meshData;
    }



    public class MeshData {
        public Vector3[] vertices;
        public int[] triangles;
        public Vector2[] uvs;
        public bool flatShading;

        int triangleIndex;

        public MeshData(bool flatShading, int size) {
            this.flatShading = flatShading;
            int nrOfVertices = size * size;
            int nrOfTriangles = (size - 1) * (size - 1) * 6;

            vertices = new Vector3[nrOfVertices];
            uvs = new Vector2[size * size];
            triangles = new int[nrOfTriangles];
        }

        public void AddTriangle(int a, int b, int c) {
            triangles [triangleIndex] = a;
            triangles [triangleIndex + 1] = b;
            triangles [triangleIndex + 2] = c;

            triangleIndex += 3;
        }

        public Mesh createMesh() {
            Mesh mesh = new Mesh ();
            if (flatShading)
                FlatShading();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.RecalculateNormals ();
            return mesh;
        }

        void FlatShading() {
            Vector3[] flatShadedVertices = new Vector3[triangles.Length];
            Vector2[] flatShadedUVs = new Vector2[triangles.Length];

            for(int i = 0; i < triangles.Length; i++) {
                flatShadedVertices[i] = vertices[triangles[i]];
                flatShadedUVs[i] = uvs[triangles[i]];
                triangles[i] = i;
            }

            vertices = flatShadedVertices;
            uvs = flatShadedUVs;
        }

    }

}