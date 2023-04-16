using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct LeftGesture{
    public string name;
    public List<Vector3> fingersData;
    public UnityEvent onRecognized;
}

public class LeftHandGestureDetector : MonoBehaviour
{

    public OVRSkeleton skeleton;
    public List<LeftGesture> gestures;
    public bool debugMode = true;
    public float threshold = 0.1f;

    private List<OVRBone> fingerBones;
    private LeftGesture prevGesture;

    // Start is called before the first frame update
    void Start()
    {
        fingerBones = new List<OVRBone>(skeleton.Bones);
        prevGesture = new LeftGesture();
    }

    // Update is called once per frame
    void Update()
    {
        if(debugMode && Input.GetKeyDown(KeyCode.L))
        {
            Save();
        }

        LeftGesture currGesture = Recognize();
        bool hasRecognized = !currGesture.Equals(new LeftGesture());

        if(hasRecognized && !currGesture.Equals(prevGesture)) 
        {
            Debug.Log("Gesture Detected: "+ currGesture.name);
            prevGesture = currGesture;
            currGesture.onRecognized.Invoke();
        }
    }

    void Save() 
    {
        LeftGesture g = new LeftGesture();
        g.name = "New Gesture";
        List<Vector3> data = new List<Vector3>();
        foreach (var bone in fingerBones)
        {
            data.Add(skeleton.transform.InverseTransformPoint(bone.Transform.position));
        }

        g.fingersData = data;
        gestures.Add(g);
    }

    LeftGesture Recognize()
    {
        LeftGesture currGesture = new LeftGesture();
        float currMin = Mathf.Infinity;
        foreach (var gesture in gestures)
        {
            float sumDistance = 0;
            bool isDiscarded = false;
            for (int i = 0; i < fingerBones.Count; i++)
            {
                Vector3 currData = skeleton.transform.InverseTransformPoint(fingerBones[i].Transform.position);
                float distance = Vector3.Distance(currData, gesture.fingersData[i]);
                if(distance>threshold)
                {
                    isDiscarded = true;
                    break;
                }

                sumDistance += distance;
            }

            if(!isDiscarded && sumDistance < currMin) {
                currMin = sumDistance;
                currGesture = gesture;
            }
        }
        return currGesture;
    }
}
