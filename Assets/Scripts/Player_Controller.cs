using UnityEngine;

[RequireComponent(typeof(Player_Motor))]

public class Player_Controller : MonoBehaviour {

	private const float walking = 3f;
	private const float running = 5f;
	private float speed = walking;

	[SerializeField]
	private float lookSensitivity = 4f;

	[SerializeField]
	private float height = 3.2f;

	[SerializeField]
	private float stamina = 1f;
	[SerializeField]
	private float staminaUse = 0.5f;
	[SerializeField]
	private float staminaRegen = 0.3f;

	// Checks if player is on the ground
	public Transform ground;
	public float groundRadius;
	public LayerMask whatIsGround;
	private bool grounded;
	public bool isRunning = false;


	private Player_Motor motor;

	public float getStamina() {
		return stamina;
	}

	void Start() {
		motor = GetComponent<Player_Motor> ();
	}

	void FixedUpdate() {
		grounded = Physics.OverlapSphere (ground.position, groundRadius, whatIsGround).Length != 0;
	}

	void Update() {
		// Calculate movement velocity as a 3D vector
		float _xMovement = Input.GetAxisRaw("Horizontal");
		float _zMovement = Input.GetAxisRaw ("Vertical");

		// Get the project vector toward direction of movement
		Vector3 _movHorizontal = transform.right * _xMovement;
		Vector3 _movVertical = transform.forward * _zMovement;

		// Final projected vector of movement with a given player speed
		Vector3 _velocity = (_movHorizontal + _movVertical).normalized * speed;

		// Applies vector to movement
		motor.Move(_velocity);

		// Calculate rotation as 3D Vector for turning player 
		float _yRot = Input.GetAxisRaw("Mouse X");
		Vector3 _rotation = new Vector3 (0, _yRot, 0) * lookSensitivity;

		// Apply rotation
		motor.Rotate(_rotation);

		// Calculate camera rotation as 3D Vector
		float _xRot = Input.GetAxisRaw("Mouse Y");
		Vector3 _camera_rotation = new Vector3 (_xRot, 0 , 0) * lookSensitivity;

		// Apply camera rotation
		motor.RotateCamera(_camera_rotation);

		// Controls Player Jumping Control
		if(Input.GetKeyDown(KeyCode.Space) && grounded) {
			GetComponent<Rigidbody> ().velocity = new Vector3 (0, height, 0);
		}

		// Handles Player Sprinting
		if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W) && stamina > 0) {
			isRunning = true;
			stamina -= staminaUse * Time.deltaTime;
			if (stamina >= 0.1f) {
				speed = running;
			}
		}
		else {
			stamina += staminaRegen* Time.deltaTime;
			speed = walking;
			isRunning = false;
		}
		stamina = Mathf.Clamp (stamina, 0f, 1f);
	}
}
