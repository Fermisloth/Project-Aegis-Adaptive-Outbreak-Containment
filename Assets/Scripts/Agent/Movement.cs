using UnityEngine;

public class Movement : MonoBehaviour
{
    public float baseSpeed = 2f;
    public float currentSpeed;
    public float wanderRadius = 45f;
    public bool isLockedDown = false;

    private Vector3 targetPosition;
    private float updateTimer = 0f;

    private Rigidbody rb;
    private bool moveXAxis = true;

    void Start()
    {
        currentSpeed = baseSpeed;
        rb = GetComponent<Rigidbody>();
        if(rb != null) Destroy(rb); // remove rb to prevent pushing each other
        
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true; // let pass through each other

        SetNewTarget();
    }

    void FixedUpdate()
    {
        if (isLockedDown) return; // Completely halt movement when locked down
        
        Vector3 currentPos = transform.position;
        Vector3 dir = Vector3.zero;

        // Linear grid-like organized movement
        if (moveXAxis)
        {
            if (Mathf.Abs(currentPos.x - targetPosition.x) > 0.2f)
                dir = new Vector3(Mathf.Sign(targetPosition.x - currentPos.x), 0, 0);
            else
                moveXAxis = false;
        }
        else
        {
            if (Mathf.Abs(currentPos.z - targetPosition.z) > 0.2f)
                dir = new Vector3(0, 0, Mathf.Sign(targetPosition.z - currentPos.z));
            else
            {
                updateTimer += Time.fixedDeltaTime;
                if (updateTimer > 1f)
                {
                    SetNewTarget();
                    updateTimer = 0f;
                }
                return;
            }
        }

        transform.position += dir * currentSpeed * Time.fixedDeltaTime;
    }

    private void SetNewTarget()
    {
        if (CityMapGenerator.BuildingPositions != null && CityMapGenerator.BuildingPositions.Count > 0)
        {
            targetPosition = CityMapGenerator.BuildingPositions[Random.Range(0, CityMapGenerator.BuildingPositions.Count)];
        }
        else
        {
            float rx = Random.Range(-wanderRadius, wanderRadius);
            float rz = Random.Range(-wanderRadius, wanderRadius);
            targetPosition = new Vector3(rx, transform.position.y, rz);
        }
        
        // Randomize whether they trace X or Z first to simulate different streets
        moveXAxis = Random.value > 0.5f; 
    }

    public void SetLockdown(bool lockdownStatus)
    {
        isLockedDown = lockdownStatus;
        currentSpeed = lockdownStatus ? 0.2f : baseSpeed; // Minimal speed when locked down
    }
}
