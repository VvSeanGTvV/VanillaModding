using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Graphics.CameraModifiers;

namespace VanillaModding.Common.CamModifier
{
    internal class CameraZoomMovement : ICameraModifier
    {
        private int framesToLast;
        private int framesElapsed;
        private float lastZoom = -1;
        public Vector2 targetPosition;
        public float targetZoom = 0, paddingLeft = 0.2f, paddingRight = 0.2f;

        // This makes sure that other modifiers of the same identity don't run at the same time
        public string UniqueIdentity { get; private set; }
        public bool Finished { get; private set; }
        public bool IsAScreenShake => false;
        public bool isSine = false;

        public CameraZoomMovement(Vector2 position, int frames, float Zoom = 1, float padLeft = 0.2f, float padRight = 0.2f, string uniqueIdentity = null, bool sine = false)
        {
            targetZoom = Zoom;
            targetPosition = position - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
            framesToLast = frames;
            UniqueIdentity = uniqueIdentity;
            paddingLeft = padLeft;
            paddingRight = padRight;
            isSine = sine;
        }

        public void Update(ref CameraInfo cameraInfo)
        {
            if (lastZoom <= 0) lastZoom = VanillaModdingSystem.Zoom;
            float progress = Utils.GetLerpValue(0, framesToLast, framesElapsed); // Equivalent to "(float)framesElapsed / framesToLast"
            float lerpAmount = progress switch
            {
                var p when p < paddingLeft => Utils.Remap(progress, 0, paddingLeft, 0, 1),
                var p when p > 1f - paddingRight => Utils.Remap(progress, 1f - paddingRight, 1f, 1, 0),
                _ => 1, // progress is between 0.5 and 0.8
            };
            float finalLerp = isSine ? MathF.Sin(lerpAmount * MathF.PI * 0.5f) : lerpAmount;
            cameraInfo.CameraPosition = Vector2.Lerp(cameraInfo.CameraPosition, targetPosition, finalLerp);
            VanillaModdingSystem.Zoom = MathHelper.Lerp(lastZoom, targetZoom, finalLerp);

            // Pauses the effect if the game is tabbed out or paused
            if (!Main.gameInactive && !Main.gamePaused)
            {
                framesElapsed++;
            }

            if (framesElapsed >= framesToLast)
            {
                Finished = true;
            }
        }
    }
}
