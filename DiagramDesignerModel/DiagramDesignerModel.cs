using BasicGeometries;
using ShapeGrammarEngine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DiagramDesignerModel
{
	/// <summary>
	/// In the context of shape grammar, the collection of entities reflects the current description of the diagram. 
	/// A change in the collection reflects a change of the description but not necessarily a change of the appearance of the diagram. 
	/// </summary>
	public class DiagramDesignerModel
    {
        public ProgramRequirementsTable ProgramRequirements { get; } = new ProgramRequirementsTable();

        private List<WallEntity> wallEntities = new List<WallEntity>();
        public ReadOnlyCollection<WallEntity> WallEntities => this.wallEntities.AsReadOnly();

        public List<EnclosedProgram> Programs { get; private set; } = new List<EnclosedProgram>();

        public GrammarRulesDataTable CurrentRulesInfo => this.rulesStore.CurrentRulesInfoDataTable;

        private GrammarRulesStore rulesStore = new GrammarRulesStore();

        public event EventHandler ModelChanged;

        private static readonly NLog.Logger Logger = NLog.LogManager.GetLogger("Default");

        public void CreateNewWallEntity()
		{
            this.wallEntities.Add(new WallEntity(1));
		}

        /// <summary>
        /// Add Point at the end of the specified WallEntity to extend its polyline geometry
        /// </summary>
        /// <param name="point"> the new point to add </param>
        /// <param name="index"> the index of the WallEntity for the new point </param>
        public void AddPointToWallEntityAtIndex(Point point, int index)
		{
            EntitiesOperator.AddPointToWallEntityAtIndex(ref this.wallEntities, point, index);
            this.OnModelChanged();
		}

        /// <summary>
        /// Delete multiple segments from WallEntities in Model. 
        /// </summary>
        /// <param name="segmentsToDelete"> The segments to be deleted; 
        /// each Tuple represents a single segment with the index of the containing WallEntity within all WallEntities 
        /// and the two ascending consecutive indexes indicating the line segment within the WallEntity. </param>
        public void DeleteSegmentsFromWallEntitiesAtIndexes(List<Tuple<int, int, int>> segmentsToDelete)
		{
            EntitiesOperator.DeleteSegmentsFromWallEntitiesAtIndexes(ref this.wallEntities, segmentsToDelete);
            this.OnModelChanged();
        }

    
		public void CreateNewRuleFromExample(PolylinesGeometry leftHandGeometry, PolylinesGeometry rightHandGeometry)
		{
            this.rulesStore.CreateNewRuleFromExample(leftHandGeometry, rightHandGeometry);
		}

        public void LearnFromExampleForRule(PolylinesGeometry leftHandGeometry, PolylinesGeometry rightHandGeometry, Guid ruleId)
		{
            var rule = this.rulesStore.GetRuleById(ruleId);
            try
			{
                rule.LearnFromExample(leftHandGeometry, rightHandGeometry, out _);
			}
            catch (GeometryParsingFailureException)
			{
                throw new ArgumentException("the geometires do not match the rule");
			}
            this.rulesStore.RuleUpdated(ruleId);
        }

        public PolylinesGeometry ApplyRuleGivenLeftHandGeometry(PolylinesGeometry leftHandGeometry, Guid ruleId)
		{
            var rule = this.rulesStore.GetRuleById(ruleId);
            try
			{
                var newGeo = rule.ApplyToGeometry(leftHandGeometry);
				return newGeo;
			}
			catch (GeometryParsingFailureException e)
            {
                throw new ArgumentException(e.Message);
			}
            catch (RuleApplicationFailureException e)
			{
                throw e;
			}
        }

        public void RemoveAllWallsAndPrograms()
		{
            this.wallEntities.Clear();
            this.Programs.Clear();
            this.OnModelChanged();
		}

        public void ResolvePrograms()
		{
            Logger.Debug("////////////////// Resolve Programs //////////////////");

			// make a collection of all geometry segments
			var allSegments = new List<LineSegment>();
			foreach (WallEntity we in this.wallEntities)
			{
                var lineSegments = we.Geometry.ConvertToLineSegments();
                foreach (LineSegment ls in lineSegments)
				{
                    Logger.Debug(ls.ToString());
                }

                allSegments.AddRange(lineSegments);
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
