/* ========================================================================
 * Copyright (c) 2005-2010 The OPC Foundation, Inc. All rights reserved.
 *
 * OPC Foundation MIT License 1.00
 * 
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 *
 * The complete license agreement can be found here:
 * http://opcfoundation.org/License/MIT/1.00/
 * ======================================================================*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Threading;
using System.Reflection;
using Opc.Ua;
using Opc.Ua.Server;

namespace Quickstarts.TetrisServer
{
    /// <summary>
    /// A node manager for a server that exposes several variables.
    /// </summary>
    public class TetrisNodeManager : QuickstartNodeManager
    {
        #region Constructors
        /// <summary>
        /// Initializes the node manager.
        /// </summary>
        public TetrisNodeManager(IServerInternal server, ApplicationConfiguration configuration)
        :
            base(server, configuration, Namespaces.Tetris)
        {
            SystemContext.NodeIdFactory = this;

            // get the configuration for the node manager.
            m_configuration = configuration.ParseExtension<TetrisServerConfiguration>();

            // use suitable defaults if no configuration exists.
            if (m_configuration == null)
            {
                m_configuration = new TetrisServerConfiguration();
            }

            //----------------- Initialize the Tetris Functionality --------------------
            m_TetrisSimulation = new TetrisSimulation(); 
            m_TetrisGame = new TetrisGame();
            m_TetrisControlsForm = new TetrisServerControlsForm(m_TetrisGame);
            //m_TetrisControlsForm.Show();
            //--------------------------------------------------------------------------
        }
        #endregion
        
        #region IDisposable Members
        /// <summary>
        /// An overrideable version of the Dispose.
        /// </summary>
        protected override void Dispose(bool disposing)
        {  
            if (disposing)
            {
                // TBD
            }
        }
        #endregion

        #region INodeIdFactory Members
        /// <summary>
        /// Creates the NodeId for the specified node.
        /// </summary>
        public override NodeId New(ISystemContext context, NodeState node)
        {
            return node.NodeId;
        }
        #endregion

        #region INodeManager Members
        /// <summary>
        /// Does any initialization required before the address space can be used.
        /// </summary>
        /// <remarks>
        /// The externalReferences is an out parameter that allows the node manager to link to nodes
        /// in other node managers. For example, the 'Objects' node is managed by the CoreNodeManager and
        /// should have a reference to the root folder node(s) exposed by this node manager.  
        /// </remarks>
        public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            lock (Lock)
            {
                //----------------- A few examples of hard coded nodes in the Address Space -------------------
                //counter for the NodeId identifier:
                uint id = 1;
                //a first node:
                BaseObjectState testObjectNode = new BaseObjectState(null);
                // Set the attributes:
                testObjectNode.NodeId = new NodeId(id++, NamespaceIndex);
                testObjectNode.BrowseName = new QualifiedName("test_ObjectNode", NamespaceIndex);
                testObjectNode.DisplayName = testObjectNode.BrowseName.Name;
                testObjectNode.TypeDefinitionId = ObjectTypeIds.BaseObjectType;

                // ensure testObjectNode can be found via the server object: add references to/from it!
                IList<IReference> references = null;

                if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out references))
                {
                    externalReferences[ObjectIds.ObjectsFolder] = references = new List<IReference>();
                }

                testObjectNode.AddReference(ReferenceTypeIds.Organizes, true, ObjectIds.ObjectsFolder);
                references.Add(new NodeStateReference(ReferenceTypeIds.Organizes, false, testObjectNode.NodeId));

                //a second node:
                PropertyState testPropertyNode = new PropertyState(testObjectNode);
                // Set the attributes:
                testPropertyNode.NodeId = new NodeId(id++, NamespaceIndex);
                testPropertyNode.BrowseName = new QualifiedName("test_PropertyNode", NamespaceIndex);
                testPropertyNode.DisplayName = testPropertyNode.BrowseName.Name;
                testPropertyNode.TypeDefinitionId = VariableTypeIds.PropertyType;
                testPropertyNode.ReferenceTypeId = ReferenceTypeIds.HasProperty;
                testPropertyNode.DataType = DataTypeIds.Int32;
                testPropertyNode.ValueRank = ValueRanks.OneDimension;
                testPropertyNode.ArrayDimensions = new ReadOnlyList<uint>(new uint[] { 5 });
                testPropertyNode.Value = new int[] { 7, -18, 56, 0, 3 };
                //add this node as a child of the previous node:
                testObjectNode.AddChild(testPropertyNode);
                // save in dictionary. 
                AddPredefinedNode(SystemContext, testObjectNode);
                //---------------------------------------------------------------------------------------------
            } 
        }

        /// <summary>
        /// Frees any resources allocated for the address space.
        /// </summary>
        public override void DeleteAddressSpace()
        {
            lock (Lock)
            {
                // TBD
            }
        }

        /// <summary>
        /// Returns a unique handle for the node.
        /// </summary>
        protected override NodeHandle GetManagerHandle(ServerSystemContext context, NodeId nodeId, IDictionary<NodeId, NodeState> cache)
        {
            lock (Lock)
            {
                // quickly exclude nodes that are not in the namespace. 
                if (!IsNodeIdInNamespace(nodeId))
                {
                    return null;
                }

                NodeState node = null;

                if (!PredefinedNodes.TryGetValue(nodeId, out node))
                {
                    return null;
                }

                NodeHandle handle = new NodeHandle();

                handle.NodeId = nodeId;
                handle.Node = node;
                handle.Validated = true;

                return handle;
            } 
        }

        /// <summary>
        /// Verifies that the specified node exists.
        /// </summary>
        protected override NodeState ValidateNode(
            ServerSystemContext context,
            NodeHandle handle,
            IDictionary<NodeId, NodeState> cache)
        {
            // not valid if no root.
            if (handle == null)
            {
                return null;
            }

            // check if previously validated.
            if (handle.Validated)
            {
                return handle.Node;
            }
            
            // TBD

            return null;
        }
        #endregion

        #region Overridden Methods
        #endregion

        #region Private Fields
        private TetrisServerConfiguration m_configuration;
        //------------------- The underlying system: Tetris --------------------
        TetrisServerControlsForm m_TetrisControlsForm;
        TetrisGame m_TetrisGame;
        TetrisSimulation m_TetrisSimulation;
        #endregion
    }
}
