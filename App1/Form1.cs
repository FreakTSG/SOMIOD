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

namespace App1
{
    public partial class Form1 : Form
    {
        private static readonly string BrokerIp = Default.brokerIpAddress;
        private static readonly string[] Topic = {Default.Topic};
        private static readonly HttpStatusCode CustomApiError = (HttpStatusCode)Default.CustomApiError;
        private static readonly string AppName = Default.ApplicationName;
        private static readonly string ApiBaseUrl = Default.ApiBaseUrl;

        private MqttClient _mClient;
        private readonly RestClient _restClient = new RestClient(ApiBaseUrl);

        private bool _openGarage;
        public Form1()
        {
            InitializeComponent();
        }

        #region Helpers

        private string DeserializeError(RestResponse response)
        {
            var error = JsonSerializer.Deserialize<Error>(response.Content ?? string.Empty);
            return error?.Message;
        }
        private bool CheckEntityAlreadyExists(RestResponse response)
        {
            if (response.StatusCode == CustomApiError)
                if (DeserializeError(response).Contains("already exists"))
                    return true;

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

                _openGarage = not.Content == "ON";
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

            if (CheckEntityAlreadyExists(response))
                return;

            if (response.StatusCode == 0)
            {
                MessageBox.Show("Could not connect to the API", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (response.StatusCode != HttpStatusCode.OK)
                MessageBox.Show("An error occurred while creating the application", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }
        
        #endregion

    }
}
