using UnityEngine;
using System.Collections;
using System;

public class PlaneGen : MonoBehaviour
{
	public float depth = 0.5f;
	public float l = 1f;
	public float w = 1f;
	public int resolutionX;
	public int resolutionY;
	private bool door = false;
	public float branchFactor = 100;
	private int counter = 0;
	private int tryCounter = 0;

    public Material ceilingMaterial;
	public Material floorMaterial;
	public Material wallsMaterial;

    private Camera camera;

    void Start()
    {
        Physics.gravity = new Vector3(0f, 0f, -1f);
        camera = Camera.main;
        GeneratePath ();
}

	void Update()
	{
        //	GetCurrentRoom ();

    }

	void GeneratePath()
	{
		while (GenerateRoom(null, branchFactor, 0, true)) 
		{
			GameObject[] gameObjects = FindObjectsOfType(typeof(GameObject)) as GameObject[];
			
			for (int j=0; j < gameObjects.Length; j++)
			{
				if(gameObjects[j].name.Contains("Walls") || gameObjects[j].name.Contains("Floor") || gameObjects[j].name.Contains("Ceiling"))
					DestroyImmediate(gameObjects[j]);			
			}
			counter = 0;
		}
	}

    bool GenerateRoom(Vector3[] doors, float bf , int tryCounter, bool main)
    {
		Vector3[] door1_vertex = doors;
		Vector3[] door2_vertex = null;        
		bool overlap = false;
		int doorNumber = 0;
		int resX = UnityEngine.Random.Range (10, 10);
		int resY = UnityEngine.Random.Range (10, 10);
		float length = UnityEngine.Random.Range(5, 5);
		float width = UnityEngine.Random.Range(5, 5);
		float height = UnityEngine.Random.Range (5, 5);
		Vector3[] vertices = GenerateFloorCeiling (counter, counter * 50, resX, resY, length, width, false, null, null, height);

		float factor = bf;
		float check = UnityEngine.Random.Range (1, 100);
		while (check<factor) {
			doorNumber++;
			check = UnityEngine.Random.Range (1, 100);
			factor = factor / 1.5f;
		}

		if (doorNumber < 1)
			doorNumber++;

		door2_vertex = GenerateWalls (counter, vertices, doorNumber, resX, resY, length, width, false, height);	

		if (counter > 0) {
			overlap = AttachRoom (door1_vertex, door2_vertex, counter);
			vertices = GenerateFloorCeiling (counter, counter * 50, 2, 2, length, width, true, door1_vertex, door2_vertex, height);
			GenerateWalls (counter, vertices, 2, 2, 2, length, width, true, height);

		    GameObject all_walls;
			Bounds[] all_bounds = new Bounds[counter*2-1];
            int count = 0;
			for (int j = 0; j < counter; j++) {
				all_walls = GameObject.Find ("Walls" + (j));
				all_bounds [count] = all_walls.GetComponent<Renderer> ().bounds;
                count++;
				if (j>0)
				{
					all_walls = GameObject.Find ("CorridorWalls" + (j));
					all_bounds [count] = all_walls.GetComponent<Renderer> ().bounds;
                    count++;
				}
			}
			GameObject corridor_walls = GameObject.Find ("CorridorWalls" + counter);
			Bounds new_bounds = corridor_walls.GetComponent<Renderer> ().bounds;
			for (int j = 0; j < counter*2-1; j++) {
				if (new_bounds.Intersects (all_bounds [j])) {
					overlap = true;
					break;
				}
			}
		}

		int mainDoor = 0;
		if (main == true) 
		{
			mainDoor = UnityEngine.Random.Range(0, doorNumber);
		}

		if (!overlap) 
		{
			if (counter == 0) 
				for (int j = 0; j < doorNumber; j++) 
				{ 
					Vector3[] temp = new Vector3[5];
					Array.Copy (door2_vertex, j * 5, temp, 0, 5);
					if(!overlap)
						counter++;
					if (j==mainDoor)
						overlap = GenerateRoom (temp, bf - bf/10, tryCounter, true);
					else
					    overlap = GenerateRoom (temp, bf/2, tryCounter, false);
					if (overlap) {
						j--;
						tryCounter++;
					}
					if (tryCounter > 100)
						break;
				}
			else
				for (int j = 1; j < doorNumber; j++) {
					Vector3[] temp = new Vector3[5];
					Array.Copy (door2_vertex, j * 5, temp, 0, 5);
					if(!overlap)
						counter++;
					if (j==mainDoor)
					    overlap = GenerateRoom (temp, bf - bf/10, tryCounter, true);
					else
					    overlap = GenerateRoom (temp, bf/3, tryCounter, false);
					if (overlap) {
						j--;
						tryCounter++;
					}
					if (tryCounter > 100)
						break;
				}
			tryCounter = 0;
		}
		return overlap;
    }

	Vector3 RotatePoint(Vector3 P, Vector3 center, double angle)
	{
		double newX = (P.x - center.x) * Math.Cos(angle) - (P.y - center.y) * Math.Sin (angle) + center.x;
		double newY = (P.x - center.x) * Math.Sin(angle) + (P.y - center.y) * Math.Cos (angle) + center.y;

		return new Vector3 (Convert.ToSingle(newX), Convert.ToSingle(newY), 0);
	}

	bool AttachRoom(Vector3[] door1, Vector3[] door2, int index)
	{
		bool overlap = false;

		float product = (door1 [1].y - door1 [0].y) * (door2 [0].y - door2 [1].y) + (door1 [1].x - door1 [0].x) * (door2 [0].x - door2 [1].x) + (door1 [1].z - door1 [0].z) * (door2 [0].z - door2 [1].z);
		float size1 = Convert.ToSingle (Math.Sqrt ((door1 [1].y - door1 [0].y) * (door1 [1].y - door1 [0].y) + (door1 [1].x - door1 [0].x) * (door1 [1].x - door1 [0].x) + (door1 [1].z - door1 [0].z) * (door1 [1].z - door1 [0].z)));
		float size2 = Convert.ToSingle (Math.Sqrt ((door2 [1].y - door2 [0].y) * (door2 [1].y - door2 [0].y) + (door2 [1].x - door2 [0].x) * (door2 [1].x - door2 [0].x) + (door2 [1].z - door2 [0].z) * (door2 [1].z - door2 [0].z)));
		float angle = Convert.ToSingle (Math.Acos (Math.Round (product / (size1 * size2), 4)));

		GameObject floor = GameObject.Find ("Floor" + index);
		GameObject ceiling = GameObject.Find ("Ceiling" + index);
		GameObject walls = GameObject.Find ("Walls" + index);

		if (float.IsNaN (angle))
			angle = 0;

		float rotationAngle = angle * Mathf.Rad2Deg;

		if (Math.Round (product, 3) < 0 && rotationAngle <= 90)
			rotationAngle = 360 - rotationAngle;

		float det = (door1 [1].x - door1 [0].x) * (door2 [0].y - door2 [1].y) - (door1 [1].y - door1 [0].y) * (door2 [0].x - door2 [1].x);
		double a = - Math.Atan2 (det, product) * Mathf.Rad2Deg;

		floor.transform.Translate (-(door2 [1].x - door1 [0].x), -(door2 [1].y - door1 [0].y), -(door2 [1].z - door1 [0].z));
		ceiling.transform.Translate (-(door2 [1].x - door1 [0].x), -(door2 [1].y - door1 [0].y), -(door2 [1].z - door1 [0].z));
		walls.transform.Translate (-(door2 [1].x - door1 [0].x), -(door2 [1].y - door1 [0].y), -(door2 [1].z - door1 [0].z));

		Vector3 temp1 = door1 [0];
		Vector3 temp2 = door2 [1];

		for (int i = 0; i < door2.Length; i++) {
			if (i % 5 != 4) {
				door2 [i].x = door2 [i].x - (temp2.x - temp1.x);
				door2 [i].y = door2 [i].y - (temp2.y - temp1.y);
				door2 [i].z = door2 [i].z - (temp2.z - temp1.z);
			}
		}

		floor.transform.RotateAround (new Vector3 (door1 [0].x, door1 [0].y, door1 [0].z), Vector3.forward, Convert.ToSingle (a));
		ceiling.transform.RotateAround (new Vector3 (door1 [0].x, door1 [0].y, door1 [0].z), Vector3.forward, Convert.ToSingle (a));
		walls.transform.RotateAround (new Vector3 (door1 [0].x, door1 [0].y, door1 [0].z), Vector3.forward, Convert.ToSingle (a));

		for (int i = 0; i<door2.Length; i++) {
			if (i % 5 == 4) {
			} else {
				if (door2 [i].x != door1 [0].x || door2 [i].y != door1 [0].y)
					door2 [i] = new Vector3 (0, 0, door2 [i].z) + RotatePoint (door2 [i], door1 [0], a * Mathf.Deg2Rad);
			}
		}

		Vector3 door = new Vector3 (door1 [1].x - door1 [0].x, door1 [1].y - door1 [0].y, door1 [1].z - door1 [0].z).normalized;
		Vector3 perpendicular = Vector3.Cross (door, new Vector3 (0, 0, 1));

		floor.transform.Translate (perpendicular, Space.World);
		ceiling.transform.Translate (perpendicular, Space.World);
		walls.transform.Translate (perpendicular, Space.World);

		for (int i = 0; i < door2.Length; i++) {
			if (i % 5 != 4)
				door2 [i] = door2 [i] + perpendicular;
		}

		Bounds[] all_bounds = new Bounds[index];
		for (int i = 0; i < index; i++) {
			GameObject all_walls = GameObject.Find ("Walls" + (i));
			all_bounds [i] = all_walls.GetComponent<Renderer> ().bounds;
		}

		GameObject previous_walls = GameObject.Find ("Walls" + (index - 1));
		Bounds previous_bounds = previous_walls.GetComponent<Renderer> ().bounds;
		Bounds new_bounds = walls.GetComponent<Renderer> ().bounds;

		for (int i = 0; i < index; i++) {
			if (i != index - 1)
			{
				if (new_bounds.Intersects (all_bounds [i])) {
					overlap = true;
					DestroyImmediate (walls);
					DestroyImmediate (floor);
					DestroyImmediate (ceiling);
					break;
				}
			}
		}

		if (!overlap)
			while (new_bounds.Intersects(previous_bounds)) {
				walls.transform.Translate (perpendicular, Space.World);
				floor.transform.Translate (perpendicular, Space.World);
				ceiling.transform.Translate (perpendicular, Space.World);
				for (int i = 0; i < door2.Length; i++) {
					if (i % 5 != 4)
						door2 [i] = door2 [i] + perpendicular;
				}
				new_bounds = walls.GetComponent<Renderer> ().bounds;
				for (int i = 0; i < index; i++) {
					if (i != index - 1)
					if (new_bounds.Intersects (all_bounds [i])) {
						overlap = true;
						break;
					}
				}
				if (overlap) {
					DestroyImmediate (walls);
					DestroyImmediate (floor);
					DestroyImmediate (ceiling);
					break;
				}
			}
		return overlap;		
	}

	Vector3[] GenerateFloorCeiling(int index, int shift, int resX, int resY, float length, float width, bool corridor, Vector3[] door1, Vector3[] door2, float height)
	{
		GameObject ceiling;
		GameObject floor;

		if (!corridor) 
		{
			floor = new GameObject ("Floor" + index);
			ceiling = new GameObject ("Ceiling" + index);
		} 
		else 
		{
			floor = new GameObject ("CorridorFloor" + index);
			ceiling = new GameObject ("CorridorCeiling" + index);
		}

		MeshFilter filter = ceiling.AddComponent<MeshFilter> ();
		Mesh mesh = filter.mesh;
		mesh.Clear ();

		int random;
		#region Vertices  

		Vector3[] vertices;

		if (!corridor)
		{     
			vertices = new Vector3[resX * resY * 2];
			
			//top face
			for (int y = 0; y < resY; y++) {
				for (int x = 0; x < resX; x++) {
					random = UnityEngine.Random.Range (10, 10);
					float yPos = ((float)y / (resY - 1) - .5f) * length * random + shift;
					random = UnityEngine.Random.Range (10, 10);
					float xPos = ((float)x / (resX - 1) - .5f) * width * random + shift;
					vertices [x + y * resX] = new Vector3 (xPos, yPos, 0.0f);
				}
			}
		}
		else
		{
			vertices = new Vector3[8];

            Vector3 door = new Vector3(door1[1].x - door1[0].x, door1[1].y - door1[0].y, door1[1].z - door1[0].z).normalized;
            Vector3 perpendicular = Vector3.Cross(door, new Vector3(0, 0, 1)) / 4999;

            Vector3 vector = new Vector3(0,0,0);
			vertices [0] = door1 [3] + perpendicular;
			vertices [1] = door2 [2] - perpendicular;
			vertices [2] = door1 [2] + perpendicular;
			vertices [3] = door2 [3] - perpendicular;
		}
		
		//bottom face
		for (int y = resY; y < resY*2; y++) {
			for (int x = 0; x < resX; x++) {
				if (!corridor)
					vertices [x + y * resX] = new Vector3 (vertices [(x + y * resX) - (resX * resY)].x, vertices [(x + y * resX) - (resX * resY)].y, vertices [(x + y * resX) - (resX * resY)].z);
				else
					vertices [x + y * resX] = new Vector3 (vertices [(x + y * resX) - (resX * resY)].x, vertices [(x + y * resX) - (resX * resY)].y, vertices [(x + y * resX) - (resX * resY)].z);
			}
		}

		#endregion
		
		#region UVs

		float minX = float.MaxValue;
		float minY = float.MaxValue;
		float maxX = float.MinValue;
		float maxY = float.MinValue;

		for (int i = 0; i < vertices.Length; i++)
		{
			if (vertices[i].x < minX)
				minX = vertices[i].x;
			if (vertices[i].x > maxX)
				maxX = vertices[i].x;
			if (vertices[i].y < minY)
				minY = vertices[i].y;
			if (vertices[i].y > maxY)
				maxY = vertices[i].y;
		}

		int uv_counter = 0;

		float total_distanceX = maxX - minX;
		if (total_distanceX<0)
			total_distanceX = 0 - total_distanceX;

		float total_distanceY = maxY - minY;
		if (total_distanceY<0)
			total_distanceY = 0 - total_distanceY;

		float diff_distanceX = 0;
		float diff_distanceY = 0;

		int u1 = 0;
		Vector2[] uvs = new Vector2[vertices.Length];
		for (int v = 0; v < resY; v++) {
			for (u1 = 0; u1 < resX; u1++) {
				diff_distanceX = vertices[u1 + resX*v].x - minX;
				diff_distanceY = vertices[u1 + resX*v].y - minY;
				uvs [uv_counter++] = new Vector2 (diff_distanceX/total_distanceX, diff_distanceY/total_distanceY);
			}
		}

		for (int v = 0; v < resY; v++) {
			for (u1 = 0; u1 < resX; u1++) {
				diff_distanceX = vertices[u1 + resX*v].x - minX;
				diff_distanceY = vertices[u1 + resX*v].y - minY;
				uvs [uv_counter++] = new Vector2 (diff_distanceX/total_distanceX, diff_distanceY/total_distanceY);
			}
		}
		
		#endregion
		
		#region Triangles
		int nbFaces = (resX - 1) * (resY - 1) * 2 + (resX - 1) * 2 + (resY - 1) * 2;
		int[] triangles = new int[nbFaces * 6];
		int t = 0;
		int frontCounter = 0;
		int backCounter = 0;

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
			}
		}
		#endregion

		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
		mesh.uv = uvs;
	
		mesh.RecalculateBounds ();
		mesh.Optimize ();
		
		//Attach material
		MeshRenderer rend = ceiling.AddComponent<MeshRenderer> ();

		if (ceilingMaterial) {
			rend.material = ceilingMaterial;
			rend.material.SetTextureScale("_MainTex", new Vector2(total_distanceX/2, total_distanceY/2));
		}

		MeshCollider ceilingCollider = ceiling.AddComponent<MeshCollider> ();
		ceilingCollider.sharedMesh = mesh;
		
		// floor
		for (int i = 0; i < vertices.Length; i++) 
		{
			vertices[i].z = vertices[i].z - height;
		}
		
		filter = floor.AddComponent<MeshFilter>();
		mesh = filter.mesh;
		mesh.Clear();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();
		mesh.uv = uvs;
		
		mesh.RecalculateBounds();
		mesh.Optimize();
		
		rend = floor.AddComponent<MeshRenderer>();
		
		if (floorMaterial)
		{
			rend.material = floorMaterial;
			rend.material.SetTextureScale("_MainTex", new Vector2(total_distanceX/2, total_distanceY/2));
		}

		MeshCollider floorCollider = floor.AddComponent<MeshCollider> ();
		floorCollider.sharedMesh = mesh;

        floor.AddComponent<ObjectManipulate>();
        ceiling.AddComponent<ObjectManipulate>();

        return vertices;
	}

	Vector3[] GenerateWalls(int index, Vector3[] vertices, int doorNumber, int resX, int resY, float length, float width, bool corridor, float height)
	{
		//walls
		GameObject walls;
		if (!corridor)
			walls = new GameObject("Walls" + index);
		else
			walls = new GameObject("CorridorWalls" + index);
		MeshFilter filter = walls.AddComponent<MeshFilter>();
		Mesh mesh = filter.mesh;
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
		}	
		counter = counter + (resX+resY) * 2;
		
		for (int i = 0; i < (resX + resY) * 4; i++)
		{
			wall_vertices[i+counter] = wall_vertices[i];
			if (!corridor)
				wall_vertices[i+counter].z = wall_vertices[i+counter].z + height;
			else
				wall_vertices[i+counter].z = wall_vertices[i+counter].z + height;
		}
		
		//door
		
		int[] door_vertex = new int[5*doorNumber];
		int r;

		if(!corridor)
		{			
			for(int i = 0; i < doorNumber; i++)
			{
				r = UnityEngine.Random.Range(0, resX*2 + resY*2);

				if (r==resX-1 || r==resX+resY-1 || r==resX*2+resY-1)
					door_vertex[0+i*5] = r + 1;
				else if (r==resX*2+resY*2-1)
					door_vertex[0+i*5] = 0;
				else
					door_vertex[0+i*5] = r;
					
				door_vertex[1+i*5] = door_vertex[0+i*5] + 1;
				door_vertex[2+i*5] = door_vertex[0+i*5] + (resX*2 + resY*2)*2;
				door_vertex[3+i*5] = door_vertex[2+i*5] + 1;

				if (door_vertex[0+i*5]<resX)
					door_vertex[4+i*5] = 0;
				else if (door_vertex[0+i*5]<resX+resY)
					door_vertex[4+i*5] = 1;
				else if (door_vertex[0+i*5]<resX*2+resY)
					door_vertex[4+i*5] = 2;
				else if (door_vertex[0+i*5]<resX*2+resY*2)
					door_vertex[4+i*5] = 3;

				if (i>0)
					for (int j = 0; j < door_vertex.Length/5 - 1; j = j + 5)
						if (door_vertex[0+i*5] == door_vertex[j])
						{
							i--;
							break;
						}
			}
		}
		else
		{
			door_vertex[0] = 2;
			door_vertex[1] = 3;
			door_vertex[2] = 18;
			door_vertex[3] = 19;

			door_vertex[5] = 6;
			door_vertex[6] = 7;
			door_vertex[7] = 22;
			door_vertex[8] = 23;
		}
		
		#endregion
		
		#region UVs      
		Vector2[] wall_uvs = new Vector2[wall_vertices.Length];

		float total_distance = 0;
		float diff_distance = 0;
		int u1 = 0;
		
		total_distance = wall_vertices[resX - 1].x - wall_vertices[0].x;
		if (total_distance<0)
			total_distance = 0 - total_distance;
		for (u1 = 0; u1 < resX; u1++) {
			diff_distance = wall_vertices[u1].x - wall_vertices[0].x;
			if (diff_distance<0)
				diff_distance = 0 - diff_distance;
			wall_uvs [u1] = new Vector2 (diff_distance/total_distance, 0);
		}

		int move = resX;
		
		total_distance = wall_vertices[resY - 1 + move].y - wall_vertices[move].y;
		if (total_distance<0)
			total_distance = 0 - total_distance;
		for (u1 = 0; u1 < resY; u1++) {
			diff_distance = wall_vertices[u1 + move].y - wall_vertices[move].y;
			if (diff_distance<0)
				diff_distance = 0 - diff_distance;
			wall_uvs [u1 + move] = new Vector2 (diff_distance/total_distance, 0);
		}

		move = resX+resY;
		
		total_distance = wall_vertices[resX - 1 + move].x - wall_vertices[move].x;
		if (total_distance<0)
			total_distance = 0 - total_distance;
		for (u1 = 0; u1 < resX; u1++) {
			diff_distance = wall_vertices[u1 + move].x - wall_vertices[move].x;
			if (diff_distance<0)
				diff_distance = 0 - diff_distance;
			wall_uvs [u1 + move] = new Vector2 (diff_distance/total_distance, 0);
		}

		move = resX+resX+resY;
		
		total_distance = wall_vertices[resY - 1 + move].y - wall_vertices[move].y;
		if (total_distance<0)
			total_distance = 0 - total_distance;
		for (u1 = 0; u1 < resY; u1++) {
			diff_distance = wall_vertices[u1 + move].y - wall_vertices[move].y;
			if (diff_distance<0)
				diff_distance = 0 - diff_distance;
			wall_uvs [u1 + move] = new Vector2 (diff_distance/total_distance, 0);
		}

		for (int i = resX*2 + resY*2; i < (resX*2 + resY*2)*2; i++)
		{
			wall_uvs[i] = wall_uvs[i-(resX*2 + resY*2)];
		}
				
		move = (resX+resY)*4;
		for (u1 = 0; u1 < resX; u1++) {
			wall_uvs [u1 + move] = new Vector2 (wall_uvs[u1].x, 1);
		}

		move = (resX+resY)*4 + resX;
		for (u1 = 0; u1 < resX; u1++) {
			wall_uvs [u1 + move] = new Vector2 (wall_uvs[u1].x, 1);
		}

		move = (resX+resY)*4 + resX + resY;
		for (u1 = 0; u1 < resX; u1++) {
			wall_uvs [u1 + move] = new Vector2 (wall_uvs[u1].x, 1);
		}

		move = (resX+resY)*4 + (resX + resY)*2;
		for (u1 = 0; u1 < resX; u1++) {
			wall_uvs [u1 + move] = new Vector2 (wall_uvs[u1].x, 1);
		}

		for (int i = move; i < (resX*2 + resY*2)*4; i++)
		{
			wall_uvs[i] = wall_uvs[i-(resX*2 + resY*2)];
		}
		
		#endregion
		
		#region Triangles
		
		int wall_nbFaces;
		
		if (door)
			wall_nbFaces = (resX - 1) * 4 + (resY - 1) * 4 - 1;
		else
			wall_nbFaces = (resX - 1) * 4 + (resY - 1) * 4;
		int[] wall_triangles = new int[wall_nbFaces * 6];
		int t1 = 0;
		int add = 0;
		bool newDoor = false;
		
		for (int i = 0; i < (resX - 1) * 2 + (resY - 1) * 2; i++)
		{
			if (i == resX-1 || i == resX-1 + resY-1 || i == (resX-1)*2 + resY-1 || i == (resX-1)*2 + (resY-1)*2)
				add++;

			newDoor = false;

			for (int j = 0; j<doorNumber; j++)
			{
				if ((i+add)==door_vertex[5*j] && ((resX+resY)*4 + i + add)==door_vertex[5*j+2])
				{
					newDoor = true;
					break;
				}
			}

			if (!newDoor)
			{
				wall_triangles[t1++] = i + add;
				wall_triangles[t1++] = (resX+resY)*4 + i + 1 + add;
				wall_triangles[t1++] = (resX+resY)*4 + i + add;
				
				wall_triangles[t1++] = i + add;
				wall_triangles[t1++] = i + 1 + add;
				wall_triangles[t1++] = (resX+resY)*4 + i + 1 + add;
			}
		}
		
		add = (resX+resY)*2;		
		for (int i = 0; i < (resX - 1) * 2 + (resY - 1) * 2; i++)
		{
			if (i == resX-1 || i == resX-1 + resY-1 || i == (resX-1)*2 + resY-1 || i == (resX-1)*2 + (resY-1)*2)
				add++;

			newDoor = false;
			
			for (int j = 0; j<doorNumber; j++)
			{
				if ((i+add)==door_vertex[5*j]+(resX+resY)*2 && ((resX+resY)*4 + i + add)==door_vertex[5*j+2]+(resX+resY)*2)
				{
					newDoor = true;
					break;
				}
			}
			
			if (!newDoor)
			{
				wall_triangles[t1++] = i + add;
				wall_triangles[t1++] = (resX+resY)*4 + i + add;
				wall_triangles[t1++] = (resX+resY)*4 + i + 1 + add;
				
				wall_triangles[t1++] = i + add;
				wall_triangles[t1++] = (resX+resY)*4 + i + 1 + add;
				wall_triangles[t1++] = i + 1 + add;
			}
		}				

		#endregion
		
		mesh.vertices = wall_vertices;
		mesh.triangles = wall_triangles;
		mesh.RecalculateNormals ();
		mesh.uv = wall_uvs;
		
		mesh.RecalculateBounds();
		mesh.Optimize();
		
		MeshRenderer rend = walls.AddComponent<MeshRenderer>();
		
		if (wallsMaterial)
		{
			rend.material = wallsMaterial;
			rend.material.SetTextureScale("_MainTex", new Vector2(resX*length/2, resY*width/2));
		}

		MeshCollider wallsCollider = walls.AddComponent<MeshCollider> ();
		wallsCollider.sharedMesh = mesh;

        walls.AddComponent<ObjectManipulate>();

        Vector3[] d_vertex = new Vector3[door_vertex.Length+1];
		for (int i = 0; i < door_vertex.Length; i++) 
		{
			if (i%5==4)
				d_vertex[i] = new Vector3(door_vertex[i], 0, 0);
			else
				d_vertex[i] = wall_vertices[door_vertex[i]];
		}
		d_vertex[door_vertex.Length] = new Vector3(door_vertex[0], 0, 0);


		return d_vertex;
	}

	void GetCurrentRoom()
	{
		GameObject camera = GameObject.Find("Main Camera");
		GameObject[] gameObjects = FindObjectsOfType(typeof(GameObject)) as GameObject[];
		
		for (int j=0; j < gameObjects.Length; j++)
		{
			GameObject temp = GameObject.Find("Walls" + j);
			if (temp!=null)
				if (temp.GetComponent<Renderer> ().bounds.Contains(camera.transform.position))
					print(j);
		}
	}

}