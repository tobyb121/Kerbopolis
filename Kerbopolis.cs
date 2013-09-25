using System;
using System.Collections;
using UnityEngine;
using UnityEngineInternal;

namespace Kerbopolis
{
    [KSPAddon(KSPAddon.Startup.Flight,false)]
    public class Kerbopolis:MonoBehaviour
    {
        void Start()
        {
            Util.DebugPrint("Starting Kerbopolis");
            gameObject.AddComponent<CityGenerator>();
            transform.parent = FlightGlobals.getMainBody().transform;
            pqs=gameObject.AddComponent<PQSCity>();
            pqs.repositionToSphere = true;
            pqs.repositionToSphereSurface = true;
            pqs.reorientToSphere=true;
            radialPos=FlightGlobals.getMainBody().transform.InverseTransformPoint(FlightGlobals.ActiveVessel.transform.position);
            radialPos.Normalize();
            pqs.repositionRadial = radialPos;
            pqs.repositionRadiusOffset = 0;
            pqs.sphere = FlightGlobals.getMainBody().pqsController;
            pqs.frameDelta = 1;
            pqs.reorientInitialUp = initialUp;
            pqs.reorientFinalAngle = 0;
            pqs.modEnabled = true;
            pqs.reorientFinalAngle = 0;
            pqs.repositionRadiusOffset = 0;
            pqs.OnSetup();
            pqs.Orientate();
            
            windowPos = new Rect(100, 100, 100, 100);
        }
        PQSCity pqs;
        Rect windowPos;

        Vector3 radialPos;
        Vector3 initialUp = Vector3.up;
        void Update()
        {
        }

        void OnGUI()
        {
            windowPos=GUILayout.Window(1, windowPos, drawWindow, "Move City",GUILayout.Width(400));
        }

        float Latitude
        {
            get {
                radialPos.Normalize();
                return Mathf.Rad2Deg*Mathf.Asin(radialPos.normalized.y);
            }
            set {
                radialPos.y = Mathf.Sin(Mathf.Deg2Rad * value);
                radialPos.Normalize();
            }
        }

        float Longitude
        {
            get {
                radialPos.Normalize();
                return Mathf.Rad2Deg * Mathf.Atan2(radialPos.z, radialPos.x);
            }
            set {
                radialPos.Normalize();
                radialPos.x = Mathf.Cos(Mathf.Deg2Rad * value);
                radialPos.z = Mathf.Sin(Mathf.Deg2Rad * value);
            }
        }

        void drawWindow(int windowID)
        {
            GUILayout.BeginVertical();
                GUILayout.Label("Initial Up");
                GUILayout.BeginHorizontal();
                GUILayout.Label("X: ", GUILayout.ExpandWidth(false));
                    initialUp.x=GUILayout.HorizontalSlider(initialUp.x, -1, 1,GUILayout.ExpandWidth(true));
                    GUILayout.Label(initialUp.x.ToString());
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Y: ", GUILayout.ExpandWidth(false));
                    initialUp.y = GUILayout.HorizontalSlider(initialUp.y, -1, 1, GUILayout.ExpandWidth(true));
                    GUILayout.Label(initialUp.y.ToString());
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Z: ", GUILayout.ExpandWidth(false));
                    initialUp.z = GUILayout.HorizontalSlider(initialUp.z, -1, 1, GUILayout.ExpandWidth(true));
                    GUILayout.Label(initialUp.z.ToString());
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Angle: ", GUILayout.ExpandWidth(false));
                pqs.reorientFinalAngle = GUILayout.HorizontalSlider(pqs.reorientFinalAngle, -1, 1, GUILayout.ExpandWidth(true));
                GUILayout.EndHorizontal();
                GUILayout.Label("Radial Pos");
                GUILayout.BeginHorizontal();
                GUILayout.Label("Lat: ", GUILayout.Width(50));
                if (GUILayout.Button("-")) Latitude -= 0.01f;
                GUILayout.Label(Latitude.ToString(),GUILayout.Width(100));
                if (GUILayout.Button("+")) Latitude += 0.01f;
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                GUILayout.Label("Lon: ", GUILayout.Width(50));
                if (GUILayout.Button("-")) Longitude -= 0.01f;
                GUILayout.Label(Longitude.ToString(), GUILayout.Width(100));
                if (GUILayout.Button("+")) Longitude += 0.01f;
                GUILayout.EndHorizontal();
            pqs.reorientInitialUp = initialUp;
            pqs.repositionRadial = radialPos;
            pqs.repositionRadiusOffset = 0;
            if (GUILayout.Button("Orientate"))
            {
                Vector3 pos = transform.localPosition;
                print(transform.localPosition);
                pqs.Orientate();
                print(transform.localPosition);
                //transform.localPosition = pos;
            }
            GUILayout.EndVertical();
        }
    }
}
