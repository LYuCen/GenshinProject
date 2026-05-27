using mController;
using RootMotion.FinalIK;

namespace Player.Context
{
    public class PlayerIkContext
    {
        public CameraController CameraController;
        public AimIK AimIK;
        public FullBodyBipedIK BodyBipedIK;
        public bool ListenEvents = true;
        public bool IsLocal = true;
    }
}