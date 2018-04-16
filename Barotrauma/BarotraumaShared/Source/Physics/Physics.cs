﻿using FarseerPhysics.Dynamics;

namespace Barotrauma
{
    static class Physics
    {
        public const Category CollisionNone         = Category.None;
        public const Category CollisionAll          = Category.All;
        public const Category CollisionWall         = Category.Cat1;
        public const Category CollisionCharacter    = Category.Cat2;
        public const Category CollisionPlatform     = Category.Cat3;
        public const Category CollisionStairs       = Category.Cat4;
        public const Category CollisionItem         = Category.Cat5;
        public const Category CollisionItemBlocking = Category.Cat6;
        public const Category CollisionProjectile   = Category.Cat7;
        public const Category CollisionLevel        = Category.Cat8;
        public const Category CollisionRepair       = Category.Cat9;

        public static float DisplayToRealWorldRatio = 1.0f / 80.0f;

        public const float DisplayToSimRation = 100.0f;            
    }
}
