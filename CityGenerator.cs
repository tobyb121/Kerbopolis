using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class CityGenerator : MonoBehaviour
{
	public Material roofMaterial;
	public Material baseMaterial;
	public Material[] sideMaterials;
	public Material roadMaterial;
	public int Seed=3;
	
	public int CityWidth=500;
	public int CityHeight=500;
	
	private Block city;
	
	public CityGenerator ()
	{
	}
	
	private static System.Random _rand;
	
	private ProceduralBuilding GenerateBuilding (){
		GameObject buildingObj = new GameObject ();
		buildingObj.transform.parent = this.transform;
		ProceduralBuilding building = buildingObj.AddComponent<ProceduralBuilding> ();
		building.RoofMaterial = roofMaterial;
		building.BaseMaterial = baseMaterial;
		return building;
	}
	
	private ProceduralBuilding GenerateBuilding (float xWidth, float zWidth, float heightMultiplier)
	{
		ProceduralBuilding building=GenerateBuilding();
		building.xWidth=xWidth;
		building.zWidth=zWidth;
		building.heightMultiplier=heightMultiplier;
		return building;
	}
	private ProceduralBuilding GenerateBuilding (Vector3 pos, float xWidth, float zWidth, float heightMultiplier){
		ProceduralBuilding building=GenerateBuilding(xWidth,zWidth,heightMultiplier);
		building.transform.localPosition=pos;
		return building;
	}
	
	private void populateBlock(Block block){
		float r=(block.centre-city.centre).magnitude*2/city.width;
		float h_multiplier=Mathf.Exp (-3 * r * r);
		float L,w,x;
		ProceduralBuilding building=null;
		GameObject blockBase=block.createBase(gameObject,roadMaterial);
		
		//Corners
		GenerateBuilding(new Vector3(block.left,0,block.bottom),20,20,h_multiplier);
		GenerateBuilding(new Vector3(block.left,0,block.top-20),20,20,h_multiplier);
		GenerateBuilding(new Vector3(block.right-20,0,block.bottom),20,20,h_multiplier);
		GenerateBuilding(new Vector3(block.right-20,0,block.top-20),20,20,h_multiplier);
		//LeftEdge
		L=block.height-40;
		x=0;
		while(L>=ProceduralBuilding.minWidth){
			w=(float)(_rand.NextDouble()*(Mathf.Min (L,ProceduralBuilding.maxWidth)-ProceduralBuilding.minWidth)+ProceduralBuilding.minWidth);
			building=GenerateBuilding(new Vector3(block.left,0,block.bottom+20+x),0,w,h_multiplier);
			L-=w;
			x+=w;
		}
		if(building&&x>0&&L>0) building.zWidth+=L;
		//RightEdge
		L=block.height-40;
		x=0;
		while(L>=ProceduralBuilding.minWidth){
			w=(float)(_rand.NextDouble()*(Mathf.Min (L,ProceduralBuilding.maxWidth)-ProceduralBuilding.minWidth)+ProceduralBuilding.minWidth);
			building=GenerateBuilding(0,w,h_multiplier);
			building.transform.localPosition=new Vector3(block.right-building.xWidth,0,block.bottom+20+x);
			L-=w;
			x+=w;
		}
		if(building&&x>0&&L>0) building.zWidth+=L;
		//BottomEdge
		L=block.width-40;
		x=0;
		while(L>=ProceduralBuilding.minWidth){
			w=(float)(_rand.NextDouble()*(Mathf.Min (L,ProceduralBuilding.maxWidth)-ProceduralBuilding.minWidth)+ProceduralBuilding.minWidth);
			building=GenerateBuilding(new Vector3(block.left+20+x,0,block.bottom),w,0,h_multiplier);
			L-=w;
			x+=w;
		}
		if(building&&x>0&&L>0) building.xWidth+=L;
		//TopEdge
		L=block.width-40;
		x=0;
		while(L>=ProceduralBuilding.minWidth){
			w=(float)(_rand.NextDouble()*(Mathf.Min (L,ProceduralBuilding.maxWidth)-ProceduralBuilding.minWidth)+ProceduralBuilding.minWidth);
			building=GenerateBuilding(w,0,h_multiplier);
			building.transform.localPosition=new Vector3(block.left+20+x,0,block.top-building.zWidth);
			L-=w;
			x+=w;
		}
		if(building&&x>0&&L>0) building.xWidth+=L;
	}
	
	private Texture2D loadTexture(string fileName){
		Texture2D texture;
		byte[] imagedata;
		texture=new Texture2D(128,128);
		try{
			System.IO.FileStream file=System.IO.File.OpenRead(fileName);
			imagedata=new byte[file.Length];
			file.Read(imagedata,0,(int)file.Length);
			texture.LoadImage(imagedata);
		}
		catch(System.Exception e){
            Util.DebugPrint("Error: " + e.Message);
        }
		return texture;
	}

	void Start ()
	{	
		_rand=new System.Random(Seed);
		/*Shader diffuse=Shader.Find("Diffuse");
		baseMaterial=new Material(diffuse);
		baseMaterial.mainTexture=GameDatabase.Instance.GetTexture("Kerbopolis/Textures/shops",false);
		roofMaterial=new Material(diffuse);
		roofMaterial.mainTexture=GameDatabase.Instance.GetTexture("Kerbopolis/Textures/roof",false);
		roadMaterial=new Material(diffuse);
		roadMaterial.mainTexture=GameDatabase.Instance.GetTexture("Kerbopolis/Textures/roads",false);
		sideMaterials=new Material[4];
		sideMaterials[0]=new Material(diffuse);
		sideMaterials[0].mainTexture=GameDatabase.Instance.GetTexture("Kerbopolis/Textures/buildings0",false);
		sideMaterials[1]=new Material(diffuse);
		sideMaterials[1].mainTexture=GameDatabase.Instance.GetTexture("Kerbopolis/Textures/buildings1",false);
		sideMaterials[2]=new Material(diffuse);
		sideMaterials[2].mainTexture=GameDatabase.Instance.GetTexture("Kerbopolis/Textures/buildings2",false);
		sideMaterials[3]=new Material(diffuse);
		sideMaterials[3].mainTexture=GameDatabase.Instance.GetTexture("Kerbopolis/Textures/buildings3",false);
        */
        ProceduralBuilding.SideMaterials=sideMaterials;
        
        
		List<Block> blocks=new List<Block>();
		
		city = new Block ();
				
		city.left = -CityWidth/2f;
		city.right = CityWidth/2f;
		city.top = CityHeight/2f;
		city.bottom = -CityHeight/2f;
				
		blocks.AddRange (city.split (_rand.Next(2,4), BlockSplit.Vertical,100));
		for (int j=0; j<4; j++) {
			int len = blocks.Count;
			for (int i=0; i<len; i++) {
				blocks.AddRange (blocks [i].split (_rand.Next(2,5),(j % 2) > 0 ? BlockSplit.Vertical : BlockSplit.Horizontal,50));
			}
			blocks.RemoveRange (0, len);
		}
		
		blocks.RemoveAll(b=>(b.centre-city.centre).magnitude>(CityWidth+CityHeight)/4f);
		
		List<Road.Node> nodes=new List<Road.Node>(blocks.Count*4);
		
		foreach(Block b in blocks){
			nodes.AddRange(b.getNodes());
		}
		
		nodes=nodes.Distinct().ToList();
		
		List<Road> roads=new List<Road>();
		
		foreach(Block b in blocks){
			IEnumerable<Road.Node> blockNodes=(from Road.Node n in nodes orderby n.v.x,n.v.z where n.v.x>=b.left&&n.v.x<=b.right&&n.v.z<=b.top&&n.v.z>=b.bottom select n);
			List<Road.Node> leftEdge=(from Road.Node n in blockNodes where n.v.x==b.left select n).ToList();
			List<Road.Node> rightEdge=(from Road.Node n in blockNodes where n.v.x==b.right select n).ToList();
			List<Road.Node> topEdge=(from Road.Node n in blockNodes where n.v.z==b.top select n).ToList();
			List<Road.Node> bottomEdge=(from Road.Node n in blockNodes where n.v.z==b.bottom select n).ToList();
			for(int i=0; i<leftEdge.Count-1; i++){
				roads.Add(new Road(leftEdge[i],leftEdge[i+1]));
				leftEdge[i].layout|=Road.NodeLayout.N;
				leftEdge[i+1].layout|=Road.NodeLayout.S;
			}
			for(int i=0; i<rightEdge.Count-1; i++){
				roads.Add(new Road(rightEdge[i],rightEdge[i+1]));
				rightEdge[i].layout|=Road.NodeLayout.N;
				rightEdge[i+1].layout|=Road.NodeLayout.S;
			}
			for(int i=0; i<topEdge.Count-1; i++){
				roads.Add(new Road(topEdge[i],topEdge[i+1]));
				topEdge[i].layout|=Road.NodeLayout.E;
				topEdge[i+1].layout|=Road.NodeLayout.W;
			}
			for(int i=0; i<bottomEdge.Count-1; i++){
				roads.Add(new Road(bottomEdge[i],bottomEdge[i+1]));
				bottomEdge[i].layout|=Road.NodeLayout.E;
				bottomEdge[i+1].layout|=Road.NodeLayout.W;
			}
		}
		
		
		roads=roads.Distinct().ToList();
		
		Mesh roadMesh=RoadGenerator.generateMesh(roads,nodes);
		GameObject roadObj=new GameObject();
		MeshFilter roadMeshFilter=roadObj.AddComponent<MeshFilter>();
		roadMeshFilter.mesh=roadMesh;
		MeshRenderer roadRenderer=roadObj.AddComponent<MeshRenderer>();
		roadRenderer.material=roadMaterial;
		roadObj.transform.parent=transform;
		BoxCollider roadCollider=roadObj.AddComponent<BoxCollider>();
		Vector3 temp=roadCollider.size;
		temp.y=5;
		roadCollider.size=temp;
		temp=roadCollider.center;
		temp.y=-2.5f;
		roadCollider.center=temp;
		
		foreach(Block b in blocks){
			b.pad(RoadGenerator.roadWidth/2);
			populateBlock(b);
		}
	}
}
