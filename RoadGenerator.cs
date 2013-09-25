using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class RoadGenerator
{
	public static float roadWidth=15;
	
	public static Mesh generateMesh(List<Road> roads,List<Road.Node> nodes){
		Mesh mesh=new Mesh();
		Vector3[] verts=new Vector3[roads.Count*4+nodes.Count*4];
		Vector2[] uv=new Vector2[roads.Count*4+nodes.Count*4];
		int[] tris=new int[roads.Count*6+nodes.Count*6];
		int r=0;
		foreach(Road road in roads){
			Vector3 dir,norm;
			dir=road.dir;
			norm=road.normal;
			tris[6*r]=4*r;
			tris[6*r+1]=4*r+1;
			tris[6*r+2]=4*r+3;
			tris[6*r+3]=4*r+3;
			tris[6*r+4]=4*r+2;
			tris[6*r+5]=4*r+0;
			verts[4*r]=road.a+dir*roadWidth/2+norm*roadWidth/2;
			verts[4*r+1]=road.a+dir*roadWidth/2-norm*roadWidth/2;
			verts[4*r+2]=road.b-dir*roadWidth/2+norm*roadWidth/2;
			verts[4*r+3]=road.b-dir*roadWidth/2-norm*roadWidth/2;
			uv[4*r]=new Vector2(0,0);
			uv[4*r+1]=new Vector2(0.5f,0);
			uv[4*r+2]=new Vector2(0,(road.b-road.a-roadWidth*dir).magnitude/roadWidth);
			uv[4*r+3]=new Vector2(0.5f,(road.b-road.a-roadWidth*dir).magnitude/roadWidth);
			r++;
		}
		for(int n=0; n<nodes.Count; n++){
			tris[6*(r+n)]=4*(r+n);
			tris[6*(r+n)+1]=4*(r+n)+1;
			tris[6*(r+n)+2]=4*(r+n)+3;
			tris[6*(r+n)+3]=4*(r+n)+3;
			tris[6*(r+n)+4]=4*(r+n)+2;
			tris[6*(r+n)+5]=4*(r+n)+0;
			verts[4*(r+n)]=nodes[n]+(Vector3.right-Vector3.forward)*roadWidth/2;
			verts[4*(r+n)+1]=nodes[n]+(-Vector3.right-Vector3.forward)*roadWidth/2;
			verts[4*(r+n)+2]=nodes[n]+(Vector3.right+Vector3.forward)*roadWidth/2;
			verts[4*(r+n)+3]=nodes[n]+(-Vector3.right+Vector3.forward)*roadWidth/2;
			switch((int)nodes[n].layout){
			case 15://NSEW
				uv[4*(r+n)]=new Vector2(0.75f,0.5f);
				uv[4*(r+n)+1]=new Vector2(1,0.5f);
				uv[4*(r+n)+2]=new Vector2(0.75f,1);
				uv[4*(r+n)+3]=new Vector2(1,1);
				break;
			case 14://SEW
				uv[4*(r+n)]=new Vector2(0.75f,0f);
				uv[4*(r+n)+1]=new Vector2(0.5f,0f);
				uv[4*(r+n)+2]=new Vector2(0.75f,0.5f);
				uv[4*(r+n)+3]=new Vector2(0.5f,0.5f);
				break;
			case 13://NEW
				uv[4*(r+n)]=new Vector2(0.5f,0.5f);
				uv[4*(r+n)+1]=new Vector2(0.75f,0.5f);
				uv[4*(r+n)+2]=new Vector2(0.5f,0f);
				uv[4*(r+n)+3]=new Vector2(0.75f,0f);
				break;
			case 11://NSE
				uv[4*(r+n)]=new Vector2(0.75f,0.5f);
				uv[4*(r+n)+1]=new Vector2(0.75f,0f);
				uv[4*(r+n)+2]=new Vector2(0.5f,0.5f);
				uv[4*(r+n)+3]=new Vector2(0.5f,0f);
				break;
			case 10://SW
				uv[4*(r+n)]=new Vector2(1f,0f);
				uv[4*(r+n)+1]=new Vector2(0.75f,0f);
				uv[4*(r+n)+2]=new Vector2(1f,0.5f);
				uv[4*(r+n)+3]=new Vector2(0.75f,0.5f);
				break;
			case 9://NW
				uv[4*(r+n)]=new Vector2(1f,0.5f);
				uv[4*(r+n)+1]=new Vector2(1f,0f);
				uv[4*(r+n)+2]=new Vector2(0.75f,0.5f);
				uv[4*(r+n)+3]=new Vector2(0.75f,0f);
				break;
			case 7://NSW
				uv[4*(r+n)]=new Vector2(0.5f,0f);
				uv[4*(r+n)+1]=new Vector2(0.5f,0.5f);
				uv[4*(r+n)+2]=new Vector2(0.75f,0f);
				uv[4*(r+n)+3]=new Vector2(0.75f,0.5f);
				break;
			case 6://SE
				uv[4*(r+n)]=new Vector2(0.75f,0f);
				uv[4*(r+n)+1]=new Vector2(1f,0f);
				uv[4*(r+n)+2]=new Vector2(0.75f,0.5f);
				uv[4*(r+n)+3]=new Vector2(1f,0.5f);
				break;
			case 5://NE
				uv[4*(r+n)]=new Vector2(0.75f,0.5f);
				uv[4*(r+n)+1]=new Vector2(1f,0.5f);
				uv[4*(r+n)+2]=new Vector2(0.75f,0f);
				uv[4*(r+n)+3]=new Vector2(1f,0f);
				break;
			default:
				uv[4*(r+n)]=new Vector2(0.5f,0);
				uv[4*(r+n)+1]=new Vector2(1,0);
				uv[4*(r+n)+2]=new Vector2(0.5f,1);
				uv[4*(r+n)+3]=new Vector2(1,1);
				break;
			}
			
		}
		mesh.vertices=verts;
		mesh.triangles=tris;
		mesh.uv=uv;
		mesh.RecalculateNormals();
		return mesh;
	}
}

