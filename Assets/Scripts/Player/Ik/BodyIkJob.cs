using UnityEngine;
using UnityEngine.Animations;

namespace Player.Ik
{
    public struct BodyIkJob : IAnimationJob
    {
        // AnimationStream 中表示 LeftHand 快照的一个索引句柄
        public TransformSceneHandle LeftHand;  
        public TransformSceneHandle RightHand;
        public float LeftWeight;
        public float RightWeight;
        
        // AnimationStream 动画系统评估构建的那一帧该动画有关 Transform 和动画依赖的 Transform 组件的只读快照
        public void ProcessAnimation(AnimationStream stream)
        {
            if (!stream.isValid) return;
            var human = stream.AsHuman();
            
            if (LeftHand.IsValid(stream))
            {
                // 通过 TransformSceneHandle 对 Transform 的映射在 stream 快照中获取到其 position值
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