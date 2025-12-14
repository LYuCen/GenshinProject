using UnityEngine;
using UnityEngine.Animations;

namespace Player.Ik
{
    public struct LeftHandIkJob : IAnimationJob
    {
        public TransformSceneHandle LeftHandHandle;
        public float Weight;
    
        public void ProcessAnimation(AnimationStream stream)
        {
            if (stream.isValid && LeftHandHandle.IsValid(stream))
            {
                var human = stream.AsHuman();
            
                human.SetGoalPosition(AvatarIKGoal.LeftHand,LeftHandHandle.GetPosition(stream));
                human.SetGoalRotation(AvatarIKGoal.LeftHand,LeftHandHandle.GetRotation(stream));
                human.SetGoalWeightPosition(AvatarIKGoal.LeftHand, Weight);
                human.SetGoalWeightRotation(AvatarIKGoal.LeftHand, Weight);
                human.SolveIK(); // 计算 Ik
            }
        }

        public void ProcessRootMotion(AnimationStream stream)
        {
        
        }
    }
}