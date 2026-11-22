using System;
using UnityEngine;


[RequireComponent(typeof(AttackModule))]
public class ParticleOnAttack : MonoBehaviour
{
    private AttackModule attackModule;
    [SerializeField] private ParticleSystem particleSystem;
    private void Awake()
    {
        attackModule = GetComponent<AttackModule>();
    }

    void OnEnable()
    {
        attackModule.OnAttack += shootParticles;
    }

    void OnDisable()
    {
        attackModule.OnAttack -= shootParticles;
    }

    void shootParticles(float _)
    {
        particleSystem.Play();
    }
}
