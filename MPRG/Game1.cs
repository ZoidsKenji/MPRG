﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace MPRG;

// other than the c# code (.cs) and the image (of sprite), the rest are built in (or part of the monogame framework porject template)
// so for now The code that I've done is Game1.cs, Player.cs, Road.cs, Sprite.cs and Traffic.cs

// -- git update command--
//
// normal commit command:
// git add .
// git commit -m "your message"
// git push origin main
//
// if fail:
// git pull origin main --rebase
// git push origin main
// or:
// git push --force origin main (Use with Caution)


public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private float Xaccel = 300;
    private float Xspeed = 0;
    
    public float cameraSpeed = 80;

    public int lastlanespawn = 3;

    public bool showbackend = true;
    public bool showfrontend = true;

    public int policespawnnum = 1;

    public Texture2D backendTexture;

    List<Sprite> sprites;

    List<Sprite> allSprites;
    List<Sprite> roads;
    List<Police> polices;
    List<Sprite> backendroads;

    List<Sprite> roadLine;
    // List<Sprite> roadLineR;



    public float playerSpeed;
    Player player;


    public float spawnCounter = 3;
    public float counter = 1;

    // ai
    GenericAlgorithm GA;
    public bool trainAI = true;
    public bool AiOpponentON = false;
    public string AiDataPath = "Data/AI";

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {   
        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 120.0);

        _graphics.SynchronizeWithVerticalRetrace = true;
        _graphics.PreferredBackBufferHeight = 960;
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.ApplyChanges();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        Texture2D texture = Content.Load<Texture2D>("MRS");
        backendTexture = Content.Load<Texture2D>("backendTexture");
        Vector2 startPos;
        startPos.X = 490;
        startPos.Y = 600;
        sprites = new List<Sprite>();
        roads = new List<Sprite>();
        polices = new List<Police>();
        backendroads = new List<Sprite>();
        roadLine = new List<Sprite>();
        // roadLineR = new List<Sprite>();

        if (AiOpponentON || trainAI)
        {
            GA = new GenericAlgorithm(Content.Load<Texture2D>("MRS"));
            GA.loadData(AiDataPath);
            foreach (AiOpponent ai in GA.population)
            {
                sprites.Add(ai);
            }
        }

        if (!trainAI)
        {
            player = new Player(texture, startPos);
            sprites.Add(player);
        }
        else
        {
            player = new Player(texture, startPos);
            sprites.Add(player);
        }
        if (showfrontend)
            {

                for (int i = 0; i < 170; i++)
                {
                    roads.Add(new Road(Content.Load<Texture2D>("road"), new Vector2(0, 480 + (i * 3))));
                }

                // for (int i = 0; i < 200; i++){
                //     roadLineR.Add(new RoadLine(Content.Load<Texture2D>("whiteLine"), new Vector2(0, 390 + (i * 3)), 0));
                // }

                for (int i = 0; i < 30; i++)
                {
                    roadLine.Add(new RoadLine(Content.Load<Texture2D>("road"), new Vector2(0, 480 + (i * 3)), 1));
                }

                for (int i = 0; i < 30; i++)
                {
                    roadLine.Add(new RoadLine(Content.Load<Texture2D>("road"), new Vector2(0, 590 + (i * 3)), 1));
                }

                for (int i = 0; i < 30; i++)
                {
                    roadLine.Add(new RoadLine(Content.Load<Texture2D>("road"), new Vector2(0, 790 + (i * 3)), 1));
                }
            }
        if (showbackend){
            backendroads.Add(new Road(Content.Load<Texture2D>("road"), new Vector2(0, 0)));
        }

        // for (int i = 0; i < 320; i++){
        //     sprites.Add(new Road(Content.Load<Texture2D>("road"), new Vector2(0, 320 - (i * 2))));
        // }

        polices.Add(new Police(Content.Load<Texture2D>("MRS"), new Vector2(640, 390)));

        
    }

    protected override void Update(GameTime gameTime)
    {
        int specialEntities = 0;
        float TotialSpecialEntSpeed = 0;


        static (float, bool) crashPhysics(Sprite actionSprite, Sprite reactionSprite, float actionXspeed, int hitboxWidth, int hitboxHeight)
        {
            int actionDiv = 20;
            int reactDiv = 10;
            bool colision = false;
            if (reactionSprite.BackendRect.Intersects(actionSprite.BackendRect) && reactionSprite != actionSprite)
            {
                colision = true;
                float speeddifferent = Math.Abs(actionSprite.speed - reactionSprite.speed);
                //Console.WriteLine(speeddifferent);
                if (reactionSprite.yPos < actionSprite.yPos)
                {
                    if ((actionSprite.yPos - reactionSprite.yPos) > hitboxHeight)
                    {
                        if (Math.Abs(actionSprite.BackendRect.X - reactionSprite.BackendRect.X) < hitboxWidth)
                        {
                            actionSprite.setSpeedTo(reactionSprite.speed - ((speeddifferent / 4) + 0.5f));
                            actionSprite.health -= (int)speeddifferent / actionDiv;
                            reactionSprite.setSpeedTo(reactionSprite.speed + (speeddifferent + 3));
                            reactionSprite.health -= (int)speeddifferent / reactDiv;
                        }
                        else
                        {
                            actionSprite.setSpeedTo(reactionSprite.speed - ((speeddifferent / 4) + 0.5f));
                            actionSprite.health -= (int)speeddifferent / actionDiv;
                            reactionSprite.setSpeedTo(reactionSprite.speed + (speeddifferent + 3));
                            reactionSprite.health -= (int)speeddifferent / reactDiv;
                        }
                    }
                    else
                    {
                        float direction = (actionXspeed != 0) ? (actionXspeed / Math.Abs(actionXspeed)) : 1;
                        actionXspeed = -Math.Abs(actionXspeed) * direction;
                        actionSprite.moveX(actionXspeed);
                        actionSprite.health -= (int)speeddifferent / (actionDiv * 2);
                        reactionSprite.health -= (int)speeddifferent / (reactDiv * 2);

                    }


                }
                else if (reactionSprite.yPos > actionSprite.yPos)
                {
                    if ((reactionSprite.yPos - actionSprite.yPos) > hitboxHeight)
                    {
                        if (Math.Abs(actionSprite.BackendRect.X - reactionSprite.BackendRect.X) < hitboxWidth)
                        {
                            actionSprite.setSpeedTo(reactionSprite.speed + ((speeddifferent / 2) + 0.5f));
                            actionSprite.health -= (int)speeddifferent / actionDiv;
                            reactionSprite.setSpeedTo(actionSprite.speed - (speeddifferent + 3));
                            reactionSprite.health -= (int)speeddifferent / reactDiv;
                        }
                        else
                        {
                            actionSprite.setSpeedTo(reactionSprite.speed + ((speeddifferent / 2) + 0.5f));
                            actionSprite.health -= (int)speeddifferent / actionDiv;
                            reactionSprite.setSpeedTo(actionSprite.speed - (speeddifferent + 3));
                            reactionSprite.health -= (int)speeddifferent / reactDiv;
                        }
                    }
                    else
                    {
                        float direction = (actionXspeed != 0) ? (actionXspeed / Math.Abs(actionXspeed)) : 1;
                        actionXspeed = -Math.Abs(actionXspeed) * direction;
                        actionSprite.moveX(actionXspeed);
                        actionSprite.health -= (int)speeddifferent / (actionDiv * 2);
                        reactionSprite.health -= (int)speeddifferent / (reactDiv * 2);

                    }
                }
                //player.accelerate(((player.Rect.Y - sprite.Rect.Y) / 2) * (float)gameTime.ElapsedGameTime.TotalSeconds);
                //Console.WriteLine("Crash");
            }
            return (actionXspeed, colision);
        }

        Xaccel = player.speed / 10;
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)){
            Exit();
        }
        
        spawnCounter += (float)gameTime.ElapsedGameTime.TotalSeconds * playerSpeed;
        counter += (float)gameTime.ElapsedGameTime.TotalSeconds * playerSpeed;
        if (spawnCounter > 300){
            int laneToSpawn = new Random().Next(0, 3);
            if (laneToSpawn != lastlanespawn){
                sprites.Add(new Traffic(Content.Load<Texture2D>("FITRS"), new Vector2(640, 390), laneToSpawn));
                lastlanespawn = laneToSpawn;
            }else{
                int laneToSpawn2 = new Random().Next(0, 2);
                if (laneToSpawn2 != lastlanespawn){
                    sprites.Add(new Traffic(Content.Load<Texture2D>("FITRS"), new Vector2(640, 390), laneToSpawn2));
                    lastlanespawn = laneToSpawn2;
                }
            }
            spawnCounter = 0;
            
        }

        if ((counter > 200) && showfrontend){
            for (int i = 0; i < 15; i++){
                roadLine.Add(new RoadLine(Content.Load<Texture2D>("road"), new Vector2(0, 350), 1));
            }  
        }

        sprites.RemoveAll(sprite => sprite.yPos > 2000 || sprite.yPos < -1000);
        roadLine.RemoveAll(line => line.Rect.Y > 1000 || line.Rect.Y < 300);

        foreach (Police policesprite in polices)
        {

            policesprite.updateObject((float)gameTime.ElapsedGameTime.TotalSeconds, playerSpeed, -player.xPos);
            if (policesprite.speed < cameraSpeed * 2 && policesprite.speed > cameraSpeed * 0.5 && policesprite.yPos < 960 && policesprite.yPos > -100)
            {
                specialEntities += 1;
                TotialSpecialEntSpeed += policesprite.speed;
            }

            // police maping (0 = road, 1 = traffic, 2 = player, 3 = police, 4 = police path)
            List<List<int>> map = [new List<int>(), new List<int>(), new List<int>()];
            //Rectangle mapcheckRect = new Rectangle(0, 0, 70, 40);
            for (int i = 0; i < 3; i++)
            {
                for (int n = 0; n < 65; n++)
                {
                    //if (mapcheckRect.Intersects(sprites.BackendRect))
                    map[i].Add(0);

                    //mapcheckRect = new Rectangle(i * 100, n * 40, 70, 40);
                }
            }

            //put police into the grid
            int PxGrid = (int)(policesprite.xPos / 210) + 1;
            int PyGrid = (int)(policesprite.yPos / 40);
            if (PyGrid < 0)
            {
                PyGrid = 0;
            }
            if (PyGrid > 64)
            {
                PyGrid = 64;
            }

            if (PxGrid < 0)
            {
                PxGrid = 0;
            }
            if (PxGrid > 2)
            {
                PxGrid = 2;
            }
            //Console.WriteLine(PxGrid);
            //Console.WriteLine(PyGrid);
            map[PxGrid][PyGrid] = 3;

            foreach (Sprite sprite in sprites)
            {
                int xGrid = (int)(sprite.xPos / 210) + 1;
                int yGrid = (int)(sprite.yPos / 40);
                int y0Grid = yGrid - 1;
                int y2Grid = yGrid + 1;
                int y3Grid = y2Grid + 1;

                if (xGrid < 0)
                {
                    xGrid = 0;
                }
                if (xGrid > 2)
                {
                    xGrid = 2;
                }

                if (yGrid < 0)
                {
                    yGrid = 0;
                    y2Grid = 0;
                    y3Grid = 0;
                }

                if (y0Grid < 0)
                {
                    y0Grid = 0;
                }
                else if (y0Grid > 64)
                {
                    y0Grid = 64;
                }

                if (yGrid > 64)
                {
                    yGrid = 64;
                }

                if (y2Grid > 64)
                {
                    y2Grid = 64;
                }

                if (y3Grid > 64)
                {
                    y3Grid = 64;
                }

                if (map[xGrid][yGrid] == 0 && sprite is not Player)
                {
                    map[xGrid][yGrid] = 1;
                    if (map[xGrid][y2Grid] == 0)
                    {
                        map[xGrid][y2Grid] = 1;
                    }
                    if (map[xGrid][y3Grid] == 0)
                    {
                        map[xGrid][y3Grid] = 1;
                    }
                    if (map[xGrid][y0Grid] == 0)
                    {
                        map[xGrid][y0Grid] = 1;
                    }
                }

                if (sprite is Player && sprite is not AiOpponent)
                {
                    map[xGrid][yGrid] = 2;
                }

                //-- police controller
                float policeXspeed = 0;
                // if (policesprite.yPos > player.yPos && !policesprite.BackendRect.Intersects(sprite.BackendRect))
                // {
                //     policesprite.setSpeedTo(policesprite.speed + (8 * (float)gameTime.ElapsedGameTime.TotalSeconds));
                // }
                // else if (policesprite.BackendRect.Intersects(sprite.BackendRect))
                // {
                //     policesprite.setSpeedTo(policesprite.speed - (40 * (float)gameTime.ElapsedGameTime.TotalSeconds));
                // }
                // else if (policesprite.speed > (playerSpeed * 0.8))
                // {
                //     policesprite.setSpeedTo(policesprite.speed - (10 * (float)gameTime.ElapsedGameTime.TotalSeconds));
                // }
                // else
                // {
                //     policesprite.setSpeedTo(policesprite.speed + (3 * (float)gameTime.ElapsedGameTime.TotalSeconds));
                // }

                //-- police crash physics
                policeXspeed = crashPhysics(policesprite, sprite, policeXspeed, 50, 80).Item1 / 50;



                //-- player controller
                if (sprite is Player playersprite && sprite is not AiOpponent)
                {

                    float fraction = 10f;
                    specialEntities += 2;
                    TotialSpecialEntSpeed += policesprite.speed * 2;

                    if (Keyboard.GetState().IsKeyDown(Keys.D) && playersprite.xPos < 500)
                    {
                        Xspeed += Xaccel * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.A) && playersprite.xPos > -500)
                    {
                        Xspeed -= Xaccel * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    else if (Xspeed > 0)
                    {
                        Xspeed -= (float)gameTime.ElapsedGameTime.TotalSeconds * fraction;
                    }
                    else if (Xspeed < 0)
                    {
                        Xspeed += (float)gameTime.ElapsedGameTime.TotalSeconds * fraction;
                    }

                    if (playersprite.xPos > 550 || player.xPos < -550)
                    {
                        Xspeed = -Math.Abs(Xspeed) * (playersprite.xPos / Math.Abs(playersprite.xPos));
                    }

                    playersprite.moveX(Xspeed);

                    playerSpeed = playersprite.speed;

                    if (playersprite.speed > 0)
                    {
                        playersprite.accelerate(-1 * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    }
                    if (Keyboard.GetState().IsKeyDown(Keys.W))
                    {
                        playersprite.accelerate(30 * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.S) && playersprite.speed > 0)
                    {
                        playersprite.accelerate(-50 * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    }


                }
                else if (sprite is AiOpponent ai)
                {
                    // ai colision punishment
                    foreach (Sprite spriteTraffic in sprites)
                    {
                        if (spriteTraffic != ai && ((spriteTraffic is AiOpponent ^ trainAI) || !trainAI))
                        {
                            if ((spriteTraffic is not Player && trainAI) || !trainAI)
                            {
                                (ai.Xspeed, bool colision) = crashPhysics(ai, spriteTraffic, ai.Xspeed, 35, 80);
                                ai.moveX(ai.Xspeed);
                                if (colision)
                                {
                                    if (spriteTraffic is Player)
                                    {
                                        ai.score -= 15;
                                    }
                                    else
                                    {
                                        ai.score -= 40;
                                    }
                                }
                            }
                        }
                    }

                    foreach (Police policeCar in polices)
                    {
                        (ai.Xspeed, bool colision) = crashPhysics(ai, policeCar, ai.Xspeed, 35, 80);
                        ai.moveX(ai.Xspeed);
                        if (colision)
                        {
                            ai.score -= 10;
                        }
                    }

                    // ai wall bounce
                    if (ai.xPos > 550 || ai.xPos < -550)
                    {
                        ai.moveX(-ai.Xspeed);
                    }

                    ai.DecisionMaking(sprites);

                    if (ai.health < 1 && GA.alivePopulation.Contains(ai))
                    {
                        GA.alivePopulation.Remove(ai);
                    }
                }

                //-- player crash physics
                Xspeed = crashPhysics(player, sprite, Xspeed, 35, 80).Item1;
                Xspeed = crashPhysics(player, policesprite, Xspeed, 35, 80).Item1;

                policesprite.moveX(policeXspeed);



                sprite.updateObject((float)gameTime.ElapsedGameTime.TotalSeconds, cameraSpeed, -player.xPos);

            }
            // List<List<int>> testMap = new List<List<int>> {
            //     new() { 0, 0, 0, 0, 0 },
            //     new() { 0, 1, 1, 1, 0 },
            //     new() { 3, 0, 0, 2, 0 }
            // };
            if (policesprite.xPos > 550 || policesprite.xPos < -550)
            {
                float direction = (policesprite.xSpeed != 0) ? (policesprite.xSpeed / Math.Abs(policesprite.xSpeed)) : 1;
                policesprite.xSpeed = -Math.Abs(policesprite.xSpeed) * direction;
            }
            policesprite.moveX(policesprite.xSpeed);
            policesprite.findPath(map, (float)gameTime.ElapsedGameTime.TotalSeconds, playerSpeed);

            // var map0 = string.Join(",", map[0]);
            // var map1 = string.Join(",", map[1]);as
            // var map2 = string.Join(",", map[2]);
            // Console.WriteLine(map2);
            // Console.WriteLine(map1);
            // Console.WriteLine(map0);

        }
        
        foreach (Sprite road in roads)
        {
            road.moveMidPoint(-player.xPos);
        }

        foreach (Sprite line in roadLine){
            line.moveMidPoint(-player.xPos);
            line.updateObject((float)gameTime.ElapsedGameTime.TotalSeconds, playerSpeed, player.xPos);
        }

        // foreach (Sprite line in roadLineR){
        //     line.moveMidPoint(-player.xPos);
        // }

        //-- camera control
        cameraSpeed = TotialSpecialEntSpeed / specialEntities;
        //smooth out the camera movement and helps to get the player closer to the centre to the camera
        if (player.yPos > 500)
        {
            cameraSpeed = cameraSpeed * 0.9f;
        }
        else
        {
            cameraSpeed = cameraSpeed * 1.1f;
        }
        //camera clamp
        if (player.yPos > 800)
        {
            cameraSpeed = playerSpeed * 0.7f;
        }
        else if (player.yPos < 100)
        {
            cameraSpeed = playerSpeed * 1.3f;
        }
        Console.WriteLine("camspeed" + cameraSpeed);

        if (GA.alivePopulation.Count == 0 && !GA.waitingForNewGen)
        {
            GA.newGen();
            GA.waitingForNewGen = true;
            GA.saveData("saves/gen1/DNA.txt");
        }

        Console.WriteLine(GA.population.Count + "populaton");

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        allSprites = [.. sprites, .. polices];

        _spriteBatch.Begin(samplerState : SamplerState.PointClamp);

        sprites.Sort((a, b) => a.Rect.Y.CompareTo(b.Rect.Y));

        allSprites.Sort((a, b) => a.Rect.Y.CompareTo(b.Rect.Y));

        

        if (showfrontend)
        { //show front end(ray casting)
            // _spriteBatch.Draw(road, new Rectangle(400, 400, 100, 200), Color.White);
            // _spriteBatch.Draw(mrs, new Rectangle(400, 400, 282, 190), Color.White);
            foreach (Sprite road in roads)
            {
                _spriteBatch.Draw(road.texture, road.Rect, road.colour);
            }
            foreach (Sprite line in roadLine)
            {
                _spriteBatch.Draw(line.texture, line.Rect, line.colour);
            }
            // foreach(Sprite line in roadLineR){
            //     _spriteBatch.Draw(line.texture, line.Rect, line.colour);
            // }
            foreach (Sprite sprite in allSprites)
            {
                _spriteBatch.Draw(sprite.texture, sprite.Rect, sprite.colour);
            }
        }
        if (showbackend)
        { //show back end
            foreach (Sprite road in backendroads)
            {
                _spriteBatch.Draw(backendTexture, road.BackendRect, road.backendColour);
            }

            foreach (Sprite sprite in sprites)
            {
                _spriteBatch.Draw(backendTexture, sprite.BackendRect, sprite.backendColour);
            }

            foreach (Sprite police in polices)
            {
                _spriteBatch.Draw(backendTexture, police.BackendRect, police.backendColour);
            }
        }

        

        _spriteBatch.End();

        base.Draw(gameTime);
    }

}
