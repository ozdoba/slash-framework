﻿// -----------------------------------------------------------------------
// <copyright file="SystemManager.cs" company="Rainy Games">
// Copyright (c) Rainy Games. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace RainyGames.GameBase
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Manages the game systems to be updated in each tick.
    /// </summary>
    public class SystemManager
    {
        #region Constants and Fields

        /// <summary>
        /// Game this manager controls the systems of.
        /// </summary>
        private Game game;

        /// <summary>
        /// Systems to be updated in each tick.
        /// </summary>
        private List<ISystem> systems;

        /// <summary>
        /// Maps system types to actual game systems.
        /// </summary>
        private Dictionary<Type, ISystem> systemsByType;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Constructs a new system manager without any systems.
        /// </summary>
        /// <param name="game">
        /// Game to manage the systems for.
        /// </param>
        public SystemManager(Game game)
        {
            this.game = game;
            this.systems = new List<ISystem>();
            this.systemsByType = new Dictionary<Type, ISystem>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the passed system to this manager. The system will be updated
        /// in each tick.
        /// </summary>
        /// <param name="system">
        /// System to add.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The passed system is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// A system of the same type has already been added.
        /// </exception>
        public void AddSystem(ISystem system)
        {
            if (system == null)
            {
                throw new ArgumentNullException("system");
            }

            Type systemType = system.GetType();

            if (!this.systemsByType.ContainsKey(systemType))
            {
                this.systems.Add(system);
                this.systemsByType.Add(system.GetType(), system);

                this.game.EventManager.QueueEvent(FrameworkEventType.SystemAdded, system);
            }
            else
            {
                throw new ArgumentException("A system of type " + systemType + " has already been added.", "system");
            }
        }

        /// <summary>
        /// Gets the system of the specified type.
        /// </summary>
        /// <param name="systemType">
        /// Type of the system to get.
        /// </param>
        /// <returns>
        /// System of the specified type.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The passed type is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// A system of the specified type has never been added.
        /// </exception>
        public ISystem GetSystem(Type systemType)
        {
            if (systemType == null)
            {
                throw new ArgumentNullException("systemType");
            }

            ISystem system;
            if (this.systemsByType.TryGetValue(systemType, out system))
            {
                return system;
            }
            else
            {
                throw new ArgumentException("A system of type " + systemType + " has never been added.", "systemType");
            }
        }

        /// <summary>
        /// Ticks all systems.
        /// </summary>
        /// <param name="dt">
        /// Time passed since the last tick, in seconds.
        /// </param>
        public void Update(float dt)
        {
            foreach (ISystem system in this.systems)
            {
                system.Update(dt);
            }
        }

        #endregion
    }
}
