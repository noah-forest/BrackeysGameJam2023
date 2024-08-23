using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Units
{
    /// <summary>
    /// struct containing information about an instance of damage thats trying to be dealt
    /// </summary>
    public struct DamageInfo
    {
        public float damage;
        public bool isCrit;
        public float critMultiplier;
        public bool isOverflow;
        /// <summary>
        /// who is dealing the damage
        /// </summary>
        public GameObject attacker;
        /// <summary>
        /// what specifically did the damage, like an ability 
        /// </summary>
        public GameObject inflictor;
    }

    /// <summary>
    /// Struct containing info of a damage exchange that just occured
    /// </summary>
    public struct  DamageReport
    {
        public float healthBeforeDamage;
        public float healthAfterDamage;
        public float damageRemainder;
        public DamageInfo damageInfo;
        public float damageDealt;
        public float incomingDamage;
        public bool wasBlocked;
        public bool wasLethal;
        public GameObject victim;
    }
}
