using UnityEngine;

public class StateInAir : MovementBase
{
    public override void UpdateState(PlayerMovement context)
    {
        Vector3 viewDirection = context.GetViewDirection(context.PreviousUpDirection);
        Vector3 MoveSum = context.transform.position;
        MoveSum += Vector3.Lerp(context.transform.position, context.transform.position + context._currentFallVelocity * Time.fixedDeltaTime * -context.PreviousUpDirection, 0.5f) - context.transform.position;
        if (context.GetMovementControllInputVector.y != 0)
        {
            MoveSum += Vector3.Lerp(context.transform.position, context.transform.position + context._movementSpeed * Time.fixedDeltaTime * viewDirection.normalized * context.GetMovementControllInputVector.y + context._movementSpeed * 0.5f * Time.fixedDeltaTime * context.transform.right * context.GetMovementControllInputVector.x, Mathf.Abs(context.GetMovementControllInputVector.y)) - context.transform.position;//input on t alows smooth speed up and fade out
            context._playerRigidBody.MoveRotation(Quaternion.Slerp(context.transform.rotation, Quaternion.LookRotation(viewDirection.normalized + context.transform.right * context.GetMovementControllInputVector.x * 0.5f, context.PreviousUpDirection), context._smoothtimeDivision));
        }
        else
        {
            MoveSum += Vector3.Lerp(context.transform.position, context.transform.position + context._movementSpeed * Time.fixedDeltaTime * context.transform.right * context.GetMovementControllInputVector.x, 0.3f) - context.transform.position;
            context._playerRigidBody.MoveRotation(Quaternion.Slerp(context.transform.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(context.transform.forward, context.PreviousUpDirection), context.PreviousUpDirection), context._smoothtimeDivision));
        }
        context._playerRigidBody.MovePosition(MoveSum);
        if (context._currentFallVelocity <= 13)
        {
            context._currentFallVelocity += Time.fixedDeltaTime * 10;
        }
        if (Physics.Raycast(context._parentOfFloorRayOrigins.GetChild(0).position, -context.PreviousUpDirection, out RaycastHit HitCentre, context._floorDist + 0.05f, context._groundLayerRaycast))
        {
            context.SwitchState(context.stateGround);
            context._currentFallVelocity = 0f;
        }
    }
}
