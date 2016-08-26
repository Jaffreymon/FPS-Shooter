using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

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

		if (Input.GetKeyDown (KeyCode.K)) {
			RpcTakeDamage (20);
		}
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

		for (int i = 0; i < disableOnDeath.Length; i++) {
			disableOnDeath [i].enabled = false;
		}
	
		Collider _col = GetComponent<Collider> ();
		if (_col != null) {
			_col.enabled = true;
		}

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

		for (int i = 0; i < disableOnDeath.Length; i++) {
			disableOnDeath [i].enabled = wasEnable [i];
		}

		Collider _col = GetComponent<Collider> ();
		if (_col != null) {
			_col.enabled = true;
		}
	}
}
