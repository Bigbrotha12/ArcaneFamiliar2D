using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace InterOperability
{
    public class SceneAssetLoader
    {
        private AsyncOperationHandle<SceneInstance> _currentLoadOperations;
        private AsyncOperationHandle<SceneInstance> _unloadOperation;

        public void LoadRemoteScene(AssetReference sceneReference)
        {
            _currentLoadOperations = Addressables.LoadSceneAsync(sceneReference, LoadSceneMode.Single);
        }

        public void UnloadActiveScene()
        {
            if(_currentLoadOperations.IsValid())
            {
                Addressables.Release<SceneInstance>(_currentLoadOperations);
            }
            
        }

        public float GetStatus()
        {
            if(_currentLoadOperations.IsValid())
            {
                return _currentLoadOperations.PercentComplete;     
            }
            
            return 1.0f;
        }

        
    }
}