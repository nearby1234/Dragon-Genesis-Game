using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceWeapon : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform m_NewWeapon;
    [SerializeField] private WeaponManager WeaponManager;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    //void Update()
    //{

    //    if(Input.GetKeyDown(KeyCode.C))
    //    {
    //        WeaponManager.ChangeWeapon(m_NewWeapon);
    //    }    
    //}

    
}

