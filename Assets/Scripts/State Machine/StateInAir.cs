using UnityEngine;

public class StateInAir : MovementBase
{
    public override void UpdateState(PlayerMovement context)
    {
        Vector3 floor;
        bool notHits = context.FloorAngleCheck(out floor, out float avgDistance);//geting flor mormal vector info
        Vector3 viewDirection = context.GetViewDirection(floor);
        Vector3 MoveSum = context.transform.position;
        MoveSum += Vector3.Lerp(context.transform.position, context.transform.position + context.fallSpeed * Time.fixedDeltaTime * Vector3.Scale(context.transform.up, new Vector3(-1, -1, -1)) + context.fallSpeedMovement * Time.fixedDeltaTime * context.transform.right * Input.GetAxis("Horizontal"), 0.5f) - context.transform.position;
        context.rb.MovePosition(MoveSum);
        if (Physics.Raycast(context.GroundParent.GetChild(0).position, -context.GroundParent.GetChild(0).transform.up, out RaycastHit HitCentre, context.floorDist + 0.05f, context.GroundLayers))
        {
            context.SwitchState(context.stateGround);
        }
    }
}
