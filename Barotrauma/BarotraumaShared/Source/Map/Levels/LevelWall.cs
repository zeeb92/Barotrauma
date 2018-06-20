﻿using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Voronoi2;

namespace Barotrauma
{
    partial class LevelWall : IDisposable
    {        
        private List<VoronoiCell> cells;                
        public List<VoronoiCell> Cells
        {
            get { return cells; }
        }

        private Body body;
        public Body Body
        {
            get { return body; }
        }

        private float moveState;
        private float moveLength;

        private Vector2 moveAmount;
        public Vector2 MoveAmount
        {
            get { return moveAmount; }
            set
            {
                moveAmount = value;
                moveLength = moveAmount.Length();
            }
        }

        public float MoveSpeed;

        private Vector2? originalPos;

        public float MoveState
        {
            get { return moveState; }
            set { moveState = MathHelper.Clamp(value, 0.0f, MathHelper.TwoPi); }
        }

        public LevelWall(List<Vector2> vertices, Color color, bool giftWrap = false)
        {
            if (giftWrap)
            {
                vertices = MathUtils.GiftWrap(vertices);
            }

            VoronoiCell wallCell = new VoronoiCell(vertices.ToArray());
            for (int i = 0; i < wallCell.edges.Count; i++)
            {
                wallCell.edges[i].Cell1 = wallCell;
                wallCell.edges[i].IsSolid = true;
            }
            cells = new List<VoronoiCell>() { wallCell };

            body = CaveGenerator.GeneratePolygons(cells, out List<Vector2[]> triangles, false);
#if CLIENT
            List<VertexPositionTexture> bodyVertices = CaveGenerator.GenerateRenderVerticeList(triangles);
            SetBodyVertices(bodyVertices.ToArray(), color);
            SetWallVertices(CaveGenerator.GenerateWallShapes(cells), color);
#endif
        }

        public LevelWall(List<Vector2> edgePositions, Vector2 extendAmount, Color color)
        {
            cells = new List<VoronoiCell>();
            for (int i = 0; i < edgePositions.Count - 1; i++)
            {
                Vector2[] vertices = new Vector2[4];
                vertices[0] = edgePositions[i];
                vertices[1] = edgePositions[i + 1];
                vertices[2] = vertices[0] + extendAmount;
                vertices[3] = vertices[1] + extendAmount;

                VoronoiCell wallCell = new VoronoiCell(vertices);
                wallCell.edges[0].Cell1 = wallCell;
                wallCell.edges[1].Cell1 = wallCell;
                wallCell.edges[2].Cell1 = wallCell;
                wallCell.edges[3].Cell1 = wallCell;
                wallCell.edges[0].IsSolid = true;

                if (i > 1)
                {
                    wallCell.edges[3].Cell2 = cells[i - 1];
                    cells[i - 1].edges[1].Cell2 = wallCell;
                }

                cells.Add(wallCell);
            }

            body = CaveGenerator.GeneratePolygons(cells, out List<Vector2[]> triangles, false);

#if CLIENT
            List<VertexPositionTexture> bodyVertices = CaveGenerator.GenerateRenderVerticeList(triangles);
            SetBodyVertices(bodyVertices.ToArray(), color);
            SetWallVertices(CaveGenerator.GenerateWallShapes(cells), color);
#endif
        }

        public void Update(float deltaTime)
        {
            if (body.BodyType == BodyType.Static) return;

            Vector2 bodyPos = ConvertUnits.ToDisplayUnits(body.Position);
            Cells.ForEach(c => c.Translation = bodyPos);

            if (!originalPos.HasValue) originalPos = bodyPos;

            if (moveLength > 0.0f && MoveSpeed > 0.0f)
            {
                moveState += MoveSpeed / moveLength * deltaTime;
                moveState %= MathHelper.TwoPi;

                Vector2 targetPos = ConvertUnits.ToSimUnits(originalPos.Value + moveAmount * (float)Math.Sin(moveState));            
                body.ApplyForce((targetPos - body.Position).ClampLength(1.0f) * body.Mass);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
#if CLIENT
            if (wallVertices != null)
            {
                wallVertices.Dispose();
                wallVertices = null;
            }
            if (bodyVertices != null)
            {
                BodyVertices.Dispose();
                bodyVertices = null;
            }
#endif
        }
    }
}
