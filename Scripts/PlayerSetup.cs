using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour {

	// List of components to disable for spawned players
	[SerializeField]
	Behaviour[] componentsToDisable;

	[SerializeField]
	string remoteLayerName = "Remote Player";

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
		}
		RegisterPlayer ();
	}

	void RegisterPlayer() {
		string _ID = "Player " + GetComponent<NetworkIdentity> ().netId;
		transform.name = _ID;
	}

	void AssignRemotePlayer() {
		gameObject.layer = LayerMask.NameToLayer (remoteLayerName);
	}

	void DisableComponents() {
		for (int i = 0; i < componentsToDisable.Length; i++) {
			componentsToDisable [i].enabled = false;
		}
	}

	void OnDisable() {
		if (sceneCamera != null) {
			sceneCamera.gameObject.SetActive (true);
		}
	}
}
