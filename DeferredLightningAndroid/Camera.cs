using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DeferredLightingAndroid
{
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {
        public float CameraArc { get; set; } = -30;

        public float CameraRotation { get; set; } = 0;

        public float CameraDistance { get; set; } = 1000;

        public Vector3 Position { get; private set; }
        public float NearPlaneDistance { get; set; } = 1;
        public float FarPlaneDistance { get; set; } = 3000;


        public Matrix View { get; private set; }

        public Matrix Projection { get; private set; }

        KeyboardState currentKeyboardState = new KeyboardState();
        GamePadState currentGamePadState = new GamePadState();

        public Camera(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {

            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);

            // TODO: Add your update code here

            float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            // Check for input to rotate the camera up and down around the model.
            if (currentKeyboardState.IsKeyDown(Keys.Up) ||
                currentKeyboardState.IsKeyDown(Keys.W))
            {
                CameraArc += time * 0.1f;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Down) ||
                currentKeyboardState.IsKeyDown(Keys.S))
            {
                CameraArc -= time * 0.1f;
            }

            CameraArc += currentGamePadState.ThumbSticks.Right.Y * time * 0.05f;

            // Limit the arc movement.
            if (CameraArc > 90.0f)
                CameraArc = 90.0f;
            else if (CameraArc < -90.0f)
                CameraArc = -90.0f;

            // Check for input to rotate the camera around the model.
            if (currentKeyboardState.IsKeyDown(Keys.Right) ||
                currentKeyboardState.IsKeyDown(Keys.D))
            {
                CameraRotation += time * 0.1f;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Left) ||
                currentKeyboardState.IsKeyDown(Keys.A))
            {
                CameraRotation -= time * 0.1f;
            }

            CameraRotation += currentGamePadState.ThumbSticks.Right.X * time * 0.05f;

            // Check for input to zoom camera in and out.
            if (currentKeyboardState.IsKeyDown(Keys.Z))
                CameraDistance += time * 0.25f;

            if (currentKeyboardState.IsKeyDown(Keys.X))
                CameraDistance -= time * 0.25f;

            CameraDistance += currentGamePadState.Triggers.Left * time * 0.25f;
            CameraDistance -= currentGamePadState.Triggers.Right * time * 0.25f;

            // Limit the arc movement.
            if (CameraDistance > 11900.0f)
                CameraDistance = 11900.0f;
            else if (CameraDistance < 10.0f)
                CameraDistance = 10.0f;

            if (currentGamePadState.Buttons.RightStick == ButtonState.Pressed ||
                currentKeyboardState.IsKeyDown(Keys.R))
            {
                CameraArc = -30;
                CameraRotation = 0;
                CameraDistance = 100;
            }

            View = Matrix.CreateTranslation(0, -10, 0) *
                      Matrix.CreateRotationY(MathHelper.ToRadians(CameraRotation)) *
                      Matrix.CreateRotationX(MathHelper.ToRadians(CameraArc)) *
                      Matrix.CreateLookAt(new Vector3(0, 0, -CameraDistance),
                                          new Vector3(0, 0, 0), Vector3.Up);

            Position = Vector3.Transform(Vector3.Zero, Matrix.Invert(View));

            float aspectRatio = (float)Game.Window.ClientBounds.Width /
                                (float)Game.Window.ClientBounds.Height;
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                    aspectRatio,
                                                                    NearPlaneDistance,
                                                                    FarPlaneDistance);
            base.Update(gameTime);
        }
    }
}