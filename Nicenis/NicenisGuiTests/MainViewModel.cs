﻿/*
 * Author   JO Hyeong-Ryeol
 * Since    2017.10.15
 * 
 * This file is a part of the Nicenis project.
 * https://github.com/nicenis/nicenis
 * 
 * Copyright (C) 2017 JO Hyeong-Ryeol. All rights reserved.
 */

using Nicenis.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Reflection;

#if NICENIS_UWP
using Windows.UI.Core;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using System.Windows.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace NicenisGuiTests
{
    class MainViewModel : ViewModelBase
    {
        #region Assert

        public interface ILogWriter
        {
            /// <summary>
            /// Appends a log message asynchronously.
            /// This method must be thread-safe.
            /// </summary>
            /// <param name="log">A log message to append.</param>
            void AppendLogAsync(string log);
        }

        public static class Helper
        {
            static ILogWriter _logWriter;

            public static void SetLogWriter(ILogWriter logWriter)
            {
                _logWriter = logWriter;
            }

            public static void AppendLog(string log)
            {
                _logWriter.AppendLogAsync(log);
            }

            public static void IsTrue(bool value, [CallerMemberName] string name = null)
            {
                AppendLog(string.Format("{0}: {1}", name, value ? "success" : "fail"));
            }
        }

        #endregion


        #region FrameworkViewModelTestViewModel

        [TestClass]
        class FrameworkViewModelTestViewModel : FrameworkViewModelBase
        {
            #region TestViewModel

            private class TestViewModel : FrameworkViewModelBase
            {
                int _intValue1;

                public int IntValue1
                {
                    get { return _intValue1; }
                    set
                    {
                        if (_intValue1 == value)
                            return;

                        _intValue1 = value;
                        PostPropertyChanged();
                    }
                }

                public int IntValue1ChangeCount { get; private set; }

                protected override void OnPropertyChanged(PropertyChangedEventArgs e)
                {
                    base.OnPropertyChanged(e);

                    switch (e.PropertyName)
                    {
                        case nameof(IntValue1):
                            IntValue1ChangeCount++;
                            break;
                    }
                }

                public void PostPropertyChangedExplicitly(params string[] propertyNames)
                {
                    PostPropertyChanged(propertyNames);
                }

                public void CancelPropertyChangedExplicitly()
                {
                    CancelPostPropertyChanged();
                }
            }

            #endregion


            #region Consturctors

            public FrameworkViewModelTestViewModel() { }

            #endregion


            [TestMethod]
            public void PostPropertyChanged_Must_Not_Raise_PropertyChanged_Event_Immediately()
            {
                // arrange
                var viewModel = new TestViewModel();

                // act
                viewModel.IntValue1 = 1;

                // assert
                Helper.IsTrue(viewModel.IntValue1ChangeCount == 0);
            }

            [TestMethod]
            public async void PostPropertyChanged_Raise_PropertyChanged_Event_Asynchronously()
            {
                // arrange
                var viewModel = new TestViewModel();

                // act
                viewModel.IntValue1 = 1;
                await Task.Delay(1000);

                // assert
                Helper.IsTrue(viewModel.IntValue1ChangeCount == 1);
            }

            [TestMethod]
            public async void PostPropertyChanged_Must_Not_Raise_Duplicated_PropertyChanged_Event()
            {
                // arrange
                var viewModel = new TestViewModel();

                // act
                viewModel.PostPropertyChangedExplicitly(nameof(TestViewModel.IntValue1), nameof(TestViewModel.IntValue1));
                await Task.Delay(1000);

                // assert
                Helper.IsTrue(viewModel.IntValue1ChangeCount == 1);
            }

            [TestMethod]
            public async void PostPropertyChanged_Must_Cancel_PropertyChanged_Event_If_Requested()
            {
                // arrange
                var viewModel = new TestViewModel();

                // act
                viewModel.IntValue1 = 1;
                viewModel.CancelPropertyChangedExplicitly();
                await Task.Delay(1000);

                // assert
                Helper.IsTrue(viewModel.IntValue1ChangeCount == 0);
            }
        }

        #endregion


        #region Constructors

        public MainViewModel() { }

        #endregion


        public async Task RunAsync()
        {
            var testClasseTypes = GetType().GetNestedTypes(BindingFlags.NonPublic)
                                       .Where(p => p.GetTypeInfo().GetCustomAttributes(typeof(TestClassAttribute), inherit: true).Any())
                                       .ToArray();

            foreach (var testClass in testClasseTypes)
            {
                var testInstance = Activator.CreateInstance(testClass);
                var testMethods = testClass.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                                           .Where(p => p.CustomAttributes.Any(p2 => p2.AttributeType == typeof(TestMethodAttribute)))
                                           .ToArray();

                foreach (var testMethod in testMethods)
                {
                    await Task.Delay(0);
                    testMethod.Invoke(testInstance, null);
                }
            }
        }
    }
}
