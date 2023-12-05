using System;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Exceptions;

namespace SOMIOD_IS.SqlHelpers
{
    public static class BrokerHelper
    {
        public static void FireNotification(string endPoint, string topic, Notification notification)
        {
            MqttClient mClient = null;

            try
            {
                mClient = new MqttClient(endPoint);
                var clientId = System.Guid.NewGuid().ToString();
                mClient.Connect(clientId);

                if (!mClient.IsConnected)
                    throw new BrokerException("Couldn't connect to message broker endpoint '" + endPoint + "'");

                mClient.Publish(topic, Encoding.UTF8.GetBytes(XmlHelper.Serialize(notification).OuterXml));

                // Optionally add a callback mechanism or async handling here
            }
            catch (MqttConnectionException)
            {
                throw new BrokerException("Couldn't connect to message broker endpoint '" + endPoint + "'");
            }
            catch (Exception e)
            {
                throw new BrokerException("Error while sending notification: " + e.Message);
            }
            finally
            {
                if (mClient != null && mClient.IsConnected)
                {
                    mClient.Disconnect();
                }
            }
        }
    }
}