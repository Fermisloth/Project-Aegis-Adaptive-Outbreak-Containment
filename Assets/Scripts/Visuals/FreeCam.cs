using UnityEngine;
using UnityEngine.InputSystem;

public class FreeCam : MonoBehaviour
{
    public float moveSpeed = 15f;
    public float lookSpeed = 0.5f; // reduced look speed for new input system since delta is larger
    
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
        if (Mouse.current == null || Keyboard.current == null) return;

        // Look around with Right Mouse Button
        if (Mouse.current.rightButton.isPressed)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            rotationY += mouseDelta.x * lookSpeed;
            rotationX -= mouseDelta.y * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -90f, 90f);
            
            transform.localRotation = Quaternion.Euler(rotationX, rotationY, 0);
        }

        // Movement with WASD
        float moveX = 0f;
        float moveZ = 0f;
        float moveY = 0f;

        if (Keyboard.current.wKey.isPressed) moveZ += 1f;
        if (Keyboard.current.sKey.isPressed) moveZ -= 1f;
        if (Keyboard.current.dKey.isPressed) moveX += 1f;
        if (Keyboard.current.aKey.isPressed) moveX -= 1f;
        
        if (Keyboard.current.eKey.isPressed) moveY = 1f;
        if (Keyboard.current.qKey.isPressed) moveY = -1f;

        Vector3 move = transform.right * moveX + transform.forward * moveZ + transform.up * moveY;
        transform.position += move.normalized * moveSpeed * Time.deltaTime;
    }
}
