using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

/// <summary>
/// Handles randomization of
/// </summary>
[Serializable]
public class WeightedObjectRandomizer : MonoBehaviour
{
    /// <summary>
    /// All weighted objects to spawn
    /// </summary>
    [SerializeField]
    private WeightedObject[] weightedObjects;

    /// <summary>
    /// Selects an object randomly based on weights
    /// </summary>
    /// <typeparam name="T">Expected type of object to return</typeparam>
    /// <returns>Randomly selected object</returns>
    public T GetRandomObject<T>() where T : Object
    {
        return GetRandomObject().GetComponent<T>();
    }

    /// <summary>
    /// Selects an object randomly based on weights
    /// </summary>
    /// <returns>Randomly selected object</returns>
    public GameObject GetRandomObject()
    {
        int totalWeight = weightedObjects.Sum(x => x.weight);
        int i = Random.Range(1, totalWeight + 1);

        foreach (WeightedObject wo in weightedObjects)
        {
            if (i > wo.weight) i -= wo.weight;
            else return wo.weightedObject;
        }

        Debug.LogError("Could not find random object for some reason");
        return null;
    }
}

/// <summary>
/// Container for information of weighted object
/// </summary>
[Serializable]
public struct WeightedObject
{
    /// <summary>
    /// Object to select
    /// </summary>
    public GameObject weightedObject;
    /// <summary>
    /// Weight of object
    /// </summary>
    public int weight;

    public WeightedObject(GameObject weightedObject, int weight)
    {
        this.weightedObject = weightedObject;
        this.weight = weight;
    }
}