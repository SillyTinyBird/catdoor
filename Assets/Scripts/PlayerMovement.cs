using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed= 5f;
    public Transform cam;
    public float rayDistance = 10f;
    public float floorDist;
    public float delta;
    public Transform GroundParent;
    public LayerMask GroundLayers;
    private Rigidbody rb;
    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        Physics.Raycast(GroundParent.GetChild(0).position, -GroundParent.GetChild(0).transform.up, out RaycastHit HitCentre, rayDistance, GroundLayers);
        Vector3 floor = FloorAngleCheck();//geting flor mormal vector info


        Vector3 viewDirection = cam.position - transform.position;
        viewDirection = Vector3.Scale(viewDirection, new Vector3(-1, -1, -1));
        viewDirection = Vector3.ProjectOnPlane(viewDirection, floor);
        Vector3 MoveSum = transform.position;

        //rb.MovePosition(HitCentre.point + floor * floorDist);


        if(Input.GetAxis("Vertical") > 0)
        {
            MoveSum += Vector3.Lerp(transform.position, transform.position + movementSpeed * Time.fixedDeltaTime * viewDirection.normalized, Input.GetAxis("Vertical")) - transform.position;//input on t alows smooth speed up and fade out

            rb.MoveRotation(Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(viewDirection.normalized, floor),
                smoothTime));
            
        }
        else
        {
            rb.MoveRotation(Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward,floor), floor), smoothTime));

            //AVOID SHARP CORNERS!!!
        }
        
        MoveSum += Vector3.SmoothDamp(transform.position, HitCentre.point + floor * (floorDist + 0.1f), ref velocity, smoothTime) - transform.position;
        Debug.DrawLine(transform.position, MoveSum, Color.magenta, 0, false);
        rb.MovePosition(MoveSum);
    }
    Vector3 FloorAngleCheck()
    {
        Vector3 HitDir = transform.up;
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
        }
        Debug.DrawLine(transform.position, transform.position + (HitDir.normalized * 5f), Color.cyan);
        return HitDir.normalized;
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
