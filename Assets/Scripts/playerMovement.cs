using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public float movementSpeed= 5f;
    public Transform cam;
    public float rayDistance = 10f;
    public float floorDist;
    public float delta;
    public Transform[] GroundChecks;
    public Transform GroundParent;
    public LayerMask GroundLayers;
    private Rigidbody rb;
    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;
    //private camFollow cameraController;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit HitCentre;
        Physics.Raycast(GroundChecks[0].position, -GroundChecks[0].transform.up, out HitCentre, rayDistance, GroundLayers);
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
            //also, avoid angles on which camera cant keep up (controll looses(since contorlls are dependant on camera position))
        }
        MoveSum += Vector3.SmoothDamp(transform.position, HitCentre.point + floor * (floorDist + 0.1f), ref velocity, smoothTime) - transform.position;
        Debug.DrawLine(transform.position, MoveSum, Color.magenta, 0, false);
        rb.MovePosition(MoveSum);

    }
    void DrawDebug(Vector3 viewDirection, Vector3 direction)
    {
        Debug.DrawLine(transform.position, transform.position + cam.position - transform.position, Color.magenta, 0.0f, false);
        DrawPlane(transform.position, transform.up);
        Debug.DrawLine(transform.position, transform.position + viewDirection.normalized, Color.blue, 0.0f, false);
        Debug.DrawLine(transform.position, transform.position + direction, Color.white, 0.0f, false);
    }
    void DrawPlane(Vector3 position, Vector3 normal)
    {

        Vector3 v3;

        if (normal.normalized != Vector3.forward)
        {
            v3 = Vector3.Cross(normal, Vector3.forward).normalized * normal.magnitude;
        }
        else
        {
            v3 = Vector3.Cross(normal, Vector3.up).normalized * normal.magnitude;
        }

        var corner0 = position + v3;
        var corner2 = position - v3;
        var q = Quaternion.AngleAxis(90.0f, normal);
        v3 = q * v3;
        var corner1 = position + v3;
        var corner3 = position - v3;

        Debug.DrawLine(corner0, corner2, Color.green);
        Debug.DrawLine(corner1, corner3, Color.green);
        Debug.DrawLine(corner0, corner1, Color.green);
        Debug.DrawLine(corner1, corner2, Color.green);
        Debug.DrawLine(corner2, corner3, Color.green);
        Debug.DrawLine(corner3, corner0, Color.green);
        Debug.DrawRay(position, normal, Color.red);
    }
    Vector3 FloorAngleCheck()
    {
        Vector3 HitDir = transform.up;
        for (int i = 0; i < GroundParent.childCount; i++)
        {
            Transform item = GroundParent.GetChild(i);
            RaycastHit Hit;
            Physics.Raycast(item.position, -item.transform.up, out Hit, rayDistance, GroundLayers);
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
