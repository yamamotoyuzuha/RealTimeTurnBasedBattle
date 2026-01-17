using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRideCheck : MonoBehaviour
{
    [Header("乗れる場所にいる")]
    [SerializeField] private bool isCanRide;
    public bool IsCanRide => isCanRide;

    [Header("乗れるバディモン")]
    [SerializeField] private GameObject buddyMonObj;
    public GameObject BuddyMonObj => buddyMonObj;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("BuddyMonster"))
        {
            isCanRide = true;

            //当たったコライダーのオブジェクトを入れる
            buddyMonObj = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("BuddyMonster"))
        {
            isCanRide = false;
            buddyMonObj = null;
        }
    }
}
