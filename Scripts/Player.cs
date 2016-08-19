using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {
	
	[SerializeField]
	private int maxHealth = 100;

	[SyncVar]
	private int currHealth;

	void Awake() {
		SetDefaults ();
	}

	public void TakeDamage(int _amount) {
		currHealth -= _amount;

		Debug.Log (transform.name + " has " + currHealth);
	}

	public void SetDefaults() {
		currHealth = maxHealth;
	}
}
