using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float _movementSpeed = 5f;
    public Transform _cam;
    public float _rayDistance = 10f;
    public float _floorDist;
    public Transform _parentOfFloorRayOrigins;
    public LayerMask _groundLayerRaycast;
    public Rigidbody _playerRigidBody;
    public float _smoothTimeRotationDivision = 0.3F;
    public float _smoothTimeMoveDivision = 0.0F;
    [HideInInspector]
    public Vector3 _velocity = Vector3.zero;
    private Vector3 previousUpDirection;
    [HideInInspector]
    public float _currentFallVelocity = 0.0f;

    //state machine
    private MovementBase _currentState;
    public StateOnGround stateGround = new StateOnGround();
    public StateOnEdge stateEdge = new StateOnEdge();
    public StateInAir stateAir = new StateInAir();
    public StateBumped stateBumped = new StateBumped();
    
    private bool _bumped = false;

    private InputMap _inputMap;

    private void Awake() => _inputMap = new InputMap();
    private void OnEnable() => _inputMap.Enable();
    private void OnDisable() => _inputMap.Disable();
    void Start()
    {
        PreviousUpDirection = transform.up;
        _currentState = stateGround;
    }

    void FixedUpdate()
    {
        ChangeSmoothCoefficient();
        _currentState.UpdateState(this);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Walkable")) _bumped = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Walkable")) _bumped = false;
    }
    public Vector3 PreviousUpDirection { get => previousUpDirection; set => previousUpDirection = value; }
    public void SwitchState(MovementBase state)
    {
        _currentState = state;
        Debug.Log(_currentState);
    }
    private void ChangeSmoothCoefficient()
    {
        if (_inputMap.Player.Movement.ReadValue<Vector2>()!=Vector2.zero)
        {
            if (_smoothTimeMoveDivision < 1.0f)
            {
                _smoothTimeMoveDivision += 0.05f;
            }
        }
        else
        {
            if (_smoothTimeMoveDivision > 0.0f)
            {
                _smoothTimeMoveDivision -= 0.05f;
            }
        }
    }
    public void Jump()
    {
        _currentFallVelocity = -6f;
        SwitchState(stateAir);
        _currentState.UpdateState(this);
    }
    public Vector3 GetViewDirection(Vector3 floor)
    {
        Vector3 viewDirection = _cam.position - transform.position;
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
        for (int i = 0; i < _parentOfFloorRayOrigins.childCount; i++)
        {
            Transform item = _parentOfFloorRayOrigins.GetChild(i);
            Physics.Raycast(item.position, -item.transform.up, out RaycastHit Hit, _rayDistance, _groundLayerRaycast);
            if (Hit.transform != null)
            {
                HitDir += Hit.normal;
                Debug.DrawLine(item.position, Hit.point);
                DrawSurfaceHitted(Hit);
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
        avgDistance = dist / _parentOfFloorRayOrigins.childCount;
        if (miss > Mathf.CeilToInt(2.0f / 3.0f * _parentOfFloorRayOrigins.childCount))
        {
            floor = Vector3.zero;
        }
        return ret;
    }
    public bool CheckBumpedCondition => _bumped;
    public Vector2 GetMovementControllInputVector => _inputMap.Player.Movement.ReadValue<Vector2>();
    public float GetJumpControllInputValue => _inputMap.Player.Jump.ReadValue<float>();
    void DrawSurfaceHitted(RaycastHit hit)
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
