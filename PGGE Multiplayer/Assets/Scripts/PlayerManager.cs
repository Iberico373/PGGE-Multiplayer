using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public string mPlayerPrefabName;
    public PlayerSpawnPoints mSpawnPoints;

    [HideInInspector]
    public GameObject mPlayerGameObject;
    [HideInInspector]
    private ThirdPersonCamera mThirdPersonCamera;

    private void Start()
    {
        StartCoroutine(Coroutine_DelayPlayerLoad(1.0f));
    }

    IEnumerator Coroutine_DelayPlayerLoad(float secs)
    {
        yield return new WaitForSeconds(secs);

        CreatePlayer();
    }

    //Instantiates player character
    void CreatePlayer()
    {
        //Spawn player on a random spawn point
        Transform mPlayerSpawnPoint = mSpawnPoints.GetSpawnPoint();
        mPlayerGameObject = PhotonNetwork.Instantiate(mPlayerPrefabName,
            mPlayerSpawnPoint.position, 
            mPlayerSpawnPoint.rotation, 
            0);

        mPlayerGameObject.GetComponent<PlayerMovement>().mFollowCameraForward = false;

        //Initialize third person camera script on the main camera
        mThirdPersonCamera = Camera.main.gameObject.AddComponent<ThirdPersonCamera>();
        mThirdPersonCamera.mPlayer = mPlayerGameObject.transform;
        mThirdPersonCamera.mDamping = 5.0f;
        mThirdPersonCamera.mPositionOffset = new Vector3(1.0f, 2.0f, -3.0f);
        mThirdPersonCamera.mCameraType = CameraType.Follow_Track_Pos_Rot;
    }
}
