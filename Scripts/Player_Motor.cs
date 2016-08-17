using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player_Motor : MonoBehaviour {

	[SerializeField]
	private Camera cam;

	private Vector3 velocity = Vector3.zero;
	private Vector3 rotation = Vector3.zero;
	private Vector3 camera_rotation = Vector3.zero;
	private Vector3 jump_Vector = Vector3.zero;

	private Rigidbody rb;

	// Obtains player model
	void Start() {
		rb = GetComponent<Rigidbody>();
	}

	// Gets a movement vector
	public void Move(Vector3 _velocity) {
		velocity = _velocity;
	}

	// Gets a rotation vector
	public void Rotate(Vector3 _rotation) {
		rotation = _rotation;
	}

	// Gets a camera rotation vector
	public void RotateCamera(Vector3 _camera_rotation) {
		camera_rotation = _camera_rotation;
	}

	public void JumpUp(Vector3 jumpHeight) {
		jump_Vector = jumpHeight;
	}

	// Run every physics iteration
	void FixedUpdate() {
		PerformMovement();
		PerformRotation ();
	}

	// Performs movement based on velocity vector
	void PerformMovement() {
		if (velocity != Vector3.zero) {
			rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
		}
		if(jump_Vector != Vector3.zero) {
			rb.AddForce (jump_Vector * Time.fixedDeltaTime);
		}
	}

	// Performs rotation based on rotation vector
	void PerformRotation() {
		rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));

		if (cam != null) {
			cam.transform.Rotate(-camera_rotation);
		}
	}
}
