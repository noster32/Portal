using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CSceneLoadTrigger : MonoBehaviour
{
    [SerializeField] private int sceneNum = 1;
    [SerializeField] private Transform elevator;
    private bool m_isActive = false;

    private PlayerPositionData playerPositionData = new PlayerPositionData();

    private void OnTriggerEnter(Collider other)
    {
        if (!m_isActive && other.tag == "Player")
        {
            m_isActive = true;

            var playerMovement = other.transform.GetComponent<CPlayerMovement>();

            Vector3 playerPos = elevator.InverseTransformPoint(playerMovement.transform.position);
            Quaternion local = Quaternion.Inverse(elevator.transform.rotation) * Camera.main.transform.rotation;
            Vector3 playerVelocity = elevator.InverseTransformDirection(playerMovement.m_oRigidBody.velocity); 
            
            playerPositionData.position = playerPos;
            playerPositionData.rotation = local;
            playerPositionData.velocity = playerVelocity;

            CSceneLoader.Instance.LoadSceneAsync(sceneNum, playerPositionData, (a_oAsyncOperation) =>
            {
                Debug.LogFormat("Percent : {0}", a_oAsyncOperation.progress);
            });
        }
    }
}
