using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace glumpis.CharacterStats
{
	[Serializable]
	public class CharacterStat
	{
		public float BaseValue;

		public float MaxValue = float.MaxValue;

		public float MinValue = float.MinValue;

		public bool alwaysCeilValue;

		public float Value
		{
			get
			{
				if (isDirty || BaseValue != lastBaseValue)
				{
					lastBaseValue = BaseValue;
					_value = CalculateFinalValue();
					isDirty = false;
				}
				if (alwaysCeilValue) return Mathf.Ceil(_value);
				return _value;
			}
		}

		protected bool isDirty = true;
		protected float _value;
		protected float lastBaseValue = float.MinValue;

		private readonly List<StatModifier> statModifiers;
		public readonly ReadOnlyCollection<StatModifier> StatModifiers;

		public CharacterStat()
		{
			statModifiers = new();
			StatModifiers = statModifiers.AsReadOnly();
		}

		public CharacterStat(float baseValue) : this()
		{
			BaseValue = baseValue;
		}

		public CharacterStat(float baseValue, float minValue, float maxValue = float.MaxValue) : this()
		{
			BaseValue = baseValue;
			MinValue = minValue;
			MaxValue = maxValue;
		}

		public virtual void AddModifier(StatModifier mod)
		{
			isDirty = true;
			statModifiers.Add(mod);
			statModifiers.Sort(CompareModifierOrder);
		}

		protected virtual int CompareModifierOrder(StatModifier a, StatModifier b)
		{
			if (a.Order < b.Order) return -1;
			else if (a.Order > b.Order) return 1;
			return 0; // if (a.Order == b.Order)
		}

		public virtual bool RemoveModifier(StatModifier mod)
		{
			if (statModifiers.Remove(mod))
			{
				isDirty = true;
				return true;
			}
			return false;
		}

		public virtual bool RemoveAllModifiersFromSource(object source)
		{
			bool didRemove = false;

			for (int i = statModifiers.Count - 1; i >= 0; i--)
			{
				if (statModifiers[i].Source == source)
				{
					isDirty = true;
					didRemove = true;
					statModifiers.RemoveAt(i);
				}
			}
			return didRemove;
		}

		protected virtual float CalculateFinalValue()
		{
			float finalValue = BaseValue;
			float sumPercentAdd = 0;

			for (int i = 0; i < statModifiers.Count; i++)
			{
				StatModifier mod = statModifiers[i];

				if (mod.Type == StatModType.Flat)
				{
					finalValue += mod.Value;
				}
				else if (mod.Type == StatModType.PercentAdd)
				{
					sumPercentAdd += mod.Value;
					if (i + 1 >= statModifiers.Count || statModifiers[i + 1].Type != StatModType.PercentAdd)
					{
						finalValue *= 1 + sumPercentAdd;
						sumPercentAdd = 0;
					}
				}
				else if (mod.Type == StatModType.PercentMult)
				{
					finalValue *= 1 + mod.Value;
				}
				else if (mod.Type == StatModType.PercentMultReduction)
				{
					finalValue *= 1 - mod.Value;
				}
			}

			if(finalValue > MaxValue)
			{
				return (float)Math.Round(MaxValue, 2);
			} else if(finalValue < MinValue)
			{
				return (float)Math.Round(MinValue, 2);
			} else
			{
				//12.0001f != 12f
				return (float)Math.Round(finalValue, 2);
			}
		}

		// Implicit and Explicit cast operator overloads, this lets you treat a stat as an int
		public static implicit operator float(CharacterStat s) => s.Value;
		public static implicit operator int(CharacterStat s) => (int)s.Value;

		//Operator overloads so stuff like myInt + myStat works
		public static int operator +(int i, CharacterStat s) => i + (int)s.Value;
		public static int operator -(int i, CharacterStat s) => i - (int)s.Value;
		public static float operator +(float i, CharacterStat s) => i + s.Value;
		public static float operator -(float i, CharacterStat s) => i - s.Value;
		//these let you do stuff like myStat + myMod or myStat -= myMod
		public static CharacterStat operator +(CharacterStat s, StatModifier m)
		{
			CharacterStat modifiedStat = s;
			s.AddModifier(m);
			return modifiedStat;
		}
		public static CharacterStat operator -(CharacterStat s, StatModifier m)
		{
			CharacterStat modifiedStat = s;
			s.RemoveModifier(m);
			return modifiedStat;
		}
	}
}