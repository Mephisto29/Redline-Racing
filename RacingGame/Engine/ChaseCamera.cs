/*
 * This class is used to simulate a lazy chase camera
*/
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace RacingGame.Engine
{
    public class ChaseCamera
    {
        //the desired changes to the camera
        public Vector3 chasePosition;
        public Vector3 chaseDirection;
        public Vector3 up;

        public Vector3 desiredPositionOffset;
        public Vector3 desiredPosition;
        public Vector3 lookAtOffset;
        public Vector3 lookAt;

        //camera physics, so that the desired effect of lazy cam can be generated
        public float stiffness;
        public float damping;
        public float mass;

        //current properties of the camera
        public Vector3 Position;
        public Vector3 PreviousPosition;
        public Vector3 Velocity;

        //perspective properties
        public float aspectRatio;
        public float fieldOfView;
        public float nearPlaneDistance;
        public float farPlaneDistance;

        //matrix properties
        public Matrix view;
        public Matrix projection;

        //constructor
        public ChaseCamera()
        {
            chasePosition = Vector3.Zero;
            chaseDirection = Vector3.Forward;
            up = Vector3.Up;

            desiredPositionOffset = new Vector3(0, 0.0f, 0.0f);
            lookAtOffset = new Vector3(0, 0.2f, 0);
            stiffness = 8000.0f;
            damping = 1000.0f;
            mass = 50;

            aspectRatio = 4.0f / 3.0f;
            fieldOfView = MathHelper.ToRadians(30.0f);
            nearPlaneDistance = 1.0f;
            farPlaneDistance = 100000.0f;
        }

        private void UpdateWorldPositions()
        {
            // Construct a matrix to transform from object space to worldspace
            Matrix transform = Matrix.Identity;
            transform.Up = up;
            transform.Forward = chaseDirection;
            transform.Right = Vector3.Cross(up, chaseDirection);

            desiredPosition = chasePosition + Vector3.TransformNormal(desiredPositionOffset, transform);
            lookAt = chasePosition + Vector3.TransformNormal(lookAtOffset, transform);
        }

        private void UpdateMatrices()
        {
            view = Matrix.CreateLookAt(this.Position, this.lookAt, this.up);
            projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance);
        }

        public void Reset()
        {
            UpdateWorldPositions();

            // Stop motion
            Velocity = Vector3.Zero;

            // Force desired position
            Position = desiredPosition;

            UpdateMatrices();
        }

        public void Update(GameTime gameTime)
        {
            UpdateWorldPositions();

            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector3 stretch = Position - desiredPosition;
            Vector3 force = -stiffness * stretch - damping * Velocity;

            Vector3 acceleration = force / mass;
            Velocity += acceleration * elapsedTime;

            PreviousPosition = Position;
            Position += Velocity * elapsedTime;

            UpdateMatrices();
        }
    }
}
