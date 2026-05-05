using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Rotates the camera around the world origin (0, 0, 0).
/// Attach this script to your Camera GameObject.
/// Default starting position: (0, 0, -10)
/// </summary>
public class CameraOrbit : MonoBehaviour
{
	[Header("Orbit Settings")]
	[Tooltip("Horizontal rotation speed in degrees per second (auto-rotate) or degrees per pixel (mouse drag).")]
	public float rotationSpeedX = 50f;

	[Tooltip("Vertical rotation speed in degrees per second (auto-rotate) or degrees per pixel (mouse drag).")]
	public float rotationSpeedY = 30f;

	[Tooltip("Automatically rotate without input.")]
	public bool autoRotate = true;

	[Header("Mouse Drag")]
	[Tooltip("Allow click-and-drag to orbit manually.")]
	public bool enableMouseDrag = true;
	
	public float mouseSensitivity = 0.3f;

	[Header("Zoom")]
	[Tooltip("Allow scroll-wheel zoom.")]
	public bool enableZoom = true;

	public float zoomSpeed = 2f;
	public float minDistance = 2f;
	public float maxDistance = 50f;

	[Header("Vertical Clamp")]
	[Tooltip("Limit how high/low the camera can orbit (degrees).")]
	public float minVerticalAngle = -80f;
	public float maxVerticalAngle = 80f;

	// ── Internal state ─────────────────────────────────────────────────────────
	private float currentYaw = 0f;    // horizontal angle around Y axis
	private float currentPitch = 0f;    // vertical angle around X axis
	private float currentDistance;

	private Vector3 target = Vector3.zero;  // always the origin

	private Vector3 lastMousePosition;

	// ───────────────────────────────────────────────────────────────────────────
	void Start()
	{
		// Initialise distance from the camera's starting position
		currentDistance = Vector3.Distance(transform.position, target);

		// Derive initial yaw/pitch from the starting transform so the camera
		// snaps smoothly into its orbit without jumping.
		Vector3 offset = transform.position - target;
		currentYaw = Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg;
		currentPitch = Mathf.Asin(offset.y / currentDistance) * Mathf.Rad2Deg;
	}

	// ───────────────────────────────────────────────────────────────────────────
	void Update()
	{
		HandleMouseDrag();
		HandleZoom();

		if (autoRotate)
		{
			currentYaw += rotationSpeedX * Time.deltaTime;
		}

		// Clamp vertical angle
		currentPitch = Mathf.Clamp(currentPitch, minVerticalAngle, maxVerticalAngle);

		// Convert spherical → Cartesian and apply
		transform.position = SphericalToCartesian(currentYaw, currentPitch, currentDistance);
		transform.LookAt(target);
	}

	// ───────────────────────────────────────────────────────────────────────────
	private void HandleMouseDrag()
	{
		if (!enableMouseDrag) return;

		//if (Input.GetMouseButtonDown(0))
		//{
		//	lastMousePosition = Input.mousePosition;
		//}

		//if (Input.GetMouseButton(0))
		//{
		//	Vector3 delta = Input.mousePosition - lastMousePosition;
		//	lastMousePosition = Input.mousePosition;

		//	currentYaw += delta.x * mouseSensitivity * rotationSpeedX * Time.deltaTime;
		//	currentPitch -= delta.y * mouseSensitivity * rotationSpeedY * Time.deltaTime;
		//}
	}

	private void HandleZoom()
	{
		if (!enableZoom) return;

		//float scroll = Input.GetAxis("Mouse ScrollWheel");
		//if (Mathf.Abs(scroll) > 0.001f)
		//{
		//	currentDistance -= scroll * zoomSpeed;
		//	currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
		//}
	}

	// ───────────────────────────────────────────────────────────────────────────
	/// <summary>Converts spherical coordinates to a world-space Cartesian position.</summary>
	private Vector3 SphericalToCartesian(float yawDeg, float pitchDeg, float radius)
	{
		float yaw = yawDeg * Mathf.Deg2Rad;
		float pitch = pitchDeg * Mathf.Deg2Rad;

		float x = radius * Mathf.Cos(pitch) * Mathf.Sin(yaw);
		float y = radius * Mathf.Sin(pitch);
		float z = radius * Mathf.Cos(pitch) * Mathf.Cos(yaw);

		return target + new Vector3(x, y, z);
	}
}