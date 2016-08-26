using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {
	
	[SerializeField]
	private RectTransform staminaFill;

	[SerializeField]
	private Text healthCount;

	[SerializeField]
	private Player_Controller controller;

	private WeaponManager weaponHandler;
	private Player playerHandler;

	[SerializeField]
	private Text ammoCount;

	public void SetController(Player_Controller _controller) {
		controller = _controller;
	}

	// Links UIPrefab with in-scene player
	public void SetPlayer(Player _player) {
		playerHandler = _player;
		weaponHandler = _player.GetComponent<WeaponManager>();
	}

	void SetStamina(float _amount) {
		staminaFill.localScale = new Vector3(1f, _amount, 1f);
	}

	public void SetAmmoCount (int _totAmmo) {
		ammoCount.text = "Ammo: " + _totAmmo;
	}

	public void SetHealth(int _health) {
		healthCount.text = "Health: " + _health;
	}

	void Update() {
		SetStamina(controller.getStamina ());
		SetAmmoCount(weaponHandler.getCurrWeapon().clipSize);
		SetHealth (playerHandler.getHealth ());
	}
		
}
