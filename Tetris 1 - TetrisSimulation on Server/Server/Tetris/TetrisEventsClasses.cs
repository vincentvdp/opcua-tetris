using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Xml;
using System.Runtime.Serialization;
using Opc.Ua;

namespace Quickstarts.TetrisServer
{
    #region TetrisLinesMadeEventState Class
    /// <summary>
    /// Stores an instance of the TetrisLinesMadeEventType ObjectType.
    /// </summary>
    public partial class TetrisLinesMadeEventState : BaseEventState
    {
        #region Constructors
        /// <summary>
        /// Initializes the type with its default attribute values.
        /// </summary>
        public TetrisLinesMadeEventState(NodeState parent)
            : base(parent)
        {
            //TBD
        }

        /// <summary>
        /// Returns the id of the default type definition node for the instance.
        /// </summary>
        protected override NodeId GetDefaultTypeDefinitionId(NamespaceTable namespaceUris)
        {
            return Opc.Ua.NodeId.Create(Quickstarts.TetrisServer.ObjectTypes_TetrisEvents.TetrisLinesMadeEventType,
                                        Quickstarts.TetrisServer.Namespaces.Tetris, namespaceUris);
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// A description for the NrOfLines Property.
        /// </summary>
        public PropertyState<uint> NrOfLines
        {
            get
            {
                return m_NrOfLines;
            }

            set
            {
                if (!Object.ReferenceEquals(m_NrOfLines, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_NrOfLines = value;
            }
        }

        /// <summary>
        /// A description for the PointsScored Property.
        /// </summary>
        public PropertyState<double> PointsScored
        {
            get
            {
                return m_PointsScored;
            }

            set
            {
                if (!Object.ReferenceEquals(m_PointsScored, value))
                {
                    ChangeMasks |= NodeStateChangeMasks.Children;
                }

                m_PointsScored = value;
            }
        }
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Populates a list with the children that belong to the node.
        /// </summary>
        /// <param name="context">The context for the system being accessed.</param>
        /// <param name="children">The list of children to populate.</param>
        public override void GetChildren(
            ISystemContext context,
            IList<BaseInstanceState> children)
        {
            if (m_NrOfLines != null)
            {
                children.Add(m_NrOfLines);
            }

            if (m_PointsScored != null)
            {
                children.Add(m_PointsScored);
            }

            base.GetChildren(context, children);
        }

        /// <summary>
        /// Finds the child with the specified browse name.
        /// </summary>
        protected override BaseInstanceState FindChild(
            ISystemContext context,
            QualifiedName browseName,
            bool createOrReplace,
            BaseInstanceState replacement)
        {
            if (QualifiedName.IsNull(browseName))
            {
                return null;
            }

            BaseInstanceState instance = null;

            switch (browseName.Name)
            {
                case Quickstarts.TetrisServer.BrowseNames_TetrisEvents.NrOfLines:
                    {
                        if (createOrReplace)
                        {
                            if (NrOfLines == null)
                            {
                                if (replacement == null)
                                {
                                    NrOfLines = new PropertyState<uint>(this);
                                }
                                else
                                {
                                    NrOfLines = (PropertyState<uint>)replacement;
                                }
                            }
                        }

                        instance = NrOfLines;
                        break;
                    }

                case Quickstarts.TetrisServer.BrowseNames_TetrisEvents.PointsScored:
                    {
                        if (createOrReplace)
                        {
                            if (PointsScored == null)
                            {
                                if (replacement == null)
                                {
                                    PointsScored = new PropertyState<double>(this);
                                }
                                else
                                {
                                    PointsScored = (PropertyState<double>)replacement;
                                }
                            }
                        }

                        instance = PointsScored;
                        break;
                    }
            }

            if (instance != null)
            {
                return instance;
            }

            return base.FindChild(context, browseName, createOrReplace, replacement);
        }
        #endregion

        #region Private Fields
        private PropertyState<uint> m_NrOfLines;
        private PropertyState<double> m_PointsScored;
        #endregion
    }
    #endregion
}