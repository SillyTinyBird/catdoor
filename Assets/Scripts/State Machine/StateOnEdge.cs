using UnityEngine;

public class StateOnEdge : MovementBase
{
    public override void UpdateState(PlayerMovement context)
    {
        if (context.GetJumpControllInputValue > 0.0f)//happens two times for each jump since player too close to the ground
        {
            context.Jump();
            return;
        }
        Vector3 floor;
        bool notHits = context.FloorAngleCheck(out floor, out float avgDistance);//geting flor mormal vector info
        Vector3 viewDirection = context.GetViewDirection(context.PreviousUpDirection);
        Vector3 MoveSum = context.transform.position;
        if (context.GetMovementControllInputVector != Vector2.zero)
        {
            MoveSum += Vector3.Lerp(context.transform.position,
                context.transform.position + 
                context._movementSpeed * Time.fixedDeltaTime * viewDirection.normalized * context.GetMovementControllInputVector.y + 
                context._movementSpeed * Time.fixedDeltaTime * Vector3.Cross(viewDirection.normalized, -context.transform.up) * context.GetMovementControllInputVector.x,
                context._smoothTimeMoveDivision) - context.transform.position;//input on t alows smooth speed up and fade out
            context._playerRigidBody.MoveRotation(Quaternion.Slerp(context.transform.rotation, Quaternion.LookRotation(viewDirection.normalized * context.GetMovementControllInputVector.y + Vector3.Cross(viewDirection.normalized, -context.transform.up) * context.GetMovementControllInputVector.x, context.PreviousUpDirection), context._smoothTimeRotationDivision));
        }
        context._playerRigidBody.MovePosition(MoveSum);
        if (floor == Vector3.zero)
        {
            context.SwitchState(context.stateAir);
        }
        if (notHits)
        {
            context.SwitchState(context.stateGround);
        }
    }
}
