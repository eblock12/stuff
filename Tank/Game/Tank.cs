using System;
using System.Collections.Generic;
using System.Text;

using Tank.Common;

namespace Tank.Game
{
    public class Tank
    {
        // tank's position in 2-D
        private Vector2 position;

        // the rotation angle of the tank base (in degrees)
        private double baseRotation;

        // the rotation angle of the tank turret (in degrees)
        private double turretRotation;

        // the direction the tank last moved in
        private DriveDirection lastDirection;

        // the direction the tank is currently moving in
        private DriveDirection direction;

        // the rotation angle the tank is turning to
        private double rotateToAngle;

        // the speed the tank is changing its direction at
        private double driveSpeed;

        // this tank's normal rotation rate
        private double normalRotationSpeed;

        // this tank's normal driving speed
        private double normalDriveSpeed;

        // model transformer for the tank's base
        private ModelTransformer tankBase;

        // model transformer for the tank's turret
        private ModelTransformer tankTurret;

        // value to move tank on Y axis to stick to floor
        private double floorAdjustment;

        // adjusts bullet spawning so it appears to come from turret
        private static Vector3 BulletPositionOffset = new Vector3(0.0, 0.32, 0);

        // scales the tank by this multiplier uniformly on all axes
        private const double Scaler = 0.6;

        public Tank(Model tankBaseModel, Model tankTurretModel, 
            double normalRotationSpeed, double normalDriveSpeed)
        {
            this.tankBase = new ModelTransformer(tankBaseModel);
            this.tankTurret = new ModelTransformer(tankTurretModel);
            this.normalRotationSpeed = normalRotationSpeed;
            this.normalDriveSpeed = normalDriveSpeed;
            this.lastDirection = DriveDirection.Right;

            position = Vector2.Zero;
            turretRotation = baseRotation = 0;

            this.tankBase.EnableProjection = true;

            floorAdjustment = -this.tankBase.Model.BoundingVolume.Bottom;
        }

        public double Angle
        {
            get { return baseRotation; }
        }

        public Vector3 Position3
        {
            get { return new Vector3(position.X, floorAdjustment, position.Y); }
        }

        public Vector3 Direction
        {
            get
            {
                double angle = -Utility.ConvertDegreesToRadians(baseRotation);
                return new Vector3(Math.Cos(angle), 0, Math.Sin(angle));
            }
        }

        public Vector3 TurretDirection
        {
            get
            {
                double angle = -Utility.ConvertDegreesToRadians(turretRotation);
                return new Vector3(Math.Cos(angle), 0, Math.Sin(angle));
            }
        }

        public double TurretAngle
        {
            get { return turretRotation; }
            set { turretRotation = value; }
        }

        public void Move(DriveDirection direction)
        {
            if (this.direction != DriveDirection.None)
            {
                lastDirection = this.direction;
            }
            this.direction = direction;

            driveSpeed = normalDriveSpeed;
            double newAngle = rotateToAngle;
            switch (direction)
            {
                case DriveDirection.Up:
                    newAngle = 0;
                    break;
                case DriveDirection.Down:
                    newAngle = 180.0;
                    break;
                case DriveDirection.Left:
                    newAngle = -90;
                    break;
                case DriveDirection.Right:
                    newAngle = 90;
                    break;
                case DriveDirection.UpLeft:
                    newAngle = -45;
                    break;
                case DriveDirection.UpRight:
                    newAngle = 45;
                    break;
                case DriveDirection.DownLeft:
                    newAngle = -135;
                    break;
                case DriveDirection.DownRight:
                    newAngle = 135;
                    break;
                default:
                    driveSpeed = 0;
                    break;

            }

            //if (newAngle != rotateToAngle)
            //Console.WriteLine("{0} {1}", newAngle, rotateToAngle);
            double deltaAngle = Math.Abs(newAngle - rotateToAngle);

            if (newAngle == 180.0)
            {
                double absAngle = Math.Abs(rotateToAngle);
                if (absAngle >= 90.0)
                {
                    newAngle = 180.0;
                }
                else
                {
                    newAngle = 0;
                    driveSpeed = -driveSpeed;
                }
            }
            else if (deltaAngle == 180.0)
            {
                newAngle = rotateToAngle;
                driveSpeed = -driveSpeed;
            }
            else if (deltaAngle > 90)
            {
                newAngle = newAngle - 90;
            }

            if ((newAngle < 0 && rotateToAngle > 0) || (newAngle > 0 && rotateToAngle < 0))
            {
                newAngle = -newAngle;
            }

            //if (rotateToAngle != newAngle)
           // Console.WriteLine("N: {0} {1} {2}", rotateToAngle, newAngle, deltaAngle);

            rotateToAngle = newAngle;
        }

        public void Update(double deltaTime)
        {
            double rotationRate = deltaTime * normalRotationSpeed;
            if (Utility.IsNearlyEqual(rotateToAngle, baseRotation, rotationRate))
            {
                baseRotation = rotateToAngle;
            }
            else if (baseRotation < rotateToAngle)
            {
                baseRotation += rotationRate;
            }
            else if (baseRotation > rotateToAngle)
            {
                baseRotation -= rotationRate;
            }

            double rotationRadians = Utility.ConvertDegreesToRadians(baseRotation - 90.0);
            Vector2 directionVector = new Vector2(Math.Cos(rotationRadians), 
                Math.Sin(rotationRadians));
            position += directionVector * driveSpeed * deltaTime;
        }

        public void Render()
        {
            tankBase.RotationAngle = -baseRotation - 90.0;
            tankBase.Displacement = Position3;
            tankBase.Scale = new Vector3(Tank.Scaler);

            tankTurret.RotationAngle = turretRotation;
            tankTurret.Displacement = tankBase.Displacement;
            tankTurret.Scale = new Vector3(Tank.Scaler);

            Renderer.Instance.DrawModel(tankBase);
            Renderer.Instance.DrawModel(tankTurret);
        }

        /// <summary>
        /// Gets the tank's position projected into 2-D window space.
        /// </summary>
        /// <returns>A Vector2 containing the 2-D projected position</returns>
        public Vector2 GetProjectedPosition()
        {
            return tankBase.ProjectedPosition;
        }

        public Bullet FireBullet(Model bulletModel)
        {
            Vector3 direction = this.TurretDirection;
            Vector3 turretOffset = Tank.BulletPositionOffset + direction * 1.3;

            return new Bullet(bulletModel, Position3 + turretOffset, direction);
        }
    }

    public enum DriveDirection
    {
        None,
        Up,
        Down,
        Left,
        Right,
        UpLeft,
        UpRight,
        DownLeft,
        DownRight
    }
}
