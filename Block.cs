using System;
using UnityEngine;

public struct Block
{	
	public float left;
	public float right;
	public float top;
	public float bottom;

	public float width {
		get{ return right - left;}
		set{ right = left + value;}
	}

	public float height {
		get{ return top - bottom;}
		set{ top = bottom + value;}
	}
		
	public Vector3 centre {
		get	{ return new Vector3 (left + 0.5f * width, 0, bottom + 0.5f * height);}
	}
	
	public GameObject createBase (GameObject parent, Material material)
	{
		GameObject blockBase = new GameObject ();
		blockBase.transform.parent=parent.transform;
		Mesh mesh = new Mesh ();
		mesh.vertices = new Vector3[]{
			new Vector3 (right, 0, bottom),
			new Vector3 (left, 0, bottom),
			new Vector3 (right, 0, top),
			new Vector3 (left, 0, top)
		};
		mesh.triangles = new int[]{
			1,3,0,
			0,3,2
		};
		mesh.uv=new Vector2[]{
			new Vector2(0.75f,0.5f),
			new Vector2(0.5f,0.5f),
			new Vector2(0.75f,1f),
			new Vector2(0.5f,1f)
		};
		mesh.RecalculateNormals();
			
		MeshFilter filter = blockBase.AddComponent<MeshFilter> ();
		filter.mesh=mesh;
		MeshRenderer renderer=blockBase.AddComponent<MeshRenderer>();
		renderer.material=material;
		return blockBase;
	}
	
	public Block[] split (int count, BlockSplit splitMode, float minSize)
	{
		Block[] result;
		int n = count;
		if ((splitMode == BlockSplit.Horizontal && this.height / n < minSize) || (splitMode == BlockSplit.Vertical && this.width / n < minSize)) {
			return new Block[]{this};
		}
		result = new Block[n];
		for (int i=0; i<n; i++) {
			Block b = this;
			if (splitMode == BlockSplit.Horizontal) {
				b.bottom = bottom + i * height / n;
				b.top = bottom + (i + 1) * height / n;
			} else {
				b.left = left + i * width / n;
				b.right = left + (i + 1) * width / n;
			}
			result [i] = b;
		}
		return result;
	}
	
	public void pad (float padding)
	{
		left += padding;
		right -= padding;
		top -= padding;
		bottom += padding;
	}
	
	public Road.Node[] getNodes ()
	{
		Road.Node[] nodes = new Road.Node[4];
		nodes [0] = new Vector3 (left, 0, bottom);
		nodes [1] = new Vector3 (right, 0, bottom);
		nodes [2] = new Vector3 (right, 0, top);
		nodes [3] = new Vector3 (left, 0, top);
		return nodes;
	}
	
	public Road[] getRoads ()
	{
		Road[] roads = new Road[4];
		
		return roads;
	}
}
	
public enum BlockSplit
{
	Horizontal,
	Vertical
}