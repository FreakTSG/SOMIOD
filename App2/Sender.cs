using App2.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using static App2.Properties.Settings;

namespace App2
{
    public partial class Sender : Form
    {
        #region Constants
        private static readonly string ApplicationName = Default.ApplicationName;
        private static readonly string ContainerName = Default.ContainerName;
        private static readonly string DataName = Default.DataName;
        private static readonly string ApiBaseUrl = Default.ApiBaseUrl;
        #endregion

        //Connection
        private readonly RestClient _restClient = new RestClient(ApiBaseUrl);


        public Sender()
        {
            InitializeComponent();
        }

        #region Helper
        private string DeserializeError(RestResponse response)
        {
            var error = JsonSerializer.Deserialize<Error>(response.Content ?? string.Empty);
            return error?.Message;
        }

        private bool CheckEntityAlreadyExists(RestResponse response)
        {
            if (response.StatusCode == (HttpStatusCode)422)
                if (DeserializeError(response).Contains("already exists"))
                    return true;

            return false;
        }
        #endregion

        #region API Calls

        private void CreateData(string dataName, string applicationName, string containerName, string content)
        {

            string uniqueName = $"{dataName}_{DateTime.Now:yyyyMMddHHmmssfff}";
            var data = new Data(dataName,content);

            var request = new RestRequest($"api/somiod/{applicationName}/{containerName}/data", Method.Post);

            request.AddObject(data);

            var bodyParameter = request.Parameters.FirstOrDefault(p => p.Type == ParameterType.RequestBody);

            var response = _restClient.Execute(request);

            if (CheckEntityAlreadyExists(response)) return;

            if (response.StatusCode != HttpStatusCode.OK)
            {
                if (response.StatusCode == 0)
                {
                    MessageBox.Show("Could not connect to the API", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    Console.WriteLine(response.Content);
                    MessageBox.Show($"An error occurred while creating the data: {response.StatusCode}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (response.StatusCode != HttpStatusCode.OK)
                MessageBox.Show("An error occurred while creating the data", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        #endregion

        private void btn_Abrir_Click(object sender, EventArgs e)
        {
            CreateData(DataName, ApplicationName, ContainerName, "Abrir");
        }

        private void btn_Fechar_Click(object sender, EventArgs e)
        {
            CreateData(DataName, ApplicationName, ContainerName, "Fechar");
        }
    }
}
