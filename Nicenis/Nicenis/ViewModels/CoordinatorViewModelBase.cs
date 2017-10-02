﻿/*
 * Author   JO Hyeong-Ryeol
 * Since    2017.10.02
 * 
 * This file is a part of the Nicenis project.
 * https://github.com/nicenis/nicenis
 * 
 * Copyright (C) 2017 JO Hyeong-Ryeol. All rights reserved.
 */

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#if NICENIS_UWP
using TheDispatcherOperation = Windows.Foundation.IAsyncAction;
using TheDispatcherPriority = Windows.UI.Core.CoreDispatcherPriority;
#else
using System.Windows.Threading;
using TheDispatcherOperation = System.Windows.Threading.DispatcherOperation;
using TheDispatcherPriority = System.Windows.Threading.DispatcherPriority;
#endif

namespace Nicenis.ViewModels
{
    /// <summary>
    /// Provides base implementation with coordination related codes for view models.
    /// </summary>
    public abstract class CoordinatorViewModelBase : DispatcherViewModelBase
    {
        #region Helpers

        /// <summary>
        /// The posted property names to notify change.
        /// This property must be accessed on the thread that the Dispatcher is associated with.
        /// </summary>
        List<string> _postedPropertyNames;

        /// <summary>
        /// The action that raises events for the posted property names.
        /// This property must be accessed on the thread that the Dispatcher is associated with.
        /// </summary>
        TheDispatcherOperation _invokeAction;

        /// <summary>
        /// Raises events for the posted property names.
        /// This method must be called on the thread that the Dispatcher is associated with.
        /// </summary>
        private void RaiseEventsForPostedPropertyNames()
        {
            Debug.Assert(_invokeAction != null);

#if NICENIS_UWP
            _invokeAction.Close();
#endif
            _invokeAction = null;

            if (_postedPropertyNames == null)
                return;

            foreach (var propertyName in _postedPropertyNames)
                OnPropertyChanged(propertyName);

            _postedPropertyNames.Clear();
        }

        /// <summary>
        /// Posts an action that raises events for the posted property names if required.
        /// This method must be called on the thread that the Dispatcher is associated with.
        /// </summary>
        private void PostActionIfRequired()
        {
            if (_invokeAction != null)
                return;

#if NICENIS_UWP
            _invokeAction = Dispatcher.RunAsync
            (
                priority: PostDispatcherPriority,
                agileCallback: RaiseEventsForPostedPropertyNames
            );
#else
            _invokeAction = Dispatcher.InvokeAsync
            (
                callback: RaiseEventsForPostedPropertyNames,
                priority: PostDispatcherPriority
            );
#endif
        }

        #endregion


        /// <summary>
        /// The dispatcher priority to use to post.
        /// </summary>
        protected virtual TheDispatcherPriority PostDispatcherPriority => TheDispatcherPriority.Normal;

        /// <summary>
        /// Posts a PropertyChanged event.
        /// This method must be called on the thread that the Dispatcher is associated with.
        /// </summary>
        /// <param name="propertyName">The property name that changed. An Empty value or null indicates that all of the properties have changed. If this parameter is not specified, the property name obtained by the CallerMemberName attribute is used.</param>
        protected virtual void PostPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (_postedPropertyNames == null)
            {
                _postedPropertyNames = new List<string>();
                _postedPropertyNames.Add(propertyName);
            }
            else
            {
                if (_postedPropertyNames.Contains(propertyName) == false)
                    _postedPropertyNames.Add(propertyName);
            }

            PostActionIfRequired();
        }

        /// <summary>
        /// Posts PropertyChanged events.
        /// This method must be called on the thread that the Dispatcher is associated with.
        /// </summary>
        /// <param name="propertyNames">The property names that changed.</param>
        protected void PostPropertyChanged(IEnumerable<string> propertyNames)
        {
            Debug.Assert(propertyNames != null);

            if (_postedPropertyNames == null)
            {
                _postedPropertyNames = new List<string>();
                _postedPropertyNames.AddRange(propertyNames);
            }
            else
            {
                foreach (string propertyName in propertyNames)
                {
                    if (_postedPropertyNames.Contains(propertyName) == false)
                        _postedPropertyNames.Add(propertyName);
                }
            }

            PostActionIfRequired();
        }

        /// <summary>
        /// Posts PropertyChanged events.
        /// This method must be called on the thread that the Dispatcher is associated with.
        /// </summary>
        /// <param name="propertyNames">The property names that changed.</param>
        protected void PostPropertyChanged(params string[] propertyNames)
        {
            PostPropertyChanged((IEnumerable<string>)propertyNames);
        }

        /// <summary>
        /// Cancels posted PropertyChanged events if it is cancellable.
        /// This method must be called on the thread that the Dispatcher is associated with.
        /// </summary>
        protected virtual void CancelPostPropertyChanged()
        {
            if (_invokeAction == null)
                return;

#if NICENIS_UWP
            _invokeAction.Cancel();
#else
            _invokeAction.Abort();
#endif
            _invokeAction = null;
        }

        /// <summary>
        /// Posts a PropertyChanged event after cancelling previous posts.
        /// This method must be called on the thread that the Dispatcher is associated with.
        /// </summary>
        /// <param name="propertyName">The property name that changed. An Empty value or null indicates that all of the properties have changed. If this parameter is not specified, the property name obtained by the CallerMemberName attribute is used.</param>
        protected void PostPropertyChangedAfterCancel([CallerMemberName] string propertyName = null)
        {
            CancelPostPropertyChanged();
            PostPropertyChanged(propertyName);
        }

        /// <summary>
        /// Posts PropertyChanged events after cancelling previous posts.
        /// This method must be called on the thread that the Dispatcher is associated with.
        /// </summary>
        /// <param name="propertyNames">The property names that changed.</param>
        protected void PostPropertyChangedAfterCancel(IEnumerable<string> propertyNames)
        {
            CancelPostPropertyChanged();
            PostPropertyChanged(propertyNames);
        }

        /// <summary>
        /// Posts PropertyChanged events after cancelling previous posts.
        /// This method must be called on the thread that the Dispatcher is associated with.
        /// </summary>
        /// <param name="propertyNames">The property names that changed.</param>
        protected void PostPropertyChangedAfterCancel(params string[] propertyNames)
        {
            PostPropertyChangedAfterCancel((IEnumerable<string>)propertyNames);
        }
    }
}
