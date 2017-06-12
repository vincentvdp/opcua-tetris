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
            m_TetrisControlsForm.Show();
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
                //counter for the NodeId identifier:
                uint id = 1;

                // Define the nodes:

                #region sample nodes
                //----------------- A few examples of hard coded nodes in the Address Space -------------------
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
                testPropertyNode.ArrayDimensions = new ReadOnlyList<uint>(new uint[] {5});
                testPropertyNode.Value = new int[] { 7, -18, 56, 0, 3 };
                //add this node as a child of the previous node:
                testObjectNode.AddChild(testPropertyNode);
                // save in dictionary. 
                AddPredefinedNode(SystemContext, testObjectNode);
                //---------------------------------------------------------------------------------------------
                #endregion

                #region tetris simulation nodes
                //------------------------------ The nodes for the tetris simulation --------------------------

                // The object node under wich the other nodes containing the data will be organized:
                BaseObjectState tetrisSimulationNode = new BaseObjectState(null);
                tetrisSimulationNode.NodeId = new NodeId(id++, NamespaceIndex);
                tetrisSimulationNode.BrowseName = new QualifiedName("Tetris_Simulation", NamespaceIndex);
                tetrisSimulationNode.DisplayName = tetrisSimulationNode.BrowseName.Name;
                tetrisSimulationNode.SymbolicName = tetrisSimulationNode.BrowseName.Name;
                tetrisSimulationNode.TypeDefinitionId = ObjectTypeIds.BaseObjectType;

                // ensure tetrisSimulationNode can be found via the server object: add references to/from it!
                references = null;
                if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out references))
                {
                    externalReferences[ObjectIds.ObjectsFolder] = references = new List<IReference>();
                }
                // Forward Reference FROM the ObjectsFolder node TO our tetris node:
                references.Add(new NodeStateReference(ReferenceTypeIds.Organizes, false, tetrisSimulationNode.NodeId));

                // Inverse Reference FROM our tetris node TO the ObjectsFolder node:
                tetrisSimulationNode.AddReference(ReferenceTypeIds.Organizes, true, ObjectIds.ObjectsFolder);

                // -------------------- Nodes containing data -------------------
                // The nodes containing some simulated tetris data:

                //Score:
                BaseDataVariableState simulatedScoreNode = new BaseDataVariableState(tetrisSimulationNode);
                simulatedScoreNode.NodeId = new NodeId(id++, NamespaceIndex);
                simulatedScoreNode.BrowseName = new QualifiedName("simulated_Score", NamespaceIndex);
                simulatedScoreNode.DisplayName = simulatedScoreNode.BrowseName.Name;
                simulatedScoreNode.TypeDefinitionId = VariableTypeIds.BaseDataVariableType;
                simulatedScoreNode.ReferenceTypeId = ReferenceTypeIds.HasComponent;
                simulatedScoreNode.DataType = DataTypeIds.Int32;
                simulatedScoreNode.ValueRank = ValueRanks.Scalar;
                //add the reference from the Tetris_Simulation Object Node to this one:
                tetrisSimulationNode.AddChild(simulatedScoreNode);

                //Game Over status:
                BaseDataVariableState simulatedGameOverNode = new BaseDataVariableState(tetrisSimulationNode);
                simulatedGameOverNode.NodeId = new NodeId(id++, NamespaceIndex);
                simulatedGameOverNode.BrowseName = new QualifiedName("simulated_GameOver", NamespaceIndex);
                simulatedGameOverNode.DisplayName = simulatedGameOverNode.BrowseName.Name;
                simulatedGameOverNode.TypeDefinitionId = VariableTypeIds.BaseDataVariableType;
                simulatedGameOverNode.ReferenceTypeId = ReferenceTypeIds.HasComponent;
                simulatedGameOverNode.DataType = DataTypeIds.Boolean;
                simulatedGameOverNode.ValueRank = ValueRanks.Scalar;
                //add the reference from the Tetris_Simulation Object Node to this one:
                tetrisSimulationNode.AddChild(simulatedGameOverNode);

                //Paused status:
                BaseDataVariableState simulatedPausedNode = new BaseDataVariableState(tetrisSimulationNode);
                simulatedPausedNode.NodeId = new NodeId(id++, NamespaceIndex);
                simulatedPausedNode.BrowseName = new QualifiedName("simulated_Paused", NamespaceIndex);
                simulatedPausedNode.DisplayName = simulatedPausedNode.BrowseName.Name;
                simulatedPausedNode.TypeDefinitionId = VariableTypeIds.BaseDataVariableType;
                simulatedPausedNode.ReferenceTypeId = ReferenceTypeIds.HasComponent;
                simulatedPausedNode.DataType = DataTypeIds.Boolean;
                simulatedPausedNode.ValueRank = ValueRanks.Scalar;
                simulatedPausedNode.AccessLevel = AccessLevels.CurrentReadOrWrite;
                simulatedPausedNode.Value = false;
                //add the reference from the Tetris_Simulation Object Node to this one:
                tetrisSimulationNode.AddChild(simulatedPausedNode);
                //---------------------------------------------------------------
                
                AddPredefinedNode(SystemContext, tetrisSimulationNode);
                //---------------------------------------------------------------------------------------------
                #endregion

                #region teris game nodes
                //------------------------------ The nodes for the tetris game --------------------------------

                // The object node under wich the other nodes containing the data will be organized:
                BaseObjectState tetrisGameNode = new BaseObjectState(null);
                tetrisGameNode.NodeId = new NodeId(id++, NamespaceIndex);
                tetrisGameNode.BrowseName = new QualifiedName("Tetris_Game", NamespaceIndex);
                tetrisGameNode.DisplayName = tetrisGameNode.BrowseName.Name;
                tetrisGameNode.SymbolicName = tetrisGameNode.BrowseName.Name;
                tetrisGameNode.TypeDefinitionId = ObjectTypeIds.BaseObjectType;

                // ensure tetrisGameNode can be found via the server object: add references to/from it!
                references = null;
                if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out references))
                {
                    externalReferences[ObjectIds.ObjectsFolder] = references = new List<IReference>();
                }
                // Forward Reference FROM the ObjectsFolder node TO our tetris node:
                references.Add(new NodeStateReference(ReferenceTypeIds.Organizes, false, tetrisGameNode.NodeId));

                // Inverse Reference FROM our tetris node TO the ObjectsFolder node:
                tetrisGameNode.AddReference(ReferenceTypeIds.Organizes, true, ObjectIds.ObjectsFolder);

                // -------------------- Nodes containing data -------------------
                // The nodes containing some simulated tetris data:

                //Score:
                BaseDataVariableState scoreNode = new BaseDataVariableState(tetrisGameNode);
                scoreNode.NodeId = new NodeId(id++, NamespaceIndex);
                scoreNode.BrowseName = new QualifiedName("Score", NamespaceIndex);
                scoreNode.DisplayName = scoreNode.BrowseName.Name;
                scoreNode.TypeDefinitionId = VariableTypeIds.BaseDataVariableType;
                scoreNode.ReferenceTypeId = ReferenceTypeIds.HasComponent;
                scoreNode.DataType = DataTypeIds.Int32;
                scoreNode.ValueRank = ValueRanks.Scalar;
                //add the reference from the Tetris_Game Object Node to this one:
                tetrisGameNode.AddChild(scoreNode);

                //Game Over status:
                BaseDataVariableState gameOverNode = new BaseDataVariableState(tetrisGameNode);
                gameOverNode.NodeId = new NodeId(id++, NamespaceIndex);
                gameOverNode.BrowseName = new QualifiedName("GameOver", NamespaceIndex);
                gameOverNode.DisplayName = gameOverNode.BrowseName.Name;
                gameOverNode.TypeDefinitionId = VariableTypeIds.BaseDataVariableType;
                gameOverNode.ReferenceTypeId = ReferenceTypeIds.HasComponent;
                gameOverNode.DataType = DataTypeIds.Boolean;
                gameOverNode.ValueRank = ValueRanks.Scalar;
                //add the reference from the Tetris_Game Object Node to this one:
                tetrisGameNode.AddChild(gameOverNode);

                //Paused status:
                BaseDataVariableState pausedNode = new BaseDataVariableState(tetrisGameNode);
                pausedNode.NodeId = new NodeId(id++, NamespaceIndex);
                pausedNode.BrowseName = new QualifiedName("Paused", NamespaceIndex);
                pausedNode.DisplayName = pausedNode.BrowseName.Name;
                pausedNode.TypeDefinitionId = VariableTypeIds.BaseDataVariableType;
                pausedNode.ReferenceTypeId = ReferenceTypeIds.HasComponent;
                pausedNode.DataType = DataTypeIds.Boolean;
                pausedNode.ValueRank = ValueRanks.Scalar;
                //add the reference from the Tetris_Game Object Node to this one:
                tetrisGameNode.AddChild(pausedNode);

                //SecondsTillUnpause:
                BaseDataVariableState secondsTillUnpauseNode = new BaseDataVariableState(tetrisGameNode);
                secondsTillUnpauseNode.NodeId = new NodeId(id++, NamespaceIndex);
                secondsTillUnpauseNode.BrowseName = new QualifiedName("Seconds_Till_Unpause", NamespaceIndex);
                secondsTillUnpauseNode.DisplayName = secondsTillUnpauseNode.BrowseName.Name;
                secondsTillUnpauseNode.TypeDefinitionId = VariableTypeIds.BaseDataVariableType;
                secondsTillUnpauseNode.ReferenceTypeId = ReferenceTypeIds.HasComponent;
                secondsTillUnpauseNode.DataType = DataTypeIds.Int32;
                secondsTillUnpauseNode.ValueRank = ValueRanks.Scalar;
                //add the reference from the Tetris_Game Object Node to this one:
                tetrisGameNode.AddChild(secondsTillUnpauseNode);

                //SecondsTillUnpause:
                BaseDataVariableState fieldNode = new BaseDataVariableState(tetrisGameNode);
                fieldNode.NodeId = new NodeId(id++, NamespaceIndex);
                fieldNode.BrowseName = new QualifiedName("Field", NamespaceIndex);
                fieldNode.DisplayName = fieldNode.BrowseName.Name;
                fieldNode.TypeDefinitionId = VariableTypeIds.BaseDataVariableType;
                fieldNode.ReferenceTypeId = ReferenceTypeIds.HasComponent;
                fieldNode.DataType = DataTypeIds.Int32;
                fieldNode.ValueRank = ValueRanks.TwoDimensions;
                //fieldNode.ArrayDimensions = ???
                // => get the matrix (twodimensional array) dimension lengths:
                uint x = (uint)m_TetrisGame.DynamicField.GetLength(0);
                uint y = (uint)m_TetrisGame.DynamicField.GetLength(1);
                fieldNode.ArrayDimensions = new ReadOnlyList<uint>(new uint[] { x, y });
                //add the reference from the Tetris_Game Object Node to this one:
                tetrisGameNode.AddChild(fieldNode);
                //---------------------------------------------------------------

                AddPredefinedNode(SystemContext, tetrisGameNode);
                //---------------------------------------------------------------------------------------------
                #endregion

                #region tetris method node
                //------------------------------ The method node for the tetris game --------------------------

                // ------ The  method state itself: ------
                MethodState pauseMethodNode = new MethodState(tetrisGameNode);
                pauseMethodNode.NodeId = new NodeId(id++, NamespaceIndex);
                pauseMethodNode.BrowseName = new QualifiedName("Pause_Method", NamespaceIndex);
                pauseMethodNode.DisplayName = pauseMethodNode.BrowseName.Name;
                pauseMethodNode.SymbolicName = pauseMethodNode.BrowseName.Name;
                pauseMethodNode.Description = new LocalizedText("A method to pause the TetrisGame for X seconds");
                pauseMethodNode.ReferenceTypeId = ReferenceTypeIds.HasComponent;
                pauseMethodNode.UserExecutable = true;
                pauseMethodNode.Executable = true;

                // ------ Add the InputArguments: ------
                pauseMethodNode.InputArguments = new PropertyState<Argument[]>(pauseMethodNode);
                pauseMethodNode.InputArguments.NodeId = new NodeId(id++, NamespaceIndex);
                pauseMethodNode.InputArguments.BrowseName = BrowseNames.InputArguments;
                pauseMethodNode.InputArguments.DisplayName = new LocalizedText("InputArgs_PauseMethod");
                pauseMethodNode.InputArguments.SymbolicName = pauseMethodNode.InputArguments.DisplayName.Text;
                // These are properties of the method state (closer relation with parent than "HasComponent"):
                pauseMethodNode.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
                pauseMethodNode.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
                //Attributes related to the Value (= Argument): 
                pauseMethodNode.InputArguments.DataType = DataTypeIds.Argument;
                pauseMethodNode.InputArguments.ValueRank = ValueRanks.Scalar;
                // Create the Argument (= Value of PropertyNode 'InputArguments'):
                Argument[] args_in = new Argument[1]; // only one inputargument
                args_in[0] = new Argument();
                args_in[0].Name = "SecondsToPause";
                args_in[0].Description = "The number of seconds to pause the Tetris Game";
                args_in[0].DataType = DataTypeIds.UInt32;
                args_in[0].ValueRank = ValueRanks.Scalar;
                // Add the Argument as the Value of the method Node:
                pauseMethodNode.InputArguments.Value = args_in;

                // ------ Add the OutputArguments: ------
                pauseMethodNode.OutputArguments = new PropertyState<Argument[]>(pauseMethodNode);
                pauseMethodNode.OutputArguments.NodeId = new NodeId(id++, NamespaceIndex);
                pauseMethodNode.OutputArguments.BrowseName = BrowseNames.OutputArguments;
                pauseMethodNode.OutputArguments.DisplayName = new LocalizedText("OutputArgs_PauseMethod");
                pauseMethodNode.OutputArguments.SymbolicName = pauseMethodNode.OutputArguments.DisplayName.Text;
                // These are properties of the method state (closer relation with parent than "HasComponent"):
                pauseMethodNode.OutputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
                pauseMethodNode.OutputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
                //Attributes related to the Value (= Argument): 
                pauseMethodNode.OutputArguments.DataType = DataTypeIds.Argument;
                pauseMethodNode.OutputArguments.ValueRank = ValueRanks.Scalar;
                // Create the Argument (= Value of PropertyNode 'InputArguments'):
                Argument[] args_out = new Argument[1]; // only one outputargument
                args_out[0] = new Argument();
                args_out[0].Name = "SecondsToPause_StringResult";
                args_out[0].Description = "A string telling the Nr. of seconds the game is paused";
                args_out[0].DataType = DataTypeIds.String;
                args_out[0].ValueRank = ValueRanks.Scalar;
                // Add the Argument as the Value of the method Node:
                pauseMethodNode.OutputArguments.Value = args_out;
                
                // ------ set up myPauseMethod method handler: ------
                pauseMethodNode.OnCallMethod = new GenericMethodCalledEventHandler(OnPauseMethod);
                // (From now on OnPauseMethod(...) will be called whenever this method is called by the client!)

                // ------ Add the Method Node as child to the Tetris_Game parent node... ------
                tetrisGameNode.AddChild(pauseMethodNode);
                // ------ ... and add it to the predefined nodes: ------
                AddPredefinedNode(SystemContext, pauseMethodNode);
                
                //---------------------------------------------------------------------------------------------
                #endregion

                //------------------------ Initialize UpdateTimer---------------------------
                m_UpdateTimer = new Timer(UpdateTimerCallback, null, 1000, 500);
                //--------------------------------------------------------------------------                
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

        #region UpdateTimer

        private void UpdateTimerCallback(object state)
        {
            #region Update Tetris Simulation Nodes
            foreach (NodeState nodestate in PredefinedNodes.Values)
            {
                if (nodestate.SymbolicName == "Tetris_Simulation")
                {
                    //When the Tetris_Simulation Node is found, get it's children:
                    List<BaseInstanceState> children = new List<BaseInstanceState>();
                    nodestate.GetChildren(SystemContext, children);
                    //Iterate through the children and update the necessary nodes:
                    foreach (BaseInstanceState child in children)
                    {
                        BaseDataVariableState childvariable = child as BaseDataVariableState;
                        if (childvariable != null)
                        {
                            switch (childvariable.BrowseName.Name)
                            {
                                case "simulated_Score":
                                    childvariable.Value = m_TetrisSimulation.Score;
                                    break;
                                case "simulated_GameOver":
                                    childvariable.Value = m_TetrisSimulation.GameOver;
                                    break;
                                case "simulated_Paused":
                                    if (childvariable.Value != null)
                                    {
                                        m_TetrisSimulation.Paused = (bool)childvariable.Value;
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    // make known that this nodes and it's child nodes have changed:
                    nodestate.ClearChangeMasks(SystemContext, true);
                    // Tetris node found, no further need to iterate:
                    break;
                }
            }
            #endregion
            
            #region Update Tetris Games Nodes
            foreach (NodeState nodestate in PredefinedNodes.Values)
            {
                if (nodestate.SymbolicName == "Tetris_Game")
                {
                    //When the Tetris_Game Node is found, get it's children:
                    List<BaseInstanceState> children = new List<BaseInstanceState>();
                    nodestate.GetChildren(SystemContext, children);
                    //Iterate through the children and update the necessary nodes:
                    foreach (BaseInstanceState child in children)
                    {
                        BaseDataVariableState childvariable = child as BaseDataVariableState;
                        if (childvariable != null)
                        {
                            switch (childvariable.BrowseName.Name)
                            {
                                case "Score":
                                    childvariable.Value = m_TetrisGame.Score;
                                    break;
                                case "GameOver":
                                    childvariable.Value = m_TetrisGame.GameOver;
                                    break;
                                case "Paused":
                                    childvariable.Value = m_TetrisGame.Paused;
                                    break;
                                case "Seconds_Till_Unpause":
                                    childvariable.Value = m_TetrisGame.SecondsTillUnpause;
                                    break;
                                case "Field":
                                    childvariable.Value = m_TetrisGame.DynamicField;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    // make known that this nodes and it's child nodes have changed:
                    nodestate.ClearChangeMasks(SystemContext, true);
                    // Tetris node found, no further need to iterate:
                    break;
                }
            }
            #endregion            
        }

        #endregion

        #region OnPauseMethod
        public ServiceResult OnPauseMethod(
                    ISystemContext context,
                    MethodState method,
                    IList<object> inputArguments,
                    IList<object> outputArguments)
        {
            // All arguments must be provided:
            if (inputArguments.Count < 1) return StatusCodes.BadArgumentsMissing;

            // Check the data type of the input arguments:
            uint? secondsToPause = inputArguments[0] as uint?;
            if (secondsToPause == null) return StatusCodes.BadTypeMismatch;

            // Do the useful things this method is supposed to do:            
            lock (m_lock)
            {
                // Call the pause function on the tetris game:
                m_TetrisGame.pause((uint)secondsToPause);

                // The calling function sets default values for all output arguments,
                // you only need to update them here:                
                uint checkSeconds = m_TetrisGame.SecondsTillUnpause;
                // Did the "Pause for ... second" work?:
                if (checkSeconds == secondsToPause) // yes!
                {
                    outputArguments[0] = "Success: Game was paused for " + checkSeconds.ToString() + " seconds";
                    return ServiceResult.Good;
                }
                else // no!
                {
                    outputArguments[0] = "NO Success: Game was already manually paused at serverside!";
                    // The method was succesfully finished, but the result might not be what the client wanted, so try again later:
                    return StatusCodes.GoodCallAgain;
                }
            }
        }
        #endregion

        #region Private Fields
        private TetrisServerConfiguration m_configuration;
        //------------------- The underlying system: Tetris --------------------
        TetrisServerControlsForm m_TetrisControlsForm;
        TetrisGame m_TetrisGame;
        TetrisSimulation m_TetrisSimulation;
        Timer m_UpdateTimer;
        object m_lock = new object();
        #endregion
    }
}
