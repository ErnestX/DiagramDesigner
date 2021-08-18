﻿using DiagramDesignerEngine;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiagramDesignerModel
{
    /// <summary>
    /// In the context of shape grammar, the collection of entities reflects the current description of the diagram. 
    /// A change in the collection reflects a change of the description but not necessarily a change of the appearance of the diagram. 
    /// </summary>
    public class DDModel
    {
        public ProgramRequirementsTable ProgramRequirements { get; } = new ProgramRequirementsTable();

        public List<WallEntity> WallEntities { get; private set; } = new List<WallEntity>();

        public List<EnclosedProgram> Programs { get; private set; } = new List<EnclosedProgram>();

        public event EventHandler ModelChanged;

        public void CreateNewWallEntity()
		{
            this.WallEntities.Add(new WallEntity(1));
		}

        public void AddPointToWallEntityAtIndex(Point point, int index)
		{
            if (index >= this.WallEntities.Count)
			{
                return;
			}
            this.WallEntities[index].AddPointToGeometry(point);

            this.OnModelChanged();
		}

        public void RemoveAllWallsAndPrograms()
		{
            this.WallEntities.Clear();
            this.Programs.Clear();
            this.OnModelChanged();
		}

        public void ResolvePrograms()
		{
            // make a collection of all geometry segments
            var allSegments = new List<LineSegment>();
            foreach (WallEntity we in this.WallEntities)
			{
                allSegments.AddRange(we.Geometry.ConvertToLineSegments());
			}

			this.Programs = (new ProgramsFinder(allSegments, this.ProgramRequirements)).FindPrograms();
            this.OnModelChanged();
		}

		public double TotalEnclosedArea()
        {
            // TODO: stub
            return 0;
        }

        public double TotalPerimeterLength()
        {
            // TODO: stub
            return 0;
        }

        private void OnModelChanged()
		{
            if (this.ModelChanged != null)
            {
                this.ModelChanged(this, null);
            }
        }
    }
}