using UnityEngine;
using System.Collections;

namespace UVFree {
	//[ExecuteInEditMode]
	public class MaterialController : MonoBehaviour {
		public enum DemoType {
			modelsLocal=0,
			modelsGlobal=1,
			terrains=2
		};

		public int currentVariation = 0;
		public Material[] materials;
		public MaterialVariant[] variations;
		public DemoType demoType = DemoType.modelsLocal;
		public Vector3 modelCamPosition = new Vector3(0.0f, 7.0f, -7.0f);
		public float terrainDistance = 75.0f;
		public float camLerpRate = 3.0f;
		public float camZoom = 1.0f;
		public float turnRate = 180.0f;
		public Transform lightTransform;
		public Camera modelCamera;
		public Camera terrainCamera;
		public Vector3 localLightPosition;
		public Transform[] modelsLocal;
		public Transform[] modelsGlobal;
		public Transform[] modelsTerrain;
		public UnityEngine.UI.Text instructionGUIText;
		public bool guiOn = true;

		private Camera currentCamera;
		private Vector3 camPosition = new Vector3(0.0f, 7.0f, -7.0f);

		private string shaderLabel = "";
		private string texturesLabel = "";

		private int lastVariation = -1;
		private int focusModelIndex = 0;

		private Transform[] models;
		private Transform[][] demoModels;

		private Vector3 modelRotation;
		private Vector3 terrainRotation = new Vector3(45.0f, 0.0f, 0.0f);

		private Vector3 oldMousePosition;

		private TextMesh[] labels;

		void Awake() {

			// initialize array of demo models
			demoModels = new Transform[3][];
			demoModels[0] = modelsLocal;
			demoModels[1] = modelsGlobal;
			demoModels[2] = modelsTerrain;

			// change demo mode
			ChangeDemoMode(demoType);

			// make sure the terrain cam is at a proper rotation
			ClampTerrainRotation();

			FindLabels();
		}

		void FindLabels()
		{
			labels = GameObject.FindObjectsOfType<TextMesh>();
		}
		// Use this for initialization
		void Start () {

			RefreshShaderLabel();
			RefreshSelectedTerrain();
			RefreshInstructionGUIText();
			RefreshModelLabels();
		}
		
		// Update is called once per frame
		void Update () {
			UpdateInputs();
			UpdateMaterials();
			UpdateSpinning();
			UpdateFocus();
			UpdateLightPosition();
		}

		// update the light position to stay in the camera view
		void UpdateLightPosition()
		{
			lightTransform.position = currentCamera.transform.localToWorldMatrix.MultiplyPoint(localLightPosition);
			lightTransform.GetChild (0).gameObject.SetActive (guiOn);
			lightTransform.GetChild (1).gameObject.SetActive (guiOn);
		}

		void RefreshInstructionGUIText()
		{
			instructionGUIText.gameObject.SetActive(guiOn);
		}

		void RefreshModelLabels()
		{
			foreach(TextMesh label in labels)
			{
				label.gameObject.SetActive (guiOn);
			}
		}
		// apply the keyboard/mouse inputs
		void UpdateInputs()
		{
			
			// spacebar for toggling gui
			if (Input.GetKeyDown (KeyCode.Space))
			{
				guiOn = !guiOn;
				RefreshInstructionGUIText();
				RefreshModelLabels ();
			}
			
			// left arrow for previous focus
			if (Input.GetKeyDown (KeyCode.LeftArrow))
			{
				SelectPrevModel();
			}
			
			// right arrow for next focus
			if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				SelectNextModel();
			}
			
			// up arrow for prev texture
			if (Input.GetKeyDown (KeyCode.UpArrow))
			{
				SelectPrevTexture();
			}
			
			// down arrow for next texture
			if (Input.GetKeyDown (KeyCode.DownArrow))
			{
				SelectNextTexture();
			}
			
			// zooming
			float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
			camZoom -= scrollWheel * 0.1f;
			
			// dragging
			Vector3 newMousePosition = currentCamera.ScreenToViewportPoint(Input.mousePosition);
			Vector3 mouseDelta = oldMousePosition - newMousePosition;
			
			float h = mouseDelta.x;
			float v = mouseDelta.y;
			
			// left mouse to rotate models or camera for terrain
			if (Input.GetMouseButton (0))
			{
				if (demoType == DemoType.terrains)
				{
					terrainRotation.y -= h * turnRate;
					terrainRotation.x += v * turnRate;
					ClampTerrainRotation();
				}
				else
				{
					modelRotation.y += h * turnRate;
					modelRotation.x -= v * turnRate;
				}
			}
			
			// right button to rotate light
			if (Input.GetMouseButton (1))
			{
				lightTransform.Rotate (
					-v*180.0f,
					h*180.0f,
					0.0f, 
					Space.World
					);
			}
			
			oldMousePosition = newMousePosition;
		}


		void ClampTerrainRotation()
		{
			terrainRotation.x = ClampAngle(terrainRotation.x, 15.0f, 89.0f);
			terrainRotation.y = ClampAngle(terrainRotation.y, -360.0f, 360.0f);
		}

		static float ClampAngle (float angle, float min, float max) {

			if (angle < -360)
				angle += 360;

			if (angle > 360)
				angle -= 360;

			return Mathf.Clamp (angle, min, max);
		}

		void RefreshModels()
		{
			models = demoModels[(int) demoType];
			EnsureFocusModelSelected();
		}

		void SelectNextModel()
		{
			focusModelIndex++;
			RefreshShaderLabel();
			RefreshSelectedTerrain();
		}

		void SelectPrevModel()
		{
			focusModelIndex--;
			RefreshShaderLabel();
			RefreshSelectedTerrain();
		}

		void RefreshShaderLabel()
		{
			if (!EnsureFocusModelSelected())
			{
				shaderLabel = "No Shaders";
				return;
			}
			shaderLabel = "Shader: " + models[focusModelIndex].name;
		}

		bool EnsureFocusModelSelected()
		{
			if (models.Length == 0) 
				return false;

			if (focusModelIndex < 0)
			{
				focusModelIndex = models.Length-1;
			}
			else if (focusModelIndex >= models.Length)
			{
				focusModelIndex = 0;
			}

			return true;
		}

		void RefreshSelectedTerrain()
		{
			if (demoType != DemoType.terrains) return;

			if (!EnsureFocusModelSelected()) return;

			for(int i = 0; i < models.Length; i++)
			{
				models[i].gameObject.SetActive (i == focusModelIndex);
			}
		}

		void ChangeDemoMode(DemoType newDemoMode)
		{
			DemoType oldDemoType = demoType;

			if (oldDemoType == DemoType.terrains && newDemoMode != DemoType.terrains
			    || newDemoMode == DemoType.terrains && oldDemoType != DemoType.terrains)
			{
				focusModelIndex = 0;

			}

			demoType = newDemoMode;
			RefreshModels ();


			// hide or show local, global, and terrains based on the new demoType
			if (demoType == DemoType.terrains)
			{
				terrainCamera.enabled = true;
				modelCamera.enabled = false;
				currentCamera = terrainCamera;

				EnableRenderers(modelsLocal, false);
				EnableRenderers(modelsGlobal, false);
				EnableObjects(modelsTerrain, true);

			}
			else
			{
				modelCamera.enabled = true;
				terrainCamera.enabled = false;
				currentCamera = modelCamera;
				camPosition = modelCamPosition;

				EnableRenderers(modelsLocal, true);
				EnableRenderers(modelsGlobal, true);
				EnableObjects(modelsTerrain, false);
			}

			RefreshShaderLabel();
			RefreshSelectedTerrain();
		}

		void EnableRenderers(Transform[] modelList, bool visibility)
		{
			foreach(Transform model in modelList)
			{
				model.GetComponent<Renderer>().enabled = visibility;
			}
		}

		void EnableObjects(Transform[] objects, bool visibility)
		{
			foreach(Transform t in objects)
			{
				t.gameObject.SetActive (visibility);
			}
		}

		void OnGUI () {

			if (!guiOn) return;

			GUILayout.BeginVertical ("box", GUILayout.Width(240));
			{
				GUILayout.BeginVertical ("box");
				{
					GUILayout.Label ("Choose a shader type");
					GUILayout.BeginHorizontal ();
					{
						if (GUILayout.Button (demoType == DemoType.modelsLocal ? "> Local <" :"Local", GUILayout.Width (80)))
						{
							ChangeDemoMode(DemoType.modelsLocal);
						}

						if (GUILayout.Button (demoType == DemoType.modelsGlobal ? "> Global <" :"Global", GUILayout.Width (80)))
						{
							ChangeDemoMode(DemoType.modelsGlobal);
						}

						if (GUILayout.Button (demoType == DemoType.terrains ? "> Terrain <" :"Terrain", GUILayout.Width (80)))
						{
							ChangeDemoMode(DemoType.terrains);
						}
					}
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndVertical ();

				// focus section
				GUILayout.BeginVertical ("box");
				{
					GUILayout.Label (shaderLabel, GUILayout.MinHeight (35));
					GUILayout.BeginHorizontal ();
					{
						if (GUILayout.Button ("Prev"))
						{
							SelectPrevModel();
						}
						if (GUILayout.Button("Next"))
						{
							SelectNextModel();
						}
					}
					GUILayout.EndHorizontal ();
				}
				GUILayout.EndVertical ();

				// texture section
				if (IsDemoModel())
				{
					GUILayout.BeginVertical ("box");
					{
						GUILayout.Label (texturesLabel, GUILayout.MinHeight (35));
						GUILayout.BeginHorizontal ();
						{
							if (GUILayout.Button ("Prev"))
							{
								SelectPrevTexture();
							}

							if (GUILayout.Button ("Next"))
							{
								SelectNextTexture();
							}
						}
						GUILayout.EndHorizontal ();
					}
					GUILayout.EndVertical ();
				}
			}
			GUILayout.EndVertical ();
		}

		void SelectPrevTexture()
		{
			currentVariation--;
		}

		void SelectNextTexture()
		{
			currentVariation++;
		}

		bool IsDemoModel()
		{
			return (demoType == DemoType.modelsLocal || demoType == DemoType.modelsGlobal);
		}
		void UpdateSpinning()
		{
			if (models.Length == 0) return;

			if (IsDemoModel())
			{
				foreach(Transform model in modelsLocal)
				{
					model.rotation = Quaternion.Euler(modelRotation);								
				}
				foreach(Transform model in modelsGlobal)
				{
					model.rotation = Quaternion.Euler(modelRotation);								
				}
			}
		}

		void UpdateFocus()
		{
			if (!EnsureFocusModelSelected()) return;

			Transform focusTransform = models[focusModelIndex].transform;
			Transform mainCameraTransform = currentCamera.transform;

			camZoom = Mathf.Clamp (camZoom, 0.5f, 1.5f);

			Vector3 focusPosition = focusTransform.position;
			Vector3 newPosition = focusPosition;

			if (IsDemoModel())
			{
				// calculate the desired new camera position
				newPosition += camZoom*camPosition;

				// and lerp the position to there and rotation to as if it was from there
				mainCameraTransform.position = Vector3.Lerp (mainCameraTransform.position, newPosition, camLerpRate * Time.deltaTime);
				Quaternion newRotation = Quaternion.LookRotation (focusPosition - newPosition);
				mainCameraTransform.rotation = Quaternion.Lerp (mainCameraTransform.rotation, newRotation, camLerpRate * Time.deltaTime);
			}
			else if (demoType == DemoType.terrains)
			{
				newPosition = Quaternion.Euler (terrainRotation) * new Vector3(0.0f, 0.0f, -terrainDistance*camZoom);

				// no lerping, just jump directly
				mainCameraTransform.position = newPosition;
				Quaternion newRotation = Quaternion.LookRotation(-newPosition);
				mainCameraTransform.rotation = newRotation;
			}
		}
		bool EnsureVariationSelected()
		{
			if (variations.Length == 0) return false;

			if (currentVariation < 0) 
			{
				currentVariation = variations.Length-1;
			}
			else if (currentVariation >= variations.Length)
			{
				currentVariation = 0;
			}

			return true;
		}

		void UpdateMaterials()
		{
			if (currentVariation == lastVariation) return;
			if (!EnsureVariationSelected()) return;

			lastVariation = currentVariation;
			MaterialVariant variant = variations[currentVariation];

			foreach(Material material in materials)
			{
				SetTexture(material, "_MainTex", variant.mainTex, variant.mainScale);
				SetTexture(material, "_TopTex", variant.topTex, variant.topScale);
				SetTexture(material, "_BottomTex", variant.bottomTex, variant.mainScale);
				SetTexture(material, "_LeftTex", variant.leftTex, variant.mainScale);
				SetTexture(material, "_RightTex", variant.rightTex, variant.mainScale);
				SetTexture(material, "_FrontTex", variant.frontTex, variant.mainScale);
				SetTexture(material, "_BackTex", variant.backTex, variant.mainScale);
				SetTexture(material, "_BumpMap", variant.bump, variant.mainScale);
				SetTexture(material, "_TopBump", variant.topBump, variant.topScale);
				SetTexture(material, "_BottomBump", variant.bottomBump, variant.mainScale);
				SetTexture(material, "_LeftBump", variant.leftBump, variant.mainScale);
				SetTexture(material, "_RightBump", variant.rightBump, variant.mainScale);
				SetTexture(material, "_FrontBump", variant.frontBump, variant.mainScale);
				SetTexture(material, "_BackBump", variant.backBump, variant.mainScale);
			}
			texturesLabel = "Textures: " + variations[currentVariation].name;
		}

		void SetTexture(Material material, string textureName, Texture2D texture, Vector2 scale)
		{
			if (material.HasProperty(textureName))
			{
				material.SetTexture (textureName, texture);
				material.SetTextureScale (textureName, scale);
			}
		}

	}

	[System.Serializable]
	public class MaterialVariant
	{
		public string name;
		public Texture2D mainTex;
		
		public Texture2D topTex;
		public Texture2D bottomTex;
		
		public Texture2D rightTex;
		public Texture2D leftTex;
		
		public Texture2D frontTex;
		public Texture2D backTex;
		
		public Texture2D bump;
		
		public Texture2D topBump;
		public Texture2D bottomBump;
		
		public Texture2D rightBump;
		public Texture2D leftBump;
		
		public Texture2D frontBump;
		public Texture2D backBump;
		
		public Vector2 mainScale = Vector2.one;
		public Vector2 topScale = Vector2.one;
	}
}