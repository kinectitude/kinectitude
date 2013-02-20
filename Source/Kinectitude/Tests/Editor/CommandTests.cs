using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kinectitude.Editor.Models.Values;
using Attribute = Kinectitude.Editor.Models.Attribute;
using Kinectitude.Editor.Models;
using Kinectitude.Editor.Commands;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.ComponentModel;
using Kinectitude.Editor.Base;
using Kinectitude.Editor.Views.Utils;
using System.Windows;
using Kinectitude.Editor.Views.Dialogs;
using Kinectitude.Editor.Models.Statements.Events;
using Kinectitude.Core.Events;
using Kinectitude.Editor.Models.Statements.Loops;
using Kinectitude.Editor.Models.Statements.Conditions;
using Kinectitude.Editor.Models.Transactions;
using Kinectitude.Editor.Views.Main;
using Kinectitude.Editor.Storage;

namespace Kinectitude.Tests.Editor
{
    [TestClass]
    public class CommandTests
    {
        private sealed class MockDialogService : IDialogService
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

        public CommandTests()
        {
            Workspace.Instance.DialogService = MockDialogService.Instance;
        }

        private static void AssertAfterLog(int ignoreCommands)
        {
            Assert.AreEqual(1, Workspace.Instance.CommandHistory.UndoableCommands.Count - ignoreCommands);
            Assert.AreEqual(0, Workspace.Instance.CommandHistory.RedoableCommands.Count);
            Assert.IsTrue(ignoreCommands > 0 || null != Workspace.Instance.CommandHistory.LastUndoableCommand);
            Assert.IsNull(Workspace.Instance.CommandHistory.LastRedoableCommand);
        }

        private static void AssertAfterUndo(int ignoreCommands)
        {
            Assert.AreEqual(0, Workspace.Instance.CommandHistory.UndoableCommands.Count - ignoreCommands);
            Assert.AreEqual(1, Workspace.Instance.CommandHistory.RedoableCommands.Count);
            Assert.IsNotNull(Workspace.Instance.CommandHistory.LastRedoableCommand);
            Assert.IsTrue(ignoreCommands > 0 || null == Workspace.Instance.CommandHistory.LastUndoableCommand);
        }

        private static void AssertAfterRedo(int ignoreCommands)
        {
            AssertAfterLog(ignoreCommands);
        }

        private static void TestCommand(Action action, Action postconditions)
        {
            MockDialogService.Instance.Start();
            Workspace.Instance.CommandHistory.Clear();
            action();

            if (null != postconditions)
            {
                postconditions();
            }
        }

        private static void TestUndoableCommand(Action preconditions, Action action, Action postconditions, int ignoreCommands)
        {
            MockDialogService.Instance.Start();
            Workspace.Instance.CommandHistory.Clear();
            
            if (null != preconditions)
            {
                preconditions();
            }

            action();
            AssertAfterLog(ignoreCommands);
            
            if (null != postconditions)
            {
                postconditions();
            }

            Workspace.Instance.CommandHistory.Undo();
            AssertAfterUndo(ignoreCommands);

            if (null != preconditions)
            {
                preconditions();
            }

            Workspace.Instance.CommandHistory.Redo();
            AssertAfterRedo(ignoreCommands);

            if (null != postconditions)
            {
                postconditions();
            }
        }

        private static void TestUndoableCommand(Action preconditions, Action action, Action postconditions)
        {
            TestUndoableCommand(preconditions, action, postconditions, 0);
        }

        [TestMethod]
        public void Attribute_ClearValue()
        {
            var attribute = new Attribute("test") { Value = new Value(5, true) };

            TestUndoableCommand(
                () => Assert.AreEqual(5, attribute.Value.GetIntValue()),
                () => attribute.ClearValueCommand.Execute(null),
                () => Assert.AreEqual(0, attribute.Value.GetIntValue())
            );
        }

        [TestMethod]
        public void Entity_Rename()
        {
            var entity = new Entity();

            TestCommand(
                () => entity.RenameCommand.Execute(null),
                () => MockDialogService.Instance.AssertShowed<NameDialog>()
            );
        }

        [TestMethod]
        public void Entity_Properties()
        {
            var entity = new Entity();

            TestCommand(
                () => entity.PropertiesCommand.Execute(null),
                () => MockDialogService.Instance.AssertShowed<EntityDialog>()
            );
        }

        [TestMethod]
        public void Entity_AddAttribute()
        {
            var entity = new Entity();

            TestUndoableCommand(
                () => Assert.AreEqual(0, entity.Attributes.Count),
                () => entity.AddAttributeCommand.Execute(null),
                () => Assert.AreEqual(1, entity.Attributes.Count)
            );
        }

        [TestMethod]
        public void Entity_RemoveAttribute()
        {
            var entity = new Entity();
            var attribute = new Attribute("test");
            entity.AddAttribute(attribute);

            TestUndoableCommand(
                () => Assert.AreEqual(1, entity.Attributes.Count),
                () => entity.RemoveAttributeCommand.Execute(attribute),
                () => Assert.AreEqual(0, entity.Attributes.Count)
            );
        }

        [TestMethod]
        public void Entity_AddEvent()
        {
            var entity = new Entity();

            TestUndoableCommand(
                () => Assert.AreEqual(0, entity.Events.Count),
                () => entity.AddEventCommand.Execute(Workspace.Instance.Events.First()),
                () => Assert.AreEqual(1, entity.Events.Count)
            );
        }

        [TestMethod]
        public void Entity_RemoveEvent()
        {
            var entity = new Entity();
            var evt = new Event(Workspace.Instance.GetPlugin(typeof(TriggerOccursEvent).FullName));
            entity.AddEvent(evt);

            TestUndoableCommand(
                () => Assert.AreEqual(1, entity.Events.Count),
                () => entity.RemoveEventCommand.Execute(evt),
                () => Assert.AreEqual(0, entity.Events.Count)
            );
        }

        [TestMethod]
        public void Entity_DeleteStatement()
        {
            var entity = new Entity();
            var evt = new Event(Workspace.Instance.GetPlugin(typeof(TriggerOccursEvent).FullName));
            entity.AddEvent(evt);
            var loop = new WhileLoop();
            evt.AddStatement(loop);

            TestUndoableCommand(
                () => Assert.AreEqual(1, evt.Statements.Count),
                () => entity.DeleteStatementCommand.Execute(loop),
                () => Assert.AreEqual(0, evt.Statements.Count)
            );
        }

        [TestMethod]
        public void Game_Rename()
        {
            var game = new Game("Test Game");

            TestCommand(
                () => game.RenameCommand.Execute(null),
                () => MockDialogService.Instance.AssertShowed<NameDialog>()
            );
        }

        [TestMethod]
        public void Game_AddPrototype()
        {
            var game = new Game("Test Game");

            TestUndoableCommand(
                () => Assert.AreEqual(0, game.Prototypes.Count),
                () => game.AddPrototypeCommand.Execute(null),
                () => Assert.AreEqual(1, game.Prototypes.Count),
                1   // ignore setting prototype name
            );
        }

        [TestMethod]
        public void Game_AddAttribute()
        {
            var game = new Game("Test Game");

            TestUndoableCommand(
                () => Assert.AreEqual(0, game.Attributes.Count),
                () => game.AddAttributeCommand.Execute(null),
                () => Assert.AreEqual(1, game.Attributes.Count)
            );
        }

        [TestMethod]
        public void Game_RemoveAttribute()
        {
            var game = new Game("Test Game");
            var attribute = new Attribute("test");
            game.AddAttribute(attribute);

            TestUndoableCommand(
                () => Assert.AreEqual(1, game.Attributes.Count),
                () => game.RemoveAttributeCommand.Execute(attribute),
                () => Assert.AreEqual(0, game.Attributes.Count)
            );
        }

        [TestMethod]
        public void Game_AddScene()
        {
            var game = new Game("Test Game");

            TestUndoableCommand(
                () => Assert.AreEqual(0, game.Scenes.Count),
                () => game.AddSceneCommand.Execute(null),
                () => Assert.AreEqual(1, game.Scenes.Count),
                1 // ignore setting scene name
            );
        }

        [TestMethod]
        public void Game_RemoveItem()
        {
            var game = new Game("Test Game");
            var scene = new Scene("Test Scene");
            game.AddScene(scene);

            TestUndoableCommand(
                () => Assert.AreEqual(1, game.Scenes.Count),
                () => game.RemoveItemCommand.Execute(scene),
                () => Assert.AreEqual(0, game.Scenes.Count)
            );

            var prototype = new Entity() { Name = "Prototype" };
            game.AddPrototype(prototype);

            TestUndoableCommand(
                () => Assert.AreEqual(1, game.Prototypes.Count),
                () => game.RemoveItemCommand.Execute(prototype),
                () => Assert.AreEqual(0, game.Prototypes.Count)
            );
        }

        //[TestMethod]
        //public void Project_AddAsset()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod]
        //public void Project_RemoveAsset()
        //{
        //    Assert.Fail();
        //}

        [TestMethod]
        public void Project_OpenItem()
        {
            var project = new Project();
            var game = new Game("Test Game");
            project.Game = game;

            TestCommand(
                () => project.OpenItemCommand.Execute(game),
                () =>
                {
                    Assert.AreEqual(1, project.OpenItems.Count);
                    Assert.IsNotNull(project.InspectorItem);
                }
            );
        }

        [TestMethod]
        public void Project_InspectItem()
        {
            var project = new Project();
            var game = new Game("Test Game");
            project.Game = game;

            TestCommand(
                () => project.InspectItemCommand.Execute(game),
                () => Assert.IsNotNull(project.InspectorItem)
            );
        }

        [TestMethod]
        public void Project_CloseItem()
        {
            var project = new Project();
            var game = new Game("Test Game");
            project.Game = game;
            project.OpenItem(game);

            TestCommand(
                () => project.CloseItemCommand.Execute(game),
                () =>
                {
                    Assert.AreEqual(0, project.OpenItems.Count);
                    Assert.IsNotNull(project.InspectorItem);
                }
            );
        }

        [TestMethod]
        public void Project_Browse()
        {
            var project = new Project();

            TestCommand(
                () => project.BrowseCommand.Execute(null),
                () => MockDialogService.Instance.AssertShowedFolderDialog()
            );
        }

        //[TestMethod]
        //public void Project_Commit()
        //{
        //    Assert.Fail();
        //}

        [TestMethod]
        public void Scene_Rename()
        {
            var scene = new Scene("Test Scene");

            TestCommand(
                () => scene.RenameCommand.Execute(null),
                () => MockDialogService.Instance.AssertShowed<NameDialog>()
            );
        }

        [TestMethod]
        public void Scene_AddAttribute()
        {
            var scene = new Scene("Test Scene");

            TestUndoableCommand(
                () => Assert.AreEqual(0, scene.Attributes.Count),
                () => scene.AddAttributeCommand.Execute(null),
                () => Assert.AreEqual(1, scene.Attributes.Count)
            );
        }

        [TestMethod]
        public void Scene_RemoveAttribute()
        {
            var scene = new Scene("Test Scene");
            var attribute = new Attribute("test");
            scene.AddAttribute(attribute);

            TestUndoableCommand(
                () => Assert.AreEqual(1, scene.Attributes.Count),
                () => scene.RemoveAttributeCommand.Execute(attribute),
                () => Assert.AreEqual(0, scene.Attributes.Count)
            );
        }

        //[TestMethod]
        //public void Scene_AddEntity()
        //{
        //    Assert.Fail();
        //}

        [TestMethod]
        public void Scene_RemoveEntity()
        {
            var scene = new Scene("Test Scene");
            var entity = new Entity();
            scene.AddEntity(entity);

            TestUndoableCommand(
                () => Assert.AreEqual(1, scene.Entities.Count),
                () => scene.RemoveEntityCommand.Execute(entity),
                () => Assert.AreEqual(0, scene.Entities.Count)
            );
        }

        [TestMethod]
        public void Scene_Properties()
        {
            var scene = new Scene("Test Scene");

            TestCommand(
                () => scene.PropertiesCommand.Execute(null),
                () => MockDialogService.Instance.AssertShowed<SceneDialog>()
            );
        }

        //[TestMethod]
        //public void Scene_Cut()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod]
        //public void Scene_Copy()
        //{
        //    Assert.Fail();
        //}

        //[TestMethod]
        //public void Scene_Paste()
        //{
        //    Assert.Fail();
            
        //}

        [TestMethod]
        public void Scene_Delete()
        {
            var scene = new Scene("Test Scene");
            var entity = new Entity();
            scene.AddEntity(entity);

            TestUndoableCommand(
                () => Assert.AreEqual(1, scene.EntityPresenters.Count),
                () => scene.DeleteCommand.Execute(Enumerable.Repeat(scene.EntityPresenters.First(), 1)),
                () => Assert.AreEqual(0, scene.EntityPresenters.Count)
            );
        }

        [TestMethod]
        public void AbstractStatement_InsertBefore()
        {
            var evt = new Event(Workspace.Instance.GetPlugin(typeof(TriggerOccursEvent).FullName));
            var loop = new WhileLoop();
            evt.AddStatement(loop);
            var stmt = new ForLoop();

            TestUndoableCommand(
                () =>
                {
                    Assert.AreEqual(1, evt.Statements.Count);
                    Assert.AreEqual(0, evt.IndexOf(loop));
                    Assert.AreEqual(-1, evt.IndexOf(stmt));
                },
                () => loop.InsertBeforeCommand.Execute(stmt),
                () =>
                {
                    Assert.AreEqual(2, evt.Statements.Count);
                    Assert.AreEqual(1, evt.IndexOf(loop));
                    Assert.AreEqual(0, evt.IndexOf(stmt));
                }
            );
        }

        [TestMethod]
        public void CompositeStatement_AddAction()
        {
            var loop = new WhileLoop();
            var stmt = new ForLoop();

            TestUndoableCommand(
                () =>
                {
                    Assert.AreEqual(0, loop.Statements.Count);
                    Assert.AreEqual(-1, loop.IndexOf(stmt));
                },
                () => loop.AddActionCommand.Execute(stmt),
                () =>
                {
                    Assert.AreEqual(1, loop.Statements.Count);
                    Assert.AreEqual(0, loop.IndexOf(stmt));
                }
            );
        }

        [TestMethod]
        public void AbstractConditionGroup_AddElseIf()
        {
            var group = new ConditionGroup();

            TestUndoableCommand(
                () => Assert.AreEqual(0, group.Statements.Count),
                () => group.AddElseIfCommand.Execute(new ExpressionCondition()),
                () => Assert.AreEqual(1, group.Statements.Count)
            );
        }

        [TestMethod]
        public void AbstractConditionGroup_AddElse()
        {
            var group = new ConditionGroup();

            TestUndoableCommand(
                () => Assert.IsNull(group.Else),
                () => group.AddElseCommand.Execute(null),
                () => Assert.IsNotNull(group.Else)
            );
        }

        [TestMethod]
        public void Event_AddStatement()
        {
            var evt = new Event(Workspace.Instance.GetPlugin(typeof(TriggerOccursEvent).FullName));

            TestUndoableCommand(
                () => Assert.AreEqual(0, evt.Statements.Count),
                () => evt.AddStatementCommand.Execute(new WhileLoop()),
                () => Assert.AreEqual(1, evt.Statements.Count)
            );
        }

        [TestMethod]
        public void EntityRenameTransaction_Commit()
        {
            var entity = new Entity();
            var transaction = new EntityRenameTransaction(entity);
            transaction.Name = "testEntity";

            TestUndoableCommand(
                () => Assert.IsNull(entity.Name),
                () => transaction.CommitCommand.Execute(null),
                () => Assert.AreEqual("testEntity", entity.Name)
            );
        }

        [TestMethod]
        public void EntityTransaction_Commit()
        {
            var prototype = new Entity() { Name = "prototype" };
            var entity = new Entity();
            var transaction = new EntityTransaction(Enumerable.Repeat(prototype, 1), entity);
            transaction.Name = "testEntity";
            transaction.AddPrototype(prototype);
            transaction.AddComponent(transaction.AvailableComponents.First());

            TestUndoableCommand(
                () => 
                {
                    Assert.IsNull(entity.Name);
                    Assert.AreEqual(0, entity.Prototypes.Count);
                    Assert.AreEqual(0, entity.Components.Count);
                },
                () => transaction.CommitCommand.Execute(null),
                () =>
                {
                    Assert.AreEqual("testEntity", entity.Name);
                    Assert.AreEqual(1, entity.Prototypes.Count);
                    Assert.AreEqual(1, entity.Components.Count);
                }
            );
        }

        [TestMethod]
        public void EntityTransaction_AddPrototype()
        {
            var prototype = new Entity() { Name = "prototype" };
            var entity = new Entity();
            var transaction = new EntityTransaction(Enumerable.Repeat(prototype, 1), entity);
            transaction.PrototypeToAdd = prototype;

            TestCommand(
                () => transaction.AddPrototypeCommand.Execute(null),
                () =>
                {
                    Assert.AreEqual(0, transaction.AvailablePrototypes.Count);
                    Assert.AreEqual(1, transaction.SelectedPrototypes.Count);
                }
            );
        }

        [TestMethod]
        public void EntityTransaction_RemovePrototype()
        {
            var prototype = new Entity() { Name = "prototype" };
            var entity = new Entity();
            var transaction = new EntityTransaction(Enumerable.Repeat(prototype, 1), entity);
            transaction.AddPrototype(prototype);
            transaction.SelectedPrototype = prototype;

            TestCommand(
                () => transaction.RemovePrototypeCommand.Execute(prototype),
                () =>
                {
                    Assert.AreEqual(1, transaction.AvailablePrototypes.Count);
                    Assert.AreEqual(0, transaction.SelectedPrototypes.Count);
                }
            );
        }

        [TestMethod]
        public void EntityTransaction_MovePrototypeUp()
        {
            var prototype0 = new Entity() { Name = "prototype0" };
            var prototype1 = new Entity() { Name = "prototype0" };
            var entity = new Entity();
            var transaction = new EntityTransaction(new [] { prototype0, prototype1 }, entity);
            transaction.AddPrototype(prototype0);
            transaction.AddPrototype(prototype1);
            transaction.SelectedPrototype = prototype1;

            TestCommand(
                () => transaction.MovePrototypeUpCommand.Execute(null),
                () =>
                {
                    Assert.AreEqual(0, transaction.SelectedPrototypes.IndexOf(prototype1));
                    Assert.AreEqual(1, transaction.SelectedPrototypes.IndexOf(prototype0));
                }
            );
        }

        [TestMethod]
        public void EntityTransaction_MovePrototypeDown()
        {
            var prototype0 = new Entity() { Name = "prototype0" };
            var prototype1 = new Entity() { Name = "prototype0" };
            var entity = new Entity();
            var transaction = new EntityTransaction(new[] { prototype0, prototype1 }, entity);
            transaction.AddPrototype(prototype0);
            transaction.AddPrototype(prototype1);
            transaction.SelectedPrototype = prototype0;

            TestCommand(
                () => transaction.MovePrototypeDownCommand.Execute(null),
                () =>
                {
                    Assert.AreEqual(0, transaction.SelectedPrototypes.IndexOf(prototype1));
                    Assert.AreEqual(1, transaction.SelectedPrototypes.IndexOf(prototype0));
                }
            );
        }

        [TestMethod]
        public void EntityTransaction_AddComponent()
        {
            var transaction = new EntityTransaction(Enumerable.Empty<Entity>(), new Entity());
            transaction.ComponentToAdd = transaction.AvailableComponents.First();
            int available = transaction.AvailableComponents.Count;

            TestCommand(
                () => transaction.AddComponentCommand.Execute(null),
                () =>
                {
                    Assert.AreEqual(available - 1, transaction.AvailableComponents.Count);
                    Assert.AreEqual(1, transaction.SelectedComponents.Count);
                }
            );
        }

        [TestMethod]
        public void EntityTransaction_RemoveComponent()
        {
            var transaction = new EntityTransaction(Enumerable.Empty<Entity>(), new Entity());
            transaction.AddComponent(transaction.AvailableComponents.First());
            transaction.SelectedComponent = transaction.SelectedComponents.First();
            int available = transaction.AvailableComponents.Count;

            TestCommand(
                () => transaction.RemoveComponentCommand.Execute(null),
                () =>
                {
                    Assert.AreEqual(available + 1, transaction.AvailableComponents.Count);
                    Assert.AreEqual(0, transaction.SelectedComponents.Count);
                }
            );
        }

        [TestMethod]
        public void SceneTransaction_Commit()
        {
            var scene = new Scene("Test Scene");
            var transaction = new SceneTransaction(scene);
            transaction.Name = "Test Scene 2";
            transaction.AddManager(transaction.AvailableManagers.First());

            TestUndoableCommand(
                () =>
                {
                    Assert.AreEqual("Test Scene", scene.Name);
                    Assert.AreEqual(0, scene.Managers.Count);
                },
                () => transaction.CommitCommand.Execute(null),
                () =>
                {
                    Assert.AreEqual("Test Scene 2", scene.Name);
                    Assert.AreEqual(1, scene.Managers.Count);
                }
            );
        }

        [TestMethod]
        public void SceneTransaction_AddManager()
        {
            var transaction = new SceneTransaction(new Scene("Test Scene"));
            transaction.ManagerToAdd = transaction.AvailableManagers.First();
            int available = transaction.AvailableManagers.Count;

            TestCommand(
                () => transaction.AddManagerCommand.Execute(null),
                () =>
                {
                    Assert.AreEqual(available - 1, transaction.AvailableManagers.Count);
                    Assert.AreEqual(1, transaction.SelectedManagers.Count);
                }
            );
        }

        [TestMethod]
        public void SceneTransaction_RemoveManager()
        {
            var transaction = new SceneTransaction(new Scene("Test Scene"));
            transaction.ManagerToAdd = transaction.AvailableManagers.First();
            transaction.AddManager(transaction.ManagerToAdd);
            transaction.SelectedManager = transaction.SelectedManagers.First();
            int available = transaction.AvailableManagers.Count;

            TestCommand(
                () => transaction.RemoveManagerCommand.Execute(null),
                () =>
                {
                    Assert.AreEqual(available + 1, transaction.AvailableManagers.Count);
                    Assert.AreEqual(0, transaction.SelectedManagers.Count);
                }
            );
        }
    }
}
