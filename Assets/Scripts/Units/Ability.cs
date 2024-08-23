using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Units
{
    public enum Triggers
    {
        custom,
        passve,
        onHit,
        onCrit,
        onKill,
        onHurt,
        onBlock,
        onDeath,
        onDig,
        onRevive,
        onActorHurt,
        onHeal,
        onBattleStart,
        onBattleEnd,
    }
    abstract class Ability : ScriptableObject
    {
        public float[] powerLevels { get; private set; }
        public string abilityName { get; private set; }

        public string tooltipDisplayString;

    }
}
