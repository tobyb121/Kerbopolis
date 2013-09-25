using System;
using UnityEngine;

public struct Road
{
	private Vector3 _a;

	public Vector3 a
	{
		get{ return _a;}	
	}

	private Vector3 _b;

	public Vector3 b
	{
		get { return _b;}	
	}

	public Vector3 dir {
		get{ return (_b - _a).normalized;}
	}
	
	public Vector3 normal{
		get{ return new Vector3(dir.z,0,-dir.x);}
	}
		
	public Road (Vector3 a, Vector3 b)
	{
		if (a.x < b.x || (a.x == b.x && a.z > b.z)) {
			_a = a;
			_b = b;
		} else {
			_a = b;
			_b = a;
		}
	}
	
	public class Node:IEquatable<Node>
	{
		public Vector3 v;
		public NodeLayout layout;
		public Node(){
			v=new Vector3();
			layout=NodeLayout.none;
		}
		public Node(Vector3 vector){
			this.v=vector;
			layout=NodeLayout.none;	
		}	
		public static implicit operator Vector3(Node n){
			return n.v;
		}
		public static implicit operator Node(Vector3 vect){
			Node n=new Node();
			n.v=vect;
			return n;
		}
		
		public override bool Equals (object obj)
		{
			if(obj is Node)
				return Equals(obj as Node);
			return base.Equals (obj);
		}
		
		public override int GetHashCode ()
		{
			return v.GetHashCode();
		}

		public bool Equals (Node other)
		{
			return (other.v==this.v);
		}
	}
	
	public enum NodeLayout{
		none=0x00,
		N=0x01,
		S=0x02,
		E=0x04,
		W=0x08
	}
}