using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed = 5f;
    public Transform cam;
    public float rayDistance = 10f;
    public float floorDist;
    public float delta;
    public Transform GroundParent;
    public LayerMask GroundLayers;
    public Rigidbody rb;
    public float smoothTime = 0.3F;
    public Vector3 velocity = Vector3.zero;
    public Vector3 previousUp;
    public float fallSpeed = 0.5f;
    public float fallSpeedMovement = 1f;
    public float currentVelocity = 1f;
    //state machine
    private MovementBase currentState;
    public StateOnGround stateGround = new StateOnGround();
    public StateOnEdge stateEdge = new StateOnEdge();
    public StateInAir stateAir = new StateInAir();

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        previousUp = transform.up;
        currentState = stateGround;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //rb.MovePosition(HitCentre.point + floor * floorDist);
        currentState.UpdateState(this);
        /*
        if (notHits)
        {
            //move up-down
            MoveSum += Vector3.SmoothDamp(transform.position, HitCentre.point + floor * floorDist, ref velocity, smoothTime) - transform.position;
            if (Input.GetAxis("Vertical") != 0)
            {
                MoveSum += Vector3.Lerp(transform.position, transform.position + movementSpeed * Time.fixedDeltaTime * viewDirection.normalized * Input.GetAxis("Vertical") + movementSpeed * Time.fixedDeltaTime * transform.right * Input.GetAxis("Horizontal"), Mathf.Abs(Input.GetAxis("Vertical"))) - transform.position;//input on t alows smooth speed up and fade out
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(viewDirection.normalized, floor), smoothTime));
                //other movement needs to be put in one line, in (Input.GetAxis("Vertical") > 0) shoud stay only rotation
            }
            else
            {
                MoveSum += Vector3.Lerp(transform.position, transform.position + movementSpeed * Time.fixedDeltaTime * transform.right * Input.GetAxis("Horizontal"), 0.3f) - transform.position;
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, floor), floor), smoothTime));
                //ANY OTHER MOVEMENT
            }
            previousUp = floor;
        }
        else
        {
            if (floor != Vector3.zero)
            {
                //no need to move up/down and rotation
                if (Input.GetAxis("Vertical") != 0)
                {
                    MoveSum += Vector3.Lerp(transform.position, transform.position + movementSpeed * Time.fixedDeltaTime * viewDirection.normalized * Input.GetAxis("Vertical"), Mathf.Abs(Input.GetAxis("Vertical"))) - transform.position;//input on t alows smooth speed up and fade out
                    rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(viewDirection.normalized, previousUp), smoothTime));
                    //other movement needs to be put in one line, in (Input.GetAxis("Vertical") > 0) shoud stay only rotation
                }
                else
                {
                    MoveSum += Vector3.Lerp(transform.position, transform.position + movementSpeed * Time.fixedDeltaTime * transform.right * Input.GetAxis("Horizontal"), 0.3f) - transform.position;
                    //rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, previousUp), previousUp), smoothTime));
                    //ANY OTHER MOVEMENT
                }
            }
            else
            {
                //idk add here coroutine of falling or something idk
                MoveSum += Vector3.Lerp(transform.position, transform.position + fallSpeed * Time.fixedDeltaTime * Vector3.Scale(transform.up,new Vector3(-1,-1,-1)) + fallSpeedMovement * Time.fixedDeltaTime * transform.right * Input.GetAxis("Horizontal"), 0.5f) - transform.position;
            }

        }

        //3,5,9,11
        Debug.DrawLine(transform.position, MoveSum, Color.magenta, 0, false);
        rb.MovePosition(MoveSum);*/
        Debug.Log(currentState);
    }
    public void SwitchState(MovementBase state)
    {
        currentState = state;
    }
    public Vector3 GetViewDirection(Vector3 floor)
    {
        Vector3 viewDirection = cam.position - transform.position;
        viewDirection = Vector3.Scale(viewDirection, new Vector3(-1, -1, -1));
        viewDirection = Vector3.ProjectOnPlane(viewDirection, floor);
        return viewDirection;
    }
    bool onTheGround()
    {
        int[] corners = { 3, 5, 9, 11 };
        foreach (int i in corners)
        {
            Transform item = GroundParent.GetChild(i);
            if(!Physics.Raycast(item.position, -transform.up, out _, floorDist+0.1f, GroundLayers))
            {
                return false;
            }
        }
        return true;
    }
    public bool FloorAngleCheck(out Vector3 floor)//returns false if not on floor
    {
        bool ret = true;
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
            }
            else
            {
                ret = false;
            }
        }
        Debug.DrawLine(transform.position, transform.position + (HitDir.normalized * 2f), Color.cyan);
        floor =  HitDir.normalized;
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
