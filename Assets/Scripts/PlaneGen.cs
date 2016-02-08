using UnityEngine;
using System.Collections;

public class PlaneGen : MonoBehaviour
{
	public float height = 5f;
	public float depth = 0.5f;

    public Material ceilingMaterial;
	public Material floorMaterial;
	public Material wallsMaterial;

    void Start()
    {				
        //Create a 2D plane procedurally on our new Object
        GeneratePlane();
    }


    void GeneratePlane()
    {
		GameObject ceiling = new GameObject("Ceiling");
		GameObject floor = new GameObject("Floor");
		GameObject walls = new GameObject("Walls");

		MeshFilter filter = ceiling.AddComponent<MeshFilter> ();
		Mesh mesh = filter.mesh;
		mesh.Clear ();

		float length = 3f;
		float width = 3f;
		int resX = 8;
		int resY = 8;
		int random;

		int verticesLength = 0;

		#region Vertices      
		Vector3[] vertices = new Vector3[resX * resY * 2 + resX * 4 + resY * 4];

		//top face
		for (int y = 0; y < resY; y++) {
			random = Random.Range (7, 10);
			// [ -length / 2, length / 2 ]
			float yPos = ((float)y / (resY - 1) - .5f) * length * random;
			for (int x = 0; x < resX; x++) {
				random = Random.Range (7, 10);
				// [ -width / 2, width / 2 ]
				float xPos = ((float)x / (resX - 1) - .5f) * width * random;
				vertices [x + y * resX] = new Vector3 (xPos, yPos, 0.0f);
			}
		}

		//bottom face
		for (int y = resY; y < resY*2; y++) {
			for (int x = 0; x < resX; x++) {
				vertices [x + y * resX] = new Vector3 (vertices [(x + y * resX) - (resX * resY)].x, vertices [(x + y * resX) - (resX * resY)].y, depth);
				verticesLength = x + y * resX;
			}
		}

		int count = verticesLength + 1;
		//front face
		for (int i = 0; i < resX; i++) {
			vertices [count] = vertices [i];
			count++;
		}
		for (int i = 0; i < resX; i++) {
			vertices [count] = vertices [i + resX * resY];
			count++;
		}

		//back face
		for (int i = 0; i < resX; i++) {
			vertices [count] = vertices [i + (resY - 1) * resX];
			count++;
		}
		for (int i = 0; i < resX; i++) {
			vertices [count] = vertices [i + resX * resY + (resY - 1) * resX];
			count++;
		}

		//left face
		for (int i = 0; i < resY*2; i++) {
			vertices [count] = vertices [i * resX];
			count++;
		}

		//right face
		for (int i = 0; i < resY*2; i++) {
			vertices [count] = vertices [i * resX + resX - 1];
			count++;
		}
		

		#endregion

		#region Normales
		Vector3[] normales = new Vector3[vertices.Length];
		for (int n = 0; n < normales.Length; n++) {
			if (n < resX * resY)
				normales [n] = -Vector3.forward;
			else if (n < resX * resY * 2)
				normales [n] = Vector3.forward;
			else if (n < resX * resY * 2 + resX * 2)
				normales [n] = Vector3.down;
			else if (n < resX * resY * 2 + resX * 4)
				normales [n] = Vector3.up;
			else if (n < resX * resY * 2 + resX * 4 + resY * 2)
				normales [n] = Vector3.left;
			else if (n < resX * resY * 2 + resX * 4 + resY * 4)
				normales [n] = Vector3.right;
		}
		#endregion

		#region UVs      
		int uv_counter = 0;
		Vector2[] uvs = new Vector2[vertices.Length];
		for (int v = 0; v < resY*2; v++) {
			for (int u = 0; u < resX; u++) {
				uvs [uv_counter++] = new Vector2 ((float)u / (resX - 1), (float)v / (resY - 1));
				//uv_counter++;
			}
		}

		for (int v = 0; v < 4; v++) {
			for (int u = 0; u < resX; u++) {
				uvs [uv_counter++] = new Vector2 ((float)u / (resX - 1), (float)v / (resY - 1));
			}
		}


		for (int u = 0; u < 2; u++) {
			for (int v = 0; v < resY; v++) {
				uvs [uv_counter++] = new Vector2 ((float)u, (float)v / (resY - 1));
			}
		}

		for (int u = 0; u < 2; u++) {
			for (int v = 0; v < resY; v++) {
				uvs [uv_counter++] = new Vector2 ((float)u, (float)v / (resY - 1));
			}
		}

		#endregion

		#region Triangles
		int nbFaces = (resX - 1) * (resY - 1) * 2 + (resX - 1) * 2 + (resY - 1) * 2;
		int[] triangles = new int[nbFaces * 6];
		int t = 0;
		int frontCounter = 0;
		int backCounter = 0;
		int topCounter = 0;
		int bottomCounter = 0;
		int leftCounter = 0;
		int rightCounter = 0;
		for (int face = 0; face < nbFaces; face++) {
			int i = face % (resX - 1) + (face / (resY - 1) * resX);

			if (face < (resX - 1) * (resY - 1)) {
				if (i % resX == 0)
					frontCounter++;
				triangles [t++] = i % resX + resX * frontCounter;
				triangles [t++] = i % resX + 1 + resX * (frontCounter - 1);
				triangles [t++] = i % resX + resX * (frontCounter - 1);

				triangles [t++] = i % resX + resX * frontCounter;
				triangles [t++] = i % resX + resX + 1 + resX * (frontCounter - 1);
				triangles [t++] = i % resX + 1 + resX * (frontCounter - 1);
			} else if (face < (resX - 1) * (resY - 1) * 2) {
				if (i % resX == 0)
					backCounter++;
				triangles [t++] = i % resX + resX * resY + resX * backCounter;
				triangles [t++] = i % resX + resX * resY + resX * (backCounter - 1);
				triangles [t++] = i % resX + resX * resY + resX * (backCounter - 1) + 1;
				
				triangles [t++] = i % resX + resX * resY + resX * backCounter;
				triangles [t++] = i % resX + resX * resY + resX * (backCounter - 1) + 1;
				triangles [t++] = i % resX + resX * resY + resX * backCounter + 1;
			} else if (face < (resX - 1) * (resY - 1) * 2 + resX - 1) {
				topCounter++;
				i = resX * resY * 2;
				if (topCounter < 2) {
					for (int j = 0; j < resX-1; j++) {
						triangles [t++] = i + j;
						triangles [t++] = i + j + resX + 1;
						triangles [t++] = i + j + resX;
				
						triangles [t++] = i + j;
						triangles [t++] = i + j + 1;
						triangles [t++] = i + j + resX + 1;
					}
				}
			} else if (face < (resX - 1) * (resY - 1) * 2 + (resX - 1) * 2) {
				bottomCounter++;
				i = resX * resY * 2 + resX * 2;
				if (bottomCounter < 2) {
					for (int j = 0; j < resX-1; j++) {
						triangles [t++] = i + j;
						triangles [t++] = i + j + resX;
						triangles [t++] = i + j + resX + 1;
						
						triangles [t++] = i + j;
						triangles [t++] = i + j + resX + 1;
						triangles [t++] = i + j + 1;
					}
				}
			} else if (face < (resX - 1) * (resY - 1) * 2 + (resX - 1) * 2 + (resY - 1)) {
				leftCounter++;
				i = resX * resY * 2 + resX * 4;
				if (leftCounter < 2) {
					for (int j = 0; j < resY-1; j++) {
						triangles [t++] = i + j;
						triangles [t++] = i + j + resY;
						triangles [t++] = i + j + resY + 1;
						
						triangles [t++] = i + j;
						triangles [t++] = i + j + resY + 1;
						triangles [t++] = i + j + 1;
					}
				}

			} else if (face < (resX - 1) * (resY - 1) * 2 + (resX - 1) * 2 + (resY - 1) * 2) {
				rightCounter++;
				i = resX * resY * 2 + resX * 4 + resY * 2;
				if (rightCounter < 2) {
					for (int j = 0; j < resY-1; j++) {
						triangles [t++] = i + j;
						triangles [t++] = i + j + resY + 1;
						triangles [t++] = i + j + resY;
						
						triangles [t++] = i + j;
						triangles [t++] = i + j + 1;
						triangles [t++] = i + j + resY + 1;
					}
				}
				
			}
		}
		#endregion

		mesh.vertices = vertices;
		mesh.normals = normales;
		mesh.uv = uvs;
		mesh.triangles = triangles;

		mesh.RecalculateBounds ();
		mesh.Optimize ();


		//Attach material
		MeshRenderer rend = ceiling.AddComponent<MeshRenderer> ();

		if (ceilingMaterial) {
			rend.material = ceilingMaterial;
			rend.material.SetTextureScale("_MainTex", new Vector2(resX*length/2, resY*width/2));
		}


		// floor
		for (int i = 0; i < vertices.Length; i++) 
		{
			vertices[i].z = vertices[i].z + height;
		}
		
		filter = floor.AddComponent<MeshFilter>();
		mesh = filter.mesh;
		mesh.Clear();
		mesh.vertices = vertices;
		mesh.normals = normales;
		mesh.uv = uvs;
		mesh.triangles = triangles;
		
		mesh.RecalculateBounds();
		mesh.Optimize();

		rend = floor.AddComponent<MeshRenderer>();
		
		if (floorMaterial)
		{
			rend.material = floorMaterial;
			rend.material.SetTextureScale("_MainTex", new Vector2(resX*length/2, resY*width/2));
		}

		//walls
		filter = walls.AddComponent<MeshFilter>();
		mesh = filter.mesh;
		mesh.Clear();

		#region Vertices
		Vector3[] wall_vertices = new Vector3[(resX + resY) * 2 * 4];
		int counter = 0;

		for (int i = 0; i < resX; i++)
		{
			wall_vertices[i] = vertices[i+resX*resY];
			counter++;
		}

		for (int i = 0; i < resY; i++)
		{
			wall_vertices[i+counter] = vertices[resX*resY + i*resX + resX-1];
		}
		counter = counter + resY;

		for (int i = 0; i < resX; i++)
		{
			wall_vertices[i+counter] = vertices[resX*resY*2 - 1 - i];
		}
		counter = counter + resX;
		
		for (int i = 0; i < resY; i++)
		{
			wall_vertices[i+counter] = vertices[(resY-1-i)*resX + resX*resY];
		}
		counter = counter + resY;

		for (int i = 0; i < (resX + resY) * 2; i++)
		{
			wall_vertices[i+counter] = wall_vertices[i];
		/*	if (i<resX)
				wall_vertices[i+counter].y = wall_vertices[i+counter].y + 0.1f; 
			else if (i<resX+resY)
				wall_vertices[i+counter].x = wall_vertices[i+counter].x - 0.1f; 
			else if (i<resX*2+resY)
				wall_vertices[i+counter].y = wall_vertices[i+counter].y - 0.1f; 
			else if (i<resX*2+resY*2)
				wall_vertices[i+counter].x = wall_vertices[i+counter].x + 0.1f; */
		}	
		counter = counter + (resX+resY) * 2;

		for (int i = 0; i < (resX + resY) * 4; i++)
		{
			wall_vertices[i+counter] = wall_vertices[i];
			wall_vertices[i+counter].z = wall_vertices[i+counter].z + height;
		}




		#endregion

		#region Normales
		Vector3[] wall_normales = new Vector3[wall_vertices.Length];
		int shift = resX*2 + resY*2;
		for (int n = 0; n < wall_normales.Length; n++) {
			if (n < resX)
				wall_normales [n] = Vector3.down;
			else if (n < resX + resY)
				wall_normales [n] = Vector3.right;
			else if (n < resX*2 + resY)
				wall_normales [n] = Vector3.up;
			else if (n < resX*2 + resY*2)
				wall_normales [n] = Vector3.left;

			else if (n < shift + resX)
				wall_normales [n] = Vector3.up;
			else if (n < shift + resX + resY)
				wall_normales [n] = Vector3.left;
			else if (n < shift + resX*2 + resY)
				wall_normales [n] = Vector3.down;
			else if (n < shift + resX*2 + resY*2)
				wall_normales [n] = Vector3.right;

			else if (n < shift*2 + resX)
				wall_normales [n] = Vector3.down;
			else if (n < shift*2 + resX + resY)
				wall_normales [n] = Vector3.right;
			else if (n < shift*2 + resX*2 + resY)
				wall_normales [n] = Vector3.up;
			else if (n < shift*2 + resX*2 + resY*2)
				wall_normales [n] = Vector3.left;

			else if (n < shift*3 + resX)
				wall_normales [n] = Vector3.up;
			else if (n < shift*3 + resX + resY)
				wall_normales [n] = Vector3.left;
			else if (n < shift*3 + resX*2 + resY)
				wall_normales [n] = Vector3.down;
			else if (n < shift*3 + resX*2 + resY*2)
				wall_normales [n] = Vector3.right;


		}
		#endregion

		#region UVs      
		Vector2[] wall_uvs = new Vector2[wall_vertices.Length];

		//outside
		for (int i = 0; i < resX; i++)
		{ 
			wall_uvs[i] = new Vector2 ((float)i / (resX - 1), 0);
		}
		for (int i = 0; i < resY; i++)
		{ 
			wall_uvs[i+resX] = new Vector2 (1, 0);
		}
		for (int i = 0; i < resX; i++)
		{ 
			wall_uvs[i+resX+resY] = new Vector2 (1 - (float)i / (resX - 1), 0);
		}
		for (int i = 0; i < resY; i++)
		{ 
			wall_uvs[i+resX+resY+resX] = new Vector2 (0, 0);
		}

		//inside
		for (int i = 0; i < resX; i++)
		{ 
			wall_uvs[i+(resX+resY)*2] = new Vector2 ((float)i / (resX - 1), 1);
		}	
		for (int i = 0; i < resY; i++)
		{ 
			wall_uvs[i+resX+(resX+resY)*2] = new Vector2 (1, 1);
		}	
		for (int i = 0; i < resX; i++)
		{ 
			wall_uvs[i+resX+resY+(resX+resY)*2] = new Vector2 (1 - (float)i / (resX - 1), 1);
		}	
		for (int i = 0; i < resY; i++)
		{ 
			wall_uvs[i+resX+resY+resX+(resX+resY)*2] = new Vector2 (0, 1);
		}

		//top outside
		for (int i = 0; i < resX; i++)
		{ 
			wall_uvs[i+(resX+resY)*2*2] = new Vector2 ((float)i / (resX - 1), 1);
		}
		
		for (int i = 0; i < resY; i++)
		{ 
			wall_uvs[i+resX+(resX+resY)*2*2] = new Vector2 (1, 1);
		}	
		for (int i = 0; i < resX; i++)
		{ 
			wall_uvs[i+resX+resY+(resX+resY)*2*2] = new Vector2 (1 - (float)i / (resX - 1), 1);
		}	
		for (int i = 0; i < resY; i++)
		{ 
			wall_uvs[i+resX+resY+resX+(resX+resY)*2*2] = new Vector2 (0, 1);
		}

		//top inside
		for (int i = 0; i < resX; i++)
		{ 
			wall_uvs[i+(resX+resY)*6] = new Vector2 ((float)i / (resX - 1), 1);
		}	
		for (int i = 0; i < resY; i++)
		{ 
			wall_uvs[i+resX+(resX+resY)*6] = new Vector2 (1, 1);
		}	
		for (int i = 0; i < resX; i++)
		{ 
			wall_uvs[i+resX+resY+(resX+resY)*6] = new Vector2 (1 - (float)i / (resX - 1), 1);
		}	
		for (int i = 0; i < resY; i++)
		{ 
			wall_uvs[i+resX+resY+resX+(resX+resY)*6] = new Vector2 (0, 1);
		}
		#endregion
		
		#region Triangles
		int wall_nbFaces = (resX - 1) * 4 + (resY - 1) * 4;
		int[] wall_triangles = new int[wall_nbFaces * 6];
		int t1 = 0;
		int add = 0;

		for (int i = 0; i < (resX - 1) * 2 + (resY - 1) * 2; i++)
		{
			if (i == resX-1 || i == resX-1 + resY-1 || i == (resX-1)*2 + resY-1 || i == (resX-1)*2 + (resY-1)*2)
				add++;

			wall_triangles[t1++] = i + add;
			wall_triangles[t1++] = (resX+resY)*4 + i + 1 + add;
			wall_triangles[t1++] = (resX+resY)*4 + i + add;

			wall_triangles[t1++] = i + add;
			wall_triangles[t1++] = i + 1 + add;
			wall_triangles[t1++] = (resX+resY)*4 + i + 1 + add;
		}

		add = (resX+resY)*2;		
		for (int i = 0; i < (resX - 1) * 2 + (resY - 1) * 2; i++)
		{
			if (i == resX-1 || i == resX-1 + resY-1 || i == (resX-1)*2 + resY-1 || i == (resX-1)*2 + (resY-1)*2)
				add++;
			
			wall_triangles[t1++] = i + add;
			wall_triangles[t1++] = (resX+resY)*4 + i + add;
			wall_triangles[t1++] = (resX+resY)*4 + i + 1 + add;
			
			wall_triangles[t1++] = i + add;
			wall_triangles[t1++] = (resX+resY)*4 + i + 1 + add;
			wall_triangles[t1++] = i + 1 + add;
		}

		/*add = 0;
		int shift = resX* 2 + resY * 2;
		for (int i = shift; i < (resX - 1) * 4 + (resY - 1) * 4; i++)
		{
			if (i == resX-1+shift || i == resX-1 + resY-1+shift || i == (resX-1)*2 + resY-1+shift || i == (resX-1)*2 + (resY-1)*2+shift)
				add++;
			
			wall_triangles[t1++] = i + add;
			wall_triangles[t1++] = (resX+resY)*4 + i + add;
			wall_triangles[t1++] = (resX+resY)*4 + i + 1 + add;
			
			wall_triangles[t1++] = i + add;
			wall_triangles[t1++] = (resX+resY)*4 + i + 1 + add;
			wall_triangles[t1++] = i + 1 + add;
		}*/



		#endregion

		mesh.vertices = wall_vertices;
		mesh.normals = wall_normales;
		mesh.uv = wall_uvs;
		mesh.triangles = wall_triangles;
		
		mesh.RecalculateBounds();
		mesh.Optimize();
		
		rend = walls.AddComponent<MeshRenderer>();
		
		if (wallsMaterial)
		{
			rend.material = wallsMaterial;
			rend.material.SetTextureScale("_MainTex", new Vector2(resX*length/2, resY*width/2));
		}

		ceiling.AddComponent<Ceiling>();

    }
}