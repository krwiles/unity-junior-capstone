#nullable enable

using UnityEngine;
using UnityEngine.InputSystem;

namespace Preview
{
    // Simple preview controller: instantiates the given prefab and moves it
    // to the world position under the mouse on a target Z plane.
    public class TowerPreview : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private PlacementProbe _previewPrefab;
        [SerializeField] private float _targetZ = 0f;
        [SerializeField] private bool _instantiateOnAwake = true;

        private GameObject? _instance;
        private PlacementProbe? _placementProbe;

        private void Awake()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }

            if (_instantiateOnAwake && _previewPrefab != null)
            {
                _instance = Instantiate(_previewPrefab.gameObject, Vector3.zero, Quaternion.identity);
                _placementProbe = _instance.GetComponent<PlacementProbe>();
            }

        }

        private void OnEnable()
        {
            if (!_instantiateOnAwake && _instance == null && _previewPrefab != null)
                _instance = Instantiate(_previewPrefab.gameObject, Vector3.zero, Quaternion.identity);
                _placementProbe = _instance.GetComponent<PlacementProbe>();
        }

        private void OnDisable()
        {
            if (_instance != null)
            {
                Destroy(_instance);
                _instance = null;
                _placementProbe = null;
            }
        }

        private void Update()
        {
            if (_instance == null || _camera == null || _placementProbe == null) return;

            Vector3 pointer = GetPointerWorldPosition();
            Vector3 snapped = GetSnappedWorldPosition(pointer);
            _instance.transform.position = snapped;
        }

        // Public helper: get the world position under the pointer on the configured Z plane
        public Vector3 GetPointerWorldPosition()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
                if (_camera == null) return Vector3.zero;
            }

            Vector2 screenPos = Mouse.current != null ? Mouse.current.position.ReadValue() : (Vector2)Input.mousePosition;
            Ray ray = _camera.ScreenPointToRay(screenPos);
            var plane = new Plane(Vector3.forward, new Vector3(0f, 0f, _targetZ));
            if (plane.Raycast(ray, out float enter))
            {
                return ray.GetPoint(enter);
            }

            return Vector3.zero;
        }

        public Vector3 GetSnappedWorldPosition(Vector3 worldPosition)
        {
            float snappedX = Mathf.Floor(worldPosition.x) + 0.5f;
            float snappedY = Mathf.Floor(worldPosition.y) + 0.5f;
            return new Vector3(snappedX, snappedY, _targetZ);
        }
    }
}
