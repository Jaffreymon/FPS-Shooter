using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
public class PlayerSetup : NetworkBehaviour {

	// List of components to disable for spawned players
	[SerializeField]
	Behaviour[] componentsToDisable;

	[SerializeField]
	string remoteLayerName = "Remote Player";

	[SerializeField]
	GameObject playerUIPrefab;
	private GameObject playerUIInstance;

	Camera sceneCamera;

	// Use this for initialization
	void Start () {
		// Only non-local players loss control of other player objects
		if (!isLocalPlayer) {
			DisableComponents ();
			AssignRemotePlayer ();
		} 
		// Sets main camera of scene false when local player joins
		else {
			sceneCamera = Camera.main;
			if (sceneCamera != null) {
				sceneCamera.gameObject.SetActive (false);
			}

			// Create PlayerUI Image
			playerUIInstance = Instantiate(playerUIPrefab);
			playerUIInstance.name = playerUIPrefab.name;

			// Configure PlayerUI
			PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();
			if (ui == null) {
				Debug.LogError ("No PlayerUI component on PlayerUI Prefab");
			}
			ui.SetController(GetComponent<Player_Controller>());
			ui.SetPlayer (GetComponent<Player>());
		}
 		GetComponent<Player>().Setup();
	}

	public override void OnStartClient(){
		base.OnStartClient ();

		string _netID = GetComponent<NetworkIdentity>().netId.ToString();
		Player _player = GetComponent<Player> ();

		GameManager.RegisterPlayer (_netID, _player);
	}

	void AssignRemotePlayer() {
		//gameObject.layer = LayerMask.NameToLayer (remoteLayerName);
		Util.SetLayerRecursive (this.gameObject, LayerMask.NameToLayer(remoteLayerName));

	}

	void DisableComponents() {
		for (int i = 0; i < componentsToDisable.Length; i++) {
			componentsToDisable [i].enabled = false;
		}
	}

	void OnDisable() {
		Destroy(playerUIInstance);	

		if (sceneCamera != null) {
			sceneCamera.gameObject.SetActive (true);
		}

		GameManager.DeRegisteredPlayer (transform.name);
	}
}
