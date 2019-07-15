﻿using System;
using System.Collections.Generic;

namespace Core
{
    public abstract partial class Unit
    {
        private class BehaviourController
        {
            private readonly List<IUnitBehaviour> activeBehaviours = new List<IUnitBehaviour>();
            private readonly Dictionary<Type, IUnitBehaviour> activeBehavioursByType = new Dictionary<Type, IUnitBehaviour>();

            internal void DoUpdate(int deltaTime)
            {
                foreach (IUnitBehaviour unitBehaviour in activeBehaviours)
                    unitBehaviour.DoUpdate(deltaTime);
            }

            internal void HandleUnitAttach(Unit unit)
            {
                foreach (UnitBehaviour unitBehaviour in unit.unitBehaviours)
                    TryAddBehaviour(unitBehaviour, unit);

                TryAddBehaviour(unit.visibleAuraController, unit);

                foreach (IUnitBehaviour unitBehaviour in activeBehaviours)
                    unitBehaviour.HandleUnitAttach(unit);
            }

            internal void HandleUnitDetach()
            {
                foreach (IUnitBehaviour unitBehaviour in activeBehaviours)
                    unitBehaviour.HandleUnitDetach();

                activeBehaviours.Clear();
                activeBehavioursByType.Clear();
            }

            internal TUnitBehaviour FindBehaviour<TUnitBehaviour>()
            {
                return activeBehavioursByType.TryGetValue(typeof(TUnitBehaviour), out IUnitBehaviour behaviour) ? (TUnitBehaviour)behaviour : default;
            }

            private void TryAddBehaviour(IUnitBehaviour unitBehaviour, Unit unit)
            {
                if (unitBehaviour.HasServerLogic && unit.World.HasServerLogic || unitBehaviour.HasClientLogic && unit.World.HasClientLogic)
                {
                    activeBehaviours.Add(unitBehaviour);
                    activeBehavioursByType.Add(unitBehaviour.GetType(), unitBehaviour);
                }
            }
        }
    }
}