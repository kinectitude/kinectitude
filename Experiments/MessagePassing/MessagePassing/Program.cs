using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessagePassing.Core;
using MessagePassing.Components;

namespace MessagePassing
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Construct game objects and set local variables

            Game game = new Game();

            game["x"] = 0;
            game["y"] = 0;

            Scene scene = new Scene();
            game.AddScene(scene);

            Entity player = new Entity();
            scene.AddEntity(player);

            player["score"] = 0;
            player["money"] = 10;
            player["health"] = 100;

            TransformComponent playerTransform = new TransformComponent();
            player.AddComponent(playerTransform);

            playerTransform.X = 100.0d;
            playerTransform.Y = 100.0d;

            PhysicsComponent playerPhysics = new PhysicsComponent();
            player.AddComponent(playerPhysics);

            playerPhysics.Type = "Player";
            playerPhysics.LinearVelocityX = 8.0d;
            playerPhysics.LinearVelocityY = 8.0d;

            Entity coin = new Entity();
            scene.AddEntity(coin);

            coin["money"] = 10000;

            TransformComponent coinTransform = new TransformComponent();
            coin.AddComponent(coinTransform);

            coinTransform.X = 200.0d;
            coinTransform.Y = 200.0d;

            PhysicsComponent coinPhysics = new PhysicsComponent();
            coin.AddComponent(coinPhysics);

            coinPhysics.LinearVelocityX = 0.0d;
            coinPhysics.LinearVelocityY = 0.0d;

            TextRenderComponent coinTextRender = new TextRenderComponent();
            coin.AddComponent(coinTextRender);

            // Subscribe to some events.

            game.Subscribe("ChangeX", (data) =>
                {
                    game["x"] = data[0];
                    if ((int)game["x"] == 5)
                    {
                        game.Publish("SomethingInteresting", game["x"]);
                    }
                }
            );

            game.Subscribe("ChangeY", (data) =>
                {
                    game["y"] = data[0];
                    if ((int)game["y"] == 10)
                    {
                        game.Publish("SomethingCool", game["y"]);
                    }
                }
            );

            player.Subscribe("SomethingInteresting", (data) =>
                {
                    player["score"] = (int)player["score"] + (int)data[0];
                    player.Publish("ChangeY", 10);
                    player["score"] = (int)player["score"] + (int)data[0];
                    player.Publish("ChangeY", 15);
                }
            );

            player.Subscribe("SomethingCool", (data) =>
                {
                    player.Publish("ChangeX", 12);
                    player["health"] = (int)player["health"] + (int)data[0];
                }
            );

            player.Subscribe("AddMoney", (data) =>
                {
                    player["money"] = (int)player["money"] + (int)data[0];
                }
            );

            coin.Subscribe("TestActions", (data) =>
                {
                    coin.Publish("Physics.SetType", "Coin");
                    coin.Publish("Transform.SetPosition", 70.0d, 80.0d);
                }
            );

            coin.Subscribe("Physics.Collision", (data) =>
                {
                    coin.Publish("AddMoney", coin["money"]);
                }
            , "Player");

            // Initialize game

            player.Initialize();
            coin.Initialize();

            // Simulate game

            game.Publish("ChangeX", 5);
            scene.Publish("TestActions");
            scene.GetManager<PhysicsManager>().SimulateCollision();

            bool passed = true;

            if ((int)game["x"] != 12)
            {
                passed = false;
            }

            if ((int)game["y"] != 15)
            {
                passed = false;
            }

            if ((int)player["score"] != 10)
            {
                passed = false;
            }

            if ((int)player["health"] != 110)
            {
                passed = false;
            }

            if ((int)player["money"] != 10010)
            {
                passed = false;
            }

            if (!coinTextRender.Success)
            {
                passed = false;
            }

            if (coinPhysics.Type != "Coin")
            {
                passed = false;
            }

            Console.WriteLine("Did we pass? {0}", passed);
        }
    }
}
