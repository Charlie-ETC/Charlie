#if UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX
using System;
using UnityEngine;

namespace Vuforia
{
    public interface ITrackableEventHandler {}
    public class ReconstructionBehaviour : MonoBehaviour {
        public PropBehaviour AssociateProp(PropBehaviour templateBehaviour, Prop newProp) { return null; }
        public SurfaceBehaviour AssociateSurface(SurfaceBehaviour templateBehaviour, Surface newSurface) { return null; }
        public void RegisterPropCreatedCallback(Action<Prop> callback) {}
        public void RegisterSurfaceCreatedCallback(Action<Surface> callback) {}
        public void UnregisterPropCreatedCallback(Action<Prop> callback) {}
        public void UnregisterSurfaceCreatedCallback(Action<Surface> callback) {}
    }
    public class PropBehaviour : MonoBehaviour {}
    public class SurfaceBehaviour :MonoBehaviour {}
    public class TrackableBehaviour : MonoBehaviour {
        public string TrackableName { get; }
        public void RegisterTrackableEventHandler(ITrackableEventHandler trackableEventHandler) {}
        public enum Status
        {
            NOT_FOUND = -1,
            UNKNOWN = 0,
            UNDEFINED = 1,
            DETECTED = 2,
            TRACKED = 3,
            EXTENDED_TRACKED = 4
        }
    }
    public class Prop {}
    public class Surface {}
    public class VuforiaRuntime {
        public static VuforiaRuntime Instance { get { return new VuforiaRuntime (); } }
        public void RegisterVuforiaInitErrorCallback(Action<VuforiaUnity.InitError> callback) {}
        public void UnregisterVuforiaInitErrorCallback(Action<VuforiaUnity.InitError> callback) {}
    }
    public class VuforiaConfiguration {
        public static VuforiaConfiguration Instance;
        public GenericVuforiaConfiguration Vuforia { get; }

        public class GenericVuforiaConfiguration
        {
            public GenericVuforiaConfiguration() {}
            public string LicenseKey { get; set; }
            public bool DelayedInitialization { get; set; }
            public int MaxSimultaneousImageTargets { get; set; }
            public int MaxSimultaneousObjectTargets { get; set; }
            public bool UseDelayedLoadingObjectTargets { get; set; }
        }
    }
}
#endif
