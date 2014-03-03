﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelegateAction.cs" company="Slash Games">
//   Copyright (c) Slash Games. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Slash.Unity.Common.Triggers.Actions
{
    using System;

    using UnityEngine;

    public class DelegateAction : MonoBehaviour, IAction
    {
        #region Fields

        public MethodDelegate Delegate;

        #endregion

        #region Public Methods and Operators

        public void Execute(object actionArgs)
        {
            if (this.Delegate != null)
            {
                this.Delegate.Invoke();
            }
        }

        #endregion
    }

    [Serializable]
    public class MethodDelegate
    {
        #region Fields

        public string Method;

        public MonoBehaviour Target;

        private Delegate action;

        #endregion

        #region Public Methods and Operators

        public void Invoke(params object[] args)
        {
            if (this.action == null)
            {
                // TODO: Not availabe on Windows Store targets. Find another way to create delegate (or dynamically invoke method with doing so) and create Unity.Common library for Windows Store targets.
                throw new NotImplementedException();

                // this.action = Delegate.CreateDelegate(typeof(Action), this.Target, this.Method);
            }
            this.action.DynamicInvoke(args);
        }

        #endregion
    }
}