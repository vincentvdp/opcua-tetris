using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Xml;
using System.Runtime.Serialization;
using Opc.Ua;

namespace Quickstarts.TetrisServer
{
    //-------------------------------------------------------------------------------------------------------------------------------

    #region ObjectType Identifiers for Tetris Events
    /// <summary>
    /// A class that declares constants for all ObjectTypes in the Model Design.
    /// </summary>
    public static partial class ObjectTypes_TetrisEvents
    {
        /// <summary>
        /// The identifier for my TetrisLinesMadeEventType ObjectType.
        /// </summary>
        public const uint TetrisLinesMadeEventType = 235;
    }
    #endregion

    #region Variable Identifiers for Tetris Events
    /// <summary>
    /// A class that declares constants for all Variables in the Model Design.
    /// </summary>
    public static partial class Variables_TetrisEvents
    {
        /// <summary>
        /// The identifier for the TetrisLinesMadeEventType_NrOfLines Variable.
        /// </summary>
        public const uint TetrisLinesMadeEventType_NrOfLines = 245;

        /// <summary>
        /// The identifier for the TetrisLinesMadeEventType_PointsScored Variable.
        /// </summary>
        public const uint TetrisLinesMadeEventType_PointsScored = 246;
    }
    #endregion

    //-------------------------------------------------------------------------------------------------------------------------------

    #region ObjectType NodeId's for Tetris Events
    /// <summary>
    /// A class that declares constants for all ObjectTypes in the Model Design.
    /// </summary>
    public static partial class ObjectTypeIds_TetrisEvents
    {
        /// <summary>
        /// The identifier for the TetrisLinesMadeEventType ObjectType.
        /// </summary>
        public static readonly ExpandedNodeId TetrisLinesMadeEventType
            = new ExpandedNodeId(ObjectTypes_TetrisEvents.TetrisLinesMadeEventType,
                                    Quickstarts.TetrisServer.Namespaces.Tetris);
    }
    #endregion

    #region Variable NodeId's for my Events
    /// <summary>
    /// A class that declares constants for all Variables in the Model Design.
    /// </summary>
    public static partial class VariableIds_TetrisEvents
    {
        /// <summary>
        /// The identifier for the TetrisLinesMadeEventType_NrOfLines Variable.
        /// </summary>        
        public static readonly ExpandedNodeId TetrisLinesMadeEventType_NrOfLines
            = new ExpandedNodeId(Variables_TetrisEvents.TetrisLinesMadeEventType_NrOfLines,
                                    Quickstarts.TetrisServer.Namespaces.Tetris);

        /// <summary>
        /// The identifier for the TetrisLinesMadeEventType_PointsScored Variable.
        /// </summary>
        public static readonly ExpandedNodeId TetrisLinesMadeEventType_PointsScored
            = new ExpandedNodeId(Variables_TetrisEvents.TetrisLinesMadeEventType_PointsScored,
                                    Quickstarts.TetrisServer.Namespaces.Tetris);
    }
    #endregion

    //-------------------------------------------------------------------------------------------------------------------------------

    #region BrowseName Declarations for Tetris Events
    /// <summary>
    /// Declares all of the BrowseNames used in the Model Design.
    /// </summary>
    public static partial class BrowseNames_TetrisEvents
    {
        /// <summary>
        /// The BrowseName for the CurrentStep component.
        /// </summary>
        public const string NrOfLines = "NrOfLines";

        /// <summary>
        /// The BrowseName for the CycleId component.
        /// </summary>
        public const string PointsScored = "PointsScored";

        /// <summary>
        /// The BrowseName for the SystemCycleStatusEventType component.
        /// </summary>
        public const string TetrisLinesMadeEventType = "TetrisLinesMadeEventType";
    }
    #endregion

    //-------------------------------------------------------------------------------------------------------------------------------

}