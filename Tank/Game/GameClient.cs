using System;
using System.Collections.Generic;
using System.Text;

using Tank.Common;

using Tao.Sdl;

namespace Tank.Game
{
    class GameClient : ITask
    {
        private Model tankBarrelModel, tankBaseModel, bulletModel;

        private List<Tank> tankList = new List<Tank>();
        private List<Bullet> bulletList = new List<Bullet>();
        private List<Bullet> deadBulletList = new List<Bullet>();

        private CameraMode cameraMode;
        private double circleCameraAngle;
        private Vector3 circleCameraPosition;

        private bool firedBullet = false;

        private Tank playerTank;
        private Level level;

        private ControlBinding exitGameBinding = new ControlBinding("ExitGame", ControlBinding.KeyEscape);
        private ControlBinding moveUpBinding = new ControlBinding("MoveUp", 'w');
        private ControlBinding moveDownBinding = new ControlBinding("MoveDown", 's');
        private ControlBinding moveLeftBinding = new ControlBinding("MoveLeft", 'a');
        private ControlBinding moveRightBinding = new ControlBinding("MoveRight", 'd');
        private ControlBinding fireBulletBinding = new ControlBinding("FireBullet", MouseButton.Left);

        private ControlBinding debugResetCameraBinding = new ControlBinding("DebugResetCamera", 'r');

        private int startTime;
        private double pendingTime;

        private const double PlayerTankSpeed = 5.0;
        private const double PlayerTankRotateSpeed = 500.0;

        private const double TimeDelta = 0.016;

        #region ITask Members

        public void OnRun()
        {
            int currentTime = Sdl.SDL_GetTicks();
            pendingTime += (double)(currentTime - startTime) / 1000.0;
            startTime = currentTime;

            while (pendingTime > 0)
            {
                this.CheckUserInput();
                pendingTime -= GameClient.TimeDelta;
            }

            this.RenderGameObjects();
        }

        public void OnStart()
        {
            // load models
            tankBaseModel = Model.LoadModelFromX3m(Resources.TankBaseModelFile);
            tankBarrelModel = Model.LoadModelFromX3m(Resources.TankBarrelModelFile);
            bulletModel = Model.LoadModelFromX3m(Resources.BulletModelFile);

            // setup materials
            tankBaseModel.Material.DiffuseColor = new Color(0.2, 0.2, 1.0);
            tankBarrelModel.Material.DiffuseColor = new Color(0.3, 0.3, 1.0);

            // create the player's tank
            playerTank = new Tank(tankBaseModel, tankBarrelModel, 
                PlayerTankRotateSpeed, PlayerTankSpeed);
            tankList.Add(playerTank);

            // setup a new level
            level = new Level(20, 20, 25.0, 20.0);

            // camera is initially in circular overhead mode
            cameraMode = CameraMode.CircleOverhead;
            circleCameraAngle = 0.0;

            // start keeping track of running time
            startTime = Sdl.SDL_GetTicks();
        }

        public void OnStop()
        {
        }

        #endregion

        private void CheckUserInput()
        {
            ControlManager controlMgr = ControlManager.Instance;

            if (controlMgr.IsPressed(exitGameBinding))
            {
                Logger.Instance.LogInformation(Resources.ClientTag, Resources.UserQuitRequest);
                Engine.Instance.RemoveAllTasks();
            }

            if (controlMgr.IsPressed(moveUpBinding))
            {
                if (controlMgr.IsPressed(moveLeftBinding))
                {
                    playerTank.Move(DriveDirection.UpLeft);
                }
                else if (controlMgr.IsPressed(moveRightBinding))
                {
                    playerTank.Move(DriveDirection.UpRight);
                }
                else
                {
                    playerTank.Move(DriveDirection.Up);
                }
            }
            else if (controlMgr.IsPressed(moveDownBinding))
            {
                if (controlMgr.IsPressed(moveLeftBinding))
                {
                    playerTank.Move(DriveDirection.DownLeft);
                }
                else if (controlMgr.IsPressed(moveRightBinding))
                {
                    playerTank.Move(DriveDirection.DownRight);
                }
                else
                {
                    playerTank.Move(DriveDirection.Down);
                }
            }
            else if (controlMgr.IsPressed(moveLeftBinding))
            {
                playerTank.Move(DriveDirection.Left);
            }
            else if (controlMgr.IsPressed(moveRightBinding))
            {
                playerTank.Move(DriveDirection.Right);
            }
            else
            {
                playerTank.Move(DriveDirection.None);
            }

            //if (ControlManager.Instance.IsPressed(moveUpBinding))
            //{
            //    playerTank.Drive(-TimeDelta * PlayerTankSpeed);
            //    playerTank.RotateTo(90, TimeDelta * PlayerTankRotateSpeed);
            //}
            //else if (ControlManager.Instance.IsPressed(moveDownBinding))
            //{
            //    playerTank.Drive(TimeDelta * PlayerTankSpeed);
            //    playerTank.RotateTo(90, TimeDelta * PlayerTankRotateSpeed);
            //}
            //else if (ControlManager.Instance.IsPressed(moveLeftBinding))
            //{
            //    playerTank.Drive(-TimeDelta * PlayerTankSpeed);
            //    playerTank.RotateTo(0, TimeDelta * PlayerTankRotateSpeed);
            //}
            //else if (ControlManager.Instance.IsPressed(moveRightBinding))
            //{
            //    playerTank.Drive(TimeDelta * PlayerTankSpeed);
            //    playerTank.RotateTo(0, TimeDelta * PlayerTankRotateSpeed);
            //}

            //if (ControlManager.Instance.IsPressed(debugResetCameraBinding))
            //{
            //    circleCameraAngle = 0;
            //    cameraMode = CameraMode.CircleOverhead;
            //}

            if (ControlManager.Instance.IsPressed(fireBulletBinding))
            {
                if (!firedBullet)
                {
                    bulletList.Add(playerTank.FireBullet(bulletModel));
                    firedBullet = true;
                }
            }
            else
            {
                firedBullet = false;
            }

            foreach (Tank tank in tankList)
            {
                tank.Update(TimeDelta);
            }

            if (deadBulletList.Count > 0)
            {
                foreach (Bullet deadBullet in deadBulletList)
                {
                    bulletList.Remove(deadBullet);
                }
                deadBulletList.Clear();
            }

            foreach (Bullet bullet in bulletList)
            {
                bullet.Update(TimeDelta);

                if (bullet.Position.X > level.Width / 2.0 || bullet.Position.X < -level.Width / 2.0 ||
                    bullet.Position.Z > level.Height / 2.0 || bullet.Position.Z < -level.Height / 2.0)
                {
                    deadBulletList.Add(bullet);
                }
            }

            Vector2 playerWindowPosition = playerTank.GetProjectedPosition();
            Vector2 mousePosition = new Vector2((double)ControlManager.Instance.MouseX, (double)ControlManager.Instance.MouseY);
            Vector2 playerToMouse = mousePosition - playerWindowPosition;

            double playerToMouseAngle = -Utility.ConvertRadiansToDegrees(playerToMouse.Angle()); 

            playerTank.TurretAngle = playerToMouseAngle;

            if (cameraMode == CameraMode.CircleOverhead)
            {
                circleCameraAngle += TimeDelta;

                if (circleCameraAngle >= Math.PI / 2)
                {
                    cameraMode = CameraMode.Overhead;
                }

                circleCameraPosition = new Vector3(Math.Cos(circleCameraAngle) * 10.0,
                    Math.Abs(Math.Sin(circleCameraAngle)) * 20.0 + 5.0, Math.Sin(circleCameraAngle) * 10.0);
            }
        }

        private void RenderGameObjects()
        {
            switch (cameraMode)
            {
                case CameraMode.Overhead:
                    Renderer.Instance.CameraPosition = new Vector3(0, 25.0, 10.0);
                    Renderer.Instance.CameraFocusPoint = Vector3.Zero;
                    Renderer.Instance.CameraUp = Vector3.UnitY;
                    break;

                case CameraMode.CircleOverhead:
                    Renderer.Instance.CameraPosition = circleCameraPosition;
                    Renderer.Instance.CameraFocusPoint = Vector3.Zero;
                    Renderer.Instance.CameraUp = Vector3.UnitY;
                    break;

                case CameraMode.FirstPersonTest:
                    Vector3 position = new Vector3(0.0, 0.6, 0);
                    position += playerTank.Position3;
                    Renderer.Instance.CameraPosition = position;
                    Renderer.Instance.CameraFocusPoint = position + playerTank.Direction * 2.0;
                    Renderer.Instance.CameraUp = Vector3.UnitY;
                    break;
            }

            level.Render();

            foreach (Tank tank in tankList)
            {
                tank.Render();
            }

            foreach (Bullet bullet in bulletList)
            {
                bullet.Render();
            }
        }
    }

    public enum CameraMode
    {
        Overhead,
        FirstPersonTest,
        CircleOverhead
    }
}
