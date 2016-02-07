using UnityEngine;
using System.Collections;

public class PlaneGen : MonoBehaviour
{

    public Material spriteMaterial;

    void Start()
    {

        //Create a new GameObject named "Spawned GameObject (Plane)"
        GameObject spawnedGameObject = new GameObject("Spawned GameObject (Plane)");

        //Create a 2D plane procedurally on our new Object
        GeneratePlane(spawnedGameObject);

    }


    void GeneratePlane(GameObject spawnedGameObject)
    {
        // You can change that line to provide another MeshFilter
        MeshFilter filter = spawnedGameObject.AddComponent<MeshFilter>();
        Mesh mesh = filter.mesh;
        mesh.Clear();

        float length = 1f;
        float width = 1f;
		float depth = 0.5f;
        int resX = 8; // 2 minimum
		int resY = 8;
		int random;

		int verticesLength = 0;

        #region Vertices      
        Vector3[] vertices = new Vector3[resX * resY * 2 + resX*4 + resY*4];

		//top face
        for (int y = 0; y < resY; y++)
        {
			random = Random.Range(5,10);
            // [ -length / 2, length / 2 ]
            float yPos = ((float)y / (resY - 1) - .5f) * length * random;
            for (int x = 0; x < resX; x++)
            {
				random = Random.Range(5,10);
                // [ -width / 2, width / 2 ]
                float xPos = ((float)x / (resX - 1) - .5f) * width * random;
                vertices[x + y * resX] = new Vector3(xPos, yPos, 0.0f);
            }
        }

		//bottom face
		for (int y = resY; y < resY*2; y++)
		{
			for (int x = 0; x < resX; x++)
			{
				vertices[x + y * resX] = new Vector3(vertices[(x + y * resX) - (resX * resY)].x, vertices[(x + y * resX) - (resX * resY)].y, depth);
				verticesLength = x + y * resX;
			}
		}

		int count = verticesLength + 1;
		//front face
		for(int i = 0; i < resX; i++)
		{
			vertices[count] = vertices[i];
			count++;
		}
		for(int i = 0; i < resX; i++)
		{
			vertices[count] = vertices[i + resX*resY];
			count++;
		}

		//back face
		for(int i = 0; i < resX; i++)
		{
			vertices[count] = vertices[i + (resY-1)*resX];
			count++;
		}
		for(int i = 0; i < resX; i++)
		{
			vertices[count] = vertices[i + resX*resY + (resY-1)*resX];
			count++;
		}

		//left face
		for(int i = 0; i < resY*2; i++)
		{
			vertices[count] = vertices[i*resX];
			count++;
		}

		//right face
		for(int i = 0; i < resY*2; i++)
		{
			vertices[count] = vertices[i*resX + resX - 1];
			count++;
		}
		

        #endregion

        #region Normales
        Vector3[] normales = new Vector3[vertices.Length];
        for (int n = 0; n < normales.Length; n++)
		{
			if (n<resX*resY)
            	normales[n] = -Vector3.forward;
			else if (n < resX * resY * 2)
				normales[n] = Vector3.forward;
			else if (n < resX * resY * 2 + resX*2)
				normales[n] = Vector3.down;
			else if (n < resX * resY * 2 + resX*4)
				normales[n] = Vector3.up;
			else if (n < resX * resY * 2 + resX*4 + resY*2)
				normales[n] = Vector3.left;
			else if (n < resX * resY * 2 + resX*4 + resY*4)
				normales[n] = Vector3.right;
		}
        #endregion

        #region UVs      
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int v = 0; v < resY*2; v++)
        {
            for (int u = 0; u < resX; u++)
            {
                uvs[u + v * resX] = new Vector2((float)u / (resX - 1), (float)v / (resY - 1));
            }
        }
        #endregion

        #region Triangles
		int nbFaces = (resX - 1) * (resY - 1) * 2 + (resX-1)*2 + (resY-1)*2;
        int[] triangles = new int[nbFaces * 6];
        int t = 0;
		int frontCounter = 0;
		int backCounter = 0;
		int topCounter = 0;
		int bottomCounter = 0;
		int leftCounter = 0;
		int rightCounter = 0;
        for (int face = 0; face < nbFaces; face++)
        {
			int i = face % (resX - 1) + (face / (resY - 1) * resX);

			if (face<(resX - 1) * (resY - 1))
			{
	            if (i%resX==0)
					frontCounter++;
				triangles[t++] = i%resX + resX*frontCounter;
				triangles[t++] = i%resX + 1 + resX*(frontCounter-1);
				triangles[t++] = i%resX + resX*(frontCounter-1);

				triangles[t++] = i%resX + resX*frontCounter;
				triangles[t++] = i%resX + resX + 1 + resX*(frontCounter-1);
				triangles[t++] = i%resX + 1 + resX*(frontCounter-1);
			}
			else if(face<(resX - 1) * (resY - 1) * 2)
			{
				if (i%resX==0)
					backCounter++;
				triangles[t++] = i%resX + resX*resY + resX*backCounter;
				triangles[t++] = i%resX + resX*resY + resX*(backCounter-1);
				triangles[t++] = i%resX + resX*resY + resX*(backCounter-1) + 1;
				
				triangles[t++] = i%resX + resX*resY + resX*backCounter;
				triangles[t++] = i%resX + resX*resY + resX*(backCounter-1) + 1;
				triangles[t++] = i%resX + resX*resY + resX*backCounter + 1;
			}
			else if (face<(resX - 1) * (resY - 1) * 2 + resX-1)
			{
				topCounter++;
				i = resX*resY*2;
 				if (topCounter<2)
				{
					for(int j = 0; j < resX-1; j++)
					{
						triangles[t++] = i+j;
						triangles[t++] = i+j + resX + 1;
						triangles[t++] = i+j + resX;
				
						triangles[t++] = i+j;
						triangles[t++] = i+j + 1;
						triangles[t++] = i+j + resX + 1;
					}
				}
			}
			else if (face<(resX - 1) * (resY - 1) * 2 + (resX-1)*2)
			{
				bottomCounter++;
				i = resX*resY*2 + resX*2;
				if (bottomCounter<2)
				{
					for(int j = 0; j < resX-1; j++)
					{
						triangles[t++] = i+j;
						triangles[t++] = i+j + resX;
						triangles[t++] = i+j + resX + 1;
						
						triangles[t++] = i+j;
						triangles[t++] = i+j + resX + 1;
						triangles[t++] = i+j + 1;
					}
				}
			}
			else if (face<(resX - 1) * (resY - 1) * 2 + (resX-1)*2 + (resY-1))
			{
				leftCounter++;
				i = resX*resY*2 + resX*4;
				if (leftCounter<2)
				{
					for(int j = 0; j < resY-1; j++)
					{
						triangles[t++] = i+j;
						triangles[t++] = i+j + resY;
						triangles[t++] = i+j + resY + 1;
						
						triangles[t++] = i+j;
						triangles[t++] = i+j + resY + 1;
						triangles[t++] = i+j + 1;
					}
				}

			}
			else if (face<(resX - 1) * (resY - 1) * 2 + (resX-1)*2 + (resY-1)*2)
			{
				rightCounter++;
				i = resX*resY*2 + resX*4 + resY*2;
				if (rightCounter<2)
				{
					for(int j = 0; j < resY-1; j++)
					{
						triangles[t++] = i+j;
						triangles[t++] = i+j + resY + 1;
						triangles[t++] = i+j + resY;
						
						triangles[t++] = i+j;
						triangles[t++] = i+j + 1;
						triangles[t++] = i+j + resY + 1;
					}
				}
				
			}
        }
        #endregion

        mesh.vertices = vertices;
        mesh.normals = normales;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
        mesh.Optimize();


        //Attach material
        MeshRenderer rend = spawnedGameObject.AddComponent<MeshRenderer>();

        if (spriteMaterial)
        {
            rend.material = spriteMaterial;
        }
    }
}