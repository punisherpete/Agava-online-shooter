using BehaviorDesigner.Runtime.Tasks;

namespace BehaviourTrees
{
    [TaskCategory("Movement")]
    public class StopMovement : Action 
    {
        public SharedBotNavMesh SelfBotNavMesh;

        public override TaskStatus OnUpdate()
        {
            SelfBotNavMesh.Value.Stop();
            return TaskStatus.Success;
        }
    }
}
