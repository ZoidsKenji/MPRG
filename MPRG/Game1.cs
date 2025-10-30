using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.DXGI;
using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
using SharpDX.XInput;
using SharpDX.Direct2D1;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using MonoGame;

namespace MPRG;

// other than the c# code (.cs) and the image (of sprite), the rest are built in (or part of the monogame framework porject template)
// so for now The code that I've done is Game1.cs, Player.cs, Road.cs, Sprite.cs and Traffic.cs...
// The .spritefont is code found online for the font and edit by me

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

//commenting example
// ```
// Name : --
// Parameter : --
// Return : --
// Purpose : --
// ```


public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private Microsoft.Xna.Framework.Graphics.SpriteBatch _spriteBatch;
    private SpriteFont spriteFont;
    private SpriteFont bigFont;
    private Vector2 fontPos;
    private float Xaccel = 300;
    private float Xspeed = 0;

    public int wincondition = 0; // 0 = nothing, 1 = win, 2 = lose

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
    List<Sprite> background;

    List<Sprite> roadLine;

    List<Button> mainMenuButtons;
    // List<Sprite> roadLineR;

    private MouseState lastMouseState;
    private KeyboardState lastKeyboardState;

    public float playerSpeed;
    Player player;

    public bool pauseGame = false;
    public bool ingame = false;
    public float TimeMultipier = 1f;
    public (int, int, float) game_time = (0, 0, 0);

    public float spawnCounter = 3;
    public float counter = 1;

    // ai
    GenericAlgorithm GA;
    public bool trainAI = true;
    public bool AiOpponentON = true;
    public string AiDataPath = "saves/gen1";

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    // ```
    // Name : Initialize
    // Parameter : --
    // Return : --
    // Purpose : initialise the game, set the screen size, fps, VSync
    // ```
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

    // ```
    // Name : LoadContent
    // Parameter : --
    // Return : --
    // Purpose : it runs at the very start and loads all the content such as texture, and create class
    // ```
    protected override void LoadContent()
    {
        Random random = new Random();
        game_time.Item1 = random.Next(0, 23);

        _spriteBatch = new Microsoft.Xna.Framework.Graphics.SpriteBatch(GraphicsDevice);
        spriteFont = Content.Load<SpriteFont>("font");
        bigFont = Content.Load<SpriteFont>("Bigfont");

        fontPos = new Vector2(0, 0);

        string AiDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "MPRG", "saves", "gen1"
        );

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
        background = new List<Sprite>();
        mainMenuButtons = new List<Button>();
        // roadLineR = new List<Sprite>();

        Texture2D buttonTexture = Content.Load<Texture2D>("backendTexture");
        mainMenuButtons.Add(new Button(buttonTexture, new Vector2(460, 700), new Vector2(300, 100), "Play", Color.Black * 0.5f, Color.White));
        mainMenuButtons.Add(new Button(buttonTexture, new Vector2(160, 700), new Vector2(300, 100), "Options", Color.Black * 0.5f, Color.White));
        mainMenuButtons.Add(new Button(buttonTexture, new Vector2(760, 700), new Vector2(300, 100), "Quit", Color.Black * 0.5f, Color.White));

        if (AiOpponentON || trainAI)
        {
            GA = new GenericAlgorithm(Content.Load<Texture2D>("S2K"));
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
            background.Add(new Background(Content.Load<Texture2D>("whiteLine")));

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

        if (showbackend)
        {
            backendroads.Add(new Road(Content.Load<Texture2D>("road"), new Vector2(0, 0)));
        }

        // for (int i = 0; i < 320; i++){
        //     sprites.Add(new Road(Content.Load<Texture2D>("road"), new Vector2(0, 320 - (i * 2))));
        // }

        polices.Add(new Police(Content.Load<Texture2D>("RS6Police"), new Vector2(640, 390)));



    }

    // ```
    // Name : Update
    // Parameter : GameTime gameTime
    // Return : --
    // Purpose : it runs every frame and deals with most of the stuff in game, update the sprites, controlls the player.
    // ```
    protected override void Update(GameTime gameTime)
    {
        int specialEntities = 0;
        float TotialSpecialEntSpeed = 0;

        MouseState mouseState = Mouse.GetState();
        Rectangle mouseRect = new(mouseState.X, mouseState.Y, 1, 1);

        game_time.Item3 += TimeMultipier * (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (game_time.Item3 >= 60)
        {
            game_time.Item3 = 0;
            game_time.Item2 += 1;

            if (game_time.Item2 >= 60)
            {
                game_time.Item2 = 0;
                game_time.Item1 += 1;

                if (game_time.Item1 >= 24)
                {
                    game_time = (0, 0, 0);
                }
            }
        }

        if (game_time.Item1 < 6 || game_time.Item1 > 18)
        {
            foreach (Background backg in background)
            {
                backg.backendColour = Color.DarkGreen;
            }

            foreach (Sprite road in roads)
            {
                road.colour = Color.Gray;
            }
            foreach (Sprite line in roadLine)
            {
                line.colour = Color.Gray;
            }
        }
        else
        {
            foreach (Background backg in background)
            {
                backg.backendColour = Color.Green;
            }

            foreach (Sprite road in roads)
            {
                road.colour = Color.White;
            }
            foreach (Sprite line in roadLine)
            {
                line.colour = Color.White;
            }
        }

        // ```
        // Name : crashPhysics
        // Parameter : Sprite actionSprite, Sprite reactionSprite, float actionXspeed, int hitboxWidth, int hitboxHeight
        // Return : (actionXspeed, colision)
        // Purpose : It deals with all the car crash physic by changing the speed / velocity and also deals with the damage
        // ```
        static (float, bool) crashPhysics(Sprite actionSprite, Sprite reactionSprite, float actionXspeed, int hitboxWidth, int hitboxHeight)
        {
            int actionDiv = 20;
            int reactDiv = 10;
            bool colision = false;
            float iFrameAdd = 3;

            if (actionSprite.iFrame == 0 || reactionSprite.iFrame == 0)
            {
                if (reactionSprite.BackendRect.Intersects(actionSprite.BackendRect) && reactionSprite != actionSprite)
                {
                    Console.WriteLine("action " + actionSprite + " : reaction " + reactionSprite);
                    actionSprite.iFrame = iFrameAdd;
                    reactionSprite.iFrame = iFrameAdd;
                    colision = true;
                    float energydifferent = Math.Abs(actionSprite.speed - reactionSprite.speed);
                    //Console.WriteLine(speeddifferent);
                    if (reactionSprite.yPos < actionSprite.yPos)
                    {
                        if ((actionSprite.yPos - reactionSprite.yPos) > hitboxHeight)
                        {
                            if (Math.Abs(actionSprite.BackendRect.X - reactionSprite.BackendRect.X) < hitboxWidth)
                            {
                                actionSprite.setSpeedTo(reactionSprite.speed - ((energydifferent / 4) + 0.5f));
                                actionSprite.health -= (int)energydifferent / actionDiv;
                                reactionSprite.setSpeedTo(reactionSprite.speed + (energydifferent + 3));
                                reactionSprite.health -= (int)energydifferent / reactDiv;
                            }
                            else
                            {
                                actionSprite.setSpeedTo(reactionSprite.speed - ((energydifferent / 4) + 0.5f));
                                actionSprite.health -= (int)energydifferent / actionDiv;
                                reactionSprite.setSpeedTo(reactionSprite.speed + (energydifferent + 3));
                                reactionSprite.health -= (int)energydifferent / reactDiv;
                            }
                        }
                        else
                        {
                            float direction = (actionXspeed != 0) ? (actionXspeed / Math.Abs(actionXspeed)) : 1;
                            actionXspeed = -Math.Abs(actionXspeed) * direction;
                            actionSprite.moveX(actionXspeed);
                            actionSprite.health -= (int)energydifferent / (actionDiv * 2);
                            reactionSprite.health -= (int)energydifferent / (reactDiv * 2);

                        }


                    }
                    else if (reactionSprite.yPos > actionSprite.yPos)
                    {
                        if ((reactionSprite.yPos - actionSprite.yPos) > hitboxHeight)
                        {
                            if (Math.Abs(actionSprite.BackendRect.X - reactionSprite.BackendRect.X) < hitboxWidth)
                            {
                                actionSprite.setSpeedTo(reactionSprite.speed + ((energydifferent / 2) + 0.5f));
                                actionSprite.health -= (int)energydifferent / actionDiv;
                                reactionSprite.setSpeedTo(actionSprite.speed - (energydifferent + 3));
                                reactionSprite.health -= (int)energydifferent / reactDiv;
                            }
                            else
                            {
                                actionSprite.setSpeedTo(reactionSprite.speed + ((energydifferent / 2) + 0.5f));
                                actionSprite.health -= (int)energydifferent / actionDiv;
                                reactionSprite.setSpeedTo(actionSprite.speed - (energydifferent + 3));
                                reactionSprite.health -= (int)energydifferent / reactDiv;
                            }
                        }
                        else
                        {
                            float direction = (actionXspeed != 0) ? (actionXspeed / Math.Abs(actionXspeed)) : 1;
                            actionXspeed = -Math.Abs(actionXspeed) * direction;
                            actionSprite.moveX(actionXspeed);
                            actionSprite.health -= (int)energydifferent / (actionDiv * 2);
                            reactionSprite.health -= (int)energydifferent / (reactDiv * 2);

                        }
                    }
                    //player.accelerate(((player.Rect.Y - sprite.Rect.Y) / 2) * (float)gameTime.ElapsedGameTime.TotalSeconds);
                    //Console.WriteLine("Crash");
                }

            }
            return (actionXspeed, colision);
        }

        if (Keyboard.GetState().IsKeyDown(Keys.P) && lastKeyboardState.IsKeyUp(Keys.P))
        {
            if (pauseGame)
            {
                pauseGame = false;
            }
            else
            {
                pauseGame = true;
            }
        }

        if (!pauseGame && ingame)
        {

            Xaccel = player.speed / 10;
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            spawnCounter += (float)gameTime.ElapsedGameTime.TotalSeconds * playerSpeed;
            counter += (float)gameTime.ElapsedGameTime.TotalSeconds * playerSpeed;
            if (spawnCounter > 300)
            {
                int laneToSpawn = new Random().Next(0, 3);
                if (laneToSpawn != lastlanespawn)
                {
                    sprites.Add(new Traffic(Content.Load<Texture2D>("FITRS"), new Vector2(640, 390), laneToSpawn));
                    lastlanespawn = laneToSpawn;
                }
                else
                {
                    int laneToSpawn2 = new Random().Next(0, 2);
                    if (laneToSpawn2 != lastlanespawn)
                    {
                        sprites.Add(new Traffic(Content.Load<Texture2D>("FITRS"), new Vector2(640, 390), laneToSpawn2));
                        lastlanespawn = laneToSpawn2;
                    }
                }
                spawnCounter = 0;

            }

            if ((counter > 200) && showfrontend)
            {
                for (int i = 0; i < 15; i++)
                {
                    roadLine.Add(new RoadLine(Content.Load<Texture2D>("road"), new Vector2(0, 350), 1));
                }
            }

            sprites.RemoveAll(sprite => sprite.yPos > 2000 || sprite.yPos < -1500);
            roadLine.RemoveAll(line => line.Rect.Y > 1000 || line.Rect.Y < 300);

            foreach (Police policesprite in polices)
            {

                policesprite.updateObject((float)gameTime.ElapsedGameTime.TotalSeconds, playerSpeed, -player.xPos, player.yPos);
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

                    if (sprite is Player && sprite is not AiOpponent && !trainAI)
                    {
                        map[xGrid][yGrid] = 2;
                    }
                    else if (trainAI)
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
                        GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

                        if ((Keyboard.GetState().IsKeyDown(Keys.D) || gamePadState.ThumbSticks.Left.X >= 0.5f) && playersprite.xPos < 500)
                        {
                            Xspeed += Xaccel * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        }
                        else if ((Keyboard.GetState().IsKeyDown(Keys.A) || gamePadState.ThumbSticks.Left.X <= -0.5f) && playersprite.xPos > -500)
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
                            playersprite.accelerate(-1 * (float)gameTime.ElapsedGameTime.TotalSeconds, (float)gameTime.ElapsedGameTime.TotalSeconds, 0, 0);
                        }
                        if (Keyboard.GetState().IsKeyDown(Keys.W) || gamePadState.Triggers.Right > 0)
                        {
                            playersprite.accelerate(30 * (float)gameTime.ElapsedGameTime.TotalSeconds, (float)gameTime.ElapsedGameTime.TotalSeconds, 0.9f, 0);
                        }
                        else if ((Keyboard.GetState().IsKeyDown(Keys.S) || gamePadState.Triggers.Left > 0) && playersprite.speed > 0)
                        {
                            playersprite.accelerate(-50 * (float)gameTime.ElapsedGameTime.TotalSeconds, (float)gameTime.ElapsedGameTime.TotalSeconds, 0, 1);
                        }

                        if (Keyboard.GetState().IsKeyDown(Keys.Up) || gamePadState.Buttons.B == ButtonState.Pressed)
                        {
                            playersprite.GearChange(1);
                        }
                        else if (Keyboard.GetState().IsKeyDown(Keys.Down) || gamePadState.Buttons.X == ButtonState.Pressed)
                        {
                            playersprite.GearChange(-1);
                        }


                    }
                    else if (sprite is AiOpponent ai)
                    {
                        ai.updateCarsPos(sprites);
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
                            ai.Xspeed = ai.Xspeed / 4;
                            ai.moveX(-ai.Xspeed / 10);

                            if (ai.xPos > 550)
                            {
                                ai.xPos = 550;
                            }
                            else
                            {
                                ai.xPos = -550;
                            }
                        }
                        float deadZone = 1.1f;
                        if (ai.xPos > (550 * deadZone) || ai.xPos < (-550 * deadZone))
                        {
                            ai.health -= (int)(10 * (float)gameTime.ElapsedGameTime.TotalSeconds);
                        }

                        ai.DecisionMaking(sprites, (float)gameTime.ElapsedGameTime.TotalSeconds);
                    }

                    //-- player crash physics
                    if (trainAI == (sprite is not AiOpponent))
                    {
                        Xspeed = crashPhysics(player, sprite, Xspeed, 35, 80).Item1;
                    }
                    else if (sprite is not AiOpponent)
                    {
                        Xspeed = crashPhysics(player, sprite, Xspeed, 35, 80).Item1;
                    }

                    Xspeed = crashPhysics(player, policesprite, Xspeed, 35, 80).Item1;

                    policesprite.moveX(policeXspeed);



                    sprite.updateObject((float)gameTime.ElapsedGameTime.TotalSeconds, cameraSpeed, -player.xPos, player.yPos);

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

            foreach (Sprite line in roadLine)
            {
                line.moveMidPoint(-player.xPos);
                line.updateObject((float)gameTime.ElapsedGameTime.TotalSeconds, playerSpeed, player.xPos, player.yPos);
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

            if (trainAI)
            {
                var alivePopulation = GA.population.Where(ai => ai.health > 0).ToList();

                if ((alivePopulation.Count <= 0) && !GA.waitingForNewGen)
                {
                    string AiDataPath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "MPRG", "saves", "gen1"
                    );
                    Console.WriteLine("saving data");
                    //GA.newGen();
                    //GA.waitingForNewGen = true;
                    GA.saveData(AiDataPath);
                    //player.health = 100;
                }

                Console.WriteLine(alivePopulation.Count + "populaton");
                foreach (AiOpponent ai in alivePopulation)
                {
                    Console.WriteLine("aiSpeed" + (int)ai.speed + " aiHealth" + ai.health + " aiXpos" + ai.xPos + " aiYpos" + ai.yPos + " aiXspeed " + ai.Xspeed + " aiScore" + ai.score);
                }

                if (player.health <= 0 && wincondition == 0)
                {
                    wincondition = 2;
                    string AiDataPath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "MPRG", "saves", "gen1"
                    );
                    Console.WriteLine("saving data");
                    GA.saveData(AiDataPath);
                }
                else if (alivePopulation.Count == 0 && wincondition == 0)
                {
                    wincondition = 1;
                }
            }
        }
        else if (!ingame)
        {

            foreach (Button button in mainMenuButtons)
            {
                if (mouseRect.Intersects(button.Rect))
                {
                    button.colour = Color.White;
                    button.textcolour = Color.Black;

                    if (mouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released)
                    {

                    }
                }
                else
                {
                    button.colour = Color.Black * 0.5f;
                    button.textcolour = Color.White;
                }
            }


        }

        lastMouseState = mouseState;
        lastKeyboardState = Keyboard.GetState();

        base.Update(gameTime);
    }

    // ```
    // Name : Draw
    // Parameter : GameTime gameTime
    // Return : --
    // Purpose : it runs every frame, it draw and display the screen. Also deals with the layer of drawing sprite.
    // ```
    protected override void Draw(GameTime gameTime)
    {
        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        if (ingame)
        {
            Color glitchLeft = Color.Aqua;
            int glitchPosDif = 2;
            Color glitchRight = Color.DeepPink;

            if (game_time.Item1 < 6 || game_time.Item1 > 18)
            {
                byte redValue = 20;
                byte greenValue = 23;
                byte blueValue = 72;
                Color darkBlue = new Color(redValue, greenValue, blueValue);
                GraphicsDevice.Clear(darkBlue); //MidnightBlue
            }
            else
            {
                GraphicsDevice.Clear(Color.CornflowerBlue);

            }

            allSprites = [.. sprites, .. polices];

            sprites.Sort((a, b) => a.Rect.Y.CompareTo(b.Rect.Y));

            allSprites.Sort((a, b) => a.Rect.Y.CompareTo(b.Rect.Y));



            if (showfrontend)
            { //show front end(ray casting)
              // _spriteBatch.Draw(road, new Rectangle(400, 400, 100, 200), Color.White);
              // _spriteBatch.Draw(mrs, new Rectangle(400, 400, 282, 190), Color.White);

                foreach (Sprite back in background)
                {
                    _spriteBatch.Draw(back.texture, back.BackendRect, back.backendColour);
                }

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

            string healthHUD = "Health: " + player.health.ToString();
            string speedHUD = "Speed: " + ((int)(player.speed / 3f)).ToString();
            string gearHUD = "Gear: " + ((int)player.gear).ToString();
            string rpmHUD = "Rpm: " + ((int)player.rpm).ToString();
            string timeHUD = "Time: " + game_time.Item1 + " : " + game_time.Item2 + " : " + (int)game_time.Item3;

            List<AiOpponent> ingameplayers = GA.population.Where(ai => ai.health > 0).ToList();
            int playerPos = 1;
            foreach(AiOpponent ai in ingameplayers)
            {
                if (ai.yPos < player.yPos)
                {
                    playerPos++;
                }
            }

            string positionHUD = "Pos: " + playerPos.ToString() + " / " + (ingameplayers.Count + 1).ToString();

            if (wincondition != 0)
            {
                string wincon = "Game Over";
                if (wincondition == 1)
                {
                    wincon = "You Win!!!";
                    Console.WriteLine("win");
                }
                else
                {
                    Console.WriteLine("lose");
                    wincon = "Game Over";
                }

                Rectangle rectangle = new Rectangle(460, 300, 400, 90);
                _spriteBatch.Draw(backendTexture, rectangle, Color.Blue * 0.2f);
                rectangle = new Rectangle(400, 305, 520, 80);
                _spriteBatch.Draw(backendTexture, rectangle, Color.Blue * 0.2f);
                rectangle = new Rectangle(360, 310, 600, 70);
                _spriteBatch.Draw(backendTexture, rectangle, Color.Blue * 0.2f);
                rectangle = new Rectangle(330, 315, 660, 60);
                _spriteBatch.Draw(backendTexture, rectangle, Color.Blue * 0.2f);

                Vector2 WinFontOrigin = new Vector2();
                Vector2 winTextPos = new Vector2();
                winTextPos.X = 480;
                winTextPos.Y = 960;
                WinFontOrigin.X = -503;
                WinFontOrigin.Y = -300;
                _spriteBatch.DrawString(bigFont, wincon, fontPos, glitchLeft, 0, WinFontOrigin, 1.0f, SpriteEffects.None, 0.5f);
                WinFontOrigin.X = -497;
                _spriteBatch.DrawString(bigFont, wincon, fontPos, glitchRight, 0, WinFontOrigin, 1.0f, SpriteEffects.None, 0.5f);
                WinFontOrigin.X = -500;
                _spriteBatch.DrawString(bigFont, wincon, fontPos, Color.White, 0, WinFontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            }

            if (pauseGame && ingame)
            {
                Vector2 WinFontOrigin = new Vector2();
                Vector2 winTextPos = new Vector2();
                winTextPos.X = 480;
                winTextPos.Y = 0;
                WinFontOrigin.X = -540;
                WinFontOrigin.Y = -5;
                _spriteBatch.DrawString(spriteFont, "Game Paused", fontPos, Color.Red, 0, WinFontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            }

            Vector2 FontOrigin = new Vector2();//spriteFont.MeasureString(healthHUD) / 2;
            FontOrigin.X = -5 - glitchPosDif;
            FontOrigin.Y = -5;
            _spriteBatch.DrawString(spriteFont, healthHUD, fontPos, glitchRight, 0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            FontOrigin.X = -5 + glitchPosDif;
            FontOrigin.Y = -5;
            _spriteBatch.DrawString(spriteFont, healthHUD, fontPos, glitchLeft, 0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            FontOrigin.X = -5;
            FontOrigin.Y = -5;
            _spriteBatch.DrawString(spriteFont, healthHUD, fontPos, Color.White, 0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);

            FontOrigin.X = -5 - glitchPosDif;
            FontOrigin.Y = -25;
            _spriteBatch.DrawString(spriteFont, speedHUD, fontPos, glitchRight, 0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            FontOrigin.X = -5 + glitchPosDif;
            FontOrigin.Y = -25;
            _spriteBatch.DrawString(spriteFont, speedHUD, fontPos, glitchLeft, 0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            FontOrigin.X = -5;
            FontOrigin.Y = -25;
            _spriteBatch.DrawString(spriteFont, speedHUD, fontPos, Color.White, 0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);

            FontOrigin.X = -5 - glitchPosDif;
            FontOrigin.Y = -45;
            _spriteBatch.DrawString(spriteFont, gearHUD, fontPos, glitchRight, 0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            FontOrigin.X = -5 + glitchPosDif;
            FontOrigin.Y = -45;
            _spriteBatch.DrawString(spriteFont, gearHUD, fontPos, glitchLeft, 0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            FontOrigin.X = -5;
            FontOrigin.Y = -45;
            _spriteBatch.DrawString(spriteFont, gearHUD, fontPos, Color.White, 0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);

            FontOrigin.X = -5 - glitchPosDif;
            FontOrigin.Y = -65;
            _spriteBatch.DrawString(spriteFont, rpmHUD, fontPos, glitchRight, 0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            FontOrigin.X = -5 + glitchPosDif;
            FontOrigin.Y = -65;
            _spriteBatch.DrawString(spriteFont, rpmHUD, fontPos, glitchLeft, 0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            FontOrigin.X = -5;
            FontOrigin.Y = -65;
            _spriteBatch.DrawString(spriteFont, rpmHUD, fontPos, Color.White, 0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);

            FontOrigin.X = -1100 - glitchPosDif;
            FontOrigin.Y = -5;
            _spriteBatch.DrawString(spriteFont, timeHUD, fontPos, glitchRight, 0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            FontOrigin.X = -1100 + glitchPosDif;
            FontOrigin.Y = -5;
            _spriteBatch.DrawString(spriteFont, timeHUD, fontPos, glitchLeft, 0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            FontOrigin.X = -1100;
            FontOrigin.Y = -5;
            _spriteBatch.DrawString(spriteFont, timeHUD, fontPos, Color.White, 0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);

            FontOrigin.X = -1100 - glitchPosDif;
            FontOrigin.Y = -25;
            _spriteBatch.DrawString(spriteFont, positionHUD, fontPos, glitchRight, 0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            FontOrigin.X = -1100 + glitchPosDif;
            FontOrigin.Y = -25;
            _spriteBatch.DrawString(spriteFont, positionHUD, fontPos, glitchLeft, 0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            FontOrigin.X = -1100;
            FontOrigin.Y = -25;
            _spriteBatch.DrawString(spriteFont, positionHUD, fontPos, Color.White, 0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);

        }
        else
        {
            byte redValue = 33;
            byte greenValue = 9;
            byte blueValue = 72;
            Color darkBlue = new Color(redValue, greenValue, blueValue);
            GraphicsDevice.Clear(darkBlue); 

            Color glitchLeft = Color.Aqua;
            int glitchPosDif = 2;
            Color glitchRight = Color.DeepPink;

            Vector2 WinFontOrigin = new Vector2();
            Vector2 winTextPos = new Vector2();
            winTextPos.X = 480;
            winTextPos.Y = 960;
            int xpostemp = -130;
            WinFontOrigin.X = xpostemp - glitchPosDif;
            WinFontOrigin.Y = -40;
            _spriteBatch.DrawString(bigFont, "MPRG", fontPos, glitchLeft, 0, WinFontOrigin, 3.0f, SpriteEffects.None, 0.5f);
            WinFontOrigin.X = xpostemp + glitchPosDif;
            _spriteBatch.DrawString(bigFont, "MPRG", fontPos, glitchRight, 0, WinFontOrigin, 3.0f, SpriteEffects.None, 0.5f);
            WinFontOrigin.X = xpostemp;
            _spriteBatch.DrawString(bigFont, "MPRG", fontPos, Color.White, 0, WinFontOrigin, 3.0f, SpriteEffects.None, 0.5f);

            Vector2 FontOrigin = new Vector2();
            FontOrigin.X = -440;
            FontOrigin.Y = -340;
            _spriteBatch.DrawString(spriteFont, "Motorway Pursuit Racing Game", fontPos, Color.White, 0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);

            foreach(Button button in mainMenuButtons)
            {
                _spriteBatch.Draw(button.texture, button.Rect, button.colour);
                _spriteBatch.DrawString(spriteFont, button.text, new Vector2(button.pos.X + (button.size.X / 2) - 35, button.pos.Y + (button.size.Y / 2) - 30), button.textcolour, 0, new Vector2(0, 0), 1.5f, SpriteEffects.None, 0.5f);

            }
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

}
