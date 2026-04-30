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
    
    private HealthState healthState;
    public bool isQuarantinedInBuilding = false;

    private Animator animator;

    void Start()
    {
        currentSpeed = baseSpeed;
        rb = GetComponent<Rigidbody>();
        if(rb != null) Destroy(rb); // remove rb to prevent pushing each other
        
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true; // let pass through each other

        healthState = GetComponent<HealthState>();
        animator = GetComponentInChildren<Animator>();

        if (animator != null)
        {
            animator.SetTrigger("walk");
        }

        SetNewTarget();
    }

    void FixedUpdate()
    {
        if (healthState != null && healthState.CurrentState == InfectionState.Dead) 
        {
            if (animator != null && !animator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
            {
                animator.SetTrigger("idle");
            }
            return;
        }

        if (isLockedDown) 
        {
            if (animator != null && !animator.GetCurrentAnimatorStateInfo(0).IsName("idle"))
            {
                animator.SetTrigger("idle");
            }
            return; 
        }
        else
        {
            if (animator != null && !animator.GetCurrentAnimatorStateInfo(0).IsName("walk"))
            {
                animator.SetTrigger("walk");
            }
        }
        
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
                    if (!isQuarantinedInBuilding) SetNewTarget();
                    updateTimer = 0f;
                }
                return;
            }
        }

        if (dir != Vector3.zero)
        {
            transform.position += dir * currentSpeed * Time.fixedDeltaTime;
            
            // Rotate agent to face movement direction smoothly
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 15f);
        }
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

    public void SendToBuilding()
    {
        if (CityMapGenerator.BuildingPositions != null && CityMapGenerator.BuildingPositions.Count > 0)
        {
            targetPosition = CityMapGenerator.BuildingPositions[Random.Range(0, CityMapGenerator.BuildingPositions.Count)];
            isQuarantinedInBuilding = true;
            moveXAxis = Random.value > 0.5f; 
        }
    }

    public void ReleaseFromBuilding()
    {
        isQuarantinedInBuilding = false;
        SetNewTarget();
    }
}
