using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {
	
	[SerializeField]
	private RectTransform staminaFill;

	[SerializeField]
	private Player_Controller controller;

	private WeaponManager weaponHandler;

	[SerializeField]
	private Text ammoCount;

	public void SetController(Player_Controller _controller) {
		controller = _controller;
	}

	// Links UIPrefab with in-scene player
	public void SetPlayer(Player _player) {
		weaponHandler = _player.GetComponent<WeaponManager>();
	}

	void SetStamina(float _amount) {
		staminaFill.localScale = new Vector3(1f, _amount, 1f);
	}

	public void setAmmoCount (int _totAmmo) {
		ammoCount.text = "Ammo: " + _totAmmo;
	}

	void Update() {
		SetStamina(controller.getStamina ());
		setAmmoCount(weaponHandler.getCurrWeapon().clipSize);
	}
		
}
