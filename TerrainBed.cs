using UnityEngine;
using System.Collections;

public class TerrainBed : MonoBehaviour
{
	
	public Block city;
	public Material material;
	
	void Start ()
	{
		MeshFilter meshFilter=gameObject.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer=gameObject.AddComponent<MeshRenderer>();
		
		Mesh mesh=new Mesh();
		mesh.vertices=new Vector3[]{
			new Vector3(1.1f*city.left,-0.05f,1.1f*city.bottom),
			new Vector3(1.1f*city.left,-0.05f,1.1f*city.top),
			new Vector3(1.1f*city.right,-0.05f,1.1f*city.top),
			new Vector3(1.1f*city.right,-0.05f,1.1f*city.bottom),
			new Vector3(2f*city.left,-50,2f*city.bottom),
			new Vector3(2f*city.left,-50,2f*city.top),
			new Vector3(2f*city.right,-50,2f*city.top),
			new Vector3(2f*city.right,-50,2f*city.bottom)
		};
		mesh.triangles=new int[]{
			0,1,2,
			2,3,0,
			4,0,3,
			3,7,4,
			5,1,0,
			0,4,5,
			6,2,1,
			1,5,6,
			7,3,2,
			2,6,7
		};
		mesh.uv=new Vector2[]{
			new Vector2(city.left/100,city.bottom/100),
			new Vector2(city.left/100,city.top/100),
			new Vector2(city.right/100,city.top/100),
			new Vector2(city.right/100,city.bottom/100),
			new Vector2((city.left-city.width)/100,(city.bottom-city.height)/100),
			new Vector2((city.left-city.width)/100,(city.top+city.height)/100),
			new Vector2((city.right+city.width)/100,(city.top+city.height)/100),
			new Vector2((city.width+city.width)/100,(city.bottom-city.height)/100)
		};
		mesh.RecalculateNormals();
		
		meshFilter.mesh=mesh;
		meshRenderer.material=material;
		
		MeshCollider meshCollider=gameObject.AddComponent<MeshCollider>();
		meshCollider.convex=true;
		meshCollider.sharedMesh=mesh;
	}
}

