using UnityEngine;

public class FreeCam : MonoBehaviour
{
    public float moveSpeed = 15f;
    public float lookSpeed = 2f;
    
    private float rotationX = 0f;
    private float rotationY = 0f;
    
    void Start()
    {
        // Force the camera background to stable Solid Color 
        Camera.main.clearFlags = CameraClearFlags.SolidColor;
        Camera.main.backgroundColor = new Color(0.1f, 0.1f, 0.15f); // Dark stable hue
        Camera.main.orthographic = false;

        // Set optimal view for new large layout
        transform.position = new Vector3(0, 95f, -80f);
        transform.localRotation = Quaternion.Euler(50f, 0f, 0f);

        Vector3 rot = transform.localRotation.eulerAngles;
        rotationY = rot.y;
        rotationX = rot.x;
    }

    void Update()
    {
        // Look around with Right Mouse Button
        if (Input.GetMouseButton(1))
        {
            rotationY += Input.GetAxis("Mouse X") * lookSpeed;
            rotationX -= Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -90f, 90f);
            
            transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0);
        }

        // Movement with WASD
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        float moveY = 0f;
        
        if (Input.GetKey(KeyCode.E)) moveY = 1f;
        if (Input.GetKey(KeyCode.Q)) moveY = -1f;

        Vector3 move = transform.right * moveX + transform.forward * moveZ + transform.up * moveY;
        transform.position += move * moveSpeed * Time.deltaTime;
    }
}
