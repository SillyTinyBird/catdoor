using UnityEngine;

public class StateOnGround : MovementBase
{
    public override void UpdateState(PlayerMovement context)
    {
        if (context.GetJumpControllInputValue != false)//happens two times for each jump since player too close to the ground
        {
            context.Jump();
            return;
        }
        if (context.CheckBumpedCondition)
        {
            context.SwitchState(context.stateBumped);
            context.stateBumped.UpdateState(context);
            return;
        }
        Vector3 floor;
        bool notHits = context.FloorAngleCheck(out floor, out float avgDistance);//geting flor mormal vector info
        Vector3 viewDirection = context.GetViewDirection(floor);
        Vector3 MoveSum = context.transform.position;
        bool hit = Physics.Raycast(context._parentOfFloorRayOrigins.GetChild(0).position, -context._parentOfFloorRayOrigins.GetChild(0).transform.up, out RaycastHit HitCentre, context._floorDist + 0.05f, context._groundLayerRaycast);

        MoveSum += Vector3.SmoothDamp(context.transform.position, context.transform.position + (floor * (context._floorDist - avgDistance)) /*HitCentre.point + floor * context.floorDist*/, ref context._velocity, context._smoothtimeDivision) - context.transform.position;
        if (context.GetMovementControllInputVector.y != 0)
        {
            MoveSum += Vector3.Lerp(context.transform.position, context.transform.position + context._movementSpeed * Time.fixedDeltaTime * viewDirection.normalized * context.GetMovementControllInputVector.y + context._movementSpeed * 0.5f * Time.fixedDeltaTime * context.transform.right * context.GetMovementControllInputVector.x, Mathf.Abs(context.GetMovementControllInputVector.y)) - context.transform.position;//input on t alows smooth speed up and fade out
            context._playerRigidBody.MoveRotation(Quaternion.Slerp(context.transform.rotation, Quaternion.LookRotation(viewDirection.normalized + context.transform.right * context.GetMovementControllInputVector.x * 0.5f, floor), context._smoothtimeDivision));
        }
        else
        {
            MoveSum += Vector3.Lerp(context.transform.position, context.transform.position + context._movementSpeed * Time.fixedDeltaTime * context.transform.right * context.GetMovementControllInputVector.x, 0.3f) - context.transform.position;
            context._playerRigidBody.MoveRotation(Quaternion.Slerp(context.transform.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(context.transform.forward, floor), floor), context._smoothtimeDivision));
        }
        context.PreviousUpDirection = floor;
        context._playerRigidBody.MovePosition(MoveSum);
        if (!notHits)
        {
            context.SwitchState(context.stateEdge);
        }
    }
}
