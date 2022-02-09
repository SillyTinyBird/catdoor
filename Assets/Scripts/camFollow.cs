using UnityEngine;
public class camFollow : MonoBehaviour
{
    [Range(0.05f, 10.0f)]
    public float mouseSensitivity;
    public Transform anchor;
    [Range(0.05f, 1.0f)]
    public float smoothTime = 0.3F;
    public float magnitude;
    public LayerMask GroundLayers;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        //rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        inputToPosition();
    }
    void inputToPosition()
    {
        Vector3 direction = Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * mouseSensitivity,
        Vector3.Cross(transform.position - anchor.position, anchor.up))
        * Quaternion.AngleAxis(Input.GetAxis("Mouse X") * mouseSensitivity, anchor.up)
        * Quaternion.LookRotation(transform.position - anchor.position, anchor.up) * Vector3.forward;
        RaycastHit Hit;
        
        if(Physics.Raycast(anchor.position,direction, out Hit, magnitude + 0.1f, GroundLayers))
        {
            transform.position = anchor.position + direction * (Vector3.Distance(anchor.position,Hit.point)-0.3f);
        }
        else
        {
            transform.position = anchor.position + direction * magnitude;
        }
        transform.LookAt(anchor, anchor.up);
    }
}
