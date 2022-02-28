using UnityEngine;

public class StateOnGround : MovementBase
{
    public override void UpdateState(PlayerMovement context)
    {
        Vector3 floor;
        bool notHits = context.FloorAngleCheck(out floor, out float avgDistance);//geting flor mormal vector info
        Vector3 viewDirection = context.GetViewDirection(floor);
        Vector3 MoveSum = context.transform.position;
        bool hit = Physics.Raycast(context.GroundParent.GetChild(0).position, -context.GroundParent.GetChild(0).transform.up, out RaycastHit HitCentre, context.floorDist + 0.05f, context.GroundLayers);
        /*if(!hit)
        {
            context.SwitchState(context.stateEdge);
            context.stateEdge.UpdateState(context);
            return;
        }*/
        //use just elevation for gravity piece, decide gravity with average distance (?)

        MoveSum += Vector3.SmoothDamp(context.transform.position, context.transform.position + (floor * (context.floorDist - avgDistance)) /*HitCentre.point + floor * context.floorDist*/, ref context.velocity, context.smoothTime) - context.transform.position;
        if (Input.GetAxis("Vertical") != 0)
        {
            MoveSum += Vector3.Lerp(context.transform.position, context.transform.position + context.movementSpeed * Time.fixedDeltaTime * viewDirection.normalized * Input.GetAxis("Vertical") + context.movementSpeed * Time.fixedDeltaTime * context.transform.right * Input.GetAxis("Horizontal"), Mathf.Abs(Input.GetAxis("Vertical"))) - context.transform.position;//input on t alows smooth speed up and fade out
            context.rb.MoveRotation(Quaternion.Slerp(context.transform.rotation, Quaternion.LookRotation(viewDirection.normalized, floor), context.smoothTime));
            //other movement needs to be put in one line, in (Input.GetAxis("Vertical") > 0) shoud stay only rotation

            //place side movement into rotations to eliminate drifts!!!
        }
        else
        {
            MoveSum += Vector3.Lerp(context.transform.position, context.transform.position + context.movementSpeed * Time.fixedDeltaTime * context.transform.right * Input.GetAxis("Horizontal"), 0.3f) - context.transform.position;
            context.rb.MoveRotation(Quaternion.Slerp(context.transform.rotation, Quaternion.LookRotation(Vector3.ProjectOnPlane(context.transform.forward, floor), floor), context.smoothTime));
            //ANY OTHER MOVEMENT
        }
        context.previousUp = floor;
        context.rb.MovePosition(MoveSum);
        if(!notHits)
        {
            context.SwitchState(context.stateEdge);
        }
        /*if(input.getaxis(jump)
    context.velocity = 10
    context.state = in air*/
    }
    public bool IsBetween(float value, float bound1, float bound2)
    {
        if (bound1 > bound2)
            return value >= bound2 && value <= bound1;
        return value >= bound1 && value <= bound2;
    }
}
