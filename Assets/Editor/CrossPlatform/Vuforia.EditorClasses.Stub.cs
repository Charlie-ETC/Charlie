#if UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX
using UnityEngine;
using Vuforia;

namespace Vuforia.EditorClasses {
    public interface IDefaultBehaviourAttacher
    {
        void AddDefaultInitializationErrorHandler(GameObject go);
        void AddDefaultSmartTerrainEventHandler(GameObject go, PropBehaviour propBehaviour, SurfaceBehaviour surfaceBehaviour);
        void AddDefaultTrackableBehaviour(GameObject go);
    }

    public class DefaultTrackableBehaviourPlaceholder : MonoBehaviour
    {
        public DefaultTrackableBehaviourPlaceholder() {}
    }

    public class DefaultSmartTerrainEventHandlerPlaceHolder : MonoBehaviour
    {
        public DefaultSmartTerrainEventHandlerPlaceHolder() {}

        public PropBehaviour PropBehaviour { get; set; }
        public SurfaceBehaviour SurfaceBehaviour { get; set; }
    }

    public class DefaultInitializationErrorHandlerPlaceHolder : MonoBehaviour
    {
        public DefaultInitializationErrorHandlerPlaceHolder() {}
    }

    public static class GameObjectFactory
    {
        public static void AddDefaultTrackableBehaviour(GameObject go) {}
        public static GameObject CreateARCamera() { return null; }
        public static GameObject CreateCloudRecognition() { return null; }
        public static GameObject CreateCloudRecognitionImageTarget() { return null; }
        public static GameObject CreateCylinderTarget() { return null; }
        public static GameObject CreateImageTarget() { return null; }
        public static GameObject CreateMultiTarget() { return null; }
        public static GameObject CreateObjectTarget() { return null; }
        public static GameObject CreateSmartTerrain() { return null; }
        public static GameObject CreateSmartTerrainProp() { return null; }
        public static GameObject CreateUserDefinedTargetBuilder() { return null; }
        public static GameObject CreateUserDefinedTargetBuilderImageTarget() { return null; }
        public static GameObject CreateVirtualButton() { return null; }
        public static GameObject CreateVuMark() { return null; }
        public static void SetDefaultBehaviourTypeConfiguration(IDefaultBehaviourAttacher configuration) {}
    }
}
#endif
