using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace MPRG
{
    internal class Player : Sprite
    {
        // these value are for toyota mrs
        public float rpm = 800;
        public float redLine = 6500;
        public float rpmLim = 7500;
        public float idleRpm = 800;
        public List<float> gearRatio = new List<float> { 3.230f, 1.913f, 1.258f, 0.918f, 0.731f };
        public List<float> torque = new List<float> { 60, 70, 120, 160, 171, 170, 160, 130, 120 }; // for every 1000 rpm in Nm
        public float finalDriveRatio = 4.285f;
        public float tyreCircumference = 1.893f;
        public float gear = 1;

        public bool manualGear = false;
        public float gearFrame = 0;

        public float dragCoefficient = 0.28f;
        public float frontalArea = 1.8f;

        public float rollingResistanceCoefficient = 0.007f;

        public float brakingForce = 200;

        // rpm equation:
        // rpm = (speed * gearRatio * finalDriveRatio * 60) / tyreCircumference
        // speed = (rpm * tyreCircumference) / (gearRatio * finalDriveRatio * 60)

        public override Rectangle Rect
        {
            get
            {
                return new Rectangle((int)pos.X, (int)pos.Y, 300, 300);
            }
        }

        public override Rectangle BackendRect
        {
            get
            {
                return new Rectangle((int)(xPos * 0.23) + (150 - 35), (int)yPos, 70, 90);
            }
        }

        public Player(Texture2D texture, Vector2 pos) : base(texture, pos)
        {
            this.backendColour = Color.LightGray;
            this.yPos = 435;
            this.speed = 80;

            this.rpm = 800;
            this.redLine = 6500;
            this.rpmLim = 7500;
            this.idleRpm = 800;
            this.gearRatio = new List<float> { 3.230f, 1.913f, 1.258f, 0.918f, 0.731f };
            this.torque = new List<float> { 60, 70, 120, 160, 171, 170, 160, 130, 120 }; // for every 1000 rpm in Nm
            this.finalDriveRatio = 4.285f;
            this.tyreCircumference = 1.893f;
            this.gear = 1;
            this.mass = 1050; // in kg

        }

        public virtual void accelerate(float accel, float time, float throttle, float brake)
        {
            // Engine & Gear
            float momentOfInertia = 0.18f;
            float viscousDampingCoefficent = 0.05f;
            double pi = Math.PI;
            float rpmtorque = torque[(int)rpm / 1000] * throttle;
            float viscousLoss = viscousDampingCoefficent * ((rpm * 2 * (float)pi) / 60); // (rpm * 2 * (float)pi) / 60 is the angular speed
            // braking
            float brakeTorque = brakingForce * brake * (float)(tyreCircumference / (2 * pi));
            float crankBrakeTorque = -brakeTorque / (gearRatio[(int)gear - 1] * finalDriveRatio * 0.95f);
            float brakeNetTorque = rpmtorque + crankBrakeTorque - viscousLoss;
            // total rpm
            float netTorque = rpmtorque + brakeNetTorque - viscousLoss;
            float angularAccel = netTorque / momentOfInertia;
            float rpmPerSec = angularAccel * 60 / (2 * (float)pi);
            rpm += rpmPerSec * time;
            

            // drag (air resistance)
            float wheelTorque = rpmtorque * gearRatio[(int)gear - 1] * finalDriveRatio * 0.95f; // 0.95 is drive train lost
            float engineForce = wheelTorque / (float)(tyreCircumference / (2 * pi));

            float rollingResistance = rollingResistanceCoefficient * mass * 9.81f;
            float dragForce = airDens * frontalArea * dragCoefficient * speed * speed * 0.5f;

            float longitudinalAcceleration = (engineForce - dragForce - rollingResistance - brakeTorque) / mass;

            speed = ((rpm * tyreCircumference) / (gearRatio[(int)gear - 1] * finalDriveRatio * 60)) * 3f * 2.237f; // the 2.237 makes it mph
            

            Console.WriteLine("playerSpeed" + speed + " playerHealth" + health + " playerXpos" + xPos);
            // if (speed < 0)
            // {
            //     speed = 10;
            // }
        }

        public override void setSpeedTo(float Speed)
        {
            speed = Speed;
            if (speed < 0)
            {
                speed = 10;
            }
        }

        // ```
        // Name : updateObject
        // Parameter : float time, float camSpeed, float midPointX
        // Return : --
        // Purpose : it override from Sprite class, and updates the sprite
        // ```
        public override void updateObject(float time, float camSpeed, float midPoint, float playerY)
        {
            this.yPos += (camSpeed - speed) * time;
            if (health < 1)
            {
                //this.yPos = 1001;
                speed = 0;
            }

            if (iFrame > 0)
            {
                iFrame -= 1;
            }
            else
            {
                iFrame = 0;
            }

            if (gearFrame > 0)
            {
                gearFrame -= 1;
            }
            else
            {
                gearFrame = 0;
            }

            if (rpm > idleRpm)
            {
                // float momentOfInertia = 0.18f;
                // float viscousDampingCoefficent = 0.05f;
                // double pi = Math.PI;
                // float viscousLoss = viscousDampingCoefficent * ((rpm * 2 * (float)pi) / 60); // (rpm * 2 * (float)pi) / 60 is the angular speed
                // float netTorque = -viscousLoss;
                // float angularAccel = netTorque / momentOfInertia;
                // float rpmPerSec = angularAccel * 60 / (2 * (float)pi);
                // rpm += rpmPerSec * time;
            }
            else
            {
                rpm = 810;
            }

            if (rpm > rpmLim)
            {
                rpm = rpmLim;
            }

            if (!manualGear)
            {
                if (rpm >= redLine && gear < gearRatio.Count)
                {
                    gear += 1;
                    rpm = (rpm * gearRatio[(int)gear - 1]) / gearRatio[(int)gear - 2];
                }
                else if (rpm < 1500 && gear > 1)
                {
                    gear -= 1;
                    rpm = (rpm * gearRatio[(int)gear - 1]) / gearRatio[(int)gear];
                }
            }
        }

        public void GearChange(int gearUp)
        {
            if (manualGear && gearFrame == 0)
            {
                if (gearUp == 1 && gear < gearRatio.Count)
                {
                    gear += 1;
                    rpm = (rpm * gearRatio[(int)gear - 1]) / gearRatio[(int)gear - 2];
                    gearFrame = 120;
                }
                else if (gearUp == -1 && gear > 1)
                {
                    gear -= 1;
                    rpm = (rpm * gearRatio[(int)gear - 1]) / gearRatio[(int)gear];
                    gearFrame = 120;

                }
            }
        }

    }

}