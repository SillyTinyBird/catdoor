using UnityEngine;

public class StateOnGround : MovementBase
{
    public override void UpdateState(PlayerMovement context)
    {
        if (context.GetJumpControllInputValue > 0.0f)//happens two times for each jump since player too close to the ground
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

        MoveSum += Vector3.SmoothDamp(context.transform.position, context.transform.position + (floor * (context._floorDist - avgDistance)), ref context._velocity, context._smoothTimeRotationDivision) - context.transform.position;
        if (context.GetMovementControllInputVector != Vector2.zero)
        {
            MoveSum += Vector3.Lerp(context.transform.position,
                context.transform.position +
                context._movementSpeed * Time.fixedDeltaTime * viewDirection.normalized * context.GetMovementControllInputVector.y + //forward component
                context._movementSpeed * Time.fixedDeltaTime * Vector3.Cross(viewDirection.normalized, -context.transform.up) * context.GetMovementControllInputVector.x,//side component
                context._smoothTimeMoveDivision) - context.transform.position;
            context._playerRigidBody.MoveRotation(Quaternion.Slerp(context.transform.rotation, Quaternion.LookRotation(viewDirection.normalized * context.GetMovementControllInputVector.y + Vector3.Cross(viewDirection.normalized, -context.transform.up) * context.GetMovementControllInputVector.x, floor), context._smoothTimeRotationDivision));
        }
        else
        {
            context._playerRigidBody.MoveRotation(Quaternion.Slerp(context.transform.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(context.transform.forward, floor), floor), context._smoothTimeRotationDivision));
        }
        context.PreviousUpDirection = floor;
        context._playerRigidBody.MovePosition(MoveSum);
        if (!notHits)
        {
            context.SwitchState(context.stateEdge);
        }
    }
}
