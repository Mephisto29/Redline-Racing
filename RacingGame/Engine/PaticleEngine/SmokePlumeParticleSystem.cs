#region File Description
//-----------------------------------------------------------------------------
// SmokePlumeParticleSystem.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace RacingGame.Engine.PaticleEngine
{
    /// <summary>
    /// Custom particle system for creating a giant plume of long lasting smoke.
    /// </summary>
    class SmokePlumeParticleSystem : ParticleSystem
    {
        public SmokePlumeParticleSystem(Game game, ContentManager content, GraphicsDevice device)
            : base(game, content, device)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = "Textures//Particles//smoke";

            settings.MaxParticles = 100;

            settings.Duration = TimeSpan.FromSeconds(0.5f);

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 1f;

            settings.MinVerticalVelocity = 0.5f;
            settings.MaxVerticalVelocity = 1.1f;

            // Create a wind effect by tilting the gravity vector sideways.
            settings.Gravity = new Vector3(0, -0.5f, 0);

            settings.EndVelocity = 0.2f;

            //settings.MinRotateSpeed = -1;
            //settings.MaxRotateSpeed = 1;

            settings.MinStartSize = 5f;
            settings.MaxStartSize = 10f;

            settings.MinEndSize = 50f;
            settings.MaxEndSize = 200f;
        }
    }
}
