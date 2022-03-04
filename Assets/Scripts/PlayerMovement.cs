using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 5f;
    public Transform cam;
    public float rayDistance = 10f;
    public float floorDist;
    public Transform GroundParent;
    public LayerMask GroundLayers;
    [HideInInspector]
    public Rigidbody rb;
    public float smoothTime = 0.3F;
    public Vector3 velocity = Vector3.zero;
    public Vector3 previousUp;
    public float fallSpeed = 0.5f;
    public float fallSpeedMovement = 1f;
    [HideInInspector]
    public float currentVelocity = 0.0f;


    //state machine
    private MovementBase currentState;
    public StateOnGround stateGround = new StateOnGround();
    public StateOnEdge stateEdge = new StateOnEdge();
    public StateInAir stateAir = new StateInAir();
    public StateBumped stateBumped = new StateBumped();

    private Vector3 bumpOrigin;
    public Transform bump;
    public bool bumped = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        previousUp = transform.up;
        currentState = stateGround;
        bumpOrigin = bump.localPosition;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        currentState.UpdateState(this);

        Debug.Log(currentState);
    }
    public void BumpMove() => bump.localPosition = bumpOrigin + (Vector3.right * 0.1f * RangeInt(Input.GetAxis("Horizontal"))) + (Vector3.forward * 0.1f * RangeInt(Input.GetAxis("Vertical")));
    private int RangeInt(float input)
    {
        if (input == 0) return 0;
        if (input > 0) return 1;
        if (input < 0) return -1;
        return 0;
    }
    private void OnTriggerEnter(Collider other) => bumped = true;
    private void OnTriggerExit(Collider other) => bumped = false;
    public void SwitchState(MovementBase state) => currentState = state;
    public void Jump()
    {
        currentVelocity = -6f;
        currentState = stateAir;
        currentState.UpdateState(this);
    }
    public Vector3 GetViewDirection(Vector3 floor)
    {
        Vector3 viewDirection = cam.position - transform.position;
        viewDirection = Vector3.Scale(viewDirection, new Vector3(-1, -1, -1));
        viewDirection = Vector3.ProjectOnPlane(viewDirection, floor);
        return viewDirection;
    }
    public bool FloorAngleCheck(out Vector3 floor, out float avgDistance)//returns false if not on floor
    {
        bool ret = true;
        float dist = 0;
        int miss = 0;
        Vector3 HitDir = Vector3.zero;
        for (int i = 0; i < GroundParent.childCount; i++)
        {
            Transform item = GroundParent.GetChild(i);
            Physics.Raycast(item.position, -item.transform.up, out RaycastHit Hit, rayDistance, GroundLayers);
            if (Hit.transform != null)
            {
                HitDir += Hit.normal;
                Debug.DrawLine(item.position, Hit.point);
                DrawSurface(Hit);
                dist += Hit.distance;
            }
            else
            {
                ret = false;
                miss++;
            }
        }
        Debug.DrawLine(transform.position, transform.position + (HitDir.normalized * 2f), Color.cyan);
        floor = HitDir.normalized;
        avgDistance = dist / GroundParent.childCount;
        if (miss > Mathf.CeilToInt(2.0f / 3.0f * GroundParent.childCount))
        {
            floor = Vector3.zero;
        }
        return ret;
    }
    void DrawSurface(RaycastHit hit)
    {
        MeshCollider meshCollider = hit.collider as MeshCollider;
        if (meshCollider == null || meshCollider.sharedMesh == null)
            return;

        Mesh mesh = meshCollider.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        Vector3 p0 = vertices[triangles[hit.triangleIndex * 3 + 0]];
        Vector3 p1 = vertices[triangles[hit.triangleIndex * 3 + 1]];
        Vector3 p2 = vertices[triangles[hit.triangleIndex * 3 + 2]];
        Transform hitTransform = hit.collider.transform;
        p0 = hitTransform.TransformPoint(p0);
        p1 = hitTransform.TransformPoint(p1);
        p2 = hitTransform.TransformPoint(p2);
        Debug.DrawLine(p0, p1, Color.yellow, 0, false);
        Debug.DrawLine(p1, p2, Color.yellow, 0, false);
        Debug.DrawLine(p2, p0, Color.yellow, 0, false);
    }
}
