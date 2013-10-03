using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineInternal;

namespace Kerbopolis
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
    public class Kerbopolis : MonoBehaviour
    {
        void Start()
		{
			Util.DebugPrint ("Starting Kerbopolis");
						
			Shader diffuse = Shader.Find ("Diffuse");
			Shader emissiveDiffuse = Shader.Find ("KSP/Emissive/Diffuse");
			Color nightLightColour;
			
			ConfigNode config = GameDatabase.Instance.GetConfigNode ("Kerbopolis/Config/KERBOPOLIS");
		
			CityGenerator.baseMaterial = new Material (diffuse);
			if (config.HasValue ("baseTexture")) {
				CityGenerator.baseMaterial.mainTexture = GameDatabase.Instance.GetTexture (config.GetValue ("baseTexture", 0), false);
				Util.DebugPrint ("Set Base Texture");
			}
			CityGenerator.roofMaterial = new Material (diffuse);
			if (config.HasValue ("roofTexture"))
				CityGenerator.roofMaterial.mainTexture = GameDatabase.Instance.GetTexture (config.GetValue ("roofTexture", 0), false);
			CityGenerator.roadMaterial = new Material (diffuse);
			if (config.HasValue ("roadTexture"))
				CityGenerator.roadMaterial.mainTexture = GameDatabase.Instance.GetTexture (config.GetValue ("roadTexture", 0), false);
			CityGenerator.grassMaterial = new Material (diffuse);
			if (config.HasValue ("grassTexture"))
				CityGenerator.grassMaterial.mainTexture = GameDatabase.Instance.GetTexture (config.GetValue ("grassTexture", 0), false);
			if(config.HasValue("nightLightColour"))
				nightLightColour = ConfigNode.ParseVector4(config.GetValue("nightLightColour"));
			else
				nightLightColour=new Color(1,0.82f,0f,1f);
			Util.DebugPrint ("Setting wall textures");
			string[] wallTextures = config.GetValues ("wallTexture");
			Util.DebugPrint ("Got: " + wallTextures.Length.ToString () + " Wall Textures");
			ProceduralBuilding.SideMaterials = new Material[wallTextures.Length];
			Util.DebugPrint ("Initialised Texture Array");
			for (int i = 0; i < wallTextures.Length; i++) {
				Util.DebugPrint ("Setting Wall Texture: " + i.ToString () + " " + wallTextures [i]);
                string[] textureComponents = wallTextures[i].Split(',');
                if (textureComponents.Length > 1)
                {
                    ProceduralBuilding.SideMaterials [i]=new Material(emissiveDiffuse);
                    ProceduralBuilding.SideMaterials [i].SetTexture("_MainTex",GameDatabase.Instance.GetTexture (textureComponents[0], false));
                    ProceduralBuilding.SideMaterials [i].SetTexture("_Emissive",GameDatabase.Instance.GetTexture (textureComponents[1], false));
					ProceduralBuilding.SideMaterials [i].SetColor("_EmissiveColor",nightLightColour);
                }
                else{
				    ProceduralBuilding.SideMaterials [i] = new Material (diffuse);
				    ProceduralBuilding.SideMaterials [i].mainTexture = GameDatabase.Instance.GetTexture (wallTextures [i], false);
                }
            }
			Util.DebugPrint ("Loaded Textures");
			
			ConfigNode[] cityConfigs=config.GetNodes("CITY");
			cities=new List<CityGenerator>();
            Util.DebugPrint("Found "+cityConfigs.Length+" Cities");
			foreach(ConfigNode cityConfig in cityConfigs){
                Util.DebugPrint("Loading city...");
				GameObject city=new GameObject();
				CityGenerator gen=city.AddComponent<CityGenerator> ();
                string bodyName = cityConfig.GetValue("body");
                CelestialBody body = FlightGlobals.Bodies.Find(b => b.GetName() == bodyName);
                if (!body)
                    body = FlightGlobals.getMainBody();
                float lat=float.Parse(cityConfig.GetValue("latitude"));
				Util.DebugPrint("Lat:\t"+lat.ToString());
                float lon = float.Parse(cityConfig.GetValue("longitude"));
				Util.DebugPrint("Lon:\t"+lon.ToString());
                float rot=float.Parse(cityConfig.GetValue("rotation"));
				Util.DebugPrint("rot:\t"+rot.ToString());
                float alt = float.Parse(cityConfig.GetValue("altitude"));
				Util.DebugPrint("alt:\t"+alt.ToString());
                Vector2 size = ConfigNode.ParseVector2(cityConfig.GetValue("size"));
				Util.DebugPrint("size:\t"+size.ToString());
                int seed = int.Parse(cityConfig.GetValue("seed"));
                float minBlockSize = float.Parse(cityConfig.GetValue("minBlockSize"));
                int maxSplit = int.Parse(cityConfig.GetValue("maxSplit"));
                int splitPasses = int.Parse(cityConfig.GetValue("splitPasses"));
                float visibleRange = float.Parse(cityConfig.GetValue("visibleRange"));
                gen.initialiseCity(body,lat,lon,alt,rot,size,seed,minBlockSize,maxSplit,splitPasses,visibleRange);
				cities.Add(gen);
                Util.DebugPrint("Loaded City");
			}
            
			windowPos = new Rect (100, 100, 100, 100);
		}

        private List<CityGenerator> cities;

        Rect windowPos;
    }
}
