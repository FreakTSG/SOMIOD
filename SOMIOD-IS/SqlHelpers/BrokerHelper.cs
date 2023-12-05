using System;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Exceptions;

namespace SOMIOD_IS.SqlHelpers
{
    public static class BrokerHelper
    {
        // Method to publish a notification message to an MQTT broker
        public static void FireNotification(string endPoint, string topic, Notification notification)
        {
            MqttClient mClient = null;

            try
            {
                // Initialize MQTT client with the provided endpoint
                mClient = new MqttClient(endPoint);

                // Generate a unique client ID for this connection
                var clientId = System.Guid.NewGuid().ToString();

                // Connect to the MQTT broker
                mClient.Connect(clientId);

                // Check if the connection was successful
                if (!mClient.IsConnected)
                    throw new BrokerException("Couldn't connect to message broker endpoint '" + endPoint + "'");

                // Serialize the notification object to XML and publish it to the specified topic
                mClient.Publish(topic, Encoding.UTF8.GetBytes(XmlHelper.Serialize(notification).OuterXml));

                // Callback mechanism
                // For instance, using the Publish event of the client:
                mClient.MqttMsgPublished += (sender, e) =>
                {
                    if (e.IsPublished)
                    {
                        // Action to take if the message was successfully published
                        Console.WriteLine($"Message with ID {e.MessageId} was successfully published.");
                    }
                    else
                    {
                        // Action to take if the message failed to publish
                        Console.WriteLine($"Failed to publish message with ID {e.MessageId}.");
                    }
                };
            }
            catch (MqttConnectionException)
            {
                // Specific exception for MQTT connection issues
                throw new BrokerException("Couldn't connect to message broker endpoint '" + endPoint + "'");
            }
            catch (Exception e)
            {
                // General exception handling
                throw new BrokerException("Error while sending notification: " + e.Message);
            }
            finally
            {
                // Ensure the client is properly disconnected and resources are released
                if (mClient != null && mClient.IsConnected)
                {
                    mClient.Disconnect();
                }
            }
        }
    }
}
