using UnityEngine;

public class StateBumped : MovementBase
{
    public override void UpdateState(PlayerMovement context)
    {
        Vector3 floor;
        bool notHits = context.FloorAngleCheck(out floor, out float avgDistance);//geting flor mormal vector info
        Vector3 viewDirection = context.GetViewDirection(context.PreviousUpDirection);
        Vector3 MoveSum = context.transform.position;
        if (context.GetMovementControllInputVector.y != 0)
        {
            MoveSum += Vector3.Lerp(context.transform.position, context.transform.position + context._movementSpeed * Time.fixedDeltaTime * viewDirection.normalized * context.GetMovementControllInputVector.y + context._movementSpeed * 0.5f * Time.fixedDeltaTime * context.transform.right * context.GetMovementControllInputVector.x, Mathf.Abs(context.GetMovementControllInputVector.y)) - context.transform.position;//input on t alows smooth speed up and fade out
            context._playerRigidBody.MoveRotation(Quaternion.Slerp(context.transform.rotation, Quaternion.LookRotation(viewDirection.normalized + context.transform.right * context.GetMovementControllInputVector.x * 0.5f, context.PreviousUpDirection), context._smoothtimeDivision));
        }
        else
        {
            MoveSum += Vector3.Lerp(context.transform.position, context.transform.position + context._movementSpeed * Time.fixedDeltaTime * context.transform.right * context.GetMovementControllInputVector.x, 0.3f) - context.transform.position;
        }
        context._playerRigidBody.MovePosition(MoveSum);
        if (floor == Vector3.zero)
        {
            context.SwitchState(context.stateAir);
        }
        if (!notHits)
        {
            context.SwitchState(context.stateEdge);
        }
        if (!context.CheckBumpedCondition)
        {
            context.SwitchState(context.stateGround);
        }
    }
}
