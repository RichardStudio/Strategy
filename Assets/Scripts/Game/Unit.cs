using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;
using System.ComponentModel;
using Photon.Pun;

[RequireComponent(typeof(NavMeshAgent))]
public class Unit : MonoBehaviourPunCallbacks
{
    public HealthBar HealthBar;
    public float Health;
    public float Damage;
    public float BaseAttackDistance;
    private float additionalAttackDistance;
    private float attackDistance;

    private bool isWin;
    private bool isDead;

    private NavMeshAgent navMeshAgent;
    private Animator animator;
    [HideInInspector] public GameObject target;
    private GameObject otherMainBuilding;
    [HideInInspector] public GameObject mainBuilding;


    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        if (transform.tag == "Left")
        {
            otherMainBuilding = GameObject.FindGameObjectWithTag("RightBase");
            mainBuilding = GameObject.FindGameObjectWithTag("LeftBase");
        }
        else if (transform.tag == "Right")
        {
            otherMainBuilding = GameObject.FindGameObjectWithTag("LeftBase");
            mainBuilding = GameObject.FindGameObjectWithTag("RightBase");
        }
        else
        {
            if (transform.position.x < 500)
            {
                transform.tag = "Left";
                otherMainBuilding = GameObject.FindGameObjectWithTag("RightBase");
                mainBuilding = GameObject.FindGameObjectWithTag("LeftBase");
            }
            else
            {
                transform.tag = "Right";
                otherMainBuilding = GameObject.FindGameObjectWithTag("LeftBase");
                mainBuilding = GameObject.FindGameObjectWithTag("RightBase");
            }
        }
        transform.LookAt(otherMainBuilding.transform);
        attackDistance = BaseAttackDistance;
        additionalAttackDistance = BaseAttackDistance + 15;
    }

    void Update()
    {
        if (transform.tag == "Left")
        {
            target = GetClosestEnemy(GameObject.FindGameObjectsWithTag("Right"));
        }
        else
        {
            target = GetClosestEnemy(GameObject.FindGameObjectsWithTag("Left"));
        }
        if (target == otherMainBuilding)
        {
            attackDistance = additionalAttackDistance;
        }
        else
        {
            attackDistance = BaseAttackDistance;
        }

        if (otherMainBuilding.GetComponent<Base>().baseHP <= 0)
        {
            isWin= true;
        }
        if (mainBuilding.GetComponent<Base>().baseHP <= 0)
        {
            isDead = true;
        }

        AnimationPlay();
    }

    GameObject GetClosestEnemy(GameObject[] enemies)
    {
        GameObject target = null;
        float closestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (GameObject enemy in enemies)
        {
            if (enemy.GetComponent<Unit>().Health > 0)
            {
                Vector3 direction = enemy.transform.position - currentPosition;
                float dist = direction.sqrMagnitude;
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    target = enemy;
                }
            }
        }
        if (target == null)
        {
            return otherMainBuilding;
        }
        return target;
    }

    void AnimationSetBool(string trigger)
    {
        foreach (var param in animator.parameters)
        {
            animator.SetBool(param.name, param.name == trigger);
        }
    }

    void AnimationPlay()
    {
        if (isDead)
        {
            navMeshAgent.SetDestination(transform.position);
            AnimationSetBool("Die");
            return;
        }
        if (isWin)
        {
            navMeshAgent.SetDestination(transform.position);
            AnimationSetBool("Victory");
            return;
        }
        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance > attackDistance)
        {
            navMeshAgent.updateRotation = true;
            navMeshAgent.SetDestination(target.transform.position);
            AnimationSetBool("Run");
        }
        else if (distance <= attackDistance)
        {
            navMeshAgent.updateRotation = false;
            transform.LookAt(target.transform.position); //transform.LookAt(Vector3.Scale(target.transform.position, Vector3.left));
            navMeshAgent.SetDestination(transform.position);
            AnimationSetBool("Attack");
        }
    }

    public void RecieveDamage(float dmg)
    {
        Health -= dmg;
        HealthBar.SetSliderValue(Health);
        if (Health <= 0)
        {
            Health = 0;
            HealthBar.SetSliderValue(Health);
            isDead = true;
            photonView.RPC("Deathrattle", RpcTarget.Others);
            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(this.photonView);
            }
        }
    }

    public void Attack()
    {
        if (target != null && target != otherMainBuilding)
        {
            target.GetComponent<Unit>().RecieveDamage(Damage);
        }
        else if (target == otherMainBuilding)
        {
            target.GetComponent<Base>().RecieveDamage(Damage);
        }
    }

    [PunRPC]
    private void Deathrattle()
    {
        isDead = true;
    }
}
