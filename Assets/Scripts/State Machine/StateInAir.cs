using UnityEngine;

public class StateInAir : MovementBase
{
    public override void UpdateState(PlayerMovement context)
    {
        Vector3 viewDirection = context.GetViewDirection(context.PreviousUpDirection);
        Vector3 MoveSum = context.transform.position;
        MoveSum += Vector3.Lerp(context.transform.position, context.transform.position + context._currentFallVelocity * Time.fixedDeltaTime * -context.PreviousUpDirection, 0.5f) - context.transform.position;
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
        if (context._currentFallVelocity <= 13)
        {
            context._currentFallVelocity += Time.fixedDeltaTime * 10;
        }
        if (Physics.Raycast(context._parentOfFloorRayOrigins.GetChild(0).position, -context.PreviousUpDirection, out RaycastHit HitCentre, context._floorDist + 0.05f, context._groundLayerRaycast))
        {
            context.SwitchState(context.stateEdge);
            context._currentFallVelocity = 0f;
        }
    }
}
