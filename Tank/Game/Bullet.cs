using System;
using System.Collections.Generic;
using System.Text;

using Tank.Common;

namespace Tank.Game
{
    public class Bullet
    {
        private ModelTransformer bulletModel;
        private Vector3 position;
        private Vector3 direction;
        private double angle;

        private const double Speed = 6.0;

        public Bullet(Model bulletModel, Vector3 position, Vector3 direction)
        {
            this.bulletModel = new ModelTransformer(bulletModel);
            this.position = position;
            this.direction = direction;

            Vector2 direction2 = new Vector2(direction.X, direction.Z);
            this.angle = -Utility.ConvertRadiansToDegrees(direction2.Angle()) + 90.0;
        }

        public Vector3 Position
        {
            get { return position; }
        }

        public void Update(double deltaTime)
        {
            position += direction * deltaTime * Bullet.Speed;
        }

        public void Render()
        {
            bulletModel.Displacement = position;
            bulletModel.RotationAngle = angle;

            Renderer.Instance.DrawModel(bulletModel);
        }
    }
}
