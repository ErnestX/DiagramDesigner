﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesignerEngine
{
    class WallEntity : Entity
    {
        public double WallThickness;

        public override Geometry Geometry => throw new NotImplementedException();

        public override void Draw()
        {
            throw new NotImplementedException();
        }
    }
}
