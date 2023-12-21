using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using App1.Models;
using static App1.Properties.Settings;
using Application = App1.Models.Application;
using RestSharp;
using System.Text.Json;
using App1.Properties;
using System.IO;
using System.Xml.Serialization;
using Container = App1.Models.Container;

namespace App1
{
    public partial class Form1 : Form
    {
        private static readonly string BrokerIp = Default.brokerIpAddress;
        private static readonly string[] Topic = { Default.Topic };
        private static readonly string AppName = Default.ApplicationName;
        private static readonly string ApiBaseUrl = Default.ApiBaseUrl;
        private static readonly string ContainerName = Default.ContainerName;
        private static readonly string SubscriptionName = Default.SubscriptionName;
        private static readonly string Event = Default.Event;
        private static readonly string EndPoint = Default.EndPoint;

        private MqttClient _mClient;
        private readonly RestClient _restClient = new RestClient(ApiBaseUrl);

        private bool _openGarage;
        public Form1()
        {
            InitializeComponent();
        }


        #region Helpers

        
        private bool CheckEntityAlreadyExists(RestResponse response)
        {
            if (response.StatusCode == (HttpStatusCode)422) { 
                return true;
            }
            return false;
        }

        private void UpdateDoorState()
        {
            if (_openGarage)
            {
                _openGarage = true;
                PortaoImage.Image = Resources.aberto;
                return;
            }

            _openGarage = false;
            PortaoImage.Image = Resources.fechado;
        }

        private void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs args)
        {
            string message = Encoding.UTF8.GetString(args.Message);
            using (TextReader reader = new StringReader(message))
            {
                var not = (Notification)new XmlSerializer(typeof(Notification)).Deserialize(reader);
                if (not.EventType != "CREATE") return;

                _openGarage = not.Content == "Abrir";
                UpdateDoorState();
            }

        }
        #endregion

        #region Message Broker

        private void ConnectToBroker()
        {
            _mClient = new MqttClient(BrokerIp);
            _mClient.Connect(Guid.NewGuid().ToString());

            if (!_mClient.IsConnected)
            {
                MessageBox.Show("Could not connect to message broker", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SubscribeToTopics()
        {
            _mClient.MqttMsgPublishReceived += client_MqttMsgPublishReceived;
            byte[] qosLevels = { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE };
            _mClient.Subscribe(Topic, qosLevels);
        }

        #endregion


        #region API Calls
        
        private void CreateApplication(string appName)
        {
            Application app = new Application(appName);

            var request = new RestRequest("api/somiod", Method.Post);
            request.AddObject(app);

            var response = _restClient.Execute(request);

            if (CheckEntityAlreadyExists(response) == true) return;

            if (response.StatusCode == 0)
            {
                MessageBox.Show("Could not connect to the API", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (response.StatusCode != HttpStatusCode.OK)
                MessageBox.Show("An error occurred while creating the application", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }
        
        private void CreateContainer(string containerName, string applicationName) 
        {
            var container = new Container(containerName, applicationName);

            var request = new RestRequest($"api/somiod/{applicationName}", Method.Post);

            request.AddObject(container);

            var response = _restClient.Execute(request);

            if (CheckEntityAlreadyExists(response)) return;

            if(response.StatusCode != 0)
            {
                MessageBox.Show("Could not connect to the API", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (response.StatusCode != HttpStatusCode.OK)
                MessageBox.Show("An error occurred while creating the container", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void CreateSubscription(string subscriptionName, string applicationName, string containerName, string Event, string endPoint)
        {
            var subscription = new Subscription(subscriptionName, containerName, Event, endPoint);

            var request = new RestRequest($"api/somiod/{applicationName}/{containerName}/sub", Method.Post);

            request.AddObject(subscription);

            var response = _restClient.Execute(request);

            if (CheckEntityAlreadyExists(response)) return;

            if (response.StatusCode != 0)
            {
                MessageBox.Show("Could not connect to the API", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (response.StatusCode != HttpStatusCode.OK)
                MessageBox.Show("An error occurred while creating the subscription", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void Form1Initialize(object sender, EventArgs e)
        {
            ConnectToBroker();
            SubscribeToTopics();
            CreateApplication(AppName);
            CreateContainer(ContainerName,AppName);
            CreateSubscription(SubscriptionName,AppName,ContainerName, Event, EndPoint);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_mClient.IsConnected)
            {
                _mClient.Unsubscribe(Topic);
                _mClient.Disconnect();
            }
        }
        #endregion

    }
}
