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
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms;
using System.IO;
using Opc.Ua;
using Opc.Ua.Client;

namespace Quickstarts.TetrisClient
{
    /// <summary>
    /// The main form for a simple Tetris Client application.
    /// </summary>
    public partial class MainForm : Form
    {
        #region Constructors
        /// <summary>
        /// Creates an empty form.
        /// </summary>
        private MainForm()
        {
            InitializeComponent();
        }
        
        /// <summary>
        /// Creates a form which uses the specified client configuration.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        public MainForm(ApplicationConfiguration configuration)
        {
            InitializeComponent();

            // save the configuration.
            m_configuration = configuration;

            // find the client certificate.
            m_clientCertificate = m_configuration.SecurityConfiguration.ApplicationCertificate.Find(true);  
            
            // set up a callback to handle certificate validation errors.
            m_configuration.CertificateValidator.CertificateValidation += new CertificateValidationEventHandler(CertificateValidator_CertificateValidation);

            // create the callback.
            m_MonitoredItem_Notification = new MonitoredItemNotificationEventHandler(MonitoredItem_Notification);
        }
        #endregion
        
        #region Private Fields
        private ApplicationConfiguration m_configuration;
        private X509Certificate2 m_clientCertificate;
        private Session m_session;
        private Subscription m_subscription;
        private MonitoredItemNotificationEventHandler m_MonitoredItem_Notification;
        private SessionReconnectHandler m_reconnectHandler;
        //----------------- Tetris Fields ----------------------
        private List<MonitoredItem> m_MonitoredItem_list_for_Tetris;
        private TetrisFieldViewer m_TetrisViewer;
        private NodeId m_pauseMethod_NodeId;
        private NodeId m_pauseMethod_Parent_NodeId;
        private List<string> m_eventFieldsToDisplay = 
            new List<string> { BrowseNames.EventType, BrowseNames.SourceName, BrowseNames.Time, BrowseNames.Message };
        private List<NodeId> m_eventTypes_Received = new List<NodeId> { ObjectTypeIds.BaseEventType };        
        //------------------------------------------------------
        #endregion

        #region Private Methods
        /// <summary>
        /// Finds the endpoint that best matches the current settings.
        /// </summary>
        private EndpointDescription SelectEndpoint()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                // determine the URL that was selected.
                string discoveryUrl = UrlCB.Text;

                if (UrlCB.SelectedIndex >= 0)
                {
                    discoveryUrl = (string)UrlCB.SelectedItem;
                }

                // return the selected endpoint.
                return FormUtils.SelectEndpoint(discoveryUrl, UseSecurityCK.Checked);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// Populates the branch in the tree view.
        /// </summary>
        /// <param name="sourceId">The NodeId of the Node to browse.</param>
        /// <param name="nodes">The node collect to populate.</param>
        private void PopulateBranch(NodeId sourceId, TreeNodeCollection nodes)
        {
            try
            {
                nodes.Clear();

                // find all of the components of the node.
                BrowseDescription nodeToBrowse1 = new BrowseDescription();

                nodeToBrowse1.NodeId = sourceId;
                nodeToBrowse1.BrowseDirection = BrowseDirection.Forward;
                nodeToBrowse1.ReferenceTypeId = ReferenceTypeIds.Aggregates;
                nodeToBrowse1.IncludeSubtypes = true;
                nodeToBrowse1.NodeClassMask = (uint)(NodeClass.Object | NodeClass.Variable | NodeClass.Method);
                nodeToBrowse1.ResultMask = (uint)BrowseResultMask.All;
                
                // find all nodes organized by the node.
                BrowseDescription nodeToBrowse2 = new BrowseDescription();

                nodeToBrowse2.NodeId = sourceId;
                nodeToBrowse2.BrowseDirection = BrowseDirection.Forward;
                nodeToBrowse2.ReferenceTypeId = ReferenceTypeIds.Organizes;
                nodeToBrowse2.IncludeSubtypes = true;
                nodeToBrowse2.NodeClassMask = (uint)(NodeClass.Object | NodeClass.Variable);
                nodeToBrowse2.ResultMask = (uint)BrowseResultMask.All;

                BrowseDescriptionCollection nodesToBrowse = new BrowseDescriptionCollection();
                nodesToBrowse.Add(nodeToBrowse1);
                nodesToBrowse.Add(nodeToBrowse2);

                // fetch references from the server.
                ReferenceDescriptionCollection references = FormUtils.Browse(m_session, nodesToBrowse, false);
                
                // process results.
                for (int ii = 0; ii < references.Count; ii++)
                {
                    ReferenceDescription target = references[ii];

                    // add node.
                    TreeNode child = new TreeNode(Utils.Format("{0}", target));
                    child.Tag = target;
                    child.Nodes.Add(new TreeNode());
                    nodes.Add(child);
                }

                // update the attributes display.
                DisplayAttributes(sourceId);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Displays the attributes and properties in the attributes view.
        /// </summary>
        /// <param name="sourceId">The NodeId of the Node to browse.</param>
        private void DisplayAttributes(NodeId sourceId)
        {
            try
            {
                AttributesLV.Items.Clear();

                ReadValueIdCollection nodesToRead = new ReadValueIdCollection();

                // attempt to read all possible attributes.
                for (uint ii = Attributes.NodeClass; ii <= Attributes.UserExecutable; ii++)
                {
                    ReadValueId nodeToRead = new ReadValueId();
                    nodeToRead.NodeId = sourceId;
                    nodeToRead.AttributeId = ii;
                    nodesToRead.Add(nodeToRead);
                }

                int startOfProperties = nodesToRead.Count;

                // find all of the pror of the node.
                BrowseDescription nodeToBrowse1 = new BrowseDescription();

                nodeToBrowse1.NodeId = sourceId;
                nodeToBrowse1.BrowseDirection = BrowseDirection.Forward;
                nodeToBrowse1.ReferenceTypeId = ReferenceTypeIds.HasProperty;
                nodeToBrowse1.IncludeSubtypes = true;
                nodeToBrowse1.NodeClassMask = 0;
                nodeToBrowse1.ResultMask = (uint)BrowseResultMask.All;

                BrowseDescriptionCollection nodesToBrowse = new BrowseDescriptionCollection();
                nodesToBrowse.Add(nodeToBrowse1);

                // fetch property references from the server.
                ReferenceDescriptionCollection references = FormUtils.Browse(m_session, nodesToBrowse, false);

                for (int ii = 0; ii < references.Count; ii++)
                {
                    // ignore external references.
                    if (references[ii].NodeId.IsAbsolute)
                    {
                        continue;
                    }

                    ReadValueId nodeToRead = new ReadValueId();
                    nodeToRead.NodeId = (NodeId)references[ii].NodeId;
                    nodeToRead.AttributeId = Attributes.Value;
                    nodesToRead.Add(nodeToRead);
                }

                // read all values.
                DataValueCollection results = null;
                DiagnosticInfoCollection diagnosticInfos = null;

                m_session.Read(
                    null,
                    0,
                    TimestampsToReturn.Neither,
                    nodesToRead,
                    out results,
                    out diagnosticInfos);

                ClientBase.ValidateResponse(results, nodesToRead);
                ClientBase.ValidateDiagnosticInfos(diagnosticInfos, nodesToRead);

                // process results.
                for (int ii = 0; ii < results.Count; ii++)
                {
                    string name = null;
                    string datatype = null;
                    string value = null;

                    // process attribute value.
                    if (ii < startOfProperties)
                    {
                        // ignore attributes which are invalid for the node.
                        if (results[ii].StatusCode == StatusCodes.BadAttributeIdInvalid)
                        {
                            continue;
                        }

                        // get the name of the attribute.
                        name = Attributes.GetBrowseName(nodesToRead[ii].AttributeId);

                        // display any unexpected error.
                        if (StatusCode.IsBad(results[ii].StatusCode))
                        {
                            datatype = Utils.Format("{0}", Attributes.GetDataTypeId(nodesToRead[ii].AttributeId));
                            value = Utils.Format("{0}", results[ii].StatusCode);
                        }

                        // display the value.
                        else
                        {
                            TypeInfo typeInfo = TypeInfo.Construct(results[ii].Value);

                            datatype = typeInfo.BuiltInType.ToString();

                            if (typeInfo.ValueRank >= ValueRanks.OneOrMoreDimensions)
                            {
                                datatype += "[]";
                            }

                            value = Utils.Format("{0}", results[ii].Value);
                        }
                    }

                    // process property value.
                    else
                    {
                        // ignore properties which are invalid for the node.
                        if (results[ii].StatusCode == StatusCodes.BadNodeIdUnknown)
                        {
                            continue;
                        }

                        // get the name of the property.
                        name = Utils.Format("{0}", references[ii-startOfProperties]);

                        // display any unexpected error.
                        if (StatusCode.IsBad(results[ii].StatusCode))
                        {
                            datatype = String.Empty;
                            value = Utils.Format("{0}", results[ii].StatusCode);
                        }

                        // display the value.
                        else
                        {
                            TypeInfo typeInfo = TypeInfo.Construct(results[ii].Value);

                            datatype = typeInfo.BuiltInType.ToString();

                            if (typeInfo.ValueRank >= ValueRanks.OneOrMoreDimensions)
                            {
                                datatype += "[]";
                            }

                            value = Utils.Format("{0}", results[ii].Value);
                        }
                    }

                    // add the attribute name/value to the list view.
                    ListViewItem item = new ListViewItem(name);
                    item.SubItems.Add(datatype);
                    item.SubItems.Add(value);
                    AttributesLV.Items.Add(item);
                }

                // adjust width of all columns.
                for (int ii = 0; ii < AttributesLV.Columns.Count; ii++)
                {
                    AttributesLV.Columns[ii].Width = -2;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Converts a monitoring filter to text for display.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>The deadback formatted as a string.</returns>
        private string DeadbandFilterToText(MonitoringFilter filter)
        {
            DataChangeFilter datachangeFilter = filter as DataChangeFilter;

            if (datachangeFilter != null)
            {
                if (datachangeFilter.DeadbandType == (uint)DeadbandType.Absolute)
                {
                    return Utils.Format("{0:##.##}", datachangeFilter.DeadbandValue);
                }

                if (datachangeFilter.DeadbandType == (uint)DeadbandType.Percent)
                {
                    return Utils.Format("{0:##.##}%", datachangeFilter.DeadbandValue);
                }
            }

            return "None";
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Connects to a server.
        /// </summary>
        private void Server_ConnectMI_Click(object sender, EventArgs e)
        {            
            try
            {
                // disconnect any existing session.
                Server_DisconnectMI_Click(sender, e);

                EndpointDescription endpointDescription = SelectEndpoint();
                EndpointConfiguration endpointConfiguration = EndpointConfiguration.Create(m_configuration);
                ConfiguredEndpoint endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);

                // create the channel object used to connect to the server.
                ITransportChannel channel = SessionChannel.Create(
                    m_configuration,
                    endpoint.Description,
                    endpoint.Configuration,
                    m_clientCertificate,
                    m_configuration.CreateMessageContext());

                // create the session object.
                m_session = new Session(channel, m_configuration, endpoint, m_clientCertificate);

                // sets the watchdog timer used to detect failures.
                m_session.KeepAliveInterval = 2000;

                // set up keep alive callback.
                m_session.KeepAlive += new KeepAliveEventHandler(Session_KeepAlive);

                // create the session.
                m_session.Open("Tetris Client", null);

                // populate the browse view.
                PopulateBranch(ObjectIds.ObjectsFolder, BrowseNodesTV.Nodes);

                ConnectedLB.Text = "Connected";
                ServerUrlLB.Text = endpointDescription.EndpointUrl;
                LastKeepAliveTimeLB.Text = "---";
                BrowseNodesTV.Enabled = true;
                MonitoredItemsLV.Enabled = true;

                // ------------------ Start Receiving Events --------------------
                StartReceivingEvents();
                // --------------------------------------------------------------

            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Make a monitored item on a subscription for Events
        /// </summary>
        private void StartReceivingEvents()
        {
            // Create the subscription is not yet created:
            if (m_subscription == null)
            {
                m_subscription = new Subscription(m_session.DefaultSubscription);
                m_subscription.PublishingEnabled = true;
                m_subscription.PublishingInterval = 1000;
                m_subscription.KeepAliveCount = 10;
                m_subscription.LifetimeCount = 30;
                m_subscription.MaxNotificationsPerPublish = 1000;
                m_subscription.Priority = 100;

                m_session.AddSubscription(m_subscription);

                m_subscription.Create();
            }

            // Create the Event filter:
            FilterDefinition filter = new FilterDefinition();
            // Choose the Sever Object for AreaId: we want to receive events from the whole server
            // (choosing another Node will result in getting events only from that section of the Address Space)
            filter.AreaId = ObjectIds.Server;
            filter.Severity = EventSeverity.Min;
            // We want to receive all types of events (later new fields will be dynamically added):
            filter.EventTypes = new NodeId[] { ObjectTypeIds.BaseEventType };
            filter.SelectClauses = filter.ConstructSelectClauses(m_session, (NodeId[])filter.EventTypes);

            // Create a monitored item based on the current filter settings:
            MonitoredItem monitoredItem_TetrisEvent = filter.CreateMonitoredItem(m_session);
            // Set the callback: this event handler will process the event notification:
            monitoredItem_TetrisEvent.Notification += 
                new MonitoredItemNotificationEventHandler(MonitoredItem_EventNotification);

            // Add the monitored item to the subscription:
            m_subscription.AddItem(monitoredItem_TetrisEvent);
            m_subscription.ApplyChanges();
        }

        /// <summary>
        /// Handles the reception of a notification for an Event
        /// </summary>
        private void MonitoredItem_EventNotification(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new MonitoredItemNotificationEventHandler(MonitoredItem_EventNotification), monitoredItem, e);
                return;
            }

            try
            {                
                // Get the event fields:
                EventFieldList eventFieldList = e.NotificationValue as EventFieldList;
                if (eventFieldList == null) return;

                // Get the EventTypeNodeId:
                NodeId eventTypeId = FormUtils.FindEventType(monitoredItem, eventFieldList);
                if (NodeId.IsNull(eventTypeId)) return;

                // Create string with the fields of the event:
                // first get the filter with its select clauses and then add the fields to the string.
                string eventString = "";
                EventFilter filter = monitoredItem.Filter as EventFilter;
                if (filter != null)
                {
                    // Iterate through all the select clauses and respective received event fields:
                    for (int ii = 0; ii < filter.SelectClauses.Count; ii++)
                    {
                        string fieldstring = "";
                        // The SelectClause must contain a BrowsePath representing the field
                        // AND: only display the field if it is in the list of fields we WANT to display
                        // AND: also the event field must contain a value (must not be null):
                        if (filter.SelectClauses[ii].BrowsePath.Count != 0
                            && m_eventFieldsToDisplay.Contains(filter.SelectClauses[ii].BrowsePath[0].Name)
                            && eventFieldList.EventFields[ii].Value != null)
                        {
                            // First set the field's name:
                            fieldstring = filter.SelectClauses[ii].BrowsePath[0].Name;
                            // Then add the value to the string representing the field:
                            switch (fieldstring)
                            {
                                case BrowseNames.EventType:
                                    // Special case: we want the EventType NAME, not the actual NodeId, to be displayed:
                                    fieldstring += ": " + (m_session.NodeCache.Find(eventTypeId)).DisplayName;
                                    break;
                                default:
                                    fieldstring += ": " + eventFieldList.EventFields[ii].Value.ToString();
                                    break;
                            }
                        }else continue; // get going with the next event field = next iteration!

                        // Add the fieldstring to the eventString that we'll later display:
                        eventString += fieldstring + Environment.NewLine;
                    }
                    // After all the event fields added: Report Event to TetrisViewer!
                    // (but not if it is not yet initialized!)
                    if(m_TetrisViewer != null) m_TetrisViewer.AddEvent(eventString);
                }
                

                // If the received eventTYpeId is not the BaseEventType nor an evenType already received before and added to the list:
                // * Find the EventType node, get its specific fields,
                //   and add them to the SelectClauses of the subscription and also to the list of fields to display.
                // * At the end of the loop: also set the "evenTypeId" variable to its superType NodeId:
                //   the while loop will be iterated till an EventType node is encoutered
                //   that has already been added to the the list of received events.
                //   (this way also the fields of the eventTypes in between the received EventType
                //   and the BaseEventType in the EventType Hierarchy are added to the subscription)
                while (!m_eventTypes_Received.Contains(eventTypeId))
                {
                    // Find the EventType node in the Address Space's Type Hierarchy ...
                    Node eventTypeNode = m_session.NodeCache.FetchNode(eventTypeId);
                    if (eventTypeNode == null) return; // (or generate an error if you want)
                    // ... and get the eventType node's properties (= event fields)...
                    IList<IReference> EventPropertyNodes = eventTypeNode.Find(ReferenceTypeIds.HasProperty, false);
                    // ... and extract the event fields' names (as a List of strings) from this list of property nodes:
                    List<string> newEventFields = new List<string>();
                    foreach (IReference referenceToPropertyNode in EventPropertyNodes)
                    {
                        string eventFieldBrowseName = m_session.NodeCache.Find(referenceToPropertyNode.TargetId).BrowseName.Name;
                        newEventFields.Add(eventFieldBrowseName);
                    }
                    
                    // Add the new event fields to the SelectClauses of the current filter ...
                    EventFilter eventFilter = monitoredItem.Filter as EventFilter;
                    if (eventFilter != null)
                    {
                        foreach (string fieldName in newEventFields)
                        {
                            eventFilter.AddSelectClause(eventTypeId, new QualifiedName(fieldName, eventTypeId.NamespaceIndex));
                        }
                    }
                    // ... and apply the changes:
                    monitoredItem.Filter = eventFilter;
                    m_subscription.ApplyChanges();

                    // Add the new event fields to the List of fields to be displayed:
                    m_eventFieldsToDisplay.AddRange(newEventFields);
                    
                    // Add the Event to our list of received EventTypes:
                    m_eventTypes_Received.Add(eventTypeId);

                    // Set the "eventTypeId" variable to its superType:
                    // this way the while loop will be iterated untill all superTypes are also added
                    // (the received event's superTypes can also have event fields relevant to their subtypes!)
                    eventTypeId = (NodeId)eventTypeNode.GetSuperType(m_session.TypeTree);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        /// <summary>
        /// Disconnects from the current session.
        /// </summary>
        private void Server_ReconnectComplete(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new EventHandler(Server_ReconnectComplete), sender, e);
                return;
            }

            try
            {
                // ignore callbacks from discarded objects.
                if (!Object.ReferenceEquals(sender, m_reconnectHandler))
                {
                    return;
                }

                m_session = m_reconnectHandler.Session;

                foreach (Subscription subscription in m_session.Subscriptions)
                {
                    m_subscription = subscription;
                    break;
                }

                m_reconnectHandler.Dispose();
                m_reconnectHandler = null;

                ConnectedLB.Text = "Connected";
                LastKeepAliveTimeLB.Text = "---";
                BrowseNodesTV.Enabled = true;
                MonitoredItemsLV.Enabled = true;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles a keep alive notification for a session by updating the status bar.
        /// </summary>
        private void Session_KeepAlive(Session session, KeepAliveEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new KeepAliveEventHandler(Session_KeepAlive), session, e);
                return;
            }

            try
            {
                // check for events from discarded sessions.
                if (!Object.ReferenceEquals(session, m_session))
                {
                    return;
                }

                if (ServiceResult.IsBad(e.Status))
                {
                    if (m_reconnectHandler == null)
                    {
                        LastKeepAliveTimeLB.Text = e.Status.ToString();
                        LastKeepAliveTimeLB.ForeColor = Color.Red;

                        ConnectedLB.Text = "Reconnecting";
                        BrowseNodesTV.Enabled = false;
                        MonitoredItemsLV.Enabled = false;
                        AttributesLV.Items.Clear();

                        m_reconnectHandler = new SessionReconnectHandler();
                        m_reconnectHandler.BeginReconnect(m_session, 10000, Server_ReconnectComplete);
                    }

                    return;
                }

                LastKeepAliveTimeLB.Text = Utils.Format("{0:HH:mm:ss}", e.CurrentTime.ToLocalTime());
                LastKeepAliveTimeLB.ForeColor = Color.Empty;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        /// <summary>
        /// Cleans up when the main form closes.
        /// </summary>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_reconnectHandler != null)
            {
                m_reconnectHandler.Dispose();
                m_reconnectHandler = null;
            }

            if (m_session != null)
            {
                m_session.Close(1000);
                m_session = null;
            }
        }

        /// <summary>
        /// Disconnects from the current session.
        /// </summary>
        private void Server_DisconnectMI_Click(object sender, EventArgs e)
        {
            try
            {
                // stop any reconnect operation.
                if (m_reconnectHandler != null)
                {
                    m_reconnectHandler.Dispose();
                    m_reconnectHandler = null;
                }

                // close the session.
                if (m_session != null)
                {
                    m_session.Close(10000);
                    m_session = null;
                    m_subscription = null;
                }

                ConnectedLB.Text = "Disconnected";
                ServerUrlLB.Text = "---";
                LastKeepAliveTimeLB.Text = "---";
                MonitoredItemsLV.Items.Clear();
                BrowseNodesTV.Nodes.Clear();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        /// <summary>
        /// Prompts the user to accept an untrusted certificate.
        /// </summary>
        private void CertificateValidator_CertificateValidation(CertificateValidator sender, CertificateValidationEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new CertificateValidationEventHandler(CertificateValidator_CertificateValidation), sender, e);
                return;
            }

            try
            {
                DialogResult result = MessageBox.Show(
                    e.Certificate.Subject,
                    "Untrusted Certificate", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Warning);

                e.Accept = (result == DialogResult.Yes);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Server_DiscoverMI_Click(object sender, EventArgs e)
        {
            try
            {
                string endpointUrl = new DiscoverServerDlg().ShowDialog(m_configuration);

                if (endpointUrl != null)
                {
                    UrlCB.SelectedIndex = -1;
                    UrlCB.Text = endpointUrl;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles the DropDown event of the UrlCB control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void UrlCB_DropDown(object sender, EventArgs e)
        {
            UrlCB.Items.Clear();

            // create a table of application descriptions that match the drop down entries.
            List<ApplicationDescription> lookupTable = new List<ApplicationDescription>();
            UrlCB.Tag = lookupTable;
            
            try
            {
                Cursor = Cursors.WaitCursor;

                try
                {
                    // set a short timeout because this is happening in the drop down event.
                    EndpointConfiguration configuration = EndpointConfiguration.Create(m_configuration);
                    configuration.OperationTimeout = 5000;

                    // Connect to the local discovery server and find the available servers.
                    using (DiscoveryClient client = DiscoveryClient.Create(new Uri("opc.tcp://localhost:4840"), configuration))
                    {
                        ApplicationDescriptionCollection servers = client.FindServers(null);

                        // populate the drop down list with the discovery URLs for the available servers.
                        for (int ii = 0; ii < servers.Count; ii++)
                        {
                            if (servers[ii].ApplicationType == ApplicationType.DiscoveryServer)
                            {
                                continue;
                            }

                            for (int jj = 0; jj < servers[ii].DiscoveryUrls.Count; jj++)
                            {
                                string discoveryUrl = servers[ii].DiscoveryUrls[jj];

                                // Many servers will use the '/discovery' suffix for the discovery endpoint.
                                // The URL without this prefix should be the base URL for the server. 
                                if (discoveryUrl.EndsWith("/discovery"))
                                {
                                    discoveryUrl = discoveryUrl.Substring(0, discoveryUrl.Length - "/discovery".Length);
                                }

                                // remove duplicates.
                                int index = UrlCB.FindStringExact(discoveryUrl);

                                if (index < 0)
                                {
                                    UrlCB.Items.Add(discoveryUrl);
                                }
                            }
                        }
                    }
                }
                finally
                {
                    Cursor = Cursors.Default;
                } 
            }
            catch (Exception)
            {
                // add the default URLs even if the LDS is not running.
                UrlCB.Items.Add("http://localhost:62541/Quickstarts/TetrisServer");
                UrlCB.Items.Add("opc.tcp://localhost:62542/Quickstarts/TetrisServer");
            }
        }

        /// <summary>
        /// Handles the Click event of the Help_ContentsMI control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Help_ContentsMI_Click(object sender, EventArgs e)
        {
            try
            {
                const string helpFile = "OPC UA Tetris Quickstart.chm";

                DirectoryInfo info = new DirectoryInfo(".");

                FileInfo[] files = info.GetFiles(helpFile, SearchOption.AllDirectories);

                while (files == null || files.Length == 0)
                {
                    // don't go any higher than the source directory.
                    if (info.Name == "Source")
                    {
                        return;
                    }

                    info = info.Parent;

                    if (info != null)
                    {
                        files = info.GetFiles(helpFile, SearchOption.AllDirectories);
                    }
                }
                
                System.Diagnostics.Process.Start("hh.exe", "\"" + files[0].FullName + "\"");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Fetches the children for a node the first time the node is expanded in the tree view.
        /// </summary>
        private void BrowseNodesTV_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            try
            {
                // check if node has already been expanded once.
                if (e.Node.Nodes.Count != 1 || e.Node.Nodes[0].Text != String.Empty)
                {
                    return;
                }

                // get the source for the node.
                ReferenceDescription reference = e.Node.Tag as ReferenceDescription;

                if (reference == null || reference.NodeId.IsAbsolute)
                {
                    e.Cancel = true;
                    return;
                }

                // populate children.
                PopulateBranch((NodeId)reference.NodeId, e.Node.Nodes);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Updates the display after a node is selected.
        /// </summary>
        private void BrowseNodesTV_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                // get the source for the node.
                ReferenceDescription reference = e.Node.Tag as ReferenceDescription;

                if (reference == null || reference.NodeId.IsAbsolute)
                {
                    return;
                }

                // populate children.
                PopulateBranch((NodeId)reference.NodeId, e.Node.Nodes);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Ensures the correct node is selected before displaying the context menu.
        /// </summary>
        private void BrowseNodesTV_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                BrowseNodesTV.SelectedNode = BrowseNodesTV.GetNodeAt(e.X, e.Y);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        /// <summary>
        /// Handles the Click event of the Browse_MonitorMI control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Browse_MonitorMI_Click(object sender, EventArgs e)
        {
            try
            {  
                // check if operation is currently allowed.
                if (m_session == null || BrowseNodesTV.SelectedNode == null)
                {
                    return;
                }

                // can only subscribe to local variables. 
                ReferenceDescription reference = (ReferenceDescription)BrowseNodesTV.SelectedNode.Tag;

                if (reference.NodeId.IsAbsolute || reference.NodeClass != NodeClass.Variable)
                {
                    return;
                }

                ListViewItem item = CreateMonitoredItem((NodeId)reference.NodeId, Utils.Format("{0}", reference));

                m_subscription.ApplyChanges();

                MonitoredItem monitoredItem = (MonitoredItem)item.Tag;
                                
                if (ServiceResult.IsBad(monitoredItem.Status.Error))
                {
                    item.SubItems[8].Text = monitoredItem.Status.Error.StatusCode.ToString();
                }

                //item.SubItems.Add(monitoredItem.DisplayName); // Original BUG (should be commented out)
                item.SubItems[2].Text = monitoredItem.MonitoringMode.ToString();
                item.SubItems[3].Text = monitoredItem.SamplingInterval.ToString();
                item.SubItems[4].Text = DeadbandFilterToText(monitoredItem.Filter);
                
                MonitoredItemsLV.Columns[0].Width = -2;
                MonitoredItemsLV.Columns[1].Width = -2;
                MonitoredItemsLV.Columns[8].Width = -2;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles the Click event of the Browse_MonitorMI_Tetris control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Browse_MonitorMI_Tetris_Click(object sender, EventArgs e)
        {
            try
            {
                // Check the number of ListViewItems (= number of Monitored Items (= MI)):
                int countBefore = MonitoredItemsLV.Items.Count;

                // Call the event handler for creating a normal MI (so it is also listed in the ListView):
                Browse_MonitorMI_Click(sender, e);

                // The last Item added to the ListView for MI's i the one created in Browse_MonitorMI_Click.
                // But first check whether Browse_MonitorMI_Click did not abort before creating the new MI:
                int countAfter = MonitoredItemsLV.Items.Count;
                if (countAfter > countBefore) //this means a monitored item was succesfully created
                {
                    // get the last added item and get its related Monitored Item (via its 'Tag'):
                    MonitoredItem monitoredItem = (MonitoredItem)MonitoredItemsLV.Items[countAfter - 1].Tag;
                    // add this item to the list of MI's that are related to our Tetris Game and TetrisViewer,
                    // and also initialize and show the form if not yet done:                
                    if (monitoredItem.NodeClass == NodeClass.Variable) // only Variable Nodes can provide data
                    {
                        if (m_MonitoredItem_list_for_Tetris == null)
                        { m_MonitoredItem_list_for_Tetris = new List<MonitoredItem>(); }
                        m_MonitoredItem_list_for_Tetris.Add(monitoredItem);

                        if (m_TetrisViewer == null || m_TetrisViewer.IsDisposed)
                        {
                            m_TetrisViewer = new TetrisFieldViewer();
                            m_TetrisViewer.Show();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles the Click event of the Browse_SetPauseMethod control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Browse_SetPauseMethod_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if operation is currently allowed:
                if (m_session == null || BrowseNodesTV.SelectedNode == null) return;

                // Get the tag (= reference description) of the selected node:
                ReferenceDescription methodReferenceDescription = 
                    (ReferenceDescription)BrowseNodesTV.SelectedNode.Tag;

                // Check whether it's indeed a Method node + check whether it's the correct method:
                if (methodReferenceDescription.NodeClass != NodeClass.Method
                    || methodReferenceDescription.BrowseName.Name != "Pause_Method") return;

                // Also check whether the parent is a Tetris_Game Object Node:
                ReferenceDescription parentReferenceDescription = 
                    (ReferenceDescription)BrowseNodesTV.SelectedNode.Parent.Tag;
                if (parentReferenceDescription.NodeClass != NodeClass.Object
                    || parentReferenceDescription.BrowseName.Name != "Tetris_Game") return;

                // We got this far without returning, so those two nodes are the correct nodes:
                m_pauseMethod_NodeId = (NodeId)methodReferenceDescription.NodeId;
                m_pauseMethod_Parent_NodeId = (NodeId)parentReferenceDescription.NodeId;

                // Just left with enabling the calling of the method through the "Pause for ..." button of the TetrisViewer form
                // (but first initialize the TetrisViewer form if not yet done)
                if (m_TetrisViewer == null || m_TetrisViewer.IsDisposed)
                {
                    m_TetrisViewer = new TetrisFieldViewer();
                    m_TetrisViewer.Show();
                }
                m_TetrisViewer.OnPauseClicked = 
                    new TetrisFieldViewer.PauseMethodClickedEventHandler(PauseMethod_in_TetrisViewer_Click);

                // Indicate succesful coupling of the pause method to the TetrisViewer form:
                MessageBox.Show("Method Succesfully coupled to TetrisViewer Form \"Pause for ...\" button!");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles the Click event of the "Pause for ..." button in the Tetris Viewer form.
        /// </summary>
        /// <param name="secondsToPause">The number of seconds to pause the "Tetris process" on the server.</param>
        private string PauseMethod_in_TetrisViewer_Click(uint secondsToPause)
        {
            //Simply just call the method (necessary arguments (two NodeId's) were previously determined):
            IList<object> outputArguments = m_session.Call(
                m_pauseMethod_Parent_NodeId,
                m_pauseMethod_NodeId,
                secondsToPause);
            // return the returned outputarguments:
            return outputArguments[0].ToString();
        }

        /// <summary>
        /// Creates the monitored item.
        /// </summary>
        private ListViewItem CreateMonitoredItem(NodeId nodeId, string displayName)
        {
            if (m_subscription == null)
            {
                m_subscription = new Subscription(m_session.DefaultSubscription);

                m_subscription.PublishingEnabled = true;
                m_subscription.PublishingInterval = 1000;
                m_subscription.KeepAliveCount = 10;
                m_subscription.LifetimeCount = 10;
                m_subscription.MaxNotificationsPerPublish = 1000;
                m_subscription.Priority = 100;

                m_session.AddSubscription(m_subscription);

                m_subscription.Create();
            }

            // add the new monitored item.
            MonitoredItem monitoredItem = new MonitoredItem(m_subscription.DefaultItem);

            monitoredItem.StartNodeId = nodeId;
            monitoredItem.AttributeId = Attributes.Value;
            monitoredItem.DisplayName = displayName;
            monitoredItem.MonitoringMode = MonitoringMode.Reporting;
            monitoredItem.SamplingInterval = 1000;
            monitoredItem.QueueSize = 0;
            monitoredItem.DiscardOldest = false;

            monitoredItem.Notification += m_MonitoredItem_Notification;

            m_subscription.AddItem(monitoredItem);

            // add the attribute name/value to the list view.
            ListViewItem item = new ListViewItem(monitoredItem.ClientHandle.ToString());
            monitoredItem.Handle = item;

            item.SubItems.Add(monitoredItem.DisplayName);
            item.SubItems.Add(monitoredItem.MonitoringMode.ToString());
            item.SubItems.Add(monitoredItem.SamplingInterval.ToString());
            item.SubItems.Add(DeadbandFilterToText(monitoredItem.Filter));
            item.SubItems.Add(String.Empty); //for value
            item.SubItems.Add(String.Empty); //for quality
            item.SubItems.Add(String.Empty); //for timestamp
            item.SubItems.Add(String.Empty); //for error

            item.Tag = monitoredItem;
            MonitoredItemsLV.Items.Add(item);

            if (ServiceResult.IsBad(monitoredItem.Status.Error))
            {
                item.SubItems[8].Text = monitoredItem.Status.Error.StatusCode.ToString();
            }
            
            return item;
        }

        /// <summary>
        /// Prompts the use to write the value of a varible.
        /// </summary>
        private void Browse_WriteMI_Click(object sender, EventArgs e)
        {
            try
            {
                // check if operation is currently allowed.
                if (m_session == null || BrowseNodesTV.SelectedNode == null)
                {
                    return;
                }

                // can only subscribe to local variables. 
                ReferenceDescription reference = (ReferenceDescription)BrowseNodesTV.SelectedNode.Tag;

                if (reference.NodeId.IsAbsolute || reference.NodeClass != NodeClass.Variable)
                {
                    return;
                }

                new WriteValueDlg().ShowDialog(m_session, (NodeId)reference.NodeId, Attributes.Value);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        /// <summary>
        /// Handles the Click event of the Browse_ReadHistoryMI control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Browse_ReadHistoryMI_Click(object sender, EventArgs e)
        {
            try
            {
                // check if operation is currently allowed.
                if (m_session == null || BrowseNodesTV.SelectedNode == null)
                {
                    return;
                }

                // can only subscribe to local variables. 
                ReferenceDescription reference = (ReferenceDescription)BrowseNodesTV.SelectedNode.Tag;

                if (reference.NodeId.IsAbsolute || reference.NodeClass != NodeClass.Variable)
                {
                    return;
                }

                new ReadHistoryDlg().ShowDialog(m_session, (NodeId)reference.NodeId);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Updates the display with a new value for a monitored variable. 
        /// </summary>
        private void MonitoredItem_Notification(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new MonitoredItemNotificationEventHandler(MonitoredItem_Notification), monitoredItem, e);
                return;
            }

            try
            {
                if (m_session == null)
                {
                    return;
                }

                MonitoredItemNotification notification = e.NotificationValue as MonitoredItemNotification;

                if (notification == null)
                {
                    return;
                }

                ListViewItem item = (ListViewItem)monitoredItem.Handle;

                item.SubItems[5].Text = Utils.Format("{0}", notification.Value.WrappedValue);
                item.SubItems[6].Text = Utils.Format("{0}", notification.Value.StatusCode);
                item.SubItems[7].Text = Utils.Format("{0:HH:mm:ss.fff}", notification.Value.ServerTimestamp.ToLocalTime());

                // ----------------------- Update the TetrisViewer (if needed): ---------------------------
                if (m_MonitoredItem_list_for_Tetris != null)
                {
                    if (m_MonitoredItem_list_for_Tetris.Contains(monitoredItem))
                    {
                        UpdateTetrisViewer(notification, monitoredItem.DisplayName);
                    }
                }
                // ----------------------------------------------------------------------------------------
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Updates the TetrisViewer (a new notification has arrived).
        /// </summary>
        private void UpdateTetrisViewer(MonitoredItemNotification notification, string displayName)
        {
            try
            {
                // Update a field of the TetrisViewer according to the passed displayName string:
                switch (displayName)
                {
                    case "Score":
                        m_TetrisViewer.Score = (int)notification.Value.Value;
                        break;
                    case "GameOver":
                        if ((bool)notification.Value.Value) m_TetrisViewer.MyMessage = "GAME OVER!!!";
                        else m_TetrisViewer.MyMessage = "Game Reset!";
                        break;
                    case "Paused":
                        m_TetrisViewer.GameActive = !(bool)notification.Value.Value;
                        break;
                    case "Seconds_Till_Unpause":
                        m_TetrisViewer.SecsRemaining = Convert.ToUInt32(notification.Value.Value);
                        break;
                    case "Field":
                        m_TetrisViewer.Field = (int[,])((Matrix)notification.Value.Value).ToArray();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Changes the monitoring mode for the currently selected monitored items.
        /// </summary>
        private void Monitoring_MonitoringMode_Click(object sender, EventArgs e)
        {
            try
            {
                // check if operation is currently allowed.
                if (m_session == null || m_subscription == null || MonitoredItemsLV.SelectedItems.Count == 0)
                {
                    return;
                }

                // determine the monitoring mode being requested.
                MonitoringMode monitoringMode = MonitoringMode.Disabled;

                if (sender == Monitoring_MonitoringMode_ReportingMI)
                {
                    monitoringMode = MonitoringMode.Reporting;
                }

                if (sender == Monitoring_MonitoringMode_SamplingMI)
                {
                    monitoringMode = MonitoringMode.Sampling;
                }

                // update the monitoring mode.
                List<MonitoredItem> itemsToChange = new List<MonitoredItem>();

                for (int ii = 0; ii < MonitoredItemsLV.SelectedItems.Count; ii++)
                {
                    MonitoredItem monitoredItem = MonitoredItemsLV.SelectedItems[ii].Tag as MonitoredItem;

                    if (monitoredItem != null)
                    {
                        itemsToChange.Add(monitoredItem);
                    }
                }

                // apply the changes to the server.
                m_subscription.SetMonitoringMode(monitoringMode, itemsToChange);

                // update the display.
                for (int ii = 0; ii < itemsToChange.Count; ii++)
                {
                    ListViewItem item = itemsToChange[ii].Handle as ListViewItem;

                    if (item != null)
                    {
                        item.SubItems[8].Text = String.Empty;

                        if (ServiceResult.IsBad(itemsToChange[ii].Status.Error))
                        {
                            item.SubItems[8].Text = itemsToChange[ii].Status.Error.StatusCode.ToString();
                        }

                        item.SubItems[2].Text = itemsToChange[ii].Status.MonitoringMode.ToString();
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Changes the sampling interval for the currently selected monitored items.
        /// </summary>
        private void Monitoring_SamplingInterval_Click(object sender, EventArgs e)
        {
            try
            {
                // check if operation is currently allowed.
                if (m_session == null || m_subscription == null || MonitoredItemsLV.SelectedItems.Count == 0)
                {
                    return;
                }

                // determine the sampling interval being requested.
                double samplingInterval = 0;

                if (sender == Monitoring_SamplingInterval_1000MI)
                {
                    samplingInterval = 1000;
                }
                else if (sender == Monitoring_SamplingInterval_2500MI)
                {
                    samplingInterval = 2500;
                }
                else if (sender == Monitoring_SamplingInterval_5000MI)
                {
                    samplingInterval = 5000;
                }

                // update the monitoring mode.
                List<MonitoredItem> itemsToChange = new List<MonitoredItem>();

                for (int ii = 0; ii < MonitoredItemsLV.SelectedItems.Count; ii++)
                {
                    MonitoredItem monitoredItem = MonitoredItemsLV.SelectedItems[ii].Tag as MonitoredItem;

                    if (monitoredItem != null)
                    {
                        monitoredItem.SamplingInterval = (int)samplingInterval;
                        itemsToChange.Add(monitoredItem);
                    }
                }

                // apply the changes to the server.
                m_subscription.ApplyChanges();

                // update the display.
                for (int ii = 0; ii < itemsToChange.Count; ii++)
                {
                    ListViewItem item = itemsToChange[ii].Handle as ListViewItem;

                    if (item != null)
                    {
                        item.SubItems[8].Text = String.Empty;

                        if (ServiceResult.IsBad(itemsToChange[ii].Status.Error))
                        {
                            item.SubItems[8].Text = itemsToChange[ii].Status.Error.StatusCode.ToString();
                        }

                        item.SubItems[3].Text = itemsToChange[ii].Status.SamplingInterval.ToString();
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Changes the deadband for the currently selected monitored items.
        /// </summary>
        private void Monitoring_Deadband_Click(object sender, EventArgs e)
        {
            try
            {
                // check if operation is currently allowed.
                if (m_session == null || m_subscription == null || MonitoredItemsLV.SelectedItems.Count == 0)
                {
                    return;
                }

                // determine the filter being requested.
                DataChangeFilter filter = new DataChangeFilter();
                filter.Trigger = DataChangeTrigger.StatusValue;

                if (sender == Monitoring_Deadband_Absolute_5MI)
                {
                    filter.DeadbandType = (uint)DeadbandType.Absolute;
                    filter.DeadbandValue = 5.0;
                }
                else if (sender == Monitoring_Deadband_Absolute_10MI)
                {
                    filter.DeadbandType = (uint)DeadbandType.Absolute;
                    filter.DeadbandValue = 10.0;
                }
                else if (sender == Monitoring_Deadband_Absolute_25MI)
                {
                    filter.DeadbandType = (uint)DeadbandType.Absolute;
                    filter.DeadbandValue = 25.0;
                }
                else if (sender == Monitoring_Deadband_Percentage_1MI)
                {
                    filter.DeadbandType = (uint)DeadbandType.Percent;
                    filter.DeadbandValue = 1.0;
                }
                else if (sender == Monitoring_Deadband_Percentage_5MI)
                {
                    filter.DeadbandType = (uint)DeadbandType.Percent;
                    filter.DeadbandValue = 5.0;
                }
                else if (sender == Monitoring_Deadband_Percentage_10MI)
                {
                    filter.DeadbandType = (uint)DeadbandType.Percent;
                    filter.DeadbandValue = 10.0;
                }
                else
                {
                    filter = null;
                }

                // update the monitoring mode.
                List<MonitoredItem> itemsToChange = new List<MonitoredItem>();

                for (int ii = 0; ii < MonitoredItemsLV.SelectedItems.Count; ii++)
                {
                    MonitoredItem monitoredItem = MonitoredItemsLV.SelectedItems[ii].Tag as MonitoredItem;

                    if (monitoredItem != null)
                    {
                        monitoredItem.Filter = filter;
                        itemsToChange.Add(monitoredItem);
                    }
                }

                // apply the changes to the server.
                m_subscription.ApplyChanges();

                // update the display.
                for (int ii = 0; ii < itemsToChange.Count; ii++)
                {
                    ListViewItem item = itemsToChange[ii].Handle as ListViewItem;

                    if (item != null)
                    {
                        item.SubItems[8].Text = String.Empty;

                        if (ServiceResult.IsBad(itemsToChange[ii].Status.Error))
                        {
                            itemsToChange[ii].Filter = null;
                            item.SubItems[8].Text = itemsToChange[ii].Status.Error.StatusCode.ToString();
                        }

                        item.SubItems[4].Text = DeadbandFilterToText(itemsToChange[ii].Status.Filter);
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles the Click event of the Monitoring_DeleteMI control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Monitoring_DeleteMI_Click(object sender, EventArgs e)
        {
            try
            {
                // check if operation is currently allowed.
                if (MonitoredItemsLV.SelectedItems.Count == 0)
                {
                    return;
                }

                // collect the items to delete.
                List<ListViewItem> itemsToDelete = new List<ListViewItem>();

                for (int ii = 0; ii < MonitoredItemsLV.SelectedItems.Count; ii++)
                {
                    MonitoredItem monitoredItem = MonitoredItemsLV.SelectedItems[ii].Tag as MonitoredItem;

                    if (monitoredItem != null)
                    {
                        monitoredItem.Notification -= m_MonitoredItem_Notification;
                        itemsToDelete.Add(MonitoredItemsLV.SelectedItems[ii]);

                        if (m_subscription != null)
                        {
                            m_subscription.RemoveItem(monitoredItem);
                        }
                    }
                }

                // update the server.
                if (m_subscription != null)
                {
                    m_subscription.ApplyChanges();

                    // check the status.
                    for (int ii = 0; ii < itemsToDelete.Count; ii++)
                    {
                        MonitoredItem monitoredItem = itemsToDelete[ii].Tag as MonitoredItem;

                        if (ServiceResult.IsBad(monitoredItem.Status.Error))
                        {
                            itemsToDelete[ii].SubItems[8].Text = monitoredItem.Status.Error.StatusCode.ToString();
                            continue;
                        }
                    }
                }

                // remove the items.
                for (int ii = 0; ii < itemsToDelete.Count; ii++)
                {
                    itemsToDelete[ii].Remove();
                }

                MonitoredItemsLV.Columns[0].Width = -2;
                MonitoredItemsLV.Columns[1].Width = -2;
                MonitoredItemsLV.Columns[8].Width = -2;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BrowsingMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Browse_MonitorMI.Enabled = true;
            Browse_ReadHistoryMI.Enabled = true;
            Browse_WriteMI.Enabled = true;

            if (m_session == null || BrowseNodesTV.SelectedNode == null)
            {
                Browse_MonitorMI.Enabled = false;
                Browse_ReadHistoryMI.Enabled = false;
                Browse_WriteMI.Enabled = false;
                return;
            }

            ReferenceDescription reference = (ReferenceDescription)BrowseNodesTV.SelectedNode.Tag;

            if (reference.NodeId.IsAbsolute || reference.NodeClass != NodeClass.Variable)
            {
                Browse_MonitorMI.Enabled = false;
                Browse_ReadHistoryMI.Enabled = false;
                Browse_WriteMI.Enabled = false;
                return;
            }
        }

        private void Monitoring_WriteMI_Click(object sender, EventArgs e)
        {
            try
            {
                // check if operation is currently allowed.
                if (m_session == null || m_subscription == null || MonitoredItemsLV.SelectedItems.Count == 0)
                {
                    return;
                }

                MonitoredItem monitoredItem = MonitoredItemsLV.SelectedItems[0].Tag as MonitoredItem;

                if (monitoredItem != null)
                {
                    new WriteValueDlg().ShowDialog(m_session, (NodeId)monitoredItem.ResolvedNodeId, Attributes.Value);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Creates monitored items from a saved list of node ids.
        /// </summary>
        private void File_LoadMI_Click(object sender, EventArgs e)
        {
            try
            {
                // load previously saved items from a file.
                List<string> aliases = new List<string>();

                using (StreamReader reader = new StreamReader(".\\SavedItems.txt", System.Text.Encoding.UTF8))
                {
                    string alias = reader.ReadLine();

                    while (alias != null)
                    {
                        alias = alias.Trim();

                        if (!String.IsNullOrEmpty(alias))
                        {
                            aliases.Add(alias);
                        }

                        alias = reader.ReadLine();
                    }
                }

                // an alias is a identifier to a node that can be persisted.
                // aliases are easier to use because they do not depend on dynamic namespace indexes.
                // some servers will also provide hierarchial human readable aliases that can be used in wildcard operations.
                // however, they will not be supported by all servers (note: they are only a proposed UA feature at this time).
                // if a server does not support aliases the absolute node id syntax can be used.
                IList<object> results = null;

                try
                {
                    results = m_session.Call(
                         ObjectTypeIds.ServerType,
                         MethodIds.ServerType_LookupNodeIdsForAliases,
                         new object[] { aliases.ToArray() });
                }
                catch (Exception)
                {
                    // create an empty list if the server does not support the method.
                    results = new object[] { new NodeId[aliases.Count] };
                }

                // remove the existing nodes.
                List<MonitoredItem> itemsToDelete = new List<MonitoredItem>();

                for (int ii = 0; ii < MonitoredItemsLV.Items.Count; ii++)
                {
                    MonitoredItem monitoredItem = MonitoredItemsLV.Items[ii].Tag as MonitoredItem;

                    if (monitoredItem != null)
                    {
                        itemsToDelete.Add(monitoredItem);
                    }
                }

                MonitoredItemsLV.Items.Clear();

                if (m_subscription != null)
                {
                    m_subscription.RemoveItems(itemsToDelete);
                }

                // add the new ones.
                List<NodeId> nodesToAdd = new List<NodeId>();
                List<string> displayNames = new List<string>();

                if (results.Count > 0)
                {
                    NodeId[] nodeIds = results[0] as NodeId[];

                    if (nodeIds != null)
                    {
                        for (int ii = 0; ii < nodeIds.Length; ii++)
                        {
                            // attempt to parse the alias as a node id if lookup failed.
                            if (NodeId.IsNull(nodeIds[ii]))
                            {
                                try
                                {
                                    nodeIds[ii] = ExpandedNodeId.Parse(aliases[ii], m_session.NamespaceUris);
                                }
                                catch (Exception)
                                {
                                    nodeIds[ii] = null;
                                }
                            }

                            // add node to list.
                            if (!NodeId.IsNull(nodeIds[ii]))
                            {
                                nodesToAdd.Add(nodeIds[ii]);
                                displayNames.Add(aliases[ii]);
                            }
                        }
                    }
                }

                // create items.
                for (int ii = 0; ii < nodesToAdd.Count; ii++)
                {
                    CreateMonitoredItem(nodesToAdd[ii], displayNames[ii]);
                }

                // apply changes.
                m_subscription.ApplyChanges();

                MonitoredItemsLV.Columns[0].Width = -2;
                MonitoredItemsLV.Columns[1].Width = -2;
                MonitoredItemsLV.Columns[8].Width = -2;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Saves the current monitored items.
        /// </summary>
        private void File_SaveMI_Click(object sender, EventArgs e)
        {
            try
            {
                List<NodeId> nodeIds = new List<NodeId>();

                for (int ii = 0; ii < MonitoredItemsLV.Items.Count; ii++)
                {
                    MonitoredItem monitoredItem = MonitoredItemsLV.Items[ii].Tag as MonitoredItem;

                    if (monitoredItem != null)
                    {
                        nodeIds.Add(monitoredItem.ResolvedNodeId);
                    }
                }

                // an alias is a identifier to a node that can be persisted.
                // aliases are easier to use because they do not depend on dynamic namespace indexes.
                // some servers will also provide hierarchial human readable aliases that can be used in wildcard operations.
                // however, they will not be supported by all servers (note: they are only a proposed UA feature at this time).
                // if a server does not support aliases the absolute node id syntax can be used.
                IList<object> results = null;

                try
                {
                    m_session.Call(
                         ObjectTypeIds.ServerType,
                         MethodIds.ServerType_LookupAliasesForNodeIds,
                         new object[] { nodeIds.ToArray() }); ;
                }
                catch (Exception)
                {
                    // create an empty list if the server does not support the method.
                    results = new object[] { new string[nodeIds.Count] };
                }


                if (results.Count > 0)
                {
                    string[] aliases = results[0] as string[];

                    if (aliases != null)
                    {
                        using (StreamWriter writer = new StreamWriter(".\\SavedItems.txt", false, System.Text.Encoding.UTF8))
                        {
                            for (int ii = 0; ii < aliases.Length; ii++)
                            {
                                // save an absolute node id as the alias if alias not available.
                                if (String.IsNullOrEmpty(aliases[ii]))
                                {
                                    string namespaceUri = m_session.NamespaceUris.GetString(nodeIds[ii].NamespaceIndex);
                                    aliases[ii] = new ExpandedNodeId(nodeIds[ii], namespaceUri).ToString();
                                }

                                // write the alias.
                                if (!String.IsNullOrEmpty(aliases[ii]))
                                {
                                    writer.WriteLine(aliases[ii]);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
        
    }
}
