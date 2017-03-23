using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour {
	[SyncVar]
	private bool _isDead = false;

	public bool isDead  {
		get { return _isDead; }
		protected set{ _isDead = value; }
	}

	[SerializeField]
	private int maxHealth = 100;

	[SyncVar]
	private int currHealth;

	[SerializeField]
	private Behaviour[] disableOnDeath;
	private bool[] wasEnable;

	[SerializeField]
	private GameObject[] disableGameObjectsOnDeath;

	[SerializeField]
	private GameObject deathExplosion;
	[SerializeField]
	private AudioClip explosionSound;

	public int getHealth() {
		return currHealth;
	}

	public void Setup () {
		wasEnable = new bool[disableOnDeath.Length];

		for (int i = 0; i < wasEnable.Length; i++) {
			wasEnable [i] = disableOnDeath[i].enabled;
		}

		SetDefaults ();
	}

	void Update() {
		if (!isLocalPlayer)
			return;

		if (Input.GetKeyDown (KeyCode.K))
			RpcTakeDamage (20);
	}

	[ClientRpc]
	public void RpcTakeDamage(int _amount) {
		if (_isDead)
			return;

		currHealth -= _amount;

		Debug.Log (transform.name + " has " + currHealth);

		if (currHealth <= 0) {
			Die ();
		}
	}

	private void Die() {
		isDead = true;

		// Disable player controls on death
		for (int i = 0; i < disableOnDeath.Length; i++) {
			disableOnDeath [i].enabled = false;
		}

		// Disable player visuals on death
		for (int i = 0; i < disableGameObjectsOnDeath.Length; i++) {
			disableGameObjectsOnDeath[i].SetActive(false);
		}
	
		// Disable player collider on death
		Collider _col = GetComponent<Collider> ();
		if (_col != null) {
			_col.enabled = true;
		}

		// Player's camera transitions to main scene camera on death
		if (isLocalPlayer) {
			GameManager.instance.SetSceneCameraActive (true);
			GetComponent<PlayerSetup> ().playerUIInstance.SetActive (false);
		}


		GameObject explosionFX = (GameObject) Instantiate (deathExplosion, transform.position, Quaternion.identity);
		AudioSource.PlayClipAtPoint (explosionSound, transform.position);
		Destroy (explosionFX, 3f);

		Debug.Log (transform.name + " WASTED");

		// Call Respawn Methods
		StartCoroutine(Respawn());
	}

	private IEnumerator Respawn () {
		yield return new WaitForSeconds (GameManager.instance.matchingSettings.respawnTime);
		SetDefaults ();
		Transform _spawnPoint = NetworkManager.singleton.GetStartPosition ();
		transform.position = _spawnPoint.position;
		transform.rotation = _spawnPoint.rotation;

		Debug.Log (transform.name + " respawned");
	}

	public void SetDefaults() {
		isDead = false;

		currHealth = maxHealth;

		// Enables player controls
		for (int i = 0; i < disableOnDeath.Length; i++) {
			disableOnDeath [i].enabled = wasEnable [i];
		}

		// Enables player collider
		Collider _col = GetComponent<Collider> ();
		if (_col != null) {
			_col.enabled = true;
		}

		// Enables player visuals in game
		for (int i = 0; i < disableGameObjectsOnDeath.Length; i++) {
			disableGameObjectsOnDeath[i].SetActive(true);
		}

		// Player's camera transitions to game on respawn
		if (isLocalPlayer) {
			GameManager.instance.SetSceneCameraActive (false);
			GetComponent<PlayerSetup> ().playerUIInstance.SetActive (true);
		}

		//Resets player's ammo count on death
		GetComponent<WeaponManager>().getCurrWeapon().clipSize = GetComponent<WeaponManager>().getCurrWeapon().getMaxClipSize();
	}
}
