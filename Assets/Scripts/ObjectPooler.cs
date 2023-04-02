using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Handles object pooling
/// </summary>
public class ObjectPooler : MonoBehaviour
{
    [SerializeField]
    private GameObject objectToPool;
    [SerializeField]
    private int numToPool;
    [SerializeField]
    private bool canSpawnMoreThanPool;

    private readonly List<GameObject> objectsPooled = new();

    private void Start()
    {
        for (int i = 0; i < numToPool; i++)
        {
            GameObject go = Instantiate(objectToPool, transform);
            IPoolableObject poolableInterface = go.GetComponent<IPoolableObject>();
            if (poolableInterface != null) poolableInterface.Pooler = this;
            go.SetActive(false);
            objectsPooled.Add(go);
        }
    }

    /// <summary>
    /// Spawns a new object from the object pool safely, obeying all rules of the pooler
    /// </summary>
    /// <returns>Reference to object spawned</returns>
    public GameObject Spawn()
    {
        GameObject toSpawn;
        try
        {
            toSpawn = objectsPooled.First(x => !x.activeSelf);
        }
        catch
        {
            if (canSpawnMoreThanPool)
            {
                toSpawn = Instantiate(objectToPool, transform);
                objectsPooled.Add(toSpawn);
            }
            else
            {
                return null;
            }
        }

        toSpawn.GetComponent<IPoolableObject>()?.OnSpawn();

        toSpawn.SetActive(true);
        return toSpawn;
    }

    /// <summary>
    /// Despawns the selected game object from the object pool safely
    /// </summary>
    /// <param name="gameObject"></param>
    public static void Despawn(GameObject gameObject)
    {
        gameObject.GetComponent<IPoolableObject>()?.OnDespawn();
        gameObject.SetActive(false);
    }
}

/// <summary>
/// Utilize this on all poolable objects.
/// </summary>
public interface IPoolableObject
{
    /// <summary>
    /// The <c>ObjectPooler</c> associated with this object
    /// </summary>
    public ObjectPooler Pooler { get; set; }

    /// <summary>
    /// Called when object spawns. Use instead of Start()
    /// </summary>
    public void OnSpawn();

    /// <summary>
    /// Called when object despawns. Use instead of OnDestroy()
    /// </summary>
    public void OnDespawn();
}