using UnityEngine;
using System.Collections;
using System;

public class ProceduralBuilding : MonoBehaviour {
	
	public static float minHeight=20;
	public static float maxHeight=150;
	public static float minWidth=15;
	public static float maxWidth=25;
	
	private float _height=0;
	public float Height{
		get{return _height;}
		set{_height=value;}
	}
	
	public float heightMultiplier=1;
	
	private float _xWidth=0;
	public float xWidth{
		get{
			if(_xWidth==0) _xWidth = (float)(minWidth + (maxWidth - minWidth) * _rand.NextDouble());
			return _xWidth;
		}
		set{_xWidth=value;}
	}
	public float _zWidth=0;
	public float zWidth{
		get{
			if(_zWidth==0) _zWidth = (float)(minWidth + (maxWidth - minWidth) * _rand.NextDouble());
			return _zWidth;
		}	
		set{_zWidth=value;}
	}
	
	private float texture_height=18;
	
	private static System.Random _rand=new System.Random(20891);
	
	private Material _sideMaterial;
	public Material SideMaterial{
		get{
			return _sideMaterial;	
		}
	}
	public static Material[] SideMaterials;
	public Material BaseMaterial;
	public Material RoofMaterial;
	
	private MeshFilter meshFilter;
	private MeshRenderer meshRenderer;
	
	void Awake() {
		meshFilter = gameObject.AddComponent<MeshFilter>();
		meshRenderer=gameObject.AddComponent<MeshRenderer>();	
		_sideMaterial=SideMaterials[_rand.Next(SideMaterials.Length)];
	}
	
	void Start () {
		int shopIndex=_rand.Next(3);
		
		if(_height==0) _height = (float)(minHeight + (maxHeight - minHeight) * _rand.NextDouble()*heightMultiplier);
        if(_xWidth==0) _xWidth = (float)(minWidth + (maxWidth - minWidth) * _rand.NextDouble());
        if(_zWidth==0) _zWidth = (float)(minWidth + (maxWidth - minWidth) * _rand.NextDouble());
		
		Vector3[] verts=new Vector3[]{
			new Vector3(0,0,0),//lower
			new Vector3(_xWidth,0,0),
			new Vector3(_xWidth,0,_zWidth),
			new Vector3(0,0,_zWidth),
			new Vector3(0,4,0),//middle
			new Vector3(_xWidth,4,0),
			new Vector3(_xWidth,4,_zWidth),
			new Vector3(0,4,_zWidth),
			new Vector3(0,4+_height,0),//upper
			new Vector3(_xWidth,4+_height,0),
			new Vector3(_xWidth,4+_height,_zWidth),
			new Vector3(0,4+_height,_zWidth)
		};
		Vector3[] polys=new Vector3[]{
			verts[0],verts[4],verts[5],verts[1],//0,1,2,3
			verts[1],verts[5],verts[6],verts[2],//4,5,6,7
			verts[2],verts[6],verts[7],verts[3],//8,9,10,11
			verts[3],verts[7],verts[4],verts[0],//12,13,14,15
			verts[4],verts[8],verts[9],verts[5],//16,17,18,19
			verts[5],verts[9],verts[10],verts[6],//20,21,22,23
			verts[6],verts[10],verts[11],verts[7],//24,25,26,27
			verts[7],verts[11],verts[8],verts[4],//28,29,30,31
			verts[8],verts[9],verts[10],verts[11]//32,33,34,35
		};
		
		int[] trisBase=new int[]{
			0,2,3,    0,1,2,
			4,6,7,    4,5,6,
			8,10,11,  8,9,10,
			12,14,15, 12,13,14
		};
		int[] trisSides=new int[]{
			16,18,19, 16,17,18,
			20,22,23, 20,21,22,
			24,26,27, 24,25,26,
			28,30,31, 28,29,30
		};
		int[] trisTop=new int[]{
			32,35,33, 33,35,34
		};
		
		float vy=Mathf.Floor(3*_height/texture_height)/3f;
		float ux=Mathf.Floor(_xWidth/7.5f);
		float uz=Mathf.Floor(_zWidth/7.5f);
				
		Vector2[] uv=new Vector2[]{
			new Vector2(0,(shopIndex)/3f),//base
			new Vector2(0,(shopIndex+1)/3f),
			new Vector2(ux,(shopIndex+1)/3f),
			new Vector2(ux,(shopIndex)/3f),
			new Vector2(0,(shopIndex)/3f),
			new Vector2(0,(shopIndex+1)/3f),
			new Vector2(uz,(shopIndex+1)/3f),
			new Vector2(uz,(shopIndex)/3f),
			new Vector2(0,(shopIndex)/3f),
			new Vector2(0,(shopIndex+1)/3f),
			new Vector2(ux,(shopIndex+1)/3f),
			new Vector2(ux,(shopIndex)/3f),
			new Vector2(0,(shopIndex)/3f),
			new Vector2(0,(shopIndex+1)/3f),
			new Vector2(uz,(shopIndex+1)/3f),
			new Vector2(uz,(shopIndex)/3f),
			new Vector2(0,0),//sides
			new Vector2(0,vy),
			new Vector2(ux,vy),
			new Vector2(ux,0),
			new Vector2(0,0),
			new Vector2(0,vy),
			new Vector2(uz,vy),
			new Vector2(uz,0),
			new Vector2(0,0),
			new Vector2(0,vy),
			new Vector2(ux,vy),
			new Vector2(ux,0),
			new Vector2(0,0),
			new Vector2(0,vy),
			new Vector2(uz,vy),
			new Vector2(uz,0),
			new Vector2(0,0),//top
			new Vector2(0,1),
			new Vector2(1,1),
			new Vector2(1,0)
		};
				
		Mesh mesh=new Mesh();
		mesh.vertices=polys;
		mesh.subMeshCount=3;
		mesh.SetTriangles(trisBase,1);
		mesh.SetTriangles(trisSides,0);
		mesh.SetTriangles(trisTop,2);
		mesh.RecalculateNormals();
		mesh.uv=uv;
		
		
		meshFilter.mesh=mesh;
		meshRenderer.materials=new Material[]{_sideMaterial,BaseMaterial,RoofMaterial};

        gameObject.layer = 15;
        gameObject.AddComponent<BoxCollider>();
	}
}
