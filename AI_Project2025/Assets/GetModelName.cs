using TMPro;
using Unity.MLAgents.Policies;
using UnityEngine;

public class GetModelName : MonoBehaviour
{
    public BehaviorParameters behaviorParameters;
    public TextMeshProUGUI tmp;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tmp.text = behaviorParameters.Model.name;
    }

}
