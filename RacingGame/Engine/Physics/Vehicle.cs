/*
 * This class is used to simulate a vehicle's movement. This class is the super class of all vehicles
 * 
 * It uses a gearbox and the motor in order to work
*/
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using RacingGame.Dashboards;
using RacingGame.Engine;
using RacingGame.Engine.HelperFunctions;
using RacingGame.Enviroment;
using RacingGame.Engine.TerrainLoader;
using RacingGame.PlayerData;
using RacingGame.Engine.Audio;
using RacingGame.Engine.PaticleEngine;

namespace RacingGame.Engine.Physics
{
    abstract class Vehicle
    {
        #region varriables

        //protected data members
        //current graphics device and the car model provided
        protected GraphicsDeviceManager graphics;
        protected Model carModel;
        protected Gauges carGauge;
        protected int player;
        protected bool drawSpheres;

        //constatant componenets
        protected const float height = 0.0f;
        protected const float rotationRate = 1.5f;
        protected const float mass = 1.0f;
        protected const float resistance = 0.999f;
        protected const float brakeResistance = 0.985f;
        protected const float handBrakeResistance = 0.98f;
        protected const float reverseResistance = 0.99f;
        protected const float airResistance = 0.98f;
        protected const float gravity = 0.975f;
        protected const float colistionResistance = 0.98f;
        
        protected BoundingSphere[] waypointSpheres;
        protected int point;

        //calculation componenets
        protected float maxRotation = 45.0f;
        protected float thrustAmount = 0.0f;
        protected float elapsedTime;
        protected float aspectRatio;
        protected float currentRotation;
        protected float carScale;
        protected float speed;
        protected float slipFactor;
        protected float heightOffset;
        protected float collisionHeight;
        protected bool reversing;

        // NoS variables
        protected bool nos = false;
        protected bool nosActive = false;
        protected float nosAmmount = 300;
        protected float maxSpeed = 150;

        //physics componenets
        protected Vector3 force;
        protected Vector3 acceleration;
        protected Vector3 velocity;
        protected Vector3 position;
        protected Vector3 previousPosition;

        protected Vector3 direction;
        protected Vector3 previousDirection;
        protected Vector3 normalvector;
        protected Vector3 rightVector;

        //reset variables
        protected Vector3 resetposition;
        protected Vector3 resetDirection;
        protected Vector3 resetRight;
        protected Vector3 resetNormal;
        protected int resetCounter;

        //Collision Detection boolean and counter
        protected bool collisionActive = false;
        protected bool heightMapCollision = false;
        protected float maxCollisionHeight = 51;

        //collision Detection Vectors
        protected Vector3 previousNormal;
        protected Vector3 newNormal;

        //transforms
        protected Matrix transformationMatrix;
        protected Matrix rotationMatrix;
        protected Matrix orientation;

        //current motor in the car
        protected CarMotor currentMotor;

        //input handeling
        protected InputHandler currentState;

        //heightmap to be used
        protected HeightMapGenerator currentHeightMap;

        //collision volumes
        protected BoundingBox collisionBox;
        protected BoundingSphere collisionSphere;

        //sound effect manager
        protected bool enableSound;
        protected SoundEffectPlayer sound;

        //if off track
        protected bool offTrack = false;

        //smoke particle generator
        protected SmokePlumeParticleSystem smoke;

        //rotation data
        protected float carGlobalRotation = 0;

        //wheel data  
        protected ModelBone frontWheelLeft;
        protected ModelBone frontWheelRight;
        protected ModelBone backWheelLeft;
        protected ModelBone backWheelRight;

        #endregion

        #region Getters and Setters

        //getters and setters
        public bool Nos
        {
            get { return nos; }
            set { nos = value; }
        }

        public bool NosActive
        {
            get { return nosActive; }
            set { nosActive = value; }
        }

        public float NosAmmount
        {
            get { return nosAmmount; }
            set { nosAmmount = value; }
        }

        public void AdjustVolume(float volume)
        {
            sound.AdjustVolume(volume);
        }

        public void PlayCollisionSound()
        {
            sound.playCollisionSound(true);
        }

        public Model CarModel
        {
            get { return carModel; }
            set { carModel = value; }
        }

        public bool CollisionActive
        {
            get { return collisionActive; }
            set { collisionActive = value; }
        }

        public BoundingSphere[] Waypoints
        {
            set { waypointSpheres = value; }
        }

        public Vector3 Force
        {
            get { return force; }
        }

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }
        public float CollisionHeight
        {
            get { return collisionHeight; }
            set { collisionHeight = value; }
        }

        public Vector3 PreviousPosition
        {
            get { return previousPosition; }
            set { previousPosition = value; }
        }

        public Vector3 Velocity
        {
            get { return velocity; }
        }

        public Vector3 Acceleration
        {
            get { return acceleration; }
        }

        public Vector3 Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        public Vector3 Normal
        {
            get { return normalvector; }
        }

        public Vector3 Right
        {
            get { return rightVector; }
        }

        public Matrix TransformationMatrix
        {
            get { return transformationMatrix; }
        }

        public Matrix RotationMatrix
        {
            get { return rotationMatrix; }
        }

        public CarMotor Motor
        {
            get { return currentMotor; }
        }

        public float CarScale
        {
            get { return carScale; }
        }

        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }

        public float Rotation
        {
            get { return currentRotation; }
        }

        public float SlipFactor
        {
            get { return slipFactor; }
        }

        public float Scale
        {
            get { return carScale; }
        }

        public int Point
        {
            get { return point; }
            set { point = value; }
        }

        public float HeightOffset
        {
            get { return heightOffset; }
            set { heightOffset = value; }
        }

        public int Player
        {
            get { return player; }
            set { player = value; }
        }
        public bool DrawSpheres
        {
            get { return drawSpheres; }
            set { drawSpheres = value; }
        }

        public bool HeightMapCollision
        {
            get { return heightMapCollision; }
            set { heightMapCollision = value; }
        }

        public float MaxCollisionHeight
        {
            set { maxCollisionHeight = value; }
        }

        public bool OffTrack
        {
            get { return offTrack; }
        }

        public InputHandler Input
        {
            set { currentState = value; }
        }

        //collision volumes
        public BoundingSphere Sphere
        {
            get
            {
                collisionSphere.Center = position / 20;
                return collisionSphere;
            }
        }

        public BoundingBox Box
        {
            get { return UpdateBoundingBox(); }
        }

        public SoundEffectPlayer Sound
        {
            get { return sound; }
        }
        //end of getters and setters

        public Vehicle(ref GraphicsDeviceManager device, Vector3 start, InputHandler input)
        {
            graphics = device;
            position = start;
            currentState = input;

            aspectRatio = (float)graphics.GraphicsDevice.Viewport.Width / graphics.GraphicsDevice.Viewport.Height;
            reset();
        }

        public void reset()
        {
            //position = new Vector3(0, height, 0);
            direction = Vector3.Forward;
            normalvector = Vector3.Up;
            rightVector = Vector3.Right;
            velocity = Vector3.Zero;
            force = Vector3.Zero;
            speed = 0.0f;
        }

        public void resetPosition()
        {
            position = resetposition;
            direction = resetDirection;
            rightVector = resetRight;
            normalvector = resetNormal;
            speed = 0f;
        }

        //reset gears to neutral
        public void resetGears()
        {
            velocity = Vector3.Zero;
            force = Vector3.Zero;
            speed = 0.0f;
            currentMotor.Throttle = 0;
            currentMotor.GearBoxUsed.CurrentGear = 0;
        }
        #endregion

        //update vehice movement
        virtual public void update(GameTime gameTime, bool canRace, bool canPlaySound)
        {
            //update particle system
            smoke.Update(gameTime);

            if (currentState.NosActivate)
            {
                if (nosAmmount > 0)
                    nosActive = true;
            }
            else
                nosActive = false;

            if (speed == 0)
                currentRotation = 0;

            maxSpeed = 150 + (50 * currentMotor.GearBoxUsed.CurrentGear);


            if ((nosActive && currentState.IsAccelerating) && (nosAmmount > 0) && nos)
            {
                if (speed <= maxSpeed)
                    speed += 1.0f;
                else
                    speed = maxSpeed + 5f;

                nosAmmount--;

                if (enableSound)
                    sound.playNOSSound(true);
            }

            else
                sound.playNOSSound(false);

            if ((speed < 5 && speed > -5) && (!currentState.IsAccelerating && !currentState.IsDecelerating) && (currentState.IsTurningLeft || currentState.IsTurningRight))
                speed = 0;

            #region Handle Movement
            //enable sound for the vehicle
            enableSound = canPlaySound;

            //update input
            currentState.update(Keyboard.GetState(), gameTime);
            currentMotor.GearBoxUsed.keyBoardState = currentState;
            elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //slip factor used for car slip
            slipFactor = 0;

            //can race is used for the race countdown to enable racing
            currentMotor.GearBoxUsed.CanChangeGear = canRace;

            //rotation of the car
            Vector2 rotationAmount = Vector2.Zero;

            //turning left or right
            if (currentState.IsTurningLeft && (speed > 5 || speed < -5))
            {
                float temp = maxRotation;

                if (currentRotation < 0)
                    currentRotation = 0;

                if (speed > 35 || speed < -35)
                {
                    if (currentRotation < maxRotation)
                        currentRotation += 1.0f;
                    else if (currentRotation <= 0)
                        currentRotation -= 1.0f;

                    rotationAmount.X = currentRotation / maxRotation;
                    rotationAmount.X *= (speed / Math.Abs(speed));
                }

                else
                {
                    maxRotation = Math.Abs(speed);
                    if (currentRotation >= maxRotation)
                        currentRotation = maxRotation;
                    else if (currentRotation < maxRotation)
                        currentRotation += 0.5f;
                    else if (currentRotation <= 0)
                        currentRotation -= 0.5f;

                    rotationAmount.X = currentRotation / maxRotation;
                    rotationAmount.X *= (speed / Math.Abs(speed));
                }

                maxRotation = temp;
            }

            else if (currentState.IsTurningRight && (speed > 5 || speed < -5))
            {
                float temp = maxRotation;

                if (currentRotation > 0)
                    currentRotation = 0;

                if (speed > 35 || speed < -35)
                {
                    if (currentRotation > -maxRotation)
                        currentRotation -= 1.0f;
                    else if (currentRotation < -maxRotation)
                        currentRotation = -maxRotation;

                    rotationAmount.X = currentRotation / maxRotation;
                    rotationAmount.X *= (speed / Math.Abs(speed));
                }

                else
                {
                    maxRotation = Math.Abs(speed);
                    if (currentRotation <= -maxRotation)
                        currentRotation = -maxRotation;

                    else if (currentRotation > -maxRotation)
                        currentRotation -= 0.5f;
                    else if (currentRotation <= -maxRotation)
                        currentRotation = -maxRotation;

                    rotationAmount.X = currentRotation / maxRotation;
                    rotationAmount.X *= (speed / Math.Abs(speed));
                }

                maxRotation = temp;
            }

            //if the car is not being rotated, damp the rotation down to 0
            if (!(currentState.IsTurningLeft || currentState.IsTurningRight))
                currentRotation *= 0.90f;

            //if car is being rotated, and the rotation at a particular speed is to large, let the car slip and lose speed
            else
            {
                slipFactor = 0;

                if (Math.Abs(speed) < 1)
                    slipFactor = 0;

                else
                {
                    if (currentRotation != 0 && !(nosActive && (nosAmmount > 0) && Nos))
                    {
                        slipFactor = mass * Math.Abs(speed) * 0.00020f;

                        if (currentState.HandBrakeEngaged)
                            slipFactor *= 3.0f;

                        slipFactor = Math.Min(slipFactor, 0.91f);

                        currentRotation *= 1 - slipFactor;
                    }
                }
            }

            //convert rotation ammount to radians per second
            rotationAmount = rotationAmount * rotationRate * elapsedTime + (rotationAmount * slipFactor);

            if (!currentState.HandBrakeEngaged)
            {
                //sound of handbrake disable
                if (enableSound && speed > 100)
                    sound.playSlidingSound(false);
                else if (enableSound && speed < 50)
                    sound.playSlidingSound(false);

                if (currentState.IsAccelerating && reversing)
                    currentMotor.Throttle = 1.0f;
                else if (currentState.IsAccelerating)
                {
                    currentMotor.Throttle = 1.0f;
                    if (speed < -20)
                        speed *= brakeResistance;
                }
                else if (!currentState.IsAccelerating)
                    currentMotor.Throttle = 0.0f;
            }
            else
            {
                //sound of handbrake enable
                if (enableSound && speed > 100)
                {
                    sound.playSlidingSound(true);

                    //rear wheel
                    smoke.AddParticle(new Vector3(Position.X * 0.05f, Position.Y * 0.05f, Position.Z * 0.05f), Vector3.Zero);
                }
                else if (enableSound && speed < 50)
                    sound.playSlidingSound(false);

                currentMotor.Throttle = 0.0f;
            }

            //if car is in reverse and the throttle is down, increase speed
            if (currentState.IsDecelerating && currentMotor.GearBoxUsed.CurrentGear == -1)
            {
                currentMotor.Throttle = 1.0f;
                if (speed > 20)
                    speed *= brakeResistance;
                else if (!reversing)
                {
                    currentRotation = 0;
                    reversing = true;
                }
            }
            else if (currentState.IsDecelerating && !(currentMotor.GearBoxUsed.CurrentGear == -1))
            {
                currentMotor.Throttle = 0.0f;
                speed *= brakeResistance;
            }

            //if car is not accelerating or decelerating, throttle is released and apply resistance of brakes
            else if (!currentState.IsDecelerating && !currentState.IsAccelerating)
            {
                currentMotor.Throttle = 0.0f;
                speed *= resistance;
            }

            //current thrust applied
            thrustAmount = 0.0f;

            //add resistance
            if (currentMotor.Throttle == 0.0f)
                speed *= resistance;

            //clamp  speed
            if (speed < 0.99 && speed > 0 || speed > -0.99 && speed < 0)
                speed = 0;

            //if handbrake is engaged appy resistance
            if (currentState.HandBrakeEngaged)
                speed *= handBrakeResistance;

            //sound of turning
            if (enableSound && speed > 100 && Math.Abs(currentRotation) > 15)
                sound.playSlidingSound(true);
            else if (enableSound && speed < 50)
                sound.playSlidingSound(false);

            thrustAmount += currentMotor.CurrentPower / 5;

            rotationMatrix = Matrix.CreateRotationY(rotationAmount.X);


            // modelHalfHeight is meant to represent distance from fbx origin to feet plane
            Vector3 tocamera = position - previousPosition;
            //Vector3 pointToRotateAround = new Vector3(10,10,10);
            Vector3 pointToRotateAround = rotationMatrix.Translation +
                              rotationMatrix.Right * (100 * tocamera);


            //create rotation matrix

            rotationMatrix.Translation -= pointToRotateAround;
            rotationMatrix *= Matrix.CreateFromAxisAngle(rightVector, rotationAmount.Y);// * Matrix.CreateRotationY(rotationAmount.X);//angle is the angle to rotate just this frame only
            rotationMatrix.Translation += pointToRotateAround;

            //rotationMatrix = Matrix.CreateFromAxisAngle(rightVector, rotationAmount.Y) * Matrix.CreateRotationY(rotationAmount.X);






            //change direction to the rotated direction
            previousDirection = direction;

            //direction = Vector3.TransformNormal(direction, rotationMatrix);
            direction = Vector3.TransformNormal(direction, Matrix.CreateFromAxisAngle(normalvector, rotationAmount.Y) * Matrix.CreateRotationY(rotationAmount.X));
            //change the normal vector based on the rotation
            normalvector = Vector3.TransformNormal(normalvector, rotationMatrix);

            //normalize vectors otherwise rounding error could occur which could break the movement
            direction.Normalize();
            normalvector.Normalize();

            rightVector = Vector3.Cross(direction, normalvector);

            // The same instability may cause the 3 orientation vectors may
            // also diverge. Either the Up or Direction vector needs to be
            // re-computed with a cross product to ensure orthagonality
            normalvector = Vector3.Cross(rightVector, direction);

            orientation.Right = rightVector;
            orientation.Forward = direction;
            orientation.Up = normalvector;

            //calculate the force
            force = direction * thrustAmount * -1;
            //calculate the acceleration
            acceleration = force / mass;
            //calculate the velocity
            velocity += acceleration * elapsedTime;
            //calculate resistance
            velocity *= resistance;

            float speedApplyFactor = 1;

            if (acceleration.Length() > 0)
                speedApplyFactor = Vector3.Dot(Vector3.Normalize(acceleration), direction * -1.0f);

            speed += acceleration.Length() * speedApplyFactor;

            if (slipFactor > 0.03)
            {
                speed *= 0.986f;

                //rear wheel
                smoke.AddParticle(new Vector3(Position.X * 0.05f, Position.Y * 0.05f, Position.Z * 0.05f), Vector3.Zero);
            }
            #endregion

            #region UpdateMatrices
            //apply velocity to the model

            //reset data
            if (resetCounter >= 100 && position.Y == 4)
            {
                resetposition = position;
                resetRight = rightVector;
                resetNormal = normalvector;
                resetDirection = direction;

                resetCounter = 0;
            }
            resetCounter++;
            previousPosition = position;
            position += (direction * -1.0f) * elapsedTime * speed;
            previousNormal = normalvector;

            //if car is on the heightmap, change the orientation of the car based on the tile normal
            if (currentHeightMap.IsOnHeightMap(position, carScale, 20))
            {
                Vector3 normal;
                Vector3 originalPosition = position;
                currentHeightMap.GetHeightAndNormal(position, out position.Y, out normal, carScale);

                if (originalPosition.Y - position.Y > 0.1f)
                {
                    if (speed > 60)
                    {
                        position.Y = originalPosition.Y * gravity;

                        if (position.Y - originalPosition.Y < 0.1f || position.Y - originalPosition.Y > -0.1f)
                            originalPosition.Y = position.Y;// = originalPosition.Y;

                        speed = speed * airResistance;
                    }
                }

                orientation.Up = normal;
                if (speed > 0 || speed < 0)
                {
                    orientation.Forward = (position - previousPosition) / (speed * 0.0167f);
                    direction = orientation.Forward;
                }
                else
                {
                    orientation.Forward = -1 * direction;
                }
                orientation.Right = Vector3.Cross(orientation.Up, orientation.Forward);
                direction = -1 * orientation.Forward;
                normalvector = (orientation.Up);
                rightVector = orientation.Right;
            }

            newNormal = normalvector;

            //collision with the collision map
            if (heightMapCollision)
            {
                collisionHeight = currentHeightMap.getCollision(position);

                if (collisionHeight >= maxCollisionHeight)
                {
                    //sound.playCollisionSound(true);
                    if (speed > 20)
                    {
                        speed *= colistionResistance;
                        currentMotor.GearBoxUsed.CurrentGear = 0;

                    }
                    else
                    {
                        speed = 20.0f;
                    }
                    offTrack = true;
                }
                else
                    offTrack = false;
            }

            //set the model transformation matrix
            transformationMatrix = Matrix.Identity;
            transformationMatrix.Forward = direction;
            transformationMatrix.Up = normalvector;
            transformationMatrix.Right = rightVector;
            transformationMatrix.Translation = position;

            transformationMatrix = transformationMatrix * Matrix.CreateScale(carScale, carScale, carScale);

            //enable engine sounds
            if (enableSound)
                sound.UpdateEngineSounds();

            //update motor
            currentMotor.update(gameTime, speed);
            #endregion
        }

        //pause all game sounds
        public void PauseSound()
        {
            sound.PauseAllSounds();
        }

        //collision box and sphere abstract methods to be implemented for each car
        protected abstract BoundingBox CreateBoundingBox();
        protected abstract BoundingSphere CreateBoundingSphere();

        protected BoundingBox UpdateBoundingBox()
        {
            //obtain original box
            Vector3[] boxCorners = new Vector3[8];
            //get corners of that box
            collisionBox.GetCorners(boxCorners);
            //transform acording to the vehicle transform
            Vector3.Transform(boxCorners, ref transformationMatrix, boxCorners);

            for (int i = 0; i < 8; i++)
                Vector3.Transform(boxCorners[i], Matrix.CreateTranslation(new Vector3(0, heightOffset, 0) * carScale));

            //return transformed boundingbox
            return BoundingBox.CreateFromPoints(boxCorners);
        }

        //normal bounding sphere
        protected BoundingSphere UpdateBoundingSphere()
        {
            collisionSphere.Center = position / 20;
            return collisionSphere;
        }

        //if vehicle passes a waypoint
        public void PassWaypoint(Player player)
        {
            if (player.Car.Sphere.Contains(waypointSpheres[point]) != ContainmentType.Disjoint)
            {
                if (point < waypointSpheres.Length - 1)
                {
                    point++;
                }
            }
        }

        //draw the vehicle
        virtual public void draw(ChaseCamera cameraPosition, Viewport currentViewPort, GameTime gameTime)
        {
            //update smoke particles
            smoke.SetCamera(cameraPosition.view, cameraPosition.projection);
            smoke.Draw(gameTime);

            if (drawSpheres)
                BoundingSphereRenderer.Render(waypointSpheres[point], graphics.GraphicsDevice, cameraPosition.view, cameraPosition.projection, Color.Red);

            graphics.GraphicsDevice.Viewport = currentViewPort;
            graphics.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;

            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[carModel.Bones.Count];
            carModel.CopyAbsoluteBoneTransformsTo(transforms);

            foreach (ModelMesh mesh in carModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.Begin();

                    effect.EnableDefaultLighting();

                    float direction = speed / Math.Abs(speed);
                    /*Vector3 pointToRotateAround = rotationMatrix.Translation +
                              rotationMatrix.Right * (position - previousPosition);

                    transforms[mesh.ParentBone.Index].Translation -= pointToRotateAround;
                    transforms[mesh.ParentBone.Index] *= transformationMatrix;
                    transforms[mesh.ParentBone.Index].Translation += pointToRotateAround;*/
                    effect.World = transforms[mesh.ParentBone.Index]/* Matrix.CreateRotationY((direction > 0 || direction < 0 ? direction : 1) * currentRotation / 100)*/* Matrix.CreateRotationY(carGlobalRotation) * transformationMatrix;

                    effect.View = cameraPosition.view;
                    effect.Projection = cameraPosition.projection;

                    effect.End();
                }

                mesh.Draw();
            }

            //BoundingSphereRenderer.Render(collisionSphere, graphics.GraphicsDevice, cameraPosition.view, cameraPosition.projection, Color.Blue);
        }

        //Name of the car
        public abstract string Name { get; }

        public abstract void changeGearBoxToAutomatic();
        public abstract void changeGearBoxToManual();
        public abstract void createGauge(Viewport viewport);
        public abstract void drawGauge();

        //set methods
        public void setHeightmapGenerator(HeightMapGenerator currentHeightMapGenerator)
        {
            currentHeightMap = currentHeightMapGenerator;
        }

        public void setNewHeightOffset(float newHeight)
        {
            heightOffset = newHeight;
        }
    }
}
