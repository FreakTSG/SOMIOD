using System;
using System.Text;
using System.Threading;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Exceptions;

namespace SOMIOD_IS.SqlHelpers
{
    public static class BrokerHelper
    {
        private static readonly string Guid = System.Guid.NewGuid().ToString();
        // Method to publish a notification message to an MQTT broker
        public static void FireNotification(string endPoint, string topic, Notification notification)
        {
            try
            {
                // Initialize MQTT client with the provided endpoint
                var mClient = new MqttClient(endPoint);

                // Connect to the MQTT broker
                mClient.Connect(Guid);

                // Check if the connection was successful
                if (!mClient.IsConnected)
                    throw new BrokerException("Couldn't connect to message broker endpoint '" + endPoint + "'");

                // Serialize the notification object to XML and publish it to the specified topic
                mClient.Publish(topic, Encoding.UTF8.GetBytes(XmlHelper.Serialize(notification).OuterXml));

                if (mClient.IsConnected)
                {
                    Thread.Sleep(1000);
                    mClient.Disconnect();
                }
            }
            catch (Exception e)
            {
                if (e is BrokerException) throw e;
                else if (e is MqttConnectionException)
                    throw new BrokerException("Couldn't connect to message broker endpoint '" + endPoint + "'");
                else
                    throw new BrokerException(e.Message);
            }
        }
    }
}
