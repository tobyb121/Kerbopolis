using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class CityGenerator : MonoBehaviour
{
	public static Material roofMaterial;
	public static Material baseMaterial;
	public static Material[] sideMaterials;
	public static Material roadMaterial;
	public static Material grassMaterial;
	private int _seed;
	private float _cityWidth;
	private float _cityHeight;
	private float _altitude;
	private float _rotation;
	private float _minBlockSize;
	private int _maxSplit;
	private int _splitPasses;
	private Vector3 _radialPos=new Vector3(1,0,0);
    private Vector3 initialUp = Vector3.up;
    private CelestialBody _body;
    private float _visibleRange;

    private PQSCity pqs;
    private List<GameObject> renderers;

    private bool _initialised;
	
	float Latitude {
		get {
			_radialPos.Normalize ();
			return Mathf.Rad2Deg * Mathf.Asin (_radialPos.normalized.y);
		}
		set {
			_radialPos.y = Mathf.Sin (Mathf.Deg2Rad * value);
			_radialPos.Normalize ();
		}
	}

	float Longitude {
		get {
			_radialPos.Normalize ();
			return Mathf.Rad2Deg * Mathf.Atan2 (_radialPos.z, _radialPos.x);
		}
		set {
			_radialPos.Normalize ();
			_radialPos.x = Mathf.Cos (Mathf.Deg2Rad * value);
			_radialPos.z = Mathf.Sin (Mathf.Deg2Rad * value);
		}
	}    

	private Block city;
	private System.Random _rand;
	
	private ProceduralBuilding GenerateBuilding ()
	{
		GameObject buildingObj = new GameObject ();
		buildingObj.transform.parent = this.transform;
		ProceduralBuilding building = buildingObj.AddComponent<ProceduralBuilding> ();
		building.RoofMaterial = roofMaterial;
		building.BaseMaterial = baseMaterial;
        renderers.Add(buildingObj);
		if(building.SideMaterial.shader.name=="KSP/Emissive/Diffuse"){
			Animation animObj=buildingObj.AddComponent<Animation>();
			AnimationClip clip=new AnimationClip();
			AnimationCurve curve=new AnimationCurve();
			float switchOn=(float)(20+5*(_rand.NextDouble()+_rand.NextDouble()));
			curve=new AnimationCurve(
				new Keyframe(0,1),
				new Keyframe(switchOn,1),
				new Keyframe(switchOn+0.001f,0),
				new Keyframe(100,0)
			);
			clip.SetCurve("",typeof(Material),"_EmissiveColor.a",curve);
			clip.wrapMode=WrapMode.Loop;
			animObj.AddClip(clip,"main");
			animObj.clip=animObj["main"].clip;
			animObj["main"].weight=1f;
			TimeOfDayAnimation todAnim=buildingObj.AddComponent<TimeOfDayAnimation>();
		}
		return building;
	}

	private ProceduralBuilding GenerateBuilding (float xWidth, float zWidth, float heightMultiplier)
	{
		ProceduralBuilding building = GenerateBuilding ();
		building.xWidth = xWidth;
		building.zWidth = zWidth;
		building.heightMultiplier = heightMultiplier;
		return building;
	}

	private ProceduralBuilding GenerateBuilding (Vector3 pos, float xWidth, float zWidth, float heightMultiplier)
	{
		ProceduralBuilding building = GenerateBuilding (xWidth, zWidth, heightMultiplier);
		building.transform.localPosition = pos;
		return building;
	}

	private void populateBlock (Block block)
	{
		float r = (block.centre - city.centre).magnitude * 2 / city.width;
		float h_multiplier = Mathf.Exp (-3 * r * r);
		float L, w, x;
		ProceduralBuilding building = null;
		GameObject blockBase = block.createBase (gameObject, roadMaterial);
        renderers.Add(blockBase);

		//Corners
		GenerateBuilding (new Vector3 (block.left, 0, block.bottom), 20, 20, h_multiplier);
		GenerateBuilding (new Vector3 (block.left, 0, block.top - 20), 20, 20, h_multiplier);
		GenerateBuilding (new Vector3 (block.right - 20, 0, block.bottom), 20, 20, h_multiplier);
		GenerateBuilding (new Vector3 (block.right - 20, 0, block.top - 20), 20, 20, h_multiplier);
		//LeftEdge
		L = block.height - 40;
		x = 0;
		while (L >= ProceduralBuilding.minWidth) {
			w = (float)(_rand.NextDouble () * (Mathf.Min (L, ProceduralBuilding.maxWidth) - ProceduralBuilding.minWidth) + ProceduralBuilding.minWidth);
			building = GenerateBuilding (new Vector3 (block.left, 0, block.bottom + 20 + x), 0, w, h_multiplier);
			L -= w;
			x += w;
		}
		if (building && x > 0 && L > 0)
			building.zWidth += L;
		//RightEdge
		L = block.height - 40;
		x = 0;
		while (L >= ProceduralBuilding.minWidth) {
			w = (float)(_rand.NextDouble () * (Mathf.Min (L, ProceduralBuilding.maxWidth) - ProceduralBuilding.minWidth) + ProceduralBuilding.minWidth);
			building = GenerateBuilding (0, w, h_multiplier);
			building.transform.localPosition = new Vector3 (block.right - building.xWidth, 0, block.bottom + 20 + x);
			L -= w;
			x += w;
		}
		if (building && x > 0 && L > 0)
			building.zWidth += L;
		//BottomEdge
		L = block.width - 40;
		x = 0;
		while (L >= ProceduralBuilding.minWidth) {
			w = (float)(_rand.NextDouble () * (Mathf.Min (L, ProceduralBuilding.maxWidth) - ProceduralBuilding.minWidth) + ProceduralBuilding.minWidth);
			building = GenerateBuilding (new Vector3 (block.left + 20 + x, 0, block.bottom), w, 0, h_multiplier);
			L -= w;
			x += w;
		}
		if (building && x > 0 && L > 0)
			building.xWidth += L;
		//TopEdge
		L = block.width - 40;
		x = 0;
		while (L >= ProceduralBuilding.minWidth) {
			w = (float)(_rand.NextDouble () * (Mathf.Min (L, ProceduralBuilding.maxWidth) - ProceduralBuilding.minWidth) + ProceduralBuilding.minWidth);
			building = GenerateBuilding (w, 0, h_multiplier);
			building.transform.localPosition = new Vector3 (block.left + 20 + x, 0, block.top - building.zWidth);
			L -= w;
			x += w;
		}
		if (building && x > 0 && L > 0)
			building.xWidth += L;
	}

    public void initialiseCity(CelestialBody body,float latitude, float longitude, float altitude, float rotation, Vector2 size, int seed, float minBlockSize, int maxSplit, int splitPasses, float visibleRange)
	{
        _body = body;
		Longitude = longitude;
		Latitude = latitude;
		_altitude = altitude;
		_rotation = rotation;
		_cityWidth = size.x;
		_cityHeight = size.y;
		_seed = seed;
		_minBlockSize = minBlockSize;
		_maxSplit = maxSplit;
		_splitPasses = splitPasses;
        _initialised = true;
        _visibleRange = visibleRange;
	}

	void Start ()
	{
        Util.DebugPrint("Starting City");

        if (!_initialised)
            throw new System.Exception("No call to CityGenerator.initialiseCity prior to Start");

		_rand = new System.Random (_seed);
				
        transform.parent = _body.transform;
		
        pqs = gameObject.AddComponent<PQSCity> ();
        Util.DebugPrint("PQSCity Component added");
		pqs.repositionToSphere = true;
        pqs.repositionToSphereSurface = false;
        pqs.reorientToSphere = true;
        pqs.repositionRadial = _radialPos;
        pqs.sphere = _body.pqsController;
        pqs.frameDelta = 1;
        pqs.reorientInitialUp = initialUp;
        pqs.modEnabled = true;
        pqs.reorientFinalAngle = _rotation * Mathf.Deg2Rad;
        pqs.repositionRadiusOffset = _altitude;

        Util.DebugPrint("Setting up LOD");
        PQSCity.LODRange range = new PQSCity.LODRange();
        range.objects = new GameObject[0];
        range.visibleRange = _visibleRange;
        pqs.lod = new PQSCity.LODRange[] { range };
        Util.DebugPrint("LOD created");

        renderers = new List<GameObject>();
        Util.DebugPrint("PQS created");
        
		List<Block> blocks = new List<Block> ();

		city = new Block ();

		city.left = -_cityWidth / 2f;
		city.right = _cityWidth / 2f;
		city.top = _cityHeight / 2f;
		city.bottom = -_cityHeight / 2f;

		blocks.AddRange (city.split (_rand.Next (2, 4), BlockSplit.Vertical, 100));
		for (int j = 0; j < 4; j++) {
			int len = blocks.Count;
			for (int i = 0; i < len; i++) {
				blocks.AddRange (blocks [i].split (_rand.Next (2, 5), (j % 2) > 0 ? BlockSplit.Vertical : BlockSplit.Horizontal, 50));
			}
			blocks.RemoveRange (0, len);
		}

		blocks.RemoveAll (b => (b.centre - city.centre).magnitude > (_cityWidth + _cityHeight) / 4f);

        Util.DebugPrint("Block layout complete");

		List<Road.Node> nodes = new List<Road.Node> (blocks.Count * 4);

		foreach (Block b in blocks) {
			nodes.AddRange (b.getNodes ());
		}

		nodes = nodes.Distinct ().ToList ();

        Util.DebugPrint("Road Nodes complete");

		List<Road> roads = new List<Road> ();

		foreach (Block b in blocks) {
			IEnumerable<Road.Node> blockNodes = (from Road.Node n in nodes orderby n.v.x, n.v.z where n.v.x >= b.left && n.v.x <= b.right && n.v.z <= b.top && n.v.z >= b.bottom select n);
			List<Road.Node> leftEdge = (from Road.Node n in blockNodes where n.v.x == b.left select n).ToList ();
			List<Road.Node> rightEdge = (from Road.Node n in blockNodes where n.v.x == b.right select n).ToList ();
			List<Road.Node> topEdge = (from Road.Node n in blockNodes where n.v.z == b.top select n).ToList ();
			List<Road.Node> bottomEdge = (from Road.Node n in blockNodes where n.v.z == b.bottom select n).ToList ();
			for (int i = 0; i < leftEdge.Count - 1; i++) {
				roads.Add (new Road (leftEdge [i], leftEdge [i + 1]));
				leftEdge [i].layout |= Road.NodeLayout.N;
				leftEdge [i + 1].layout |= Road.NodeLayout.S;
			}
			for (int i = 0; i < rightEdge.Count - 1; i++) {
				roads.Add (new Road (rightEdge [i], rightEdge [i + 1]));
				rightEdge [i].layout |= Road.NodeLayout.N;
				rightEdge [i + 1].layout |= Road.NodeLayout.S;
			}
			for (int i = 0; i < topEdge.Count - 1; i++) {
				roads.Add (new Road (topEdge [i], topEdge [i + 1]));
				topEdge [i].layout |= Road.NodeLayout.E;
				topEdge [i + 1].layout |= Road.NodeLayout.W;
			}
			for (int i = 0; i < bottomEdge.Count - 1; i++) {
				roads.Add (new Road (bottomEdge [i], bottomEdge [i + 1]));
				bottomEdge [i].layout |= Road.NodeLayout.E;
				bottomEdge [i + 1].layout |= Road.NodeLayout.W;
			}
		}

		roads = roads.Distinct ().ToList ();

        Util.DebugPrint("Road map calculated");

		Mesh roadMesh = RoadGenerator.generateMesh (roads, nodes);
		GameObject roadObj = new GameObject ();
		roadObj.layer=15;
		MeshFilter roadMeshFilter = roadObj.AddComponent<MeshFilter> ();
		roadMeshFilter.mesh = roadMesh;
		MeshRenderer roadRenderer = roadObj.AddComponent<MeshRenderer> ();
		roadRenderer.material = roadMaterial;
		roadObj.transform.parent = transform;
		BoxCollider roadCollider = roadObj.AddComponent<BoxCollider> ();
		Vector3 temp = roadCollider.size;
		temp.y = 5;
		roadCollider.size = temp;
		temp = roadCollider.center;
		temp.y = -2.5f;
		roadCollider.center = temp;
        renderers.Add(roadObj);

		Util.DebugPrint("Creating Terrain Bed");
		GameObject terrainBedObj=new GameObject();
		terrainBedObj.layer=15;
		terrainBedObj.transform.parent=transform;
		TerrainBed terrainBed=terrainBedObj.AddComponent<TerrainBed>();
		terrainBed.city=city;
		terrainBed.material=grassMaterial;
		renderers.Add(terrainBedObj);
		
		Util.DebugPrint("Populating Blocks");

		foreach (Block b in blocks) {
			b.pad (RoadGenerator.roadWidth / 2);
			populateBlock (b);
		}
        Util.DebugPrint("Blocks Populated");

        Util.DebugPrint("Setting LOD renderers");
        range.renderers = renderers.ToArray();
        range.Setup();
        
        Util.DebugPrint("Starting PQS");
        pqs.OnSetup();
        pqs.Orientate();
	}

    private int f;

    void Update()
    {
        if((f++%20)==0)
            pqs.OnUpdateFinished();
    }
}
