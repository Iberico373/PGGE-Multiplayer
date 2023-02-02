using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PGGE.Multiplayer
{
    public class PlayerManager : MonoBehaviourPunCallbacks
    {
        public string mPlayerPrefabName;
        public PlayerSpawnPoints mSpawnPoints;
        public GameObject playerCam;

        [HideInInspector]
        public GameObject mPlayerGameObject;
        [HideInInspector]
        private ThirdPersonCamera mThirdPersonCamera;

        private void Start()
        {
            StartCoroutine(Coroutine_DelayPlayerLoad(1.0f));
        }

        public void LeaveRoom()
        {
            Debug.LogFormat("LeaveRoom");
            PhotonNetwork.LeaveRoom();
        }

        public override void OnLeftRoom()
        {
            Debug.LogFormat("OnLeftRoom()");
            SceneManager.LoadScene("Menu");
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
}
