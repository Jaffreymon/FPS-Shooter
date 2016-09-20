using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

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

	[HideInInspector]
	public GameObject playerUIInstance;

	// Use this for initialization
	void Start () {
		//TODO enable cursor visibility when testing
		//Cursor.visible = false;

		// Only non-local players loss control of other player objects
		if (!isLocalPlayer) {
			DisableComponents ();
			AssignRemotePlayer ();
		} 
		// Setup player in game
		else {
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

		GameManager.instance.SetSceneCameraActive (true);

		GameManager.DeRegisteredPlayer (transform.name);
	}
}
