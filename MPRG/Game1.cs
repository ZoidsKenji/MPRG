using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

    public int lastlanespawn = 3;

    public bool showbackend = true;
    public bool showfrontend = true;

    public int policespawnnum = 1;

    public Texture2D backendTexture;

    List<Sprite> sprites;

    List<Sprite> allSprites;
    List<Sprite> roads;
    List<Sprite> polices;
    List<Sprite> backendroads;

    List<Sprite> roadLine;
    // List<Sprite> roadLineR;



    public float playerSpeed;
    Player player;


    public float spawnCounter = 3;
    public float counter = 1;

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
        polices = new List<Sprite>();
        backendroads = new List<Sprite>();
        roadLine = new List<Sprite>();
        // roadLineR = new List<Sprite>();


        player = new Player(texture, startPos);
        if (showfrontend){

            for (int i = 0; i < 170; i++){
                roads.Add(new Road(Content.Load<Texture2D>("road"), new Vector2(0, 480 + (i * 3))));
            }
            
            // for (int i = 0; i < 200; i++){
            //     roadLineR.Add(new RoadLine(Content.Load<Texture2D>("whiteLine"), new Vector2(0, 390 + (i * 3)), 0));
            // }
            
            for (int i = 0; i < 30; i++){
                roadLine.Add(new RoadLine(Content.Load<Texture2D>("road"), new Vector2(0, 480 + (i * 3)), 1));
            }

            for (int i = 0; i < 30; i++){
                roadLine.Add(new RoadLine(Content.Load<Texture2D>("road"), new Vector2(0, 590 + (i * 3)), 1));
            }

            for (int i = 0; i < 30; i++){
                roadLine.Add(new RoadLine(Content.Load<Texture2D>("road"), new Vector2(0, 790 + (i * 3)), 1));
            }
        }
        if (showbackend){
            backendroads.Add(new Road(Content.Load<Texture2D>("road"), new Vector2(0, 0)));
        }

        // for (int i = 0; i < 320; i++){
        //     sprites.Add(new Road(Content.Load<Texture2D>("road"), new Vector2(0, 320 - (i * 2))));
        // }
        sprites.Add(player);
        polices.Add(new Police(Content.Load<Texture2D>("MRS"), new Vector2(640, 390)));

        
    }

    protected override void Update(GameTime gameTime)
    {
        static float crashPhysics(Sprite actionSprite, Sprite reactionSprite, float actionXspeed, int hitboxWidth, int hitboxHeight)
        {
            if (reactionSprite.BackendRect.Intersects(actionSprite.BackendRect) && reactionSprite != actionSprite)
            {
                float speeddifferent = Math.Abs(actionSprite.speed - reactionSprite.speed);
                Console.WriteLine(speeddifferent);
                if (reactionSprite.yPos < actionSprite.yPos)
                {
                    if ((actionSprite.yPos - reactionSprite.yPos) > hitboxHeight)
                    {
                        if (Math.Abs(actionSprite.BackendRect.X - reactionSprite.BackendRect.X) < hitboxWidth)
                        {
                            actionSprite.setSpeedTo(reactionSprite.speed - ((speeddifferent / 4) + 0.5f));
                            reactionSprite.setSpeedTo(reactionSprite.speed + (speeddifferent + 3));
                        }
                        else
                        {
                            actionSprite.setSpeedTo(reactionSprite.speed - ((speeddifferent / 4) + 0.5f));
                            reactionSprite.setSpeedTo(reactionSprite.speed + (speeddifferent + 3));
                        }
                    }
                    else
                    {
                        actionXspeed = -(Math.Abs(actionXspeed) / actionXspeed);
                        actionSprite.moveX(actionXspeed);
                    }


                }
                else if (reactionSprite.yPos > actionSprite.yPos)
                {
                    if ((reactionSprite.yPos - actionSprite.yPos) > hitboxHeight)
                    {
                        if (Math.Abs(actionSprite.BackendRect.X - reactionSprite.BackendRect.X) < hitboxWidth)
                        {
                            actionSprite.setSpeedTo(reactionSprite.speed + ((speeddifferent / 2) + 0.5f));
                            reactionSprite.setSpeedTo(actionSprite.speed - (speeddifferent + 3));
                        }
                        else
                        {
                            actionSprite.setSpeedTo(reactionSprite.speed + ((speeddifferent / 2) + 0.5f));
                            reactionSprite.setSpeedTo(actionSprite.speed - (speeddifferent + 3));
                        }
                    }
                    else
                    {
                        actionXspeed = -(Math.Abs(actionXspeed) / actionXspeed);
                        actionSprite.moveX(actionXspeed);
                    }
                }
                //player.accelerate(((player.Rect.Y - sprite.Rect.Y) / 2) * (float)gameTime.ElapsedGameTime.TotalSeconds);
                //Console.WriteLine("Crash");
            }
            return actionXspeed;
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

        sprites.RemoveAll(sprite => sprite.yPos > 1280 || sprite.yPos < -90);
        roadLine.RemoveAll(line => line.Rect.Y > 1000 || line.Rect.Y < 300);

        foreach (Sprite policesprite in polices){

            policesprite.updateObject((float)gameTime.ElapsedGameTime.TotalSeconds, playerSpeed, -player.xPos);

            foreach (Sprite sprite in sprites)
            {
                //-- police controller
                float policeXspeed = 0;
                if (policesprite.yPos > player.yPos && !policesprite.BackendRect.Intersects(sprite.BackendRect))
                {
                    policesprite.setSpeedTo(policesprite.speed + (8 * (float)gameTime.ElapsedGameTime.TotalSeconds));
                }
                else if (policesprite.BackendRect.Intersects(sprite.BackendRect))
                {
                    policesprite.setSpeedTo(policesprite.speed - (40 * (float)gameTime.ElapsedGameTime.TotalSeconds));
                }
                else if (policesprite.speed > (playerSpeed * 0.8))
                {
                    policesprite.setSpeedTo(policesprite.speed - (10 * (float)gameTime.ElapsedGameTime.TotalSeconds));
                }
                else
                {
                    policesprite.setSpeedTo(policesprite.speed + (3 * (float)gameTime.ElapsedGameTime.TotalSeconds));
                }

                // police path finding
                List<List<bool>> map = new List<List<bool>>();
                map.Add(new List<bool>());
                map.Add(new List<bool>());
                Console.WriteLine("" + map.Count);

                //-- police crash physics
                policeXspeed = crashPhysics(policesprite, sprite, policeXspeed, 50, 80);


                
                //-- player controller
                if (sprite is Player playersprite)
                {

                    float fraction = 10f;

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
                        playersprite.accelerate(10 * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    }
                    else if (Keyboard.GetState().IsKeyDown(Keys.S) && playersprite.speed > 0)
                    {
                        playersprite.accelerate(-20 * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    }


                }

                //-- player crash physics
                Xspeed = crashPhysics(player, sprite, Xspeed, 35, 80);
                Xspeed = crashPhysics(player, policesprite, Xspeed, 35, 80);

                policesprite.moveX(policeXspeed);



                sprite.updateObject((float)gameTime.ElapsedGameTime.TotalSeconds, playerSpeed, -player.xPos);

            }
        }
        foreach (Sprite road in roads){
            road.moveMidPoint(-player.xPos);
        }

        foreach (Sprite line in roadLine){
            line.moveMidPoint(-player.xPos);
            line.updateObject((float)gameTime.ElapsedGameTime.TotalSeconds, playerSpeed, player.xPos);
        }

        // foreach (Sprite line in roadLineR){
        //     line.moveMidPoint(-player.xPos);
        // }
        
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
