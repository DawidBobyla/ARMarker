using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class MultipleImagesTrackerManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> _prefabsToSpawn = new ();

    private ARTrackedImageManager _trackedImageManager;
    private Dictionary<string, GameObject> _arGameObjects;

    private void Start()
    {
        _trackedImageManager = GetComponent<ARTrackedImageManager>();
        if(_trackedImageManager == null) return;
        _trackedImageManager.trackablesChanged.AddListener(OnTrackablesChanged);
        _arGameObjects = new Dictionary<string, GameObject>();
        SetupSceneElements();
    }

    private void OnDestroy()
    {
        _trackedImageManager.trackablesChanged.RemoveListener(OnTrackablesChanged);
    }

    private void OnTrackablesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
    {
        foreach (var trackedImage in args.added)
        {
            UpdateTrackedImages(trackedImage);
        }
        foreach (var trackedImage in args.updated)
        {
            UpdateTrackedImages(trackedImage);
        }
        foreach (var trackedImage in args.removed)
        {
            UpdateTrackedImages(trackedImage.Value);
        }
    }

    private void UpdateTrackedImages(ARTrackedImage trackedImage)
    {
        if(trackedImage == null) return;
        if (trackedImage.trackingState is TrackingState.Limited or TrackingState.None)
        {
            _arGameObjects[trackedImage.referenceImage.name].gameObject.SetActive(false);
            return;
        }

        _arGameObjects[trackedImage.referenceImage.name].gameObject.SetActive(true);
        var tmpTransform = trackedImage.transform;
        _arGameObjects[trackedImage.referenceImage.name].transform.position = tmpTransform.position;
        _arGameObjects[trackedImage.referenceImage.name].transform.rotation = tmpTransform.rotation;
    }

    private void SetupSceneElements()
    {
        foreach (var prefab in _prefabsToSpawn)
        {
            var arObject = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            arObject.name = prefab.name;
            arObject.gameObject.SetActive(false);
            _arGameObjects.Add(arObject.name, arObject);
        }
    }
}
