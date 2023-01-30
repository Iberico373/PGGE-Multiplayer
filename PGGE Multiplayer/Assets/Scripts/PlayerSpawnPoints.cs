using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnPoints : MonoBehaviour
{
    //List containing all the transforms of the spawn points
    public List<Transform> mSpawnPoints = new List<Transform>();

    public Transform GetSpawnPoint()
    {
        //If no spawn points are set, set the 'PlayerSpawnPoints' object as the spawn point
        if (mSpawnPoints.Count == 0)
        {
            return this.transform;
        }

        //Randomly set a spawn point from a list
        return mSpawnPoints[Random.Range(0, mSpawnPoints.Count)];
    }
}
