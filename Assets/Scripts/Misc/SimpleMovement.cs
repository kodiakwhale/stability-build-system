using UnityEngine;

public class SimpleMovement : MonoBehaviour {

    public float speed;
	public float gravity;
	public float jumpVelocity;
    public float sensitivity;

    public Transform playerView;
	CharacterController controller;

	private Vector3 vel;
    private bool canMove = true;

	private float rotX = 0.0f;
	private float rotY = 0.0f;

    void Start () {
        controller = GetComponent<CharacterController>();
		Cursor.lockState = CursorLockMode.Locked;
    }

    void Update () {
        if (!canMove) {
			return;
		}
		if (controller.isGrounded) {
			GroundMove();
			if (Input.GetButton("Jump")) {
				vel.y = jumpVelocity;
			}
		} else {
			AirMove();
		}

		RotateCamera();
		controller.Move(vel * Time.deltaTime);
    }

    void GroundMove () {
		float x = Input.GetAxisRaw("Horizontal");
		float y = Input.GetAxisRaw("Vertical");
		Vector3 temp = (transform.forward * y + transform.right * x).normalized * speed;

		vel.x = temp.x;
		vel.y = 0;
		vel.z = temp.z;
	}

	void AirMove () {
		float x = Input.GetAxisRaw("Horizontal");
		float y = Input.GetAxisRaw("Vertical");
		Vector3 temp = (transform.forward * y + transform.right * x).normalized * speed;

		vel.x = temp.x;
		vel.y -= gravity * Time.deltaTime;
		vel.z = temp.z;
	}

    public void RotateCamera () {
		rotX -= Input.GetAxisRaw("Mouse Y") * sensitivity;
		rotY += Input.GetAxisRaw("Mouse X") * sensitivity;

		//clamp x rotation
		if (rotX < -90)
			rotX = -90;
		else if (rotX > 90)
			rotX = 90;

		transform.rotation = Quaternion.Euler(0, rotY, 0);
		playerView.rotation = Quaternion.Euler(rotX, rotY, 0);
	}

}
