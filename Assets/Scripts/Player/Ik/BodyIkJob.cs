using UnityEngine;
using UnityEngine.Animations;

namespace Player.Ik
{
    public struct BodyIkJob : IAnimationJob
    {
        public TransformSceneHandle LeftHand;
        public TransformSceneHandle RightHand;
        public float LeftWeight;
        public float RightWeight;
        
        
        public void ProcessAnimation(AnimationStream stream)
        {
            if (!stream.isValid) return;
            var human = stream.AsHuman();
            
            if (LeftHand.IsValid(stream))
            {
                human.SetGoalPosition(AvatarIKGoal.LeftHand,LeftHand.GetPosition(stream));
                human.SetGoalRotation(AvatarIKGoal.LeftHand,LeftHand.GetRotation(stream));
                human.SetGoalWeightPosition(AvatarIKGoal.LeftHand, LeftWeight);
                human.SetGoalWeightRotation(AvatarIKGoal.LeftHand, LeftWeight);
            }

            if (RightHand.IsValid(stream))
            {
                human.SetGoalPosition(AvatarIKGoal.RightHand,RightHand.GetPosition(stream));
                human.SetGoalRotation(AvatarIKGoal.RightHand, RightHand.GetRotation(stream));
                human.SetGoalWeightPosition(AvatarIKGoal.RightHand, RightWeight);
                human.SetGoalWeightRotation(AvatarIKGoal.RightHand, RightWeight);
            }
            
            human.SolveIK();
        }

        public void ProcessRootMotion(AnimationStream stream)
        {
            
        }
    }
}