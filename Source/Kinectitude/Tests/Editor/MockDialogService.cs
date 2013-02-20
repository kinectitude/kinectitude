using Kinectitude.Editor.Views.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Kinectitude.Tests.Editor
{
    internal sealed class MockDialogService : IDialogService
    {
        private static readonly Lazy<MockDialogService> instance = new Lazy<MockDialogService>(() => new MockDialogService());

        public static MockDialogService Instance
        {
            get { return instance.Value; }
        }

        private Type dialogType;
        private bool showedLoad;
        private bool showedSave;
        private bool showedFolder;

        private MockDialogService() { }

        public void Start()
        {
            dialogType = null;
            showedLoad = false;
            showedSave = false;
            showedFolder = false;
        }

        public void AssertShowed<TWindow>()
        {
            Assert.AreEqual(typeof(TWindow), dialogType);
        }

        public void AssertShowedLoadDialog()
        {
            Assert.IsTrue(showedLoad);
        }

        public void AssertShowedSaveDialog()
        {
            Assert.IsTrue(showedSave);
        }

        public void AssertShowedFolderDialog()
        {
            Assert.IsTrue(showedFolder);
        }

        public void ShowDialog<TWindow>(object viewModel, DialogCallback onDialogClose) where TWindow : Window, new()
        {
            dialogType = typeof(TWindow);

            if (null != onDialogClose)
            {
                onDialogClose(true);
            }
        }

        public void ShowDialog<TWindow>(object viewModel = null) where TWindow : Window, new()
        {
            ShowDialog<TWindow>(viewModel, null);
        }

        public void ShowLoadDialog(FileDialogCallback onClose)
        {
            showedLoad = true;
        }

        public void ShowSaveDialog(FileDialogCallback onClose)
        {
            showedSave = true;
        }

        public void ShowFolderDialog(FolderDialogCallback onClose)
        {
            showedFolder = true;
        }
    }
}
