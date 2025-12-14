using UnityEngine;
using UnityEngine.Animations;

namespace Player.Ik
{
    public struct RightHandIkJob : IAnimationJob
    {

        public TransformSceneHandle RightHandHandle;
        public float Weight;
        
        public void ProcessAnimation(AnimationStream stream)
        {
            if (stream.isValid && RightHandHandle.IsValid(stream))
            {
                var human = stream.AsHuman();
                
                human.SetGoalPosition(AvatarIKGoal.RightHand,RightHandHandle.GetPosition(stream));
                human.SetGoalRotation(AvatarIKGoal.RightHand, RightHandHandle.GetRotation(stream));
                human.SetGoalWeightPosition(AvatarIKGoal.RightHand,Weight);
                human.SetGoalWeightRotation(AvatarIKGoal.RightHand,Weight);
                human.SolveIK();
            }
        }

        public void ProcessRootMotion(AnimationStream stream)
        {
            
        }
    }
}